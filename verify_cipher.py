# -*- coding: utf-8 -*-
"""
verify_cipher.py — Реализация и верификация реального cipher из beelightLib.dll.

Алгоритм (из FUN_10001880):
1. Пакет: [magic 2B][length 2B][?1B][IV 6B][cmd 1B][attr 1B][rand 1B][rand 1B][data NB]
2. Шифрование: offset 0x0b (byte 11) до конца
3. cipher[i] ^= TABLE[table_idx] ^ IV[iv_idx]
4. table_idx = (table_idx + 1) % TABLE_LEN (0x209 = 521 or 519)
5. iv_idx = (iv_idx + 1); после каждых 6 байт — bit-rotate-left всего 48-bit IV

Packet layout (before encryption):
  [0-1]   magic = 0x674D (LE: 0x4D, 0x67)
  [2-3]   length = (data_len + 10) as LE uint16
  [4]     unknown (not explicitly set — probably 0)
  [5-10]  IV = 6 random bytes
  [11]    cmd
  [12]    attr
  [13]    rand
  [14]    rand
  [15..]  data payload
"""
import sys
import os
from cipher_table import CIPHER_TABLE

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
TABLE = CIPHER_TABLE
TABLE_LEN = len(TABLE)


def rotate_iv_left_1bit(iv6):
    """
    Bit-rotate-left the 48-bit IV by 1 bit.
    IV is stored as bytes [0..5] where byte[0] is LSB of 48-bit value.
    
    From disassembly (FUN_10001880):
      The rotation goes: byte5 -> byte0 -> byte1 -> byte2 -> byte3 -> byte4 -> byte5
      Each byte: new = (old << 1) | (carry_from_next_byte >> 7)
      Carry chain: 5->0->1->2->3->4->5
    
    Actually looking more carefully at the asm:
      bStack00000007 = bStack00000007 * 2 | local_8 >> 7    (byte0 = byte0*2 | byte5>>7)
      local_10[1] = param_5._3_1_ * 2 | local_10[1]        (byte1 = byte1*2 | byte0_old>>7)
      ...carry propagates upward...
      local_8 = bVar5 | local_8 * 2                         (byte5 = byte5*2 | byte4>>7)
    
    This is a LEFT rotate of the entire 48-bit value by 1 bit.
    """
    # Treat iv6 as 48-bit little-endian integer
    val = int.from_bytes(iv6, 'little')
    # Rotate left by 1 within 48 bits
    val = ((val << 1) | (val >> 47)) & ((1 << 48) - 1)
    return val.to_bytes(6, 'little')


def encrypt_payload(plaintext_from_0xb, iv6):
    """
    XOR-encrypt bytes starting at offset 0x0b using TABLE and IV.
    
    plaintext_from_0xb: bytes to encrypt (from offset 0x0b onwards)
    iv6: 6-byte IV (bytes [5..10] of the packet)
    
    Returns encrypted bytes.
    """
    iv = bytearray(iv6)
    result = bytearray(plaintext_from_0xb)
    table_idx = 0
    iv_idx = 0
    
    for i in range(len(result)):
        result[i] ^= TABLE[table_idx] ^ iv[iv_idx]
        
        table_idx += 1
        if table_idx >= TABLE_LEN:
            table_idx = 0
        
        iv_idx += 1
        if iv_idx >= 6:
            iv = bytearray(rotate_iv_left_1bit(bytes(iv)))
            iv_idx = 0
    
    return bytes(result)


def decrypt_payload(ciphertext_from_0xb, iv6):
    """Decryption is the same as encryption (XOR is symmetric)."""
    return encrypt_payload(ciphertext_from_0xb, iv6)


def parse_long_writes(filepath):
    """Parse CSV to extract long data packets (>10 bytes)."""
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


