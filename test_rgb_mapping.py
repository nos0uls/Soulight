# -*- coding: utf-8 -*-
"""
test_rgb_mapping.py — Точный тест маппинга RGB каналов через инверсию.

Метод (проверенный в channel_mapping_test.py который работал):
1. Инвертируем только ОДИН байт в каждой тройке cipher
2. Отправляем модифицированный пакет
3. Наблюдаем какой цвет получается

Для green.csv (исходный = зелёный screen capture):
- Если инвертируем R-байт: зелёный + красный = ЖЁЛТЫЙ
- Если инвертируем G-байт: зелёный -> чёрный/тёмный
- Если инвертируем B-байт: зелёный + синий = ЦИАН

COM7 @ 500000 baud.
"""
import sys
import os
import time

sys.stdout.reconfigure(encoding="utf-8")

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

# region ===== Настройки =====
BASE = os.path.dirname(os.path.abspath(__file__))
CAPTURE_PATH = os.path.join(BASE, "green.csv")
COM_PORT = "COM7"
BAUD = 500000
# endregion


# region ===== Парсинг CSV =====
def parse_writes(filepath):
    """Извлекает все WRITE-операции из CSV-файла захвата."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append(raw)
    return writes


def is_color_packet(raw):
    """Реальные цветовые пакеты: 239-245 байт."""
    return 239 <= len(raw) <= 245
# endregion


# region ===== Модификаторы =====
def mod_byte0_only(raw):
    """Инвертирует только байт 0 в каждой тройке (позиции 0, 3, 6... от начала color region)."""
    m = bytearray(raw)
    extra = len(raw) - 238
    color_start = 12 + extra

    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 0:
            m[i] ^= 0xFF
    return bytes(m)


def mod_byte1_only(raw):
    """Инвертирует только байт 1 в каждой тройке."""
    m = bytearray(raw)
    extra = len(raw) - 238
    color_start = 12 + extra

    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 1:
            m[i] ^= 0xFF
    return bytes(m)


def mod_byte2_only(raw):
    """Инвертирует только байт 2 в каждой тройке."""
    m = bytearray(raw)
    extra = len(raw) - 238
    color_start = 12 + extra

    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 2:
            m[i] ^= 0xFF
    return bytes(m)
# endregion


# region ===== Отправка =====
def replay_with_mod(writes, modifier=None, label=""):
    """Replay всех пакетов с модификацией 239-245."""
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)

    color_count = 0
    for w in writes:
        if is_color_packet(w) and modifier:
            w = modifier(w)
            color_count += 1
        ser.write(w)
        ser.flush()
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)

    print("  %s: modified %d color packets" % (label, color_count))
    time.sleep(4)
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("RGB MAPPING TEST — инверсия по одному байту")
    print("=" * 60)
    print()

    writes = parse_writes(CAPTURE_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))

    print("green.csv: %d writes, %d color packets (239-245)" % (len(writes), n_color))
    print()
    print("Исходный цвет = screen capture (зелёноватый UI)")
    print()

    tests = [
        ("TEST 0: Оригинал (контроль)", None),
        ("TEST 1: Инверсия байта 0 в тройках", mod_byte0_only),
        ("TEST 2: Инверсия байта 1 в тройках", mod_byte1_only),
        ("TEST 3: Инверсия байта 2 в тройках", mod_byte2_only),
    ]

    for label, modifier in tests:
        print("--- %s ---" % label)
        replay_with_mod(writes, modifier, label.split(":")[0])

    print()
    print("=" * 60)
    print("РЕЗУЛЬТАТЫ — какой цвет показала лента?")
    print("  TEST 0 (оригинал): ___")
    print("  TEST 1 (byte 0):   ___")
    print("  TEST 2 (byte 1):   ___")
    print("  TEST 3 (byte 2):   ___")
    print()
    print("ИНТЕРПРЕТАЦИЯ:")
    print("  Если byte X инверсия даёт ЖЁЛТЫЙ -> byte X = R-канал")
    print("  Если byte X инверсия даёт ЦИАН   -> byte X = B-канал")
    print("  Если byte X инверсия даёт ТЁМНЫЙ -> byte X = G-канал")


if __name__ == "__main__":
    main()
# endregion
