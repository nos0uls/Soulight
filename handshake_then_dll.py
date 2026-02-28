# -*- coding: utf-8 -*-
"""
handshake_then_dll.py - Replay handshake, then send DLL-generated packets.

The handshake replay switches controller to PC mode.
Then we try: DLL packets (MgL format), unencrypted wire packets,
and XOR key=[00,00,00] wire packets.

Run with 32-bit Python!
"""
import sys, os, time, ctypes, struct, random

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll")

try:
    import serial
except ImportError:
    print("pip install pyserial"); sys.exit(1)

COM_PORT = "COM7"
BAUD = 500000


def parse_writes(filepath):
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for ln, line in enumerate(f):
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5: continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
            except ValueError: continue
            if raw: writes.append(raw)
    return writes


def get_handshake_writes(filepath):
    """Get all writes before first big (>=238) payload."""
    writes = parse_writes(filepath)
    result = []
    for w in writes:
        if len(w) >= 238:
            break
        result.append(w)
    return result


def send_handshake(ser, hs_writes):
    """Replay handshake writes."""
    for raw in hs_writes:
        ser.write(raw)
        ser.flush()
        is_hdr = len(raw) == 5 and raw[0:3] == b'\x55\xAA\x5A'
        time.sleep(0.005 if is_hdr else 0.08)
    # Drain responses
    time.sleep(0.5)
    resp = ser.read(8192)
    return resp


def build_plain_color(r, g, b, num_leds=75):
    """Build UNENCRYPTED 238-byte wire packet (key=000000)."""
    # byte[1] for len=238 must be 0x32
    plain = bytearray(238)
    plain[0] = random.randint(0, 255)
    plain[1] = 0x32  # byte[1] = 0x32 for len=238 (no XOR since key=0)
    plain[6] = 0x05; plain[7] = 0x05; plain[8] = 0xFF
    plain[9] = 0xE3; plain[11] = 0x4B
    for i in range(num_leds):
        plain[12+i*3] = r; plain[13+i*3] = g; plain[14+i*3] = b
    return bytes(plain)


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    if struct.calcsize("P") * 8 != 32:
        print("Need 32-bit Python!"); return

    dll = ctypes.CDLL(DLL_PATH)
    print("DLL loaded")

    # Load handshake
    cap = os.path.join(CSV_DIR, "__pycache__", "surely_full_red.csv")
    hs = get_handshake_writes(cap)
    print(f"Handshake: {len(hs)} writes")

    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    ser.dtr = True; ser.rts = True
    time.sleep(0.3); ser.read(4096)

    # Phase 1: Handshake
    print("\n=== HANDSHAKE ===")
    resp = send_handshake(ser, hs)
    print(f"Response: {len(resp)} bytes")

    # Phase 2: DLL scen_package (RED) - MgL format
    print("\n=== TEST A: DLL scen_package RED (10s) ===")
    start = time.time()
    n = 0
    while time.time() - start < 10.0:
        buf = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf)
        raw = buf.raw
        end = max((i for i in range(len(raw)) if raw[i]), default=0)
        ser.write(raw[:end+1]); ser.flush()
        time.sleep(0.08); n += 1
    print(f"Sent {n} DLL RED packets. LED RED?")
    time.sleep(1)

    # Phase 3: DLL large_screendata (RED)
    print("\n=== TEST B: DLL large_screendata RED (5s) ===")
    # Build 50x1 RED image
    img = (ctypes.c_ubyte * 150)()
    for i in range(50):
        img[i*3] = 255; img[i*3+1] = 0; img[i*3+2] = 0
    start = time.time(); n = 0
    while time.time() - start < 5.0:
        buf2 = ctypes.create_string_buffer(2048)
        try:
            dll.get_large_screendata_package(1, 50, img, 150, buf2)
            raw2 = buf2.raw
            end2 = max((i for i in range(min(1515,len(raw2))) if raw2[i]), default=0)
            if end2 > 5:
                ser.write(raw2[:end2+1]); ser.flush()
                time.sleep(0.08); n += 1
        except: break
    print(f"Sent {n} DLL screendata packets. LED RED?")
    time.sleep(1)

    # Phase 4: Unencrypted wire packets (key=000000)
    print("\n=== TEST C: Unencrypted wire GREEN (5s) ===")
    start = time.time(); n = 0
    while time.time() - start < 5.0:
        hdr = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])
        payload = build_plain_color(0, 255, 0)
        ser.write(hdr); ser.flush(); time.sleep(0.005)
        ser.write(payload); ser.flush(); time.sleep(0.075); n += 1
    print(f"Sent {n} unencrypted GREEN packets. LED GREEN?")

    # Phase 5: Wire packets with all possible simple keys
    print("\n=== TEST D: Wire BLUE with key from response ===")
    # Try using bytes from controller's first response as key
    resp_bytes = resp[:30] if resp else b'\x00' * 30
    keys_to_try = [
        [0x00, 0x00, 0x00],
        [resp_bytes[0], resp_bytes[1], resp_bytes[2]] if len(resp_bytes) > 2 else [0,0,0],
        [resp_bytes[5], resp_bytes[6], resp_bytes[7]] if len(resp_bytes) > 7 else [0,0,0],
    ]
    for ki, key in enumerate(keys_to_try):
        print(f"  Key {ki}: [{key[0]:02x},{key[1]:02x},{key[2]:02x}]")
        for _ in range(20):
            plain = bytearray(238)
            plain[0] = random.randint(0,255)
            plain[1] = 0x32 ^ key[1]
            plain[6] = 0x05; plain[7] = 0x05; plain[8] = 0xFF
            plain[9] = 0xE3; plain[11] = 0x4B
            for i in range(75):
                plain[12+i*3] = 0; plain[13+i*3] = 0; plain[14+i*3] = 255
            cipher = bytes(plain[i] ^ key[i%3] for i in range(238))
            ser.write(bytes([0x55,0xAA,0x5A,0xEE,0x00]))
            ser.flush(); time.sleep(0.005)
            ser.write(cipher); ser.flush(); time.sleep(0.075)
        time.sleep(1)
        print(f"  Key {ki} done. LED BLUE?")

    ser.close()
    print("\nDone! Report which test (A/B/C/D) changed color.")


if __name__ == "__main__":
    main()
