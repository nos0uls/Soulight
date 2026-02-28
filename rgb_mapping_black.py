# -*- coding: utf-8 -*-
"""
rgb_mapping_black.py — Точный тест RGB маппинга используя black.csv.

Для black.csv: plaintext color = 0x00, значит cipher = keystream.
XOR cipher с (R, G, B) напрямую даёт целевой цвет.

Метод:
1. Отправляем black.csv с XOR(R, G, B) на color region
2. Наблюдаем цвет ленты

Если byte 0 = R, byte 1 = G, byte 2 = B:
  - RED (255,0,0): лента красная
  - GREEN (0,255,0): лента зелёная
  - BLUE (0,0,255): лента синяя

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
CSV_PATH = os.path.join(BASE, "black.csv")
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
    """Пакеты 238-245 байт — color data."""
    return 238 <= len(pkt) <= 245
# endregion


# region ===== Модификация =====
def modify_plen238(pkt, r, g, b):
    """
    plen=238: XOR cipher напрямую с (R, G, B).
    
    Для black.csv: cipher = keystream (потому что plaintext = 0).
    XOR с (R, G, B) даёт: new_cipher = keystream ^ (R, G, B)
    При дешифровке: plain = new_cipher ^ keystream = (R, G, B)
    """
    m = bytearray(pkt)
    
    # Color data: байты 12-236 (75 * 3 = 225)
    for led in range(NUM_LEDS):
        base = 12 + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] ^= r
            m[base + 1] ^= g
            m[base + 2] ^= b
    
    return bytes(m)


def modify_plen_other(pkt, r, g, b):
    """
    plen=239-245: XOR cipher с (R, G, B).
    
    Color region начинается с байта (12 + extra), где extra = plen - 238.
    """
    m = bytearray(pkt)
    extra = len(pkt) - 238
    color_start = 12 + extra
    
    # Сколько LED влезает
    color_len = len(pkt) - color_start - 1  # -1 for tail
    num_leds = color_len // 3
    
    for led in range(min(num_leds, NUM_LEDS)):
        base = color_start + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] ^= r
            m[base + 1] ^= g
            m[base + 2] ^= b
    
    return bytes(m)


def modify_color_packet(pkt, r, g, b):
    """Модифицирует color packet любого размера."""
    if len(pkt) == 238:
        return modify_plen238(pkt, r, g, b)
    else:
        return modify_plen_other(pkt, r, g, b)
# endregion


# region ===== Replay =====
def replay_with_color(writes, r, g, b, label=""):
    """Replay всех пакетов с модификацией цвета."""
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    
    modified_count = 0
    for w in writes:
        if is_color_packet(w):
            w = modify_color_packet(w, r, g, b)
            modified_count += 1
        
        ser.write(w)
        ser.flush()
        
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("  %s: %d color packets" % (label, modified_count))
    time.sleep(4)
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("RGB MAPPING TEST — black.csv + XOR(R,G,B)")
    print("=" * 60)
    print()
    print("Для black.csv: cipher = keystream (plaintext = 0)")
    print("XOR cipher с (R,G,B) -> получаем целевой цвет")
    print()
    
    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    print("black.csv: %d packets, %d color (238-245)" % (len(writes), n_color))
    print()
    
    # Тестируем порядок каналов
    # Если byte0=R, byte1=G, byte2=B:
    #   (255,0,0) -> КРАСНЫЙ
    #   (0,255,0) -> ЗЕЛЁНЫЙ
    #   (0,0,255) -> СИНИЙ
    # Если другой порядок — цвета будут перепутаны
    
    tests = [
        ("КРАСНЫЙ (R=255,G=0,B=0)", 255, 0, 0),
        ("ЗЕЛЁНЫЙ (R=0,G=255,B=0)", 0, 255, 0),
        ("СИНИЙ (R=0,G=0,B=255)", 0, 0, 255),
        ("БЕЛЫЙ (255,255,255)", 255, 255, 255),
        ("ЖЁЛТЫЙ (255,255,0)", 255, 255, 0),
        ("ЦИАН (0,255,255)", 0, 255, 255),
        ("ПУРПУР (255,0,255)", 255, 0, 255),
    ]
    
    for name, r, g, b in tests:
        print("--- %s ---" % name)
        replay_with_color(writes, r, g, b, name)
    
    print()
    print("=" * 60)
    print("РЕЗУЛЬТАТЫ:")
    print("Если byte0=R, byte1=G, byte2=B — цвета совпадут с названиями")
    print("Если перепутаны — определим правильный порядок:")
    print("  Если (255,0,0) = синий -> byte0=B")
    print("  Если (0,255,0) = красный -> byte1=R")
    print("  и т.д.")


if __name__ == "__main__":
    main()
# endregion
