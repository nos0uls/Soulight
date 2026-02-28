# -*- coding: utf-8 -*-
"""
analyze_writes.py - Analyze ALL write sizes and content in captures.
Also test: replay green.csv WITHOUT 238-byte packets to see if color
is encoded elsewhere.
"""
import sys
import os
import time
from collections import Counter

sys.stdout.reconfigure(encoding="utf-8")
BASE = os.path.dirname(os.path.abspath(__file__))

try:
    import serial
except ImportError:
    serial = None


def parse_all_writes(filepath):
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


def analyze_sizes(name, writes):
    """Show distribution of write sizes."""
    sizes = Counter(len(w) for w in writes)
    print("\n=== %s: %d writes ===" % (name, len(writes)))
    for sz, cnt in sorted(sizes.items()):
        pct = 100.0 * cnt / len(writes)
        print("  size %3d: %3d writes (%.1f%%)" % (sz, cnt, pct))
        # Show first example
        for w in writes:
            if len(w) == sz:
                preview = " ".join("%02x" % b for b in w[:30])
                if len(w) > 30:
                    preview += " ..."
                print("           example: %s" % preview)
                break


def compare_5byte_headers(name, writes):
    """Analyze 5-byte writes (headers/heartbeats)."""
    hdrs = [w for w in writes if len(w) == 5]
    if not hdrs:
        return
    byte3_vals = Counter(h[3] for h in hdrs)
    print("\n  5-byte headers byte[3] values:")
    for val, cnt in sorted(byte3_vals.items()):
        print("    0x%02x: %d times" % (val, cnt))


def main():
    # Part 1: Analyze write sizes for all captures
    print("=" * 60)
    print("PART 1: Write size analysis across captures")
    print("=" * 60)

    for name in ["green", "blue", "black", "white", "surely_full_red"]:
        path = os.path.join(BASE, name + ".csv")
        if not os.path.exists(path):
            continue
        writes = parse_all_writes(path)
        analyze_sizes(name, writes)
        compare_5byte_headers(name, writes)

    # Part 2: What are the non-5, non-238 writes?
    print("\n\n" + "=" * 60)
    print("PART 2: Non-standard writes (not 5 or 238 bytes)")
    print("=" * 60)
    for name in ["green", "blue", "black"]:
        path = os.path.join(BASE, name + ".csv")
        if not os.path.exists(path):
            continue
        writes = parse_all_writes(path)
        other = [(i, w) for i, w in enumerate(writes) if len(w) not in (5, 238)]
        if other:
            print("\n  %s: %d non-standard writes:" % (name, len(other)))
            for idx, w in other[:10]:
                hex_str = " ".join("%02x" % b for b in w[:40])
                if len(w) > 40:
                    hex_str += " ..."
                print("    [%d] len=%d: %s" % (idx, len(w), hex_str))
        else:
            print("\n  %s: ALL writes are 5 or 238 bytes" % name)

    # Part 3: Replay test - green WITHOUT 238-byte packets
    if serial is None:
        print("\nSkipping replay test (no pyserial)")
        return

    print("\n\n" + "=" * 60)
    print("PART 3: Replay tests")
    print("=" * 60)

    green_writes = parse_all_writes(os.path.join(BASE, "green.csv"))

    # Test A: Only non-238 writes (skip color data)
    print("\n--- TEST A: Green WITHOUT 238-byte packets ---")
    print(">>> If green = color is NOT in 238-byte data <<<")
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    for w in green_writes:
        if len(w) == 238:
            continue  # SKIP color data
        ser.write(w)
        ser.flush()
        if len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    time.sleep(3)
    ser.close()
    print("  Sent only non-238 writes. 3 sec pause...")

    time.sleep(1)

    # Test B: Only 238-byte packets (no headers)
    print("\n--- TEST B: Green ONLY 238-byte packets (no headers) ---")
    print(">>> If green = color IS in 238-byte data <<<")
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    for w in green_writes:
        if len(w) != 238:
            continue  # SKIP non-data
        ser.write(w)
        ser.flush()
        time.sleep(0.033)
    time.sleep(3)
    ser.close()
    print("  Sent only 238-byte packets. 3 sec pause...")

    time.sleep(1)

    # Test C: Full replay (control)
    print("\n--- TEST C: Full green replay (control) ---")
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    for w in green_writes:
        ser.write(w)
        ser.flush()
        if len(w) == 238:
            time.sleep(0.033)
        elif len(w) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.05)
    time.sleep(3)
    ser.close()
    print("  Full replay done.")

    print("\n" + "=" * 60)
    print("RESULTS:")
    print("  A (no 238-byte):   ___ (green or white/nothing?)")
    print("  B (only 238-byte): ___ (green or white/nothing?)")
    print("  C (full replay):   ___ (green, control)")


if __name__ == "__main__":
    main()
