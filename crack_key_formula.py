# -*- coding: utf-8 -*-
"""
crack_key_formula.py - Find the formula: key3 = f(nonce0, nonce1)

CONFIRMED:
  - Wire cipher = pure 3-byte XOR, key changes per packet
  - XOR of two keystreams = perfect period-3
  - Plaintext: [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B, RGB*75, 00]
  - Keystream at position 2: ks[2] = cipher[2] ^ 0x00 = cipher[2]
  - For BLACK: ks[12+i] = cipher[12+i] (since LED=0)

Wait - but crack_wire_v2 showed the keystream is NOT perfect period-3!
  err_p3=173 out of 225! That's 77% errors!
  
But KS0 XOR KS1 IS perfect period-3!

This means: the keystream has period-3 MODULAR STRUCTURE but the actual
key bytes vary. Each packet's keystream[i] depends on (i mod 3) in a
consistent way across packets.

Let me re-examine. From crack_wire_v2.py:
  LED keystream triplets:
    [0] e2 14 b5  (unique)
    [1] 04 14 82  (transition)  
    [2] 11 14 82  (stabilizes)
    [3+] 11 14 82 (repeats)

byte[1] (0x14) is CONSTANT. byte[0] goes e2->04->11->11.
byte[2] goes b5->82->82.

So the "key" is NOT constant across the packet - it EVOLVES.
But the XOR of two packets' keystreams IS constant period-3.

This means: KS_A[i] = base_key_A[i%3] + evolution[i]
KS_B[i] = base_key_B[i%3] + evolution[i]  (same evolution)
KS_A XOR KS_B = base_key_A XOR base_key_B = constant period-3

So there's a SHARED evolution function that's the same for all packets,
plus a per-packet 3-byte base key derived from the nonce.

If I can find the evolution function, I can construct any keystream!
"""
import sys
import os
from cipher_table_runtime import CIPHER_TABLE

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
TABLE = CIPHER_TABLE


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
    red_pairs = [(h, p) for h, p in parse_paired_writes(os.path.join(CSV_DIR, "red.csv"))
                 if len(p) == 238]
    
    print(f"BLACK 238-byte: {len(black_pairs)}")
    print(f"RED   238-byte: {len(red_pairs)}")

    # ================================================================
    # Extract per-channel keystreams
    # ================================================================
    # For BLACK: cipher[i] = ks[i] for i >= 2 (known plain = 0 except nonce)
    # ks[i] = base_key[i%3] XOR evolution[i]
    #
    # To isolate evolution: take one packet, extract ks, then
    # evolution[i] = ks[i] XOR ks[i%3] ... no that's circular.
    #
    # Better: if we XOR all ks values at same (i%3) position,
    # we get the evolution for that channel.
    
    # Take packet 0
    pkt0 = black_pairs[0][1]
    
    # Full keystream (positions 2-237, using known plaintext)
    PLAIN_HEADER = bytes([0, 0, 0, 0, 0, 0, 0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B])
    
    ks0 = bytearray(238)
    for i in range(2, 12):
        ks0[i] = pkt0[i] ^ PLAIN_HEADER[i]
    for i in range(12, 238):
        ks0[i] = pkt0[i]  # plain = 0
    
    # Extract 3 channels
    ch0 = [ks0[i] for i in range(12, 237, 3)]  # positions 12,15,18,...
    ch1 = [ks0[i] for i in range(13, 237, 3)]  # positions 13,16,19,...
    ch2 = [ks0[i] for i in range(14, 237, 3)]  # positions 14,17,20,...
    
    print(f"\nPkt 0 channel 0 (first 15): {' '.join(f'{v:02x}' for v in ch0[:15])}")
    print(f"Pkt 0 channel 1 (first 15): {' '.join(f'{v:02x}' for v in ch1[:15])}")
    print(f"Pkt 0 channel 2 (first 15): {' '.join(f'{v:02x}' for v in ch2[:15])}")
    
    # Channel 1 should be constant (0x14 from earlier analysis)
    ch1_unique = set(ch1)
    print(f"\nChannel 1 unique values: {len(ch1_unique)} -> {[f'{v:02x}' for v in sorted(ch1_unique)]}")
    
    # Extract evolution by XOR-ing each channel with its first value
    ev0 = [v ^ ch0[0] for v in ch0]
    ev1 = [v ^ ch1[0] for v in ch1]
    ev2 = [v ^ ch2[0] for v in ch2]
    
    print(f"\nEvolution ch0 (first 15): {' '.join(f'{v:02x}' for v in ev0[:15])}")
    print(f"Evolution ch1 (first 15): {' '.join(f'{v:02x}' for v in ev1[:15])}")
    print(f"Evolution ch2 (first 15): {' '.join(f'{v:02x}' for v in ev2[:15])}")
    
    # Verify: is evolution the same across different packets?
    print(f"\n=== Verifying evolution consistency across packets ===")
    
    for pidx in range(1, min(5, len(black_pairs))):
        pkt = black_pairs[pidx][1]
        ks = bytearray(238)
        for i in range(2, 12):
            ks[i] = pkt[i] ^ PLAIN_HEADER[i]
        for i in range(12, 238):
            ks[i] = pkt[i]
        
        pch0 = [ks[i] for i in range(12, 237, 3)]
        pch1 = [ks[i] for i in range(13, 237, 3)]
        pch2 = [ks[i] for i in range(14, 237, 3)]
        
        pev0 = [v ^ pch0[0] for v in pch0]
        pev1 = [v ^ pch1[0] for v in pch1]
        pev2 = [v ^ pch2[0] for v in pch2]
        
        match0 = sum(1 for a, b in zip(ev0, pev0) if a == b)
        match1 = sum(1 for a, b in zip(ev1, pev1) if a == b)
        match2 = sum(1 for a, b in zip(ev2, pev2) if a == b)
        
        print(f"  Pkt {pidx}: ev0_match={match0}/75 ev1_match={match1}/75 ev2_match={match2}/75")
    
    # ================================================================
    # Check if evolution matches TABLE
    # ================================================================
    print(f"\n=== Does evolution come from TABLE? ===")
    
    # Evolution is: ev[j] = ks[12+j*3] XOR ks[12]
    # If ks[12+j*3] = TABLE_func(j) XOR base_key, then
    # ev[j] = TABLE_func(j) XOR TABLE_func(0)
    
    # Check: ev0[j] = TABLE[j*3] XOR TABLE[0]?
    for offset in range(TABLE_LEN):
        match = sum(1 for j in range(75) 
                    if ev0[j] == (TABLE[(offset + j*3) % TABLE_LEN] ^ TABLE[offset]))
        if match > 50:
            print(f"  ev0 vs TABLE[{offset}+j*3] XOR TABLE[{offset}]: {match}/75")
    
    # Or maybe ks[12+i] = TABLE[i] for the full sequence (not just every 3rd)
    # Check: does TABLE[0:225] match the full LED keystream?
    for offset in range(TABLE_LEN):
        match = 0
        for i in range(min(225, TABLE_LEN - offset)):
            if ks0[12 + i] == TABLE[offset + i]:
                match += 1
        if match > 100:
            print(f"  ks vs TABLE[{offset}:]: {match} matches")
    
    # ================================================================
    # What if evolution = TABLE values at positions 0,3,6,9... ?
    # ================================================================
    print(f"\n=== Check: ev_ch0 = TABLE[::3]? ===")
    for base in range(3):
        for offset in range(TABLE_LEN):
            table_ch = [TABLE[(offset + j) % TABLE_LEN] for j in range(0, 225, 3)]
            ev_xor = [a ^ b for a, b in zip(ev0, table_ch)]
            if len(set(ev_xor)) == 1:
                print(f"  base={base} offset={offset}: ev0 = TABLE[{offset}::3] XOR 0x{ev_xor[0]:02x}")
    
    # ================================================================
    # Direct approach: try ks[i] = TABLE[i % TABLE_LEN] XOR something_simple
    # ================================================================
    print(f"\n=== ks[i] = TABLE[i % 521] XOR constant? ===")
    for const in range(256):
        match = sum(1 for i in range(225) 
                    if ks0[12 + i] == (TABLE[i % TABLE_LEN] ^ const))
        if match > 100:
            print(f"  const=0x{const:02x}: {match}/225")
    
    # ================================================================  
    # Maybe different: ks[i] = TABLE[some_func(i)] XOR key
    # where some_func involves the nonce
    # ================================================================
    
    # Let me look at this from a completely different angle.
    # The keystream for 238-byte packets:
    # Position 2-11 (header after nonce): ks = cipher XOR known_plain
    # Let me check: is ks[2] == ks[5]? (same position mod 3)
    
    print(f"\n=== Keystream at header positions (known plaintext) ===")
    for pidx in range(5):
        pkt = black_pairs[pidx][1]
        ks = bytearray(12)
        for i in range(2, 12):
            ks[i] = pkt[i] ^ PLAIN_HEADER[i]
        
        # Also get LED ks
        led_ks = [pkt[12], pkt[13], pkt[14]]  # first LED triplet
        
        print(f"  Pkt {pidx}: ks[2:12]=[{' '.join(f'{ks[i]:02x}' for i in range(2,12))}] "
              f"led_ks[0:3]=[{' '.join(f'{v:02x}' for v in led_ks)}]")
        
        # Check: ks[2]==ks[5]==ks[8]==ks[11]? (mod 3 = 2)
        pos_mod0 = [ks[i] for i in [2, 5, 8, 11]]  # assuming ks starts at pos 0
        # Actually: position in stream: 2,3,4,5,6,7,8,9,10,11
        # mod 3: 2,0,1,2,0,1,2,0,1,2
        
        # Positions with same mod:
        mod0_pos = [3, 6, 9]  # stream pos mod 3 = 0
        mod1_pos = [4, 7, 10]  # mod 3 = 1  
        mod2_pos = [2, 5, 8, 11]  # mod 3 = 2
        
        for mod_val, positions in [(0, mod0_pos), (1, mod1_pos), (2, mod2_pos)]:
            vals = [ks[p] for p in positions]
            led_val = led_ks[mod_val]
            print(f"    mod{mod_val}: header_vals={[f'{v:02x}' for v in vals]} "
                  f"led[0]={led_val:02x} "
                  f"match_led={'Y' if all(v == led_val for v in vals) else 'N'}")

    # ================================================================
    # KEY INSIGHT: Check if header ks values evolve same as LED region
    # ================================================================
    print(f"\n=== Full position-by-position keystream (pkt 0) ===")
    pkt = black_pairs[0][1]
    
    # Reconstruct full ks using known plaintext
    # pos 0,1: unknown (nonce)
    # pos 2-11: known header
    # pos 12-237: known (all 0)
    
    full_ks = bytearray(238)
    for i in range(2, 12):
        full_ks[i] = pkt[i] ^ PLAIN_HEADER[i]
    for i in range(12, 238):
        full_ks[i] = pkt[i]
    
    # Print by mod-3 channel
    for ch in range(3):
        positions = list(range(2 + ((ch - 2) % 3), 238, 3))
        # Actually simpler: just do all positions and group by mod 3
        pass
    
    print("Position: ks_value (grouped by position mod 3)")
    for i in range(2, min(30, 238)):
        mod = i % 3
        print(f"  [{i:3d}] mod={mod} ks=0x{full_ks[i]:02x}")
    
    # Show evolution: for channel 0 (mod 0), list all values
    print(f"\nChannel 0 (pos mod 3 == 0): positions 3,6,9,12,15,18,21,24...")
    ch0_full = [(i, full_ks[i]) for i in range(3, 238, 3)]
    for i, v in ch0_full[:20]:
        print(f"  [{i:3d}] 0x{v:02x}")
    
    print(f"\nChannel 2 (pos mod 3 == 2): positions 2,5,8,11,14,17,20,23...")
    ch2_full = [(i, full_ks[i]) for i in range(2, 238, 3)]
    for i, v in ch2_full[:20]:
        print(f"  [{i:3d}] 0x{v:02x}")


if __name__ == "__main__":
    main()
