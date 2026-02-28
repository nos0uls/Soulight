# -*- coding: utf-8 -*-
"""
build_sender.py - Understand the FULL plaintext format and build a working sender.

From verify_constant_key.py results:

BLACK 238-byte, header pattern [00 00 00 00 05 05 ff e3 00 4b] (19 packets):
  plain = [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B, 00*75*3, 00]
  This is 2+4+2+4+225+1 = 238. Checks out.
  
  0x4B = 75 = number of LEDs
  0xE3 = 227 = 75*3 + 2? or 238-11 = 227? 
  Actually 0xE3 = 227 = total_len - 11 = payload_data_len?
  238 - 11 = 227. Yes! E3 is the remaining length after the 11-byte header.
  
  byte[8] = 0xFF = ?
  byte[6:8] = 05 05 = cmd and sub-cmd? (CTRL_DEVICE=5)
  byte[2:6] = 00 00 00 00 = padding/zeros
  byte[0:2] = nonce (changes per packet)
  byte[11] = 0x4B = 75 = num_leds
  byte[237] = 0x00 = tail/checksum

RED 238-byte, same header pattern should give:
  plain = [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B, FF,00,00 * 75, ??]

But RED showed led_ok=False for all 18 packets! And headers like:
  [54 7c 5f ff 00 5f fa 05 a0 1c 00 14]
  
That's very different from BLACK. plain[2]=0x5F, not 0x00.
And plain[6]=0xFA (not 0x05), plain[8]=0xA0 (not 0xFF).

Wait: 0x05 XOR 0xFF = 0xFA. And 0xFF XOR 0x5F = 0xA0.
So: plain_red[6] = plain_black[6] XOR 0xFF = 0x05 XOR 0xFF = 0xFA
And: plain_red[8] = plain_black[8] XOR 0x5F = 0xFF XOR 0x5F = 0xA0

This is suspicious. It looks like the RED value (0xFF) is being XOR'd
into the header. As if the header is ALSO encrypted with the LED color?
No - that doesn't make sense.

Actually: 0x4B XOR 0x5F = 0x14. And plain_red[11] = 0x14.
0x4B = 75. 0x14 = 20. 75 XOR 0x5F != 20 meaningfully.

Hmm, let me look at this differently.
For BLACK: key extracted from pos 12,13,14 = [e2,14,b5] (first packet)
For this key: plain[2] = cipher[2] XOR key[2] = ea XOR b5 = 5f. Not 00!

But earlier we had the 19 "perfect" packets where plain[2:12] = [00,00,00,00,05,05,ff,e3,00,4b].
Those were extracted with key from pos 12 as well. So for THOSE packets,
cipher[2] XOR key[2] = 0x00 -> cipher[2] = key[2].
But for the "imperfect" ones: cipher[2] != key[2].

THIS IS THE KEY: the "key" at position 12 is NOT the same as position 2!
The cipher is NOT simple 3-byte XOR with a constant key.
The key CHANGES position.

But KS_A XOR KS_B is perfect period-3! So the DIFFERENCE between keys
at any two positions is always the same regardless of packet.

This means: ks[i] = base_key[i%3] XOR position_mask[i]
where position_mask[i] is SHARED across all packets.

For the "perfect" 19 packets: position_mask happens to be 0 at positions
12-236 AND 2-11. So those packets have a key that doesn't need the mask.

For the "imperfect" packets: the mask is non-zero at some positions.

I can extract position_mask by XOR-ing any two packets.
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

    black_pairs = [(h, p) for h, p in parse_paired_writes(os.path.join(CSV_DIR, "black.csv"))
                   if len(p) == 238]
    
    # Separate "perfect" (header=00...) and "imperfect"
    perfect = []
    imperfect = []
    
    for hdr, payload in black_pairs:
        k = [payload[12], payload[13], payload[14]]
        plain2 = payload[2] ^ k[2]
        if plain2 == 0x00:
            perfect.append((hdr, payload))
        else:
            imperfect.append((hdr, payload))
    
    print(f"Perfect (plain[2]=00): {len(perfect)}")
    print(f"Imperfect: {len(imperfect)}")
    
    # For perfect packets: ks = constant per mod-3 channel across ALL positions
    # For imperfect: ks changes at certain positions
    
    # XOR a perfect and imperfect packet ciphertext
    # XOR = (plain_A XOR ks_A) XOR (plain_B XOR ks_B)
    # If plain_A == plain_B (both BLACK, same format):
    #   XOR = ks_A XOR ks_B = constant period-3
    # If plain_A != plain_B (different header):
    #   XOR = (plain_A XOR plain_B) XOR (ks_A XOR ks_B)
    
    # XOR two perfect packets -> should be constant period-3 in LED region
    p1 = perfect[0][1]
    p2 = perfect[1][1]
    xor_pp = bytes(a ^ b for a, b in zip(p1, p2))
    print(f"\nPerfect XOR Perfect [12:24]: {' '.join(f'{b:02x}' for b in xor_pp[12:24])}")
    print(f"  Period-3 check: {xor_pp[12]==xor_pp[15]==xor_pp[18]==xor_pp[21]}")
    
    # XOR perfect and imperfect
    i1 = imperfect[0][1]
    xor_pi = bytes(a ^ b for a, b in zip(p1, i1))
    print(f"\nPerfect XOR Imperfect [2:24]: {' '.join(f'{b:02x}' for b in xor_pi[2:24])}")
    print(f"  LED region [12:24]: {' '.join(f'{b:02x}' for b in xor_pi[12:24])}")
    # LED region should still be period-3 (both are BLACK, same plaintext)
    led_p3 = (xor_pi[12] == xor_pi[15] == xor_pi[18] and
              xor_pi[13] == xor_pi[16] == xor_pi[19])
    print(f"  LED period-3: {led_p3}")
    
    # Header region: XOR should show plaintext difference
    # If both are BLACK (same plaintext), then header XOR = ks difference = period-3
    hdr_p3 = (xor_pi[3] == xor_pi[12] and xor_pi[4] == xor_pi[13] and xor_pi[5] == xor_pi[14])
    print(f"  Header bytes [3:6] match LED [12:15]: {hdr_p3}")
    print(f"    xor[3]={xor_pi[3]:02x} xor[12]={xor_pi[12]:02x}")
    print(f"    xor[4]={xor_pi[4]:02x} xor[13]={xor_pi[13]:02x}")
    print(f"    xor[5]={xor_pi[5]:02x} xor[14]={xor_pi[14]:02x}")
    
    # ================================================================
    # KEY TEST: Are BOTH types really BLACK?
    # Maybe "imperfect" packets have different plaintext (not all-zero LED)?
    # Let's check: for imperfect packets, do bytes 12+ show a repeating pattern?
    # ================================================================
    print(f"\n=== Imperfect packet LED region analysis ===")
    for pidx, (hdr, payload) in enumerate(imperfect[:5]):
        # Check if LED region (12:237) has period-3
        trips = set()
        for j in range(12, 237, 3):
            if j + 2 < 238:
                trips.add((payload[j], payload[j+1], payload[j+2]))
        
        # If period-3 with constant key and same plaintext, there should be 1 unique triplet
        print(f"  Imperfect pkt {pidx}: unique triplets in LED region: {len(trips)}")
        if len(trips) <= 5:
            for t in sorted(trips):
                count = sum(1 for j in range(12, 237, 3) if j+2 < 238 and
                            payload[j]==t[0] and payload[j+1]==t[1] and payload[j+2]==t[2])
                print(f"    [{t[0]:02x} {t[1]:02x} {t[2]:02x}] x{count}")
        else:
            # Count most common
            from collections import Counter
            trip_list = []
            for j in range(12, 237, 3):
                if j + 2 < 238:
                    trip_list.append((payload[j], payload[j+1], payload[j+2]))
            c = Counter(trip_list)
            for t, cnt in c.most_common(3):
                print(f"    [{t[0]:02x} {t[1]:02x} {t[2]:02x}] x{cnt}")
    
    # ================================================================
    # If imperfect packets also have 1 dominant triplet, it's still period-3 XOR
    # but with a DIFFERENT key. The "imperfect" label was wrong - the plaintext
    # header is just different for those packets.
    # 
    # REAL QUESTION: can we just use ANY key3 and ANY nonce?
    # The controller must validate something. What?
    # 
    # From the data: c[1] = 0x32 for ALL 238-byte packets.
    # plain[1] = n1, cipher[1] = n1 XOR k1 = 0x32
    # So: n1 = 0x32 XOR k1. The controller sees c[1]=0x32 and knows len=238.
    # This is NOT a nonce - it's a LENGTH INDICATOR!
    #
    # And c[0] varies. plain[0] = n0 (nonce). key[0] varies.
    # c[0] = n0 XOR k0.
    #
    # Similarly: c[2] and c[5] were the SAME in every packet!
    # c[2] = plain[2] XOR k2. c[5] = plain[5] XOR k2.
    # If c[2] == c[5], then plain[2] == plain[5].
    # From our decrypted headers: plain[2] == plain[5] confirmed (e.g. 5f,5f or 00,00).
    #
    # So the first 6 bytes of plaintext: [n0, len_indicator, X, 0, 0, X]
    # where X = some value that varies.
    #
    # The controller decrypts with some key derivation and checks the structure.
    # If we can figure out how key3 is derived, we can send any color.
    # ================================================================
    
    # Let me collect ALL (n0, key3) pairs from perfect packets
    # where we KNOW the plaintext header is [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B]
    # So: n0 = payload[0] XOR k0
    #     n1 = payload[1] XOR k1
    #     k0 = payload[12], k1 = payload[13], k2 = payload[14]
    
    print(f"\n{'='*60}")
    print("Nonce -> Key mapping (from perfect 238-byte BLACK packets)")
    print(f"{'='*60}")
    
    nk_pairs = []
    for hdr, payload in perfect:
        k0, k1, k2 = payload[12], payload[13], payload[14]
        n0 = payload[0] ^ k0
        n1 = payload[1] ^ k1
        nk_pairs.append((n0, n1, k0, k1, k2))
        print(f"  n0=0x{n0:02x} n1=0x{n1:02x} -> k0=0x{k0:02x} k1=0x{k1:02x} k2=0x{k2:02x}")
    
    # Check: n1 XOR k1 should always be 0x32 (since c[1]=0x32)
    for n0, n1, k0, k1, k2 in nk_pairs:
        assert n1 ^ k1 == 0x32, f"n1 XOR k1 != 0x32: {n1:02x} ^ {k1:02x} = {n1^k1:02x}"
    print(f"\n  Confirmed: n1 XOR k1 = 0x32 for all packets")
    
    # n1 = 0x32 XOR k1 -> there's only one free variable between n1 and k1
    # The controller sends c[1]=0x32, and both n1 and k1 are derived from some seed.
    
    # Look for patterns:
    print(f"\n  k0 values: {[f'{k0:02x}' for _,_,k0,_,_ in nk_pairs]}")
    print(f"  k1 values: {[f'{k1:02x}' for _,_,_,k1,_ in nk_pairs]}")
    print(f"  k2 values: {[f'{k2:02x}' for _,_,_,_,k2 in nk_pairs]}")
    print(f"  n0 values: {[f'{n0:02x}' for n0,_,_,_,_ in nk_pairs]}")
    
    # Check: is k0 derived from n0?
    print(f"\n  k0 XOR n0: {[f'{k0^n0:02x}' for n0,_,k0,_,_ in nk_pairs]}")
    print(f"  k2 XOR n0: {[f'{k2^n0:02x}' for n0,_,_,_,k2 in nk_pairs]}")
    
    # Check: k0+n0, k0-n0, k0*n0 mod 256
    print(f"  (k0+n0)%256: {[(k0+n0)%256 for n0,_,k0,_,_ in nk_pairs]}")
    
    # Try: is there a linear relationship?
    # k0 = a * n0 + b (mod 256)?
    if len(nk_pairs) >= 2:
        n0_a, _, k0_a, _, _ = nk_pairs[0]
        n0_b, _, k0_b, _, _ = nk_pairs[1]
        # k0_a = a * n0_a + b
        # k0_b = a * n0_b + b
        # k0_a - k0_b = a * (n0_a - n0_b)
        dn = (n0_a - n0_b) % 256
        dk = (k0_a - k0_b) % 256
        
        if dn != 0:
            # Find a such that a * dn = dk (mod 256)
            for a in range(256):
                if (a * dn) % 256 == dk:
                    b = (k0_a - a * n0_a) % 256
                    # Verify on all pairs
                    ok = all((a * n0 + b) % 256 == k0 for n0, _, k0, _, _ in nk_pairs)
                    if ok:
                        print(f"\n  *** FOUND: k0 = ({a} * n0 + {b}) % 256 ***")
                        break
    
    # Do the same for k2
    if len(nk_pairs) >= 2:
        n0_a, _, _, _, k2_a = nk_pairs[0]
        n0_b, _, _, _, k2_b = nk_pairs[1]
        dn = (n0_a - n0_b) % 256
        dk2 = (k2_a - k2_b) % 256
        
        if dn != 0:
            for a in range(256):
                if (a * dn) % 256 == dk2:
                    b = (k2_a - a * n0_a) % 256
                    ok = all((a * n0 + b) % 256 == k2 for n0, _, _, _, k2 in nk_pairs)
                    if ok:
                        print(f"  *** FOUND: k2 = ({a} * n0 + {b}) % 256 ***")
                        break
    
    # Try lookup table approach: maybe k0 = TABLE[n0]?
    from cipher_table_runtime import CIPHER_TABLE
    for offset in range(len(CIPHER_TABLE)):
        match = all(CIPHER_TABLE[(n0 + offset) % len(CIPHER_TABLE)] == k0 
                    for n0, _, k0, _, _ in nk_pairs)
        if match:
            print(f"\n  *** FOUND: k0 = TABLE[(n0 + {offset}) % {len(CIPHER_TABLE)}] ***")
            break
    
    for offset in range(len(CIPHER_TABLE)):
        match = all(CIPHER_TABLE[(n0 + offset) % len(CIPHER_TABLE)] == k2 
                    for n0, _, _, _, k2 in nk_pairs)
        if match:
            print(f"  *** FOUND: k2 = TABLE[(n0 + {offset}) % {len(CIPHER_TABLE)}] ***")
            break
    
    # Check k1: it's related to n1 via n1 XOR k1 = 0x32
    # n1 is the second nonce byte. Is n1 random, or derived from n0?
    print(f"\n  n0 vs n1 relationship:")
    for n0, n1, k0, k1, k2 in nk_pairs[:10]:
        print(f"    n0=0x{n0:02x} n1=0x{n1:02x} n0^n1=0x{n0^n1:02x} "
              f"n0+n1={n0+n1} k1=0x{k1:02x}")

    print("\nDONE")


if __name__ == "__main__":
    main()
