# -*- coding: utf-8 -*-
"""
real_color_test.py - XOR modification of REAL color packets (239-245 bytes).

Discovery: 238-byte packets are heartbeat/keepalive ONLY.
Real color data is in 239-245 byte packets!

Test: replay green.csv with all 239+ byte packets XOR-modified.
Phase 1: Original green (fresh port)
Phase 2: All 239+ packets inverted at bytes 14+ skipping mod-1 (fresh port)
Phase 3: All 239+ packets inverted at ALL bytes 14+ (fresh port)

COM7 @ 500000 baud.
"""
import sys
import os
import time

sys.stdout.reconfigure(encoding="utf-8")

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

BASE = os.path.dirname(os.path.abspath(__file__))
CSV_PATH = os.path.join(BASE, "green.csv")


def parse_writes(filepath):
    writes = []
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
            if raw:
                writes.append(raw)
    return writes


def is_color_packet(raw):
    """Real color packets are 239-245 bytes (NOT 238!)."""
    return 239 <= len(raw) <= 245


def mod_invert_all(raw):
    """XOR ALL bytes from 14 to end-1 with 0xFF."""
    m = bytearray(raw)
    for i in range(14, len(m) - 1):
        m[i] ^= 0xFF
    return bytes(m)


def mod_invert_skip_mod1(raw):
    """XOR bytes 14+ with 0xFF, but SKIP mod-1 positions (structural padding)."""
    m = bytearray(raw)
    for i in range(14, len(m) - 1):
        if (i - 12) % 3 != 1:  # skip mod-1 (structural)
            m[i] ^= 0xFF
    return bytes(m)


def mod_invert_mod2_only(raw):
    """XOR ONLY mod-2 positions from byte 14+ (one channel only)."""
    m = bytearray(raw)
    for i in range(14, len(m) - 1):
        if (i - 12) % 3 == 2:
            m[i] ^= 0xFF
    return bytes(m)


def replay_with_mod(writes, modifier=None, label=""):
    """Replay all writes. Apply modifier to 239-245 byte packets only."""
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)

    color_count = 0
    for w in writes:
        if is_color_packet(w) and modifier:
            w = modifier(w)
            color_count += 1
        ser.write(w)
        ser.flush()
        if len(w) >= 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)

    print("  %s: modified %d color packets" % (label, color_count))
    time.sleep(3)
    ser.close()
    time.sleep(0.5)


def main():
    print("=" * 55)
    print("REAL COLOR TEST - modify 239-245 byte packets!")
    print("=" * 55)

    writes = parse_writes(CSV_PATH)
    n_color = sum(1 for w in writes if is_color_packet(w))
    n_238 = sum(1 for w in writes if len(w) == 238)
    print("green.csv: %d writes, %d color (239-245), %d heartbeat (238)\n" % (
        len(writes), n_color, n_238))

    # Test 1: Original (control)
    print("--- TEST 1: GREEN original ---")
    replay_with_mod(writes, None, "original")

    # Test 2: Invert ALL bytes 14+ (except last byte)
    print("\n--- TEST 2: Invert ALL bytes 14+ in color packets ---")
    print(">>> Should change color if XOR works! <<<")
    replay_with_mod(writes, mod_invert_all, "invert_all")

    # Test 3: Invert skip mod-1
    print("\n--- TEST 3: Invert bytes 14+ SKIP mod-1 ---")
    replay_with_mod(writes, mod_invert_skip_mod1, "skip_mod1")

    # Test 4: Invert only mod-2 positions
    print("\n--- TEST 4: Invert ONLY mod-2 positions ---")
    replay_with_mod(writes, mod_invert_mod2_only, "mod2_only")

    print("\n" + "=" * 55)
    print("RESULTS:")
    print("  1 (original):       ___ (should be GREEN)")
    print("  2 (all inverted):   ___ (color change?)")
    print("  3 (skip mod-1):     ___ (color change?)")
    print("  4 (mod-2 only):     ___ (color change?)")
    print()
    print("ANY color change in 2/3/4 = XOR on real packets WORKS!")


if __name__ == "__main__":
    main()
