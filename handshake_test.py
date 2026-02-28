# -*- coding: utf-8 -*-
"""
handshake_test.py - Try different handshake approaches to switch controller
from microphone mode to PC mode.

Run with 32-bit Python (needs DLL)!

Approaches:
1. DLL get_connect_package (MgL format)
2. Wire-format CTRL_DEVICE WORKMODE=PC packet
3. Multiple heartbeats with different cmd bytes
4. GenFramePackage(LP_ATTR_REQ, LP_CMD_CTRL_DEVICE, [LP_CTRL_WORKMODE, LP_WK_MODE_PC])
"""
import sys
import os
import ctypes
import struct
import time
import random

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll"
)

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

COM_PORT = "COM7"
BAUD_RATE = 500000
NUM_LEDS = 75


def build_wire_packet(cmd, attr, data, key=None):
    """
    Build a wire-format packet:
      GenFramePackage format guess:
        plaintext = [nonce0, nonce1, attr, cmd, data...]
        cipher = plaintext XOR key3
        frame = [55 AA 5A len 00] + cipher
    
    Or maybe:
        plaintext = [cmd, attr, data...]  (no nonce at payload level)
        The frame header IS the only framing.
    """
    if key is None:
        key = [random.randint(1, 254) for _ in range(3)]
    
    k0, k1, k2 = key
    
    # Build plaintext - try format similar to our analyzed 238-byte packets
    # But for small control packets, format might be different
    # 
    # From unpack_package output: [00 06 02 00 00 64 ff 00 00 01]
    # This was: [00] [data_len=06] [cmd=02] [attr=00] [data: 00 64 ff 00 00 01]
    # That's DLL internal format (after MgL strip).
    #
    # For wire format, let's try the simplest: just XOR a small payload
    
    # Approach A: bare minimum
    plain = bytearray([cmd, attr] + list(data))
    cipher = bytearray(len(plain))
    for i in range(len(plain)):
        cipher[i] = plain[i] ^ key[i % 3]
    
    frame_hdr = bytes([0x55, 0xAA, 0x5A, len(cipher), 0x00])
    return frame_hdr, bytes(cipher)


def build_color_packet(r, g, b, key=None):
    """Build 238-byte encrypted color packet."""
    if key is None:
        key = [random.randint(1, 254) for _ in range(3)]
    k0, k1, k2 = key
    
    plain = bytearray(238)
    plain[0] = random.randint(0, 255)
    plain[1] = random.randint(0, 255)
    plain[6] = 0x05
    plain[7] = 0x05
    plain[8] = 0xFF
    plain[9] = 0xE3
    plain[11] = 0x4B
    for i in range(NUM_LEDS):
        base = 12 + i * 3
        plain[base] = r
        plain[base + 1] = g
        plain[base + 2] = b
    
    cipher = bytearray(238)
    for i in range(238):
        cipher[i] = plain[i] ^ key[i % 3]
    
    frame_hdr = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])
    return frame_hdr, bytes(cipher)


