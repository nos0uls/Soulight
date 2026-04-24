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

import colorsys
from dataclasses import dataclass
from typing import Iterable, List, Optional, Sequence, Tuple

import numpy as np

from soulight.screen_mirroring.layout import ScreenMirrorLayout, LayoutLed, SampleRect
from soulight.screen_mirroring.screen_capture import CaptureFrame, CaptureRegion


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
        self._prev_np: Optional[np.ndarray] = None  # shape (N, 3) float32

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
        self._prev_np = None

    # Применяем сглаживание к новому списку цветов.
    # factor=0.0 означает мгновенную реакцию.
    # factor=1.0 означает фактически полную инерцию.
    def apply(self, colors: Sequence[Tuple[int, int, int]]) -> List[Tuple[int, int, int]]:
        # numpy vectorized blend — на порядки быстрее поэлементного Python-цикла.
        current = np.array(colors, dtype=np.float32)
        if self._prev_np is None or self._factor <= 0.0:
            self._prev_np = current.copy()
            return [tuple(int(v) for v in row) for row in current]

        if self._prev_np.shape != current.shape:
            self._prev_np = current.copy()
            return [tuple(int(v) for v in row) for row in current]

        # Weighted blend: prev * keep + current * (1 - keep)
        blended = self._prev_np * self._factor + current * (1.0 - self._factor)
        np.clip(blended, 0, 255, out=blended)
        self._prev_np = blended.copy()
        return [tuple(int(v) for v in row) for row in blended]


# Эта функция делает полный sampling одного кадра.
# На входе уже должен быть готовый layout, чтобы здесь не пересчитывать геометрию.
def sample_frame(
    frame: CaptureFrame,
    layout: ScreenMirrorLayout,
    smoother: Optional[FrameSmoother] = None,
    saturation_boost: float = 1.0,
) -> SampledColors:
    # width/height layout проверяем всегда, даже если пиксели приходят как edge strips.
    # Так sampler остаётся привязан к реальному размеру исходного монитора.
    if frame.width != layout.capture_width or frame.height != layout.capture_height:
        raise ValueError("frame size does not match layout capture size")

    logical_colors: List[Tuple[int, int, int]] = []
    for led in layout.leds:
        if not led.enabled:
            logical_colors.append((0, 0, 0))
            continue
        color = _average_rect_bgra(frame, led)
        # Saturation boost делает цвета насыщеннее (как у Beelight).
        # При boost=1.0 цвет не меняется.
        if saturation_boost != 1.0:
            color = _boost_saturation(*color, saturation_boost)
        logical_colors.append(color)

    if smoother is not None:
        logical_colors = smoother.apply(logical_colors)

    # physical_colors включает start_offset как чёрный суффикс.
    # bridge.py реверсирует весь массив перед отправкой, поэтому
    # чёрные offset-LED должны быть в конце буфера — после реверса
    # они окажутся в начале физической ленты (до монитора).
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
    # numpy vectorized — быстрее чем Python loop + bytearray.append
    arr = np.array(list(colors), dtype=np.uint8)
    return bytes(arr.flatten())


# Эта функция считает средний RGB цвет прямоугольника внутри BGRA numpy-кадра.
# Используем numpy slicing — на порядки быстрее поэлементного Python-цикла.
def _average_rect_bgra(frame: CaptureFrame, led: LayoutLed) -> Tuple[int, int, int]:
    rect = led.sample_rect
    # Если у нас есть полный кадр — используем старый и самый простой путь.
    # Это удобно и для совместимости, и для синтетических тестов.
    if frame.bgra is not None:
        return _average_rect_from_region(
            region=CaptureRegion(
                left=0,
                top=0,
                width=frame.width,
                height=frame.height,
                bgra=frame.bgra,
            ),
            frame_width=frame.width,
            frame_height=frame.height,
            rect=rect,
        )

    # Если full frame нет, значит capture path принёс только edge strips.
    # Тогда выбираем нужный strip по положению rect и считаем средний цвет уже в нём.
    if frame.edge_regions:
        region = _select_edge_region(frame=frame, led=led)
        if region is not None:
            return _average_rect_from_region(
                region=region,
                frame_width=frame.width,
                frame_height=frame.height,
                rect=rect,
            )

    raise ValueError("capture frame does not contain accessible BGRA data for sample rect")


# Этот helper выбирает один edge strip, в котором целиком лежит sample rect.
# Layout уже знает точную сторону каждого LED, поэтому side —
# самый надёжный источник правды для выбора strip buffer.
def _select_edge_region(frame: CaptureFrame, led: LayoutLed) -> Optional[CaptureRegion]:
    edge_regions = frame.edge_regions or {}
    if led.side == "top":
        return edge_regions.get("top")
    if led.side == "left":
        return edge_regions.get("left")
    if led.side == "bottom":
        return edge_regions.get("bottom")
    if led.side == "right":
        return edge_regions.get("right")
    return None


# Этот helper переводит global screen rect в локальные координаты region buffer.
# После этого mean считается так же, как и раньше, через numpy slicing.
def _average_rect_from_region(
    region: CaptureRegion,
    frame_width: int,
    frame_height: int,
    rect: SampleRect,
) -> Tuple[int, int, int]:
    # Сначала ограничиваем rect границами всего исходного кадра.
    # Это сохраняет прежнюю защиту от выходов за экран.
    x0 = max(0, min(frame_width, rect.x))
    y0 = max(0, min(frame_height, rect.y))
    x1 = max(x0 + 1, min(frame_width, rect.x + rect.width))
    y1 = max(y0 + 1, min(frame_height, rect.y + rect.height))

    local_x0 = max(0, min(region.width, x0 - region.left))
    local_y0 = max(0, min(region.height, y0 - region.top))
    local_x1 = max(local_x0 + 1, min(region.width, x1 - region.left))
    local_y1 = max(local_y0 + 1, min(region.height, y1 - region.top))

    # region.bgra — numpy array shape (H, W, 4) dtype=uint8, порядок каналов BGRA
    sample = region.bgra[local_y0:local_y1, local_x0:local_x1]
    if sample.size == 0:
        return (0, 0, 0)

    # mean по осям 0,1 даёт средний цвет всей зоны (4 канала)
    avg = sample.mean(axis=(0, 1))
    # BGRA → RGB
    return (int(avg[2]), int(avg[1]), int(avg[0]))


# region Color correction helpers

# Простой clamp нужен, чтобы после blend и других вычислений
# байтовые значения не выходили за диапазон 0..255.
def _clamp(value: int) -> int:
    return max(0, min(255, int(value)))


# Усиливает насыщенность одного RGB кортежа.
# boost=1.0 — без изменений, boost=1.5 — на 50% ярче, boost=0.5 — на 50% бледнее.
# Работает в пространстве HSV: увеличивает S канал без изменения H и V.
def _boost_saturation(r: int, g: int, b: int, boost: float) -> Tuple[int, int, int]:
    if boost == 1.0:
        return (r, g, b)
    h, s, v = colorsys.rgb_to_hsv(r / 255.0, g / 255.0, b / 255.0)
    s = min(1.0, s * boost)
    nr, ng, nb = colorsys.hsv_to_rgb(h, s, v)
    return (_clamp(int(nr * 255)), _clamp(int(ng * 255)), _clamp(int(nb * 255)))

# endregion
