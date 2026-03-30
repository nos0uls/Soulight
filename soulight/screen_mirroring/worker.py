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

    def __init__(
        self,
        config,
        monitor_index,
        edge_fraction,
        smoothing_factor,
        saturation_boost,
        parent=None,
    ):
        super().__init__(parent)
        self._config = config
        self._monitor_index = int(monitor_index)
        self._edge_fraction = float(edge_fraction)
        self._smoothing_factor = float(smoothing_factor)
        self._saturation_boost = float(saturation_boost)
        self._engine: ScreenMirrorEngine = None

    @pyqtSlot()
    def shutdown(self):
        """Закрывает engine и его ресурсы в контексте worker thread."""
        if self._engine is not None:
            self._engine.close()
            self._engine = None

    @pyqtSlot()
    def process_frame(self):
        """
        Вызывается по сигналу из UI thread.
        Делает capture + sample в background thread и эмитит результат.
        """
        try:
            if self._engine is None:
                # Engine создаётся лениво ПРЯМО в worker thread.
                # Это важно для Windows backend mss: capture object должен жить
                # в том же потоке, где потом используется.
                self._engine = ScreenMirrorEngine(
                    config=self._config,
                    monitor_index=self._monitor_index,
                    edge_fraction=self._edge_fraction,
                    smoothing_factor=self._smoothing_factor,
                    saturation_boost=self._saturation_boost,
                )
                self._engine.rebuild_layout()

            result = self._engine.process_next_frame()
            # Отправляем physical_colors в UI thread
            self.frame_ready.emit(result.sampled.physical_colors)
        except Exception as e:
            self.error_occurred.emit(str(e))
