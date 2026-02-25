# -*- coding: utf-8 -*-
"""
reverse_cipher_v3.py — Reverse the cipher from captured packets.

Strategy: instead of guessing the format, use BLACK packets where plaintext LED data = 0x00.
If we can find a consistent table by XOR-ing known-plaintext with ciphertext, we can
reconstruct the actual keystream and from that deduce the table.

From our earlier crypto analysis (which WAS correct):
  - XOR cipher with 3-byte repeating key worked for LED region
  - But the REAL cipher is more complex (6-byte IV + table + rotate)
  - The 3-byte appearance was an artifact of how the rotation interacts with period

New approach:
  1. Take two BLACK 238-byte packets  
  2. XOR them together -> differences = IV-dependent keystream differences
  3. Since plaintext is the same for both, C1 ^ C2 = KS1 ^ KS2
  
  OR better: use the DLL function directly by finding a way to run 32-bit code.

Actually, let me think about this differently.

The cipher from FUN_10001880:
  - IV = 6 bytes, stored at pkt[5..10]  (PLAINTEXT, not encrypted)
  - Encryption: pkt[11..end] ^= TABLE[t] ^ IV_rotated[i%6]
  - TABLE cycles mod TABLE_LEN, IV rotates left by 1 bit every 6 bytes

But captured packets don't have magic 0x4D67 at [0..1].
Byte[1] IS constant per packet length though.

HYPOTHESIS: Maybe the DLL output is NOT what goes on the wire.
The .NET code may do additional framing/transformation.

From BeelightProtocol.cs we know:
  get_large_screendata_package(IntPtr pscreen, int row, int column, int channel, IntPtr pout)
  returns int (probably output length)
  
The .NET code then probably wraps this in a frame with checksum.

But wait - in the original crypto analysis, we found that a simple 3-byte XOR
decoded the ENTIRE packet (including what we thought was header) correctly.
The plaintext header was [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B].

That means: the ENTIRE 238 bytes are encrypted with the same 3-byte XOR.
There is NO plaintext header portion visible.

This contradicts FUN_10001880 which leaves bytes [0-10] unencrypted.

CONCLUSION: The function that creates screen data packets is DIFFERENT from FUN_10001880,
OR there's additional encryption applied by the .NET layer on top.

Let me check: does the packet format from our 3-byte XOR analysis match the DLL format?

From our analysis, plaintext byte[1] = n1 (varies per packet).
From DLL: plaintext byte[1] = 0x67 (constant magic).
These don't match -> the screen data packets use a DIFFERENT format.

So: get_large_screendata_package probably has its OWN packet builder,
not using FUN_10001880.
"""
import sys
import os
from collections import defaultdict

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_long_writes(filepath):
    pkts = []
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
            if len(raw) > 10:
                pkts.append(raw)
    return pkts


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    # Load BLACK 238-byte packets
    black_pkts = [p for p in parse_long_writes(os.path.join(CSV_DIR, "black.csv")) if len(p) == 238]
    red_pkts = [p for p in parse_long_writes(os.path.join(CSV_DIR, "red.csv")) if len(p) == 238]
    
    print(f"BLACK 238-byte: {len(black_pkts)}")
    print(f"RED   238-byte: {len(red_pkts)}")
    
    if not black_pkts or not red_pkts:
        print("Need both black and red packets")
        return
    
    # From earlier crypto analysis, we know:
    # For BLACK: plaintext = [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B, 
    #                         00,00,00 × 75, 00]
    # For RED:   plaintext = [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B,
    #                         FF,00,00 × 75, 00]
    
    # The 3-byte XOR key was extracted from the LED region where we know the plaintext.
    # Let's re-extract it and use it to decrypt the full packet, then compare
    # the decrypted header with the DLL format.
    
    print("\n=== Decrypting with 3-byte XOR (verified working) ===\n")
    
    for color, pkts, rgb in [("BLACK", black_pkts, (0,0,0)), ("RED", red_pkts, (0xFF,0,0))]:
        print(f"--- {color} ---")
        for idx, pkt in enumerate(pkts[:3]):
            r, g, b = rgb
            # Extract key from LED region (offset 12, repeating RGB triplet)
            # key3[0] = pkt[12] ^ R, key3[1] = pkt[13] ^ G, key3[2] = pkt[14] ^ B
            k0 = pkt[12] ^ r
            k1 = pkt[13] ^ g
            k2 = pkt[14] ^ b
            key3 = [k0, k1, k2]
            
            # Decrypt entire packet
            plain = bytes([pkt[i] ^ key3[i % 3] for i in range(len(pkt))])
            
            # Show header
            hdr = ' '.join(f'{x:02x}' for x in plain[:15])
            led_sample = ' '.join(f'{x:02x}' for x in plain[12:18])
            tail = f'{plain[-1]:02x}'
            
            print(f"  Pkt {idx}: key3=[{k0:02x},{k1:02x},{k2:02x}] "
                  f"hdr=[{hdr}] led=[{led_sample}] tail=[{tail}]")
            
            # Check: does the decrypted header match DLL format?
            # DLL: [0]=0x4D, [1]=0x67, [2]=0x4C, [3]=len_hi, [4]=len_lo
            if plain[0] == 0x4D and plain[1] == 0x67:
                print(f"    *** MAGIC MATCH! DLL format confirmed ***")
            else:
                print(f"    Magic: 0x{plain[0]:02x}{plain[1]:02x} (expected 0x4D67)")
            
            # What ARE bytes 0,1 in the plaintext?
            # We previously called them "nonce" but let's check the pattern
            print(f"    plain[0:5] = [{' '.join(f'{x:02x}' for x in plain[:5])}]")
            print(f"    plain[6:12] = [{' '.join(f'{x:02x}' for x in plain[6:12])}]")
        print()
    
    # Cross-check: for BLACK, the entire LED region should decrypt to 0
    print("=== Verifying 3-byte XOR on BLACK LED region ===")
    errors = 0
    for idx, pkt in enumerate(black_pkts):
        k0 = pkt[12]  # XOR with 0 = key itself
        k1 = pkt[13]
        k2 = pkt[14]
        # Check all LED bytes
        for j in range(12, 237, 3):
            if pkt[j] != k0 or pkt[j+1] != k1 or pkt[j+2] != k2:
                errors += 1
                break
        # Check tail
        if pkt[237] != k0:  # tail at offset 237, 237%3=0
            errors += 1
    print(f"  Packets with LED errors: {errors}/{len(black_pkts)}")
    
    # Now the key question: is the cipher REALLY just 3-byte XOR,
    # or does it just LOOK like that because the table+IV+rotate produces
    # a pattern that appears to repeat every 3 bytes?
    
    # If the DLL cipher is table[t] ^ IV_rotated[i%6], and the result
    # appears as period-3, then TABLE * IV_rotate must produce period-3 keystream.
    
    # OR: the screen data packets use a completely different (simpler) cipher.
    # Let's check get_large_screendata_package to see.
    
    print("\n=== Analysis of key3 relationship to packet bytes ===")
    print("If 3-byte XOR is the real cipher, key3 must be derivable from packet header")
    
    # In our earlier analysis, plain[0] and plain[1] are the "nonce"
    # and key3 has a deterministic but non-trivial relationship to them.
    # This nonce might be generated by rand() in the DLL, and key3 is
    # derived from the table somehow.
    
    # Let's check: is key3 = TABLE[some_offset]?
    from cipher_table import CIPHER_TABLE
    table = CIPHER_TABLE
    
    print(f"\nTable length: {len(table)} bytes")
    
    # For each BLACK packet, extract key3 and search for it in the table
    found = 0
    for idx, pkt in enumerate(black_pkts[:10]):
        k0, k1, k2 = pkt[12], pkt[13], pkt[14]
        key3 = bytes([k0, k1, k2])
        
        # Search for key3 in table
        pos = table.find(key3)
        if pos >= 0:
            print(f"  Pkt {idx}: key3=[{k0:02x},{k1:02x},{k2:02x}] FOUND at table[{pos}]")
            found += 1
        else:
            # Search for individual bytes
            p0 = [i for i in range(len(table)) if table[i] == k0]
            print(f"  Pkt {idx}: key3=[{k0:02x},{k1:02x},{k2:02x}] NOT found as triplet. "
                  f"K0 at positions: {p0[:5]}...")
    
    print(f"\n  Found {found}/10 key3 triplets in table")


if __name__ == "__main__":
    main()
