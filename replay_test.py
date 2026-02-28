# -*- coding: utf-8 -*-
"""
replay_test.py - Replay captured packets to LED strip.
Tests if controller accepts replayed packets.
"""
import sys
import os
import time
import struct

# Don't need 32-bit for this - just serial
try:
    import serial
except ImportError:
    print("Need pyserial! pip install pyserial")
    sys.exit(1)

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
COM_PORT = "COM7"
BAUD_RATE = 500000


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
    print("=== Replay Test ===")
    print(f"Port: {COM_PORT} @ {BAUD_RATE}")

    # Load RED captures
    red_pairs = parse_paired_writes(os.path.join(CSV_DIR, "red.csv"))
    print(f"RED pairs loaded: {len(red_pairs)}")

    if not red_pairs:
        print("No RED pairs found!")
        return

    # Open serial
    ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=1)
    ser.dtr = False
    time.sleep(0.05)
    ser.dtr = True
    time.sleep(0.1)
    print(f"Serial opened: {ser.is_open}")

    # Drain any pending data
    ser.read(1024)

    # Send heartbeat first to establish connection
    hb = bytes([0x55, 0xAA, 0x5A, 0xF1, 0x00])
    for _ in range(3):
        ser.write(hb)
        ser.flush()
        time.sleep(0.1)
    ser.read(1024)  # drain responses
    print("Heartbeats sent")

    # REPLAY: send captured RED packets exactly as they were
    print(f"\nReplaying {min(50, len(red_pairs))} RED packets...")
    for i in range(min(50, len(red_pairs))):
        frame_hdr, payload = red_pairs[i % len(red_pairs)]

        # Send frame header (55 AA 5A len 00)
        ser.write(frame_hdr)
        ser.flush()
        time.sleep(0.005)

        # Send payload
        ser.write(payload)
        ser.flush()
        time.sleep(0.095)

    # Check response
    time.sleep(0.2)
    resp = ser.read(1024)
    print(f"Response after replay: {len(resp)} bytes")
    if resp:
        print(f"  First 40: {resp[:40].hex()}")

    print("\nCheck LED strip - did it turn RED?")
    print("Waiting 5 seconds with continuous replay...")

    # Continue for 5 seconds
    start = time.time()
    idx = 0
    while time.time() - start < 5.0:
        frame_hdr, payload = red_pairs[idx % len(red_pairs)]
        ser.write(frame_hdr)
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.095)
        idx += 1

    print(f"Sent {idx} packets total")

    # Now try GREEN
    green_pairs = parse_paired_writes(os.path.join(CSV_DIR, "green.csv"))
    if green_pairs:
        print(f"\nSwitching to GREEN ({len(green_pairs)} pairs)...")
        start = time.time()
        idx = 0
        while time.time() - start < 5.0:
            frame_hdr, payload = green_pairs[idx % len(green_pairs)]
            ser.write(frame_hdr)
            ser.flush()
            time.sleep(0.005)
            ser.write(payload)
            ser.flush()
            time.sleep(0.095)
            idx += 1
        print(f"Sent {idx} GREEN packets")

    # Now BLUE
    blue_pairs = parse_paired_writes(os.path.join(CSV_DIR, "blue.csv"))
    if blue_pairs:
        print(f"\nSwitching to BLUE ({len(blue_pairs)} pairs)...")
        start = time.time()
        idx = 0
        while time.time() - start < 5.0:
            frame_hdr, payload = blue_pairs[idx % len(blue_pairs)]
            ser.write(frame_hdr)
            ser.flush()
            time.sleep(0.005)
            ser.write(payload)
            ser.flush()
            time.sleep(0.095)
            idx += 1
        print(f"Sent {idx} BLUE packets")

    # OFF (black)
    black_pairs = parse_paired_writes(os.path.join(CSV_DIR, "black.csv"))
    if black_pairs:
        print(f"\nTurning OFF (BLACK)...")
        for i in range(20):
            frame_hdr, payload = black_pairs[i % len(black_pairs)]
            ser.write(frame_hdr)
            ser.flush()
            time.sleep(0.005)
            ser.write(payload)
            ser.flush()
            time.sleep(0.095)

    ser.close()
    print("\nDone! Did the LED strip change colors?")
    print("RED (5s) -> GREEN (5s) -> BLUE (5s) -> OFF")


if __name__ == "__main__":
    main()
