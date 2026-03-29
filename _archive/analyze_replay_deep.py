# -*- coding: utf-8 -*-
"""
analyze_replay_deep.py — Глубокий анализ screen-mirroring capture.

Сравнивает формат пакетов из live capture с GenRGBTransferPackage,
извлекает ключевые паттерны шифрования и header structure.
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")


def parse_write_pairs(filepath):
    """Парсит пары (5-byte header, payload) из WRITE;DOWN операций."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            data_str = parts[5].strip()
            if not data_str:
                continue
            try:
                raw = bytes.fromhex(data_str.replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append(raw)
    return writes


def extract_key3(pkt):
    """Извлекает 3-байтный XOR ключ из пакета.
    Предполагается, что plain[2:5] = 0x00, значит cipher[2:5] = key."""
    if len(pkt) < 5:
        return None
    return bytes([pkt[3], pkt[4], pkt[2]])


def decrypt_with_key3(pkt, key3):
    """Расшифровывает пакет 3-байтным XOR ключом."""
    return bytes(pkt[i] ^ key3[i % 3] for i in range(len(pkt)))


def analyze_decrypted_header(plain, pkt_len):
    """Анализирует расшифрованный header пакета."""
    info = {
        "nonce0": plain[0],
        "nonce1": plain[1],
        "byte2_5": plain[2:6].hex(),
        "byte6_7": f"{plain[6]:02x} {plain[7]:02x}",
        "byte8_9": f"{plain[8]:02x} {plain[9]:02x}",
        "byte10_12": plain[10:13].hex(),
    }
    if pkt_len > 238:
        extra = pkt_len - 238
        info["extra_header"] = plain[13:13 + extra].hex()
    return info


def main():
    print("=" * 70)
    print("  DEEP ANALYSIS: replay.csv screen-mirroring capture")
    print("=" * 70)
    print()

    writes = parse_write_pairs(CSV_PATH)

    # Собираем большие пакеты (238-245)
    big_pkts = [w for w in writes if 238 <= len(w) <= 245]
    print(f"Всего больших пакетов: {len(big_pkts)}")
    print()

    # Анализ первых 8 пакетов разных длин
    seen_lens = set()
    samples = []
    for pkt in big_pkts:
        if len(pkt) not in seen_lens:
            seen_lens.add(len(pkt))
            samples.append(pkt)
        if len(samples) >= 8:
            break

    print("--- Расшифрованные headers (по одному на каждую длину) ---")
    for pkt in sorted(samples, key=len):
        key3 = extract_key3(pkt)
        plain = decrypt_with_key3(pkt, key3)
        info = analyze_decrypted_header(plain, len(pkt))

        print(f"\n  len={len(pkt)}, key3={key3.hex()}")
        print(f"    nonce: {info['nonce0']:02x} {info['nonce1']:02x}")
        print(f"    [2:6]: {info['byte2_5']}")
        print(f"    [6:8]: {info['byte6_7']}")
        print(f"    [8:10]: {info['byte8_9']}")
        print(f"    [10:13]: {info['byte10_12']}")
        if "extra_header" in info:
            print(f"    extra header: {info['extra_header']}")

        # Покажем первые LED данные
        color_start = len(pkt) - 225
        led_data = plain[color_start:color_start + 12]
        print(f"    color_start={color_start}, first 4 LEDs: ", end="")
        for j in range(0, 12, 3):
            r, g, b = led_data[j], led_data[j + 1], led_data[j + 2]
            print(f"({r},{g},{b}) ", end="")
        print()

        # Последние LED
        last_led = plain[-3:]
        print(f"    last LED (74): ({last_led[0]},{last_led[1]},{last_led[2]})")

    # Сравним с GenRGBTransferPackage
    print()
    print("=" * 70)
    print("  СРАВНЕНИЕ С GenRGBTransferPackage")
    print("=" * 70)

    try:
        from soulight.protocol.bridge import BeelightBridge
        bridge = BeelightBridge()
        if bridge.init():
            # Генерируем solid red
            dotnet_pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
            if dotnet_pkt:
                print(f"\n  .NET packet len={len(dotnet_pkt)}")
                # Проверяем, начинается ли с 55 AA 5A
                if dotnet_pkt[:3] == b'\x55\xAA\x5A':
                    # Пакет включает frame header
                    payload = dotnet_pkt[5:]
                    print(f"  Frame header: {dotnet_pkt[:5].hex()}")
                    print(f"  Payload len: {len(payload)}")
                    key3_dn = extract_key3(payload)
                    plain_dn = decrypt_with_key3(payload, key3_dn)
                else:
                    # Пакет = чистый payload
                    key3_dn = extract_key3(dotnet_pkt)
                    plain_dn = decrypt_with_key3(dotnet_pkt, key3_dn)
                    print(f"  No frame header, raw payload")

                print(f"  key3: {key3_dn.hex()}")
                print(f"  plain[0:15]: {plain_dn[:15].hex()}")

                # LED data
                dn_color_start = len(plain_dn) - 225
                dn_leds = plain_dn[dn_color_start:dn_color_start + 12]
                print(f"  color_start={dn_color_start}, first 4 LEDs: ", end="")
                for j in range(0, 12, 3):
                    r, g, b = dn_leds[j], dn_leds[j + 1], dn_leds[j + 2]
                    print(f"({r},{g},{b}) ", end="")
                print()
            else:
                print("  GenRGBTransferPackage returned None")
        else:
            print("  Bridge init failed")
    except Exception as e:
        print(f"  Error: {e}")

    # Анализ: есть ли checksum / MAC в пакетах
    print()
    print("=" * 70)
    print("  CHECKSUM / INTEGRITY ANALYSIS")
    print("=" * 70)

    # Для каждого размера пакетов, проверим стабильность заголовка
    from collections import defaultdict
    by_len = defaultdict(list)
    for pkt in big_pkts:
        by_len[len(pkt)].append(pkt)

    for pkt_len in sorted(by_len.keys()):
        pkts = by_len[pkt_len]
        print(f"\n  --- len={pkt_len} ({len(pkts)} packets) ---")

        # Расшифруем все и проверим header bytes
        plains = []
        for pkt in pkts:
            key3 = extract_key3(pkt)
            plain = decrypt_with_key3(pkt, key3)
            plains.append(plain)

        # Проверяем, какие позиции header стабильны
        color_start = pkt_len - 225
        stable = []
        varying = []
        for pos in range(color_start):
            values = set(p[pos] for p in plains)
            if len(values) == 1:
                stable.append((pos, list(values)[0]))
            else:
                varying.append((pos, min(values), max(values), len(values)))

        print(f"    Header bytes 0..{color_start - 1}: "
              f"{len(stable)} stable, {len(varying)} varying")

        for pos, val in stable:
            print(f"      [{pos:2d}] = 0x{val:02x} (stable)")
        for pos, mn, mx, cnt in varying[:5]:
            print(f"      [{pos:2d}] = 0x{mn:02x}..0x{mx:02x} ({cnt} unique values)")


if __name__ == "__main__":
    main()
