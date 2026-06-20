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

import threading
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
        prefer_dxcam=True,
        parent=None,
    ):
        super().__init__(parent)
        self._config = config
        self._monitor_index = int(monitor_index)
        self._edge_fraction = float(edge_fraction)
        self._smoothing_factor = float(smoothing_factor)
        self._saturation_boost = float(saturation_boost)
        self._prefer_dxcam = bool(prefer_dxcam)
        self._engine: ScreenMirrorEngine = None
        # Этот счётчик помогает залогировать только первые удачные кадры.
        # Так мы видим, что worker реально продолжает жить после старта.
        self._processed_frames = 0

    # Компактный worker log для дебага thread/lifecycle проблем.
    # Сообщения короткие, чтобы не зашумлять консоль во время normal run.
    def _debug_log(self, stage: str, message: str) -> None:
        print(
            f"[MirrorWorker:{stage}] "
            f"thread={threading.get_ident()} {message}"
        )

    @pyqtSlot()
    def shutdown(self):
        """Закрывает engine и его ресурсы в контексте worker thread."""
        if self._engine is not None:
            self._debug_log("shutdown", f"frames={self._processed_frames}")
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
                self._debug_log(
                    "engine-create",
                    (
                        f"monitor={self._monitor_index} edge={self._edge_fraction:.3f} "
                        f"smooth={self._smoothing_factor:.3f} sat={self._saturation_boost:.3f} "
                        f"dxcam={self._prefer_dxcam}"
                    ),
                )
                try:
                    self._engine = ScreenMirrorEngine(
                        config=self._config,
                        monitor_index=self._monitor_index,
                        edge_fraction=self._edge_fraction,
                        smoothing_factor=self._smoothing_factor,
                        saturation_boost=self._saturation_boost,
                        prefer_dxcam=self._prefer_dxcam,
                    )
                    self._engine.rebuild_layout()
                except Exception as engine_error:
                    self._debug_log(
                        "engine-create-failed",
                        f"{type(engine_error).__name__}: {engine_error}",
                    )
                    raise

            try:
                result = self._engine.process_next_frame()
            except Exception as first_error:
                # Первый кадр на Windows иногда падает из-за гонки старта backend/thread.
                # Делаем один тихий retry с небольшой задержкой, чтобы не ронять mirroring.
                if self._processed_frames == 0:
                    self._debug_log(
                        "first-frame-retry",
                        f"{type(first_error).__name__}: {first_error}",
                    )
                    import time
                    time.sleep(0.1)  # 100ms перед retry
                    result = self._engine.process_next_frame()
                else:
                    raise
            self._processed_frames += 1
            if self._processed_frames <= 3:
                self._debug_log(
                    "frame-ready",
                    (
                        f"count={self._processed_frames} "
                        f"physical_leds={len(result.sampled.physical_colors)}"
                    ),
                )
            # Отправляем physical_colors в UI thread
            self.frame_ready.emit(result.sampled.physical_colors)
        except Exception as e:
            self._debug_log("error", f"{type(e).__name__}: {e}")
            self.error_occurred.emit(str(e))
