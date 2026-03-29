# layout.py — Геометрия зон захвата для Screen Mirroring.
#
# Этот модуль превращает LEDConfig в понятную структуру для sampling:
# - какой LED в каком порядке идёт по физической ленте
# - какой прямоугольник экрана соответствует этому LED
# - какие LED выключены и должны стать чёрными
# - сколько физических LED нужно отправить с учётом start_offset
#
# Важно:
# start_offset добавляет чёрные LED в начало физического буфера.
# Это нужно, потому что лента может начинаться не с логического первого LED у экрана.

from dataclasses import dataclass
from typing import List, Tuple

from soulight.led_config import (
    LEDConfig,
    SIDE_TOP,
    SIDE_BOTTOM,
    SIDE_LEFT,
    SIDE_RIGHT,
)


# Этот dataclass описывает одну прямоугольную зону экрана.
# Именно из неё потом будет браться средний цвет для одного LED.
@dataclass(frozen=True)
class SampleRect:
    x: int
    y: int
    width: int
    height: int


# Этот dataclass описывает один LED в уже готовом порядке обхода.
# Здесь есть и логическая информация, и физический индекс в буфере ленты.
@dataclass(frozen=True)
class LayoutLed:
    side: str
    side_index: int
    enabled: bool
    logical_index: int
    physical_index: int
    sample_rect: SampleRect


# Этот dataclass хранит весь итоговый layout для одного монитора.
# Он нужен как единая точка правды для capture/sampling/send pipeline.
@dataclass(frozen=True)
class ScreenMirrorLayout:
    capture_width: int
    capture_height: int
    edge_depth: int
    logical_led_count: int
    physical_led_count: int
    leds: List[LayoutLed]


# Эта таблица задаёт точный порядок сторон и направление индексов на стороне.
# Булево поле reverse означает, что индексы стороны надо читать в обратном порядке.
#
# Базовые индексы по сторонам такие:
# - top: слева направо
# - bottom: слева направо
# - left: сверху вниз
# - right: сверху вниз
#
# Затем reverse разворачивает этот порядок там, где физическая лента идёт наоборот.
_TRAVERSAL_MAP = {
    (1, True): [
        (SIDE_BOTTOM, True),
        (SIDE_LEFT, True),
        (SIDE_TOP, False),
        (SIDE_RIGHT, False),
    ],
    (2, True): [
        (SIDE_RIGHT, False),
        (SIDE_BOTTOM, True),
        (SIDE_LEFT, True),
        (SIDE_TOP, False),
    ],
    (3, True): [
        (SIDE_TOP, False),
        (SIDE_RIGHT, False),
        (SIDE_BOTTOM, True),
        (SIDE_LEFT, True),
    ],
    (4, True): [
        (SIDE_LEFT, True),
        (SIDE_TOP, False),
        (SIDE_RIGHT, False),
        (SIDE_BOTTOM, True),
    ],
    (1, False): [
        (SIDE_RIGHT, True),
        (SIDE_TOP, True),
        (SIDE_LEFT, False),
        (SIDE_BOTTOM, False),
    ],
    (2, False): [
        (SIDE_TOP, True),
        (SIDE_LEFT, False),
        (SIDE_BOTTOM, False),
        (SIDE_RIGHT, True),
    ],
    (3, False): [
        (SIDE_LEFT, False),
        (SIDE_BOTTOM, False),
        (SIDE_RIGHT, True),
        (SIDE_TOP, True),
    ],
    (4, False): [
        (SIDE_BOTTOM, False),
        (SIDE_RIGHT, True),
        (SIDE_TOP, True),
        (SIDE_LEFT, False),
    ],
}


# Эта функция строит полный layout для screen mirroring.
# Она ничего не захватывает и ничего не отправляет в ленту.
# Она только отвечает на вопрос: "какой LED чему соответствует на экране?"
def build_layout(config: LEDConfig, capture_width: int, capture_height: int, edge_fraction: float = 0.08) -> ScreenMirrorLayout:
    if capture_width <= 0 or capture_height <= 0:
        raise ValueError("capture size must be positive")

    if not (0.0 < edge_fraction < 0.5):
        raise ValueError("edge_fraction must be between 0.0 and 0.5")

    traversal = _TRAVERSAL_MAP.get((config.start_corner, config.clockwise))
    if traversal is None:
        raise ValueError("unsupported start_corner / clockwise combination")

    # Толщина зоны вдоль края экрана.
    # Это та полоса, из которой будем усреднять цвет.
    edge_depth = max(1, int(min(capture_width, capture_height) * edge_fraction))

    leds: List[LayoutLed] = []
    logical_index = 0
    # LED цвета начинаются с индекса 0.
    # Offset чёрные LED будут в конце буфера (indices total..total+offset-1).
    # bridge.py реверсирует массив → offset окажется в начале физической ленты.
    physical_index = 0

    for side, reverse in traversal:
        side_count = config.counts.get(side, 0)
        side_enabled = config.enabled.get(side, [])
        ordered_indices = list(range(side_count))
        if reverse:
            ordered_indices.reverse()

        for side_index in ordered_indices:
            enabled = side_enabled[side_index] if side_index < len(side_enabled) else True
            sample_rect = _build_sample_rect(
                side=side,
                side_index=side_index,
                side_count=side_count,
                capture_width=capture_width,
                capture_height=capture_height,
                edge_depth=edge_depth,
            )
            leds.append(
                LayoutLed(
                    side=side,
                    side_index=side_index,
                    enabled=enabled,
                    logical_index=logical_index,
                    physical_index=physical_index,
                    sample_rect=sample_rect,
                )
            )
            logical_index += 1
            physical_index += 1

    return ScreenMirrorLayout(
        capture_width=capture_width,
        capture_height=capture_height,
        edge_depth=edge_depth,
        logical_led_count=config.total,
        physical_led_count=config.start_offset + config.total,
        leds=leds,
    )


# Эта функция создаёт одну sampling-зону для одного LED.
# Деление простое и стабильное: каждая сторона разбивается на равные сегменты.
def _build_sample_rect(
    side: str,
    side_index: int,
    side_count: int,
    capture_width: int,
    capture_height: int,
    edge_depth: int,
) -> SampleRect:
    if side_count <= 0:
        raise ValueError("side_count must be positive")

    if side in (SIDE_TOP, SIDE_BOTTOM):
        x0, x1 = _segment_bounds(side_index, side_count, capture_width)
        y = 0 if side == SIDE_TOP else capture_height - edge_depth
        return SampleRect(x=x0, y=y, width=max(1, x1 - x0), height=edge_depth)

    if side in (SIDE_LEFT, SIDE_RIGHT):
        y0, y1 = _segment_bounds(side_index, side_count, capture_height)
        x = 0 if side == SIDE_LEFT else capture_width - edge_depth
        return SampleRect(x=x, y=y0, width=edge_depth, height=max(1, y1 - y0))

    raise ValueError(f"unsupported side: {side}")


# Эта функция делит длину стороны на равные сегменты.
# Мы используем целочисленные границы, чтобы зоны всегда были чистыми rect,
# без дробных координат и без накопления ошибок по float.
def _segment_bounds(index: int, count: int, total_length: int) -> Tuple[int, int]:
    start = int(index * total_length / count)
    end = int((index + 1) * total_length / count)
    if end <= start:
        end = min(total_length, start + 1)
    return start, end
