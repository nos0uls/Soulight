# -*- coding: utf-8 -*-
"""
analyze_capture_handshake.py — Анализ handshake из replay.csv и сравнение
с историческим handshake из surely_full_red.csv.

Также анализируем: какие 5-byte frame headers используются в capture,
и есть ли в capture что-то уникальное (SyncConfig, mode switch, etc).
"""
import os
import sys
import subprocess

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")
REPO_PATH = os.path.dirname(os.path.abspath(__file__))


def parse_all_ops(filepath):
    """Парсит ВСЕ операции (WRITE и READ) из capture."""
    ops = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" in line or "IRP_MJ_READ" in line:
                parts = line.split(";")
                if len(parts) <= 5:
                    continue
                op_type = "WRITE" if "WRITE" in parts[3] else "READ"
                direction = "DOWN" if "DOWN" in parts[3] else "UP"
                data_str = parts[5].strip()
                raw = b""
                if data_str:
                    try:
                        raw = bytes.fromhex(data_str.replace(" ", ""))
                    except ValueError:
                        pass
                ops.append({
                    "type": op_type,
                    "dir": direction,
                    "data": raw,
                    "len": len(raw),
                })
    return ops


def load_git_csv_writes(blob_spec):
    """Загружает WRITE операции из git blob."""
    try:
        result = subprocess.run(
            ["git", "show", blob_spec],
            capture_output=True, cwd=REPO_PATH, timeout=10
        )
        if result.returncode != 0:
            return []
        writes = []
        for line in result.stdout.decode("utf-8", errors="replace").splitlines():
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
    except Exception:
        return []


def main():
    print("=" * 70)
    print("  CAPTURE HANDSHAKE ANALYSIS")
    print("=" * 70)

    ops = parse_all_ops(CSV_PATH)
    writes_down = [op for op in ops if op["type"] == "WRITE" and op["dir"] == "DOWN"]
    reads_up = [op for op in ops if op["type"] == "READ" and op["dir"] == "UP"]

    print(f"\n  Total ops: {len(ops)}")
    print(f"  WRITE;DOWN: {len(writes_down)}")
    print(f"  READ;UP: {len(reads_up)}")

    # Показываем первые 30 операций (все типы)
    print(f"\n  --- First 30 operations (all types) ---")
    for i, op in enumerate(ops[:30]):
        data_hex = op["data"].hex() if op["data"] else "(empty)"
        if len(data_hex) > 60:
            data_hex = data_hex[:60] + "..."
        print(f"    [{i:3d}] {op['type']:5s} {op['dir']:4s} len={op['len']:3d}  {data_hex}")

    # Показываем первые 20 WRITE;DOWN
    print(f"\n  --- First 20 WRITE;DOWN packets ---")
    for i, op in enumerate(writes_down[:20]):
        data_hex = op["data"].hex()
        if len(data_hex) > 80:
            data_hex = data_hex[:80] + "..."
        print(f"    [{i:3d}] len={op['len']:3d}  {data_hex}")

    # Индекс первого большого пакета
    first_big_idx = None
    for i, op in enumerate(writes_down):
        if op["len"] >= 238:
            first_big_idx = i
            break

    if first_big_idx is not None:
        print(f"\n  First big packet (>=238) at WRITE index {first_big_idx}")
        print(f"  Handshake = first {first_big_idx} WRITE packets")

    # ================================================================
    # Сравнение с историческим handshake
    # ================================================================
    print()
    print("=" * 70)
    print("  COMPARISON WITH HISTORICAL HANDSHAKE")
    print("=" * 70)

    hist_writes = load_git_csv_writes("HEAD~30:surely_full_red.csv")
    if not hist_writes:
        hist_writes = load_git_csv_writes("HEAD~50:surely_full_red.csv")
    if not hist_writes:
        # Попробуем найти blob
        try:
            result = subprocess.run(
                ["git", "log", "--all", "--oneline", "--diff-filter=D", "--", "surely_full_red.csv"],
                capture_output=True, cwd=REPO_PATH, timeout=10
            )
            commits = result.stdout.decode().strip().splitlines()
            if commits:
                commit = commits[0].split()[0]
                hist_writes = load_git_csv_writes(f"{commit}:surely_full_red.csv")
                print(f"  Found surely_full_red.csv at commit {commit}")
        except Exception:
            pass

    if hist_writes:
        # Первые пакеты до первого большого
        hist_first_big = None
        for i, w in enumerate(hist_writes):
            if len(w) >= 238:
                hist_first_big = i
                break

        if hist_first_big:
            print(f"\n  Historical handshake: {hist_first_big} WRITE packets")
            print(f"  Capture handshake: {first_big_idx} WRITE packets")

            print(f"\n  --- Historical handshake packets ---")
            for i, w in enumerate(hist_writes[:min(hist_first_big, 20)]):
                print(f"    [{i:3d}] len={len(w):3d}  {w.hex()[:80]}")

            # Сравниваем уникальные 5-byte пакеты
            hist_5b = set(w.hex() for w in hist_writes[:hist_first_big] if len(w) == 5)
            cap_5b = set(
                writes_down[i]["data"].hex()
                for i in range(first_big_idx)
                if writes_down[i]["len"] == 5
            )

            print(f"\n  Historical unique 5-byte: {hist_5b}")
            print(f"  Capture unique 5-byte:    {cap_5b}")
            print(f"  In capture but not hist:  {cap_5b - hist_5b}")
            print(f"  In hist but not capture:  {hist_5b - cap_5b}")
    else:
        print("  Could not load historical handshake")

    # ================================================================
    # Анализ 5-byte frame headers из capture (перед каждым большим)
    # ================================================================
    print()
    print("=" * 70)
    print("  5-BYTE FRAME HEADERS IN CAPTURE")
    print("=" * 70)

    from collections import Counter
    frame_headers = Counter()
    for i, op in enumerate(writes_down):
        if op["len"] == 5:
            frame_headers[op["data"].hex()] += 1

    print(f"\n  Total 5-byte packets: {sum(frame_headers.values())}")
    for hdr, count in frame_headers.most_common(20):
        raw = bytes.fromhex(hdr)
        if raw[:3] == b'\x55\xAA\x5A':
            payload_len = raw[3]
            print(f"    {hdr}  count={count:3d}  (55AA5A frame, payload_len={payload_len})")
        else:
            print(f"    {hdr}  count={count:3d}")

    # ================================================================
    # Уникальные non-frame 5-byte пакеты (control commands)
    # ================================================================
    print()
    print("=" * 70)
    print("  CONTROL COMMANDS (5-byte, not 55AA5A frames)")
    print("=" * 70)

    for hdr, count in frame_headers.most_common():
        raw = bytes.fromhex(hdr)
        if raw[:3] != b'\x55\xAA\x5A':
            # Расшифровываем 3-byte XOR
            key3 = bytes([raw[3], raw[4], raw[2]])
            plain = bytes(raw[i] ^ key3[i % 3] for i in range(5))
            print(f"    {hdr}  count={count:3d}  plain={plain.hex()}")


if __name__ == "__main__":
    main()
