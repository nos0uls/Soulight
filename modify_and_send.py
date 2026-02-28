# -*- coding: utf-8 -*-
"""
modify_and_send.py — Модификация захваченных пакетов для произвольного цвета.

Метод (проверенный в прошлых тестах):
1. Загружаем захваченные пакеты из green.csv
2. Дешифруем с key3 (извлекаем из пакета)
3. Заменяем color data на нужный solid color
4. Зашифровываем обратно с ТЕМ ЖЕ key3
5. Отправляем

Этот метод работает потому что key3 остаётся валидным для контроллера.

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
    """
    Дешифрует plen=238 пакет.

    Возвращает: (plaintext, key3)
    """
    if len(pkt) != 238:
        return None, None

    # Извлекаем key3: cipher[2:5] = key3[2], key3[0], key3[1] (потому что plain[2:5] = 0)
    k0, k1, k2 = pkt[3], pkt[4], pkt[2]
    key3 = bytes([k0, k1, k2])

    # Дешифруем: plain[i] = cipher[i] ^ key3[i % 3]
    plain = bytes([pkt[i] ^ key3[i % 3] for i in range(238)])

    return plain, key3


def encrypt_packet_238(plain, key3):
    """
    Зашифровывает plaintext с заданным key3.

    Возвращает: cipher (238 байт)
    """
    cipher = bytes([plain[i] ^ key3[i % 3] for i in range(238)])
    return cipher


def modify_color_in_plaintext(plain, r, g, b, start_pos=12):
    """
    Заменяет color data в plaintext на solid color.

    plaintext structure:
      [0:2]   - nonce
      [2:12]  - header (00 00 00 00 05 05 FF E3 00 4B)
      [12:237] - 75 RGB triplets (225 bytes)
      [237]   - tail (0x00)
    """
    modified = bytearray(plain)

    # Заменяем все 75 LED на один цвет
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
    print("BEELIGHT — Модификация захваченных пакетов")
    print("=" * 60)
    print()

    # Загружаем пакеты
    writes = parse_writes(CAPTURE_PATH)
    pkts_238 = [w for w in writes if len(w) == 238]

    print("green.csv: %d writes, %d plen=238" % (len(writes), len(pkts_238)))

    if not pkts_238:
        print("ОШИБКА: нет plen=238 пакетов в захвате!")
        return

    print()

    # Дешифруем первый пакет для проверки
    plain0, key3_0 = decrypt_packet_238(pkts_238[0])
    print("Первый пакет:")
    print("  key3 = [%02x %02x %02x]" % (key3_0[0], key3_0[1], key3_0[2]))
    print("  plaintext header: %s" % ' '.join('%02x' % b for b in plain0[:12]))
    print("  plaintext color[0:3]: %s" % ' '.join('%02x' % b for b in plain0[12:15]))
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

    # Wire header для plen=238
    WIRE_HEADER = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])

    # Тесты: разные цвета
    tests = [
        ("КРАСНЫЙ", 255, 0, 0),
        ("ЗЕЛЁНЫЙ", 0, 255, 0),
        ("СИНИЙ", 0, 0, 255),
        ("БЕЛЫЙ", 255, 255, 255),
        ("ЖЁЛТЫЙ", 255, 255, 0),
    ]

    for name, r, g, b in tests:
        print("--- %s RGB(%d,%d,%d) ---" % (name, r, g, b))

        # Используем все захваченные пакеты, модифицируя цвет
        sent = 0
        for pkt in pkts_238:
            # Дешифруем
            plain, key3 = decrypt_packet_238(pkt)
            if plain is None:
                continue

            # Модифицируем цвет
            modified = modify_color_in_plaintext(plain, r, g, b)

            # Зашифровываем обратно с ТЕМ ЖЕ key3
            cipher = encrypt_packet_238(modified, key3)

            # Отправляем с wire header
            ser.write(WIRE_HEADER + cipher)
            ser.flush()
            sent += 1
            time.sleep(0.04)

        print("  Отправлено %d пакетов" % sent)
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
