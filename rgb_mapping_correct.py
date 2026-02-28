# -*- coding: utf-8 -*-
"""
rgb_mapping_correct.py — Правильный тест RGB маппинга с учётом offset.

Открытие: color_start зависит от plen:
- plen=238: color_start = 12
- plen=239: color_start = 13
- plen=240: color_start = 14
- plen=241: color_start = 15
- ...
- plen=245: color_start = 19

Формула: color_start = 12 + (plen - 238) = plen - 226

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
CSV_PATH = os.path.join(BASE, "green.csv")
COM_PORT = "COM7"
BAUD = 500000
# endregion


# region ===== Парсинг CSV =====
def parse_writes(filepath):
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
    """Реальные color-пакеты: 239-245 байт."""
    return 239 <= len(raw) <= 245


def get_color_start(plen):
    """Возвращает начало color data для данного plen."""
    # Формула: color_start = plen - 226
    # plen=238 -> 12, plen=241 -> 15, plen=245 -> 19
    return plen - 226
# endregion


# region ===== Модификаторы =====
def mod_byte0_correct(raw):
    """XOR байт 0 в каждой тройке, начиная с правильного color_start."""
    m = bytearray(raw)
    color_start = get_color_start(len(raw))
    
    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 0:
            m[i] ^= 0xFF
    
    return bytes(m)


def mod_byte1_correct(raw):
    """XOR байт 1 в каждой тройке."""
    m = bytearray(raw)
    color_start = get_color_start(len(raw))
    
    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 1:
            m[i] ^= 0xFF
    
    return bytes(m)


def mod_byte2_correct(raw):
    """XOR байт 2 в каждой тройке."""
    m = bytearray(raw)
    color_start = get_color_start(len(raw))
    
    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 2:
            m[i] ^= 0xFF
    
    return bytes(m)
# endregion


# region ===== Replay =====
def replay_with_mod(writes, modifier=None, label=""):
    """Replay с модификацией color-пакетов."""
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

    print("  %s: %d color packets" % (label, color_count))
    time.sleep(5)
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("RGB MAPPING — правильный offset для каждого plen")
    print("=" * 60)
    print()
    print("Формула: color_start = plen - 226")
    print("  plen=238 -> start=12")
    print("  plen=241 -> start=15")
    print("  plen=245 -> start=19")
    print()
    
    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    print("green.csv: %d packets, %d color (239-245)" % (len(writes), n_color))
    print()
    
    tests = [
        ("TEST 0: Оригинал", None),
        ("TEST 1: XOR byte 0 (R-канал?)", mod_byte0_correct),
        ("TEST 2: XOR byte 1 (G-канал?)", mod_byte1_correct),
        ("TEST 3: XOR byte 2 (B-канал?)", mod_byte2_correct),
    ]
    
    for label, modifier in tests:
        print("--- %s ---" % label)
        replay_with_mod(writes, modifier, label.split(":")[0])
    
    print()
    print("=" * 60)
    print("РЕЗУЛЬТАТЫ:")
    print("  TEST 0 (оригинал): ___")
    print("  TEST 1 (byte 0):  ___")
    print("  TEST 2 (byte 1):  ___")
    print("  TEST 3 (byte 2):  ___")


if __name__ == "__main__":
    main()
# endregion
