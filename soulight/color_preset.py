# color_preset.py — Сохранение и загрузка последнего цвета

import json
import os

# Путь к файлу конфигурации
CONFIG_FILE = os.path.join(os.path.dirname(__file__), "color_preset.json")


class ColorPreset:
    """
    Сохраняет и загружает последний выбранный цвет.
    Автоматически сохраняется при изменении.
    """

    def __init__(self):
        # Текущий цвет RGB (0-255)
        self.r = 255
        self.g = 0
        self.b = 255
        # Brightness (0-255)
        self.brightness = 255
        # Последние сохранённые значения (для избежания лишних записей)
        self._last_saved = None

    def _snapshot(self):
        return {"r": self.r, "g": self.g, "b": self.b, "brightness": self.brightness}

    def set_color(self, r, g, b):
        """Устанавливает цвет RGB."""
        self.r = r
        self.g = g
        self.b = b

    def set_brightness(self, value):
        """Устанавливает яркость."""
        self.brightness = max(0, min(255, int(value)))

    def save(self):
        """Сохраняет preset в JSON файл только если значения изменились."""
        data = self._snapshot()
        if data == self._last_saved:
            return
        self._last_saved = data
        try:
            with open(CONFIG_FILE, "w") as f:
                json.dump(data, f, indent=2)
        except Exception as e:
            print(f"[ColorPreset] Ошибка сохранения: {e}")

    def load(self):
        """Загружает preset из JSON файла."""
        if not os.path.exists(CONFIG_FILE):
            self._last_saved = self._snapshot()
            return

        try:
            with open(CONFIG_FILE, "r") as f:
                data = json.load(f)
            self.r = data.get("r", 255)
            self.g = data.get("g", 0)
            self.b = data.get("b", 255)
            self.brightness = data.get("brightness", 255)
            self._last_saved = self._snapshot()
        except Exception as e:
            print(f"[ColorPreset] Ошибка загрузки: {e}")
            self._last_saved = self._snapshot()

    def as_tuple(self):
        """Возвращает цвет как (r, g, b)."""
        return (self.r, self.g, self.b)
