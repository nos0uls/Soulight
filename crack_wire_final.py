# -*- coding: utf-8 -*-
"""
crack_wire_final.py - Crack the .NET wire cipher by deep keystream analysis.

Key data from crack_wire_v2.py:
  - BLACK 238-byte: plaintext LED region = all 0x00
  - So ciphertext[12:237] = keystream[12:237]
  - Keystream is NOT simple 3-byte XOR
  - Keystream evolves: first triplet differs, then stabilizes on a pattern
  - byte[1] of key is CONSTANT across triplets
  - byte[0] and byte[2] change in early triplets then lock

  Header: [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B]
  byte[1]=0x32 CONSTANT in ciphertext -> cipher[1] = plain[1] XOR ks[1]
  
Strategy:
  1. Extract FULL keystream from each BLACK packet
  2. Look for relationship between keystream and DLL runtime table
  3. Look for relationship between header bytes and keystream
  4. Check if keystream = TABLE-based cipher with known parameters
"""
import sys
import os
from collections import Counter
from cipher_table_runtime import CIPHER_TABLE

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
TABLE = CIPHER_TABLE
TABLE_LEN = len(TABLE)  # 521


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
    print(f"BLACK 238-byte pairs: {len(black_pairs)}")

    # Known plaintext for BLACK 238-byte:
    # [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B, 00*225, 00]
    KNOWN_PLAIN = bytearray(238)
    KNOWN_PLAIN[6] = 0x05
    KNOWN_PLAIN[7] = 0x05
    KNOWN_PLAIN[8] = 0xFF
    KNOWN_PLAIN[9] = 0xE3
    KNOWN_PLAIN[10] = 0x00
    KNOWN_PLAIN[11] = 0x4B
    # bytes 0,1 = nonce (unknown per packet)
    # bytes 2-5 = 0
    # bytes 12-236 = 0 (LED data)
    # byte 237 = 0 (tail)

    # Extract keystream for known positions (2-11, 12-237)
    print("\n=== Full keystream extraction ===\n")
    
    all_keystreams = []
    for pidx, (hdr, payload) in enumerate(black_pairs):
        ks = bytearray(238)
        # Positions 2-11: known plaintext, so ks = cipher XOR plain
        for i in range(2, 12):
            ks[i] = payload[i] ^ KNOWN_PLAIN[i]
        # Positions 12-237: plain=0, so ks = cipher
        for i in range(12, 238):
            ks[i] = payload[i]
        # Positions 0-1: nonce, unknown -> ks unknown
        # But ks[0] XOR n0 = cipher[0], ks[1] XOR n1 = cipher[1]
        # cipher[1] = 0x32 always. So n1 XOR ks[1] = 0x32
        # We know ks[1] from position 1... but we don't know it directly
        # However: ks should be continuous. ks[1] should follow pattern from ks[2:]
        
        all_keystreams.append(ks)
    
    # Analyze keystream at known positions (2-237)
    # Check: does the DLL TABLE appear in the keystream?
    ks0 = all_keystreams[0]
    
    print("Pkt 0 keystream[2:20]:", ' '.join(f'{ks0[i]:02x}' for i in range(2, 20)))
    print("DLL TABLE[0:18]:     ", ' '.join(f'{TABLE[i]:02x}' for i in range(18)))
    
    # XOR keystream with TABLE to see if there's a simple relationship
    print("\nks[2:20] XOR TABLE[0:18]:", 
          ' '.join(f'{ks0[i+2]^TABLE[i]:02x}' for i in range(18)))
    
    # Maybe keystream = TABLE[offset:] XOR something?
    # Try: for each possible table offset, XOR ks with TABLE and check pattern
    print("\n=== Searching for TABLE offset ===")
    
    best_offset = -1
    best_score = 0
    
    for offset in range(TABLE_LEN):
        # XOR ks[12:237] with TABLE[offset:offset+225] (wrapping)
        xor_result = bytearray(225)
        for i in range(225):
            t_idx = (offset + i) % TABLE_LEN
            xor_result[i] = ks0[12 + i] ^ TABLE[t_idx]
        
        # Check if xor_result has a simple pattern (constant, period-3, etc)
        # Check period-3
        if len(set(xor_result[::3])) <= 3 and len(set(xor_result[1::3])) <= 3:
            k0_vals = set(xor_result[::3])
            k1_vals = set(xor_result[1::3])
            k2_vals = set(xor_result[2::3])
            total_unique = len(k0_vals) + len(k1_vals) + len(k2_vals)
            if total_unique < best_score or best_offset == -1:
                best_score = total_unique
                best_offset = offset
                if total_unique <= 6:
                    print(f"  offset={offset}: k0_vals={k0_vals} k1_vals={k1_vals} k2_vals={k2_vals}")
        
        # Check constant
        if len(set(xor_result)) <= 5:
            print(f"  offset={offset}: NEAR-CONSTANT! unique={len(set(xor_result))} "
                  f"vals={set(list(xor_result)[:10])}")
    
    # Different approach: XOR two different packets' keystreams
    # If they use the same table but different IVs, the XOR should reveal the IV difference
    print("\n=== XOR of two keystreams ===")
    ks1 = all_keystreams[1]
    
    xor_ks = bytes(ks0[i] ^ ks1[i] for i in range(2, 238))
    print(f"KS0 XOR KS1 [0:30]: {' '.join(f'{b:02x}' for b in xor_ks[:30])}")
    
    # Check period of XOR
    for period in [1, 2, 3, 6, 12]:
        matches = sum(1 for i in range(len(xor_ks) - period) if xor_ks[i] == xor_ks[i + period])
        total = len(xor_ks) - period
        print(f"  period {period:2d}: {matches}/{total} matches ({matches/total*100:.1f}%)")
    
    # Maybe the cipher is: ks[i] = TABLE[(start + i) % TABLE_LEN] XOR IV_byte
    # Where IV_byte rotates through a 6-byte IV
    # This is exactly the DLL cipher! But with different parameters.
    
    # Let's try: for each possible start_offset and 6-byte IV,
    # check if ks = TABLE[start+i] XOR IV[i%6]
    # We can determine IV[i%6] from the first 6 ks bytes:
    # IV[i%6] = ks[i] XOR TABLE[(start+i) % TABLE_LEN]
    
    print("\n=== Trying DLL cipher model: ks[i] = TABLE[(start+i)%521] XOR IV[i%6] ===")
    print("(without bit-rotation of IV)\n")
    
    ks_known = bytes(ks0[12:237])  # 225 bytes of known keystream
    
    best_matches = 0
    best_start = 0
    
    for start in range(TABLE_LEN):
        # Derive IV from first 6 bytes
        iv = bytearray(6)
        for j in range(6):
            iv[j] = ks_known[j] ^ TABLE[(start + j) % TABLE_LEN]
        
        # Check if this IV predicts the rest
        matches = 0
        for i in range(6, 225):
            predicted = TABLE[(start + i) % TABLE_LEN] ^ iv[i % 6]
            if predicted == ks_known[i]:
                matches += 1
        
        if matches > best_matches:
            best_matches = matches
            best_start = start
            if matches > 200:
                print(f"  start={start}: IV={iv.hex()} matches={matches}/219")
    
    print(f"\n  Best: start={best_start} matches={best_matches}/219")
    
    # Try WITH bit-rotation
    print("\n=== Trying DLL cipher model WITH IV bit-rotation ===\n")
    
    def rotate_iv_left_1bit(iv6):
        val = int.from_bytes(iv6, 'little')
        val = ((val << 1) | (val >> 47)) & ((1 << 48) - 1)
        return val.to_bytes(6, 'little')
    
    best_matches_rot = 0
    best_start_rot = 0
    
    for start in range(TABLE_LEN):
        # Derive initial IV from first 6 bytes
        iv = bytearray(6)
        for j in range(6):
            iv[j] = ks_known[j] ^ TABLE[(start + j) % TABLE_LEN]
        
        # Now simulate with rotation
        cur_iv = bytearray(iv)
        cur_iv = bytearray(rotate_iv_left_1bit(bytes(cur_iv)))  # rotate after first 6
        
        matches = 0
        iv_idx = 0
        for i in range(6, 225):
            predicted = TABLE[(start + i) % TABLE_LEN] ^ cur_iv[iv_idx]
            if predicted == ks_known[i]:
                matches += 1
            iv_idx += 1
            if iv_idx >= 6:
                cur_iv = bytearray(rotate_iv_left_1bit(bytes(cur_iv)))
                iv_idx = 0
        
        if matches > best_matches_rot:
            best_matches_rot = matches
            best_start_rot = start
            if matches > 200:
                print(f"  start={start}: IV={iv.hex()} matches={matches}/219")
    
    print(f"\n  Best with rotation: start={best_start_rot} matches={best_matches_rot}/219")
    
    # Try: maybe the start offset is derived from the header/nonce bytes
    # Or maybe the table is used differently
    
    # Let's also check: what if the wire cipher uses the SAME table but
    # starts at a different offset for each packet?
    # The offset could be derived from bytes[0..5] of the payload
    
    print("\n=== Checking ks as raw TABLE slice (no IV) ===")
    for start in range(TABLE_LEN):
        matches = sum(1 for i in range(225) 
                      if TABLE[(start + i) % TABLE_LEN] == ks_known[i])
        if matches > 100:
            print(f"  start={start}: matches={matches}/225")
    
    # Maybe the table bytes are XOR'd with a single byte?
    print("\n=== TABLE[i] XOR const = ks[i]? ===")
    for start in range(TABLE_LEN):
        # The constant would be ks_known[0] XOR TABLE[start]
        const = ks_known[0] ^ TABLE[start]
        matches = sum(1 for i in range(225)
                      if (TABLE[(start + i) % TABLE_LEN] ^ const) == ks_known[i])
        if matches > 150:
            print(f"  start={start} const=0x{const:02x}: matches={matches}/225")

    print("\nDONE")


if __name__ == "__main__":
    main()
