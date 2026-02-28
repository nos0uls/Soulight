# -*- coding: utf-8 -*-
"""
send_fresh_packet.py - Send freshly-constructed packets with random key3.

THEORY: Controller doesn't validate key3 vs nonce. The replay failed
because of timing/DTR issues, not because of stale keys.

Wire format:
  Frame: [55 AA 5A] [payload_len] [00]
  Payload: XOR-encrypted with 3-byte key [k0, k1, k2]
  
  Plaintext for 238-byte (75 LEDs, cmd=CTRL_SYNC_RGB):
    [0]  = n0 (random nonce byte 0)
    [1]  = n1 (random nonce byte 1)  
    [2:6] = [00 00 00 00] (padding)
    [6]  = 0x05 (LP_CMD_CTRL_DEVICE? or sub-protocol)
    [7]  = 0x05
    [8]  = 0xFF (constant?)
    [9]  = 0xE3 = 227 = remaining_len (238-11)
    [10] = 0x00
    [11] = 0x4B = 75 = num_leds
    [12:237] = R,G,B * 75 (LED data)
    [237] = 0x00 (tail)

Cipher: cipher[i] = plaintext[i] XOR key[i % 3]

We'll generate fresh packets with random key3 and random nonce.
"""
import sys
import os
import time
import random
import struct

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

COM_PORT = "COM7"
BAUD_RATE = 500000
NUM_LEDS = 75


def build_color_packet(r, g, b, key=None):
    """Build a 238-byte encrypted payload for solid color."""
    if key is None:
        key = [random.randint(0, 255) for _ in range(3)]
    
    k0, k1, k2 = key
    
    # Build plaintext
    n0 = random.randint(0, 255)
    n1 = random.randint(0, 255)
    
    plain = bytearray(238)
    plain[0] = n0
    plain[1] = n1
    plain[2] = 0x00
    plain[3] = 0x00
    plain[4] = 0x00
    plain[5] = 0x00
    plain[6] = 0x05
    plain[7] = 0x05
    plain[8] = 0xFF
    plain[9] = 0xE3  # 227 = 238-11
    plain[10] = 0x00
    plain[11] = 0x4B  # 75 LEDs
    
    for i in range(NUM_LEDS):
        base = 12 + i * 3
        plain[base] = r
        plain[base + 1] = g
        plain[base + 2] = b
    
    plain[237] = 0x00  # tail
    
    # Encrypt
    cipher = bytearray(238)
    for i in range(238):
        cipher[i] = plain[i] ^ key[i % 3]
    
    return bytes(cipher), key


def build_frame_header(payload_len):
    """Build 55 AA 5A frame header."""
    return bytes([0x55, 0xAA, 0x5A, payload_len & 0xFF, 0x00])


def main():
    print("=== Fresh Packet Sender ===")
    print(f"Port: {COM_PORT} @ {BAUD_RATE}")
    
    ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=1)
    ser.dtr = False
    time.sleep(0.05)
    ser.dtr = True
    time.sleep(0.2)
    
    # Drain
    ser.read(4096)
    
    # Send initial heartbeats to wake up controller
    hb = bytes([0x55, 0xAA, 0x5A, 0xF1, 0x00])
    print("Sending initial heartbeats...")
    for _ in range(10):
        ser.write(hb)
        ser.flush()
        time.sleep(0.05)
    
    ser.read(4096)  # drain
    
    # ================================================================
    # Test 1: Send RED with random key
    # ================================================================
    print("\n--- Test 1: RED with random key (10 seconds) ---")
    
    start = time.time()
    count = 0
    while time.time() - start < 10.0:
        payload, key = build_color_packet(255, 0, 0)
        frame_hdr = build_frame_header(len(payload))
        
        # Send frame header
        ser.write(frame_hdr)
        ser.flush()
        time.sleep(0.005)
        
        # Send payload
        ser.write(payload)
        ser.flush()
        time.sleep(0.075)
        
        count += 1
        
        if count <= 2:
            print(f"  Sent pkt {count}: key=[{key[0]:02x},{key[1]:02x},{key[2]:02x}] "
                  f"hdr={frame_hdr.hex()} payload[0:6]={payload[:6].hex()}")
    
    print(f"  Sent {count} RED packets. Check LED strip!")
    
    # Read any response
    time.sleep(0.2)
    resp = ser.read(4096)
    if resp:
        print(f"  Response: {len(resp)} bytes, first 20: {resp[:20].hex()}")
    
    # ================================================================
    # Test 2: GREEN
    # ================================================================
    print("\n--- Test 2: GREEN (5 seconds) ---")
    start = time.time()
    count = 0
    while time.time() - start < 5.0:
        payload, _ = build_color_packet(0, 255, 0)
        ser.write(build_frame_header(len(payload)))
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.075)
        count += 1
    print(f"  Sent {count} GREEN packets")
    
    # ================================================================
    # Test 3: BLUE
    # ================================================================
    print("\n--- Test 3: BLUE (5 seconds) ---")
    start = time.time()
    count = 0
    while time.time() - start < 5.0:
        payload, _ = build_color_packet(0, 0, 255)
        ser.write(build_frame_header(len(payload)))
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.075)
        count += 1
    print(f"  Sent {count} BLUE packets")
    
    # ================================================================
    # Test 4: OFF
    # ================================================================
    print("\n--- Turning OFF ---")
    for _ in range(20):
        payload, _ = build_color_packet(0, 0, 0)
        ser.write(build_frame_header(len(payload)))
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.08)
    
    ser.close()
    print("\nDone! Did LED change? RED (10s) -> GREEN (5s) -> BLUE (5s) -> OFF")


if __name__ == "__main__":
    main()
