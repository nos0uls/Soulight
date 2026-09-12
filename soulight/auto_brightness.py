# auto_brightness.py — Автоматическая яркость и цветовая температура.
#
# Два источника, работающие независимо и комбинируемо:
#   1. Ambient: средняя яркость кадра веб-камеры (OpenCV) → luma → уровень.
#   2. Время: кривая день/ночь → cap яркости + тёплая температура ночью.
#
# Итоговая цель = min(ambient_level, time_cap). При выключенном ambient
# цель = time_cap, при выключенном time — чистый ambient.
#
# Сервис — один daemon-поток; Qt не нужен. Ручное движение слайдера
# яркости приостанавливает авто-режим на manual_pause_s секунд.

import threading
import time
from typing import Optional

try:
    import cv2
    CV2_AVAILABLE = True
except ImportError:
    CV2_AVAILABLE = False

try:
    import numpy as np
except ImportError:  # pragma: no cover
    np = None


# ---------------------------------------------------------------------------
# Чистые функции (тестируемые без железа)
# ---------------------------------------------------------------------------

def _clamp255(v: float) -> int:
    return max(0, min(255, int(round(v))))


def day_weight(hour: float, day_start: float, night_start: float,
               transition_h: float) -> float:
    """
    Вес "дня" в точке суток: 1.0 в середине дня, 0.0 ночью, плавный
    линейный переход transition_h часов на обеих границах.
    Все часы — float 0..24 (например 21.5 = 21:30).
    """
    day_len = (night_start - day_start) % 24.0
    if day_len == 0.0:
        return 0.5  # вырожденный случай: день == ночь
    hh = (hour - day_start) % 24.0
    if hh >= day_len:
        return 0.0
    t = max(1e-6, transition_h)
    # ramp-up у рассвета (hh→0), ramp-down у заката (hh→day_len)
    w = min(hh / t, (day_len - hh) / t, 1.0)
    return max(0.0, w)


def time_cap(hour: float, day_start: float, night_start: float,
             day_cap: int, night_cap: int, transition_min: int) -> int:
    """Cap яркости по времени суток (0-255)."""
    w = day_weight(hour, day_start, night_start, transition_min / 60.0)
    return _clamp255(night_cap + (day_cap - night_cap) * w)


def night_warmth(hour: float, day_start: float, night_start: float,
                 max_warmth: float, transition_min: int) -> float:
    """Тёплая температура ночью: 0 днём → max_warmth глубокой ночью."""
    w = day_weight(hour, day_start, night_start, transition_min / 60.0)
    return max(0.0, min(1.0, max_warmth)) * (1.0 - w)


def ambient_level(luma: float, luma_dark: float, luma_bright: float,
                  min_level: int, max_level: int) -> int:
    """
    Маппинг яркости комнаты (luma 0-255) в яркость ленты:
    темнее luma_dark → min_level, светлее luma_bright → max_level.
    """
    lo, hi = sorted((float(luma_dark), float(luma_bright)))
    if hi - lo < 1.0:
        hi = lo + 1.0
    t = (float(luma) - lo) / (hi - lo)
    t = max(0.0, min(1.0, t))
    return _clamp255(min_level + t * (max_level - min_level))


# ---------------------------------------------------------------------------
# Сервис
# ---------------------------------------------------------------------------

DEFAULTS = {
    "ambient_enabled": False,
    "camera_index": 0,
    "poll_interval": 3.0,      # сек между кадрами камеры
    "luma_dark": 30.0,         # luma комнаты «темно»
    "luma_bright": 160.0,      # luma комнаты «светло»
    "min_level": 40,           # яркость ленты в темноте
    "max_level": 255,          # яркость ленты при светлой комнате
    "time_enabled": False,
    "day_start": 8.0,          # 08:00
    "night_start": 22.0,       # 22:00
    "day_cap": 255,
    "night_cap": 70,
    "transition_min": 45,
    "warmth_night": 0.0,       # 0 = без сдвига температуры
    "manual_pause_s": 300.0,   # пауза авто-режима после ручного слайдера
}


