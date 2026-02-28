# -*- coding: utf-8 -*-
"""
send_modified_real.py — Отправка РЕАЛЬНОГО захваченного пакета с модификацией цвета.

Берём реальный plen=238 пакет из захвата, модифицируем color data XOR-ом,
и отправляем. Это должно работать т.к. прошлые XOR-тесты работали.

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
# endregion


# region ===== Модификация пакета =====
def modify_color_in_packet(cipher_238, target_r, target_g, target_b):
    """
    Модифицирует color data в зашифрованном plen=238 пакете.
    
    Зная что plaintext color region (байты 12-236) содержит RGB тройки,
    и что XOR модификация работает, мы можем:
    1. Извлечь key3 из пакета
    2. Вычислить текущий plaintext color
    3. XOR-ить чтобы получить нужный цвет
    
    Формула: new_cipher[i] = old_cipher[i] ^ old_plain[i] ^ new_plain[i]
    Но old_plain = old_cipher ^ key3, поэтому:
    new_cipher[i] = old_cipher[i] ^ (old_cipher[i] ^ key3[i%3]) ^ new_plain[i]
                  = key3[i%3] ^ new_plain[i]
    """
    pkt = bytearray(cipher_238)
    
    # Извлекаем key3
    k0, k1, k2 = pkt[3], pkt[4], pkt[2]
    key3 = [k0, k1, k2]
    
    # Модифицируем color region (байты 12-236 = 225 байт = 75 LEDs * 3)
    for led in range(NUM_LEDS):
        base = 12 + led * 3
        # new_cipher = key3 ^ new_plain
        pkt[base + 0] = key3[(base + 0) % 3] ^ target_r
        pkt[base + 1] = key3[(base + 1) % 3] ^ target_g
        pkt[base + 2] = key3[(base + 2) % 3] ^ target_b
    
    return bytes(pkt)
# endregion


# region ===== Отправка =====
def send_color(ser, header_5, cipher_238, r, g, b, duration=4.0, fps=25):
    """
    Отправляет модифицированный пакет в течение duration секунд.
    """
    modified = modify_color_in_packet(cipher_238, r, g, b)
    
    interval = 1.0 / fps
    start = time.time()
    sent = 0
    
    while time.time() - start < duration:
        # Отправляем header и payload РАЗДЕЛЬНО (как в реальном захвате)
        ser.write(header_5)
        ser.flush()
        ser.write(modified)
        ser.flush()
        sent += 1
        time.sleep(interval)
    
    elapsed = time.time() - start
    print("  RGB(%d,%d,%d): %d кадров за %.1f сек" % (r, g, b, sent, elapsed))
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("BEELIGHT MODIFIED REAL PACKET — тест с реальным пакетом")
    print("=" * 60)
    print()
    
    # Парсим захват
    writes = parse_writes(CSV_PATH)
    print("Загружено %d пакетов из %s" % (len(writes), CSV_PATH))
    
    # Ищем пару [header_5] + [plen=238]
    header_5 = None
    cipher_238 = None
    for i in range(len(writes) - 1):
        if len(writes[i]) == 5 and len(writes[i+1]) == 238:
            header_5 = writes[i]
            cipher_238 = writes[i+1]
            print("Найдена пара: header=%s payload_len=%d" % (
                " ".join("%02x" % b for b in header_5), len(cipher_238)))
            break
    
    if not header_5 or not cipher_238:
        print("ОШИБКА: не найдена пара [5-byte header] + [238-byte payload]")
        return
    
    # Дешифруем для проверки
    k0, k1, k2 = cipher_238[3], cipher_238[4], cipher_238[2]
    key3 = [k0, k1, k2]
    dec = bytes([cipher_238[i] ^ key3[i % 3] for i in range(238)])
    print("key3: [%02x %02x %02x]" % (k0, k1, k2))
    print("Оригинальный plaintext[0:15]: %s" % " ".join("%02x" % b for b in dec[:15]))
    print()
    
    # Открываем порт
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Порт открыт: %s @ %d\n" % (COM_PORT, BAUD))
    
    # Тестовые цвета
    tests = [
        ("КРАСНЫЙ",   255,   0,   0),
        ("ЗЕЛЁНЫЙ",     0, 255,   0),
        ("СИНИЙ",       0,   0, 255),
        ("БЕЛЫЙ",     255, 255, 255),
        ("ЧЁРНЫЙ",      0,   0,   0),
    ]
    
    for name, r, g, b in tests:
        print("--- %s ---" % name)
        send_color(ser, header_5, cipher_238, r, g, b, duration=3.0, fps=25)
        time.sleep(0.5)
    
    ser.close()
    print("\nГотово!")


if __name__ == "__main__":
    main()
# endregion
