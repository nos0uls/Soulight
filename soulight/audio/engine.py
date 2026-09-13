# engine.py — Фоновый поток аудио-захвата и FFT.
#
# Захватывает системный аудио-вход (или микрофон) через soundcard,
# вычисляет FFT, вызывает mode-функцию и эмитит RGB массив.
#
# Важно: soundcard на Windows использует WASAPI. WASAPI требует инициализации
# COM в потоке (CoInitializeEx). Без этого открытие loopback-устройства из
# worker thread может дать PaErrorCode -9999 / WdmSyncIoctl.
# Возможно протестировать полностью только с реальным LED-контроллером.

import ctypes
import shutil
import subprocess
import sys
import threading
import time
from typing import Optional

import numpy as np

from PyQt6.QtCore import QObject, pyqtSignal

from soulight.audio.modes import AUDIO_MODES

try:
    import soundcard as sc
    SOUNDCARD_AVAILABLE = True
except ImportError:
    SOUNDCARD_AVAILABLE = False


# Специальные id источников (резолвятся в AudioEngine):
DEFAULT_SOURCE = "__default__"   # системный звук — монитор выхода по умолчанию
MIC_SOURCE = "__mic__"           # микрофон по умолчанию


def list_capture_devices():
    """
    Список источников захвата для UI: [(id, label, is_loopback), ...].

    is_loopback=True — монитор устройства ВЫВОДА (системный звук этого
    выхода): на Windows это WASAPI loopback, на Linux — PulseAudio/PipeWire
    monitor source. is_loopback=False — обычный микрофон/вход.
    Первый элемент — DEFAULT_SOURCE = звук системного выхода по умолчанию:
    для музыкальной реакции это почти всегда то, что нужно (микрофон
    в качестве дефолта почти гарантированно даёт тишину на ленте).
    """
    devices = [
        (DEFAULT_SOURCE, "System audio (default output)", True),
        (MIC_SOURCE, "Default Microphone", False),
    ]
    if not SOUNDCARD_AVAILABLE:
        return devices
    try:
        for mic in sc.all_microphones(include_loopback=True):
            is_lb = bool(getattr(mic, "isloopback", False))
            kind = "Output" if is_lb else "Mic"
            devices.append((mic.id, f"{kind}: {mic.name}", is_lb))
    except Exception:
        pass
    # Loopback-устройства первыми (пользователь обычно хочет звук с выхода)
    head, tail = devices[:2], devices[2:]
    tail.sort(key=lambda d: (not d[2], d[1]))
    return head + tail


def _default_output_monitor():
    """
    Loopback-монитор устройства вывода по умолчанию.
    Windows: get_microphone(speaker.id, include_loopback=True).
    Linux/Pulse: monitor source имеет id "<sink>.monitor" — id не совпадает,
    ищем по нему или по имени. None, если монитор не найден.
    """
    try:
        sp = sc.default_speaker()
    except Exception:
        return None
    try:
        mic = sc.get_microphone(sp.id, include_loopback=True)
        if getattr(mic, "isloopback", False):
            return mic
    except Exception:
        pass
    try:
        want = f"{sp.id}.monitor"
        for mic in sc.all_microphones(include_loopback=True):
            if getattr(mic, "isloopback", False) and (
                    mic.id == want or sp.name in mic.name):
                return mic
    except Exception:
        pass
    return None


# Windows-only: инициализация COM в аудио-потоке для WASAPI.
# Это исправляет -9999 / WdmSyncIoctl при открытии loopback из потока.
def _init_com_for_thread():
    if sys.platform == "win32":
        try:
            # 0 = COINIT_MULTITHREADED; WASAPI/loopback обычно работает с MTA.
            ctypes.windll.ole32.CoInitializeEx(None, 0)
            return True
        except Exception:
            return False
    return False


def _uninit_com_for_thread():
    if sys.platform == "win32":
        try:
            ctypes.windll.ole32.CoUninitialize()
        except Exception:
            pass


