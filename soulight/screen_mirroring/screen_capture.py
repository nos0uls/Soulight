# screen_capture.py — Чтение кадра экрана для Screen Mirroring.
#
# Здесь лежит минимальный и понятный слой захвата экрана.
# Он пока не знает ничего про LED layout и отправку в контроллер.
# Его задача простая: вернуть свежий кадр primary monitor в удобном виде.

from dataclasses import dataclass


# Этот dataclass хранит уже готовый кадр экрана.
# bgra — numpy array shape (H, W, 4) dtype=uint8, порядок каналов BGRA.
# numpy позволяет делать быстрое среднее по зонам без Python-циклов.
@dataclass
class CaptureFrame:
    width: int
    height: int
    bgra: object  # np.ndarray (H, W, 4) uint8


# Этот dataclass хранит геометрию выбранного монитора.
# Она нужна отдельно, чтобы layout строился по реальному размеру экрана.
@dataclass(frozen=True)
class MonitorGeometry:
    left: int
    top: int
    width: int
    height: int


# Этот класс отвечает только за захват экрана.
# Он переиспользует mss instance для скорости (создание нового на каждый кадр дорого).
# Orchestration (таймеры, потоки) живёт снаружи.
class ScreenCapturer:
    def __init__(self, monitor_index: int = 1):
        # В mss monitor[0] — это виртуальный общий desktop.
        # Для Ambilight нужен monitor[1+], то есть конкретный physical monitor.
        self._monitor_index = int(monitor_index)
        # Переиспользуемый mss instance — создаётся лениво при первом capture.
        self._sct = None

    @property
    def monitor_index(self) -> int:
        return self._monitor_index

    # Закрывает mss instance. Вызывать при остановке mirroring или смене монитора.
    def close(self):
        if self._sct is not None:
            try:
                self._sct.close()
            except Exception:
                pass
            self._sct = None

    # Лениво создаёт mss instance при первом обращении.
    def _ensure_sct(self):
        if self._sct is None:
            import mss
            self._sct = mss.mss()

    # Этот метод возвращает реальную геометрию выбранного монитора.
    def get_monitor_geometry(self) -> MonitorGeometry:
        self._ensure_sct()
        monitor = self._get_monitor(self._sct)
        return MonitorGeometry(
            left=int(monitor["left"]),
            top=int(monitor["top"]),
            width=int(monitor["width"]),
            height=int(monitor["height"]),
        )

    # Этот метод делает один снимок экрана.
    # Возвращает numpy BGRA array для быстрого sampling.
    def capture(self) -> CaptureFrame:
        import numpy as np

        self._ensure_sct()
        monitor = self._get_monitor(self._sct)
        shot = self._sct.grab(monitor)
        # np.array(shot) — mss ScreenShot поддерживает __array_interface__,
        # что позволяет numpy создать array напрямую без промежуточного bytes().
        bgra = np.array(shot, dtype=np.uint8).reshape(
            (int(shot.height), int(shot.width), 4)
        )
        return CaptureFrame(
            width=int(shot.width),
            height=int(shot.height),
            bgra=bgra,
        )

    # Внутренний helper для выбора монитора.
    def _get_monitor(self, sct) -> dict:
        monitors = sct.monitors
        if self._monitor_index < 1 or self._monitor_index >= len(monitors):
            raise IndexError(
                f"Monitor index {self._monitor_index} is out of range; "
                f"available monitor count is {len(monitors) - 1}"
            )
        return monitors[self._monitor_index]
