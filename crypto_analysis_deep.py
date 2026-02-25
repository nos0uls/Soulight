# -*- coding: utf-8 -*-
"""
crypto_analysis_deep.py — Глубокий криптоанализ Beelight протокола.

Фокус: понять как генерируется 3-байтовый keystream для каждого пакета,
используя пакеты с 75 повторяющимися триплетами (solid color).

Гипотеза: keystream = f(byte[0], byte[1], или какой-то nonce в заголовке)
"""

import sys
import os
import hashlib
from collections import defaultdict

# region ===== Настройки =====
CSV_DIR = os.path.dirname(os.path.abspath(__file__))
NUM_LEDS = 75

KNOWN_COLORS = {
    "red":   (0xFF, 0x00, 0x00),
    "green": (0x00, 0xFF, 0x00),
    "blue":  (0x00, 0x00, 0xFF),
    "white": (0xFF, 0xFF, 0xFF),
    "black": (0x00, 0x00, 0x00),
}
# endregion


# region ===== Парсинг =====
def parse_csv_writes(filepath):
    """Извлекает WRITE-пакеты из CSV sniffer файла."""
    packets = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE;DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            data_hex = parts[5].strip()
            len_str = parts[7].strip()
            try:
                pkt_len = int(len_str)
            except ValueError:
                continue
            if not data_hex or pkt_len <= 0:
                continue
            try:
                raw = bytes.fromhex(data_hex.replace(" ", ""))
            except ValueError:
                continue
            packets.append(raw)
    return packets


def parse_all_session_writes(filepath):
    """
    Извлекает ВСЕ пакеты (короткие и длинные) с сохранением порядка.
    Возвращает список (длина, байты).
    """
    result = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE;DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            data_hex = parts[5].strip()
            len_str = parts[7].strip()
            try:
                pkt_len = int(len_str)
            except ValueError:
                continue
            if not data_hex or pkt_len <= 0:
                continue
            try:
                raw = bytes.fromhex(data_hex.replace(" ", ""))
            except ValueError:
                continue
            result.append(raw)
    return result


def find_repeating_triplet(pkt, min_repeat=70):
    """Ищет 70+ повторяющийся 3-байтовый блок."""
    for start in range(min(20, len(pkt) - 5)):
        triplet = pkt[start:start + 3]
        count = 1
        pos = start + 3
        while pos + 2 < len(pkt) and pkt[pos:pos + 3] == triplet:
            count += 1
            pos += 3
        if count >= min_repeat:
            return start, count, triplet
    return None
# endregion


# region ===== Извлечение keystream =====
def extract_key_triplet(pkt, rgb, led_offset):
    """
    Для пакета solid color, извлекает 3-байтовый ключ XOR.
    key = encrypted_triplet XOR plain_rgb
    """
    enc = pkt[led_offset:led_offset + 3]
    return bytes([enc[i] ^ rgb[i] for i in range(3)])
# endregion


# region ===== Анализ связи header ↔ keystream =====
def analyze_header_key_correlation(color_name, rgb, packets):
    """
    Для каждого пакета с 75-repeat:
    - Извлекает keystream (3 байта)
    - Извлекает заголовок (12 байт)
    - Ищет зависимость между ними

    Если keystream = MD5(header + key)[:3], мы найдём это.
    """
    entries = []
    for i, pkt in enumerate(packets):
        result = find_repeating_triplet(pkt, min_repeat=70)
        if not result:
            continue
        led_offset, count, enc_trip = result
        key_trip = extract_key_triplet(pkt, rgb, led_offset)
        header = pkt[:led_offset]
        tail = pkt[led_offset + count * 3:]
        entries.append({
            "idx": i,
            "header": header,
            "key_triplet": key_trip,
            "enc_triplet": enc_trip,
            "led_offset": led_offset,
            "tail": tail,
            "pkt_len": len(pkt),
            "full_pkt": pkt,
        })
    return entries