class AudioEngine(QObject):
    """
    Аудио-движок: захват микрофона/loopback, FFT анализ, mapping на LED.
    Живёт в отдельном QThread (а захват выполняется в собственном потоке
    внутри soundcard, поэтому здесь инициализируем COM в начале _run_loop).
    """

    frame_ready = pyqtSignal(list)
    error_occurred = pyqtSignal(str)
    status_changed = pyqtSignal(str)
    # Уровень входного сигнала 0..1 — индикатор «источник слышит звук».
    level_changed = pyqtSignal(float)

    def __init__(
        self,
        led_count: int = 75,
        sample_rate: int = 44100,
        block_size: int = 1024,
        fps: float = 20.0,
        parent=None,
    ):
        super().__init__(parent)
        self._led_count = max(1, int(led_count))
        self._sample_rate = int(sample_rate)
        self._block_size = int(block_size)
        self._fps = max(1.0, min(60.0, float(fps)))
        self._interval = 1.0 / self._fps

        self._mode_name: Optional[str] = None
        self._mode_params: dict = {
            "sensitivity": 1.5,
            "gain": 1.0,
            "color_shift": 0.0,
        }
        self._device_id: Optional[str] = None
        self._layout_leds: Optional[list] = None

        self._running = False
        self._thread: Optional[threading.Thread] = None
        self._stop_event = threading.Event()

        # FFT bins + кэшированное окно (np.hanning каждый кадр — лишняя аллокация)
        self._freq_bins = np.fft.rfftfreq(self._block_size, 1.0 / self._sample_rate)
        self._window = np.hanning(self._block_size).astype(np.float32)

    @property
    def running(self) -> bool:
        return self._running

    @property
    def available(self) -> bool:
        return SOUNDCARD_AVAILABLE

    def set_mode(self, name: str, params: Optional[dict] = None):
        if name not in AUDIO_MODES:
            raise ValueError(f"Unknown audio mode: {name}")
        self._mode_name = name
        if params is not None:
            self._mode_params = dict(params)
        # Сглаженная история старого режима не должна перетекать в новый.
        self._mode_params.pop("history", None)

    def set_sensitivity(self, value: float):
        self._mode_params["sensitivity"] = max(0.1, min(5.0, float(value)))

    def set_gain(self, value: float):
        """Software gain для усиления яркости аудио-эффекта."""
        self._mode_params["gain"] = max(0.0, min(3.0, float(value)))

    def set_color_shift(self, value: float):
        """Смещение базового Hue режимов (0.0..1.0)."""
        self._mode_params["color_shift"] = value % 1.0

    def set_fps(self, fps: float):
        self._fps = max(1.0, min(60.0, float(fps)))
        self._interval = 1.0 / self._fps

    def set_layout(self, layout_leds: Optional[list]):
        self._layout_leds = layout_leds

    def set_led_count(self, n: int):
        """Меняет число LED на лету (Full LED toggle)."""
        self._led_count = max(1, int(n))

    def set_output_config(self, led_count: int, layout_leds: Optional[list]):
        """Атомарная смена led_count + маски — иначе кадр между двумя
        отдельными сеттерами рисует промежуточное состояние (flash)."""
        self._led_count = max(1, int(led_count))
        self._layout_leds = layout_leds

    def _resolve_device(self):
        """device_id → soundcard-устройство захвата."""
        dev = self._device_id
        if dev in (None, DEFAULT_SOURCE):
            mic = _default_output_monitor()
            if mic is not None:
                return mic
            self.status_changed.emit("No loopback — using microphone")
            return sc.default_microphone()
        if dev == MIC_SOURCE:
            return sc.default_microphone()
        return sc.get_microphone(dev, include_loopback=True)

    def start(self, mode_name: str, device_id: Optional[str] = None, params: Optional[dict] = None):
        """
        Запускает захват. device_id — id устройства из list_capture_devices():
        None/DEFAULT_SOURCE = монитор системного выхода (звук колонок),
        MIC_SOURCE = микрофон по умолчанию, иначе конкретное устройство.
        """
        if not SOUNDCARD_AVAILABLE:
            self.error_occurred.emit("soundcard не установлен. Установите: pip install soundcard")
            return
        if self._running:
            self.stop()

        self.set_mode(mode_name, params)
        self._device_id = device_id
        self._running = True
        self._stop_event.clear()

        self._thread = threading.Thread(target=self._run_loop, daemon=True)
        self._thread.start()
        self.status_changed.emit("Running")

    def stop(self):
        self._running = False
        self._stop_event.set()
        if self._thread is not None:
            self._thread.join(timeout=2.0)
            self._thread = None
        self.status_changed.emit("Stopped")

    def _compute_fft(self, chunk: np.ndarray) -> tuple[np.ndarray, np.ndarray]:
        """Возвращает (magnitudes, freq_bins) — bins пересчитываются,
        если реальный чанк короче block_size (нестандартный кадр бэкенда),
        иначе modes-функции с маской по freq_bins падают на длинах."""
        window = self._window
        freq_bins = self._freq_bins
        if len(chunk) != len(window):
            window = np.hanning(len(chunk)).astype(np.float32)
            freq_bins = np.fft.rfftfreq(len(chunk), 1.0 / self._sample_rate)
        spectrum = np.fft.rfft(chunk * window)
        return np.abs(spectrum), freq_bins

    def _process_chunk(self, chunk: np.ndarray):
        """Общий пайплайн одного аудио-блока: level → FFT → mode → маска."""
        # Уровень входа до обработки — диагностика «молчит ли источник»
        # и индикатор в UI. RMS ~0.05-0.15 = обычная громкость выхода.
        if chunk.size:
            rms = float(np.sqrt(np.mean(chunk.astype(np.float32) ** 2)))
            self.level_changed.emit(min(1.0, rms * 8.0))

        if self._mode_name is None:
            return
        mode_fn = AUDIO_MODES.get(self._mode_name)
        if mode_fn is None:
            return
        mags, freq_bins = self._compute_fft(chunk)
        colors = mode_fn(
            magnitudes=mags,
            freq_bins=freq_bins,
            led_count=self._led_count,
            params=self._mode_params,
        )
        if self._layout_leds:
            for led in self._layout_leds:
                if not led.enabled and led.logical_index < len(colors):
                    colors[led.logical_index] = (0, 0, 0)
        self.frame_ready.emit(colors)

    def _throttle(self, loop_start: float):
        """Точный sleep с учётом времени обработки кадра."""
        elapsed = time.perf_counter() - loop_start
        sleep_time = max(0.0, self._interval - elapsed)
        if sleep_time > 0:
            self._stop_event.wait(sleep_time)

    def _parec_source(self):
        """device_id → (pulse source name, label для статуса)."""
        dev = self._device_id
        if dev in (None, DEFAULT_SOURCE):
            sink = subprocess.check_output(
                ["pactl", "get-default-sink"], text=True, timeout=3).strip()
            return f"{sink}.monitor", f"System audio ({sink})"
        if dev == MIC_SOURCE:
            return "@DEFAULT_SOURCE@", "Default Microphone"
        # На Linux id из list_capture_devices — это уже имя pulse-источника.
        return dev, dev

    def _run_parec(self) -> bool:
        """
        Linux fast-path: захват через parec (pulseaudio-utils) с
        --latency-msec=5. Замерено: ~8-13мс от звука до данных против
        ~55-80мс у soundcard/PulseAudio-буфера по умолчанию.
        Возвращает False только если parec вообще не смог стартовать —
        тогда вызывающий код идёт в soundcard-fallback.
        """
        if not sys.platform.startswith("linux") or shutil.which("parec") is None:
            return False
        try:
            src, label = self._parec_source()
        except Exception:
            return False
        try:
            proc = subprocess.Popen(
                ["parec", "-d", src, "--format=float32le",
                 "--rate", str(self._sample_rate), "--channels", "1",
                 "--latency-msec=5"],
                stdout=subprocess.PIPE, stderr=subprocess.DEVNULL,
                bufsize=0,
            )
        except Exception:
            return False

        self.status_changed.emit(f"Capturing: {label}")
        need = self._block_size * 4  # float32 mono
        buf = b""
        frames = 0
        try:
            while not self._stop_event.is_set():
                loop_start = time.perf_counter()
                data = proc.stdout.read(need - len(buf))
                if data:
                    buf += data
                elif proc.poll() is not None:
                    if frames == 0:
                        return False  # источник не открылся — fallback
                    raise RuntimeError("parec exited mid-run")
                if len(buf) < need:
                    continue
                chunk = np.frombuffer(buf[:need], dtype=np.float32)
                buf = buf[need:]
                frames += 1
                self._process_chunk(chunk)
                self._throttle(loop_start)
        finally:
            proc.terminate()
            try:
                proc.wait(timeout=1.0)
            except Exception:
                proc.kill()
        return True

    def _run_loop(self):
        # ВАЖНО: soundcard на Windows открывает WASAPI-устройства.
        # WASAPI требует COM в потоке. Без этой инициализации loopback
        # может упасть с PaErrorCode -9999 / WdmSyncIoctl.
        # Проверено на схожем проекте (NoVoice): после CoInitializeEx
        # WASAPI открывается корректно в worker thread.
        # TODO: протестировать с реальным loopback-устройством и LED.
        com_owned = _init_com_for_thread()
        self.status_changed.emit("Capturing...")
        try:
            if self._run_parec():
                return
            mic = self._resolve_device()
            self.status_changed.emit(f"Capturing: {mic.name}")

            with mic.recorder(samplerate=self._sample_rate, channels=1, blocksize=self._block_size) as recorder:
                while not self._stop_event.is_set():
                    loop_start = time.perf_counter()
                    chunk = recorder.record(numframes=self._block_size)
                    self._process_chunk(chunk.flatten())
                    self._throttle(loop_start)

        except Exception as e:
            self.error_occurred.emit(f"Audio error: {e}")
            self.status_changed.emit("Error")
            self._running = False
        finally:
            if com_owned:
                _uninit_com_for_thread()
