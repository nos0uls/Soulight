# -*- coding: utf-8 -*-
"""
crack_key_final2.py - Final attempt to crack the wire cipher.

KEY FINDINGS SO FAR:
  1. Wire cipher = XOR with evolving keystream
  2. Keystream has 3 channels (pos mod 3 = 0, 1, 2)
  3. Channel 1 is mostly constant per packet (varies between packets)
  4. Channels 0 and 2 evolve: first value differs, then stabilizes
  5. Evolution ch0: [00, E6, F3, F3, F3, ...] - stabilizes at F3 after position 2
  6. Evolution ch1: [00, 00, 00, ...] - constant
  7. Evolution ch2: [00, 37, 37, 37, ...] - stabilizes at 37 after position 1
  8. KS_A XOR KS_B has perfect period-3 -> base keys differ, evolution shared

INSIGHT: Evolution pattern [00, E6, F3] then F3 forever.
  E6 = 0xE6, F3 = 0xF3. What's 0xE6 XOR 0xF3 = 0x15.
  And ch0[0] = 0xE2. ch0[1] = 0x04 = 0xE2 XOR 0xE6. ch0[2] = 0x11 = 0xE2 XOR 0xF3.
  
So: ks_ch0[i] = base0 XOR ev0[i]
  where ev0 = [0x00, 0xE6, 0xF3, 0xF3, ...]

But ev0 is NOT the same for all packets (pkt 2+ diverge).
Pkts 0 and 1 have same evolution. Pkts 2+ different.

What differs between pkt 0/1 and pkt 2+? Let's check the HEADER bytes.
Maybe the "nonce" in header determines the evolution pattern.

Actually - looking more carefully: evolution for ch2 in pkt 2 has match=1/75.
That means a COMPLETELY different evolution. Not just shifted.

Let me reconsider. Maybe the cipher involves the runtime TABLE after all,
but used differently than I tested. Or maybe the nonce bytes are indices 
into the table.

NEW APPROACH: forget about TABLE. Look at the raw data.
For each BLACK packet, I have the full keystream.
Let me see if adjacent packets share keystream structure.
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


def extract_ks_238(payload):
    """Extract keystream from BLACK 238-byte payload."""
    PLAIN_HDR = [0, 0, 0, 0, 0, 0, 0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B]
    ks = bytearray(238)
    for i in range(2, 12):
        ks[i] = payload[i] ^ PLAIN_HDR[i]
    for i in range(12, 238):
        ks[i] = payload[i]
    return ks


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    all_pairs = parse_paired_writes(os.path.join(CSV_DIR, "black.csv"))
    # Get ALL pairs, not just 238-byte
    print(f"All BLACK pairs: {len(all_pairs)}")
    
    # Group by payload length
    by_len = {}
    for hdr, payload in all_pairs:
        plen = len(payload)
        if plen not in by_len:
            by_len[plen] = []
        by_len[plen].append((hdr, payload))
    
    print("By length:")
    for plen in sorted(by_len.keys()):
        print(f"  len={plen}: {len(by_len[plen])} packets")
    
    # Focus on 238-byte packets first
    pairs_238 = by_len.get(238, [])
    if not pairs_238:
        print("No 238-byte packets!")
        return
    
    # ================================================================
    # Extract nonce + key for each packet  
    # ================================================================
    print(f"\n=== Nonce and key analysis for 238-byte packets ===\n")
    
    entries = []
    for pidx, (hdr, payload) in enumerate(pairs_238):
        ks = extract_ks_238(payload)
        
        # The nonce: plaintext[0] = n0, plaintext[1] = n1
        # cipher[0] = n0 XOR ks[0], cipher[1] = n1 XOR ks[1]
        # We know ks[2:12], but ks[0] and ks[1] are unknown
        # However, ks should be period-3 at positions 0,1 too
        # So ks[0] should match ks[3] (if period-3 holds)
        # ks[3] = cipher[3] XOR plain[3] = cipher[3] XOR 0 = cipher[3]
        # Wait: ks[3] is cipher[3] since plain[3]=0? No - plain[3]=PLAIN_HDR[3]=0.
        # ks[3] = payload[3] ^ 0 = payload[3]. Yes.
        
        # Similarly ks[0] could be ks[3] or ks[6] or ks[9]...
        # But evolution shows ks[0] != ks[3] (ch0 evolves)!
        # ks[0] = base, ks[3] = base XOR ev0[1], ks[6] = base XOR ev0[2]...
        
        # For the stable part: ks[12+3*j] for j>=2 is constant per channel
        # So the "stable key" is: sk0 = ks[18], sk1 = ks[13], sk2 = ks[14]
        # (position 18 = 12 + 2*3, should be stable)
        
        sk0 = payload[18]  # stable ch0 key (pos 18, mod=0)
        sk1 = payload[13]  # stable ch1 key (pos 13, mod=1) 
        sk2 = payload[14]  # stable ch2 key (pos 14, mod=2)
        
        # First triplet: ks at positions 12,13,14
        fk0 = payload[12]
        fk1 = payload[13]
        fk2 = payload[14]
        
        # Nonce reconstruction: 
        # n0 = cipher[0] XOR ks[0]
        # We can estimate ks[0] from the pattern
        # But we know ks[3] = payload[3] (since plain[3]=0)
        # And ks[6] = payload[6] ^ 0x05
        # And ks[9] = payload[9] ^ 0xE3
        
        ks0_est = payload[3]   # ks at pos 3 (mod 0) 
        ks1_est = payload[4]   # ks at pos 4 (mod 1)
        ks2_est = payload[2]   # ks at pos 2 (mod 2) - plain[2]=0
        
        n0_est = payload[0] ^ ks0_est  # This won't be right if evolution[1] != 0
        n1_est = payload[1] ^ ks1_est
        
        # Actually let's use the stable key directly:
        # At position 0 (mod 0): ks[0] = sk0 XOR ev0[0] 
        # But ev0[0] = 0 by definition (evolution relative to position 0)
        # Hmm, that means ks[0] = original key before evolution
        # But our "evolution" was relative to LED position 12, not position 0
        
        # Let me reconsider the evolution across ALL positions mod 0:
        # positions: 0, 3, 6, 9, 12, 15, 18, 21, ...
        # ks values: ?, p[3], p[6]^5, p[9]^E3, p[12], p[15], p[18], p[21]...
        
        ch0_all = []
        ch1_all = []  
        ch2_all = []
        
        # Position 0: unknown
        # Position 1: unknown
        # Position 2: ks[2] = payload[2] ^ 0 = payload[2]
        ch2_all.append((2, payload[2]))
        
        # Position 3: ks[3] = payload[3] ^ 0 = payload[3]
        ch0_all.append((3, payload[3]))
        
        # Position 4: ks[4] = payload[4] ^ 0 = payload[4]  
        ch1_all.append((4, payload[4]))
        
        # Position 5: ks[5] = payload[5] ^ 0 = payload[5]
        ch2_all.append((5, payload[5]))
        
        # Position 6: ks[6] = payload[6] ^ 0x05
        ch0_all.append((6, payload[6] ^ 0x05))
        
        # Position 7: ks[7] = payload[7] ^ 0x05
        ch1_all.append((7, payload[7] ^ 0x05))
        
        # Position 8: ks[8] = payload[8] ^ 0xFF
        ch2_all.append((8, payload[8] ^ 0xFF))
        
        # Position 9: ks[9] = payload[9] ^ 0xE3
        ch0_all.append((9, payload[9] ^ 0xE3))
        
        # Position 10: ks[10] = payload[10] ^ 0x00
        ch1_all.append((10, payload[10]))
        
        # Position 11: ks[11] = payload[11] ^ 0x4B
        ch2_all.append((11, payload[11] ^ 0x4B))
        
        # Position 12+: ks = payload (plain=0)
        for i in range(12, 30):
            if i % 3 == 0:
                ch0_all.append((i, payload[i]))
            elif i % 3 == 1:
                ch1_all.append((i, payload[i]))
            else:
                ch2_all.append((i, payload[i]))
        
        if pidx < 5:
            print(f"Pkt {pidx}:")
            print(f"  cipher[0:6] = {' '.join(f'{payload[i]:02x}' for i in range(6))}")
            print(f"  ch0 (mod0): {' '.join(f'[{p}]={v:02x}' for p,v in ch0_all)}")
            print(f"  ch1 (mod1): {' '.join(f'[{p}]={v:02x}' for p,v in ch1_all)}")
            print(f"  ch2 (mod2): {' '.join(f'[{p}]={v:02x}' for p,v in ch2_all)}")
        
        entries.append({
            'cipher': payload,
            'ch0': ch0_all,
            'ch1': ch1_all,
            'ch2': ch2_all,
            'sk': (sk0, sk1, sk2),
            'fk': (fk0, fk1, fk2),
        })
    
    # ================================================================
    # Analyze evolution patterns more carefully
    # ================================================================
    print(f"\n=== Evolution patterns per channel ===\n")
    
    # For each packet, show the ch0 sequence
    for pidx in range(min(10, len(entries))):
        e = entries[pidx]
        ch0_vals = [v for _, v in e['ch0']]
        # Show as deltas from stable value
        stable = ch0_vals[-1]  # last value should be stable
        deltas = [v ^ stable for v in ch0_vals]
        print(f"  Pkt {pidx}: ch0=[{' '.join(f'{v:02x}' for v in ch0_vals[:8])}] "
              f"stable=0x{stable:02x} deltas=[{' '.join(f'{d:02x}' for d in deltas[:8])}]")
    
    print()
    for pidx in range(min(10, len(entries))):
        e = entries[pidx]
        ch2_vals = [v for _, v in e['ch2']]
        stable = ch2_vals[-1]
        deltas = [v ^ stable for v in ch2_vals]
        print(f"  Pkt {pidx}: ch2=[{' '.join(f'{v:02x}' for v in ch2_vals[:8])}] "
              f"stable=0x{stable:02x} deltas=[{' '.join(f'{d:02x}' for d in deltas[:8])}]")
    
    # ================================================================
    # Check: is the delta pattern the same for all packets?
    # ================================================================
    print(f"\n=== Are delta patterns consistent? ===\n")
    
    all_ch0_deltas = []
    all_ch2_deltas = []
    for e in entries:
        ch0_vals = [v for _, v in e['ch0']]
        ch2_vals = [v for _, v in e['ch2']]
        stable0 = ch0_vals[-1]
        stable2 = ch2_vals[-1]
        all_ch0_deltas.append(tuple(v ^ stable0 for v in ch0_vals[:6]))
        all_ch2_deltas.append(tuple(v ^ stable2 for v in ch2_vals[:6]))
    
    print(f"  Unique ch0 delta patterns: {len(set(all_ch0_deltas))}")
    for pattern in sorted(set(all_ch0_deltas)):
        count = all_ch0_deltas.count(pattern)
        print(f"    {' '.join(f'{d:02x}' for d in pattern)}: {count} packets")
    
    print(f"\n  Unique ch2 delta patterns: {len(set(all_ch2_deltas))}")
    for pattern in sorted(set(all_ch2_deltas)):
        count = all_ch2_deltas.count(pattern)
        print(f"    {' '.join(f'{d:02x}' for d in pattern)}: {count} packets")
    
    # ================================================================
    # The stable key IS the real key. The initial instability is
    # because positions 0-11 have different plaintext.
    # WAIT - positions 3,4,5 have plaintext 0! So ks[3,4,5] = cipher[3,4,5]
    # And positions 12,13,14 also have plaintext 0.
    # If the cipher is pure 3-byte XOR, ks[3] should equal ks[12].
    # But ks[3] != ks[12] in many packets!
    # UNLESS... my assumed plaintext is WRONG.
    # ================================================================
    print(f"\n=== CRITICAL CHECK: Is our plaintext assumption correct? ===")
    print("Checking: cipher[3] vs cipher[12] (both should be ks at mod-0 positions)")
    print("If plaintext[3]=0 and plaintext[12]=0, and cipher is 3-byte XOR,")
    print("then cipher[3] should equal cipher[12].\n")
    
    for pidx in range(10):
        p = entries[pidx]['cipher']
        print(f"  Pkt {pidx}: c[3]=0x{p[3]:02x} c[12]=0x{p[12]:02x} "
              f"c[6]=0x{p[6]:02x} c[15]=0x{p[15]:02x} c[18]=0x{p[18]:02x} "
              f"same_3_12={'Y' if p[3]==p[12] else 'N'} "
              f"same_12_15={'Y' if p[12]==p[15] else 'N'} "
              f"same_15_18={'Y' if p[15]==p[18] else 'N'}")
    
    # If c[3] != c[12] but c[15] == c[18], then plaintext[3] might NOT be 0!
    # Or the key changes between position 3 and 12.
    
    # Let me check what c[3] XOR c[12] is across packets
    print(f"\n  c[3] XOR c[12] values:")
    xor_3_12 = set()
    for e in entries:
        p = e['cipher']
        xor_3_12.add(p[3] ^ p[12])
    print(f"    Unique: {len(xor_3_12)} values: {[f'{v:02x}' for v in sorted(xor_3_12)]}")
    
    # If this is constant -> plaintext[3] is constant but not 0
    if len(xor_3_12) == 1:
        val = list(xor_3_12)[0]
        print(f"\n    *** c[3] XOR c[12] is CONSTANT = 0x{val:02x} ***")
        print(f"    This means plaintext[3] = 0x{val:02x} (not 0 as we assumed!)")
        print(f"    Or the key evolves by exactly 0x{val:02x} between pos 3 and 12")


if __name__ == "__main__":
    main()
