# -*- coding: utf-8 -*-
"""
correct_color_mod.py — Правильная модификация цвета для ВСЕХ plen.

Открытие из криптоанализа:
- plen=238: period=3, phase=0, key3 извлекается из cipher[2:5]
- plen=239: period=12
- plen=240: period=15
- plen=241: period=6, phase=2, keystream извлекается из cipher[2:8]
- plen=242-245: period=7-10

Метод для plen=241 (проверенный):
  ks[4]=cipher[2], ks[5]=cipher[3], ks[0]=cipher[4], ks[1]=cipher[5], ks[2]=cipher[6], ks[3]=cipher[7]
  dec[i] = cipher[i] ^ ks[(i+2) % 6]
  color starts at byte 15

Для модификации:
  new_cipher[i] = ks[(i+phase) % period] ^ target_color[i % 3]

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


# region ===== Параметры шифрования по plen =====
def get_crypto_params(plen):
    """
    Возвращает (period, phase, color_start) для данного plen.
    
    Из анализа:
    - plen=238: period=3, phase=0, color_start=12
    - plen=239: period=12, phase=?, color_start=13
    - plen=240: period=15, phase=?, color_start=14
    - plen=241: period=6, phase=2, color_start=15
    - plen=242-245: period=7-10, phase=?, color_start=16-19
    
    Для plen=241 проверено что phase=2 работает.
    Для других plen нужно определить phase.
    """
    if plen == 238:
        return (3, 0, 12)
    elif plen == 239:
        return (12, 0, 13)  # пробуем phase=0
    elif plen == 240:
        return (15, 0, 14)  # пробуем phase=0
    elif plen == 241:
        return (6, 2, 15)   # проверено!
    elif plen == 242:
        return (7, 0, 16)   # пробуем phase=0
    elif plen == 243:
        return (8, 0, 17)   # пробуем phase=0
    elif plen == 244:
        return (9, 0, 18)   # пробуем phase=0
    elif plen == 245:
        return (10, 0, 19)  # пробуем phase=0
    else:
        return (3, 0, 12)   # fallback
# endregion


# region ===== Извлечение keystream =====
def extract_keystream_238(pkt):
    """
    Извлекает key3 для plen=238.
    
    plain[2:5] = 0x00 -> cipher[2:5] = key3[2], key3[0], key3[1]
    Поэтому: key3 = [cipher[3], cipher[4], cipher[2]]
    """
    return [pkt[3], pkt[4], pkt[2]]


def extract_keystream_with_phase(pkt, period, phase):
    """
    Извлекает keystream из header (нули в байтах 2-8).
    
    cipher[i] = plain[i] ^ ks[(i + phase) % period]
    Если plain[i] = 0, то cipher[i] = ks[(i + phase) % period]
    
    Для bytes 2-8 (7 нулей):
      cipher[2] = ks[(2 + phase) % period]
      cipher[3] = ks[(3 + phase) % period]
      ...
      cipher[8] = ks[(8 + phase) % period]
    
    Возвращает список length=period.
    """
    ks = [0] * period
    
    # Извлекаем из нулей в header (bytes 2-8)
    for i in range(2, 9):
        if i < len(pkt):
            pos_in_ks = (i + phase) % period
            ks[pos_in_ks] = pkt[i]
    
    return ks
# endregion


# region ===== Модификация пакетов =====
def modify_plen238(pkt, r, g, b):
    """Модификация plen=238: проверенный метод."""
    m = bytearray(pkt)
    key3 = extract_keystream_238(pkt)
    
    # Color data: байты 12-236
    for led in range(NUM_LEDS):
        base = 12 + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] = key3[(base + 0) % 3] ^ r
            m[base + 1] = key3[(base + 1) % 3] ^ g
            m[base + 2] = key3[(base + 2) % 3] ^ b
    
    return bytes(m)


def modify_packet_with_keystream(pkt, r, g, b, period, phase, color_start):
    """
    Модификация пакета с извлечением keystream.
    
    new_cipher[i] = ks[(i + phase) % period] ^ target_plain[i]
    
    Для color region: target_plain = [R, G, B, R, G, B, ...]
    """
    m = bytearray(pkt)
    ks = extract_keystream_with_phase(pkt, period, phase)
    
    # Color data
    color_len = len(pkt) - color_start - 1  # -1 for tail
    num_leds = min(color_len // 3, NUM_LEDS)
    
    for led in range(num_leds):
        base = color_start + led * 3
        if base + 2 < len(m) - 1:
            m[base + 0] = ks[(base + phase) % period] ^ r
            m[base + 1] = ks[(base + 1 + phase) % period] ^ g
            m[base + 2] = ks[(base + 2 + phase) % period] ^ b
    
    return bytes(m)


def modify_color_packet(pkt, r, g, b):
    """Модифицирует любой color packet."""
    plen = len(pkt)
    period, phase, color_start = get_crypto_params(plen)
    
    if plen == 238:
        return modify_plen238(pkt, r, g, b)
    else:
        return modify_packet_with_keystream(pkt, r, g, b, period, phase, color_start)
# endregion


# region ===== Replay =====
def replay_with_color(ser, writes, r, g, b, label=""):
    """Replay с модификацией цвета. ser должен быть открыт."""
    mod_count = 0
    for w in writes:
        plen = len(w)
        if 238 <= plen <= 245:
            w = modify_color_packet(w, r, g, b)
            mod_count += 1
        
        ser.write(w)
        ser.flush()
        
        if plen >= 238:
            time.sleep(0.033)
        elif plen == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    
    print("  %s: %d color packets" % (label, mod_count))
    time.sleep(4)
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("CORRECT COLOR MOD — правильная модификация для всех plen")
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
        if 238 <= plen <= 245:
            period, phase, color_start = get_crypto_params(plen)
            print("  plen=%d: %d packets, period=%d, phase=%d, color_start=%d" % (
                plen, plen_counts[plen], period, phase, color_start))
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
