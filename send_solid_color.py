# -*- coding: utf-8 -*-
"""
send_solid_color.py — Отправка произвольного solid-color на Beelight LED ленту.

Формат plen=238 (подтверждено криптоанализом):
  - Шифрование: XOR с 3-байтовым ключом key3, period=3, phase=0
  - cipher[i] = plaintext[i] ^ key3[i % 3]
  - Plaintext (238 байт):
      [nonce0, nonce1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B,
       R0,G0,B0, R1,G1,B1, ... R74,G74,B74,
       00]
    где 4B = 75 LEDs, E3 = 227 (оставшихся байт после header)
  - Nonce: 2 байта (plaintext[0:2]), произвольные
  - key3: 3 случайных байта, новый для каждого пакета
  - Wire format: [55 AA 5A EE 00] (5-byte header) + [238 bytes cipher]

ВАЖНО: Контроллер должен быть уже в PC-режиме (handshake через Beelight).
Запустить Beelight -> включить мирроринг экрана -> закрыть Beelight -> запустить скрипт.

COM7 @ 500000 baud.
"""
import sys
import os
import time
import random

sys.stdout.reconfigure(encoding="utf-8")

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

# region ===== Настройки =====
COM_PORT = "COM7"
BAUD = 500000
NUM_LEDS = 75

# Фиксированная часть plaintext header (байты 2-11)
# [00,00,00,00] — padding, [05,05] — команда, [FF,E3] — яркость/длина, [00,4B] — LED count
HEADER_FIXED = bytes([0x00, 0x00, 0x00, 0x00, 0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B])
TAIL = bytes([0x00])
# Wire frame header для plen=238 (0xEE = 238)
WIRE_HEADER = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])
# endregion


# region ===== Генерация пакетов =====
def build_solid_packet(r, g, b):
    """
    Строит wire-пакет (243 байта) для solid color на все 75 LED.

    Возвращает: bytes (5 wire header + 238 cipher)
    """
    # Случайный ключ и nonce для каждого пакета
    key3 = bytes([random.randint(0, 255) for _ in range(3)])
    nonce = bytes([random.randint(0, 255), random.randint(0, 255)])

    # Plaintext: nonce(2) + header(10) + LED_data(225) + tail(1) = 238
    plain = bytearray()
    plain.extend(nonce)         # байты 0-1: nonce
    plain.extend(HEADER_FIXED)  # байты 2-11: фиксированный header
    # байты 12-236: RGB тройки для 75 LED
    for _ in range(NUM_LEDS):
        plain.append(r)
        plain.append(g)
        plain.append(b)
    plain.extend(TAIL)          # байт 237: tail

    assert len(plain) == 238, "Plaintext length %d != 238" % len(plain)

    # Шифруем: cipher[i] = plain[i] ^ key3[i % 3]
    cipher = bytes([plain[i] ^ key3[i % 3] for i in range(238)])

    return WIRE_HEADER + cipher


def build_per_led_packet(led_colors):
    """
    Строит wire-пакет для индивидуальных цветов на каждый LED.

    led_colors: список из 75 кортежей (R, G, B)
    Возвращает: bytes (5 wire header + 238 cipher)
    """
    key3 = bytes([random.randint(0, 255) for _ in range(3)])
    nonce = bytes([random.randint(0, 255), random.randint(0, 255)])

    plain = bytearray()
    plain.extend(nonce)
    plain.extend(HEADER_FIXED)
    for r, g, b in led_colors:
        plain.append(r & 0xFF)
        plain.append(g & 0xFF)
        plain.append(b & 0xFF)
    plain.extend(TAIL)

    assert len(plain) == 238, "Plaintext length %d != 238" % len(plain)

    cipher = bytes([plain[i] ^ key3[i % 3] for i in range(238)])
    return WIRE_HEADER + cipher
# endregion


# region ===== Отправка =====
def send_packet(ser, r, g, b):
    """
    Отправляет один кадр цвета на ленту.
    
    ВАЖНО: wire header и payload отправляются РАЗДЕЛЬНЫМИ write операциями!
    Это критично — контроллер ожидает именно такой формат.
    """
    # Сначала отправляем wire header (5 байт)
    ser.write(WIRE_HEADER)
    ser.flush()
    
    # Затем отправляем cipher payload (238 байт)
    key3 = bytes([random.randint(0, 255) for _ in range(3)])
    nonce = bytes([random.randint(0, 255), random.randint(0, 255)])
    
    plain = bytearray()
    plain.extend(nonce)
    plain.extend(HEADER_FIXED)
    for _ in range(NUM_LEDS):
        plain.append(r)
        plain.append(g)
        plain.append(b)
    plain.extend(TAIL)
    
    cipher = bytes([plain[i] ^ key3[i % 3] for i in range(238)])
    ser.write(cipher)
    ser.flush()


def send_solid_color(ser, r, g, b, duration=5.0, fps=30):
    """
    Непрерывно отправляет solid color в течение duration секунд.

    Параметры:
        ser — открытый serial.Serial
        r, g, b — цвет (0-255)
        duration — длительность в секундах
        fps — частота отправки (кадров в секунду)
    """
    interval = 1.0 / fps
    start = time.time()
    sent = 0

    while time.time() - start < duration:
        send_packet(ser, r, g, b)
        sent += 1
        time.sleep(interval)

    elapsed = time.time() - start
    print("  RGB(%d,%d,%d): %d кадров за %.1f сек (%.1f fps)" % (
        r, g, b, sent, elapsed, sent / elapsed if elapsed > 0 else 0))
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("BEELIGHT SOLID COLOR — генерация произвольного цвета")
    print("=" * 60)
    print()
    print("COM: %s @ %d baud" % (COM_PORT, BAUD))
    print("LEDs: %d" % NUM_LEDS)
    print("Формат: plen=238, 3-byte XOR, RGB тройки")
    print()

    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)  # очищаем буфер
    print("Порт открыт\n")

    # Последовательность тестовых цветов
    tests = [
        ("КРАСНЫЙ",  255,   0,   0),
        ("ЗЕЛЁНЫЙ",    0, 255,   0),
        ("СИНИЙ",      0,   0, 255),
        ("БЕЛЫЙ",    255, 255, 255),
        ("ЖЁЛТЫЙ",   255, 255,   0),
        ("ЦИАН",       0, 255, 255),
        ("ПУРПУРНЫЙ",255,   0, 255),
        ("ЧЁРНЫЙ",     0,   0,   0),
    ]

    for name, r, g, b in tests:
        print("--- %s RGB(%d,%d,%d) ---" % (name, r, g, b))
        send_solid_color(ser, r, g, b, duration=4.0, fps=25)
        time.sleep(1.0)

    ser.close()
    print("\nГотово! Порт закрыт.")
    print()
    print("РЕЗУЛЬТАТЫ — какой цвет показала лента?")
    for name, r, g, b in tests:
        print("  %s RGB(%d,%d,%d): ___" % (name, r, g, b))


if __name__ == "__main__":
    main()
# endregion
