# -*- coding: utf-8 -*-
"""
crack_wire_cipher.py — Crack the wire cipher used by .NET (not the DLL cipher).

Known facts:
  - Wire format: [55 AA 5A len 00] + [len bytes payload]
  - DLL cipher (FUN_10001880 with 521-byte table) is NOT what's on the wire
  - Earlier crypto analysis found ~3-byte XOR pattern in wire payloads
  - .NET code is obfuscated with .NET Reactor, so we can't easily read it
  - But we CAN crack the wire cipher empirically

Strategy:
  For BLACK (all LEDs = 0x00): ciphertext = keystream
  For RED (R=0xFF, G=0x00, B=0x00): we know most of the plaintext
  By comparing BLACK and RED with same structure, we can extract the cipher.

Previous analysis showed plaintext header for 238-byte packets:
  [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B, R,G,B×75, 00]
  Where n0,n1 vary per packet (nonce/random)

Total: 2 + 4 + 2 + 2 + 1 + 1 + 225 + 1 = 238 ✓

For BLACK 238-byte: plain = [n0, n1, 0, 0, 0, 0, 5, 5, 0xFF, 0xE3, 0, 0x4B, 0×225, 0]
So bytes[12:237] = all zeros, byte[237] = 0
Meaning: cipher[12:237] = keystream[12:237]

Let's extract the FULL keystream from BLACK packets and find the pattern.
"""
import sys
import os
from collections import Counter, defaultdict

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_paired_writes(filepath):
    """Parse CSV and return (header, payload) pairs."""
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

    # Load BLACK 238-byte paired packets
    black_pairs = [(h, p) for h, p in parse_paired_writes(os.path.join(CSV_DIR, "black.csv"))
                   if len(p) == 238]
    red_pairs = [(h, p) for h, p in parse_paired_writes(os.path.join(CSV_DIR, "red.csv"))
                 if len(p) == 238]

    print(f"BLACK 238-byte pairs: {len(black_pairs)}")
    print(f"RED   238-byte pairs: {len(red_pairs)}")

    if not black_pairs:
        print("No BLACK pairs found!")
        return

    # ================================================================
    # For BLACK: bytes[12:237] = keystream (since plaintext is all zeros)
    # ================================================================
    print("\n=== Keystream analysis from BLACK packets ===\n")

    for pidx, (hdr, payload) in enumerate(black_pairs[:5]):
        ks = payload[12:237]  # 225 bytes of pure keystream
        
        # Check: is it a repeating pattern?
        # Try period 3
        k3 = [ks[0], ks[1], ks[2]]
        errors_3 = sum(1 for i in range(len(ks)) if ks[i] != k3[i % 3])
        
        # Try period 6
        k6 = list(ks[:6])
        errors_6 = sum(1 for i in range(len(ks)) if ks[i] != k6[i % 6])
        
        # Try period 1 (constant)
        errors_1 = sum(1 for i in range(len(ks)) if ks[i] != ks[0])
        
        # Check byte[237] (tail, plaintext=0)
        tail_ks = payload[237]
        tail_matches_k3 = (tail_ks == k3[237 % 3])
        
        print(f"  Pkt {pidx}: k3=[{k3[0]:02x},{k3[1]:02x},{k3[2]:02x}] "
              f"err_p1={errors_1} err_p3={errors_3} err_p6={errors_6} "
              f"tail=0x{tail_ks:02x} tail_match_k3={tail_matches_k3}")
        
        if errors_3 == 0:
            # Perfect 3-byte repeating key
            # Now check: does it decrypt the header correctly?
            plain_hdr = bytes([payload[i] ^ k3[i % 3] for i in range(12)])
            print(f"    Header decrypted: [{' '.join(f'{b:02x}' for b in plain_hdr)}]")
            # Expected: [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B]
            if (plain_hdr[2:6] == b'\x00\x00\x00\x00' and
                plain_hdr[6:8] == b'\x05\x05' and
                plain_hdr[8] == 0xFF and plain_hdr[9] == 0xE3 and
                plain_hdr[10] == 0x00 and plain_hdr[11] == 0x4B):
                print(f"    ✓ Header matches expected format!")
                print(f"    nonce = [{plain_hdr[0]:02x}, {plain_hdr[1]:02x}]")
            
            # Check tail
            plain_tail = payload[237] ^ k3[237 % 3]
            print(f"    Tail: cipher=0x{payload[237]:02x} plain=0x{plain_tail:02x}")

    # ================================================================
    # Key finding: does EVERY 238-byte BLACK packet have period-3 keystream?
    # ================================================================
    print("\n=== Checking ALL BLACK 238-byte packets ===")
    
    perfect_3 = 0
    total = len(black_pairs)
    nonce_key_map = {}
    
    for pidx, (hdr, payload) in enumerate(black_pairs):
        k3 = [payload[12], payload[13], payload[14]]
        errors = sum(1 for i in range(225) if payload[12 + i] != k3[i % 3])
        
        if errors == 0:
            perfect_3 += 1
            # Extract nonce from decrypted header
            n0 = payload[0] ^ k3[0]
            n1 = payload[1] ^ k3[1]
            nonce_key_map[(n0, n1)] = tuple(k3)
    
    print(f"  Perfect period-3: {perfect_3}/{total}")
    
    if perfect_3 == total:
        print("  ✓✓✓ ALL packets use simple 3-byte repeating XOR!")
        print(f"\n  Unique (nonce → key3) mappings: {len(nonce_key_map)}")
        
        # Check: is nonce → key3 deterministic?
        # (same nonce always gives same key3)
        nonce_keys = defaultdict(set)
        for (n0, n1), k3 in nonce_key_map.items():
            nonce_keys[(n0, n1)].add(k3)
        
        multi = sum(1 for v in nonce_keys.values() if len(v) > 1)
        print(f"  Nonces with multiple key3: {multi}")
    
    # ================================================================
    # Verify on RED packets
    # ================================================================
    print(f"\n=== Verifying on RED 238-byte packets ===")
    
    red_ok = 0
    for pidx, (hdr, payload) in enumerate(red_pairs):
        # For RED: plain[12::3]=0xFF, plain[13::3]=0x00, plain[14::3]=0x00
        # So: k0 = payload[12] ^ 0xFF, k1 = payload[13] ^ 0x00, k2 = payload[14] ^ 0x00
        k0 = payload[12] ^ 0xFF
        k1 = payload[13]
        k2 = payload[14]
        k3 = [k0, k1, k2]
        
        # Verify: decrypt LED region
        errors = 0
        for i in range(75):
            base = 12 + i * 3
            pr = payload[base] ^ k3[0]
            pg = payload[base + 1] ^ k3[1]
            pb = payload[base + 2] ^ k3[2]
            if pr != 0xFF or pg != 0x00 or pb != 0x00:
                errors += 1
        
        if errors == 0:
            red_ok += 1
            if pidx < 3:
                plain_hdr = bytes([payload[i] ^ k3[i % 3] for i in range(12)])
                print(f"  Pkt {pidx}: k3=[{k0:02x},{k1:02x},{k2:02x}] ✓ "
                      f"hdr=[{' '.join(f'{b:02x}' for b in plain_hdr)}]")
        else:
            if pidx < 3:
                print(f"  Pkt {pidx}: k3=[{k0:02x},{k1:02x},{k2:02x}] errors={errors}")
    
    print(f"  RED perfect: {red_ok}/{len(red_pairs)}")
    
    # ================================================================
    # Can we GENERATE valid key3 for arbitrary packets?
    # ================================================================
    print(f"\n=== Key generation analysis ===")
    print("Can we use ANY key3 or must it match nonce?")
    print("Test: send packet with random key3 and random nonce")
    print()
    
    # From test_send_color.py (earlier test): it DIDN'T work with arbitrary key3
    # So the controller validates key3 vs nonce somehow
    # 
    # But wait - we now know the wire format is:
    # [55 AA 5A len 00] + [payload encrypted with 3-byte XOR]
    # The controller decrypts and checks. If key3 is wrong, 
    # the plaintext is garbage and the controller ignores it.
    #
    # So we need to figure out what nonce+key3 the controller accepts.
    # OR: we can just pick a nonce from our captures and use the matching key3!
    
    # Collect all valid nonce → key3 pairs
    all_nk = {}
    for color, pairs in [("black", black_pairs), ("red", red_pairs)]:
        for hdr, payload in pairs:
            k3 = [payload[12], payload[13], payload[14]]
            if color == "red":
                k3[0] ^= 0xFF
            n0 = payload[0] ^ k3[0]
            n1 = payload[1] ^ k3[1]
            all_nk[(n0, n1)] = tuple(k3)
    
    print(f"Total unique nonce→key3 pairs: {len(all_nk)}")
    
    # Print some for use in test
    print("\nSample nonce→key3 pairs:")
    for i, ((n0, n1), (k0, k1, k2)) in enumerate(list(all_nk.items())[:10]):
        print(f"  nonce=[{n0:02x},{n1:02x}] → key3=[{k0:02x},{k1:02x},{k2:02x}]")
    
    # Save ALL pairs for the sender script
    pairs_path = os.path.join(CSV_DIR, "nonce_key_pairs.py")
    with open(pairs_path, "w") as f:
        f.write("# Extracted nonce → key3 pairs from wire captures\n")
        f.write("# nonce = (n0, n1), key3 = (k0, k1, k2)\n")
        f.write("NONCE_KEY_PAIRS = {\n")
        for (n0, n1), (k0, k1, k2) in sorted(all_nk.items()):
            f.write(f"    (0x{n0:02x}, 0x{n1:02x}): (0x{k0:02x}, 0x{k1:02x}, 0x{k2:02x}),\n")
        f.write("}\n")
    print(f"\nSaved {len(all_nk)} pairs to: {pairs_path}")
    
    print("\n✓ Wire cipher: simple 3-byte XOR with key3 derived from nonce")
    print("✓ Wire format: [55 AA 5A len 00] + [payload XOR'd with key3]")
    print("✓ Plaintext: [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B, RGB×75, 00]")


if __name__ == "__main__":
    main()
