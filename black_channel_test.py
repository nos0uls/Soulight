# -*- coding: utf-8 -*-
"""
black_channel_test.py - Test using BLACK capture as base.

BLACK plaintext color data = all zeros, so cipher = keystream.
XOR one channel at a time with 0xFF to determine R/G/B order.

COM7 @ 500000 baud. Run with 32-bit Python!
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

# region ===== Config =====
COM_PORT = "COM7"
BAUD_RATE = 500000
BASE = os.path.dirname(os.path.abspath(__file__))
CSV_PATH = os.path.join(BASE, "black.csv")
# endregion


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


# region ===== Find handshake boundary =====
def find_handshake_end(writes):
    for i, raw in enumerate(writes):
        if len(raw) >= 238:
            return i - 1 if (i > 0 and len(writes[i - 1]) == 5) else i
    return len(writes)
# endregion


# region ===== Modify BLACK packet =====
def modify_black_packet(raw, channel, value=255):
    """
    BLACK cipher = keystream (plain is all zeros in color data).
    XOR one channel position with value to light it up.
    channel: 0, 1, or 2 (which byte in the RGB triple).
    """
    modified = bytearray(raw)
    for i in range(12, 237):
        if (i - 12) % 3 == channel:
            modified[i] ^= value
    return bytes(modified)
# endregion


# region ===== Send frames sequentially =====
def send_frames(ser, writes, start_idx, num_frames, modifier=None):
    """
    Send num_frames color packets starting from start_idx.
    modifier: None = send as-is, or callable(raw) -> modified_raw.
    Returns new index.
    """
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


# region ===== Main test =====
def main():
    print("=" * 50)
    print("BLACK CHANNEL TEST - determine R/G/B byte order")
    print("=" * 50)

    writes = parse_writes(CSV_PATH)
    n238 = sum(1 for w in writes if len(w) == 238)
    hs_end = find_handshake_end(writes)
    print("BLACK: %d writes, %d color pkts, handshake=%d" % (
        len(writes), n238, hs_end))

    # 39 packets / 4 tests = ~9 per test
    fpt = max(n238 // 4, 5)
    print("Frames per test: %d\n" % fpt)

    ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)

    # Handshake
    print("--- HANDSHAKE ---")
    for raw in writes[:hs_end]:
        ser.write(raw)
        ser.flush()
        if len(raw) == 5 and raw[:3] == b'\x55\xAA\x5A':
            time.sleep(0.005)
        else:
            time.sleep(0.08)
            ser.read(4096)
    time.sleep(0.5)
    ser.read(8192)
    print("Done!\n")

    idx = hs_end

    # TEST 0: BLACK original (strip should be OFF)
    print("--- TEST 0: BLACK original (OFF) --- 2 sec pause after")
    idx, sent = send_frames(ser, writes, idx, fpt)
    print("  Sent %d frames" % sent)
    time.sleep(2)

    # TEST 1: Channel 0 = 255
    print("\n--- TEST 1: Channel 0 = 255 --- what color?")
    idx, sent = send_frames(ser, writes, idx, fpt,
                            modifier=lambda r: modify_black_packet(r, 0))
    print("  Sent %d frames" % sent)
    time.sleep(2)

    # TEST 2: Channel 1 = 255
    print("\n--- TEST 2: Channel 1 = 255 --- what color?")
    idx, sent = send_frames(ser, writes, idx, fpt,
                            modifier=lambda r: modify_black_packet(r, 1))
    print("  Sent %d frames" % sent)
    time.sleep(2)

    # TEST 3: Channel 2 = 255
    print("\n--- TEST 3: Channel 2 = 255 --- what color?")
    idx, sent = send_frames(ser, writes, idx, fpt,
                            modifier=lambda r: modify_black_packet(r, 2))
    print("  Sent %d frames" % sent)
    time.sleep(2)

    ser.close()
    print("\n" + "=" * 50)
    print("DONE! What did you see?")
    print("  TEST 0 (BLACK):     ___ (OFF?)")
    print("  TEST 1 (ch0=255):   ___ (RED/GREEN/BLUE?)")
    print("  TEST 2 (ch1=255):   ___ (RED/GREEN/BLUE?)")
    print("  TEST 3 (ch2=255):   ___ (RED/GREEN/BLUE?)")


if __name__ == "__main__":
    main()
