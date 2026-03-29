# -*- coding: utf-8 -*-
"""
analyze_capture_v2.py — Правильный парсер replay.csv.

Формат CSV:
  col[0]=#, col[1]=Time, col[2]=Function, col[3]=Direction, col[4]=Status,
  col[5]=Data, col[6]=Data(chars), col[7]=Data length, col[8]=Req length,
  col[9]=Port, col[10]=Comments

Анализируем полную WRITE;DOWN последовательность, включая handshake,
и определяем точный протокол screen mirroring.
"""
import os
import sys
from collections import Counter, defaultdict

CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")


def parse_capture(filepath):
    """Парсит ВСЕ операции из capture CSV."""
    ops = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f, 1):
            if line_no == 1:
                continue  # header
            parts = line.split(";")
            if len(parts) < 6:
                continue
            func = parts[2].strip()
            direction = parts[3].strip()
            data_str = parts[5].strip()
            raw = b""
            if data_str:
                try:
                    raw = bytes.fromhex(data_str.replace(" ", ""))
                except ValueError:
                    pass
            ops.append({
                "line": line_no,
                "func": func,
                "dir": direction,
                "data": raw,
                "len": len(raw),
            })
    return ops


def main():
    ops = parse_capture(CSV_PATH)

    # Фильтруем WRITE;DOWN и READ;UP
    writes = [op for op in ops if "IRP_MJ_WRITE" in op["func"] and op["dir"] == "DOWN"]
    reads = [op for op in ops if "IRP_MJ_READ" in op["func"] and op["dir"] == "UP"]

    print("=" * 70)
    print("  CAPTURE v2 ANALYSIS — replay.csv")
    print("=" * 70)
    print(f"\n  Total operations: {len(ops)}")
    print(f"  WRITE;DOWN: {len(writes)}")
    print(f"  READ;UP: {len(reads)}")

    # ================================================================
    # 1. Полная WRITE;DOWN последовательность
    # ================================================================
    print()
    print("=" * 70)
    print("  1. ALL WRITE;DOWN PACKETS")
    print("=" * 70)

    by_len = Counter()
    for w in writes:
        by_len[w["len"]] += 1

    print(f"\n  Packet length distribution:")
    for length, count in sorted(by_len.items()):
        print(f"    len={length:3d}: {count:4d} packets")

    # Индекс первого большого пакета (>=238)
    first_big = None
    for i, w in enumerate(writes):
        if w["len"] >= 238:
            first_big = i
            break

    print(f"\n  First big packet (>=238) at WRITE index {first_big}")
    print(f"  Handshake packets: {first_big}")

    # ================================================================
    # 2. HANDSHAKE — все пакеты до первого большого
    # ================================================================
    print()
    print("=" * 70)
    print("  2. HANDSHAKE PACKETS (before first big)")
    print("=" * 70)

    if first_big:
        for i, w in enumerate(writes[:first_big]):
            data = w["data"]
            print(f"\n    [{i:2d}] line={w['line']:4d}  len={w['len']:3d}")
            print(f"         hex: {data.hex()}")

            # Определяем тип пакета
            if data[:3] == b'\x55\xAA\x5A':
                payload_len = data[3]
                print(f"         type: FRAME HEADER (55AA5A), payload_len={payload_len}")
            elif w["len"] == 13:
                # Decrypt with key3
                key3 = bytes([data[3], data[4], data[2]])
                plain = bytes(data[j] ^ key3[j % 3] for j in range(len(data)))
                print(f"         type: 13-byte control, plain={plain.hex()}")
            elif w["len"] == 18:
                key3 = bytes([data[3], data[4], data[2]])
                plain = bytes(data[j] ^ key3[j % 3] for j in range(len(data)))
                print(f"         type: 18-byte control, plain={plain.hex()}")
            elif w["len"] == 10:
                key3 = bytes([data[3], data[4], data[2]])
                plain = bytes(data[j] ^ key3[j % 3] for j in range(len(data)))
                print(f"         type: 10-byte control, plain={plain.hex()}")
            elif w["len"] == 12:
                key3 = bytes([data[3], data[4], data[2]])
                plain = bytes(data[j] ^ key3[j % 3] for j in range(len(data)))
                print(f"         type: 12-byte control, plain={plain.hex()}")
            else:
                print(f"         type: unknown ({w['len']} bytes)")

    # ================================================================
    # 3. READ responses from controller during handshake
    # ================================================================
    print()
    print("=" * 70)
    print("  3. READ RESPONSES DURING HANDSHAKE")
    print("=" * 70)

    if first_big:
        hs_end_line = writes[first_big]["line"]
        hs_reads = [r for r in reads if r["line"] < hs_end_line and r["len"] > 0]
        for r in hs_reads:
            data = r["data"]
            # Может содержать несколько 55AA5A фреймов
            pos = 0
            frames = []
            while pos < len(data):
                if pos + 3 <= len(data) and data[pos:pos + 3] == b'\x55\xAA\x5A':
                    pl = data[pos + 3]
                    frame_end = pos + 5 + pl
                    if frame_end <= len(data):
                        frames.append(data[pos:frame_end])
                        pos = frame_end
                        continue
                pos += 1

            print(f"\n    line={r['line']:4d}  len={r['len']:3d}  frames={len(frames)}")
            for fi, frame in enumerate(frames):
                hdr = frame[:5]
                payload = frame[5:]
                print(f"      frame[{fi}]: hdr={hdr.hex()}  payload_len={len(payload)}")
                if len(payload) >= 5:
                    key3 = bytes([payload[3], payload[4], payload[2]])
                    plain = bytes(payload[j] ^ key3[j % 3] for j in range(len(payload)))
                    print(f"              payload_plain={plain.hex()}")

    # ================================================================
    # 4. First 5 big packets — проверка структуры
    # ================================================================
    print()
    print("=" * 70)
    print("  4. FIRST 5 BIG PACKETS (color data)")
    print("=" * 70)

    # Большие пакеты — смотрим, идут ли они после frame header
    for offset in range(5):
        idx = first_big + offset
        if idx >= len(writes):
            break
        w = writes[idx]
        data = w["data"]
        pkt_len = w["len"]

        # Ищем предшествующий frame header
        prev = writes[idx - 1] if idx > 0 else None
        prev_is_frame = (prev and prev["len"] == 5 and
                         prev["data"][:3] == b'\x55\xAA\x5A')

        period = pkt_len - 235
        color_start = pkt_len - 225

        # Decrypt with correct key (for .NET format, plain[2:2+period]=0)
        # Но для capture пакетов это может быть неточно!
        # Используем key3 (period=3 subset) для stable header bytes
        key3 = bytes([data[3], data[4], data[2]])
        plain3 = bytes(data[j] ^ key3[j % 3] for j in range(pkt_len))

        print(f"\n    [{offset}] line={w['line']:4d}  len={pkt_len}  prev_frame={prev_is_frame}")
        if prev_is_frame:
            print(f"         frame_hdr: {prev['data'].hex()}")
        print(f"         cipher[0:10]: {data[:10].hex()}")
        print(f"         key3_plain[0:{color_start}]: {plain3[:color_start].hex()}")

        # LED data с key3 (частично верно)
        print(f"         key3 LED[0:6]: ({plain3[color_start]},{plain3[color_start+1]},{plain3[color_start+2]}) "
              f"({plain3[color_start+3]},{plain3[color_start+4]},{plain3[color_start+5]})")

    # ================================================================
    # 5. Сравнение handshake с историческим
    # ================================================================
    print()
    print("=" * 70)
    print("  5. HANDSHAKE STRUCTURE COMPARISON")
    print("=" * 70)

    # Расшифруем все handshake пакеты и покажем LP commands
    if first_big:
        print(f"\n  Capture handshake ({first_big} writes):")
        hs_pairs = []  # (frame_header, payload)
        i = 0
        hs_writes = writes[:first_big]
        while i < len(hs_writes):
            w = hs_writes[i]
            if w["data"][:3] == b'\x55\xAA\x5A':
                # Frame header, следующий write = payload
                if i + 1 < len(hs_writes):
                    hs_pairs.append((w["data"], hs_writes[i + 1]["data"]))
                    i += 2
                    continue
            else:
                # Standalone payload (no frame header)
                hs_pairs.append((None, w["data"]))
            i += 1

        for pi, (fh, payload) in enumerate(hs_pairs):
            if len(payload) >= 5:
                key3 = bytes([payload[3], payload[4], payload[2]])
                plain = bytes(payload[j] ^ key3[j % 3] for j in range(len(payload)))
                # LP command bytes are typically at positions [5:7] or similar
                print(f"    pair[{pi}]: frame={fh.hex() if fh else 'None':>14s}  "
                      f"payload_len={len(payload):3d}  "
                      f"plain={plain.hex()}")


if __name__ == "__main__":
    main()
