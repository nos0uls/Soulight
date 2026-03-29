# -*- coding: utf-8 -*-
"""
analyze_replay_structure.py — Детальный анализ структуры пакетов screen mirroring capture.

Цели:
  1. Подтвердить shifted header pattern (05 05 ... 4B) для всех размеров
  2. Проанализировать "extra header bytes" между [5] и 05 05 marker
  3. Проверить, зависят ли extra bytes от LED data (checksum hypothesis)
  4. Сравнить с .NET GenRGBTransferPackage структурой
"""
import os
import sys
from collections import defaultdict

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")


def parse_writes(filepath):
    """Парсит WRITE;DOWN пакеты."""
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


def key3_decrypt(pkt):
    """Расшифровка простым 3-byte XOR (key из cipher[2:5])."""
    if len(pkt) < 5:
        return pkt
    key3 = bytes([pkt[3], pkt[4], pkt[2]])
    return bytes(pkt[i] ^ key3[i % 3] for i in range(len(pkt)))


def main():
    writes = parse_writes(CSV_PATH)
    big_pkts = [w for w in writes if 238 <= len(w) <= 245]

    by_len = defaultdict(list)
    for pkt in big_pkts:
        by_len[len(pkt)].append(pkt)

    # ============================================================
    # 1. Подтверждаем shifted header pattern для каждого размера
    # ============================================================
    print("=" * 70)
    print("  1. SHIFTED HEADER PATTERN ANALYSIS")
    print("=" * 70)

    for pkt_len in sorted(by_len.keys()):
        pkts = by_len[pkt_len]
        extra = pkt_len - 238
        color_start = pkt_len - 225

        # Расшифровываем все пакеты
        plains = [key3_decrypt(p) for p in pkts]

        # Ищем 05 05 pattern
        found_0505 = False
        for pos in range(5, color_start):
            vals = set(p[pos] for p in plains)
            vals_next = set(p[pos + 1] for p in plains) if pos + 1 < color_start else set()
            if vals == {0x05} and vals_next == {0x05}:
                found_0505 = True
                marker_pos = pos
                break

        # Ищем 4B (75) pattern
        found_4b = False
        for pos in range(5, color_start):
            vals = set(p[pos] for p in plains)
            if vals == {0x4B}:
                found_4b = True
                led_count_pos = pos
                break

        print(f"\n  len={pkt_len}, extra={extra}, color_start={color_start}, "
              f"n_packets={len(pkts)}")
        if found_0505:
            print(f"    05 05 at positions [{marker_pos},{marker_pos + 1}] "
                  f"(color_start - {color_start - marker_pos})")
        else:
            print(f"    05 05 NOT FOUND in header!")
        if found_4b:
            print(f"    4B at position [{led_count_pos}] "
                  f"(color_start - {color_start - led_count_pos})")
        else:
            print(f"    4B NOT FOUND in header!")

        # Покажем полный decrypted header для первого пакета
        p = plains[0]
        hdr = p[:color_start]
        print(f"    Full header: {hdr.hex()}")

        # Стабильность каждого byte
        header_info = []
        for pos in range(color_start):
            vals = set(p2[pos] for p2 in plains)
            if len(vals) == 1:
                header_info.append(f"{list(vals)[0]:02x}")
            else:
                header_info.append(f"**")
        print(f"    Stability:  {' '.join(header_info)}")

    # ============================================================
    # 2. Extra header bytes — что в них?
    # ============================================================
    print()
    print("=" * 70)
    print("  2. EXTRA HEADER BYTES ANALYSIS")
    print("=" * 70)

    for pkt_len in sorted(by_len.keys()):
        if pkt_len == 238:
            continue  # Нет extra bytes
        pkts = by_len[pkt_len]
        extra = pkt_len - 238
        color_start = pkt_len - 225
        plains = [key3_decrypt(p) for p in pkts]

        print(f"\n  len={pkt_len}, extra bytes={extra} at positions [5:{5 + extra}]")

        # Собираем extra bytes для всех пакетов
        extras = [p[5:5 + extra] for p in plains]

        # Есть ли повторяющиеся значения?
        unique_extras = set(tuple(e) for e in extras)
        print(f"    {len(extras)} packets, {len(unique_extras)} unique extra byte patterns")

        # Показываем первые 5
        for i, e in enumerate(extras[:5]):
            led_data = plains[i][color_start:]
            # XOR сумма LED data
            xor_sum = 0
            for b in led_data:
                xor_sum ^= b
            # Простая сумма LED data (mod 256)
            byte_sum = sum(led_data) % 256
            print(f"    [{i}] extra={e.hex():>{extra * 2}s}  "
                  f"xor_led={xor_sum:02x}  sum_led={byte_sum:02x}")

    # ============================================================
    # 3. Checksum hypothesis: correlate extra bytes with LED data
    # ============================================================
    print()
    print("=" * 70)
    print("  3. CHECKSUM HYPOTHESIS: CORRELATION TEST")
    print("=" * 70)

    # Для 239-byte packets (1 extra byte), проверяем корреляцию
    for pkt_len in [239, 240, 241]:
        if pkt_len not in by_len:
            continue
        pkts = by_len[pkt_len]
        extra = pkt_len - 238
        color_start = pkt_len - 225
        plains = [key3_decrypt(p) for p in pkts]

        print(f"\n  len={pkt_len}: testing if extra bytes correlate with LED data")

        # Вычисляем разные хеши LED data и сравниваем с extra bytes
        for i, p in enumerate(plains[:10]):
            extra_bytes = p[5:5 + extra]
            led_data = p[color_start:]

            xor_all = 0
            sum_all = 0
            sum_r = sum_g = sum_b = 0
            for j in range(0, 225, 3):
                r, g, b = led_data[j], led_data[j + 1], led_data[j + 2]
                xor_all ^= r ^ g ^ b
                sum_all += r + g + b
                sum_r += r
                sum_g += g
                sum_b += b

            print(f"    [{i:2d}] extra={extra_bytes.hex():<14s} "
                  f"xor={xor_all:02x} sum8={sum_all % 256:02x} "
                  f"sumR8={sum_r % 256:02x} sumG8={sum_g % 256:02x} sumB8={sum_b % 256:02x}")

    # ============================================================
    # 4. Сравнение с .NET GenRGBTransferPackage
    # ============================================================
    print()
    print("=" * 70)
    print("  4. .NET GenRGBTransferPackage COMPARISON")
    print("=" * 70)

    try:
        from soulight.protocol.bridge import BeelightBridge
        bridge = BeelightBridge()
        if not bridge.init():
            print("  Bridge init failed")
            return

        # Генерируем solid red и solid green
        for name, color in [("RED", (255, 0, 0)), ("GREEN", (0, 255, 0)), ("BLUE", (0, 0, 255))]:
            pkt = bridge.make_rgb_transfer_packet([color] * 75)
            if pkt is None:
                print(f"  {name}: GenRGBTransferPackage returned None")
                continue

            # Проверяем frame header
            if pkt[:3] == b'\x55\xAA\x5A':
                payload = pkt[5:]
                frame_hdr = pkt[:5]
            else:
                payload = pkt
                frame_hdr = None

            plain = key3_decrypt(payload)
            pkt_len = len(payload)
            color_start = pkt_len - 225

            print(f"\n  .NET {name}: payload_len={pkt_len}, color_start={color_start}")
            if frame_hdr:
                print(f"    frame_header: {frame_hdr.hex()}")
            print(f"    plain header: {plain[:color_start].hex()}")

            # Check LED data
            led0 = plain[color_start:color_start + 3]
            led1 = plain[color_start + 3:color_start + 6]
            led74 = plain[-3:]
            print(f"    LED[0]=({led0[0]},{led0[1]},{led0[2]}) "
                  f"LED[1]=({led1[0]},{led1[1]},{led1[2]}) "
                  f"LED[74]=({led74[0]},{led74[1]},{led74[2]})")

            # Stability check: same header?
            hdr_hex = plain[:color_start].hex()
            print(f"    header hex: {hdr_hex}")

    except Exception as e:
        print(f"  Error: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    main()
