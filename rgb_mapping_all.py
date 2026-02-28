# -*- coding: utf-8 -*-
"""
rgb_mapping_all.py — Тест RGB маппинга через инверсию ВСЕХ color пакетов.

Модифицируем ВСЕ пакеты 238-245 (не только 239-245).

Метод: XOR 0xFF на одной позиции в каждой тройке.
Если byte X = R-канал: инверсия добавит красный к текущему цвету.
Если byte X = G-канал: инверсия уберёт/добавит зелёный.
Если byte X = B-канал: инверсия добавит синий.

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
NUM_LEDS = 75
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


def is_color_packet(pkt):
    """ВСЕ пакеты 238-245 содержат цветовые данные."""
    return 238 <= len(pkt) <= 245
# endregion


# region ===== Модификаторы =====
def mod_byte0(pkt):
    """XOR 0xFF на позиции 0 в каждой RGB тройке."""
    m = bytearray(pkt)
    plen = len(pkt)
    
    # Color start: 12 для plen=238, 12+extra для plen>238
    extra = max(0, plen - 238)
    color_start = 12 + extra
    
    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 0:
            m[i] ^= 0xFF
    
    return bytes(m)


def mod_byte1(pkt):
    """XOR 0xFF на позиции 1 в каждой RGB тройке."""
    m = bytearray(pkt)
    plen = len(pkt)
    extra = max(0, plen - 238)
    color_start = 12 + extra
    
    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 1:
            m[i] ^= 0xFF
    
    return bytes(m)


def mod_byte2(pkt):
    """XOR 0xFF на позиции 2 в каждой RGB тройке."""
    m = bytearray(pkt)
    plen = len(pkt)
    extra = max(0, plen - 238)
    color_start = 12 + extra
    
    for i in range(color_start, len(m) - 1):
        if (i - color_start) % 3 == 2:
            m[i] ^= 0xFF
    
    return bytes(m)
# endregion


# region ===== Replay =====
def replay_with_mod(writes, modifier, label):
    """Replay всех пакетов с модификацией color-пакетов."""
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    
    mod_count = 0
    for w in writes:
        if is_color_packet(w):
            w = modifier(w)
            mod_count += 1
        
        ser.write(w)
        ser.flush()
        
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("  %s: %d color packets modified" % (label, mod_count))
    time.sleep(5)
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("RGB MAPPING — инверсия ВСЕХ color пакетов (238-245)")
    print("=" * 60)
    print()
    
    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    print("green.csv: %d packets, %d color (238-245)" % (len(writes), n_color))
    print()
    print("Исходный цвет = screen capture (зелёноватый UI Beelight)")
    print()
    
    tests = [
        ("TEST 0: Оригинал (без модификации)", None),
        ("TEST 1: Инверсия byte 0 в тройках", mod_byte0),
        ("TEST 2: Инверсия byte 1 в тройках", mod_byte1),
        ("TEST 3: Инверсия byte 2 в тройках", mod_byte2),
    ]
    
    for label, modifier in tests:
        print("--- %s ---" % label)
        if modifier:
            replay_with_mod(writes, modifier, label.split(":")[0])
        else:
            # Оригинал - просто replay
            ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
            ser.dtr = True
            ser.rts = True
            time.sleep(0.3)
            ser.read(4096)
            for w in writes:
                ser.write(w)
                ser.flush()
                if len(w) >= 238:
                    time.sleep(0.033)
                elif len(w) == 5:
                    time.sleep(0.005)
                else:
                    time.sleep(0.05)
            print("  Оригинал: replayed %d packets" % len(writes))
            time.sleep(5)
            ser.close()
            time.sleep(0.5)
    
    print()
    print("=" * 60)
    print("РЕЗУЛЬТАТЫ:")
    print("  TEST 0 (оригинал): ___")
    print("  TEST 1 (byte 0):   ___")
    print("  TEST 2 (byte 1):   ___")
    print("  TEST 3 (byte 2):   ___")
    print()
    print("ИНТЕРПРЕТАЦИЯ для зелёноватого исходника:")
    print("  ЖЁЛТЫЙ/ОРАНЖЕВЫЙ = инвертировали R-канал")
    print("  ТЁМНЫЙ/СИНИЙ    = инвертировали G-канал")
    print("  ЦИАН/ГОЛУБОЙ    = инвертировали B-канал")


if __name__ == "__main__":
    main()
# endregion
