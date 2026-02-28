# -*- coding: utf-8 -*-
"""
green_structural_test.py - XOR test that SKIPS structural bytes.

Discovery: positions mod 1 (bytes 13,16,19,...) and byte 12 are ALWAYS 0x00
in plaintext. They're structural padding, NOT color data.
Modifying them causes controller to reject the packet!

Solution: only modify NON-structural positions:
  - mod 0 positions from byte 15 onwards (every 3rd byte: 15,18,21,...)
  - mod 2 positions from byte 14 onwards (every 3rd byte: 14,17,20,...)

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


def is_structural(pos):
    """Returns True if this byte position is structural (must stay 0x00).
    Structural positions:
      - byte 12 (first byte after header)
      - all mod-1 positions: 13, 16, 19, 22, 25, ...
    """
    if pos == 12:
        return True
    if pos < 12:
        return True  # header - don't touch
    offset = (pos - 12) % 3
    return offset == 1  # mod 1 = structural padding


def mod_skip_structural(raw):
    """XOR ONLY non-structural color bytes with 0xFF.
    Skips: bytes 0-13, all mod-1 positions, byte 237.
    Modifies: mod-0 (from 15) and mod-2 (from 14) positions."""
    m = bytearray(raw)
    for i in range(14, 237):  # skip bytes 0-13 and 237
        if not is_structural(i):
            m[i] ^= 0xFF
    return bytes(m)


def mod_only_mod0(raw):
    """XOR only mod-0 positions (15,18,21,...) with 0xFF.
    This is ONE color channel. Skip byte 12 (structural)."""
    m = bytearray(raw)
    for i in range(15, 237):
        if (i - 12) % 3 == 0:
            m[i] ^= 0xFF
    return bytes(m)


def mod_only_mod2(raw):
    """XOR only mod-2 positions (14,17,20,...) with 0xFF.
    This is the OTHER color channel."""
    m = bytearray(raw)
    for i in range(14, 237):
        if (i - 12) % 3 == 2:
            m[i] ^= 0xFF
    return bytes(m)


def send_frames(ser, writes, start_idx, num_frames, modifier=None):
    sent = 0
    idx = start_idx
    while sent < num_frames and idx < len(writes):
        raw = writes[idx]
        idx += 1
        if len(raw) == 238:
            if modifier:
                raw = modifier(raw)
            ser.write(raw)
            ser.flush()
            time.sleep(0.033)
            sent += 1
        else:
            ser.write(raw)
            ser.flush()
            time.sleep(0.005)
    return idx, sent


def main():
    print("=" * 55)
    print("STRUCTURAL SKIP TEST - modify only color data bytes")
    print("=" * 55)

    writes = parse_writes(CSV_PATH)
    n238 = sum(1 for w in writes if len(w) == 238)
    print("green.csv: %d writes, %d color packets" % (len(writes), n238))

    # 18 packets / 4 tests = 4 per test, reopen port each time
    fpt = 4

    tests = [
        ("A: GREEN original", None),
        ("B: Both channels inverted (skip structural)", mod_skip_structural),
        ("C: Only mod-0 channel inverted", mod_only_mod0),
        ("D: Only mod-2 channel inverted", mod_only_mod2),
    ]

    for label, modifier in tests:
        print("\n--- %s ---" % label)
        ser = serial.Serial("COM7", 500000, timeout=0.5)
        ser.dtr = True
        ser.rts = True
        time.sleep(0.3)
        ser.read(4096)

        # Send first fpt frames (fresh port = fresh PRNG)
        idx, sent = send_frames(ser, writes, 0, fpt, modifier=modifier)
        print("  Sent %d frames" % sent)
        time.sleep(2.5)
        ser.close()
        time.sleep(0.3)

    print("\n" + "=" * 55)
    print("RESULTS:")
    print("  A (GREEN original):     ___ (should be GREEN)")
    print("  B (both ch inverted):   ___ (color change = SUCCESS!)")
    print("  C (mod-0 ch inverted):  ___ (which color?)")
    print("  D (mod-2 ch inverted):  ___ (which color?)")
    print()
    print("If B/C/D show different colors -> structural padding was the issue!")
    print("If still green -> deeper integrity check exists")


if __name__ == "__main__":
    main()
