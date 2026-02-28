# -*- coding: utf-8 -*-
"""
crack_wire_v2.py - Deep analysis of wire cipher.

The 3-byte XOR doesn't work perfectly. Let's look at the actual
keystream pattern more carefully.
"""
import sys
import os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_paired_writes(filepath):
    writes = []
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
            writes.append(raw)

    pairs = []
    for i in range(len(writes) - 1):
        w1 = writes[i]
        w2 = writes[i + 1]
        if (len(w1) == 5 and w1[0] == 0x55 and w1[1] == 0xAA
                and w1[2] == 0x5A and w1[3] == len(w2)):
            pairs.append((w1, w2))
    return pairs


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    black_pairs = [(h, p) for h, p in parse_paired_writes(os.path.join(CSV_DIR, "black.csv"))
                   if len(p) == 238]

    print(f"BLACK 238-byte pairs: {len(black_pairs)}")

    # For BLACK 238-byte, plaintext bytes[12:237] are all 0x00
    # So ciphertext[12:237] = keystream[12:237] directly

    # Let's examine the keystream for one packet in detail
    pkt = black_pairs[0][1]
    ks = list(pkt)  # Full 238 bytes = keystream XOR plaintext

    print(f"\nPacket 0 full keystream (cipher = ks since plain LED = 0):")
    print(f"  First 20: {' '.join(f'{ks[i]:02x}' for i in range(20))}")

    # The LED region: bytes 12-236 (225 bytes = 75 triplets)
    led_ks = ks[12:237]

    # Check for ANY repeating pattern
    print(f"\n  LED keystream (first 30): {' '.join(f'{b:02x}' for b in led_ks[:30])}")
    print(f"  LED keystream (last 15):  {' '.join(f'{b:02x}' for b in led_ks[-15:])}")

    # Check period-3 errors in detail
    k3 = led_ks[:3]
    print(f"\n  Assumed key3 = [{k3[0]:02x}, {k3[1]:02x}, {k3[2]:02x}]")
    print(f"  Checking where period-3 breaks:")

    first_error = None
    for i in range(len(led_ks)):
        if led_ks[i] != k3[i % 3]:
            if first_error is None:
                first_error = i
            if i < 30 or (i > 0 and led_ks[i-1] == k3[(i-1) % 3]):
                # Show error with context
                expected = k3[i % 3]
                actual = led_ks[i]
                print(f"    [{i:3d}] pos%3={i%3} expected=0x{expected:02x} got=0x{actual:02x} "
                      f"diff=0x{expected^actual:02x}")

    print(f"\n  First error at LED byte {first_error} (absolute byte {first_error + 12})")

    # Maybe the keystream has a LONGER period?
    # Try autocorrelation
    print(f"\n  Autocorrelation of LED keystream:")
    for period in range(1, 50):
        matches = sum(1 for i in range(len(led_ks) - period) if led_ks[i] == led_ks[i + period])
        total = len(led_ks) - period
        pct = matches / total * 100
        if pct > 50:
            print(f"    period={period:2d}: {matches}/{total} matches ({pct:.1f}%)")

    # XOR consecutive triplets to see difference pattern
    print(f"\n  Triplet differences (triplet[i] XOR triplet[i+1]):")
    for i in range(0, min(30, len(led_ks) - 3), 3):
        d0 = led_ks[i] ^ led_ks[i + 3]
        d1 = led_ks[i + 1] ^ led_ks[i + 4]
        d2 = led_ks[i + 2] ^ led_ks[i + 5]
        marker = "" if d0 == 0 and d1 == 0 and d2 == 0 else " *"
        print(f"    [{i//3:2d}] {led_ks[i]:02x} {led_ks[i+1]:02x} {led_ks[i+2]:02x} "
              f"-> {led_ks[i+3]:02x} {led_ks[i+4]:02x} {led_ks[i+5]:02x} "
              f"diff={d0:02x} {d1:02x} {d2:02x}{marker}")

    # Compare TWO different BLACK packets
    print(f"\n{'='*60}")
    print("Comparing two BLACK packets (C1 XOR C2 = KS1 XOR KS2)")
    print(f"{'='*60}")

    pkt1 = black_pairs[0][1]
    pkt2 = black_pairs[1][1]

    xor12 = bytes(a ^ b for a, b in zip(pkt1, pkt2))
    print(f"  P1[0:6] = {pkt1[:6].hex()}")
    print(f"  P2[0:6] = {pkt2[:6].hex()}")
    print(f"  XOR[0:6] = {xor12[:6].hex()}")
    print(f"  XOR[12:24] = {' '.join(f'{b:02x}' for b in xor12[12:24])}")

    # If both use same keystream, XOR would be 0 in LED region
    led_xor = xor12[12:237]
    zero_in_xor = sum(1 for b in led_xor if b == 0)
    print(f"  Zeros in LED XOR: {zero_in_xor}/225 ({zero_in_xor/225*100:.1f}%)")

    # Multiple packet analysis: look for structure
    print(f"\n{'='*60}")
    print("All BLACK 238-byte packets: LED triplet[0] values")
    print(f"{'='*60}")

    for pidx, (hdr, payload) in enumerate(black_pairs[:10]):
        # First few LED triplets
        trips = []
        for i in range(0, min(18, len(payload) - 12), 3):
            t = (payload[12+i], payload[12+i+1], payload[12+i+2])
            trips.append(f"{t[0]:02x}{t[1]:02x}{t[2]:02x}")
        
        # Header bytes
        hdr_hex = ' '.join(f'{payload[i]:02x}' for i in range(12))
        print(f"  [{pidx:2d}] hdr=[{hdr_hex}] trips={' '.join(trips)}")

    # Check: is there a relationship between header bytes and the keystream?
    # byte[1] is always 0x32 for 238-byte packets
    # So plaintext[1] XOR key3[1] = 0x32 for all packets
    # If plaintext[1] is constant (part of nonce), then key3[1] is constant
    # But key3 changes per packet... unless plaintext[1] also changes
    
    print(f"\n{'='*60}")
    print("Header byte analysis")
    print(f"{'='*60}")
    
    for pos in range(12):
        vals = set(p[pos] for _, p in black_pairs)
        if len(vals) == 1:
            v = list(vals)[0]
            print(f"  byte[{pos:2d}]: CONSTANT 0x{v:02x}")
        else:
            print(f"  byte[{pos:2d}]: {len(vals)} unique values "
                  f"(range 0x{min(vals):02x}-0x{max(vals):02x})")

    # The 18 packets that had perfect period-3: what's special about them?
    print(f"\n{'='*60}")
    print("Perfect period-3 vs non-perfect packets")
    print(f"{'='*60}")
    
    perfect = []
    imperfect = []
    
    for pidx, (hdr, payload) in enumerate(black_pairs):
        k3 = [payload[12], payload[13], payload[14]]
        errors = sum(1 for i in range(225) if payload[12 + i] != k3[i % 3])
        if errors == 0:
            perfect.append((pidx, hdr, payload))
        else:
            imperfect.append((pidx, hdr, payload, errors))
    
    print(f"  Perfect: {len(perfect)}, Imperfect: {len(imperfect)}")
    
    # What do the headers look like for perfect vs imperfect?
    print(f"\n  Perfect headers (first 5):")
    for pidx, hdr, payload in perfect[:5]:
        print(f"    [{pidx:2d}] frame_hdr={hdr.hex()} "
              f"data_hdr={' '.join(f'{payload[i]:02x}' for i in range(6))}")
    
    print(f"\n  Imperfect headers (first 5):")
    for pidx, hdr, payload, errors in imperfect[:5]:
        print(f"    [{pidx:2d}] frame_hdr={hdr.hex()} errors={errors} "
              f"data_hdr={' '.join(f'{payload[i]:02x}' for i in range(6))}")

    # Check frame header byte[3] for perfect vs imperfect
    perf_hdr3 = set(hdr[3] for _, hdr, _ in perfect)
    imp_hdr3 = set(hdr[3] for _, hdr, _, _ in imperfect)
    print(f"\n  Perfect frame byte[3]: {sorted(perf_hdr3)}")
    print(f"  Imperfect frame byte[3]: {sorted(imp_hdr3)}")
    
    # AHA: frame byte[3] = payload length!
    # For 238-byte payloads: frame byte[3] SHOULD be 0xEE (238)
    # But it varies... wait, these are ALL 238-byte payloads!
    # So frame byte[3] should be 238 = 0xEE for all of them.
    
    # Let me check
    print(f"\n  Frame byte[3] for ALL 238-byte pairs:")
    all_b3 = [hdr[3] for hdr, _ in black_pairs]
    unique_b3 = set(all_b3)
    print(f"    Unique values: {[f'0x{v:02x}' for v in sorted(unique_b3)]}")
    print(f"    0xEE count: {all_b3.count(0xEE)}")
    
    # Wait - I filtered for len(p)==238 but the frame header says the length!
    # So all frame byte[3] SHOULD be 0xEE=238... let me verify
    for i, (hdr, payload) in enumerate(black_pairs[:3]):
        print(f"    [{i}] frame_byte3=0x{hdr[3]:02x}={hdr[3]} payload_len={len(payload)}")


if __name__ == "__main__":
    main()
