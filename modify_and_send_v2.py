# -*- coding: utf-8 -*-
"""
modify_and_send_v2.py — Модификация захваченных пакетов БЕЗ лишнего wire header.

Ключевое исправление: пакеты 238-245 в захвате отправляются НАПРЯМУЮ,
без wire header (55 AA 5A). Wire header есть только у 5-байтных heartbeat.

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
CAPTURE_PATH = os.path.join(BASE, "green.csv")
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


# region ===== Криптография plen=238 =====
def decrypt_packet_238(pkt):
    """Дешифрует plen=238 пакет. Возвращает (plaintext, key3)."""
    if len(pkt) != 238:
        return None, None

    # key3: cipher[2:5] = key3[2], key3[0], key3[1] (потому что plain[2:5] = 0)
    k0, k1, k2 = pkt[3], pkt[4], pkt[2]
    key3 = bytes([k0, k1, k2])

    # Дешифруем
    plain = bytes([pkt[i] ^ key3[i % 3] for i in range(238)])
    return plain, key3


def encrypt_packet_238(plain, key3):
    """Зашифровывает plaintext с key3."""
    return bytes([plain[i] ^ key3[i % 3] for i in range(238)])


def modify_color_in_plaintext(plain, r, g, b, start_pos=12):
    """Заменяет color data на solid color."""
    modified = bytearray(plain)
    for led in range(NUM_LEDS):
        pos = start_pos + led * 3
        modified[pos] = r
        modified[pos + 1] = g
        modified[pos + 2] = b
    return bytes(modified)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("BEELIGHT — Модификация пакетов v2 (БЕЗ wire header)")
    print("=" * 60)
    print()

    # Загружаем пакеты
    writes = parse_writes(CAPTURE_PATH)

    # Разделяем по типам
    pkts_5 = [w for w in writes if len(w) == 5]      # heartbeat
    pkts_238 = [w for w in writes if len(w) == 238]  # color

    print("green.csv: %d writes" % len(writes))
    print("  5-byte (heartbeat): %d" % len(pkts_5))
    print("  238-byte (color): %d" % len(pkts_238))
    print()

    if not pkts_238:
        print("ОШИБКА: нет plen=238 пакетов!")
        return

    # Проверяем формат
    print("Проверка формата:")
    print("  5-byte packet: %s" % ' '.join('%02x' % b for b in pkts_5[0]))
    print("  238-byte packet[0:10]: %s" % ' '.join('%02x' % b for b in pkts_238[0][:10]))
    print()

    # Открываем порт
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Порт открыт: %s @ %d" % (COM_PORT, BAUD))
    print()
    print("ВАЖНО: Handshake должен быть уже сделан через Beelight!")
    print()

    # Тесты цветов
    tests = [
        ("КРАСНЫЙ", 255, 0, 0),
        ("ЗЕЛЁНЫЙ", 0, 255, 0),
        ("СИНИЙ", 0, 0, 255),
        ("БЕЛЫЙ", 255, 255, 255),
        ("ЖЁЛТЫЙ", 255, 255, 0),
    ]

    for name, r, g, b in tests:
        print("--- %s RGB(%d,%d,%d) ---" % (name, r, g, b))

        # Модифицируем и отправляем все 238-byte пакеты
        sent = 0
        for pkt in pkts_238:
            plain, key3 = decrypt_packet_238(pkt)
            if plain is None:
                continue

            modified = modify_color_in_plaintext(plain, r, g, b)
            cipher = encrypt_packet_238(modified, key3)

            # ОТПРАВЛЯЕМ НАПРЯМУЮ, БЕЗ WIRE HEADER!
            ser.write(cipher)
            ser.flush()
            sent += 1
            time.sleep(0.04)

        print("  Отправлено %d пакетов (напрямую, без wire header)" % sent)
        time.sleep(3)

    ser.close()
    print()
    print("Порт закрыт")
    print()
    print("РЕЗУЛЬТАТЫ:")
    for name, r, g, b in tests:
        print("  %s RGB(%d,%d,%d): ___" % (name, r, g, b))


if __name__ == "__main__":
    main()
# endregion
