# -*- coding: utf-8 -*-
"""
analyze_checksum.py - Deep analysis of packet integrity mechanism.

Compares byte 237 and other positions across packets within same capture
and across captures to identify the checksum/CRC type and location.
"""
import sys
import os

sys.stdout.reconfigure(encoding="utf-8")
BASE = os.path.dirname(os.path.abspath(__file__))


def parse_238(filepath):
    pkts = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
            except ValueError:
                continue
            if len(raw) == 238:
                pkts.append(raw)
    return pkts


def get_key3(raw):
    """Extract key3 from header (bytes 2-5 plaintext = 0)."""
    return (raw[3], raw[4], raw[2])


def main():
    # Load all captures
    caps = {}
    for name in ["black", "green", "blue", "white", "surely_full_red"]:
        path = os.path.join(BASE, name + ".csv")
        if os.path.exists(path):
            pkts = parse_238(path)
            if pkts:
                caps[name] = pkts

    # ANALYSIS 1: Check if cipher[237] ^ key_hdr[0] is constant within captures
    print("=" * 60)
    print("ANALYSIS 1: cipher[237] ^ key_hdr[237%3=0] within captures")
    print("=" * 60)
    for name, pkts in caps.items():
        vals = []
        for raw in pkts[:10]:
            k0, k1, k2 = get_key3(raw)
            # position 237 % 3 = 0, so key_hdr[0] = k0
            val = raw[237] ^ k0
            vals.append(val)
        unique = set(vals)
        status = "CONSTANT" if len(unique) == 1 else "VARIES (%d unique)" % len(unique)
        print("  %s: %s -> %s" % (
            name, " ".join("%02x" % v for v in vals[:6]), status))

    # ANALYSIS 2: Check other byte positions for constancy
    print("\n" + "=" * 60)
    print("ANALYSIS 2: Which bytes have constant cipher^key_hdr?")
    print("(Within green capture, checking bytes 12-237)")
    print("=" * 60)
    green = caps.get("green", [])
    if green:
        constant_positions = []
        for pos in range(12, 238):
            vals = set()
            for raw in green:
                k0, k1, k2 = get_key3(raw)
                key3 = [k0, k1, k2]
                val = raw[pos] ^ key3[pos % 3]
                vals.add(val)
            if len(vals) == 1:
                constant_positions.append((pos, list(vals)[0]))

        print("  Constant positions (cipher^hdr_key = same for all pkts):")
        if len(constant_positions) > 20:
            print("  %d positions constant! First 20:" % len(constant_positions))
            for pos, val in constant_positions[:20]:
                print("    byte[%d] = 0x%02x" % (pos, val))
        else:
            for pos, val in constant_positions:
                print("    byte[%d] = 0x%02x" % (pos, val))

        varying = 238 - 12 - len(constant_positions)
        print("  Varying: %d positions" % varying)

    # ANALYSIS 3: Cross-capture comparison at byte 237
    print("\n" + "=" * 60)
    print("ANALYSIS 3: cipher[237] ^ key_hdr[0] across captures")
    print("(Same value across captures = keystream has fixed offset)")
    print("=" * 60)
    for name, pkts in caps.items():
        k0, k1, k2 = get_key3(pkts[0])
        val = pkts[0][237] ^ k0
        print("  %s pkt[0]: cipher[237]=%02x ^ key_hdr[0]=%02x = %02x" % (
            name, pkts[0][237], k0, val))

    # ANALYSIS 4: Check if byte 237 plaintext could be XOR of color bytes
    # For GREEN: if we assume the keystream offset is constant
    print("\n" + "=" * 60)
    print("ANALYSIS 4: Byte structure at positions 12-14 across packets")
    print("(Check if first LED triple changes between packets)")
    print("=" * 60)
    for name in ["green", "blue", "black"]:
        if name not in caps:
            continue
        pkts = caps[name]
        print("  %s:" % name)
        for i, raw in enumerate(pkts[:5]):
            k0, k1, k2 = get_key3(raw)
            # Decrypt bytes 12-14 with header key (may or may not be correct)
            d12 = raw[12] ^ k0
            d13 = raw[13] ^ k1
            d14 = raw[14] ^ k2
            # Also show raw cipher at repeating section
            print("    pkt[%d] key3=[%02x,%02x,%02x] dec[12:15]=[%02x,%02x,%02x] "
                  "cipher[21:24]=[%02x,%02x,%02x] cipher[237]=%02x" % (
                      i, k0, k1, k2, d12, d13, d14,
                      raw[21], raw[22], raw[23], raw[237]))

    # ANALYSIS 5: Nonce analysis - are bytes 0-1 plaintext actually nonce or counter?
    print("\n" + "=" * 60)
    print("ANALYSIS 5: Nonce/counter at bytes 0-1")
    print("=" * 60)
    for name in ["green", "blue"]:
        if name not in caps:
            continue
        pkts = caps[name]
        print("  %s:" % name)
        for i, raw in enumerate(pkts[:8]):
            k0, k1, k2 = get_key3(raw)
            n0 = raw[0] ^ k0
            n1 = raw[1] ^ k1
            print("    pkt[%d] nonce=[%02x, %02x] = (%d, %d)" % (
                i, n0, n1, n0, n1))


if __name__ == "__main__":
    main()
