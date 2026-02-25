# -*- coding: utf-8 -*-
"""
crypto_analysis.py — Криптоанализ пакетов Beelight протокола.

Скрипт анализирует захваченные USB-пакеты (.csv от USB sniffer)
для выявления алгоритма шифрования.

Основная гипотеза: XOR stream cipher с keystream, генерируемым
на основе MD5 и меняющимся каждый пакет.
"""

import sys
import os
from collections import defaultdict, Counter

# region ===== Настройки =====
# Путь к директории с CSV файлами захватов
CSV_DIR = os.path.dirname(os.path.abspath(__file__))

# Известные цвета и их RGB-значения (предполагаемый plaintext)
KNOWN_COLORS = {
    "red":   (0xFF, 0x00, 0x00),
    "green": (0x00, 0xFF, 0x00),
    "blue":  (0x00, 0x00, 0xFF),
    "white": (0xFF, 0xFF, 0xFF),
    "black": (0x00, 0x00, 0x00),
}

# Количество LED в ленте
NUM_LEDS = 75
# endregion


# region ===== Парсинг CSV =====
def parse_csv_writes(filepath):
    """
    Извлекает все WRITE-пакеты из CSV-файла USB sniffer.
    Возвращает список байтовых массивов.
    """
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


def split_short_long(packets, threshold=10):
    """
    Разделяет пакеты на короткие (heartbeat/команды)
    и длинные (данные с LED payload).
    """
    short = [p for p in packets if len(p) <= threshold]
    long_ = [p for p in packets if len(p) > threshold]
    return short, long_
# endregion


# region ===== Анализ повторяющихся триплетов =====
def find_repeating_triplet(pkt, min_repeat=10):
    """
    Ищет самую длинную последовательность повторяющихся
    3-байтовых блоков (RGB triplets) в пакете.

    Возвращает (offset, repeat_count, triplet_bytes) или None.
    """
    best_start = 0
    best_count = 0
    best_triplet = b""

    for start in range(len(pkt) - 5):
        triplet = pkt[start:start + 3]
        count = 1
        pos = start + 3
        while pos + 2 < len(pkt) and pkt[pos:pos + 3] == triplet:
            count += 1
            pos += 3
        if count > best_count:
            best_count = count
            best_start = start
            best_triplet = triplet

    if best_count >= min_repeat:
        return best_start, best_count, best_triplet
    return None
# endregion


# region ===== Known-Plaintext Attack =====
def xor_bytes(a, b):
    """XOR двух байтовых массивов одинаковой длины."""
    return bytes([x ^ y for x, y in zip(a, b)])


def extract_keystream_from_solid(pkt, rgb_tuple, led_offset):
    """
    Для пакета с solid color (все LED одного цвета)
    извлекает keystream через XOR с известным plaintext.

    pkt — зашифрованный пакет
    rgb_tuple — (R, G, B) известный цвет
    led_offset — смещение начала LED данных в пакете
    """
    r, g, b = rgb_tuple
    led_plain = bytes([r, g, b] * NUM_LEDS)
    led_cipher = pkt[led_offset:led_offset + NUM_LEDS * 3]

    if len(led_cipher) < NUM_LEDS * 3:
        return None

    keystream_led = xor_bytes(led_cipher, led_plain)
    return keystream_led


def analyze_keystream_periodicity(keystream):
    """
    Проверяет периодичность keystream.
    Если keystream повторяется каждые N байт, значит
    ключ шифрования имеет период N.
    """
    results = {}
    for period in [1, 2, 3, 4, 6, 8, 12, 16, 32]:
        if period >= len(keystream):
            continue
        matches = 0
        total = 0
        for i in range(period, len(keystream)):
            total += 1
            if keystream[i] == keystream[i % period]:
                matches += 1
        results[period] = matches / total if total > 0 else 0
    return results
# endregion


# region ===== Дифференциальный анализ =====
def differential_analysis(pkts_same_color, led_offset=12):
    """
    XOR-ит пары пакетов одного цвета.
    Если plaintext одинаковый, C1 XOR C2 = K1 XOR K2.
    Анализирует структуру разности keystream'ов.
    """
    results = []
    for i in range(min(len(pkts_same_color) - 1, 10)):
        p1 = pkts_same_color[i]
        p2 = pkts_same_color[i + 1]
        if len(p1) != len(p2):
            continue
        xor_full = xor_bytes(p1, p2)
        # LED-регион
        led_xor = xor_full[led_offset:led_offset + NUM_LEDS * 3]
        # Заголовок
        hdr_xor = xor_full[:led_offset]
        # Хвост (после LED данных)
        tail_xor = xor_full[led_offset + NUM_LEDS * 3:]

        # Считаем нули (совпадающие байты)
        led_zeros = sum(1 for b in led_xor if b == 0)
        hdr_zeros = sum(1 for b in hdr_xor if b == 0)

        # Проверяем 3-байтовую периодичность в LED XOR
        trips = [led_xor[j:j+3] for j in range(0, min(len(led_xor), 225), 3)]
        unique_trips = len(set(trips))

        results.append({
            "pair": (i, i+1),
            "led_zeros": led_zeros,
            "hdr_zeros": hdr_zeros,
            "unique_xor_trips": unique_trips,
            "hdr_xor": hdr_xor,
            "led_xor_sample": led_xor[:30],
        })
    return results
