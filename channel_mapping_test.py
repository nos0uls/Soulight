# -*- coding: utf-8 -*-
"""
channel_mapping_test.py — Точный маппинг RGB-каналов в wire-пакетах Beelight.

Из предыдущих тестов мы знаем:
  - XOR модификация цветовых данных РАБОТАЕТ (чексуммы нет)
  - Реальные color-пакеты: 239-245 байт (НЕ 238!)
  - Цветовые данные начинаются с байта 12 (или 14), тройками
  - Позиция mod-1 ((i-12)%3==1) — вероятно структурный padding (всегда 0x00 в plaintext)

Этот тест XOR-ит ТОЛЬКО ОДИН канал за раз (mod-0, mod-1, mod-2),
чтобы точно определить: какой mod = R, какой = G, какой = B.

ВАЖНО: Запускать ПОСЛЕ того как Beelight сделал handshake с контроллером!
(Запустить Beelight -> включить мирроринг -> закрыть Beelight -> запустить этот скрипт)

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
CSV_PATH = os.path.join(BASE, "green.csv")
COM_PORT = "COM7"
BAUD = 500000
# endregion


# region ===== Парсинг CSV =====
def parse_writes(filepath):
    """Извлекает все WRITE-операции из CSV-файла захвата (IRPmon/USBPcap)."""
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


# region ===== Определение типа пакета =====
def is_color_packet(raw):
    """Реальные color-пакеты имеют длину 239-245 байт (НЕ 238 — те heartbeat)."""
    return 239 <= len(raw) <= 245
# endregion


# region ===== Модификаторы: по одному каналу =====
def mod_only_mod0(raw):
    """XOR ТОЛЬКО позиции mod-0: байты 12, 15, 18, 21, ...
    Это первый байт каждой тройки (один из R/G/B каналов)."""
    m = bytearray(raw)
    for i in range(12, len(m) - 1):
        if (i - 12) % 3 == 0:
            m[i] ^= 0xFF
    return bytes(m)


def mod_only_mod1(raw):
    """XOR ТОЛЬКО позиции mod-1: байты 13, 16, 19, 22, ...
    Это второй байт каждой тройки (возможно структурный padding)."""
    m = bytearray(raw)
    for i in range(13, len(m) - 1):
        if (i - 12) % 3 == 1:
            m[i] ^= 0xFF
    return bytes(m)


def mod_only_mod2(raw):
    """XOR ТОЛЬКО позиции mod-2: байты 14, 17, 20, 23, ...
    Это третий байт каждой тройки (один из R/G/B каналов)."""
    m = bytearray(raw)
    for i in range(14, len(m) - 1):
        if (i - 12) % 3 == 2:
            m[i] ^= 0xFF
    return bytes(m)
# endregion


# region ===== Отправка пакетов =====
def replay_with_mod(writes, modifier=None, label=""):
    """Replay всех write-операций из захвата. Модификатор применяется к color-пакетам."""
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)  # очищаем буфер

    color_count = 0
    for w in writes:
        # Применяем модификатор только к реальным color-пакетам (239-245 байт)
        if is_color_packet(w) and modifier:
            w = modifier(w)
            color_count += 1
        ser.write(w)
        ser.flush()
        # Тайминг: color/heartbeat пакеты — 33мс, header — 5мс, остальное — 50мс
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)

    print("  %s: модифицировано %d color-пакетов" % (label, color_count))
    time.sleep(3)  # пауза чтобы увидеть результат
    ser.close()
    time.sleep(0.5)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("CHANNEL MAPPING TEST — определение R/G/B позиций")
    print("=" * 60)

    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    n_238 = sum(1 for w in writes if len(w) == 238)
    print("green.csv: %d writes, %d color (239-245), %d heartbeat (238)" % (
        len(writes), n_color, n_238))
    print()

    # Исходный цвет: GREEN => plaintext color data содержит G=0xFF, R=0, B=0
    # XOR 0xFF на канале с G даст G=0x00 => цвет станет BLACK (или R/B если они ненулевые)
    # XOR 0xFF на канале с R даст R=0xFF => добавится красный => ЖЁЛТЫЙ (R+G)
    # XOR 0xFF на канале с B даст B=0xFF => добавится синий => ЦИАН (G+B)
    #
    # Ожидаемые результаты для GREEN исходника:
    #   Инверсия R-канала: зелёный + красный = ЖЁЛТЫЙ
    #   Инверсия G-канала: зелёный исчезает = ЧЁРНЫЙ (или тёмный)
    #   Инверсия B-канала: зелёный + синий = ЦИАН (голубой)
    #   Инверсия padding:  если mod-1 = 0x00, XOR→0xFF = мусор → возможно отклонение контроллером

    tests = [
        ("TEST 0: GREEN original (контроль)", None),
        ("TEST 1: XOR mod-0 only (байты 12,15,18,...)", mod_only_mod0),
        ("TEST 2: XOR mod-1 only (байты 13,16,19,...)", mod_only_mod1),
        ("TEST 3: XOR mod-2 only (байты 14,17,20,...)", mod_only_mod2),
    ]

    for label, modifier in tests:
        print("\n--- %s ---" % label)
        replay_with_mod(writes, modifier, label.split(":")[0].strip())

    print("\n" + "=" * 60)
    print("РЕЗУЛЬТАТЫ — какой цвет показала лента в каждом тесте?")
    print("  0 (оригинал):   ___ (должен быть ЗЕЛЁНЫЙ)")
    print("  1 (mod-0 XOR):  ___ ")
    print("  2 (mod-1 XOR):  ___")
    print("  3 (mod-2 XOR):  ___")
    print()
    print("ИНТЕРПРЕТАЦИЯ (если исходный цвет ЗЕЛЁНЫЙ):")
    print("  ЖЁЛТЫЙ  = инвертировали R-канал (R: 0→255, G остался)")
    print("  ЧЁРНЫЙ  = инвертировали G-канал (G: 255→0)")
    print("  ЦИАН    = инвертировали B-канал (B: 0→255, G остался)")
    print("  МУСОР/ОТКАЗ = инвертировали структурный padding")


if __name__ == "__main__":
    main()
# endregion
