# -*- coding: utf-8 -*-
"""
test_send_color.py — Тестовая отправка произвольного цвета в Beelight LED ленту.

На основе результатов криптоанализа:
  - Cipher: XOR с 3-байтовым ключом, period=3, phase=0
  - Plaintext (238 bytes, solid color):
      [nonce0, nonce1, 0x00, 0x00, 0x00, 0x00, 0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B,
       R, G, B, R, G, B, ... (×75),
       0x00]
  - Nonce: 2 произвольных байта (гипотеза: контроллер не проверяет)
  - Key3: 3 произвольных байта (используются для XOR всего пакета)

Heartbeat: 55 AA 5A [ee-f5] 00 (отправляется перед каждым data-пакетом)

ВНИМАНИЕ: Запускать только с подключённой лентой на COM7!
"""

import sys
import os
import time
import random

# region ===== Настройки =====
COM_PORT = "COM7"
BAUD_RATE = 500000
NUM_LEDS = 75

# Plaintext header для solid color (байты 2-11, фиксированные)
# Из криптоанализа BLACK пакетов:
#   [2]=0x00, [3]=0x00, [4]=0x00, [5]=0x00,
#   [6]=0x05, [7]=0x05, [8]=0xFF, [9]=0xE3, [10]=0x00, [11]=0x4B(=75)
HEADER_FIXED = bytes([0x00, 0x00, 0x00, 0x00, 0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B])
TAIL = bytes([0x00])

# Heartbeat байты (из захватов, byte[3] циклируется через 0xEE-0xF5)
HEARTBEAT_TEMPLATE = bytes([0x55, 0xAA, 0x5A, 0x00, 0x00])
# endregion


# region ===== Генерация пакетов =====
def build_solid_color_packet(r, g, b, key3=None, nonce=None):
    """
    Строит зашифрованный пакет для solid color (все 75 LED одного цвета).

    Параметры:
        r, g, b — значения цвета (0-255)
        key3 — 3 байта ключа XOR (если None — генерируется случайный)
        nonce — 2 байта nonce (если None — генерируется случайный)

    Возвращает: bytes (238 байт, зашифрованный пакет)
    """
    if key3 is None:
        key3 = bytes([random.randint(0, 255) for _ in range(3)])
    if nonce is None:
        nonce = bytes([random.randint(0, 255) for _ in range(2)])

    # Собираем plaintext
    # [nonce0, nonce1] + HEADER_FIXED + [R, G, B] * 75 + [0x00]
    plaintext = bytearray()
    plaintext.extend(nonce)
    plaintext.extend(HEADER_FIXED)
    plaintext.extend(bytes([r, g, b]) * NUM_LEDS)
    plaintext.extend(TAIL)

    assert len(plaintext) == 238, f"Plaintext length {len(plaintext)} != 238"

    # Шифруем: cipher[i] = plain[i] ^ key3[i % 3]
    cipher = bytes([plaintext[i] ^ key3[i % 3] for i in range(len(plaintext))])

    return cipher


def build_heartbeat(counter=None):
    """
    Строит heartbeat пакет (5 байт, не шифруется).
    55 AA 5A [byte3] 00
    byte3 циклируется в диапазоне 0xEE-0xF5.
    """
    if counter is None:
        counter = random.randint(0xEE, 0xF5)
    return bytes([0x55, 0xAA, 0x5A, counter & 0xFF, 0x00])
# endregion


# region ===== Serial I/O =====
def send_color(port, r, g, b, duration=5.0, fps=30):
    """
    Отправляет solid color на ленту в течение duration секунд.

    Параметры:
        port — объект serial.Serial
        r, g, b — цвет (0-255)
        duration — длительность в секундах
        fps — частота отправки кадров
    """
    interval = 1.0 / fps
    hb_counter = 0xEE
    start_time = time.time()
    frames_sent = 0

    print(f"  Отправка цвета RGB({r},{g},{b}) на {duration:.1f} сек @ {fps} fps...")

    while time.time() - start_time < duration:
        # Heartbeat
        hb = build_heartbeat(hb_counter)
        port.write(hb)

        # Data packet
        pkt = build_solid_color_packet(r, g, b)
        port.write(pkt)

        frames_sent += 1
        hb_counter = 0xEE + (frames_sent % 8)

        time.sleep(interval)

    print(f"  Отправлено {frames_sent} кадров за {time.time()-start_time:.1f} сек")