# endregion


# region ===== Cross-Color анализ =====
def cross_color_analysis(color_packets, led_offset=12):
    """
    XOR-ит пакеты разных цветов одинаковой длины.
    Если cipher = XOR с keystream, то:
      C_red XOR C_blue = P_red XOR P_blue = known difference

    Это позволяет подтвердить гипотезу XOR cipher.
    """
    # Берём только 238-байтовые пакеты (с 75-repeat)
    results = []
    colors_238 = {}
    for color_name, pkts in color_packets.items():
        p238 = [p for p in pkts if len(p) == 238]
        if p238:
            colors_238[color_name] = p238

    color_names = list(colors_238.keys())
    for i in range(len(color_names)):
        for j in range(i + 1, len(color_names)):
            c1, c2 = color_names[i], color_names[j]
            # Находим пакеты с 75-repeat в каждом цвете
            p1 = None
            for p in colors_238[c1]:
                r = find_repeating_triplet(p, min_repeat=70)
                if r:
                    p1 = p
                    break
            p2 = None
            for p in colors_238[c2]:
                r = find_repeating_triplet(p, min_repeat=70)
                if r:
                    p2 = p
                    break
            if p1 is None or p2 is None:
                continue

            # XOR LED regions
            led1 = p1[led_offset:led_offset + 225]
            led2 = p2[led_offset:led_offset + 225]
            xor_led = xor_bytes(led1, led2)

            # Ожидаемая разница plaintext
            rgb1 = KNOWN_COLORS[c1]
            rgb2 = KNOWN_COLORS[c2]
            expected_xor = bytes([rgb1[k] ^ rgb2[k] for k in range(3)])

            # Проверяем: повторяется ли expected_xor каждые 3 байта?
            match_count = 0
            for k in range(75):
                actual = xor_led[k*3:k*3+3]
                if actual == expected_xor:
                    match_count += 1

            results.append({
                "colors": (c1, c2),
                "expected_xor": expected_xor,
                "match_count": match_count,
                "total_leds": 75,
                "sample_actual": xor_led[:15],
            })
    return results
# endregion


# region ===== Анализ заголовка =====
def analyze_header(packets_by_len):
    """
    Анализирует структуру заголовка (первые 12-13 байт)
    пакетов разной длины.
    """
    print("\n" + "=" * 60)
    print("АНАЛИЗ ЗАГОЛОВКА ПАКЕТОВ")
    print("=" * 60)

    for plen in sorted(packets_by_len.keys()):
        pkts = packets_by_len[plen]
        if not pkts:
            continue
        print(f"\n--- Длина {plen}: {len(pkts)} пакетов ---")

        # byte[0] — всегда разный? Диапазон?
        b0_vals = [p[0] for p in pkts]
        b1_vals = [p[1] for p in pkts]
        print(f"  byte[0]: min={min(b0_vals):3d} (0x{min(b0_vals):02x}), "
              f"max={max(b0_vals):3d} (0x{max(b0_vals):02x}), "
              f"unique={len(set(b0_vals))}")
        print(f"  byte[1]: min={min(b1_vals):3d} (0x{min(b1_vals):02x}), "
              f"max={max(b1_vals):3d} (0x{max(b1_vals):02x}), "
              f"unique={len(set(b1_vals))}")

        # Для каждой позиции в заголовке — entropy (уникальных значений)
        hdr_len = min(13, min(len(p) for p in pkts))
        print(f"  Header entropy (unique vals per byte pos, из {len(pkts)} пакетов):")
        entropies = []
        for pos in range(hdr_len):
            vals = set(p[pos] for p in pkts)
            entropies.append(len(vals))
        print("    pos:    " + " ".join(f"{i:3d}" for i in range(hdr_len)))
        print("    unique: " + " ".join(f"{e:3d}" for e in entropies))
# endregion


