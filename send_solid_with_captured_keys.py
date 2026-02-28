# -*- coding: utf-8 -*-
"""
send_solid_with_captured_keys.py — Отправка solid-color используя key3 из захвата.

Гипотеза: key3 детерминирован PRNG, синхронизированным после handshake.
Если использовать key3 из захваченных пакетов ПО ПОРЯДКУ, контроллер примет.

Метод:
1. Загружаем key3 из green.csv (plen=238)
2. Строим solid-color пакеты с этими key3
3. Отправляем после handshake

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
CAPTURE_PATH = os.path.join(BASE, "green.csv")  # источник key3
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


def extract_key3_from_packet(pkt):
    """
    Извлекает key3 из plen=238 пакета.

    Для plen=238: cipher[i] = plain[i] ^ key3[i % 3]
    plain[2:5] = 0x00 (header padding)
    Поэтому: key3[0] = cipher[3], key3[1] = cipher[4], key3[2] = cipher[2]
    """
    if len(pkt) != 238:
        return None
    k0 = pkt[3]
    k1 = pkt[4]
    k2 = pkt[2]
    return bytes([k0, k1, k2])


def extract_nonce_from_packet(pkt):
    """Извлекает nonce из пакета (первые 2 байта plaintext = первые 2 байта cipher)."""
    if len(pkt) < 2:
        return None
    # nonce не зашифрован: cipher[0:2] = plain[0:2] ^ key3[0:2]
    # Но мы не знаем plain[0:2]... nonce = cipher[0:2] ^ key3[0:2]
    # Если plain[0:2] = nonce, то cipher[0:2] = nonce ^ key3[0:2]
    # Для дешифрования: nonce = cipher[0:2] ^ key3[0:2]
    return bytes([pkt[0], pkt[1]])
# endregion


# region ===== Генерация пакетов =====
def build_solid_packet_with_key(r, g, b, key3, nonce=None):
    """
    Строит wire-пакет (243 байта) для solid color с ЗАДАННЫМ key3.

    Если nonce=None, использует nonce из захвата (cipher[0:2]).
    """
    # Header plaintext (байты 2-11)
    HEADER_FIXED = bytes([0x00, 0x00, 0x00, 0x00, 0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B])
    TAIL = bytes([0x00])
    WIRE_HEADER = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])

    # Plaintext: nonce(2) + header(10) + LED_data(225) + tail(1) = 238
    plain = bytearray()

    # Nonce: если не задан, используем [0x00, 0x32] (стандартный из захвата)
    if nonce is None:
        nonce = bytes([0x00, 0x32])

    plain.extend(nonce)
    plain.extend(HEADER_FIXED)

    # 75 RGB тройки
    for _ in range(NUM_LEDS):
        plain.append(r)
        plain.append(g)
        plain.append(b)

    plain.extend(TAIL)

    # Шифруем: cipher[i] = plain[i] ^ key3[i % 3]
    cipher = bytes([plain[i] ^ key3[i % 3] for i in range(238)])

    return WIRE_HEADER + cipher
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("BEELIGHT SOLID COLOR — с key3 из захвата")
    print("=" * 60)
    print()

    # Загружаем key3 из захвата
    writes = parse_writes(CAPTURE_PATH)
    pkts_238 = [w for w in writes if len(w) == 238]

    print("green.csv: %d writes, %d plen=238" % (len(writes), len(pkts_238)))

    # Извлекаем key3 и nonce из каждого пакета
    keys_data = []
    for p in pkts_238:
        k3 = extract_key3_from_packet(p)
        n = extract_nonce_from_packet(p)
        if k3:
            keys_data.append((k3, n))

    print("Извлечено %d key3" % len(keys_data))
    print()

    # Показываем первые несколько
    print("Первые 10 key3 из захвата:")
    for i, (k3, n) in enumerate(keys_data[:10]):
        print("  [%d] nonce=[%02x %02x] key3=[%02x %02x %02x]" % (
            i, n[0], n[1], k3[0], k3[1], k3[2]))
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
    print("       (Запустить Beelight -> мирроринг -> закрыть)")
    print()

    # Тест: отправляем КРАСНЫЙ используя key3 из захвата ПО ПОРЯДКУ
    print("--- ТЕСТ 1: КРАСНЫЙ RGB(255,0,0) с key3 из green.csv ---")
    print("    Используем %d key3 по порядку" % min(20, len(keys_data)))

    key_idx = 0
    for i in range(min(20, len(keys_data))):
        k3, n = keys_data[key_idx]
        pkt = build_solid_packet_with_key(255, 0, 0, k3, n)
        ser.write(pkt)
        ser.flush()
        key_idx += 1
        time.sleep(0.04)  # ~25 fps

    print("    Отправлено %d пакетов" % min(20, len(keys_data)))
    time.sleep(2)
    print()

    # Тест 2: ЗЕЛЁНЫЙ с теми же key3
    print("--- ТЕСТ 2: ЗЕЛЁНЫЙ RGB(0,255,0) с key3 из green.csv ---")
    key_idx = 0
    for i in range(min(20, len(keys_data))):
        k3, n = keys_data[key_idx]
        pkt = build_solid_packet_with_key(0, 255, 0, k3, n)
        ser.write(pkt)
        ser.flush()
        key_idx += 1
        time.sleep(0.04)

    print("    Отправлено %d пакетов" % min(20, len(keys_data)))
    time.sleep(2)
    print()

    # Тест 3: СИНИЙ
    print("--- ТЕСТ 3: СИНИЙ RGB(0,0,255) с key3 из green.csv ---")
    key_idx = 0
    for i in range(min(20, len(keys_data))):
        k3, n = keys_data[key_idx]
        pkt = build_solid_packet_with_key(0, 0, 255, k3, n)
        ser.write(pkt)
        ser.flush()
        key_idx += 1
        time.sleep(0.04)

    print("    Отправлено %d пакетов" % min(20, len(keys_data)))
    time.sleep(2)
    print()

    ser.close()
    print("Порт закрыт")
    print()
    print("РЕЗУЛЬТАТЫ:")
    print("  ТЕСТ 1 (КРАСНЫЙ): ___")
    print("  ТЕСТ 2 (ЗЕЛЁНЫЙ): ___")
    print("  ТЕСТ 3 (СИНИЙ):   ___")
    print()
    print("Если лента светится — key3 из захвата работает!")
    print("Если нет — нужен другой подход (PRNG seed или replay полных пакетов)")


if __name__ == "__main__":
    main()
# endregion
