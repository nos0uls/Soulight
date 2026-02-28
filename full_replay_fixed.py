# -*- coding: utf-8 -*-
"""
full_replay_fixed.py — Полный replay с правильной модификацией ВСЕХ plen.

Исправление: modify_plen_other теперь реально модифицирует пакеты,
используя keystream извлечённый из нулей в header.

Для plen=239-245:
- Header содержит N нулей (N зависит от plen)
- cipher[zero_pos] = keystream[zero_pos % period]
- Извлекаем keystream и применяем к color data

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
NUM_LEDS = 75
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


# region ===== Модификация пакетов =====
def is_color_packet(pkt):
    """Пакеты 238-245 байт — color data."""
    return 238 <= len(pkt) <= 245


def modify_plen238(pkt, r, g, b):
    """
    Модификация plen=238: period=3, phase=0.
    
    Извлекаем key3 из cipher[2:5] (где plaintext = 0).
    new_cipher[i] = key3[i % 3] ^ target_plain[i]
    """
    m = bytearray(pkt)
    k0, k1, k2 = m[3], m[4], m[2]
    key3 = [k0, k1, k2]
    
    # Color data: байты 12-236
    for led in range(NUM_LEDS):
        base = 12 + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] = key3[(base + 0) % 3] ^ r
            m[base + 1] = key3[(base + 1) % 3] ^ g
            m[base + 2] = key3[(base + 2) % 3] ^ b
    
    return bytes(m)


def extract_keystream_from_zeros(pkt, period, zero_start, zero_count):
    """
    Извлекает keystream из нулевых байт header.
    
    cipher[i] = plain[i] ^ ks[(i + phase) % period]
    Если plain[i] = 0, то cipher[i] = ks[(i + phase) % period]
    
    Возвращает: (ks_list, phase)
    """
    # Собираем keystream байты из нулей
    ks_bytes = {}
    for i in range(zero_start, zero_start + zero_count):
        if i < len(pkt):
            pos_in_period = i % period
            if pos_in_period not in ks_bytes:
                ks_bytes[pos_in_period] = pkt[i]
    
    # Преобразуем в список
    ks = [0] * period
    for pos, val in ks_bytes.items():
        ks[pos] = val
    
    return ks


def modify_plen_with_period(pkt, r, g, b, period, color_start):
    """
    Общая функция модификации для любого plen с известным period.
    
    Извлекает keystream из header (нули в байтах 2-8)
    и применяет к color data.
    """
    m = bytearray(pkt)
    plen = len(pkt)
    
    # Извлекаем keystream из нулей в header (bytes 2-8)
    # Для plen>238: header длиннее, но нули всё равно там
    ks = extract_keystream_from_zeros(pkt, period, 2, 7)
    
    # Color data
    color_len = plen - color_start - 1  # -1 for tail
    num_leds = min(color_len // 3, NUM_LEDS)
    
    for led in range(num_leds):
        base = color_start + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] = ks[base % period] ^ r
            m[base + 1] = ks[(base + 1) % period] ^ g
            m[base + 2] = ks[(base + 2) % period] ^ b
    
    return bytes(m)


def get_period_for_plen(plen):
    """
    Возвращает period для данного plen.
    
    Эмпирические данные из анализа:
    - plen=238: period=3
    - plen=239: period=12 (найдено в black.csv)
    - plen=240: неизвестен (пробуем 15)
    - plen=241: period=6
    - plen=242-245: неизвестены
    """
    if plen == 238:
        return 3
    elif plen == 239:
        return 12
    elif plen == 240:
        return 15  # guess
    elif plen == 241:
        return 6
    else:
        # Для неизвестных plen пробуем period=plen-238+3
        return plen - 235


def get_color_start_for_plen(plen):
    """
    Возвращает начало color data для данного plen.
    
    Формула: color_start = plen - 226
    """
    return plen - 226


def modify_color_packet(pkt, r, g, b):
    """Модифицирует любой color packet."""
    plen = len(pkt)
    
    if plen == 238:
        return modify_plen238(pkt, r, g, b)
    else:
        period = get_period_for_plen(plen)
        color_start = get_color_start_for_plen(plen)
        return modify_plen_with_period(pkt, r, g, b, period, color_start)
# endregion


# region ===== Replay =====
def replay_with_color(ser, writes, r, g, b, label=""):
    """Replay с модификацией цвета. ser должен быть открыт."""
    mod_count = 0
    for w in writes:
        if is_color_packet(w):
            w = modify_color_packet(w, r, g, b)
            mod_count += 1
        
        ser.write(w)
        ser.flush()
        
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("  %s: %d color packets" % (label, mod_count))
    time.sleep(4)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("FULL REPLAY FIXED — модификация ВСЕХ plen")
    print("=" * 60)
    print()
    
    writes = parse_writes(CSV_PATH)
    
    # Статистика по plen
    plen_counts = {}
    for w in writes:
        plen = len(w)
        plen_counts[plen] = plen_counts.get(plen, 0) + 1
    
    print("Статистика по plen:")
    for plen in sorted(plen_counts.keys()):
        if plen >= 238 and plen <= 245:
            period = get_period_for_plen(plen)
            print("  plen=%d: %d packets, period=%d" % (plen, plen_counts[plen], period))
    print()
    
    # Открываем порт один раз
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Порт открыт: %s\n" % COM_PORT)
    
    tests = [
        ("КРАСНЫЙ", 255, 0, 0),
        ("ЗЕЛЁНЫЙ", 0, 255, 0),
        ("СИНИЙ", 0, 0, 255),
        ("БЕЛЫЙ", 255, 255, 255),
    ]
    
    for name, r, g, b in tests:
        print("--- %s ---" % name)
        replay_with_color(ser, writes, r, g, b, name)
    
    ser.close()
    print("\nГотово!")


if __name__ == "__main__":
    main()
# endregion
