# -*- coding: utf-8 -*-
"""
compare_captures.py - Compare two full session captures to identify
fixed protocol bytes vs variable (nonce/key) bytes.

If the same plaintext command is encrypted with different keys in each
session, XORing the two ciphertexts gives us key1 XOR key2 (no plaintext).
But if some bytes are fixed (not encrypted), they'll be identical.
"""
import sys, os
from collections import Counter

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_write_read_pairs(filepath):
    """Parse WRITE/READ ops and pair frame headers with data."""
    ops = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            d = ""
            if "IRP_MJ_WRITE" in line and "DOWN" in line:
                d = "W"
            elif "IRP_MJ_READ" in line and "UP" in line:
                d = "R"
            else:
                continue
            data_str = parts[5].strip()
            try:
                raw = bytes.fromhex(data_str.replace(" ", "")) if data_str else b""
            except ValueError:
                raw = b""
            if raw:
                ops.append((line_no, d, raw))

    # Reassemble frame headers + data
    frames = []
    i = 0
    while i < len(ops):
        ln, d, raw = ops[i]
        if d == "R":
            # Parse embedded 55AA5A packets
            j = 0
            while j < len(raw) - 4:
                if raw[j] == 0x55 and raw[j+1] == 0xAA and raw[j+2] == 0x5A:
                    plen = raw[j+3]
                    payload = raw[j+5:j+5+plen]
                    frames.append(('R', plen, payload, ln))
                    j += 5 + plen
                else:
                    j += 1
            i += 1
        elif d == "W" and len(raw) == 5 and raw[0:3] == b'\x55\xAA\x5A':
            plen = raw[3]
            if i + 1 < len(ops) and ops[i+1][1] == "W":
                data = ops[i+1][2]
                frames.append(('W', plen, data, ln))
                i += 2
            else:
                i += 1
        else:
            i += 1
    return frames


