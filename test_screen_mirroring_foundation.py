# test_screen_mirroring_foundation.py — Проверка layout + sampler foundation
#
# Этот тест не читает реальный экран и не открывает COM-порт.
# Он создаёт синтетический кадр, заливает в sampling-зоны известные цвета
# и проверяет, что layout/sampler собирают правильный RGB buffer.

import sys

sys.path.insert(0, ".")

from soulight.led_config import LEDConfig, SIDE_TOP, SIDE_BOTTOM, SIDE_LEFT, SIDE_RIGHT
from soulight.screen_mirroring.layout import build_layout
from soulight.screen_mirroring.sampler import flatten_rgb, sample_frame
from soulight.screen_mirroring.screen_capture import CaptureFrame


# Простая функция для записи одного пикселя в BGRA буфер.
# Она нужна, чтобы собрать синтетический кадр вручную и без внешних библиотек.
def set_pixel(buf, width, x, y, rgb):
    r, g, b = rgb
    i = (y * width + x) * 4
    buf[i] = b
    buf[i + 1] = g
    buf[i + 2] = r
    buf[i + 3] = 255


# Эта функция заливает прямоугольник одним цветом.
# Так мы точно знаем, какой средний цвет sampler должен вернуть.
def fill_rect(buf, width, rect, rgb):
    for yy in range(rect.y, rect.y + rect.height):
        for xx in range(rect.x, rect.x + rect.width):
            set_pixel(buf, width, xx, yy, rgb)


# Этот тест берёт только верхнюю сторону без других сторон.
# Так зоны не пересекаются в углах, и результат получается полностью детерминированным.
def main():
    cfg = LEDConfig()
    cfg.counts[SIDE_TOP] = 2
    cfg.counts[SIDE_BOTTOM] = 0
    cfg.counts[SIDE_LEFT] = 0
    cfg.counts[SIDE_RIGHT] = 0
    cfg._rebuild_enabled()
    cfg.start_corner = 3
    cfg.clockwise = True
    cfg.start_offset = 2

    width = 100
    height = 60
    layout = build_layout(cfg, capture_width=width, capture_height=height, edge_fraction=0.10)

    print(f"[Foundation] logical_led_count = {layout.logical_led_count}")
    print(f"[Foundation] physical_led_count = {layout.physical_led_count}")
    print(f"[Foundation] edge_depth = {layout.edge_depth}")

    if layout.logical_led_count != 2:
        raise AssertionError(f"Expected 2 logical LEDs, got {layout.logical_led_count}")

    if layout.physical_led_count != 4:
        raise AssertionError(f"Expected 4 physical LEDs, got {layout.physical_led_count}")

    # Чёрный фон кадра.
    bgra = bytearray(width * height * 4)

    expected_logical = [
        (255, 0, 0),
        (0, 255, 0),
    ]

    # Красим sampling-зоны в заранее известные цвета.
    # Так sampler должен вернуть их без изменений.
    for led, color in zip(layout.leds, expected_logical):
        fill_rect(bgra, width, led.sample_rect, color)

    frame = CaptureFrame(width=width, height=height, bgra=bytes(bgra))
    sampled = sample_frame(frame=frame, layout=layout, smoother=None)

    print(f"[Foundation] logical_colors = {sampled.logical_colors}")
    print(f"[Foundation] physical_colors = {sampled.physical_colors}")

    if sampled.logical_colors != expected_logical:
        raise AssertionError(
            f"Logical colors mismatch. Expected {expected_logical}, got {sampled.logical_colors}"
        )

    expected_physical = [
        (0, 0, 0),
        (0, 0, 0),
        (255, 0, 0),
        (0, 255, 0),
    ]
    if sampled.physical_colors != expected_physical:
        raise AssertionError(
            f"Physical colors mismatch. Expected {expected_physical}, got {sampled.physical_colors}"
        )

    flat = flatten_rgb(sampled.physical_colors)
    print(f"[Foundation] flat len = {len(flat)}")
    print(f"[Foundation] flat bytes = {flat.hex(' ')}")

    if len(flat) != 12:
        raise AssertionError(f"Expected 12 RGB bytes, got {len(flat)}")

    print("[Foundation] OK")


if __name__ == "__main__":
    main()
