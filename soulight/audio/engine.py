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
        self._use_loopback = False
        self._layout_leds: Optional[list] = None

        self._running = False
        self._thread: Optional[threading.Thread] = None
        self._stop_event = threading.Event()

        # FFT bins
        self._freq_bins = np.fft.rfftfreq(self._block_size, 1.0 / self._sample_rate)

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

    def start(self, mode_name: str, use_loopback: bool = False, params: Optional[dict] = None):
        if not SOUNDCARD_AVAILABLE:
            self.error_occurred.emit("soundcard не установлен. Установите: pip install soundcard")
            return
        if self._running:
            self.stop()

        self.set_mode(mode_name, params)
        self._use_loopback = use_loopback
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

    def _compute_fft(self, chunk: np.ndarray) -> np.ndarray:
        """Возвращает magnitudes FFT (0..Nyquist)."""
        window = np.hanning(len(chunk))
        spectrum = np.fft.rfft(chunk * window)
        return np.abs(spectrum)

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
            if self._use_loopback:
                # Loopback системного звука (с динамиков)
                spk = sc.default_speaker()
                mic = sc.get_microphone(spk.id, include_loopback=True)
            else:
                # Обычный микрофон
                mic = sc.default_microphone()

            with mic.recorder(samplerate=self._sample_rate, channels=1, blocksize=self._block_size) as recorder:
                while not self._stop_event.is_set():
                    loop_start = time.perf_counter()

                    # Читаем аудио
                    chunk = recorder.record(numframes=self._block_size)
                    chunk = chunk.flatten()

                    if self._mode_name is not None:
                        mode_fn = AUDIO_MODES.get(self._mode_name)
                        if mode_fn is not None:
                            mags = self._compute_fft(chunk)
                            colors = mode_fn(
                                magnitudes=mags,
                                freq_bins=self._freq_bins,
                                led_count=self._led_count,
                                params=self._mode_params,
                            )

                            if self._layout_leds:
                                for led in self._layout_leds:
                                    if not led.enabled and led.logical_index < len(colors):
                                        colors[led.logical_index] = (0, 0, 0)

                            self.frame_ready.emit(colors)

                    # Точный sleep с учётом времени обработки
                    elapsed = time.perf_counter() - loop_start
                    sleep_time = max(0.0, self._interval - elapsed)
                    if sleep_time > 0:
                        self._stop_event.wait(sleep_time)

        except Exception as e:
            self.error_occurred.emit(f"Audio error: {e}")
            self.status_changed.emit("Error")
            self._running = False
        finally:
            if com_owned:
                _uninit_com_for_thread()
