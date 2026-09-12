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
from typing import List, Optional, Tuple

import numpy as np

from soulight.led_config import SIDE_TOP, SIDE_BOTTOM
from soulight.screen_mirroring.layout import ScreenMirrorLayout, SampleRect
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

    # Применяем сглаживание к numpy array цветов.
    # factor=0.0 означает мгновенную реакцию.
    # factor=1.0 означает фактически полную инерцию.
    def apply(self, colors: np.ndarray) -> np.ndarray:
        # colors: shape (N, 3) float32
        if self._prev_np is None or self._factor <= 0.0:
            self._prev_np = colors.copy()
            return colors

        if self._prev_np.shape != colors.shape:
            self._prev_np = colors.copy()
            return colors

        # Weighted blend: prev * keep + current * (1 - keep)
        blended = self._prev_np * self._factor + colors * (1.0 - self._factor)
        np.clip(blended, 0, 255, out=blended)
        self._prev_np = blended.copy()
        return blended


# Эта функция делает полный sampling одного кадра.
# На входе уже должен быть готовый layout, чтобы здесь не пересчитывать геометрию.
def sample_frame(
    frame: CaptureFrame,
    layout: ScreenMirrorLayout,
    smoother: Optional[FrameSmoother] = None,
    saturation_boost: float = 1.0,
) -> SampledColors:
    if frame.width != layout.capture_width or frame.height != layout.capture_height:
        raise ValueError("frame size does not match layout capture size")

    n_leds = len(layout.leds)
    colors_np = np.zeros((n_leds, 3), dtype=np.float32)

    # Группируем LED по side для минимизации переключений между region buffers.
    side_groups: dict[str, list[int]] = {}
    for i, led in enumerate(layout.leds):
        if led.enabled:
            side_groups.setdefault(led.side, []).append(i)

    for side, indices in side_groups.items():
        region = _select_edge_region(frame, side)
        if region is None:
            continue
        if frame.rgb is None:
            # Быстрый путь для edge strips: один средний "профиль" полосы
            # (среднее по поперечной оси) + reduceat по сегментам LED.
            # ~4 numpy-операции на сторону вместо per-LED mean над 2D окном.
            _sample_side_strip(region, side, layout, indices, colors_np)
        else:
            for i in indices:
                led = layout.leds[i]
                colors_np[i] = _average_rect_rgb(region, frame.width, frame.height, led.sample_rect)

    # Векторизованный saturation boost через numpy.
    if saturation_boost != 1.0:
        colors_np = _boost_saturation_np(colors_np, saturation_boost)

    # Smoothing работает с numpy array напрямую.
    if smoother is not None:
        colors_np = smoother.apply(colors_np)

    # Конвертируем в list of tuples для совместимости с driver.
    logical_colors = [tuple(int(v) for v in row) for row in colors_np]

    # physical_colors включает start_offset как чёрный суффикс.
    physical_colors: List[Tuple[int, int, int]] = [(0, 0, 0)] * layout.physical_led_count
    for led, color in zip(layout.leds, logical_colors):
        physical_colors[led.physical_index] = color

    return SampledColors(
        physical_colors=physical_colors,
        logical_colors=logical_colors,
    )


# Векторизованный sampling одной стороны по edge strip.
# Математически эквивалентно per-LED _average_rect_rgb: среднее по rect =
# среднее по (cross-axis mean → segment mean), т.к. rect занимает всю
# толщину полосы.
def _sample_side_strip(
    region: CaptureRegion,
    side: str,
    layout: ScreenMirrorLayout,
    indices: List[int],
    out: np.ndarray,
) -> None:
    horizontal = side in (SIDE_TOP, SIDE_BOTTOM)
    strip = region.rgb  # (depth, W, 3) для top/bottom, (H, depth, 3) для left/right
    # Субсэмплинг по толщине полосы: для ambient-усреднения ~24 строк
    # неотличимы от всех 86, а mean в 3-4x дешевле.
    cross = strip.shape[0] if horizontal else strip.shape[1]
    step = max(1, cross // 24)
    # Среднее по поперечной оси полосы → 1D-профиль цвета вдоль края.
    line = (strip[::step] if horizontal else strip[:, ::step]).mean(
        axis=0 if horizontal else 1, dtype=np.float32
    )  # (N, 3)
    n = line.shape[0]

    # Собираем сегменты в координатах полосы, сортируем по start
    # (LED на стороне могут идти в обратном порядке обхода).
    segs = []
    for i in indices:
        rect = layout.leds[i].sample_rect
        if horizontal:
            s = rect.x - region.left
            e = s + rect.width
        else:
            s = rect.y - region.top
            e = s + rect.height
        s = max(0, min(n - 1, s))
        e = max(s + 1, min(n, e))
        segs.append((s, e, i))
    segs.sort()

    starts = np.array([s for s, _, _ in segs], dtype=np.int64)
    counts = np.array([e - s for s, e, _ in segs], dtype=np.float32)
    sums = np.add.reduceat(line, starts, axis=0)
    means = sums / counts[:, None]
    for (s, e, i), m in zip(segs, means):
        out[i] = m


# Этот helper выбирает edge strip по стороне LED.
def _select_edge_region(frame: CaptureFrame, side: str) -> Optional[CaptureRegion]:
    edge_regions = frame.edge_regions or {}
    if frame.rgb is not None:
        return CaptureRegion(
            left=0, top=0, width=frame.width, height=frame.height, rgb=frame.rgb,
        )
    return edge_regions.get(side)


# Считает средний RGB цвет прямоугольника внутри RGB numpy-кадра.
def _average_rect_rgb(
    region: CaptureRegion,
    frame_width: int,
    frame_height: int,
    rect: SampleRect,
) -> np.ndarray:
    x0 = max(0, min(frame_width, rect.x))
    y0 = max(0, min(frame_height, rect.y))
    x1 = max(x0 + 1, min(frame_width, rect.x + rect.width))
    y1 = max(y0 + 1, min(frame_height, rect.y + rect.height))

    local_x0 = max(0, min(region.width, x0 - region.left))
    local_y0 = max(0, min(region.height, y0 - region.top))
    local_x1 = max(local_x0 + 1, min(region.width, x1 - region.left))
    local_y1 = max(local_y0 + 1, min(region.height, y1 - region.top))

    sample = region.rgb[local_y0:local_y1, local_x0:local_x1]
    if sample.size == 0:
        return np.zeros(3, dtype=np.float32)

    return sample.mean(axis=(0, 1)).astype(np.float32)


# Векторизованный saturation boost через numpy.
# Для 75 LED проще работать с массивом напрямую через min/max/arithmetic.
def _boost_saturation_np(colors: np.ndarray, boost: float) -> np.ndarray:
    # colors: shape (N, 3) float32, значения 0..255
    # Простая модель: увеличиваем разницу между каналами и средним.
    # Это эквивалентно увеличению насыщенности без полного HSV конвертирования.
    mean = colors.mean(axis=1, keepdims=True)  # (N, 1)
    boosted = mean + (colors - mean) * boost
    np.clip(boosted, 0, 255, out=boosted)
    return boosted
