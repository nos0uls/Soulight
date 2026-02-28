# -*- coding: utf-8 -*-
"""
verify_constant_key.py - Verify that wire cipher is PURE 3-byte XOR with constant key.

HYPOTHESIS: cipher[i] = plaintext[i] XOR key[i % 3]
Key is CONSTANT across the entire packet. No evolution.

The "evolution" we saw was because header bytes 6-11 are NOT zero:
  plain[6]=0x05, plain[7]=0x05, plain[8]=0xFF, plain[9]=0xE3, plain[10]=0x00, plain[11]=0x4B

Verification:
  c[3] XOR 0 = key[0]  (plain[3]=0)
  c[6] XOR 0x05 should = key[0]
  c[9] XOR 0xE3 should = key[0]
  c[12] XOR 0 should = key[0]  (plain[12]=0 for BLACK)

For the "imperfect" packets: they have DIFFERENT plaintext headers.
They might have different values for bytes 6-11.
"""
import sys
import os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


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
    sys.stdout.reconfigure(encoding="utf-8")

    colors = {
        "black": (0, 0, 0),
        "red": (255, 0, 0),
        "green": (0, 255, 0),
        "blue": (0, 0, 255),
        "white": (255, 255, 255),
    }

    for color, (r, g, b) in colors.items():
        path = os.path.join(CSV_DIR, f"{color}.csv")
        if not os.path.exists(path):
            continue

        pairs_238 = [(h, p) for h, p in parse_paired_writes(path) if len(p) == 238]
        if not pairs_238:
            continue

        print(f"\n{'='*60}")
        print(f"{color.upper()} (R={r},G={g},B={b}) - {len(pairs_238)} packets of 238 bytes")
        print(f"{'='*60}")

        # For 238-byte: 75 LEDs, plaintext:
        # [n0, n1, ??, ??, ??, ??, ??, ??, ??, ??, ??, ??, R,G,B x75, ??]
        # We know LED region: positions 12-236 = R,G,B repeating
        # Tail: position 237

        perfect = 0
        for pidx, (hdr, payload) in enumerate(pairs_238):
            # Extract key from first LED triplet
            k0 = payload[12] ^ r
            k1 = payload[13] ^ g
            k2 = payload[14] ^ b
            key3 = [k0, k1, k2]

            # Verify: does this key decrypt the ENTIRE LED region correctly?
            led_ok = True
            for j in range(75):
                base = 12 + j * 3
                pr = payload[base] ^ k0
                pg = payload[base + 1] ^ k1
                pb = payload[base + 2] ^ k2
                if pr != r or pg != g or pb != b:
                    led_ok = False
                    break

            # Now check: does c[3] XOR 0 = k0?
            # (plain[3] should be 0 if our header is [n0,n1,00,00,00,00,...])
            hdr_check_0 = (payload[3] == k0)  # plain[3]=0?
            hdr_check_1 = (payload[4] == k1)  # plain[4]=0?
            hdr_check_2 = (payload[5] == k2)  # plain[5]=0?

            # Check c[6]: plain[6] should be some value X
            # c[6] XOR k0 = X -> X = payload[6] XOR k0
            plain6 = payload[6] ^ k0
            plain7 = payload[7] ^ k1
            plain8 = payload[8] ^ k2
            plain9 = payload[9] ^ k0
            plain10 = payload[10] ^ k1
            plain11 = payload[11] ^ k2

            # Tail
            plain237 = payload[237] ^ key3[237 % 3]

            if led_ok:
                perfect += 1

            if pidx < 5 or not led_ok:
                plain_hdr = bytes([payload[i] ^ key3[i % 3] for i in range(12)])
                print(f"  Pkt {pidx}: key=[{k0:02x},{k1:02x},{k2:02x}] "
                      f"led_ok={led_ok} hdr_match_345={hdr_check_0 and hdr_check_1 and hdr_check_2}")
                print(f"    plain[0:12]=[{' '.join(f'{b:02x}' for b in plain_hdr)}]")
                print(f"    plain[6:12]=[{plain6:02x} {plain7:02x} {plain8:02x} "
                      f"{plain9:02x} {plain10:02x} {plain11:02x}]")
                print(f"    plain[237]={plain237:02x}")

        print(f"\n  LED-perfect packets: {perfect}/{len(pairs_238)}")

    # ================================================================
    # Now check ALL packet lengths to see if header varies
    # ================================================================
    print(f"\n{'='*60}")
    print("Checking ALL packet lengths for BLACK")
    print(f"{'='*60}")

    all_pairs = parse_paired_writes(os.path.join(CSV_DIR, "black.csv"))
    by_len = {}
    for hdr, payload in all_pairs:
        plen = len(payload)
        if plen not in by_len:
            by_len[plen] = []
        by_len[plen].append((hdr, payload))

    for plen in sorted(by_len.keys()):
        packets = by_len[plen]
        # For BLACK: LED data = 0, so cipher[LED_region] = key directly
        # But we don't know where LED region starts for different lengths
        # Assume similar structure: header + LED + tail
        # 238 = 12 header + 225 LED (75*3) + 1 tail
        # N = 12 + LED_bytes + 1 -> LED_bytes = N - 13
        # num_leds = LED_bytes / 3

        led_bytes = plen - 13
        if led_bytes % 3 != 0:
            # Try different header sizes
            for hdr_size in [10, 11, 12, 13, 14]:
                test_led = plen - hdr_size - 1
                if test_led > 0 and test_led % 3 == 0:
                    led_bytes = test_led
                    break

        num_leds = led_bytes // 3 if led_bytes % 3 == 0 else -1

        pkt = packets[0][1]
        # Try extracting key from position 12 (assuming header=12)
        k0, k1, k2 = pkt[12], pkt[13], pkt[14]
        # Verify on a few more positions
        match_count = 0
        for j in range(12, min(12 + 30, plen - 1), 3):
            if pkt[j] == k0 and pkt[j+1] == k1 and pkt[j+2] == k2:
                match_count += 1
            else:
                break

        plain_hdr = bytes([pkt[i] ^ [k0, k1, k2][i % 3] for i in range(min(14, plen))])

        print(f"\n  len={plen}: {len(packets)} pkts, led_bytes={led_bytes}, "
              f"num_leds={num_leds}, key_repeat={match_count}")
        print(f"    key=[{k0:02x},{k1:02x},{k2:02x}]")
        print(f"    plain[0:14]=[{' '.join(f'{b:02x}' for b in plain_hdr)}]")

    # ================================================================
    # Collect unique plaintext headers across all BLACK 238-byte packets
    # ================================================================
    print(f"\n{'='*60}")
    print("Unique plaintext headers (BLACK 238-byte)")
    print(f"{'='*60}")

    headers = set()
    for hdr, payload in by_len.get(238, []):
        k0, k1, k2 = payload[12], payload[13], payload[14]
        plain_hdr = tuple(payload[i] ^ [k0, k1, k2][i % 3] for i in range(2, 12))
        headers.add(plain_hdr)

    print(f"Unique header patterns (bytes 2-11): {len(headers)}")
    for h in sorted(headers):
        count = sum(1 for _, p in by_len.get(238, [])
                    if tuple(p[i] ^ [p[12], p[13], p[14]][i % 3] for i in range(2, 12)) == h)
        print(f"  [{' '.join(f'{b:02x}' for b in h)}] x{count}")


if __name__ == "__main__":
    main()
