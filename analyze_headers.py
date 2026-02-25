# -*- coding: utf-8 -*-
"""
analyze_headers.py — Детальный анализ заголовков захваченных пакетов.

Цель: понять реальный формат пакета и как он соотносится с FUN_10001880.

FUN_10001880 строит:
  [0-1]  magic 0x674D
  [2]    (data_len + 10) low
  [3]    (data_len + 10) high
  [4]    ???
  [5-10] IV (6 random bytes) - NOT encrypted
  [11+]  cmd, attr, rand, rand, data... - ENCRYPTED

Но в захватах:
  - Есть heartbeat 55 AA 5A [byte] 00 (5 bytes)
  - Есть data packets 238-245 bytes
  
Вопрос: data packets содержат ТОЖЕ префикс 55 AA 5A?
Если да, то формат может быть: 55 AA 5A + [FUN_10001880 output]
"""
import sys
import os
from collections import Counter

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_all_writes(filepath):
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
            if len(raw) >= 5:
                pkts.append(raw)
    return pkts


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    colors = ["black", "red", "green", "blue", "white"]
    
    for color in colors:
        path = os.path.join(CSV_DIR, f"{color}.csv")
        if not os.path.exists(path):
            continue
        pkts = parse_all_writes(path)
        
        print(f"\n{'='*70}")
        print(f"{color.upper()}: {len(pkts)} packets total")
        
        # Separate heartbeats from data
        heartbeats = [p for p in pkts if len(p) == 5 and p[0] == 0x55 and p[1] == 0xAA and p[2] == 0x5A]
        data_pkts = [p for p in pkts if len(p) > 10]
        
        print(f"  Heartbeats: {len(heartbeats)}")
        print(f"  Data packets: {len(data_pkts)}")
        
        if not data_pkts:
            continue
        
        # Check if data packets start with 55 AA 5A
        starts_55aa5a = sum(1 for p in data_pkts if p[0] == 0x55 and p[1] == 0xAA and p[2] == 0x5A)
        print(f"  Data starting with 55 AA 5A: {starts_55aa5a}")
        
        # Check byte[0] and byte[1] values
        b0_vals = Counter(p[0] for p in data_pkts)
        b1_vals = Counter(p[1] for p in data_pkts)
        
        print(f"  byte[0] values: {dict(sorted((f'0x{k:02x}', v) for k, v in b0_vals.items()))}")
        print(f"  byte[1] values: {dict(sorted((f'0x{k:02x}', v) for k, v in b1_vals.items()))}")
        
        # Show first 5 data packet headers (full first 20 bytes)
        for idx, pkt in enumerate(data_pkts[:5]):
            hdr = ' '.join(f'{b:02x}' for b in pkt[:20])
            print(f"  Pkt {idx} (len={len(pkt)}): [{hdr}]")
        
        # Check for 0x4D67 magic anywhere in first 10 bytes
        print(f"\n  Looking for magic 0x4D67 (LE: 4d 67):")
        for idx, pkt in enumerate(data_pkts[:5]):
            for i in range(min(10, len(pkt) - 1)):
                if pkt[i] == 0x4D and pkt[i+1] == 0x67:
                    print(f"    Pkt {idx}: found at offset {i}")
                elif pkt[i] == 0x67 and pkt[i+1] == 0x4D:
                    print(f"    Pkt {idx}: found REVERSED at offset {i}")
    
    # Deep analysis on BLACK 238-byte packets
    print(f"\n{'='*70}")
    print("DEEP ANALYSIS: BLACK 238-byte packets")
    print(f"{'='*70}")
    
    black_pkts = parse_all_writes(os.path.join(CSV_DIR, "black.csv"))
    solid = [p for p in black_pkts if len(p) == 238]
    
    # byte[1] is always 0x32 from earlier analysis. Check all positions for consistency
    print("\nByte-by-byte consistency (first 20 positions):")
    for pos in range(min(20, len(solid[0]))):
        vals = set(p[pos] for p in solid)
        if len(vals) == 1:
            v = list(vals)[0]
            print(f"  [{pos:2d}] = 0x{v:02x} ({v:3d}) CONSTANT across all {len(solid)} packets")
        else:
            print(f"  [{pos:2d}] = {len(vals):3d} unique values (variable)")
    
    # Look at the actual write sequence — are heartbeat+data interleaved?
    print(f"\nWrite sequence (first 20 writes, any size):")
    all_writes = parse_all_writes(os.path.join(CSV_DIR, "black.csv"))
    for idx, pkt in enumerate(all_writes[:20]):
        ptype = "HB" if len(pkt) == 5 else f"D{len(pkt):3d}"
        hdr = ' '.join(f'{b:02x}' for b in pkt[:min(15, len(pkt))])
        print(f"  [{idx:2d}] {ptype}: [{hdr}]")


if __name__ == "__main__":
    main()
