# -*- coding: utf-8 -*-
"""
test_with_handshake.py - Test arbitrary colors WITH proper handshake.

Handshake extracted from surely_full_red.csv (writes 0-113).
Then send modified color packets from green.csv.

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
    """Color packets are 239-245 bytes."""
    return 239 <= len(raw) <= 245


# region ===== Color modification functions =====
def set_color_from_green(raw, r, g, b):
    """Set all LEDs to specified RGB color using XOR delta.
    
    Works on GREEN.csv packets (current color: R=0, G=255, B=0).
    
    Structure confirmed from tests:
      - mod-0 (positions 15,18,21,...) = GREEN channel (current=255)
      - mod-1 (positions 13,16,19,...) = BLUE channel (current=0)
      - mod-2 (positions 14,17,20,...) = RED channel (current=0)
    
    XOR delta formula for encrypted packets:
      - GREEN: XOR with (g ^ 255) to change from 255 to g
      - BLUE:  XOR with b to change from 0 to b
      - RED:   XOR with r to change from 0 to r
    """
    m = bytearray(raw)
    num_leds = (len(raw) - 15) // 3  # bytes 12-14 are header, then LED data
    
    # XOR deltas for each channel (green.csv has R=0, G=255, B=0)
    delta_g = g ^ 255  # GREEN channel: 255 -> g
    delta_b = b        # BLUE channel:  0 -> b
    delta_r = r        # RED channel:   0 -> r
    
    for led in range(num_leds):
        base = 12 + led * 3
        if base + 2 < len(m) - 1:  # don't touch last byte
            m[base + 0] ^= delta_g  # mod-0 = GREEN
            m[base + 1] ^= delta_b  # mod-1 = BLUE
            m[base + 2] ^= delta_r  # mod-2 = RED
    
    return bytes(m)


def mod_invert_all(raw):
    """XOR ALL bytes from 14 to end-1 with 0xFF."""
    m = bytearray(raw)
    for i in range(14, len(m) - 1):
        m[i] ^= 0xFF
    return bytes(m)
# endregion


def send_handshake(ser, handshake_writes):
    """Send handshake sequence to initialize controller session."""
    for w in handshake_writes:
        ser.write(w)
        ser.flush()
        # Small delay between handshake packets
        time.sleep(0.005)
    print("  Handshake sent: %d packets" % len(handshake_writes))
    time.sleep(0.2)


def send_color_sequence(ser, color_writes, modifier=None):
    """Send color packets with optional modification."""
    sent = 0
    for w in color_writes:
        if is_color_packet(w):
            if modifier:
                w = modifier(w)
            ser.write(w)
            ser.flush()
            time.sleep(0.033)
            sent += 1
        elif len(w) == 5:
            ser.write(w)
            ser.flush()
            time.sleep(0.005)
        else:
            ser.write(w)
            ser.flush()
            time.sleep(0.01)
    return sent


def main():
    print("=" * 60)
    print("HANDSHAKE + ARBITRARY COLOR TEST")
    print("=" * 60)
    
    # Load handshake from surely_full_red.csv (writes 0-113)
    red_writes = parse_writes(os.path.join(BASE, "surely_full_red.csv"))
    handshake = red_writes[:114]  # First 114 writes = handshake
    
    # Load color packets from green.csv
    green_writes = parse_writes(os.path.join(BASE, "green.csv"))
    
    print("Handshake: %d packets from surely_full_red.csv" % len(handshake))
    print("Color source: green.csv (%d writes)\n" % len(green_writes))
    
    # Test 1: Handshake + GREEN original
    print("--- TEST 1: Handshake + GREEN original ---")
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    
    send_handshake(ser, handshake)
    sent = send_color_sequence(ser, green_writes[:50], modifier=None)
    print("  Sent %d color packets" % sent)
    time.sleep(2)
    ser.close()
    time.sleep(0.5)
    
    # Test 2: Handshake + RED (all LEDs = 255,0,0)
    print("\n--- TEST 2: Handshake + RED (255,0,0) ---")
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    
    send_handshake(ser, handshake)
    sent = send_color_sequence(ser, green_writes[:50], 
                               modifier=lambda w: set_color_from_green(w, 255, 0, 0))
    print("  Sent %d color packets" % sent)
    time.sleep(2)
    ser.close()
    time.sleep(0.5)
    
    # Test 3: Handshake + BLUE (0,0,255)
    print("\n--- TEST 3: Handshake + BLUE (0,0,255) ---")
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    
    send_handshake(ser, handshake)
    sent = send_color_sequence(ser, green_writes[:50],
                               modifier=lambda w: set_color_from_green(w, 0, 0, 255))
    print("  Sent %d color packets" % sent)
    time.sleep(2)
    ser.close()
    time.sleep(0.5)
    
    # Test 4: Handshake + WHITE (255,255,255)
    print("\n--- TEST 4: Handshake + WHITE (255,255,255) ---")
    ser = serial.Serial("COM7", 500000, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    
    send_handshake(ser, handshake)
    sent = send_color_sequence(ser, green_writes[:50],
                               modifier=lambda w: set_color_from_green(w, 255, 255, 255))
    print("  Sent %d color packets" % sent)
    time.sleep(2)
    ser.close()
    
    print("\n" + "=" * 60)
    print("RESULTS:")
    print("  1 (GREEN original): ___")
    print("  2 (RED):            ___")
    print("  3 (BLUE):           ___")
    print("  4 (WHITE):          ___")
    print()
    print("If colors match = handshake + color setting WORKS!")


if __name__ == "__main__":
    main()
