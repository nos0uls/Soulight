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
        # Persistent mss instance временно отключён.
        # На этой Windows-конфигурации стабильнее создавать fresh mss() на каждый кадр.
        self._sct = None

    @property
    def monitor_index(self) -> int:
        return self._monitor_index

    # Закрывает mss instance. Вызывать при остановке mirroring или смене монитора.
    def close(self):
        # Сейчас persistent mss instance не используется.
        self._sct = None

    # Этот метод возвращает реальную геометрию выбранного монитора.
    def get_monitor_geometry(self) -> MonitorGeometry:
        import mss
        # Геометрию тоже читаем через временный mss(), чтобы lifecycle capture backend
        # был полностью локален одному вызову и не переносился между кадрами/потоками.
        with mss.mss() as temp_sct:
            monitor = self._get_monitor(temp_sct)
        return MonitorGeometry(
            left=int(monitor["left"]),
            top=int(monitor["top"]),
            width=int(monitor["width"]),
            height=int(monitor["height"]),
        )

    # Этот метод делает один снимок экрана.
    # Возвращает numpy BGRA array для быстрого sampling.
    def capture(self) -> CaptureFrame:
        import mss
        import numpy as np

        with mss.mss() as sct:
            monitor = self._get_monitor(sct)
            shot = sct.grab(monitor)
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
