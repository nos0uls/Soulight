# -*- coding: utf-8 -*-
"""
test_238_only.py — Модификация ТОЛЬКО plen=238 пакетов.

Проверяем работает ли модификация для plen=238 (метод проверен).
Остальные пакеты (239-245) отправляем без изменений.
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

BASE = os.path.dirname(os.path.abspath(__file__))
CSV_PATH = os.path.join(BASE, "green.csv")
COM_PORT = "COM7"
BAUD = 500000
NUM_LEDS = 75


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


def modify_plen238(pkt, r, g, b):
    """
    Модификация plen=238: извлекаем key3 и применяем к color data.
    
    key3 извлекается из cipher[2:5] где plaintext = 0.
    new_cipher[i] = key3[i % 3] ^ target_plain[i]
    """
    m = bytearray(pkt)
    k0, k1, k2 = m[3], m[4], m[2]
    key3 = [k0, k1, k2]
    
    # Color data: байты 12-236
    for led in range(NUM_LEDS):
        base = 12 + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] = key3[(base + 0) % 3] ^ r
            m[base + 1] = key3[(base + 1) % 3] ^ g
            m[base + 2] = key3[(base + 2) % 3] ^ b
    
    return bytes(m)


def main():
    print("=" * 60)
    print("TEST 238 ONLY — модификация только plen=238")
    print("=" * 60)
    print()
    
    writes = parse_writes(CSV_PATH)
    pkts_238 = [w for w in writes if len(w) == 238]
    pkts_other = [w for w in writes if 239 <= len(w) <= 245]
    
    print("green.csv: %d total" % len(writes))
    print("  plen=238: %d (будут модифицированы)" % len(pkts_238))
    print("  plen=239-245: %d (без изменений)" % len(pkts_other))
    print()
    
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Порт открыт\n")
    
    tests = [
        ("КРАСНЫЙ", 255, 0, 0),
        ("ЗЕЛЁНЫЙ", 0, 255, 0),
        ("СИНИЙ", 0, 0, 255),
        ("БЕЛЫЙ", 255, 255, 255),
    ]
    
    for name, r, g, b in tests:
        print("--- %s ---" % name)
        
        for w in writes:
            if len(w) == 238:
                w = modify_plen238(w, r, g, b)
            
            ser.write(w)
            ser.flush()
            
            if len(w) >= 238:
                time.sleep(0.033)
            elif len(w) == 5:
                time.sleep(0.005)
            else:
                time.sleep(0.05)
        
        print("  отправлено")
        time.sleep(4)
    
    ser.close()
    print("\nГотово!")


if __name__ == "__main__":
    main()