# endregion


# region ===== Main =====
def main():
    sys.stdout.reconfigure(encoding="utf-8")

    print("=" * 60)
    print("BEELIGHT TEST: отправка произвольного цвета")
    print("=" * 60)
    print()
    print(f"COM порт: {COM_PORT}")
    print(f"Скорость: {BAUD_RATE}")
    print(f"LED: {NUM_LEDS}")
    print()

    # Проверяем, установлен ли pyserial
    try:
        import serial
    except ImportError:
        print("ОШИБКА: pyserial не установлен!")
        print("Запустите: pip install pyserial")
        sys.exit(1)

    # Показываем пример пакета (без отправки)
    print("--- Пример пакетов ---")
    hb = build_heartbeat(0xEE)
    print(f"Heartbeat: {' '.join(f'{b:02x}' for b in hb)}")

    key3 = bytes([0xAA, 0xBB, 0xCC])
    nonce = bytes([0x42, 0x69])
    pkt = build_solid_color_packet(255, 0, 0, key3=key3, nonce=nonce)
    print(f"Red packet (first 30): {' '.join(f'{b:02x}' for b in pkt[:30])}")
    print(f"Red packet (last 5):   {' '.join(f'{b:02x}' for b in pkt[-5:])}")
    print(f"Packet length: {len(pkt)}")

    # Проверяем: расшифровываем обратно
    plain_check = bytes([pkt[i] ^ key3[i % 3] for i in range(len(pkt))])
    print(f"\nDecrypted header:  {' '.join(f'{b:02x}' for b in plain_check[:14])}")
    print(f"Decrypted LED[0]:  {' '.join(f'{b:02x}' for b in plain_check[12:15])}")
    print(f"Decrypted tail:    {' '.join(f'{b:02x}' for b in plain_check[-1:])}")
    print()

    # Спрашиваем пользователя
    print("Тестовая последовательность:")
    print("  1. КРАСНЫЙ  (5 сек)")
    print("  2. ЗЕЛЁНЫЙ  (5 сек)")
    print("  3. СИНИЙ    (5 сек)")
    print("  4. БЕЛЫЙ    (5 сек)")
    print()

    answer = input("Подключена ли лента к COM7? Отправить? (y/n): ").strip().lower()
    if answer != "y":
        print("Отменено.")
        return

    # Подключаемся и отправляем
    print(f"\nОткрываем {COM_PORT} @ {BAUD_RATE}...")
    try:
        port = serial.Serial(
            port=COM_PORT,
            baudrate=BAUD_RATE,
            timeout=1,
            write_timeout=1,
        )
    except serial.SerialException as e:
        print(f"ОШИБКА подключения: {e}")
        sys.exit(1)

    print("Подключено!")
    print()

    # DTR toggle (как в оригинальном приложении)
    port.dtr = False
    time.sleep(0.1)
    port.dtr = True
    time.sleep(0.5)

    try:
        colors = [
            ("КРАСНЫЙ", 255, 0, 0),
            ("ЗЕЛЁНЫЙ", 0, 255, 0),
            ("СИНИЙ", 0, 0, 255),
            ("БЕЛЫЙ", 255, 255, 255),
        ]

        for name, r, g, b in colors:
            print(f"\n>>> {name} <<<")
            send_color(port, r, g, b, duration=5.0, fps=30)

        print("\n\nТест завершён!")

    except KeyboardInterrupt:
        print("\nПрервано пользователем.")
    finally:
        port.close()
        print("Порт закрыт.")
# endregion


if __name__ == "__main__":
    main()
