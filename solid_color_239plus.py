# -*- coding: utf-8 -*-
"""
solid_color_239plus.py — Генерация solid color через модификацию 239-245 пакетов.

Открытие:
- 238-byte пакеты = heartbeat/keepalive (НЕ цвет!)
- 239-245 пакеты = реальные цветовые данные (screen capture)
- Прямой XOR cipher работает (проверено в real_color_test.py)

Метод:
1. Загружаем все пакеты из green.csv
2. Модифицируем 239-245 пакеты: XOR cipher напрямую чтобы получить нужный цвет
3. Отправляем все пакеты (heartbeat + модифицированные color)

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


def is_color_packet(raw):
    """Реальные цветовые пакеты: 239-245 байт."""
    return 239 <= len(raw) <= 245
# endregion


# region ===== Модификация пакетов =====
def modify_color_packet_direct_xor(pkt, r, g, b):
    """
    Модифицирует цвет в 239-245 пакете через прямой XOR cipher.

    Для screen capture данных:
    - Цветовые данные начинаются после header (разный для каждого plen)
    - Каждый LED = 3 байта RGB
    - XOR cipher напрямую меняет цвет

    Метод: находим repeating pattern в cipher (это keystream XOR color)
    и заменяем на keystream XOR new_color.

    Проблема: keystream разный для каждого пакета.
    Решение: XOR-им cipher с разницей между текущим цветом и желаемым.

    Если текущий цвет = (r0, g0, b0) и желаемый = (r, g, b),
    то XOR mask = (r0^r, g0^g, b0^b)

    Но мы не знаем r0, g0, b0 из cipher напрямую...

    Простой метод: XOR весь color region с 0xFF для инверсии.
    Для solid color: нужно знать текущий цвет в plaintext.

    Альтернатива: используем green.csv где цвет = зелёный.
    Для зелёного (0, 255, 0):
      - XOR mask для красного (255, 0, 0) = (255, 255, 0)
      - XOR mask для синего (0, 0, 255) = (0, 255, 255)
      - XOR mask для белого (255, 255, 255) = (255, 0, 255)
    """
    # Для green.csv: исходный цвет = зелёный (0, 255, 0)
    # XOR mask = (0 ^ r, 255 ^ g, 0 ^ b) = (r, 255^g, b)
    mask_r = r
    mask_g = 255 ^ g  # 255 XOR g
    mask_b = b

    m = bytearray(pkt)

    # Определяем начало color data для каждого plen
    # plen=239: extra=1 байт header -> color starts at byte 13
    # plen=240: extra=2 -> color at 14
    # plen=241: extra=3 -> color at 15
    # ...
    # plen=245: extra=7 -> color at 19
    extra = len(pkt) - 238
    color_start = 12 + extra

    # Применяем XOR mask к color region
    for i in range(color_start, len(m) - 1):
        pos_in_triplet = (i - color_start) % 3
        if pos_in_triplet == 0:
            m[i] ^= mask_r
        elif pos_in_triplet == 1:
            m[i] ^= mask_g
        else:
            m[i] ^= mask_b

    return bytes(m)
# endregion


# region ===== Отправка =====
def replay_with_color(writes, r, g, b, label=""):
    """
    Отправляет все пакеты, модифицируя 239-245 на нужный цвет.

    Исходный цвет green.csv = зелёный (0, 255, 0).
    """
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)

    color_count = 0
    for w in writes:
        if is_color_packet(w):
            w = modify_color_packet_direct_xor(w, r, g, b)
            color_count += 1
        ser.write(w)
        ser.flush()
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)

    print("  %s: modified %d color packets" % (label, color_count))
    time.sleep(3)
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("BEELIGHT SOLID COLOR — модификация 239-245 пакетов")
    print("=" * 60)
    print()
    print("Исходный цвет green.csv = ЗЕЛЁНЫЙ (0, 255, 0)")
    print()

    writes = parse_writes(CAPTURE_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    n_238 = sum(1 for w in writes if len(w) == 238)
    n_5 = sum(1 for w in writes if len(w) == 5)

    print("green.csv: %d writes" % len(writes))
    print("  5-byte (heartbeat): %d" % n_5)
    print("  238-byte (sync): %d" % n_238)
    print("  239-245 (color): %d" % n_color)
    print()

    # Тесты цветов (XOR mask рассчитан для исходного зелёного)
    tests = [
        ("КРАСНЫЙ", 255, 0, 0),      # mask = (255, 255^0=255, 0)
        ("ЗЕЛЁНЫЙ", 0, 255, 0),       # mask = (0, 0, 0) = без изменений
        ("СИНИЙ", 0, 0, 255),        # mask = (0, 255, 255)
        ("БЕЛЫЙ", 255, 255, 255),    # mask = (255, 0, 255)
        ("ЖЁЛТЫЙ", 255, 255, 0),     # mask = (255, 0, 0)
    ]

    for name, r, g, b in tests:
        print("--- %s RGB(%d,%d,%d) ---" % (name, r, g, b))
        mask_r = r
        mask_g = 255 ^ g
        mask_b = b
        print("  XOR mask: R=%02x G=%02x B=%02x" % (mask_r, mask_g, mask_b))
        replay_with_color(writes, r, g, b, name)

    print()
    print("=" * 60)
    print("РЕЗУЛЬТАТЫ:")
    for name, r, g, b in tests:
        print("  %s RGB(%d,%d,%d): ___" % (name, r, g, b))


if __name__ == "__main__":
    main()
# endregion
