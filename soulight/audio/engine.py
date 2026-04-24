# engine.py — Фоновый поток аудио-захвата и FFT.
#
# Захватывает системный аудио-вход через sounddevice,
# вычисляет FFT, вызывает mode-функцию и эмитит RGB массив.

import threading
import time
from typing import Callable, Optional

import numpy as np

from PyQt6.QtCore import QObject, pyqtSignal, pyqtSlot

from soulight.audio.modes import AUDIO_MODES

# Graceful fallback если sounddevice не установлен
try:
    import sounddevice as sd
    SOUNDDEVICE_AVAILABLE = True
except ImportError:
    SOUNDDEVICE_AVAILABLE = False


class AudioEngine(QObject):
    """
    Аудио-движок: захват микрофона/loopback, FFT анализ, mapping на LED.

    Живёт в отдельном QThread.
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
        self._fps = max(1.0, float(fps))
        self._interval = 1.0 / self._fps

        self._mode_name: Optional[str] = None
        self._mode_params: dict = {"sensitivity": 1.5}
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
        return SOUNDDEVICE_AVAILABLE

    def set_mode(self, name: str, params: Optional[dict] = None):
        if name not in AUDIO_MODES:
            raise ValueError(f"Unknown audio mode: {name}")
        self._mode_name = name
        if params is not None:
            self._mode_params = dict(params)

    def set_sensitivity(self, value: float):
        self._mode_params["sensitivity"] = max(0.1, min(5.0, float(value)))

    def set_fps(self, fps: float):
        self._fps = max(1.0, min(60.0, float(fps)))
        self._interval = 1.0 / self._fps

    def start(self, mode_name: str, params: Optional[dict] = None):
        if not SOUNDDEVICE_AVAILABLE:
            self.error_occurred.emit("sounddevice not installed. Install: pip install sounddevice")
            return
        if self._running:
            self.stop()
        self.set_mode(mode_name, params)
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

    def _capture_chunk(self) -> np.ndarray:
        """Захватывает один аудио-блок (mono)."""
        chunk = sd.rec(
            frames=self._block_size,
            samplerate=self._sample_rate,
            channels=1,
            dtype=np.float32,
            blocking=True,
        )
        return chunk.flatten()

    def _compute_fft(self, chunk: np.ndarray) -> np.ndarray:
        """Возвращает magnitudes FFT (0..Nyquist)."""
        window = np.hanning(len(chunk))
        spectrum = np.fft.rfft(chunk * window)
        return np.abs(spectrum)

    def _run_loop(self):
        mode_fn = None
        self.status_changed.emit("Capturing...")
        while not self._stop_event.is_set():
            try:
                if self._mode_name is not None:
                    mode_fn = AUDIO_MODES.get(self._mode_name)

                if mode_fn is not None:
                    chunk = self._capture_chunk()
                    mags = self._compute_fft(chunk)
                    colors = mode_fn(
                        magnitudes=mags,
                        freq_bins=self._freq_bins,
                        led_count=self._led_count,
                        params=self._mode_params,
                    )
                    self.frame_ready.emit(colors)

                self._stop_event.wait(self._interval)
            except Exception as e:
                self.error_occurred.emit(f"Audio error: {e}")
                self._stop_event.wait(0.5)