def send_and_wait(ser, data, label="", wait=0.2):
    """Send data and read response."""
    ser.write(data)
    ser.flush()
    time.sleep(wait)
    resp = ser.read(4096)
    if resp:
        print(f"  {label} -> {len(resp)} bytes: {resp[:30].hex()}{'...' if len(resp) > 30 else ''}")
    else:
        print(f"  {label} -> no response")
    return resp


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    ptr_size = struct.calcsize("P") * 8
    if ptr_size != 32:
        print(f"ERROR: Need 32-bit Python! (current: {ptr_size}-bit)")
        sys.exit(1)

    dll = ctypes.CDLL(DLL_PATH)
    print(f"DLL loaded")

    ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=0.5)
    ser.dtr = False
    time.sleep(0.1)
    ser.dtr = True
    time.sleep(0.3)
    ser.read(4096)  # drain
    print(f"Serial opened: {COM_PORT}")

    # ================================================================
    # Approach 1: DLL get_connect_package (MgL format)
    # ================================================================
    print(f"\n{'='*60}")
    print("Approach 1: DLL get_connect_package (MgL)")
    print(f"{'='*60}")

    buf = ctypes.create_string_buffer(256)
    dll.get_connect_package(buf)
    raw = buf.raw
    last_nz = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
    connect_pkt = raw[:last_nz + 1]
    print(f"  Connect packet: {connect_pkt.hex()}")

    for i in range(5):
        send_and_wait(ser, connect_pkt, f"connect[{i}]", 0.3)

    # Try color after connect
    print("  Sending RED after MgL connect...")
    for _ in range(30):
        hdr, payload = build_color_packet(255, 0, 0)
        ser.write(hdr + payload)
        ser.flush()
        time.sleep(0.08)
    time.sleep(0.5)
    print("  Check LED - RED?")

    # ================================================================
    # Approach 2: DLL get_scen_package to set mode
    # ================================================================
    print(f"\n{'='*60}")
    print("Approach 2: DLL get_scen_package (RED, MgL format)")
    print(f"{'='*60}")

    for i in range(50):
        buf2 = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf2)
        raw2 = buf2.raw
        last_nz2 = max((j for j in range(len(raw2)) if raw2[j] != 0), default=0)
        pkt2 = raw2[:last_nz2 + 1]
        ser.write(pkt2)
        ser.flush()
        time.sleep(0.08)

    time.sleep(0.5)
    resp = ser.read(4096)
    print(f"  Response: {len(resp)} bytes" if resp else "  No response")
    print("  Check LED - RED?")

    # ================================================================
    # Approach 3: Heartbeat with different byte[3] values
    # ================================================================
    print(f"\n{'='*60}")
    print("Approach 3: Various heartbeat-like frames")
    print(f"{'='*60}")

    # Try heartbeat cmd values from 0x00 to 0x06 (matching LP_CMD enum)
    for cmd in [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06]:
        hb = bytes([0x55, 0xAA, 0x5A, cmd, 0x00])
        resp = send_and_wait(ser, hb, f"heartbeat cmd={cmd:02x}", 0.2)

    # ================================================================
    # Approach 4: Wire-format CTRL_DEVICE with WORKMODE
    # ================================================================
    print(f"\n{'='*60}")
    print("Approach 4: Wire CTRL_DEVICE/WORKMODE packets")
    print(f"{'='*60}")

    # LP_CMD_CTRL_DEVICE=5, LP_ATTR_REQ=0, LP_CTRL_WORKMODE=6, LP_WK_MODE_PC=0
    # Try various plaintext formats
    attempts = [
        # (description, plaintext_bytes)
        ("cmd5 attr0 ctrl6 mode0", [0x05, 0x00, 0x06, 0x00]),
        ("cmd5 attr0 ctrl6 mode0 padded", [0x05, 0x00, 0x06, 0x00, 0x00, 0x00]),
        ("attr0 cmd5 ctrl6 mode0", [0x00, 0x05, 0x06, 0x00]),
        ("nonce cmd5 attr0 ctrl6 mode0", [random.randint(0,255), random.randint(0,255), 0x05, 0x00, 0x06, 0x00]),
        # From DLL unpack format: [00, data_len, cmd, attr, data...]
        ("dll_fmt 00 04 05 00 06 00", [0x00, 0x04, 0x05, 0x00, 0x06, 0x00]),
        ("dll_fmt 00 02 05 00 06 00", [0x00, 0x02, 0x05, 0x00, 0x06, 0x00]),
        # LP_CTRL_SWITCHER=1 to turn on
        ("switcher on", [0x05, 0x00, 0x01, 0x01]),
        ("dll_fmt switcher", [0x00, 0x02, 0x05, 0x00, 0x01, 0x01]),
    ]

    for desc, plain_data in attempts:
        key = [random.randint(1, 254) for _ in range(3)]
        cipher = bytearray(len(plain_data))
        for i in range(len(plain_data)):
            cipher[i] = plain_data[i] ^ key[i % 3]
        
        frame_hdr = bytes([0x55, 0xAA, 0x5A, len(cipher), 0x00])
        send_and_wait(ser, frame_hdr + bytes(cipher), desc, 0.3)

    # Then try color
    print("\n  Sending RED after ctrl packets...")
    for _ in range(50):
        hdr, payload = build_color_packet(255, 0, 0)
        ser.write(hdr)
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.075)
    print("  Check LED - RED?")

    # ================================================================
    # Approach 5: Replicate EXACT Beelight app startup sequence
    # by sniffing the connection (user needs to capture startup)
    # ================================================================
    print(f"\n{'='*60}")
    print("Approach 5: Try DLL connect + wire color with timing")
    print(f"{'='*60}")

    # Reset connection
    ser.dtr = False
    time.sleep(0.5)
    ser.dtr = True
    time.sleep(0.5)
    ser.read(4096)

    # Phase 1: Send connect packages from DLL
    print("  Phase 1: DLL connect packages...")
    for i in range(10):
        buf3 = ctypes.create_string_buffer(256)
        dll.get_connect_package(buf3)
        raw3 = buf3.raw
        last3 = max((j for j in range(len(raw3)) if raw3[j] != 0), default=0)
        ser.write(raw3[:last3 + 1])
        ser.flush()
        time.sleep(0.2)
    
    ser.read(4096)

    # Phase 2: Send scen_package from DLL
    print("  Phase 2: DLL scen_package (RED)...")
    for i in range(10):
        buf4 = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf4)
        raw4 = buf4.raw
        last4 = max((j for j in range(len(raw4)) if raw4[j] != 0), default=0)
        ser.write(raw4[:last4 + 1])
        ser.flush()
        time.sleep(0.2)

    # Phase 3: Send wire-format color
    print("  Phase 3: Wire color (RED)...")
    for _ in range(50):
        hdr, payload = build_color_packet(255, 0, 0)
        ser.write(hdr)
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.075)

    print("  Check LED - did anything change?")

    ser.close()
    print("\nDone! Report which approach (if any) made the LED change.")


if __name__ == "__main__":
    main()
