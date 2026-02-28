# -*- coding: utf-8 -*-
"""
simple_replay.py — Простой replay без модификации.

Проверяем работает ли вообще отправка захваченных пакетов.
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


def main():
    print("=" * 60)
    print("SIMPLE REPLAY — без модификации")
    print("=" * 60)
    print()
    
    writes = parse_writes(CSV_PATH)
    print("green.csv: %d packets" % len(writes))
    
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Порт открыт\n")
    
    print("Отправка пакетов...")
    for w in writes:
        ser.write(w)
        ser.flush()
        
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("Отправлено %d packets" % len(writes))
    time.sleep(5)
    ser.close()
    print("Готово!")


if __name__ == "__main__":
    main()
