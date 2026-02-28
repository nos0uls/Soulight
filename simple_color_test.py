# -*- coding: utf-8 -*-
"""
simple_color_test.py — Простой тест модификации цвета.

Используем тот же подход что в working channel_mapping_test.py:
XOR-им cipher напрямую БЕЗ дешифрования.

Для установки solid color: XOR-им каждый байт color region
с соответствующим значением цвета.

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
CSV_PATH = os.path.join(BASE, "green.csv")  # Используем green.csv как в working тесте
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
# endregion


# region ===== Модификация =====
def is_color_packet(raw):
    """Color пакеты: 239-245 байт (как в working тесте)."""
    return 239 <= len(raw) <= 245


def modify_direct_xor(raw, xor0, xor1, xor2):
    """
    Прямой XOR на color region — ФИКСИРОВАННЫЙ color_start=12.
    
    Как в working channel_mapping_test.py:
    - mod-0: байты 12, 15, 18, 21, ... (i-12)%3==0
    - mod-1: байты 13, 16, 19, 22, ... (i-12)%3==1
    - mod-2: байты 14, 17, 20, 23, ... (i-12)%3==2
    """
    m = bytearray(raw)
    
    for i in range(12, len(m) - 1):
        pos = (i - 12) % 3
        if pos == 0:
            m[i] ^= xor0
        elif pos == 1:
            m[i] ^= xor1
        else:
            m[i] ^= xor2
    
    return bytes(m)
# endregion


# region ===== Replay =====
def replay_with_xor(ser, writes, xor0, xor1, xor2, label=""):
    """Replay с прямым XOR на позициях mod-0, mod-1, mod-2."""
    modified = 0
    for w in writes:
        if is_color_packet(w):
            w = modify_direct_xor(w, xor0, xor1, xor2)
            modified += 1
        ser.write(w)
        ser.flush()
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("  %s: XOR(%d,%d,%d) — %d пакетов" % (label, xor0, xor1, xor2, modified))
    time.sleep(3)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("DIRECT XOR TEST — определение маппинга позиций")
    print("=" * 60)
    print()
    
    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    print("green.csv: %d writes, %d color (239-245)" % (len(writes), n_color))
    print()
    
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Порт: %s\n" % COM_PORT)
    
    # Тест 1: XOR 255 только на ОДНОЙ позиции за раз
    # Это покажет какая позиция влияет на какой цветовой канал
    # Исходные данные: [243, 0, 104] (screen capture)
    # XOR с 255: инвертирует значение
    # pos0: 243 ^ 255 = 12 (уменьшится)
    # pos1: 0 ^ 255 = 255 (увеличится до максимума)
    # pos2: 104 ^ 255 = 151 (изменится)
    
    # Исходные данные в green.csv: [243, 0, 104] для большинства LED
    # XOR для установки конкретных значений:
    # pos0: 243 ^ X = target -> X = 243 ^ target
    # pos1: 0 ^ X = target -> X = target
    # pos2: 104 ^ X = target -> X = 104 ^ target
    
    # Гипотеза: формат GRB (pos0=G, pos1=R, pos2=B)
    # КРАСНЫЙ: G=0, R=255, B=0 -> XOR(243^0, 255, 104^0) = XOR(243, 255, 104)
    # ЗЕЛЁНЫЙ: G=255, R=0, B=0 -> XOR(243^255, 0, 104^0) = XOR(12, 0, 104)
    # СИНИЙ:   G=0, R=0, B=255 -> XOR(243^0, 0, 104^255) = XOR(243, 0, 151)
    # БЕЛЫЙ:   G=255, R=255, B=255 -> XOR(12, 255, 151)
    # ЖЁЛТЫЙ:  G=255, R=255, B=0 -> XOR(12, 255, 104)
    
    tests = [
        ("ЧЁРНЫЙ",    243,   0, 104),  # G=0, R=0, B=0
        ("КРАСНЫЙ",   243, 255, 104),  # G=0, R=255, B=0
        ("ЗЕЛЁНЫЙ",    12,   0, 104),  # G=255, R=0, B=0
        ("СИНИЙ",     243,   0, 151),  # G=0, R=0, B=255
        ("БЕЛЫЙ",      12, 255, 151),  # G=255, R=255, B=255
        ("ЖЁЛТЫЙ",     12, 255, 104),  # G=255, R=255, B=0
    ]
    
    print("=== ТЕСТ: Чистые цвета (формат GRB) ===")
    print("pos0=G, pos1=R, pos2=B")
    print()
    
    for label, x0, x1, x2 in tests:
        print("--- %s ---" % label)
        replay_with_xor(ser, writes, x0, x1, x2, label)
    
    ser.close()
    print("\nГотово!")
    print()
    print("РЕЗУЛЬТАТЫ:")
    print("  Контроль: исходный цвет (screen capture)")
    print("  pos0 XOR: какой цвет изменился?")
    print("  pos1 XOR: какой цвет изменился?")
    print("  pos2 XOR: какой цвет изменился?")


if __name__ == "__main__":
    main()
# endregion
