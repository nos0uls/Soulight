# -*- coding: utf-8 -*-
"""
probe_hash_nonce.py — Test if GetHash(plaintext[2:]) == nonce.

If this is true, we can forge valid packets:
1. Build plaintext with brightness=0xFF and desired LED data
2. Compute hash = GetHash(plaintext[2:])
3. Set nonce = hash low 16 bits
4. Encrypt with key
5. Controller accepts!
"""
import os, sys, clr

BEELIGHT_EXE = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte
import System

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from soulight.protocol.bridge import BeelightBridge

ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static


def derive_key(cipher, period):
    key = [0] * period
    for i in range(period):
        src = 2 + ((i - 2) % period)
        key[i] = cipher[src]
    return bytes(key)


def decrypt(cipher, key):
    period = len(key)
    return bytes(cipher[i] ^ key[i % period] for i in range(len(cipher)))


def main():
    asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
    base_type = [t for t in asm.GetTypes() if t.Name == "LProtocolBase"][0]
    get_hash = base_type.GetMethod("GetHash", ALL)

    bridge = BeelightBridge()
    bridge.init()

    print("=" * 70)
    print("  HASH = NONCE TEST")
    print("=" * 70)

    # Generate many packets with different colors and check hash vs nonce
    test_colors = [
        ("RED", [(255, 0, 0)] * 75),
        ("GREEN", [(0, 255, 0)] * 75),
        ("BLUE", [(0, 0, 255)] * 75),
        ("WHITE", [(255, 255, 255)] * 75),
        ("BLACK", [(0, 0, 0)] * 75),
    ]

    matches = 0
    total = 0

    for color_name, colors in test_colors:
        for trial in range(10):
            pkt = bridge.make_rgb_transfer_packet(colors)
            if not pkt:
                continue

            payload = pkt[5:]
            pl = len(payload)
            period = pl - 235
            if period < 1:
                continue

            key = derive_key(payload, period)
            plain = decrypt(payload, key)

            nonce_hi = plain[0]
            nonce_lo = plain[1]
            nonce16 = (nonce_hi << 8) | nonce_lo

            # Call GetHash on plaintext[2:]
            plain_2 = bytes(plain[2:])
            arr = Array[Byte](plain_2)
            try:
                hash_val = get_hash.Invoke(None, Array[System.Object]([arr]))
                hash_lo16 = hash_val & 0xFFFF
                hash_hi16 = (hash_val >> 16) & 0xFFFF

                match_lo = "MATCH!" if hash_lo16 == nonce16 else ""
                match_hi = "MATCH!" if hash_hi16 == nonce16 else ""
                match_byte0 = "B0!" if (hash_val & 0xFF) == nonce_hi else ""
                match_byte1 = "B1!" if ((hash_val >> 8) & 0xFF) == nonce_lo else ""
                match_swap = "SWAP!" if ((hash_val & 0xFF) == nonce_lo and ((hash_val >> 8) & 0xFF) == nonce_hi) else ""

                if match_lo or match_hi or match_swap:
                    matches += 1

                total += 1

                if trial < 2:
                    print(f"  {color_name} #{trial} payload={pl} period={period}: "
                          f"nonce=0x{nonce16:04x} ({nonce_hi:02x} {nonce_lo:02x}), "
                          f"hash=0x{hash_val:08x} "
                          f"lo16=0x{hash_lo16:04x} {match_lo} "
                          f"hi16=0x{hash_hi16:04x} {match_hi} "
                          f"{match_swap}")
            except Exception as e:
                if trial < 2:
                    print(f"  {color_name} #{trial} payload={pl}: GetHash ERROR: {str(e)[:60]}")
                total += 1

    print(f"\n  Results: {matches}/{total} matches")

    # ================================================================
    # Also try: GetHash(plaintext_without_nonce) in various slices
    # ================================================================
    print(f"\n{'='*70}")
    print("  TRYING DIFFERENT HASH INPUTS")
    print("=" * 70)

    pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
    payload = pkt[5:]
    pl = len(payload)
    period = pl - 235
    key = derive_key(payload, period)
    plain = decrypt(payload, key)
    nonce16 = (plain[0] << 8) | plain[1]

    print(f"  Target nonce: 0x{nonce16:04x} ({plain[0]:02x} {plain[1]:02x})")
    print(f"  Payload len: {pl}, period: {period}")

    # Try various slices of plaintext
    slices = [
        ("plain[2:]", plain[2:]),
        ("plain[2:2+period]", plain[2:2+period]),
        ("plain[2+period:]", plain[2+period:]),
        ("plain[2+period+1:]", plain[2+period+1:]),
        ("header only (no nonce, no key)", plain[2+period:pl-225]),
        ("LED data only", plain[pl-225:]),
        ("plain[2:] + brightness=FF", bytes(plain[2:pl-225-5]) + b'\xff' + bytes(plain[pl-225-4:])),
    ]

    for name, data in slices:
        arr = Array[Byte](bytes(data))
        try:
            h = get_hash.Invoke(None, Array[System.Object]([arr]))
            lo = h & 0xFFFF
            hi = (h >> 16) & 0xFFFF
            match = ""
            if lo == nonce16: match = "LO16 MATCH!"
            elif hi == nonce16: match = "HI16 MATCH!"
            elif (h & 0xFF) == plain[1] and ((h >> 8) & 0xFF) == plain[0]: match = "SWAP MATCH!"
            print(f"  GetHash({name}, len={len(data)}): 0x{h:08x} lo=0x{lo:04x} hi=0x{hi:04x} {match}")
        except:
            print(f"  GetHash({name}, len={len(data)}): ERROR")

    # ================================================================
    # Try: is hash computed over CIPHER bytes?
    # ================================================================
    print(f"\n{'='*70}")
    print("  HASH OVER CIPHER BYTES?")
    print("=" * 70)

    cipher_nonce = (payload[0] << 8) | payload[1]
    print(f"  Cipher nonce: 0x{cipher_nonce:04x} ({payload[0]:02x} {payload[1]:02x})")

    cipher_slices = [
        ("cipher[2:]", payload[2:]),
        ("cipher[2:2+period]", payload[2:2+period]),
        ("cipher[2+period:]", payload[2+period:]),
    ]

    for name, data in cipher_slices:
        arr = Array[Byte](bytes(data))
        try:
            h = get_hash.Invoke(None, Array[System.Object]([arr]))
            lo = h & 0xFFFF
            hi = (h >> 16) & 0xFFFF
            match = ""
            if lo == cipher_nonce: match = "CIPHER LO MATCH!"
            elif hi == cipher_nonce: match = "CIPHER HI MATCH!"
            if lo == nonce16: match += " PLAIN LO MATCH!"
            elif hi == nonce16: match += " PLAIN HI MATCH!"
            print(f"  GetHash({name}, len={len(data)}): 0x{h:08x} lo=0x{lo:04x} hi=0x{hi:04x} {match}")
        except:
            print(f"  GetHash({name}, len={len(data)}): ERROR")


if __name__ == "__main__":
    main()
