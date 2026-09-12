# app_settings.py — Общие настройки приложения (режим, автояркость).
#
# JSON в user-config директории (рядом с color_preset.json /
# led_config.json). Ничего не знает про Qt — читается и из headless.

import json
import os
import sys


def _config_dir():
    if sys.platform == "win32":
        base = os.getenv("APPDATA") or os.path.expanduser("~")
        return os.path.join(base, "Soulight")
    return os.path.join(os.path.expanduser("~"), ".config", "soulight")


SETTINGS_FILE = os.path.join(_config_dir(), "app_settings.json")

DEFAULTS = {
    # Восстановление последнего режима при старте
    "restore_mode": True,
    "last_mode": "color",       # color | mirror | scene | audio | off
    "last_color": [255, 0, 255],
    "brightness": 255,
    "scene_pattern": "rainbow",
    "scene_speed": 1.0,
    "scene_fps": 20,
    "scene_full_led": True,
    "audio_mode": "spectrum",
    "audio_device": None,       # id устройства или None = default mic
    "audio_fps": 30,
    "audio_full_led": True,
    # Автояркость — ключи совпадают с auto_brightness.DEFAULTS
    "auto_enabled": False,
    "auto_ambient_enabled": False,
    "auto_camera_index": 0,
    "auto_poll_interval": 3.0,
    "auto_luma_dark": 30.0,
    "auto_luma_bright": 160.0,
    "auto_min_level": 40,
    "auto_max_level": 255,
    "auto_time_enabled": False,
    "auto_day_start": 8.0,
    "auto_night_start": 22.0,
    "auto_day_cap": 255,
    "auto_night_cap": 70,
    "auto_transition_min": 45,
    "auto_warmth_night": 0.0,
}


class AppSettings:
    def __init__(self):
        self._data = dict(DEFAULTS)
        self.load()

    def get(self, key, default=None):
        return self._data.get(key, DEFAULTS.get(key, default))

    def set(self, key, value):
        self._data[key] = value
        self.save()

    def update(self, **kw):
        self._data.update(kw)
        self.save()

    def load(self):
        try:
            if os.path.exists(SETTINGS_FILE):
                with open(SETTINGS_FILE, "r") as f:
                    self._data.update(json.load(f))
        except Exception as e:
            print(f"[Settings] Ошибка загрузки: {e}")

    def save(self):
        """Атомарная запись: tmp + os.replace — обрыв посередине не
        оставляет обрезанный JSON, который сломал бы следующую загрузку."""
        try:
            os.makedirs(os.path.dirname(SETTINGS_FILE), exist_ok=True)
            tmp = SETTINGS_FILE + ".tmp"
            with open(tmp, "w") as f:
                json.dump(self._data, f, indent=2)
            os.replace(tmp, SETTINGS_FILE)
        except Exception as e:
            print(f"[Settings] Ошибка сохранения: {e}")

    def auto_params(self) -> dict:
        """Параметры для AutoBrightnessService (без префикса auto_)."""
        return {k[5:]: v for k, v in self._data.items()
                if k.startswith("auto_") and k != "auto_enabled"}
