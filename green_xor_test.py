# -*- coding: utf-8 -*-
"""
green_xor_test.py - Definitive XOR modification test using GREEN capture.

Green capture has 18 STATIC green packets (confirmed by replay).
Phase 1: Original green (9 packets) - confirm replay works
Phase 2: ALL color bytes XOR'd with 0xFF (9 packets) - should show MAGENTA
    (inverts G=255->0, R=0->255, B=0->255 = magenta)

If Phase 2 shows MAGENTA -> XOR approach WORKS, no checksum!
If Phase 2 shows GREEN -> there IS a checksum, XOR breaks it.

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


# region ===== Parse CSV =====
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
# endregion


# region ===== Modify packets =====
def mod_all_226(raw):
    """XOR bytes 12-237 (226 bytes, EVEN count -> XOR checksum preserved)."""
    m = bytearray(raw)
    for i in range(12, 238):  # 12..237 inclusive = 226 bytes
        m[i] ^= 0xFF
    return bytes(m)


def mod_pos01(raw):
    """XOR only positions 0,1 in each triple (150 bytes, EVEN -> XOR preserved).
    Leaves position 2 untouched. Does NOT touch byte 237."""
    m = bytearray(raw)
    for i in range(12, 237):
        if (i - 12) % 3 != 2:  # pos 0 and pos 1 only
            m[i] ^= 0xFF
    return bytes(m)


def mod_pos0_only(raw):
    """XOR only position 0 in each triple (75 bytes, ODD -> XOR breaks).
    Plus byte 237 to make it 76 (EVEN -> XOR preserved)."""
    m = bytearray(raw)
    for i in range(12, 237):
        if (i - 12) % 3 == 0:
            m[i] ^= 0xFF
    m[237] ^= 0xFF  # compensate checksum
    return bytes(m)
# endregion


# region ===== Send frames =====
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
# endregion


# region ===== Main =====
def main():
    print("=" * 55)
    print("GREEN XOR TEST v2 - checksum-preserving modifications")
    print("=" * 55)

    writes = parse_writes(CSV_PATH)
    n238 = sum(1 for w in writes if len(w) == 238)
    print("green.csv: %d writes, %d color packets" % (len(writes), n238))

    # 18 packets / 4 tests = 4-5 per test
    fpt = n238 // 4
    if fpt < 3:
        fpt = 3
    print("Frames per test: %d\n" % fpt)

    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)

    idx = 0

    # Test A: Original GREEN
    print("--- TEST A: GREEN original ---")
    idx, sent = send_frames(ser, writes, idx, fpt)
    print("  Sent %d" % sent)
    time.sleep(2)

    # Test B: XOR bytes 12-237 (226 even, XOR checksum preserved)
    print("\n--- TEST B: XOR 12-237 (226 bytes, even) ---")
    print("  If color changes -> checksum was the issue!")
    idx, sent = send_frames(ser, writes, idx, fpt, modifier=mod_all_226)
    print("  Sent %d" % sent)
    time.sleep(2)

    # Test C: XOR pos 0+1 only (150 even, skip pos 2 and byte 237)
    print("\n--- TEST C: XOR pos 0+1 only (150 bytes, even) ---")
    print("  Two channels flip, one stays")
    idx, sent = send_frames(ser, writes, idx, fpt, modifier=mod_pos01)
    print("  Sent %d" % sent)
    time.sleep(2)

    # Test D: XOR pos 0 + byte 237 (76 even)
    print("\n--- TEST D: XOR pos 0 + byte237 (76 bytes, even) ---")
    print("  One channel flips + checksum compensated")
    idx, sent = send_frames(ser, writes, idx, fpt, modifier=mod_pos0_only)
    print("  Sent %d" % sent)
    time.sleep(2)

    ser.close()
    print("\n" + "=" * 55)
    print("RESULTS - what color was each test?")
    print("  A (GREEN original):    ___")
    print("  B (all 226 XOR):       ___")
    print("  C (pos 0+1 XOR):       ___")
    print("  D (pos 0 + byte237):   ___")


if __name__ == "__main__":
    main()
