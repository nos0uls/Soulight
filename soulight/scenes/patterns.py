# patterns.py — Паттерн-генераторы для сценических режимов.
#
# Каждый паттерн — функция вида:
#   pattern(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]
#
# Возвращает список RGB-кортежей длиной led_count.

import colorsys
import math
import random
from typing import List, Tuple


# region Утилиты

def _clamp(v: int) -> int:
    return max(0, min(255, int(v)))


def _hsv(h: float, s: float, v: float) -> Tuple[int, int, int]:
    r, g, b = colorsys.hsv_to_rgb(h % 1.0, s, v)
    return (_clamp(r * 255), _clamp(g * 255), _clamp(b * 255))


def _lerp(a: float, b: float, t: float) -> float:
    return a + (b - a) * t

# endregion


# region Паттерны

def rainbow(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Радуга: Hue смещается по ленте и со временем."""
    speed = params.get("speed", 1.0)
    offset = (frame_index * 0.01 * speed) % 1.0
    return [_hsv((i / led_count + offset) % 1.0, 1.0, 1.0) for i in range(led_count)]


def fire(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Огонь: красный→оранжевый, мерцание."""
    speed = params.get("speed", 1.0)
    colors = []
    for i in range(led_count):
        noise = random.uniform(-20, 20)
        flicker = math.sin(frame_index * 0.15 * speed + i * 0.3) * 30
        r = _clamp(220 + flicker + noise)
        g = _clamp(60 + flicker * 0.5 + noise * 0.3)
        b = _clamp(10 + noise * 0.1)
        colors.append((r, g, b))
    return colors


def aurora(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Полярное сияние: зелёный→фиолетовый, волны."""
    speed = params.get("speed", 1.0)
    colors = []
    for i in range(led_count):
        wave = math.sin(frame_index * 0.05 * speed + i * 0.15) * 0.5 + 0.5
        h = _lerp(0.33, 0.75, wave)  # green -> purple
        s = 0.8 + math.sin(frame_index * 0.08 * speed) * 0.15
        v = 0.6 + wave * 0.3
        colors.append(_hsv(h, s, v))
    return colors


def chase(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Бегущие огни: группа ярких LED движется по ленте."""
    speed = params.get("speed", 1.0)
    width = max(2, led_count // 10)
    pos = int((frame_index * 1.5 * speed) % led_count)
    colors = []
    for i in range(led_count):
        dist = min((i - pos) % led_count, (pos - i) % led_count)
        if dist < width:
            intensity = 1.0 - (dist / width)
            hue = (frame_index * 0.02 * speed) % 1.0
            colors.append(_hsv(hue, 1.0, intensity))
        else:
            colors.append((0, 0, 0))
    return colors


def flow(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Плавное течение цвета по ленте."""
    speed = params.get("speed", 1.0)
    colors = []
    for i in range(led_count):
        h = ((i / led_count) * 0.5 + frame_index * 0.008 * speed) % 1.0
        v = 0.7 + math.sin(frame_index * 0.1 * speed + i * 0.2) * 0.25
        colors.append(_hsv(h, 0.9, v))
    return colors


def warm(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Тёплый жёлтый, имитация свечи."""
    speed = params.get("speed", 1.0)
    colors = []
    for i in range(led_count):
        flicker = random.uniform(-15, 15)
        pulse = math.sin(frame_index * 0.08 * speed + i * 0.1) * 10
        r = _clamp(255 + flicker)
        g = _clamp(160 + flicker * 0.6 + pulse)
        b = _clamp(40 + flicker * 0.2)
        colors.append((r, g, b))
    return colors


def vitality(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Энергичная смена цветов: резкие Hue-сдвиги."""
    speed = params.get("speed", 1.0)
    colors = []
    for i in range(led_count):
        h = (frame_index * 0.03 * speed + (i / led_count) * 0.3) % 1.0
        v = 0.8 + math.sin(frame_index * 0.2 * speed + i * 0.5) * 0.15
        colors.append(_hsv(h, 1.0, v))
    return colors


def firework(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Имитация фейерверка: случайные вспышки по ленте."""
    speed = params.get("speed", 1.0)
    # Один всплеск каждые N кадров
    burst_interval = max(5, int(30 / speed))
    colors = [(0, 0, 0)] * led_count

    if frame_index % burst_interval == 0:
        center = random.randint(0, led_count - 1)
        hue = random.random()
        width = max(3, led_count // 8)
        for i in range(led_count):
            dist = min(abs(i - center), led_count - abs(i - center))
            if dist < width:
                decay = 1.0 - (dist / width)
                r, g, b = _hsv(hue, 1.0, decay)
                colors[i] = (r, g, b)
    else:
        # Затухание предыдущего кадра — обрабатывается engine через fade
        pass
    return colors


def romance(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Медленные розовые переходы."""
    speed = params.get("speed", 1.0)
    colors = []
    base_h = 0.92  # розовый
    for i in range(led_count):
        wave = math.sin(frame_index * 0.03 * speed + i * 0.08) * 0.06
        h = (base_h + wave) % 1.0
        v = 0.5 + math.sin(frame_index * 0.04 * speed) * 0.15
        colors.append(_hsv(h, 0.7, v))
    return colors


def seasons(frame_index: int, led_count: int, params: dict) -> List[Tuple[int, int, int]]:
    """Медленные сезонные палитры: весна→лето→осень→зима."""
    speed = params.get("speed", 1.0)
    # Полный цикл за ~600 кадров
    season_t = (frame_index * 0.002 * speed) % 1.0
    season = int(season_t * 4)  # 0..3
    next_season = (season + 1) % 4
    local_t = (season_t * 4) % 1.0

    palettes = [
        ((120, 220, 80), (180, 255, 120)),   # весна: зелёный
        ((255, 220, 60), (255, 180, 40)),    # лето: золотой
        ((220, 100, 20), (180, 60, 20)),     # осень: оранжевый
        ((80, 160, 220), (120, 200, 255)),   # зима: голубой
    ]
    c1 = palettes[season]
    c2 = palettes[next_season]
    colors = []
    for i in range(led_count):
        wave = math.sin(i * 0.15) * 0.15
        t = local_t + wave
        r = _clamp(_lerp(c1[0][0], c2[0][0], t))
        g = _clamp(_lerp(c1[0][1], c2[0][1], t))
        b = _clamp(_lerp(c1[0][2], c2[0][2], t))
        colors.append((r, g, b))
    return colors

# endregion


# region Реестр паттернов

PATTERNS = {
    "rainbow": rainbow,
    "fire": fire,
    "aurora": aurora,
    "chase": chase,
    "flow": flow,
    "warm": warm,
    "vitality": vitality,
    "firework": firework,
    "romance": romance,
    "seasons": seasons,
}

PATTERN_LABELS = {
    "rainbow": "Rainbow",
    "fire": "Fire",
    "aurora": "Aurora",
    "chase": "Chase",
    "flow": "Flow",
    "warm": "Warm",
    "vitality": "Vitality",
    "firework": "Firework",
    "romance": "Romance",
    "seasons": "Seasons",
}

# endregion
