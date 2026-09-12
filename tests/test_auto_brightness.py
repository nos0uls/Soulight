# test_auto_brightness.py — Тесты чистых функций автояркости и fade драйвера.

import os
import sys
import unittest

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from soulight.auto_brightness import (
    day_weight, time_cap, night_warmth, ambient_level,
)


class DayWeightTests(unittest.TestCase):
    # day 8:00 → night 22:00, переход 1ч
    DS, NS, T = 8.0, 22.0, 1.0

    def test_midday_full(self):
        self.assertAlmostEqual(day_weight(15.0, self.DS, self.NS, self.T), 1.0)

    def test_deep_night_zero(self):
        self.assertAlmostEqual(day_weight(2.0, self.DS, self.NS, self.T), 0.0)
        self.assertAlmostEqual(day_weight(0.0, self.DS, self.NS, self.T), 0.0)

    def test_dawn_ramp(self):
        # 8:30 — половина перехода после day_start → ~0.5
        self.assertAlmostEqual(day_weight(8.5, self.DS, self.NS, self.T), 0.5)

    def test_dusk_ramp(self):
        # 21:30 — за полчаса до night_start → ~0.5
        self.assertAlmostEqual(day_weight(21.5, self.DS, self.NS, self.T), 0.5)
        self.assertAlmostEqual(day_weight(22.5, self.DS, self.NS, self.T), 0.0)

    def test_monotonic_edges(self):
        w_prev = 0.0
        for m in range(6 * 60, 14 * 60, 10):  # 6:00 → 14:00
            w = day_weight(m / 60.0, self.DS, self.NS, self.T)
            self.assertGreaterEqual(w + 1e-9, w_prev)
            w_prev = w


class TimeCapTests(unittest.TestCase):
    def test_caps(self):
        self.assertEqual(time_cap(15.0, 8, 22, 255, 70, 45), 255)
        self.assertEqual(time_cap(2.0, 8, 22, 255, 70, 45), 70)
        # середина перехода — между капами
        mid = time_cap(8.0 + 45 / 120.0, 8, 22, 255, 70, 45)
        self.assertTrue(70 < mid < 255)


class NightWarmthTests(unittest.TestCase):
    def test_bounds(self):
        self.assertEqual(night_warmth(15.0, 8, 22, 0.8, 45), 0.0)
        self.assertAlmostEqual(night_warmth(2.0, 8, 22, 0.8, 45), 0.8)
        self.assertEqual(night_warmth(2.0, 8, 22, 0.0, 45), 0.0)


class AmbientLevelTests(unittest.TestCase):
    def test_mapping(self):
        self.assertEqual(ambient_level(0, 30, 160, 40, 255), 40)
        self.assertEqual(ambient_level(250, 30, 160, 40, 255), 255)
        mid = ambient_level(95, 30, 160, 40, 255)
        self.assertTrue(140 < mid < 160)  # ~ середина диапазона

    def test_swapped_thresholds(self):
        # luma_dark > luma_bright — сортируем
        self.assertEqual(ambient_level(0, 160, 30, 40, 255), 40)

    def test_degenerate_range(self):
        self.assertEqual(ambient_level(30, 50, 50, 40, 255), 40)
        self.assertEqual(ambient_level(60, 50, 50, 40, 255), 255)


class DriverFadeTests(unittest.TestCase):
    """Fade-логика драйвера без реального serial."""

    def _make_driver(self):
        from soulight.protocol.serial_driver import LEDDriver
        d = LEDDriver(port="/dev/null-nonexistent", protocol="native")
        return d

    def test_set_brightness_sets_target_not_current(self):
        d = self._make_driver()
        d.set_brightness(100)
        self.assertEqual(d._target_brightness, 100)
        self.assertEqual(d._brightness, 255.0)  # ещё не дошла

    def test_hw_dimmer_rounds(self):
        d = self._make_driver()
        d._brightness = 128.0
        self.assertEqual(d._hw_dimmer(), 502)
        d._brightness = 0.0
        self.assertEqual(d._hw_dimmer(), 0)
        d._brightness = 255.0
        self.assertEqual(d._hw_dimmer(), 1000)

    def test_temperature_targets(self):
        d = self._make_driver()
        d.set_temperature(1.0)
        self.assertAlmostEqual(d._target_temp_rgb[0], 1.0)
        self.assertAlmostEqual(d._target_temp_rgb[1], 0.8)
        self.assertAlmostEqual(d._target_temp_rgb[2], 0.55)
        d.set_temperature(0.0)
        self.assertEqual(d._target_temp_rgb, (1.0, 1.0, 1.0))

    def test_lerp_converges(self):
        d = self._make_driver()
        d._brightness = 0.0
        d._target_brightness = 200
        for _ in range(60):
            d._brightness = d._lerp_step(d._brightness, d._target_brightness)
        self.assertAlmostEqual(d._brightness, 200, delta=1.0)


class ServiceSmokeTests(unittest.TestCase):
    def test_params_update_and_status(self):
        from soulight.auto_brightness import AutoBrightnessService

        class FakeDriver:
            connected = True
            def __init__(self):
                self.bright = None
                self.temp = None
            def set_brightness(self, v): self.bright = v
            def set_temperature(self, v): self.temp = v

        drv = FakeDriver()
        svc = AutoBrightnessService(drv, {"time_enabled": True,
                                          "day_cap": 200, "night_cap": 50})
        svc.update_params(min_level=10)
        self.assertEqual(svc.params["min_level"], 10)
        # целевое значение считается без камеры
        t, w = svc._compute_target(2.0)  # ночь
        self.assertEqual(t, 50)
        self.assertEqual(w, 0.0)  # warmth_night=0 по умолчанию
        t, w = svc._compute_target(15.0)
        self.assertEqual(t, 200)


if __name__ == "__main__":
    unittest.main()