def classify_frame(plen):
    """Classify frame by payload length."""
    if plen >= 238:
        return "COLOR"
    elif plen <= 20:
        return "CTRL"
    else:
        return "MED"


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    path1 = os.path.join(CSV_DIR, "red_full.csv")
    path2 = os.path.join(CSV_DIR, "__pycache__", "surely_full_red.csv")

    f1 = parse_write_read_pairs(path1)
    f2 = parse_write_read_pairs(path2)

    print(f"Capture 1 (red_full): {len(f1)} frames")
    print(f"Capture 2 (surely_full_red): {len(f2)} frames")

    # Find first big packet in each
    big1 = next((i for i, f in enumerate(f1) if f[1] >= 238), len(f1))
    big2 = next((i for i, f in enumerate(f2) if f[1] >= 238), len(f2))
    print(f"First color packet: cap1 at frame {big1}, cap2 at frame {big2}")

    # ================================================================
    # Compare startup sequences side by side
    # ================================================================
    print(f"\n{'='*70}")
    print("STARTUP SEQUENCE COMPARISON")
    print(f"{'='*70}")

    hs1 = [(d, pl, p) for d, pl, p, _ in f1[:big1] if d == 'W']
    hs2 = [(d, pl, p) for d, pl, p, _ in f2[:big2] if d == 'W']

    print(f"\nHandshake writes: cap1={len(hs1)}, cap2={len(hs2)}")

    # Compare by payload length sequence
    seq1 = [pl for _, pl, _ in hs1]
    seq2 = [pl for _, pl, _ in hs2]
    print(f"Length sequence cap1: {seq1}")
    print(f"Length sequence cap2: {seq2}")

    # ================================================================
    # For matching-length packets, compare byte-by-byte
    # ================================================================
    print(f"\n{'='*70}")
    print("BYTE-BY-BYTE COMPARISON OF MATCHING PACKETS")
    print(f"{'='*70}")

    # Match by position in sequence
    min_len = min(len(hs1), len(hs2))
    for i in range(min_len):
        _, pl1, p1 = hs1[i]
        _, pl2, p2 = hs2[i]

        if pl1 != pl2:
            print(f"\n  [{i}] LENGTH MISMATCH: {pl1} vs {pl2}")
            continue

        if p1 == p2:
            print(f"\n  [{i}] len={pl1} IDENTICAL: {p1.hex()}")
            continue

        # Compare byte by byte
        same_mask = []
        diff_positions = []
        for j in range(min(len(p1), len(p2))):
            if p1[j] == p2[j]:
                same_mask.append("==")
            else:
                same_mask.append(f"{p1[j]:02x}/{p2[j]:02x}")
                diff_positions.append(j)

        print(f"\n  [{i}] len={pl1}")
        print(f"    cap1: {p1.hex()}")
        print(f"    cap2: {p2.hex()}")
        xor = bytes(a ^ b for a, b in zip(p1[:len(p2)], p2[:len(p1)]))
        print(f"    XOR:  {xor.hex()}")
        print(f"    diff positions: {diff_positions}")
        print(f"    same positions: {[j for j in range(min(len(p1),len(p2))) if p1[j]==p2[j]]}")

    # ================================================================
    # Analyze the heartbeat packets specifically
    # ================================================================
    print(f"\n{'='*70}")
    print("HEARTBEAT ANALYSIS")
    print(f"{'='*70}")

    # Find the repeated packet in each capture
    for cap_name, hs in [("cap1", hs1), ("cap2", hs2)]:
        counts = Counter(p.hex() for _, _, p in hs)
        top = counts.most_common(3)
        print(f"\n  {cap_name} most repeated:")
        for hex_val, cnt in top:
            print(f"    {cnt}x: {hex_val}")

    # ================================================================
    # Analyze byte[1] more carefully - is it part of encryption or plaintext?
    # ================================================================
    print(f"\n{'='*70}")
    print("BYTE[1] DEEP ANALYSIS")
    print(f"{'='*70}")

    for cap_name, frames in [("cap1", f1), ("cap2", f2)]:
        writes = [(pl, p) for d, pl, p, _ in frames if d == 'W' and len(p) >= 2]
        b1_by_len = {}
        for pl, p in writes:
            if pl not in b1_by_len:
                b1_by_len[pl] = []
            b1_by_len[pl].append(p[1])

        print(f"\n  {cap_name} byte[1] by payload length:")
        for pl in sorted(b1_by_len.keys()):
            vals = b1_by_len[pl]
            unique = sorted(set(vals))
            print(f"    len={pl:3d}: byte[1] = {[f'{v:02x}' for v in unique[:10]]} "
                  f"(n={len(vals)})")

    # ================================================================
    # Try key hypothesis: byte[0:2] are plaintext nonce, rest is encrypted
    # If so, in two captures of same command, payload[2:] XOR should give
    # key1 XOR key2 which has period 3
    # ================================================================
    print(f"\n{'='*70}")
    print("TEST: ARE FIRST 2 BYTES PLAINTEXT NONCE?")
    print(f"{'='*70}")

    # Compare same-length, same-position packets between captures
    for i in range(min(len(hs1), len(hs2))):
        _, pl1, p1 = hs1[i]
        _, pl2, p2 = hs2[i]
        if pl1 != pl2 or pl1 < 6:
            continue
        if p1 == p2:
            continue

        # If byte[0:2] are plaintext nonce, then byte[2:] = encrypted
        # XOR of two ciphertexts: ct1 XOR ct2 = (pt XOR k1) XOR (pt XOR k2) = k1 XOR k2
        # This should be periodic with period 3
        enc1 = p1[2:]
        enc2 = p2[2:]
        min_enc = min(len(enc1), len(enc2))
        if min_enc < 6:
            continue

        xor = bytes(enc1[j] ^ enc2[j] for j in range(min_enc))

        # Check period 3
        is_p3 = True
        for j in range(3, min_enc):
            if xor[j] != xor[j % 3]:
                is_p3 = False
                break

        print(f"\n  Pair [{i}] len={pl1}:")
        print(f"    nonce1={p1[:2].hex()} nonce2={p2[:2].hex()}")
        print(f"    enc_xor={xor.hex()}")
        print(f"    period-3: {is_p3}")
        if is_p3:
            print(f"    key_xor = [{xor[0]:02x}, {xor[1]:02x}, {xor[2]:02x}]")

    # ================================================================
    # Also try: byte[0:2] encrypted, whole payload encrypted
    # XOR of full payloads should be period 3
    # ================================================================
    print(f"\n{'='*70}")
    print("TEST: FULL PAYLOAD ENCRYPTED (period-3 XOR)?")
    print(f"{'='*70}")

    for i in range(min(len(hs1), len(hs2))):
        _, pl1, p1 = hs1[i]
        _, pl2, p2 = hs2[i]
        if pl1 != pl2 or pl1 < 6:
            continue
        if p1 == p2:
            continue

        min_l = min(len(p1), len(p2))
        xor = bytes(p1[j] ^ p2[j] for j in range(min_l))

        is_p3 = True
        for j in range(3, min_l):
            if xor[j] != xor[j % 3]:
                is_p3 = False
                break

        print(f"\n  Pair [{i}] len={pl1}:")
        print(f"    full_xor={xor.hex()}")
        print(f"    period-3: {is_p3}")
        if is_p3:
            print(f"    key_xor = [{xor[0]:02x}, {xor[1]:02x}, {xor[2]:02x}]")

    # ================================================================
    # Controller responses comparison
    # ================================================================
    print(f"\n{'='*70}")
    print("CONTROLLER RESPONSE COMPARISON")
    print(f"{'='*70}")

    rd1 = [(pl, p) for d, pl, p, _ in f1[:big1] if d == 'R']
    rd2 = [(pl, p) for d, pl, p, _ in f2[:big2] if d == 'R']
    print(f"  Read frames: cap1={len(rd1)}, cap2={len(rd2)}")

    # Show response length sequences
    rseq1 = [pl for pl, _ in rd1]
    rseq2 = [pl for pl, _ in rd2]
    print(f"  Read lengths cap1: {rseq1[:30]}")
    print(f"  Read lengths cap2: {rseq2[:30]}")

    print("\nDONE")


if __name__ == "__main__":
    main()