def verify_on_captures():
    """Verify the cipher by decrypting captured packets and checking known plaintext."""
    
    # Test on BLACK packets (all LED bytes should be 0x00)
    black_path = os.path.join(CSV_DIR, "black.csv")
    black_pkts = parse_long_writes(black_path)
    
    # Filter 238-byte packets (solid color)
    solid_pkts = [p for p in black_pkts if len(p) == 238]
    print(f"BLACK 238-byte packets: {len(solid_pkts)}")
    
    success = 0
    fail = 0
    
    for idx, pkt in enumerate(solid_pkts[:20]):  # First 20
        # Extract IV from bytes [5..10]
        iv6 = pkt[5:11]
        
        # Encrypted region starts at offset 0x0b (byte 11)
        encrypted = pkt[11:]
        
        # Decrypt
        decrypted = decrypt_payload(encrypted, iv6)
        
        # For BLACK solid color:
        # decrypted[0] = cmd (should be some known value)
        # decrypted[1] = attr
        # decrypted[2] = rand
        # decrypted[3] = rand
        # decrypted[4..] = data from get_scen_package: [scenid, dimmer, R, G, B, power_flag]
        # Then the rest...
        
        # Actually let me re-examine the packet layout
        # get_scen_package passes: cmd=2, attr=0, data=[scenid,dimmer,R,G,B,power], len=6
        # FUN_10001880 builds:
        #   [0-1] = 0x4D67 (magic)
        #   [2]   = (6+10) & 0xFF = 16
        #   [3]   = (6+10) >> 8 = 0
        #   [4]   = ???
        #   [5-10] = IV (6 random bytes)
        #   [11]  = cmd = 2
        #   [12]  = attr = 0
        #   [13]  = rand
        #   [14]  = rand
        #   [15-20] = data (6 bytes: scenid, dimmer, R, G, B, power)
        #
        # But that's only 21 bytes... our packets are 238 bytes
        # So solid color packets are NOT from get_scen_package!
        # They must be from get_large_screendata_package
        
        # Let's just look at what we get
        dec_hex = ' '.join(f'{b:02x}' for b in decrypted[:20])
        
        # Check: for BLACK, LED region should be all zeros
        # In 238-byte packet: header=12, LED=225, tail=1 (from our earlier analysis)
        # But that was based on wrong cipher model. Let's see what this decryption gives us.
        
        # The encrypted region is bytes[11:] = 227 bytes
        # Let's check if large portions are zero (BLACK)
        zero_count = sum(1 for b in decrypted if b == 0)
        total = len(decrypted)
        zero_pct = zero_count / total * 100
        
        status = "GOOD" if zero_pct > 80 else "BAD"
        if zero_pct > 80:
            success += 1
        else:
            fail += 1
        
        print(f"  Pkt {idx:2d}: IV={iv6.hex()} decrypt[0:20]=[{dec_hex}] "
              f"zeros={zero_count}/{total} ({zero_pct:.0f}%) {status}")
    
    print(f"\nResults: {success} good, {fail} bad out of {success+fail}")
    
    # Now try RED packets
    print(f"\n{'='*70}")
    red_path = os.path.join(CSV_DIR, "red.csv")
    red_pkts = parse_long_writes(red_path)
    solid_red = [p for p in red_pkts if len(p) == 238]
    print(f"RED 238-byte packets: {len(solid_red)}")
    
    for idx, pkt in enumerate(solid_red[:5]):
        iv6 = pkt[5:11]
        encrypted = pkt[11:]
        decrypted = decrypt_payload(encrypted, iv6)
        dec_hex = ' '.join(f'{b:02x}' for b in decrypted[:20])
        
        # Check for repeating pattern in LED region
        # For RED: expect 0xFF 0x00 0x00 repeating
        print(f"  Pkt {idx:2d}: IV={iv6.hex()} decrypt[0:20]=[{dec_hex}]")
    
    # Also try: what if the header bytes [0..4] aren't what we think?
    # Let's check magic and length
    print(f"\n{'='*70}")
    print("Checking packet headers:")
    for idx, pkt in enumerate(solid_pkts[:5]):
        hdr = pkt[:11]
        hdr_hex = ' '.join(f'{b:02x}' for b in hdr)
        # Check magic
        magic = pkt[0] | (pkt[1] << 8)
        length_field = pkt[2] | (pkt[3] << 8)
        iv_hex = pkt[5:11].hex()
        print(f"  Pkt {idx}: hdr=[{hdr_hex}] magic=0x{magic:04x} len_field={length_field} IV={iv_hex}")


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    verify_on_captures()
