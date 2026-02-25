# -*- coding: utf-8 -*-
"""
crypto_crack.py — Финальная стадия взлома шифрования Beelight.

Подтверждено:
- Cipher = XOR с 3-байтовым ключом (keystream period=3)
- Весь пакет (header + LED + tail) шифруется одним ключом
- Plaintext header содержит фиксированные поля + 2-байтовый nonce
- Ключ меняется каждый пакет и зависит от nonce

Цель: найти формулу key = f(nonce) или key = f(byte[0], byte[1])
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


# region ===== Утилиты =====
def parse_csv_long_writes(filepath, min_len=200):
    """Извлекает длинные WRITE-пакеты из CSV."""
    packets = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE;DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            data_hex = parts[5].strip()
            try:
                raw = bytes.fromhex(data_hex.replace(" ", ""))
            except ValueError:
                continue
            if len(raw) >= min_len:
                packets.append(raw)
    return packets


def find_solid_packets(packets, min_repeat=70):
    """Находит пакеты с 70+ повторяющимися RGB-триплетами."""
    results = []
    for pkt in packets:
        for start in range(min(20, len(pkt) - 5)):
            triplet = pkt[start:start + 3]
            count = 1
            pos = start + 3
            while pos + 2 < len(pkt) and pkt[pos:pos + 3] == triplet:
                count += 1
                pos += 3
            if count >= min_repeat:
                results.append((pkt, start, count, triplet))
                break
    return results


def decrypt_packet(pkt, key3):
    """Расшифровывает пакет XOR с 3-байтовым ключом."""
    return bytes([pkt[i] ^ key3[i % 3] for i in range(len(pkt))])
# endregion


# region ===== Анализ plaintext структуры =====
def analyze_plaintext_structure():
    """
    Для каждого solid-color пакета:
    1. Извлечь keystream = enc_triplet XOR plain_rgb
    2. Дешифровать ВЕСЬ пакет этим ключом
    3. Сравнить plaintext между пакетами → найти фиксированные поля
    """
    print("=" * 70)
    print("АНАЛИЗ PLAINTEXT СТРУКТУРЫ")
    print("=" * 70)

    for color_name, rgb in KNOWN_COLORS.items():
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if not os.path.exists(csv_path):
            continue

        pkts = parse_csv_long_writes(csv_path, min_len=230)
        solid = find_solid_packets(pkts)

        if not solid:
            continue

        print(f"\n### {color_name.upper()} (RGB={rgb}) — {len(solid)} solid пакетов ###")

        plaintexts = []
        for pkt, led_off, cnt, enc_trip in solid[:20]:
            # Извлекаем ключ
            key3 = bytes([enc_trip[i] ^ rgb[i] for i in range(3)])
            # Дешифруем весь пакет
            plain = decrypt_packet(pkt, key3)
            plaintexts.append({
                "plain": plain,
                "key3": key3,
                "led_off": led_off,
                "pkt_len": len(pkt),
            })

        # Показываем первые 3 plaintext
        for i, pt in enumerate(plaintexts[:5]):
            p = pt["plain"]
            kh = " ".join(f"{b:02x}" for b in pt["key3"])
            # Заголовок
            hdr = " ".join(f"{b:02x}" for b in p[:15])
            # LED data (первые 6 байт = 2 LED)
            led = " ".join(f"{b:02x}" for b in p[pt["led_off"]:pt["led_off"]+9])
            # Tail
            tail_start = pt["led_off"] + 75 * 3
            tail = " ".join(f"{b:02x}" for b in p[tail_start:])
            print(f"  [{i}] len={pt['pkt_len']} key=[{kh}]")
            print(f"      hdr:  [{hdr}]")
            print(f"      led:  [{led}]...")
            print(f"      tail: [{tail}]")

        # Найдём фиксированные позиции (одинаковые во всех plaintext)
        if len(plaintexts) >= 2:
            min_len = min(pt["pkt_len"] for pt in plaintexts)
            fixed_positions = []
            variable_positions = []
            for pos in range(min_len):
                vals = set(pt["plain"][pos] for pt in plaintexts)
                if len(vals) == 1:
                    fixed_positions.append((pos, list(vals)[0]))
                else:
                    variable_positions.append((pos, vals))

            print(f"\n  Фиксированные позиции: {len(fixed_positions)}/{min_len}")
            # Показываем фиксированные значения в виде карты
            print("  Фиксированные байты:")
            fixed_map = {}
            for pos, val in fixed_positions:
                fixed_map[pos] = val
            for pos in range(min(min_len, 20)):
                if pos in fixed_map:
                    print(f"    [{pos:3d}] = 0x{fixed_map[pos]:02x} ({fixed_map[pos]:3d}) FIXED")
                else:
                    vals = [pt["plain"][pos] for pt in plaintexts[:5]]
                    vs = " ".join(f"{v:02x}" for v in vals)
                    print(f"    [{pos:3d}] = [{vs}] VARIABLE")
# endregion


# region ===== Анализ nonce -> key =====
def analyze_nonce_to_key():
    """
    byte[0] и byte[1] plaintext — переменные (nonce).
    Ключ 3 байта — тоже переменный.
    Ищем математическую связь между nonce и ключом.
    """
    print("\n" + "=" * 70)
    print("АНАЛИЗ NONCE → KEY")
    print("=" * 70)

    # Собираем все пары (nonce_bytes, key3) из ВСЕХ цветов
    all_pairs = []

    for color_name, rgb in KNOWN_COLORS.items():
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if not os.path.exists(csv_path):
            continue

        pkts = parse_csv_long_writes(csv_path, min_len=230)
        solid = find_solid_packets(pkts)

        for pkt, led_off, cnt, enc_trip in solid:
            key3 = bytes([enc_trip[i] ^ rgb[i] for i in range(3)])
            plain = decrypt_packet(pkt, key3)
            nonce = (plain[0], plain[1])
            all_pairs.append({
                "nonce": nonce,
                "key3": key3,
                "color": color_name,
                "pkt_len": len(pkt),
                "plain_hdr": plain[:12],
            })

    print(f"\n  Всего пар nonce→key: {len(all_pairs)}")

    # Проверяем: один nonce → один ключ?
    by_nonce = defaultdict(list)
    for p in all_pairs:
        by_nonce[p["nonce"]].append(p["key3"])

    consistent = 0
    inconsistent = 0
    for nonce, keys in by_nonce.items():
        if len(set(keys)) == 1:
            consistent += 1
        else:
            inconsistent += 1

    print(f"  Один nonce → один ключ: {consistent}/{consistent + inconsistent}")
    if inconsistent > 0:
        print(f"  НЕСОВПАДЕНИЙ: {inconsistent}")
        for nonce, keys in by_nonce.items():
            if len(set(keys)) > 1:
                ks = [" ".join(f"{b:02x}" for b in k) for k in set(keys)]
                print(f"    nonce=({nonce[0]:02x},{nonce[1]:02x}): {ks}")
                if len(ks) > 3:
                    break

    # Попытка найти формулу key = f(nonce)
    print("\n  --- Попытка найти формулу ---")
    print("  nonce[0] | nonce[1] | key[0] | key[1] | key[2] | XOR/ADD patterns")
    print("  " + "-" * 70)

    for p in all_pairs[:20]:
        n0, n1 = p["nonce"]
        k0, k1, k2 = p["key3"]

        # Различные комбинации
        xor01 = n0 ^ k0
        xor11 = n1 ^ k1
        add01 = (n0 + k0) & 0xFF
        sub01 = (k0 - n0) & 0xFF

        print(f"    0x{n0:02x}   |  0x{n1:02x}   | 0x{k0:02x}  | "
              f"0x{k1:02x}  | 0x{k2:02x}  | "
              f"n0^k0=0x{xor01:02x} n1^k1=0x{xor11:02x} "
              f"n0+k0=0x{add01:02x} k0-n0=0x{sub01:02x}")

    # Проверяем: key[0] = nonce[0] ^ constant?
    print("\n  --- Проверка key[i] = nonce[j] ^ constant ---")
    for ki in range(3):
        for ni in range(2):
            xors = set()
            for p in all_pairs[:30]:
                n = p["nonce"][ni]
                k = p["key3"][ki]
                xors.add(n ^ k)
            if len(xors) == 1:
                val = list(xors)[0]
                print(f"    key[{ki}] = nonce[{ni}] ^ 0x{val:02x} — MATCH!")
            else:
                print(f"    key[{ki}] vs nonce[{ni}]: {len(xors)} unique XOR values (no constant)")

    # Проверяем: key = MD5(nonce_bytes + something)[:3]
    print("\n  --- MD5 brute-force (short seeds) ---")
    device_keys = [
        b"diy-d6lfwphynoh3",
        b"fqhgd6lfwphynoh3",
        b"diy-d6lfwphynoh3,fqhgd6lfwphynoh3",
    ]

    found_md5 = False
    for dk in device_keys:
        for p in all_pairs[:3]:
            n0, n1 = p["nonce"]
            target = p["key3"]
            # Попробуем разные nonce форматы
            nonces = [
                bytes([n0]),
                bytes([n1]),
                bytes([n0, n1]),
                bytes([n1, n0]),
            ]
            for nonce_bytes in nonces:
                for order in ["key+nonce", "nonce+key"]:
                    if order == "key+nonce":
                        data = dk + nonce_bytes
                    else:
                        data = nonce_bytes + dk
                    h = hashlib.md5(data).digest()
                    if h[:3] == target:
                        print(f"    *** FOUND: MD5({order}) = key! "
                              f"dk={dk}, nonce={nonce_bytes.hex()}")
                        found_md5 = True
                        break

    if not found_md5:
        print("    MD5 с device key не подошёл.")

    # Проверяем: может key вообще лежит в заголовке как-то?
    print("\n  --- Проверка: key[i] = header[j] для каких-то j ---")
    for p in all_pairs[:5]:
        hdr = p["plain_hdr"]
        key = p["key3"]
        print(f"    hdr=[{' '.join(f'{b:02x}' for b in hdr)}] key=[{' '.join(f'{b:02x}' for b in key)}]")
        for ki in range(3):
            for hi in range(len(hdr)):
                if hdr[hi] == key[ki]:
                    print(f"      key[{ki}]=0x{key[ki]:02x} == hdr[{hi}]")
# endregion


# region ===== Анализ plaintext header между цветами =====
def compare_plaintext_headers():
    """
    Сравниваем расшифрованные заголовки между разными цветами.
    Это поможет понять какие байты заголовка зависят от payload.
    """
    print("\n" + "=" * 70)
    print("СРАВНЕНИЕ PLAINTEXT HEADERS МЕЖДУ ЦВЕТАМИ")
    print("=" * 70)

    color_headers = {}
    for color_name, rgb in KNOWN_COLORS.items():
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if not os.path.exists(csv_path):
            continue
        pkts = parse_csv_long_writes(csv_path, min_len=230)
        solid = find_solid_packets(pkts)
        headers = []
        for pkt, led_off, cnt, enc_trip in solid[:10]:
            key3 = bytes([enc_trip[i] ^ rgb[i] for i in range(3)])
            plain = decrypt_packet(pkt, key3)
            headers.append(plain[:15])
        if headers:
            color_headers[color_name] = headers

    # Показываем по одному header от каждого цвета
    print("\n  Пример headers (один от каждого цвета, 238-byte пакеты):")
    for color_name, headers in color_headers.items():
        hdr = headers[0]
        hdr_hex = " ".join(f"{b:02x}" for b in hdr)
        print(f"  {color_name:5s}: [{hdr_hex}]")

    # Какие позиции одинаковы между ВСЕМИ цветами?
    if len(color_headers) >= 2:
        all_hdrs = []
        for hdrs in color_headers.values():
            all_hdrs.extend(hdrs)
        min_len = min(len(h) for h in all_hdrs)

        print(f"\n  Фиксированные позиции МЕЖДУ цветами (из {len(all_hdrs)} headers):")
        for pos in range(min_len):
            vals = set(h[pos] for h in all_hdrs)
            if len(vals) == 1:
                print(f"    [{pos:2d}] = 0x{list(vals)[0]:02x} — ОДИНАКОВО для всех цветов")
            elif len(vals) <= 5:
                vs = ", ".join(f"0x{v:02x}" for v in sorted(vals))
                print(f"    [{pos:2d}] = {{{vs}}} — {len(vals)} вариантов")
            else:
                print(f"    [{pos:2d}] = {len(vals)} уникальных значений — ПЕРЕМЕННАЯ")
# endregion


# region ===== Проверка: является ли byte[0] plaintext == длина пакета =====
def check_byte0_meaning():
    """Проверяем различные гипотезы о значении byte[0] и byte[1] plaintext."""
    print("\n" + "=" * 70)
    print("ЗНАЧЕНИЕ BYTE[0] И BYTE[1] В PLAINTEXT")
    print("=" * 70)

    for color_name, rgb in [("black", (0,0,0)), ("red", (0xFF,0,0))]:
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if not os.path.exists(csv_path):
            continue

        pkts = parse_csv_long_writes(csv_path, min_len=230)
        solid = find_solid_packets(pkts)

        print(f"\n  --- {color_name.upper()} ---")
        print(f"  pkt_len | plain[0] | plain[1] | p0_hex | comment")
        print(f"  " + "-" * 60)

        for pkt, led_off, cnt, enc_trip in solid[:15]:
            key3 = bytes([enc_trip[i] ^ rgb[i] for i in range(3)])
            plain = decrypt_packet(pkt, key3)

            p0 = plain[0]
            p1 = plain[1]
            plen = len(pkt)

            # Различные гипотезы для byte[0]
            comments = []
            if p0 == plen:
                comments.append("= pkt_len")
            if p0 == plen - 1:
                comments.append("= pkt_len - 1")
            if p0 == plen - 2:
                comments.append("= pkt_len - 2")
            if p0 == plen - 12:
                comments.append("= pkt_len - 12 (payload_len)")
            if p0 == (plen - 12) & 0xFF:
                comments.append("= (pkt_len - 12) & 0xFF")
            if p0 == NUM_LEDS * 3:
                comments.append("= 75*3 = 225")
            if p0 == NUM_LEDS * 3 + 1:
                comments.append("= 75*3 + 1 = 226")

            comment = "; ".join(comments) if comments else "?"
            print(f"  {plen:7d} | {p0:8d} | {p1:8d} | 0x{p0:02x}   | {comment}")
# endregion


# region ===== Main =====
def main():
    sys.stdout.reconfigure(encoding="utf-8")
    analyze_plaintext_structure()
    analyze_nonce_to_key()
    compare_plaintext_headers()
    check_byte0_meaning()


if __name__ == "__main__":
    main()
# endregion