class AutoBrightnessService:
    """
    Фоновый сервис автояркости. Зовёт driver.set_brightness() /
    set_temperature() по расписанию. Не трогает цвет и режимы.
    """

    def __init__(self, driver, params: Optional[dict] = None):
        self._driver = driver
        self._params = dict(DEFAULTS)
        if params:
            self.update_params(**params)
        self._running = False
        self._thread: Optional[threading.Thread] = None
        self._stop = threading.Event()
        self._camera = None
        self._camera_paused = False   # быстрая пауза без рестарта сервиса
        self._cam_lock = threading.Lock()  # open/read/release атомарно —
        # VideoCapture не потокобезопасен, release() во время read() = UB
        self._camera_fail_count = 0
        self._manual_until = 0.0
        self._last_luma: Optional[float] = None
        self._luma_ema: Optional[float] = None
        self._last_target: Optional[int] = None
        self._last_applied: Optional[int] = None  # реально отправленное — deadband
        self.status_cb = None  # callable(str) для UI-индикатора

    # ---- публичный API ----

    def update_params(self, **kw):
        # Атомарная подмена dict — поток _loop всегда видит консистентный
        # snapshot параметров, а не половину новых + половину старых.
        merged = {k: v for k, v in kw.items() if k in self._params}
        new_index = merged.get("camera_index")
        if new_index is not None and new_index != self._params["camera_index"]:
            with self._cam_lock:
                self._release_camera()
            self._camera_fail_count = 0
        self._params = {**self._params, **merged}

    @property
    def params(self) -> dict:
        return dict(self._params)

    @property
    def last_luma(self) -> Optional[float]:
        return self._last_luma

    @property
    def last_target(self) -> Optional[int]:
        return self._last_target

    @property
    def running(self) -> bool:
        return self._running

    def notify_manual_adjustment(self):
        """UI зовёт при ручном движении слайдера яркости."""
        self._manual_until = time.monotonic() + float(self._params["manual_pause_s"])

    @property
    def camera_paused(self) -> bool:
        return self._camera_paused

    def set_camera_paused(self, paused: bool):
        """
        Быстрая пауза веб-камеры без рестарта сервиса: камера освобождается,
        чтения пропускаются. Кривая день/ночь и manual-pause работают как раньше.
        """
        self._camera_paused = bool(paused)
        if paused:
            # Под lock'ом — release() не должен пересечься с read() в потоке.
            with self._cam_lock:
                self._release_camera()
            self._camera_fail_count = 0

    def start(self):
        if self._running:
            return
        # Сбрасываем накопленное состояние — иначе сервис рестартует с
        # часовой давности luma/EMA и наследует старый manual-pause.
        self._luma_ema = None
        self._last_luma = None
        self._last_target = None
        self._last_applied = None
        self._manual_until = 0.0
        self._running = True
        self._stop.clear()
        self._thread = threading.Thread(target=self._loop, daemon=True)
        self._thread.start()

    def stop(self):
        self._running = False
        self._stop.set()
        if self._thread is not None:
            self._thread.join(timeout=2.0)
            self._thread = None
        self._release_camera()

    # ---- внутреннее ----

    def _emit_status(self, text: str):
        if self.status_cb is not None:
            try:
                self.status_cb(text)
            except Exception:
                pass

    def _release_camera(self):
        if self._camera is not None:
            try:
                self._camera.release()
            except Exception:
                pass
            self._camera = None

    def _read_luma(self) -> Optional[float]:
        """Один кадр с камеры → средняя luma 0-255, или None при недоступности."""
        if not CV2_AVAILABLE or np is None:
            return None
        try:
            idx = int(self._params["camera_index"])
        except (TypeError, ValueError):
            return None
        # Под lock'ом: UI-поток может звать set_camera_paused/stop → release()
        # параллельно с нашим open/read — VideoCapture не потокобезопасен.
        with self._cam_lock:
            if self._camera_paused or self._stop.is_set():
                return None
            try:
                if self._camera is None:
                    cap = cv2.VideoCapture(idx)
                    if not cap.isOpened():
                        cap.release()
                        return None
                    cap.set(cv2.CAP_PROP_FRAME_WIDTH, 320)
                    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 240)
                    self._camera = cap
                ok, frame = self._camera.read()
                if not ok or frame is None:
                    raise RuntimeError("camera read failed")
                # Средняя luma; frame BGR uint8
                small = frame if frame.shape[0] <= 240 else frame[::4, ::4]
                luma = float(small[..., 0].mean() * 0.114 +
                             small[..., 1].mean() * 0.587 +
                             small[..., 2].mean() * 0.299)
                self._camera_fail_count = 0
                return luma
            except Exception:
                self._camera_fail_count += 1
                self._release_camera()
                return None

    def _compute_target(self, now_hour: float) -> tuple:
        """Возвращает (brightness 0-255, warmth 0..1)."""
        p = self._params
        cap = 255
        warmth = 0.0
        if p["time_enabled"]:
            cap = time_cap(now_hour, p["day_start"], p["night_start"],
                           p["day_cap"], p["night_cap"], p["transition_min"])
            warmth = night_warmth(now_hour, p["day_start"], p["night_start"],
                                  p["warmth_night"], p["transition_min"])

        if p["ambient_enabled"] and self._luma_ema is not None:
            lvl = ambient_level(self._luma_ema, p["luma_dark"], p["luma_bright"],
                                p["min_level"], p["max_level"])
            target = min(lvl, cap) if p["time_enabled"] else lvl
        elif p["time_enabled"]:
            target = cap
        else:
            # Ambient-only без данных (камера на паузе/недоступна, до первого
            # кадра): держим последнее применённое — а не скачок на 255.
            target = self._last_target if self._last_target is not None else int(p["min_level"])
        return target, warmth

    def _loop(self):
        last_cam = 0.0
        while not self._stop.is_set():
            try:
                last_cam = self._tick(last_cam)
            except Exception as e:
                # Битый параметр в settings и т.п. не должен убивать сервис.
                self._emit_status(f"error: {e}")
            self._stop.wait(1.0)

    def _tick(self, last_cam: float) -> float:
        """Одна итерация цикла; возвращает обновлённый last_cam."""
        p = self._params
        now = time.monotonic()

        # Камера опрашивается с периодом poll_interval; при серии
        # ошибок открытия — дополнительный backoff, чтобы не дёргать
        # cv2.VideoCapture каждый цикл на отсутствующей камере.
        if p["ambient_enabled"] and not self._camera_paused:
            backoff = 30.0 if self._camera_fail_count >= 5 else 0.0
            if now - last_cam >= float(p["poll_interval"]) + backoff:
                last_cam = now
                luma = self._read_luma()
                if luma is not None:
                    self._last_luma = luma
                    self._luma_ema = luma if self._luma_ema is None else (
                        self._luma_ema * 0.7 + luma * 0.3)

        lt = time.localtime()
        hour = lt.tm_hour + lt.tm_min / 60.0
        target, warmth = self._compute_target(hour)
        self._last_target = target

        if self._driver.connected and now >= self._manual_until:
            # Deadband: лента сама подсвечивает комнату → luma гуляет →
            # без порога цель дрожала бы каждый poll (мерцание яркости).
            if (self._last_applied is None
                    or abs(target - self._last_applied) >= 6):
                self._driver.set_brightness(target)
                self._last_applied = target
            # Температуру шлём всегда — warmth=0.0 при выключенной
            # кривой снимает устаревший ночной сдвиг.
            self._driver.set_temperature(warmth)

        self._emit_status(
            f"luma={self._last_luma if self._last_luma is None else round(self._last_luma)} "
            f"→ {target}"
            + (" (manual)" if now < self._manual_until else "")
            + (" (cam paused)" if self._camera_paused else "")
        )
        # Цель пересчитывается каждую секунду — кривая времени и
        # manual-pause движутся плавно независимо от poll_interval.
        return last_cam
