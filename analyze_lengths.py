# -*- coding: utf-8 -*-
"""
analyze_lengths.py — Анализ пакетов разной длины (238-244).

Используем BLACK захват (plaintext LED = 0x00) для извлечения
plaintext заголовков пакетов всех длин.
"""
import sys
import os
from collections import defaultdict

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_long_writes(filepath):
    pkts = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE;DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            data = parts[5].strip()
            try:
                raw = bytes.fromhex(data.replace(" ", ""))
            except ValueError:
                continue
            if len(raw) > 10:
                pkts.append(raw)
    return pkts


def find_best_triplet(pkt, min_count=10):
    best_start, best_cnt, best_trip = 0, 0, b""
    for start in range(min(25, len(pkt) - 5)):
        trip = pkt[start:start + 3]
        cnt = 1
        pos = start + 3
        while pos + 2 < len(pkt) and pkt[pos:pos + 3] == trip:
            cnt += 1
            pos += 3
        if cnt > best_cnt:
            best_cnt = cnt
            best_start = start
            best_trip = trip
    if best_cnt >= min_count:
        return best_start, best_cnt, best_trip
    return None


def decrypt_with_key3(data, key3, offset=0):
    """Расшифровывает data XOR с key3 начиная с заданного offset в keystream."""
    return bytes([data[i] ^ key3[(offset + i) % 3] for i in range(len(data))])


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    # region ===== Загрузка BLACK пакетов =====
    black_path = os.path.join(CSV_DIR, "black.csv")
    black_pkts = parse_long_writes(black_path)

    by_len = defaultdict(list)
    for p in black_pkts:
        by_len[len(p)].append(p)
    # endregion

    # region ===== Анализ каждой длины =====
    print("=" * 70)
    print("PLAINTEXT СТРУКТУРА ПАКЕТОВ ПО ДЛИНАМ (из BLACK)")
    print("=" * 70)

    for plen in sorted(by_len.keys()):
        pkts = by_len[plen]
        print(f"\n### Длина {plen}: {len(pkts)} пакетов ###")

        # Ищем пакеты с хорошими повторяющимися триплетами
        decoded_headers = []
        decoded_tails = []

        for pkt in pkts:
            result = find_best_triplet(pkt, min_count=40)
            if result is None:
                continue
            start, cnt, trip = result
            K0, K1, K2 = trip
            key3 = bytes([K0, K1, K2])

            # Расшифровываем заголовок (всё до начала LED)
            hdr_cipher = pkt[:start]
            hdr_plain = decrypt_with_key3(hdr_cipher, key3, offset=0)

            # Расшифровываем хвост (после LED)
            tail_start = start + cnt * 3
            tail_cipher = pkt[tail_start:]
            tail_plain = decrypt_with_key3(tail_cipher, key3, offset=tail_start)

            decoded_headers.append(hdr_plain)
            decoded_tails.append(tail_plain)

        if not decoded_headers:
            print("  Нет пакетов с достаточным повтором триплетов")
            # Показываем сырые байты
            for pkt in pkts[:2]:
                h = " ".join(f"{b:02x}" for b in pkt[:30])
                print(f"  Raw: [{h}]...")
            continue

        print(f"  Расшифровано: {len(decoded_headers)} пакетов")

        # Находим фиксированные позиции в заголовке
        hdr_len = len(decoded_headers[0])
        print(f"  Header length: {hdr_len} байт")
        print(f"  Header structure:")

        for pos in range(hdr_len):
            vals = set(h[pos] for h in decoded_headers if pos < len(h))
            if len(vals) == 1:
                v = list(vals)[0]
                # Аннотация известных значений
                note = ""
                if v == 75:
                    note = " (= NUM_LEDS)"
                elif v == 5:
                    note = " (= LP_CMD_CTRL_DEVICE?)"
                elif v == 0xFF:
                    note = " (= 0xFF)"
                elif v == 0xE3:
                    note = " (= 227)"
                elif v == 0:
                    note = " (= 0x00)"
                print(f"    [{pos:2d}] = 0x{v:02x} ({v:3d}) FIXED{note}")
            else:
                samples = [f"0x{h[pos]:02x}" for h in decoded_headers[:5] if pos < len(h)]
                print(f"    [{pos:2d}] = [{', '.join(samples)}] VARIABLE ({len(vals)} values)")

        # Tail
        if decoded_tails and decoded_tails[0]:
            tail_len = len(decoded_tails[0])
            print(f"  Tail length: {tail_len} байт")
            for pos in range(tail_len):
                vals = set(t[pos] for t in decoded_tails if pos < len(t))
                if len(vals) == 1:
                    v = list(vals)[0]
                    print(f"    tail[{pos}] = 0x{v:02x} FIXED")
                else:
                    samples = [f"0x{t[pos]:02x}" for t in decoded_tails[:5] if pos < len(t)]
                    print(f"    tail[{pos}] = [{', '.join(samples)}] VARIABLE")

        # Подсчёт LED-байтов
        led_bytes = plen - hdr_len - (len(decoded_tails[0]) if decoded_tails else 0)
        led_count = led_bytes // 3
        remainder = led_bytes % 3
        print(f"  LED bytes: {led_bytes} = {led_count} LEDs + {remainder} extra")
    # endregion

    # region ===== Сводная таблица =====
    print("\n" + "=" * 70)
    print("СВОДНАЯ ТАБЛИЦА")
    print("=" * 70)
    print(f"{'Len':>4s} | {'Hdr':>3s} | {'LED bytes':>9s} | {'LEDs':>4s} | {'Tail':>4s} | {'b1 cipher':>9s}")
    print("-" * 50)

    for plen in sorted(by_len.keys()):
        pkts = by_len[plen]
        # byte[1] value
        b1_vals = set(p[1] for p in pkts)
        b1_str = ", ".join(f"0x{v:02x}" for v in sorted(b1_vals))

        # Find a solid packet to determine structure
        for pkt in pkts:
            result = find_best_triplet(pkt, min_count=40)
            if result:
                start, cnt, trip = result
                tail_len = plen - start - cnt * 3
                led_bytes = cnt * 3
                print(f"{plen:4d} | {start:3d} | {led_bytes:9d} | {cnt:4d} | {tail_len:4d} | {b1_str}")
                break
        else:
            print(f"{plen:4d} |  ?? |        ?? |   ?? |   ?? | {b1_str}")
    # endregion


if __name__ == "__main__":
    main()
