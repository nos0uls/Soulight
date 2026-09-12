# color_preset.py — Сохранение и загрузка последнего цвета

import json
import os
import sys

# Конфиг живёт в user-config директории — внутри пакета писать нельзя:
# при frozen-сборке (PyInstaller) __file__ указывает во временный каталог.
def _config_dir():
    if sys.platform == "win32":
        base = os.getenv("APPDATA") or os.path.expanduser("~")
        return os.path.join(base, "Soulight")
    return os.path.join(os.path.expanduser("~"), ".config", "soulight")


CONFIG_FILE = os.path.join(_config_dir(), "color_preset.json")
# Старый путь (внутри пакета) — читаем один раз для миграции.
_LEGACY_FILE = os.path.join(os.path.dirname(__file__), "color_preset.json")


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
        try:
            os.makedirs(os.path.dirname(CONFIG_FILE), exist_ok=True)
            with open(CONFIG_FILE, "w") as f:
                json.dump(data, f, indent=2)
            self._last_saved = data
        except Exception as e:
            print(f"[ColorPreset] Ошибка сохранения: {e}")

    def load(self):
        """Загружает preset из JSON файла (с миграцией со старого пути)."""
        path = CONFIG_FILE
        if not os.path.exists(path):
            path = _LEGACY_FILE
        if not os.path.exists(path):
            self._last_saved = self._snapshot()
            return

        try:
            with open(path, "r") as f:
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
