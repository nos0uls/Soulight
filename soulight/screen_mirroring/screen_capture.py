# screen_capture.py — Чтение кадра экрана для Screen Mirroring.
#
# Здесь лежит минимальный и понятный слой захвата экрана.
# Он пока не знает ничего про LED layout и отправку в контроллер.
# Его задача простая: вернуть свежий кадр primary monitor в удобном виде.

from dataclasses import dataclass


# Этот dataclass хранит уже готовый кадр экрана.
# Мы держим сырой BGRA buffer, потому что именно так его отдаёт mss,
# и это позволяет не делать лишние копирования до этапа sampling.
@dataclass(frozen=True)
class CaptureFrame:
    width: int
    height: int
    bgra: bytes


# Этот dataclass хранит геометрию выбранного монитора.
# Она нужна отдельно, чтобы layout строился по реальному размеру экрана.
@dataclass(frozen=True)
class MonitorGeometry:
    left: int
    top: int
    width: int
    height: int


# Этот класс отвечает только за захват экрана.
# Он не хранит потоков и не делает авто-луп.
# Это удобно: orchestration можно будет позже держать в отдельном controller/service.
class ScreenCapturer:
    def __init__(self, monitor_index: int = 1):
        # В mss monitor[0] — это виртуальный общий desktop.
        # Для обычного сценария Ambilight нам нужен monitor[1], то есть primary/first monitor.
        self._monitor_index = int(monitor_index)

    @property
    def monitor_index(self) -> int:
        return self._monitor_index

    # Этот метод возвращает реальную геометрию выбранного монитора.
    # Её потом можно передать в layout builder.
    def get_monitor_geometry(self) -> MonitorGeometry:
        import mss

        with mss.mss() as sct:
            monitor = self._get_monitor(sct)
            return MonitorGeometry(
                left=int(monitor["left"]),
                top=int(monitor["top"]),
                width=int(monitor["width"]),
                height=int(monitor["height"]),
            )

    # Этот метод делает один снимок экрана.
    # Возвращаем именно bytes, чтобы дальше можно было безопасно передавать кадр
    # между слоями без привязки к жизненному циклу объекта mss.
    def capture(self) -> CaptureFrame:
        import mss

        with mss.mss() as sct:
            monitor = self._get_monitor(sct)
            shot = sct.grab(monitor)
            return CaptureFrame(
                width=int(shot.width),
                height=int(shot.height),
                bgra=bytes(shot.bgra),
            )

    # Внутренний helper для выбора монитора.
    # Если индекс вышел за пределы, мы падаем сразу с понятной ошибкой,
    # а не продолжаем с неверным экраном.
    def _get_monitor(self, sct) -> dict:
        monitors = sct.monitors
        if self._monitor_index < 1 or self._monitor_index >= len(monitors):
            raise IndexError(
                f"Monitor index {self._monitor_index} is out of range; available monitor count is {len(monitors) - 1}"
            )
        return monitors[self._monitor_index]
