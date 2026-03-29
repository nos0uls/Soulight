# -*- coding: utf-8 -*-
"""
analyze_replay_csv.py — Анализ capture файла screen mirroring сессии Beelight.

Извлекает WRITE пакеты, группирует по размерам, показывает:
- распределение длин пакетов
- временные интервалы между пакетами
- первые/последние байты характерных пакетов
- поиск handshake vs color data boundary
"""
import sys
import os
from collections import Counter

CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")


def parse_writes(filepath):
    """Парсит все IRP_MJ_WRITE;DOWN операции из CSV capture."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
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
                time_str = parts[1].strip() if len(parts) > 1 else ""
                writes.append((line_no, time_str, raw))
    return writes


def main():
    print("=" * 70)
    print(f"  АНАЛИЗ CAPTURE: {os.path.basename(CSV_PATH)}")
    print("=" * 70)
    print()

    writes = parse_writes(CSV_PATH)
    print(f"Всего WRITE операций: {len(writes)}")
    print()

    # Распределение по длинам
    len_counter = Counter(len(raw) for _, _, raw in writes)
    print("--- Распределение по длинам пакетов ---")
    for length, count in sorted(len_counter.items()):
        pct = count / len(writes) * 100
        bar = "#" * min(50, int(pct))
        print(f"  {length:4d} bytes: {count:5d} ({pct:5.1f}%) {bar}")
    print()

    # Первые 20 WRITE пакетов (handshake region)
    print("--- Первые 20 WRITE пакетов (handshake) ---")
    for i, (ln, ts, raw) in enumerate(writes[:20]):
        hex_preview = raw.hex()
        if len(hex_preview) > 80:
            hex_preview = hex_preview[:80] + "..."
        print(f"  [{i:3d}] len={len(raw):4d}  {hex_preview}")
    print()

    # Найдём boundary: первый пакет >= 238 байт
    color_start_idx = None
    for i, (ln, ts, raw) in enumerate(writes):
        if len(raw) >= 238:
            color_start_idx = i
            break

    if color_start_idx is not None:
        print(f"--- Первый большой пакет (>=238) на позиции {color_start_idx} ---")
        # Покажем 5 пакетов вокруг boundary
        start = max(0, color_start_idx - 2)
        end = min(len(writes), color_start_idx + 8)
        for i in range(start, end):
            ln, ts, raw = writes[i]
            hex_preview = raw.hex()
            if len(hex_preview) > 80:
                hex_preview = hex_preview[:80] + "..."
            marker = " <<<" if i == color_start_idx else ""
            print(f"  [{i:3d}] len={len(raw):4d}  {hex_preview}{marker}")
        print()

    # Уникальные длины в color region
    if color_start_idx is not None:
        color_writes = writes[color_start_idx:]
        color_lens = Counter(len(raw) for _, _, raw in color_writes)
        print(f"--- Color region ({len(color_writes)} пакетов) ---")
        for length, count in sorted(color_lens.items()):
            print(f"  {length:4d} bytes: {count}")
        print()

        # Покажем несколько color пакетов подробнее
        big_pkts = [(i, ln, ts, raw) for i, (ln, ts, raw) in enumerate(color_writes) if len(raw) >= 238]
        print(f"--- Первые 5 больших color пакетов (header bytes) ---")
        for idx, (i, ln, ts, raw) in enumerate(big_pkts[:5]):
            print(f"  [{color_start_idx + i:3d}] len={len(raw):3d}  "
                  f"hdr={raw[:15].hex()}  tail={raw[-5:].hex()}")
        print()

        # Последние 5 больших пакетов
        print(f"--- Последние 5 больших color пакетов ---")
        for idx, (i, ln, ts, raw) in enumerate(big_pkts[-5:]):
            print(f"  [{color_start_idx + i:3d}] len={len(raw):3d}  "
                  f"hdr={raw[:15].hex()}  tail={raw[-5:].hex()}")
        print()

    # 5-byte пакеты: сколько уникальных?
    five_byte = [raw for _, _, raw in writes if len(raw) == 5]
    five_unique = set(five_byte)
    print(f"--- 5-byte пакеты: {len(five_byte)} всего, {len(five_unique)} уникальных ---")
    for pkt in sorted(five_unique):
        count = five_byte.count(pkt)
        print(f"  {pkt.hex()}: {count}x")
    print()

    # READ operations для анализа ответов контроллера
    print("--- READ operations (responses from controller) ---")
    reads = []
    with open(CSV_PATH, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
            if "IRP_MJ_READ" not in line or "UP" not in line:
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
                reads.append((line_no, raw))

    print(f"  Total READ responses: {len(reads)}")
    read_lens = Counter(len(raw) for _, raw in reads)
    for length, count in sorted(read_lens.items()):
        print(f"  {length:4d} bytes: {count}")
    # Show first few
    for i, (ln, raw) in enumerate(reads[:5]):
        print(f"  [{i}] len={len(raw)} {raw.hex()}")


if __name__ == "__main__":
    main()
