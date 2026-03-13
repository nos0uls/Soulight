# sampler.py — Усреднение цветов по зонам экрана для Screen Mirroring.
#
# Этот модуль делает самое важное вычисление Ambilight:
# берёт кадр экрана, смотрит в заранее подготовленные прямоугольники,
# считает средний цвет для каждого LED и собирает итоговый RGB буфер.
#
# Особенности:
# - disabled LED становятся чёрными
# - start_offset превращается в чёрные LED в начале физического буфера
# - smoothing работает как blend между предыдущим и текущим кадром

from dataclasses import dataclass
from typing import Iterable, List, Optional, Sequence, Tuple

from soulight.screen_mirroring.layout import ScreenMirrorLayout, LayoutLed, SampleRect
from soulight.screen_mirroring.screen_capture import CaptureFrame


# Этот dataclass хранит уже готовый результат sampling.
# Его удобно передавать дальше в driver/protocol слой.
@dataclass(frozen=True)
class SampledColors:
    physical_colors: List[Tuple[int, int, int]]
    logical_colors: List[Tuple[int, int, int]]


# Этот класс хранит состояние сглаживания между кадрами.
# Так следующий кадр можно плавно смешать с предыдущим.
class FrameSmoother:
    def __init__(self, factor: float = 0.35):
        self._factor = 0.0
        self.factor = factor
        self._previous: Optional[List[Tuple[int, int, int]]] = None

    @property
    def factor(self) -> float:
        return self._factor

    @factor.setter
    def factor(self, value: float) -> None:
        if not (0.0 <= float(value) <= 1.0):
            raise ValueError("smoothing factor must be between 0.0 and 1.0")
        self._factor = float(value)

    # Сбрасываем историю, чтобы следующий кадр не смешивался со старым состоянием.
    # Это полезно при смене монитора, layout или режима mirroring.
    def reset(self) -> None:
        self._previous = None

    # Применяем сглаживание к новому списку цветов.
    # factor=0.0 означает мгновенную реакцию.
    # factor=1.0 означает фактически полную инерцию.
    def apply(self, colors: Sequence[Tuple[int, int, int]]) -> List[Tuple[int, int, int]]:
        current = [tuple(map(int, color)) for color in colors]
        if self._previous is None or self._factor <= 0.0:
            self._previous = list(current)
            return list(current)

        if len(self._previous) != len(current):
            self._previous = list(current)
            return list(current)

        out: List[Tuple[int, int, int]] = []
        keep = self._factor
        take = 1.0 - keep

        for (pr, pg, pb), (cr, cg, cb) in zip(self._previous, current):
            nr = int(pr * keep + cr * take)
            ng = int(pg * keep + cg * take)
            nb = int(pb * keep + cb * take)
            out.append((_clamp(nr), _clamp(ng), _clamp(nb)))

        self._previous = list(out)
        return out


# Эта функция делает полный sampling одного кадра.
# На входе уже должен быть готовый layout, чтобы здесь не пересчитывать геометрию.
def sample_frame(
    frame: CaptureFrame,
    layout: ScreenMirrorLayout,
    smoother: Optional[FrameSmoother] = None,
) -> SampledColors:
    if frame.width != layout.capture_width or frame.height != layout.capture_height:
        raise ValueError("frame size does not match layout capture size")

    logical_colors: List[Tuple[int, int, int]] = []
    for led in layout.leds:
        if not led.enabled:
            logical_colors.append((0, 0, 0))
            continue
        logical_colors.append(_average_rect_bgra(frame, led.sample_rect))

    if smoother is not None:
        logical_colors = smoother.apply(logical_colors)

    # physical_colors включает start_offset как чёрный префикс.
    # Это важно, чтобы буфер соответствовал реальной физической ленте.
    physical_colors: List[Tuple[int, int, int]] = [(0, 0, 0)] * layout.physical_led_count
    for led, color in zip(layout.leds, logical_colors):
        physical_colors[led.physical_index] = color

    return SampledColors(
        physical_colors=physical_colors,
        logical_colors=logical_colors,
    )


# Эта функция превращает список RGB-кортежей в плоский byte buffer.
# Такой буфер потом удобно передать в helper для SyncRGB или другой packet generator.
def flatten_rgb(colors: Iterable[Tuple[int, int, int]]) -> bytes:
    out = bytearray()
    for r, g, b in colors:
        out.append(_clamp(r))
        out.append(_clamp(g))
        out.append(_clamp(b))
    return bytes(out)


# Эта функция считает средний RGB цвет прямоугольника внутри BGRA кадра.
# Мы читаем сырые байты напрямую, чтобы не тянуть numpy раньше времени.
def _average_rect_bgra(frame: CaptureFrame, rect: SampleRect) -> Tuple[int, int, int]:
    stride = frame.width * 4
    x0 = max(0, min(frame.width, rect.x))
    y0 = max(0, min(frame.height, rect.y))
    x1 = max(x0 + 1, min(frame.width, rect.x + rect.width))
    y1 = max(y0 + 1, min(frame.height, rect.y + rect.height))

    total_r = 0
    total_g = 0
    total_b = 0
    pixels = 0
    data = frame.bgra

    for y in range(y0, y1):
        row_base = y * stride
        for x in range(x0, x1):
            i = row_base + x * 4
            b = data[i]
            g = data[i + 1]
            r = data[i + 2]
            total_r += r
            total_g += g
            total_b += b
            pixels += 1

    if pixels <= 0:
        return (0, 0, 0)

    return (
        total_r // pixels,
        total_g // pixels,
        total_b // pixels,
    )


# Простой clamp нужен, чтобы после blend и других вычислений
# байтовые значения не выходили за диапазон 0..255.
def _clamp(value: int) -> int:
    return max(0, min(255, int(value)))
