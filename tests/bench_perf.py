# bench_perf.py — Синтетический бенчмарк всех режимов Soulight.
#
# Запуск:  python tests/bench_perf.py [--quick]
#
# Измеряет стоимость каждого блока конвейера:
#   1. protocol: сборка wire-пакетов (rgb_transfer / color / brightness)
#   2. bridge: NativeBridge.make_* (то, что зовёт драйвер каждый кадр)
#   3. send-loop: эмуляция _send_loop для solid/per-LED (mock serial)
#   4. scenes: все паттерны, µs/кадр при 75 LED
#   5. audio: FFT + все режимы, µs/кадр
#   6. sampler: sample_frame по синтетическому edge-кадру
#   7. capture: mss full-screen vs edge strips (реальный X11, если DISPLAY)
#
# Результат — таблица µs/call и оценка CPU%/сек на типовых FPS.

import os
import sys
import time
import statistics

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

import numpy as np

NUM_LEDS = 75


def bench(fn, iters=200, warmup=5):
    """Возвращает медианное время одного вызова в микросекундах."""
    for _ in range(warmup):
        fn()
    samples = []
    batch = max(1, iters // 10)
    for _ in range(10):
        t0 = time.perf_counter()
        for _ in range(batch):
            fn()
        samples.append((time.perf_counter() - t0) / batch * 1e6)
    return statistics.median(samples)


def cpu_at(us_per_call, calls_per_sec):
    """Оценка CPU% одного ядра при заданной частоте вызовов."""
    return us_per_call * calls_per_sec / 1e4


def main():
    quick = "--quick" in sys.argv
    it = 100 if quick else 300
    results = []

    def row(name, us, fps=None, note=""):
        results.append((name, us, fps, note))

    print(f"=== Soulight perf bench ({'quick' if quick else 'full'}) ===\n")

    # ---- 1. Protocol layer -------------------------------------------------
    from soulight.protocol import lightprotocol as lp

    colors75 = [(i * 3 % 256, i * 5 % 256, i * 7 % 256) for i in range(NUM_LEDS)]
    row("lp.rgb_transfer (75 LED)", bench(lambda: lp.rgb_transfer(colors75), it), 60)
    row("lp.color (solid cmd)", bench(lambda: lp.color(255, 128, 0), it), 60)
    row("lp.brightness", bench(lambda: lp.brightness(600), it), 60)

    # ---- 2. NativeBridge ---------------------------------------------------
    from soulight.protocol.native_bridge import NativeBridge

    bridge = NativeBridge()
    bridge.init()
    row("bridge.make_color_packet", bench(lambda: bridge.make_color_packet(255, 128, 0), it), 66)
    row("bridge.make_rgb_transfer", bench(lambda: bridge.make_rgb_transfer_packet(colors75), it), 60)

    # ---- 3. Send loop (mock serial) ---------------------------------------
    # Эмулируем стоимость solid-режима: make_color_packet + write каждые 15ms.
    solid_us = bench(lambda: bridge.make_color_packet(200, 100, 50), it)
    row("send-loop solid (pkt/iter)", solid_us, 66, "every 15ms, incl. heartbeat")
    per_led_us = bench(lambda: bridge.make_rgb_transfer_packet(colors75), it)
    row("send-loop per_led (pkt/iter)", per_led_us, 60)

    # ---- 4. Scene patterns -------------------------------------------------
    from soulight.scenes.patterns import PATTERNS

    for name, fn in PATTERNS.items():
        params = {"speed": 1.0}
        row(f"scene:{name}", bench(lambda: fn(120, NUM_LEDS, params), it), 20)

    # ---- 5. Audio modes ----------------------------------------------------
    from soulight.audio.modes import AUDIO_MODES

    block = 1024
    sr = 44100
    freq_bins = np.fft.rfftfreq(block, 1.0 / sr)
    t = np.arange(block) / sr
    chunk = (np.sin(2 * np.pi * 120 * t) * 0.5 + np.sin(2 * np.pi * 3000 * t) * 0.2).astype(np.float32)
    window = np.hanning(block)
    mags = np.abs(np.fft.rfft(chunk * window))

    row("audio:FFT (rfft+abs)", bench(lambda: np.abs(np.fft.rfft(chunk * window)), it), 30)
    row("audio:np.hanning alloc", bench(lambda: np.hanning(block), it), 30, "пересоздаётся каждый кадр")
    for name, fn in AUDIO_MODES.items():
        params = {"sensitivity": 1.5, "gain": 1.0, "color_shift": 0.0}
        row(f"audio:{name}", bench(lambda: fn(mags, freq_bins, NUM_LEDS, params), it), 30)

    # ---- 6. Sampler ---------------------------------------------------------
    from soulight.led_config import LEDConfig
    from soulight.screen_mirroring.layout import build_layout
    from soulight.screen_mirroring.sampler import sample_frame, FrameSmoother
    from soulight.screen_mirroring.screen_capture import CaptureFrame, CaptureRegion

    W, H, DEPTH = 1920, 1080, 86  # 8% edge
    cfg = LEDConfig()
    layout = build_layout(cfg, W, H, 0.08)
    regions = {
        "top": CaptureRegion(0, 0, W, DEPTH, np.random.randint(0, 255, (DEPTH, W, 3), np.uint8)),
        "bottom": CaptureRegion(0, H - DEPTH, W, DEPTH, np.random.randint(0, 255, (DEPTH, W, 3), np.uint8)),
        "left": CaptureRegion(0, 0, DEPTH, H, np.random.randint(0, 255, (H, DEPTH, 3), np.uint8)),
        "right": CaptureRegion(W - DEPTH, 0, DEPTH, H, np.random.randint(0, 255, (H, DEPTH, 3), np.uint8)),
    }
    frame = CaptureFrame(width=W, height=H, edge_regions=regions)
    smoother = FrameSmoother(0.35)
    row("sampler.sample_frame", bench(lambda: sample_frame(frame, layout, smoother, 1.3), it), 60)

    # ---- 7. Real mss capture (если есть X11) --------------------------------
    if os.environ.get("DISPLAY"):
        try:
            import mss
            from soulight.screen_mirroring.screen_capture import ScreenCapturer
            with mss.mss() as sct:
                mon = sct.monitors[1]
                row(
                    "mss full-screen grab",
                    bench(lambda: sct.grab(mon), 20, warmup=2),
                    60,
                    f"{mon['width']}x{mon['height']} (legacy reference)",
                )
            cap = ScreenCapturer(monitor_index=1, prefer_dxcam=False)
            try:
                row(
                    "capture_edges (strips, real)",
                    bench(lambda: cap.capture_edges(DEPTH), 30, warmup=3),
                    60,
                    "новый путь: 4 grab'а + strip RGB",
                )
            finally:
                cap.close()
        except Exception as e:
            row("mss capture", 0, None, f"skipped: {e}")
    else:
        row("mss capture", 0, None, "skipped: no DISPLAY")

    # ---- Отчёт ---------------------------------------------------------------
    print(f"\n{'block':<34} {'µs/call':>10} {'FPS':>5} {'CPU%/core':>10}  note")
    print("-" * 90)
    for name, us, fps, note in results:
        cpu = f"{cpu_at(us, fps):8.1f}%" if fps else "       -"
        fps_s = f"{fps:>5}" if fps else "    -"
        print(f"{name:<34} {us:>10.1f} {fps_s} {cpu:>10}  {note}")


if __name__ == "__main__":
    main()
