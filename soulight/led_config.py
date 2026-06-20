# led_config.py — Модель конфигурации LED ленты.
#
# Хранит информацию о расположении LED по сторонам монитора:
# сколько на каждой стороне, какие включены/выключены,
# начальный угол и направление обхода.

import json
import os

# Путь для сохранения конфигурации
CONFIG_PATH = os.path.join(os.path.dirname(__file__), "..", "led_config.json")


# region Константы сторон и направлений

# Аппаратный лимит контроллера
MAX_LEDS = 75

# Стороны монитора
SIDE_BOTTOM = "bottom"
SIDE_RIGHT = "right"
SIDE_TOP = "top"
SIDE_LEFT = "left"

# Порядок обхода по часовой стрелке начиная с bottom-right (угол 1)
SIDES_CW = [SIDE_BOTTOM, SIDE_LEFT, SIDE_TOP, SIDE_RIGHT]

# Углы монитора (нумерация как в оригинальном Beelight)
# 1 = bottom-right, 2 = top-right, 3 = top-left, 4 = bottom-left
CORNER_NAMES = {
    1: "Bottom-Right",
    2: "Top-Right",
    3: "Top-Left",
    4: "Bottom-Left",
}

# Цвета сторон (как в оригинальном Beelight UI)
# Каждая сторона имеет свой цвет для визуального различения
SIDE_COLORS = {
    SIDE_TOP:    (0, 120, 255),    # Синий
    SIDE_BOTTOM: (255, 60, 60),    # Красный
    SIDE_LEFT:   (60, 220, 60),    # Зелёный
    SIDE_RIGHT:  (220, 180, 40),   # Жёлтый
}

# endregion


class LEDConfig:
    """
    Конфигурация LED ленты.

    Описывает сколько LED на каждой стороне монитора,
    какие включены, откуда начинается лента и в каком направлении.

    Атрибуты:
        counts — dict {side: int} — количество LED на каждой стороне
        enabled — dict {side: list[bool]} — включён ли каждый LED на стороне
        start_corner — int (1-4) — начальный угол
        clockwise — bool — направление обхода по часовой стрелке
        total — int — общее число LED (read-only)
    """

    def __init__(self):
        # Количество LED на каждой стороне (по умолчанию для 75 LED)
        self.counts = {
            SIDE_TOP: 23,
            SIDE_BOTTOM: 23,
            SIDE_LEFT: 15,
            SIDE_RIGHT: 15,
        }
        # Какие LED включены (True = активен, False = выключен)
        # Инициализируем все включёнными
        self.enabled = {}
        self._rebuild_enabled()
        # Начальный угол (1 = bottom-right, как в Beelight)
        self.start_corner = 1
        # Направление: True = по часовой стрелке
        self.clockwise = True
        # Смещение: с какого LED начинается отсчёт (0 = первый физический LED)
        self.start_offset = 0

    @property
    def total(self):
        """Общее число LED по всем сторонам."""
        return sum(self.counts.values())

    def _rebuild_enabled(self):
        """
        Пересоздаёт enabled массивы при изменении counts.
        Сохраняет существующие значения где возможно.
        """
        for side in [SIDE_TOP, SIDE_BOTTOM, SIDE_LEFT, SIDE_RIGHT]:
            count = self.counts[side]
            old = self.enabled.get(side, [])
            # Расширяем или обрезаем
            if len(old) < count:
                self.enabled[side] = old + [True] * (count - len(old))
            else:
                self.enabled[side] = old[:count]

    def remaining(self, exclude_side=None, max_total: int = MAX_LEDS):
        """Сколько LED осталось до лимита max_total (без учёта exclude_side)."""
        used = sum(c for s, c in self.counts.items() if s != exclude_side)
        return max(0, max_total - used)

    def set_count(self, side, count, max_total: int = MAX_LEDS):
        """
        Устанавливает количество LED на стороне.
        Ограничивает сумму всех сторон до max_total (по умолчанию MAX_LEDS).
        max_total позволяет учитывать start_offset из UI.
        """
        max_for_side = self.remaining(exclude_side=side, max_total=max_total)
        self.counts[side] = max(0, min(int(count), max_for_side))
        self._rebuild_enabled()

    def toggle_led(self, side, index):
        """Переключает состояние одного LED (включён/выключен)."""
        if side in self.enabled and 0 <= index < len(self.enabled[side]):
            self.enabled[side][index] = not self.enabled[side][index]

    def set_led(self, side, index, on):
        """Устанавливает состояние одного LED."""
        if side in self.enabled and 0 <= index < len(self.enabled[side]):
            self.enabled[side][index] = bool(on)

    def set_all(self, on):
        """Включает или выключает все LED."""
        for side in self.enabled:
            self.enabled[side] = [on] * len(self.enabled[side])

    def set_side(self, side, on):
        """Включает или выключает все LED на стороне."""
        if side in self.enabled:
            self.enabled[side] = [on] * len(self.enabled[side])

    def get_ordered_sides(self):
        """
        Возвращает стороны в порядке обхода от start_corner.
        Учитывает направление (clockwise / counter-clockwise).
        """
        # Маппинг: start_corner → какая сторона идёт первой (по часовой)
        # Угол 1 (bottom-right): начинаем с bottom, идём влево
        # Угол 2 (top-right): начинаем с right, идём вверх
        # Угол 3 (top-left): начинаем с top, идём вправо
        # Угол 4 (bottom-left): начинаем с left, идём вниз
        corner_to_start = {
            1: [SIDE_BOTTOM, SIDE_LEFT, SIDE_TOP, SIDE_RIGHT],
            2: [SIDE_RIGHT, SIDE_BOTTOM, SIDE_LEFT, SIDE_TOP],
            3: [SIDE_TOP, SIDE_RIGHT, SIDE_BOTTOM, SIDE_LEFT],
            4: [SIDE_LEFT, SIDE_TOP, SIDE_RIGHT, SIDE_BOTTOM],
        }
        sides = corner_to_start.get(self.start_corner, SIDES_CW)
        if not self.clockwise:
            sides = list(reversed(sides))
        return sides

    def get_active_count(self):
        """Количество включённых LED."""
        return sum(
            1 for side in self.enabled for on in self.enabled[side] if on
        )

    # region Сохранение/загрузка

    def save(self, path=None):
        """Сохраняет конфигурацию в JSON файл."""
        path = path or CONFIG_PATH
        data = {
            "counts": self.counts,
            "enabled": self.enabled,
            "start_corner": self.start_corner,
            "clockwise": self.clockwise,
            "start_offset": self.start_offset,
        }
        try:
            with open(path, "w", encoding="utf-8") as f:
                json.dump(data, f, indent=2, ensure_ascii=False)
        except Exception as e:
            print(f"[LEDConfig] Ошибка сохранения: {e}")

    def load(self, path=None):
        """Загружает конфигурацию из JSON файла. Возвращает True при успехе."""
        path = path or CONFIG_PATH
        if not os.path.exists(path):
            return False
        try:
            with open(path, "r", encoding="utf-8") as f:
                data = json.load(f)
            self.counts = data.get("counts", self.counts)
            self.enabled = data.get("enabled", self.enabled)
            self.start_corner = data.get("start_corner", 1)
            self.clockwise = data.get("clockwise", True)
            self.start_offset = data.get("start_offset", 0)
            self._rebuild_enabled()
            return True
        except Exception as e:
            print(f"[LEDConfig] Ошибка загрузки: {e}")
            return False

    # endregion
