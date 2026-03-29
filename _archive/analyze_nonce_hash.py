# -*- coding: utf-8 -*-
"""
analyze_nonce_hash.py — Test if nonce = hash(plaintext[2:]).

If two GenRGBTransferPackage packets have the same payload size and same
LED data, their plaintext[2:] is identical. If nonce is deterministic,
their nonces should match. If random, they won't.

Also: try to reverse-engineer the hash function by testing common algorithms.
"""
import os, sys, clr
from collections import defaultdict
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

BEELIGHT_EXE = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
clr.AddReference(BEELIGHT_EXE)

from System.Drawing import Color
from soulight.protocol.bridge import BeelightBridge


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
    bridge = BeelightBridge()
    bridge.init()

    # ================================================================
    # 1. Generate 200 GenRGBTransferPackage RED, group by payload size
    # ================================================================
    print("=" * 70)
    print("  NONCE DETERMINISM TEST")
    print("=" * 70)

    by_size = defaultdict(list)
    for _ in range(200):
        pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
        if pkt:
            payload = pkt[5:]
            by_size[len(payload)].append(payload)

    print(f"\n  Generated packets by payload size:")
    for sz in sorted(by_size.keys()):
        print(f"    payload={sz}: {len(by_size[sz])} packets")

    # For each size group, check if nonces are deterministic
    print(f"\n  Nonce analysis per size group:")
    for sz in sorted(by_size.keys()):
        pkts = by_size[sz]
        if len(pkts) < 2:
            continue
        period = sz - 235
        if period < 1:
            continue

        nonces = set()
        plains_2_onwards = set()
        for p in pkts:
            key = derive_key(p, period)
            plain = decrypt(p, key)
            nonce = (plain[0], plain[1])
            nonces.add(nonce)
            # Check if plaintext[2:] is the same for all
            plains_2_onwards.add(plain[2:])

        unique_nonces = len(nonces)
        unique_plains = len(plains_2_onwards)
        total = len(pkts)

        if unique_plains == 1 and unique_nonces == 1:
            verdict = "DETERMINISTIC: nonce = f(plaintext[2:])"
        elif unique_plains == 1 and unique_nonces > 1:
            verdict = "RANDOM: same plain, different nonces"
        elif unique_plains > 1:
            verdict = f"MIXED: {unique_plains} different plaintexts"
        else:
            verdict = "???"

        sample_nonce = list(nonces)[0]
        print(f"    payload={sz} (period={period}): {total} pkts, "
              f"{unique_nonces} unique nonces, {unique_plains} unique plains")
        print(f"      -> {verdict}")
        if unique_nonces <= 3:
            for n in sorted(nonces):
                print(f"         nonce = ({n[0]:02x}, {n[1]:02x})")

    # ================================================================
    # 2. If deterministic, try to find the hash function
    # ================================================================
    print(f"\n" + "=" * 70)
    print(f"  HASH FUNCTION ANALYSIS")
    print(f"=" * 70)

    # Collect (plaintext[2:], nonce) pairs across different sizes/colors
    samples = []

    for color_rgb, name in [
        ((255, 0, 0), "RED"),
        ((0, 255, 0), "GREEN"),
        ((0, 0, 255), "BLUE"),
        ((255, 255, 255), "WHITE"),
        ((0, 0, 0), "BLACK"),
    ]:
        for _ in range(50):
            pkt = bridge.make_rgb_transfer_packet([color_rgb] * 75)
            if pkt:
                payload = pkt[5:]
                pl = len(payload)
                period = pl - 235
                if period < 1: continue
                key = derive_key(payload, period)
                plain = decrypt(payload, key)
                nonce16 = (plain[0] << 8) | plain[1]
                samples.append({
                    "color": name,
                    "payload_len": pl,
                    "period": period,
                    "plain_2": plain[2:],
                    "nonce": nonce16,
                    "nonce_bytes": (plain[0], plain[1]),
                })

    # Group by (color, payload_len) and check determinism
    by_group = defaultdict(list)
    for s in samples:
        by_group[(s["color"], s["payload_len"])].append(s)

    print(f"\n  Samples by (color, size):")
    for (color, pl), group in sorted(by_group.items()):
        nonces = set(s["nonce"] for s in group)
        if len(group) >= 2:
            det = "DET" if len(nonces) == 1 else f"RANDOM({len(nonces)} unique)"
            sample_n = list(nonces)[0]
            print(f"    {color:6s} payload={pl}: {len(group)} pkts, "
                  f"nonces={det}, sample=0x{sample_n:04x}")

    # ================================================================
    # 3. Try common hash algorithms
    # ================================================================
    # Pick one size group with deterministic nonce
    det_groups = [(k, v) for k, v in by_group.items() if len(set(s["nonce"] for s in v)) == 1 and len(v) >= 2]

    if det_groups:
        print(f"\n  Testing hash algorithms on deterministic groups:")

        import struct
        import zlib

        for (color, pl), group in det_groups[:5]:
            plain_2 = group[0]["plain_2"]
            expected = group[0]["nonce"]
            n_bytes = group[0]["nonce_bytes"]

            # Test various hash algorithms
            data = bytes(plain_2)

            # Simple sum
            s16 = sum(data) & 0xFFFF
            # XOR fold
            xor16 = 0
            for i in range(0, len(data) - 1, 2):
                xor16 ^= (data[i] << 8) | data[i + 1]
            if len(data) % 2:
                xor16 ^= data[-1] << 8
            # CRC-16
            crc16_val = zlib.crc32(data) & 0xFFFF
            # CRC-32 low 16 bits
            crc32_full = zlib.crc32(data)
            crc32_lo = crc32_full & 0xFFFF
            crc32_hi = (crc32_full >> 16) & 0xFFFF
            # Sum of bytes modulo
            sum_mod = sum(data) % 65536
            # Fletcher-16
            s1 = s2 = 0
            for b in data:
                s1 = (s1 + b) % 255
                s2 = (s2 + s1) % 255
            fletcher = (s2 << 8) | s1

            print(f"\n    {color} payload={pl}: expected nonce = 0x{expected:04x} ({n_bytes[0]:02x} {n_bytes[1]:02x})")
            print(f"      sum16      = 0x{s16:04x} {'MATCH!' if s16 == expected else ''}")
            print(f"      xor16      = 0x{xor16:04x} {'MATCH!' if xor16 == expected else ''}")
            print(f"      crc16(z)   = 0x{crc16_val:04x} {'MATCH!' if crc16_val == expected else ''}")
            print(f"      crc32_lo   = 0x{crc32_lo:04x} {'MATCH!' if crc32_lo == expected else ''}")
            print(f"      crc32_hi   = 0x{crc32_hi:04x} {'MATCH!' if crc32_hi == expected else ''}")
            print(f"      fletcher16 = 0x{fletcher:04x} {'MATCH!' if fletcher == expected else ''}")
            print(f"      sum_mod    = 0x{sum_mod:04x} {'MATCH!' if sum_mod == expected else ''}")

            # Also try over just LED data
            cs = pl - 225
            led_data = bytes(plain_2[cs - 2:])  # from color_start
            led_s16 = sum(led_data) & 0xFFFF
            led_xor = 0
            for i in range(0, len(led_data) - 1, 2):
                led_xor ^= (led_data[i] << 8) | led_data[i + 1]
            print(f"      LED sum16  = 0x{led_s16:04x} {'MATCH!' if led_s16 == expected else ''}")
            print(f"      LED xor16  = 0x{led_xor:04x} {'MATCH!' if led_xor == expected else ''}")
    else:
        print(f"\n  No deterministic groups found (nonces are RANDOM)")
        # If random, check the OTHER end: are there patterns in cipher nonces?
        print(f"\n  Checking CIPHER nonce patterns:")
        for sz in sorted(by_size.keys()):
            pkts = by_size[sz]
            if len(pkts) < 5: continue
            cipher_nonces = [(p[0], p[1]) for p in pkts[:10]]
            print(f"    payload={sz}: cipher nonces = {['%02x%02x' % n for n in cipher_nonces[:5]]}")


if __name__ == "__main__":
    main()