def try_md5_hypothesis(entries, device_key="diy-d6lfwphynoh3"):
    """
    Проверяет гипотезу: keystream = MD5(device_key + nonce)
    где nonce — какая-то комбинация байтов заголовка.

    Перебирает разные варианты nonce (byte[0], byte[0:2], etc.)
    """
    print("\n--- Проверка MD5 гипотез ---")
    key_bytes = device_key.encode("ascii")

    for entry in entries[:5]:
        hdr = entry["header"]
        key_trip = entry["key_triplet"]
        kt_hex = " ".join(f"{b:02x}" for b in key_trip)
        hdr_hex = " ".join(f"{b:02x}" for b in hdr)
        print(f"\n  Пакет idx={entry['idx']}, header=[{hdr_hex}], key=[{kt_hex}]")

        # Гипотеза 1: MD5(device_key + byte[0])
        for name, nonce in [
            ("byte[0]",          bytes([hdr[0]])),
            ("byte[0:2]",        hdr[0:2]),
            ("byte[0:3]",        hdr[0:3]),
            ("byte[0:4]",        hdr[0:4]),
            ("header_full",      hdr),
            ("byte[0]+byte[2]",  bytes([hdr[0], hdr[2]])),
            ("byte[2]",          bytes([hdr[2]])),
            ("byte[2:5]",        hdr[2:5]),
        ]:
            # MD5(key + nonce)
            h1 = hashlib.md5(key_bytes + nonce).digest()
            if h1[:3] == key_trip:
                print(f"    *** MATCH: MD5(device_key + {name}) = keystream!")
                return True

            # MD5(nonce + key)
            h2 = hashlib.md5(nonce + key_bytes).digest()
            if h2[:3] == key_trip:
                print(f"    *** MATCH: MD5({name} + device_key) = keystream!")
                return True

            # MD5(key XOR nonce extended)
            # MD5(nonce)
            h3 = hashlib.md5(nonce).digest()
            if h3[:3] == key_trip:
                print(f"    *** MATCH: MD5({name}) = keystream (no device key)!")
                return True

    # Также попробуем второй ключ
    key2 = "fqhgd6lfwphynoh3"
    key2_bytes = key2.encode("ascii")
    print("\n  Trying with second key:", key2)
    for entry in entries[:3]:
        hdr = entry["header"]
        key_trip = entry["key_triplet"]
        for name, nonce in [
            ("byte[0]",     bytes([hdr[0]])),
            ("byte[0:2]",   hdr[0:2]),
            ("header_full", hdr),
        ]:
            h = hashlib.md5(key2_bytes + nonce).digest()
            if h[:3] == key_trip:
                print(f"    *** MATCH with key2: MD5(key2 + {name}) = keystream!")
                return True
            h = hashlib.md5(nonce + key2_bytes).digest()
            if h[:3] == key_trip:
                print(f"    *** MATCH with key2: MD5({name} + key2) = keystream!")
                return True

    print("  Простые MD5-гипотезы не подтвердились.")
    return False


def analyze_byte0_vs_key(entries):
    """
    Анализирует корреляцию byte[0] с keystream.
    Если byte[0] — nonce/counter, keystream должен быть его функцией.
    """
    print("\n--- byte[0] vs keystream ---")

    # Группируем по byte[0]
    by_b0 = defaultdict(list)
    for e in entries:
        by_b0[e["header"][0]].append(e["key_triplet"])

    # Если один byte[0] -> один keystream, значит byte[0] определяет ключ
    deterministic = 0
    total = 0
    for b0, keys in sorted(by_b0.items()):
        total += 1
        unique_keys = set(keys)
        if len(unique_keys) == 1:
            deterministic += 1
        if len(keys) > 1:
            kh = [" ".join(f"{b:02x}" for b in k) for k in list(unique_keys)[:3]]
            print(f"  byte[0]=0x{b0:02x}: {len(keys)} пакетов, "
                  f"{len(unique_keys)} уникальных ключей: {kh}")

    print(f"\n  Из {total} уникальных byte[0]: {deterministic} детерминированных")
    return deterministic == total


def analyze_header_structure(entries):
    """
    Глубокий анализ структуры 12-байтового заголовка.
    Ищем паттерны: какие байты повторяются, какие коррелируют.
    """
    print("\n--- Структура заголовка (12 байт) ---")

    if not entries:
        print("  Нет данных")
        return

    hdr_len = len(entries[0]["header"])
    print(f"  Header length: {hdr_len} байт")

    # Проверяем повторяющиеся паттерны внутри заголовка
    print("\n  Проверка: повторяются ли байты заголовка с каким-то периодом?")
    for entry in entries[:5]:
        hdr = entry["header"]
        hdr_hex = " ".join(f"{b:02x}" for b in hdr)
        # Проверяем период 3 (как в keystream)
        period3_match = all(hdr[i] == hdr[i % 3] for i in range(3, hdr_len))
        # Проверяем период 5
        period5_match = all(hdr[i] == hdr[i % 5] for i in range(5, min(hdr_len, 10)))
        print(f"  [{hdr_hex}] period3={period3_match} period5={period5_match}")

    # Анализ: XOR header с keystream (расширенным до 12 байт)
    print("\n  XOR header с расширенным keystream (период 3):")
    for entry in entries[:5]:
        hdr = entry["header"]
        key = entry["key_triplet"]
        # XOR header с keystream (repeated)
        decrypted_hdr = bytes([hdr[i] ^ key[i % 3] for i in range(len(hdr))])
        dh = " ".join(f"{b:02x}" for b in decrypted_hdr)
        print(f"  Decrypted header: [{dh}]")


