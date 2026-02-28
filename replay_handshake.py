# -*- coding: utf-8 -*-
"""
replay_handshake.py - Replay the exact captured handshake from surely_full_red.csv,
then send fresh color packets.

Strategy:
1. Extract all WRITE operations from the capture in order
2. Replay them with proper timing
3. After handshake completes, send our own color packets
4. Observe if LED changes

Uses 32-bit Python for DLL fallback if needed.
"""
import sys, os, time, random, struct

CSV_DIR = os.path.dirname(os.path.abspath(__file__))

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

COM_PORT = "COM7"
BAUD_RATE = 500000
NUM_LEDS = 75


def parse_writes(filepath):
    """Parse all WRITE;DOWN operations preserving order."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            data_str = parts[5].strip()
            try:
                raw = bytes.fromhex(data_str.replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append((line_no, raw))
    return writes


def build_color_packet_simple(r, g, b):
    """Build a 238-byte color packet using the known wire format.
    
    byte[1] for len=238 must be 0x32.
    We know: cipher[1] = 0x32 always for len=238.
    So plaintext[1] XOR key[1] = 0x32.
    
    For simplicity, set key = [k0, k1, k2] with random k0, k2.
    Then: plaintext[1] = 0x32 XOR k1.
    But we don't know what plaintext[1] should be...
    
    Actually - let's just use the EXACT format from captured packets.
    We have the structure from analysis.
    """
    key = [random.randint(1, 254) for _ in range(3)]
    k0, k1, k2 = key
    
    plain = bytearray(238)
    # byte[0] = nonce0 (random)
    plain[0] = random.randint(0, 255)
    # byte[1]: cipher[1] must be 0x32, so plain[1] = 0x32 XOR k1
    plain[1] = 0x32 ^ k1
    # Rest of header (from our analysis of "perfect" BLACK packets)
    plain[2] = 0x00
    plain[3] = 0x00
    plain[4] = 0x00
    plain[5] = 0x00
    plain[6] = 0x05
    plain[7] = 0x05
    plain[8] = 0xFF
    plain[9] = 0xE3  # 227 = remaining
    plain[10] = 0x00
    plain[11] = 0x4B  # 75 LEDs
    # LED data
    for i in range(NUM_LEDS):
        base = 12 + i * 3
        plain[base] = r
        plain[base + 1] = g
        plain[base + 2] = b
    plain[237] = 0x00
    
    # Encrypt
    cipher = bytearray(238)
    for i in range(238):
        cipher[i] = plain[i] ^ key[i % 3]
    
    return bytes(cipher)


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    
    # Parse the captured handshake
    cap_path = os.path.join(CSV_DIR, "__pycache__", "surely_full_red.csv")
    if not os.path.exists(cap_path):
        cap_path = os.path.join(CSV_DIR, "red_full.csv")
    
    print(f"Loading capture: {os.path.basename(cap_path)}")
    writes = parse_writes(cap_path)
    print(f"Total writes in capture: {len(writes)}")
    
    # Separate handshake from color data
    # Frame headers are 5-byte 55AA5A writes
    # Data payloads follow immediately after
    # Find where big payloads (>=238 bytes) start
    handshake_writes = []
    color_start_idx = None
    for i, (ln, raw) in enumerate(writes):
        if len(raw) >= 238:
            color_start_idx = i
            # Include the frame header before this (previous write)
            if i > 0 and len(writes[i-1][1]) == 5:
                color_start_idx = i - 1
            break
        handshake_writes.append((ln, raw))
    
    if color_start_idx is None:
        color_start_idx = len(writes)
    
    print(f"Handshake writes: {len(handshake_writes)}")
    print(f"Color data starts at write index {color_start_idx}")
    
    # Show handshake sequence
    print(f"\n--- Handshake sequence to replay ---")
    for i, (ln, raw) in enumerate(handshake_writes[:20]):
        hex_str = raw.hex()
        if len(hex_str) > 60:
            hex_str = hex_str[:60] + "..."
        is_hdr = len(raw) == 5 and raw[0:3] == b'\x55\xAA\x5A'
        tag = "HDR" if is_hdr else "DAT"
        print(f"  [{i:3d}] {tag} len={len(raw):3d} {hex_str}")
    if len(handshake_writes) > 20:
        print(f"  ... and {len(handshake_writes) - 20} more")
    
    # ================================================================
    # Open serial and replay
    # ================================================================
    print(f"\n{'='*60}")
    print("REPLAYING HANDSHAKE")
    print(f"{'='*60}")
    
    # Try to open port with retries
    ser = None
    for attempt in range(5):
        try:
            ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=0.5)
            break
        except serial.SerialException:
            print(f"  Port busy, retry {attempt+1}/5... (close Beelight app!)")
            time.sleep(2)
    if ser is None:
        print("ERROR: Cannot open COM7. Close other apps and retry.")
        return
    
    # Set DTR like the app does (from IOCTL analysis)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)  # drain
    
    # Replay all handshake writes
    for i, (ln, raw) in enumerate(handshake_writes):
        is_hdr = len(raw) == 5 and raw[0:3] == b'\x55\xAA\x5A'
        
        ser.write(raw)
        ser.flush()
        
        if is_hdr:
            # Small delay between header and data
            time.sleep(0.005)
        else:
            # Longer delay after data to allow processing
            time.sleep(0.08)
            
            # Check for response
            resp = ser.read(4096)
            if resp and i < 30:
                print(f"  [{i:3d}] Response: {len(resp)} bytes: {resp[:20].hex()}...")
    
    print(f"Handshake replayed ({len(handshake_writes)} writes)")
    
    # Read any remaining response
    time.sleep(0.5)
    resp = ser.read(4096)
    if resp:
        print(f"Final response: {len(resp)} bytes")
    
    # ================================================================
    # Now also replay a few captured color packets
    # (to see if controller accepts them as-is)
    # ================================================================
    print(f"\n{'='*60}")
    print("REPLAYING CAPTURED COLOR PACKETS (RED)")
    print(f"{'='*60}")
    
    cap_color_writes = writes[color_start_idx:color_start_idx + 40]
    for i, (ln, raw) in enumerate(cap_color_writes):
        ser.write(raw)
        ser.flush()
        if len(raw) >= 238:
            time.sleep(0.075)
        else:
            time.sleep(0.005)
    
    print(f"Sent {len(cap_color_writes)} captured color writes")
    time.sleep(1.0)
    print("Check LED - did it turn RED from captured packets?")
    
    # ================================================================
    # Now try our OWN color packets
    # ================================================================
    print(f"\n{'='*60}")
    print("SENDING OUR GREEN PACKETS (5 seconds)")
    print(f"{'='*60}")
    
    start = time.time()
    count = 0
    while time.time() - start < 5.0:
        frame_hdr = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])
        payload = build_color_packet_simple(0, 255, 0)
        
        ser.write(frame_hdr)
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.075)
        count += 1
    
    print(f"Sent {count} GREEN packets")
    print("Check LED - did it turn GREEN?")
    
    # ================================================================
    # Turn off
    # ================================================================
    print(f"\n--- Turning OFF ---")
    for _ in range(20):
        frame_hdr = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])
        payload = build_color_packet_simple(0, 0, 0)
        ser.write(frame_hdr)
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.08)
    
    ser.close()
    print("\nDone! Report results:")
    print("  1. Did LED turn RED during captured packet replay?")
    print("  2. Did LED turn GREEN during our fresh packets?")


if __name__ == "__main__":
    main()
