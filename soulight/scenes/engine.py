# engine.py — Фоновый движок сценических режимов.
#
# Работает в отдельном потоке (QThread), генерирует кадры паттерна
# и отправляет их через callback в LED driver.

import threading
import time
from typing import Callable, List, Optional, Tuple

from PyQt6.QtCore import QObject, pyqtSignal, pyqtSlot

from soulight.scenes.patterns import PATTERNS


class SceneEngine(QObject):
    """
    Движок сценических паттернов.

    Живёт в отдельном QThread, генерирует RGB-кадры и эмитит их
    через Qt-сигналы для thread-safe передачи в UI/driver.
    """

    # Сигнал: список RGB-кортежей готов для отправки
    frame_ready = pyqtSignal(list)
    # Сигнал: ошибка в потоке
    error_occurred = pyqtSignal(str)

    def __init__(
        self,
        led_count: int = 75,
        fps: float = 20.0,
        parent=None,
    ):
        super().__init__(parent)
        self._led_count = max(1, int(led_count))
        self._fps = max(1.0, float(fps))
        self._interval = 1.0 / self._fps
        self._pattern_name: Optional[str] = None
        self._pattern_params: dict = {"speed": 1.0}
        self._frame_index = 0
        self._running = False
        self._thread: Optional[threading.Thread] = None
        self._stop_event = threading.Event()
        self._layout_leds: Optional[list] = None

    @property
    def running(self) -> bool:
        return self._running

    @property
    def pattern(self) -> Optional[str]:
        return self._pattern_name

    def set_pattern(self, name: str, params: Optional[dict] = None):
        """Меняет активный паттерн на лету."""
        if name not in PATTERNS:
            raise ValueError(f"Unknown pattern: {name}")
        self._pattern_name = name
        if params is not None:
            self._pattern_params = dict(params)
        self._frame_index = 0

    def set_layout(self, layout_leds: Optional[list]):
        """Устанавливает маску layout_leds (LayoutLed) для гашения выключенных LED."""
        self._layout_leds = layout_leds

    def set_speed(self, speed: float):
        """Меняет параметр speed паттерна (0.25..4.0)."""
        self._pattern_params["speed"] = max(0.25, min(4.0, float(speed)))

    def set_fps(self, fps: float):
        """Меняет target FPS (1..60)."""
        self._fps = max(1.0, min(60.0, float(fps)))
        self._interval = 1.0 / self._fps

    def start(self, pattern_name: str, params: Optional[dict] = None):
        """Запускает engine с указанным паттерном."""
        if self._running:
            self.stop()
        self.set_pattern(pattern_name, params)
        self._running = True
        self._stop_event.clear()
        self._thread = threading.Thread(target=self._run_loop, daemon=True)
        self._thread.start()

    def stop(self):
        """Останавливает engine."""
        self._running = False
        self._stop_event.set()
        if self._thread is not None:
            self._thread.join(timeout=2.0)
            self._thread = None

    def _run_loop(self):
        """Основной loop: генерируем кадр, эмитим, спим."""
        pattern_fn = None
        while not self._stop_event.is_set():
            # Если паттерн сменился — обновляем ссылку
            if self._pattern_name is not None:
                pattern_fn = PATTERNS.get(self._pattern_name)

            if pattern_fn is not None:
                try:
                    colors = pattern_fn(
                        self._frame_index,
                        self._led_count,
                        self._pattern_params,
                    )
                    
                    if self._layout_leds:
                        for led in self._layout_leds:
                            if not led.enabled and led.logical_index < len(colors):
                                colors[led.logical_index] = (0, 0, 0)

                    self.frame_ready.emit(colors)
                    self._frame_index += 1
                except Exception as e:
                    self.error_occurred.emit(f"Pattern error: {e}")
                    time.sleep(0.5)
                    continue

            # Точный sleep с учётом времени генерации
            self._stop_event.wait(self._interval)
