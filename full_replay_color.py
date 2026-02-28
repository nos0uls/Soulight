# -*- coding: utf-8 -*-
"""
full_replay_color.py — Полный replay захвата с модификацией цвета.

Этот скрипт делает то же самое что работающий channel_mapping_test.py:
отправляет ВСЕ пакеты из захвата, но модифицирует color data в каждом
color-пакете (plen 238-245).

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
# endregion


# region ===== Модификация для разных plen =====
def is_color_packet(pkt):
    """Пакеты 238-245 байт — color data."""
    return 238 <= len(pkt) <= 245


def modify_plen238(pkt, r, g, b):
    """Модификация plen=238: XOR с key3, color data в байтах 12-236."""
    m = bytearray(pkt)
    k0, k1, k2 = m[3], m[4], m[2]
    key3 = [k0, k1, k2]
    
    for led in range(NUM_LEDS):
        base = 12 + led * 3
        if base + 2 < len(m):
            m[base + 0] = key3[(base + 0) % 3] ^ r
            m[base + 1] = key3[(base + 1) % 3] ^ g
            m[base + 2] = key3[(base + 2) % 3] ^ b
    return bytes(m)


def extract_ks6_plen241(pkt):
    """
    Извлекает 6-byte keystream для plen=241.
    
    Формат: period=6, phase=2
    cipher[i] = plain[i] ^ ks[(i+2) % 6]
    plain[2:9] = 7 zeros -> cipher[2:9] = ks[4], ks[5], ks[0], ks[1], ks[2], ks[3], ks[4]
    """
    ks = [0] * 6
    ks[4] = pkt[2]
    ks[5] = pkt[3]
    ks[0] = pkt[4]
    ks[1] = pkt[5]
    ks[2] = pkt[6]
    ks[3] = pkt[7]
    return ks


def modify_plen241(pkt, r, g, b):
    """
    Модификация plen=241: period=6, phase=2, color data с байта 15.
    
    new_cipher[i] = ks[(i+2) % 6] ^ target_plain[i]
    Для color region: target_plain = [R, G, B, R, G, B, ...]
    """
    m = bytearray(pkt)
    ks = extract_ks6_plen241(pkt)
    
    # Color data: байты 15-239 (225 bytes = 75 LEDs * 3), tail = byte 240
    color_start = 15
    for led in range(NUM_LEDS):
        base = color_start + led * 3
        if base + 2 < len(m) - 1:  # -1 for tail
            m[base + 0] = ks[(base + 2) % 6] ^ r
            m[base + 1] = ks[(base + 1 + 2) % 6] ^ g
            m[base + 2] = ks[(base + 2 + 2) % 6] ^ b
    return bytes(m)


def modify_plen_other(pkt, r, g, b):
    """
    Для plen 239, 240, 242-245: простой XOR подход.
    
    Вместо сложной дешифровки, просто XOR-им color region
    начиная с байта 12 (как в working channel_mapping_test.py).
    
    Для solid color: XOR с (r, g, b) на каждую тройку.
    """
    plen = len(pkt)
    m = bytearray(pkt)
    
    # Color data начинается с байта 12, заканчивается за 1 байт до конца
    color_start = 12
    
    for i in range(color_start, len(m) - 1):
        pos = (i - color_start) % 3
        if pos == 0:
            m[i] ^= r
        elif pos == 1:
            m[i] ^= g
        else:
            m[i] ^= b
    
    return bytes(m)


def modify_color_packet(pkt, r, g, b):
    """Модифицирует color packet любого размера."""
    if len(pkt) == 238:
        return modify_plen238(pkt, r, g, b)
    else:
        return modify_plen_other(pkt, r, g, b)
# endregion


# region ===== Replay =====
def replay_with_color(ser, writes, r, g, b, label=""):
    """Replay всех write с модификацией цвета. ser должен быть уже открыт."""
    modified_count = 0
    for w in writes:
        if is_color_packet(w):
            w = modify_color_packet(w, r, g, b)
            modified_count += 1
        
        ser.write(w)
        ser.flush()
        
        # Timing как в оригинале
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("  %s RGB(%d,%d,%d): %d color-пакетов модифицировано" % (label, r, g, b, modified_count))
    time.sleep(2)  # пауза чтобы увидеть результат
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("FULL REPLAY WITH COLOR — полный replay + модификация цвета")
    print("=" * 60)
    print()
    
    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    n_238 = sum(1 for w in writes if len(w) == 238)
    n_241 = sum(1 for w in writes if len(w) == 241)
    print("Загружено %d пакетов, из них %d color (238-245)" % (len(writes), n_color))
    print("  plen=238: %d, plen=241: %d (модифицируются)" % (n_238, n_241))
    print()
    
    # Открываем порт один раз
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Порт открыт: %s\n" % COM_PORT)
    
    tests = [
        ("КРАСНЫЙ",   255,   0,   0),
        ("ЗЕЛЁНЫЙ",     0, 255,   0),
        ("СИНИЙ",       0,   0, 255),
        ("БЕЛЫЙ",     255, 255, 255),
        ("ЧЁРНЫЙ",      0,   0,   0),
    ]
    
    for name, r, g, b in tests:
        print("--- %s ---" % name)
        replay_with_color(ser, writes, r, g, b, name)
    
    ser.close()
    print("\nГотово!")


if __name__ == "__main__":
    main()
# endregion