def analyze_sequential_packets(filepath, color_name, rgb):
    """
    Анализирует последовательные пакеты в сессии,
    чтобы найти инкрементирующийся counter/nonce.
    """
    print(f"\n--- Последовательные пакеты в {color_name} сессии ---")

    all_pkts = parse_all_session_writes(filepath)

    # Найдём пары (heartbeat, data_packet)
    pairs = []
    for i in range(len(all_pkts) - 1):
        if len(all_pkts[i]) == 5 and len(all_pkts[i + 1]) > 10:
            hb = all_pkts[i]
            dp = all_pkts[i + 1]
            result = find_repeating_triplet(dp, min_repeat=70)
            if result:
                led_off, cnt, enc_trip = result
                key_trip = extract_key_triplet(dp, rgb, led_off)
                pairs.append({
                    "heartbeat": hb,
                    "data_pkt": dp,
                    "key_triplet": key_trip,
                    "hb_byte3": hb[3],
                    "data_byte0": dp[0],
                    "data_byte1": dp[1],
                    "pkt_len": len(dp),
                })

    print(f"  Найдено {len(pairs)} пар (heartbeat + solid data)")

    if len(pairs) < 2:
        return

    # Показываем последовательные пары
    print("\n  Seq | HB[3] | Data[0] | Data[1] | Len | Key triplet")
    print("  " + "-" * 60)
    for i, p in enumerate(pairs[:15]):
        kt = " ".join(f"{b:02x}" for b in p["key_triplet"])
        print(f"  {i:3d} | 0x{p['hb_byte3']:02x}  | 0x{p['data_byte0']:02x}    | "
              f"0x{p['data_byte1']:02x}    | {p['pkt_len']:3d} | [{kt}]")

    # Ищем зависимость: byte[0] data пакета → длина
    print("\n  Анализ byte[0] data пакета:")
    b0_vals = [p["data_byte0"] for p in pairs]
    b0_diffs = [b0_vals[i+1] - b0_vals[i] for i in range(len(b0_vals)-1)]
    print(f"  byte[0] values: {[f'0x{v:02x}' for v in b0_vals[:15]]}")
    print(f"  byte[0] diffs:  {b0_diffs[:15]}")

    # Ищем зависимость heartbeat byte[3] → data byte[0]
    print("\n  Корреляция HB[3] и Data byte[0]:")
    for i, p in enumerate(pairs[:10]):
        xor_val = p["hb_byte3"] ^ p["data_byte0"]
        sum_val = (p["hb_byte3"] + p["data_byte0"]) & 0xFF
        diff_val = (p["data_byte0"] - p["hb_byte3"]) & 0xFF
        print(f"  {i}: HB[3]=0x{p['hb_byte3']:02x} D[0]=0x{p['data_byte0']:02x} "
              f"XOR=0x{xor_val:02x} SUM=0x{sum_val:02x} DIFF=0x{diff_val:02x}")
# endregion


# region ===== Попытка реверса через known-plaintext =====
def reverse_full_keystream(entries):
    """
    Для BLACK пакетов (plaintext = 0x00), весь пакет = keystream.
    Анализируем полную структуру keystream (не только LED-регион).
    """
    print("\n--- Полная структура keystream (из BLACK пакетов) ---")

    for entry in entries[:3]:
        pkt = entry["full_pkt"]
        led_off = entry["led_offset"]
        key_trip = entry["key_triplet"]

        # Для BLACK: pkt = keystream (XOR с 0 = identity)
        # НО: заголовок может НЕ быть plain RGB → заголовок шифруется отдельно?

        print(f"\n  Пакет len={len(pkt)}, LED offset={led_off}")
        print(f"  Key triplet (LED): [{' '.join(f'{b:02x}' for b in key_trip)}]")

        # Header (до LED)
        hdr = pkt[:led_off]
        print(f"  Header (raw): [{' '.join(f'{b:02x}' for b in hdr)}]")

        # Если заголовок тоже XOR с тем же keystream (period 3):
        hdr_decrypted = bytes([hdr[i] ^ key_trip[i % 3] for i in range(len(hdr))])
        print(f"  Header XOR key: [{' '.join(f'{b:02x}' for b in hdr_decrypted)}]")

        # Tail (после LED)
        tail = entry["tail"]
        if tail:
            print(f"  Tail (raw): [{' '.join(f'{b:02x}' for b in tail)}]")
            tail_dec = bytes([tail[i] ^ key_trip[i % 3] for i in range(len(tail))])
            print(f"  Tail XOR key: [{' '.join(f'{b:02x}' for b in tail_dec)}]")


