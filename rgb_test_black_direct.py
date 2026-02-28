# -*- coding: utf-8 -*-
"""
rgb_test_black_direct.py — Тест RGB маппинга через прямой XOR cipher.

Используем black.csv и метод из full_replay_color.py который РАБОТАЛ.

Для black.csv: plaintext в color region ≈ 0 (или известен).
XOR cipher с (R, G, B) напрямую даёт целевой цвет.

Модифицируем ВСЕ пакеты 238-245 (не только 239-245).

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
    """ВСЕ пакеты 238-245."""
    return 238 <= len(pkt) <= 245
# endregion


# region ===== Модификация (как в full_replay_color.py) =====
def modify_plen238(pkt, r, g, b):
    """XOR cipher напрямую с (R, G, B) для plen=238."""
    m = bytearray(pkt)
    
    # Color data: байты 12-236
    for led in range(NUM_LEDS):
        base = 12 + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] ^= r
            m[base + 1] ^= g
            m[base + 2] ^= b
    
    return bytes(m)


def modify_plen_other(pkt, r, g, b):
    """XOR cipher напрямую для plen=239-245."""
    m = bytearray(pkt)
    plen = len(pkt)
    
    # Color start по формуле
    color_start = plen - 226
    color_len = plen - color_start - 1
    num_leds = color_len // 3
    
    for led in range(min(num_leds, NUM_LEDS)):
        base = color_start + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] ^= r
            m[base + 1] ^= g
            m[base + 2] ^= b
    
    return bytes(m)


def modify_color_packet(pkt, r, g, b):
    """Модифицирует любой color packet."""
    if len(pkt) == 238:
        return modify_plen238(pkt, r, g, b)
    else:
        return modify_plen_other(pkt, r, g, b)
# endregion


# region ===== Replay =====
def replay_with_color(writes, r, g, b, label=""):
    """Replay с модификацией цвета."""
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    
    mod_count = 0
    for w in writes:
        if is_color_packet(w):
            w = modify_color_packet(w, r, g, b)
            mod_count += 1
        
        ser.write(w)
        ser.flush()
        
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("  %s: %d packets" % (label, mod_count))
    time.sleep(4)
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("RGB TEST — black.csv + прямой XOR(R,G,B)")
    print("=" * 60)
    print()
    
    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    print("black.csv: %d packets, %d color (238-245)" % (len(writes), n_color))
    print()
    
    # Тестируем разные цвета
    tests = [
        ("КРАСНЫЙ (255,0,0)", 255, 0, 0),
        ("ЗЕЛЁНЫЙ (0,255,0)", 0, 255, 0),
        ("СИНИЙ (0,0,255)", 0, 0, 255),
        ("БЕЛЫЙ (255,255,255)", 255, 255, 255),
    ]
    
    for name, r, g, b in tests:
        print("--- %s ---" % name)
        replay_with_color(writes, r, g, b, name)
    
    print()
    print("=" * 60)
    print("РЕЗУЛЬТАТЫ:")
    print("  КРАСНЫЙ: ___")
    print("  ЗЕЛЁНЫЙ: ___")
    print("  СИНИЙ:   ___")
    print("  БЕЛЫЙ:   ___")
    print()
    print("Если byte0=R, byte1=G, byte2=B — цвета совпадут с названиями")


if __name__ == "__main__":
    main()
# endregion
