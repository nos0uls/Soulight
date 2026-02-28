# -*- coding: utf-8 -*-
"""
rgb_mapping_fixed.py — Правильный тест RGB маппинга.

Исправление: color data ВСЕГДА начинается с байта 12, независимо от plen.
Дополнительные байты в 239-245 — это padding/metadata ДО color data.

Метод как в рабочем channel_mapping_test.py:
- mod-0: байты 12, 15, 18, 21, ... (i-12)%3==0
- mod-1: байты 13, 16, 19, 22, ... (i-12)%3==1
- mod-2: байты 14, 17, 20, 23, ... (i-12)%3==2

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
    """Реальные color-пакеты: 239-245 байт (как в рабочем тесте)."""
    return 239 <= len(raw) <= 245
# endregion


# region ===== Модификаторы (как в рабочем channel_mapping_test.py) =====
def mod_only_mod0(raw):
    """XOR ТОЛЬКО позиции mod-0: байты 12, 15, 18, 21, ..."""
    m = bytearray(raw)
    for i in range(12, len(m) - 1):
        if (i - 12) % 3 == 0:
            m[i] ^= 0xFF
    return bytes(m)


def mod_only_mod1(raw):
    """XOR ТОЛЬКО позиции mod-1: байты 13, 16, 19, 22, ..."""
    m = bytearray(raw)
    for i in range(13, len(m) - 1):
        if (i - 12) % 3 == 1:
            m[i] ^= 0xFF
    return bytes(m)


def mod_only_mod2(raw):
    """XOR ТОЛЬКО позиции mod-2: байты 14, 17, 20, 23, ..."""
    m = bytearray(raw)
    for i in range(14, len(m) - 1):
        if (i - 12) % 3 == 2:
            m[i] ^= 0xFF
    return bytes(m)
# endregion


# region ===== Replay =====
def replay_with_mod(writes, modifier=None, label=""):
    """Replay всех write-операций. Модификатор применяется к color-пакетам (239-245)."""
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

    print("  %s: модифицировано %d color-пакетов" % (label, color_count))
    time.sleep(5)
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("RGB MAPPING TEST — определение R/G/B позиций (исправлено)")
    print("=" * 60)
    print()

    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    n_238 = sum(1 for w in writes if len(w) == 238)
    print("green.csv: %d writes, %d color (239-245), %d heartbeat (238)" % (
        len(writes), n_color, n_238))
    print()

    tests = [
        ("TEST 0: GREEN original (контроль)", None),
        ("TEST 1: XOR mod-0 only (байты 12,15,18,...)", mod_only_mod0),
        ("TEST 2: XOR mod-1 only (байты 13,16,19,...)", mod_only_mod1),
        ("TEST 3: XOR mod-2 only (байты 14,17,20,...)", mod_only_mod2),
    ]

    for label, modifier in tests:
        print("\n--- %s ---" % label)
        replay_with_mod(writes, modifier, label.split(":")[0].strip())

    print("\n" + "=" * 60)
    print("РЕЗУЛЬТАТЫ — какой цвет показала лента в каждом тесте?")
    print("  0 (оригинал):   ___ (должен быть ЗЕЛЁНЫЙ)")
    print("  1 (mod-0 XOR):  ___ ")
    print("  2 (mod-1 XOR):  ___")
    print("  3 (mod-2 XOR):  ___")
    print()
    print("ИНТЕРПРЕТАЦИЯ (если исходный цвет ЗЕЛЁНЫЙ):")
    print("  ЖЁЛТЫЙ  = инвертировали R-канал (R: 0->255, G остался)")
    print("  ЧЁРНЫЙ = инвертировали G-канал (G: 255->0)")
    print("  ЦИАН   = инвертировали B-канал (B: 0->255, G остался)")
    print("  МУСОР/ОТКАЗ = инвертировали структурный padding")


if __name__ == "__main__":
    main()
# endregion