def reverse_full_keystream_red(entries):
    """
    Для RED пакетов (plaintext LED = FF 00 00):
    LED-регион keystream = enc XOR (FF,00,00)
    Проверяем весь пакет.
    """
    print("\n--- Полная структура keystream (из RED пакетов) ---")

    for entry in entries[:3]:
        pkt = entry["full_pkt"]
        led_off = entry["led_offset"]
        key_trip = entry["key_triplet"]

        print(f"\n  Пакет len={len(pkt)}, LED offset={led_off}")
        print(f"  Key triplet: [{' '.join(f'{b:02x}' for b in key_trip)}]")

        # Дешифруем заголовок, предполагая тот же keystream period=3
        hdr = pkt[:led_off]
        hdr_dec = bytes([hdr[i] ^ key_trip[i % 3] for i in range(len(hdr))])
        print(f"  Header XOR key: [{' '.join(f'{b:02x}' for b in hdr_dec)}]")
# endregion


# region ===== Главная функция =====
def main():
    sys.stdout.reconfigure(encoding="utf-8")

    print("=" * 60)
    print("BEELIGHT DEEP CRYPTO ANALYSIS")
    print("=" * 60)

    # Загружаем данные
    all_entries = {}
    for color_name, rgb in KNOWN_COLORS.items():
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if not os.path.exists(csv_path):
            continue
        pkts = parse_csv_writes(csv_path)
        long_pkts = [p for p in pkts if len(p) > 10]

        entries = analyze_header_key_correlation(color_name, rgb, long_pkts)
        all_entries[color_name] = entries
        print(f"  {color_name}: {len(entries)} пакетов с 75-repeat "
              f"из {len(long_pkts)} длинных")

    # 1. MD5 гипотезы
    print("\n" + "=" * 60)
    print("1. ПРОВЕРКА MD5 ГИПОТЕЗ")
    print("=" * 60)

    # Объединяем все entries
    combined = []
    for entries in all_entries.values():
        combined.extend(entries)

    if combined:
        try_md5_hypothesis(combined)

    # 2. byte[0] vs keystream корреляция
    print("\n" + "=" * 60)
    print("2. BYTE[0] VS KEYSTREAM КОРРЕЛЯЦИЯ")
    print("=" * 60)
    if combined:
        analyze_byte0_vs_key(combined)

    # 3. Структура заголовка
    print("\n" + "=" * 60)
    print("3. СТРУКТУРА ЗАГОЛОВКА")
    print("=" * 60)
    if combined:
        analyze_header_structure(combined)

    # 4. Полный keystream из BLACK пакетов
    print("\n" + "=" * 60)
    print("4. ПОЛНЫЙ KEYSTREAM (BLACK)")
    print("=" * 60)
    if "black" in all_entries and all_entries["black"]:
        reverse_full_keystream(all_entries["black"])

    # 5. Полный keystream из RED пакетов
    print("\n" + "=" * 60)
    print("5. ПОЛНЫЙ KEYSTREAM (RED)")
    print("=" * 60)
    if "red" in all_entries and all_entries["red"]:
        reverse_full_keystream_red(all_entries["red"])

    # 6. Последовательный анализ (counter detection)
    print("\n" + "=" * 60)
    print("6. ПОСЛЕДОВАТЕЛЬНЫЙ АНАЛИЗ (COUNTER DETECTION)")
    print("=" * 60)
    for color_name, rgb in [("red", (0xFF, 0, 0)), ("black", (0, 0, 0))]:
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if os.path.exists(csv_path):
            analyze_sequential_packets(csv_path, color_name, rgb)

# endregion


if __name__ == "__main__":
    main()
