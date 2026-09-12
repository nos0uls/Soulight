# headless.py — Запуск Soulight без GUI (автозапуск, systemd, SSH).
#
# Qt не импортируется вообще (кроме QtCore в движках сцен/аудио,
# которому не нужен DISPLAY) — на сервере/при автозапуске работает
# без X-сессии, пока есть доступ к serial-порту.
#
# Примеры:
#   python -m soulight --headless --mode color --color FF3300 --brightness 128
#   python -m soulight --headless --mode scene --pattern fire --fps 25 --speed 1.5
#   python -m soulight --headless --mode audio --audio-mode spectrum --fps 30
#   python -m soulight --headless --mode off
#
# Env: SOULIGHT_PORT, SOULIGHT_PROTOCOL — как в GUI.

import argparse
import signal
import sys
import threading
import time

from soulight.led_config import LEDConfig
from soulight.protocol.serial_driver import LEDDriver
from soulight.scenes.patterns import PATTERNS


def _parse_color(s: str):
    """'#FF3300' | 'FF3300' | '255,51,0' -> (r, g, b)."""
    s = s.strip().lstrip("#")
    if "," in s:
        parts = [int(p) for p in s.split(",")]
        if len(parts) != 3:
            raise ValueError("color must be RRGGBB or r,g,b")
        return tuple(max(0, min(255, p)) for p in parts)
    if len(s) != 6:
        raise ValueError("color must be RRGGBB or r,g,b")
    return (int(s[0:2], 16), int(s[2:4], 16), int(s[4:6], 16))


def build_parser() -> argparse.ArgumentParser:
    p = argparse.ArgumentParser(
        prog="soulight --headless",
        description="Soulight без GUI: solid color / scene / audio режимы.",
    )
    p.add_argument("--headless", action="store_true",
                   help="маркер режима (проставляется __main__)")
    p.add_argument("--mode", choices=["color", "scene", "audio", "off"],
                   default="color", help="режим работы (default: color)")
    p.add_argument("--color", default="FF00FF",
                   help="статичный цвет RRGGBB или r,g,b (default: FF00FF)")
    p.add_argument("--brightness", type=int, default=255,
                   help="яркость 0-255 (default: 255)")
    p.add_argument("--pattern", default="rainbow", choices=sorted(PATTERNS),
                   help="сценический паттерн (default: rainbow)")
    p.add_argument("--speed", type=float, default=1.0,
                   help="скорость паттерна 0.25-4.0 (default: 1.0)")
    p.add_argument("--fps", type=float, default=20.0,
                   help="кадров/с для scene/audio (default: 20)")
    p.add_argument("--audio-mode", default="spectrum",
                   help="аудио-режим (spectrum, electronic, lyricism, pulse, "
                        "wave, bass, disco)")
    p.add_argument("--device", default=None,
                   help="id аудио-устройства (см. --list-devices); "
                        "по умолчанию — микрофон")
    p.add_argument("--list-devices", action="store_true",
                   help="показать доступные аудио-источники и выйти")
    p.add_argument("--port", default=None,
                   help="serial порт (иначе SOULIGHT_PORT/автодетект)")
    p.add_argument("--no-retry", action="store_true",
                   help="не повторять connect при неудаче (default: retry "
                        "каждые 3с — удобно для автозапуска)")
    return p


def _layout_leds(config: LEDConfig):
    """Маска выключенных LED — та же, что GUI строит для audio/scenes."""
    from soulight.screen_mirroring.layout import build_layout
    return build_layout(config, 100, 100, 0.08).leds


def _run_scene(driver, args, config: LEDConfig, stop: threading.Event):
    """Сцены напрямую через pattern-функции — без QObject/Qt."""
    from soulight.scenes.patterns import PATTERNS as _P

    pattern_fn = _P[args.pattern]
    params = {"speed": max(0.25, min(4.0, args.speed))}
    interval = 1.0 / max(1.0, min(60.0, args.fps))
    leds = _layout_leds(config)
    frame = 0
    while not stop.is_set():
        t0 = time.perf_counter()
        colors = pattern_fn(frame, config.total, params)
        for led in leds:
            if not led.enabled and led.logical_index < len(colors):
                colors[led.logical_index] = (0, 0, 0)
        driver.set_per_led_colors(colors)
        frame += 1
        stop.wait(max(0.0, interval - (time.perf_counter() - t0)))


def _run_audio(driver, args, config: LEDConfig, stop: threading.Event):
    """Аудио через AudioEngine (QtCore только, DISPLAY не нужен)."""
    from soulight.audio.engine import AudioEngine, AUDIO_MODES

    if args.audio_mode not in AUDIO_MODES:
        raise SystemExit(
            f"Неизвестный --audio-mode {args.audio_mode!r}. "
            f"Доступно: {', '.join(sorted(AUDIO_MODES))}"
        )
    engine = AudioEngine(led_count=config.total, fps=args.fps)
    engine.set_layout(_layout_leds(config))
    engine.frame_ready.connect(driver.set_per_led_colors)
    engine.error_occurred.connect(lambda m: print(f"[audio] {m}", file=sys.stderr))
    engine.start(args.audio_mode, device_id=args.device)
    try:
        stop.wait()
    finally:
        engine.stop()


def main(argv=None):
    args = build_parser().parse_args(argv)

    if args.list_devices:
        from soulight.audio.engine import list_capture_devices
        for dev_id, label, is_lb in list_capture_devices():
            print(f"{'(default)' if dev_id is None else dev_id}\t{label}")
        return 0

    config = LEDConfig()
    driver = LEDDriver(port=args.port) if args.port else LEDDriver()
    driver.set_brightness(args.brightness)

    stop = threading.Event()

    def _sigint(*_):
        stop.set()
    signal.signal(signal.SIGINT, _sigint)
    signal.signal(signal.SIGTERM, _sigint)

    # Connect с retry — при автозапуске контроллер может ещё не быть
    # проэкспонирован в /dev.
    while not driver.connected:
        if driver.connect():
            break
        if args.no_retry:
            print("[headless] connect failed, exiting (--no-retry)", file=sys.stderr)
            return 1
        print("[headless] connect failed, retry in 3s...", file=sys.stderr)
        if stop.wait(3.0):
            return 130

    try:
        if args.mode == "color":
            r, g, b = _parse_color(args.color)
            driver.set_color(r, g, b)
            print(f"[headless] color=#{args.color} bright={args.brightness} — Ctrl+C to exit")
            stop.wait()
        elif args.mode == "scene":
            print(f"[headless] scene={args.pattern} speed={args.speed} fps={args.fps}")
            _run_scene(driver, args, config, stop)
        elif args.mode == "audio":
            print(f"[headless] audio={args.audio_mode} device={args.device or 'default-mic'} fps={args.fps}")
            _run_audio(driver, args, config, stop)
        else:  # off
            driver.set_color(0, 0, 0)
            driver.switch(False)
            print("[headless] strip off")
            return 0
    finally:
        driver.disconnect()
    return 0