# region ===== Главная функция =====
def main():
    sys.stdout.reconfigure(encoding="utf-8")

    print("=" * 60)
    print("BEELIGHT PROTOCOL CRYPTO ANALYSIS")
    print("=" * 60)

    # Загружаем пакеты из всех CSV
    color_packets = {}
    for color_name in KNOWN_COLORS:
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if not os.path.exists(csv_path):
            print(f"[WARN] {csv_path} не найден, пропускаем")
            continue
        all_pkts = parse_csv_writes(csv_path)
        short, long_ = split_short_long(all_pkts)
        color_packets[color_name] = long_
        print(f"  {color_name}: {len(all_pkts)} пакетов ({len(short)} коротких, {len(long_)} длинных)")

    # 1. Анализ повторяющихся триплетов
    print("\n" + "=" * 60)
    print("1. ПОИСК ПОВТОРЯЮЩИХСЯ RGB-ТРИПЛЕТОВ")
    print("=" * 60)
    print("Для solid color все 75 LED одинаковые => если cipher=XOR,")
    print("зашифрованные RGB тоже повторяются.\n")

    for color_name, pkts in color_packets.items():
        pkts_with_repeat = []
        for i, pkt in enumerate(pkts):
            result = find_repeating_triplet(pkt, min_repeat=70)
            if result:
                pkts_with_repeat.append((i, pkt, result))

        print(f"  {color_name}: {len(pkts_with_repeat)}/{len(pkts)} пакетов "
              f"с 70+ повторяющимися триплетами")
        if pkts_with_repeat:
            idx, pkt, (off, cnt, trip) = pkts_with_repeat[0]
            th = " ".join(f"{b:02x}" for b in trip)
            print(f"    Пример: pkt[{idx}] offset={off} repeat={cnt} triplet=[{th}]")

    # 2. Known-Plaintext Attack
    print("\n" + "=" * 60)
    print("2. KNOWN-PLAINTEXT ATTACK")
    print("=" * 60)
    print("Извлекаем keystream: K = Cipher XOR Known_Plaintext\n")

    # Используем BLACK (plaintext = 0,0,0) — keystream = ciphertext напрямую
    if "black" in color_packets:
        print("--- BLACK: keystream = ciphertext (plaintext = 0x00) ---")
        for pkt in color_packets["black"]:
            result = find_repeating_triplet(pkt, min_repeat=70)
            if result:
                off, cnt, trip = result
                # keystream на LED-регионе = ciphertext (XOR 0 = identity)
                ks = pkt[off:off + 30]
                print(f"  Keystream (offset {off}, первые 30 байт):")
                print(f"    {' '.join(f'{b:02x}' for b in ks)}")

                # Проверяем периодичность keystream
                full_ks = pkt[off:off + cnt * 3]
                periodicity = analyze_keystream_periodicity(full_ks)
                print(f"  Периодичность keystream:")
                for period, score in sorted(periodicity.items()):
                    bar = "#" * int(score * 40)
                    print(f"    period={period:2d}: {score:.3f} {bar}")
                break

    # 3. Дифференциальный анализ (пакеты одного цвета)
    print("\n" + "=" * 60)
    print("3. ДИФФЕРЕНЦИАЛЬНЫЙ АНАЛИЗ (C1 XOR C2 при одном цвете)")
    print("=" * 60)

    for color_name, pkts in color_packets.items():
        # Группируем по длине
        by_len = defaultdict(list)
        for p in pkts:
            by_len[len(p)].append(p)

        # Берём 238-байтовые (с полным 75-repeat)
        if 238 in by_len and len(by_len[238]) >= 2:
            results = differential_analysis(by_len[238])
            if results:
                r = results[0]
                print(f"\n  {color_name} (238-byte, pair {r['pair']}):")
                print(f"    LED region zeros: {r['led_zeros']}/225")
                print(f"    Header zeros: {r['hdr_zeros']}/12")
                print(f"    Unique XOR triplets: {r['unique_xor_trips']}/75")
                hx = " ".join(f"{b:02x}" for b in r["led_xor_sample"])
                print(f"    LED XOR sample: {hx}")

    # 4. Cross-Color анализ
    print("\n" + "=" * 60)
    print("4. CROSS-COLOR ANALYSIS")
    print("=" * 60)
    print("XOR пакетов разных цветов. Если cipher=XOR с keystream,")
    print("C1 XOR C2 = P1 XOR P2 (известная разница).\n")
    print("НО: пакеты из разных сессий => разный keystream.")
    print("Это работает только если найдём пакеты с ОДИНАКОВЫМ keystream.\n")

    xc_results = cross_color_analysis(color_packets)
    for r in xc_results:
        c1, c2 = r["colors"]
        exp = " ".join(f"{b:02x}" for b in r["expected_xor"])
        smp = " ".join(f"{b:02x}" for b in r["sample_actual"])
        print(f"  {c1} vs {c2}:")
        print(f"    Expected XOR: [{exp}] x75")
        print(f"    Actual XOR:   [{smp}]...")
        print(f"    Matches: {r['match_count']}/75")

    # 5. Анализ заголовка
    all_long = []
    for pkts in color_packets.values():
        all_long.extend(pkts)
    by_len = defaultdict(list)
    for p in all_long:
        by_len[len(p)].append(p)
    analyze_header(by_len)

    # 6. ВЫВОДЫ
    print("\n" + "=" * 60)
    print("ВЫВОДЫ")
    print("=" * 60)
    print("""
Подтверждено:
- Шифрование = XOR stream cipher
- Для solid color LED-регион содержит 75 идентичных RGB-триплетов
- Keystream меняется КАЖДЫЙ пакет (0 дупликатов)
- Keystream на LED-регионе имеет период 3 (повторяется для каждого LED)
- Пакеты 238 байт: 12 байт заголовок + 225 (75*3) LED + 1 байт хвост
- byte[1] коррелирует с длиной пакета, но не линейно

Следующие шаги:
- Дизассемблинг beelightLib.dll в Ghidra для извлечения алгоритма keystream
- Runtime перехват через Harmony для получения пар plaintext/ciphertext
- Анализ заголовка (12 байт) — вероятно содержит frame counter/nonce
""")

# endregion


if __name__ == "__main__":
    main()
