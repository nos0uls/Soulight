# test_rgb_transfer_diff.py — Дифф между разными RGB transfer пакетами
#
# Этот скрипт ничего не отправляет в COM-порт.
# Он генерирует несколько 245-byte пакетов через текущий bridge
# и показывает, где именно меняются байты при смене цветов LED.
#
# Цель: понять, можно ли использовать существующий encrypted packet
# как шаблон для raw patching цветовой области.

import sys

sys.path.insert(0, ".")

from soulight.protocol.bridge import BeelightBridge


# Создаём массив одинакового цвета на всю длину ленты.
def solid_leds(color, count=75):
    return [tuple(color)] * count


# Возвращаем список индексов, где два пакета отличаются.
def diff_positions(a, b):
    return [i for i, (x, y) in enumerate(zip(a, b)) if x != y]


# Печатаем компактную статистику по диффу.
def dump_diff(label, a, b):
    pos = diff_positions(a, b)
    print(f"[{label}] diff count = {len(pos)}")
    if not pos:
        return
    print(f"[{label}] first 64 diff positions = {pos[:64]}")
    print(f"[{label}] min={min(pos)} max={max(pos)}")


# Печатаем хвост пакета, где с высокой вероятностью лежат LED-данные.
def dump_tail(label, data, tail_len=240):
    start = max(0, len(data) - tail_len)
    print(f"[{label}] len = {len(data)}")
    print(f"[{label}] head = {data[:24].hex(' ')}")
    print(f"[{label}] tail-start-index = {start}")
    print(f"[{label}] tail = {data[start:].hex(' ')}")


# Проверяем несколько цветовых наборов на всей ленте.
def main():
    bridge = BeelightBridge()
    if not bridge.init():
        raise RuntimeError("bridge.init() failed")

    black = bridge.make_rgb_transfer_packet(solid_leds((0, 0, 0)))
    red = bridge.make_rgb_transfer_packet(solid_leds((255, 0, 0)))
    green = bridge.make_rgb_transfer_packet(solid_leds((0, 255, 0)))
    blue = bridge.make_rgb_transfer_packet(solid_leds((0, 0, 255)))
    white = bridge.make_rgb_transfer_packet(solid_leds((255, 255, 255)))

    for name, pkt in [
        ("black", black),
        ("red", red),
        ("green", green),
        ("blue", blue),
        ("white", white),
    ]:
        if pkt is None:
            raise RuntimeError(f"{name} packet is None")
        dump_tail(name, pkt, tail_len=80)
        print()

    dump_diff("black->red", black, red)
    dump_diff("black->green", black, green)
    dump_diff("black->blue", black, blue)
    dump_diff("black->white", black, white)

    print()
    color_start = len(black) - 225
    color_end = color_start + 225
    print(f"[Guess] color_start={color_start} color_end={color_end - 1}")

    black_seg = black[color_start:color_end]
    red_seg = red[color_start:color_end]
    green_seg = green[color_start:color_end]
    blue_seg = blue[color_start:color_end]
    white_seg = white[color_start:color_end]

    print(f"[Seg] black[:24] = {black_seg[:24].hex(' ')}")
    print(f"[Seg] red[:24] = {red_seg[:24].hex(' ')}")
    print(f"[Seg] green[:24] = {green_seg[:24].hex(' ')}")
    print(f"[Seg] blue[:24] = {blue_seg[:24].hex(' ')}")
    print(f"[Seg] white[:24] = {white_seg[:24].hex(' ')}")


if __name__ == "__main__":
    main()
