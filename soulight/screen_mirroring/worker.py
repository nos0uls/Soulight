# worker.py — Background thread для Screen Mirroring.
#
# Этот модуль выносит тяжёлую работу (capture + sampling) из UI thread.
# Без этого PyQt6 UI замирает на время каждого кадра (~15-30ms),
# что делает приложение неотзывчивым при перетаскивании окон.
#
# Архитектура:
# - MirrorWorker живёт в отдельном QThread
# - UI отправляет сигнал request_frame → worker делает capture+sample
# - Worker отправляет сигнал frame_ready → UI получает готовые цвета
# - Таймер живёт в UI thread и управляет частотой запросов

from typing import List, Tuple

from PyQt6.QtCore import QObject, pyqtSignal, pyqtSlot

from soulight.screen_mirroring.engine import ScreenMirrorEngine


class MirrorWorker(QObject):
    """
    Background worker для screen mirroring.
    Выполняет capture + sampling в отдельном потоке.
    UI thread только получает готовые RGB цвета через сигнал.
    """

    # Сигнал: кадр готов, несёт список RGB-кортежей для per-LED отправки
    frame_ready = pyqtSignal(list)
    # Сигнал: произошла ошибка при обработке кадра
    error_occurred = pyqtSignal(str)

    def __init__(self, parent=None):
        super().__init__(parent)
        self._engine: ScreenMirrorEngine = None

    @property
    def engine(self) -> ScreenMirrorEngine:
        return self._engine

    @engine.setter
    def engine(self, value: ScreenMirrorEngine):
        self._engine = value

    @pyqtSlot()
    def process_frame(self):
        """
        Вызывается по сигналу из UI thread.
        Делает capture + sample в background thread и эмитит результат.
        """
        if self._engine is None:
            return
        try:
            result = self._engine.process_next_frame()
            # Отправляем physical_colors в UI thread
            self.frame_ready.emit(result.sampled.physical_colors)
        except Exception as e:
            self.error_occurred.emit(str(e))
