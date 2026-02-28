# -*- coding: utf-8 -*-
"""
verify_cipher_v2.py — Верификация cipher FUN_10001880 на эталонных пакетах из DLL.

Формат пакета DLL:
  [0]    = 0x4D ('M')
  [1]    = 0x67 ('g')
  [2]    = 0x4C ('L')
  [3]    = (data_len + 10) >> 8
  [4]    = (data_len + 10) & 0xFF
  [5-10] = IV (6 random bytes, plaintext)
  [11+]  = encrypted: [cmd, attr, rand, rand, data...]

Cipher (FUN_10001880):
  For each byte at offset i (starting from 0 within encrypted region):
    ciphertext[i] = plaintext[i] ^ TABLE[table_idx] ^ IV[iv_idx]
    table_idx = (table_idx + 1) % TABLE_LEN
    iv_idx increments; after every 6 bytes, IV is bit-rotated left by 1 bit (48-bit)
"""
import sys
import os
from cipher_table_runtime import CIPHER_TABLE

TABLE = CIPHER_TABLE
TABLE_LEN = len(TABLE)  # 521


def rotate_iv_left_1bit(iv6):
    """Bit-rotate-left the 48-bit IV (little-endian) by 1 bit."""
    val = int.from_bytes(iv6, 'little')
    val = ((val << 1) | (val >> 47)) & ((1 << 48) - 1)
    return val.to_bytes(6, 'little')


def decrypt(encrypted_bytes, iv6):
    """Decrypt bytes[11+] using TABLE and IV."""
    iv = bytearray(iv6)
    result = bytearray(encrypted_bytes)
    table_idx = 0
    iv_idx = 0

    for i in range(len(result)):
        result[i] ^= TABLE[table_idx] ^ iv[iv_idx]

        table_idx = (table_idx + 1) % TABLE_LEN
        iv_idx += 1
        if iv_idx >= 6:
            iv = bytearray(rotate_iv_left_1bit(bytes(iv)))
            iv_idx = 0

    return bytes(result)


def encrypt(plain_bytes, iv6):
    """Encrypt = same as decrypt (XOR is symmetric)."""
    return decrypt(plain_bytes, iv6)


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    print(f"Table: {TABLE_LEN} bytes")
    print(f"First 16: {' '.join(f'{b:02x}' for b in TABLE[:16])}")
    print()

    # ================================================================
    # Test 1: get_scen_package(scenid=0, dimmer=100, R=255, G=0, B=0, power=1)
    # Expected plaintext after decryption:
    #   cmd=2, attr=0, rand, rand, scenid=0, dimmer=100, R=255, G=0, B=0, power=1
    # ================================================================
    print("=" * 60)
    print("TEST 1: get_scen_package RED")
    print("=" * 60)

    # From DLL output (5 calls):
    scen_packets = [
        bytes.fromhex("4d674c001093fdbb8354d4b4a57c564e25ce47145c"),
        bytes.fromhex("4d674c0010694506b8f2c54e1d9c34e8343a366f2b"),
        bytes.fromhex("4d674c0010943471531a1ab36c77ac0aebc1d581fd"),
        bytes.fromhex("4d674c00101b3048ec6a223c68b36570d3dfdcf383"),
        bytes.fromhex("4d674c00100c93fdaefc612bcbe10fe690f19a9806"),
    ]

    for idx, pkt in enumerate(scen_packets):
        # Verify header
        assert pkt[0:3] == b'\x4d\x67\x4c', f"Bad magic in pkt {idx}"
        length_field = (pkt[3] << 8) | pkt[4]
        iv = pkt[5:11]
        encrypted = pkt[11:]

        print(f"\n  Pkt {idx}: len_field={length_field} IV={iv.hex()} "
              f"encrypted={' '.join(f'{b:02x}' for b in encrypted)}")

        # Decrypt
        plain = decrypt(encrypted, iv)
        plain_hex = ' '.join(f'{b:02x}' for b in plain)
        print(f"  Decrypted: [{plain_hex}]")

        # Parse
        cmd = plain[0]
        attr = plain[1]
        rand0 = plain[2]
        rand1 = plain[3]
        data = plain[4:]
        print(f"  cmd={cmd} attr={attr} rand=[{rand0:02x},{rand1:02x}] "
              f"data=[{' '.join(f'{b:02x}' for b in data)}]")

        # Verify expected values
        if cmd == 2 and attr == 0:
            print(f"  ✓ cmd=2 attr=0 correct!")
            if len(data) >= 6:
                scenid, dimmer, r, g, b, power = data[0], data[1], data[2], data[3], data[4], data[5]
                print(f"  scenid={scenid} dimmer={dimmer} R={r} G={g} B={b} power={power}")
                if scenid == 0 and dimmer == 100 and r == 255 and g == 0 and b == 0 and power == 1:
                    print(f"  ✓ ALL VALUES CORRECT! Cipher verified!")
                else:
                    print(f"  ✗ Values don't match expected (0,100,255,0,0,1)")
        else:
            print(f"  ✗ cmd/attr don't match expected (2,0)")

    # ================================================================
    # Test 2: get_scen_package BLACK
    # ================================================================
    print("\n" + "=" * 60)
    print("TEST 2: get_scen_package BLACK")
    print("=" * 60)

    black_packets = [
        bytes.fromhex("4d674c0010f72ba7d88b11d0730597 91e0f8eb2dea".replace(" ", "")),
        bytes.fromhex("4d674c001071bb7e3d1c9d56e3fe9d066cf5ca9e21"),
        bytes.fromhex("4d674c0010927961c47b25b521e75c61d4324fa1d3"),
    ]

    for idx, pkt in enumerate(black_packets):
        iv = pkt[5:11]
        encrypted = pkt[11:]
        plain = decrypt(encrypted, iv)
        plain_hex = ' '.join(f'{b:02x}' for b in plain)
        print(f"\n  Pkt {idx}: IV={iv.hex()} decrypted=[{plain_hex}]")

        cmd, attr = plain[0], plain[1]
        data = plain[4:]
        print(f"  cmd={cmd} attr={attr} data=[{' '.join(f'{b:02x}' for b in data)}]")
        if cmd == 2 and attr == 0 and len(data) >= 6:
            scenid, dimmer, r, g, b, power = data[0], data[1], data[2], data[3], data[4], data[5]
            print(f"  scenid={scenid} dimmer={dimmer} R={r} G={g} B={b} power={power}")
            if r == 0 and g == 0 and b == 0:
                print(f"  ✓ BLACK confirmed!")

    # ================================================================
    # Test 3: Decrypt get_large_screendata_package (1920x1080 RED)
    # ================================================================
    print("\n" + "=" * 60)
    print("TEST 3: get_large_screendata_package (RED 1920x1080)")
    print("=" * 60)

    screen_path = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                               "pkt_screen_1920x1080.bin")
    if os.path.exists(screen_path):
        with open(screen_path, "rb") as f:
            screen_pkt = f.read()

        print(f"  Packet length: {len(screen_pkt)}")
        assert screen_pkt[0:3] == b'\x4d\x67\x4c'
        length_field = (screen_pkt[3] << 8) | screen_pkt[4]
        iv = screen_pkt[5:11]
        encrypted = screen_pkt[11:]

        print(f"  length_field={length_field} IV={iv.hex()}")
        print(f"  Encrypted region: {len(encrypted)} bytes")

        plain = decrypt(encrypted, iv)
        cmd, attr = plain[0], plain[1]
        rand0, rand1 = plain[2], plain[3]
        data = plain[4:]

        print(f"  cmd={cmd} attr={attr} rand=[{rand0:02x},{rand1:02x}]")
        print(f"  Data length: {len(data)} bytes")
        print(f"  Data[0:30]: {' '.join(f'{b:02x}' for b in data[:30])}")
        print(f"  Data[-20:]: {' '.join(f'{b:02x}' for b in data[-20:])}")

        # For RED image: the data contains processed LED colors
        # Expected: lots of 0xFF bytes (red) mixed with 0x00 (green, blue)
        # Count color-like patterns
        if cmd == 1 and attr == 1:
            print(f"  ✓ cmd=1 attr=1 (screen data) correct!")

            # Check for repeating patterns in LED data
            # LED data format from DLL: probably [R,G,B] × N_leds
            # But data is 1500 bytes, which is way more than 75*3=225
            # So it contains multiple zones/regions
            print(f"\n  Byte value distribution in data:")
            from collections import Counter
            counts = Counter(data)
            for val, cnt in counts.most_common(10):
                print(f"    0x{val:02x} ({val:3d}): {cnt} times ({cnt/len(data)*100:.1f}%)")
    else:
        print(f"  File not found: {screen_path}")
        print(f"  Run dll_call_32bit_v2.py first")

    # ================================================================
    # Test 4: Now try to understand the wire format
    # ================================================================
    print("\n" + "=" * 60)
    print("TEST 4: Analyzing wire captures with correct table")
    print("=" * 60)

    # The wire captures DON'T have MgL header and are 238-245 bytes.
    # The DLL outputs 1515 bytes with MgL header.
    # The .NET code must transform the DLL output before sending.
    #
    # Hypothesis: the .NET code strips the 5-byte MgL header,
    # splits the remaining 1510 bytes into chunks, and adds its own framing.
    # 1510 / 6 ≈ 251, 1510 / 7 ≈ 215... not matching 238-245.
    #
    # OR: the .NET code doesn't use get_large_screendata_package for
    # solid colors at all. It might call get_scen_package (21 bytes)
    # and send that on the wire. But captures are 238 bytes...
    #
    # Wait - maybe the app ISN'T in "solid color" mode but in
    # "screen mirroring" mode, and the captures show screen data
    # of a solid-color preview window?
    #
    # In that case, the 238-byte captures might be one of several
    # serial chunks that together make up the 1515-byte DLL packet.
    #
    # BUT we showed each heartbeat has exactly 1 data packet (238-245).
    # If they were chunks of 1515, we'd expect ~6 chunks per heartbeat.
    #
    # So the 238-byte packets must be from a DIFFERENT function or
    # the .NET code builds them differently.
    #
    # KEY INSIGHT: maybe the .NET code calls get_scen_package (21 bytes)
    # and then WRAPS it with additional framing to create a 238-byte packet.
    # 
    # OR: maybe there's an unpack_package reverse function that the
    # controller uses, and the wire format IS the DLL format, but
    # our capture tool shows byte-stuffed/escaped data.

    # Let's just try: treat captures as if the ENTIRE thing is encrypted
    # with the DLL cipher (no plaintext header), starting table_idx=0
    csv_dir = os.path.dirname(os.path.abspath(__file__))
    black_path = os.path.join(csv_dir, "black.csv")
    if os.path.exists(black_path):
        capture_pkts = []
        with open(black_path, "r", errors="replace") as f:
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
                if len(raw) == 238:
                    capture_pkts.append(raw)

        print(f"  BLACK 238-byte captures: {len(capture_pkts)}")

        # Try hypothesis: captures are the encrypted portion of DLL output
        # (bytes [11:] of a 1515-byte packet), split into chunks.
        # Or they are the DLL output after stripping MgL+len (5 bytes).
        # 1515 - 5 = 1510 bytes. 1510 / 238 ≈ 6.34. Not clean.
        # 1515 - 11 = 1504 bytes. 1504 / 238 ≈ 6.32. Not clean.

        # What if captures = DLL output without MgL header, and length varies?
        # DLL: 5(hdr) + 6(IV) + 4(cmd+attr+2rand) + N(data) = 15+N
        # If capture = 6(IV) + 4(cmd+attr+2rand) + N(data) = 10+N
        # Then N = 238-10 = 228. And 228 = 76*3. 76 LEDs? (we expected 75)
        # That's close! Maybe 75 LEDs + 1 byte tail.
        # 75*3 + 3 = 228. Hmm.

        # Actually from earlier analysis: plaintext was
        # [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B(=75), R,G,B×75, 00]
        # That's 2 + 4 + 2 + 2 + 1 + 1 + 225 + 1 = 238!
        # So the capture IS the complete payload for a single-frame solid color command.

        # This means the captures are NOT from get_large_screendata_package at all.
        # They are from a DIFFERENT function that sends a single 238-byte packet
        # with its own format. Let's decode with the 3-byte XOR that partially worked.

        # Actually, let's try: maybe the capture IS the DLL format without the MgL header.
        # bytes[0-5] = IV, bytes[6:] = encrypted
        pkt = capture_pkts[0]
        print(f"\n  Trying: IV = pkt[0:6], encrypted = pkt[6:]")
        iv = pkt[0:6]
        enc = pkt[6:]
        plain = decrypt(enc, iv)
        print(f"  IV={iv.hex()} plain[0:20]={' '.join(f'{b:02x}' for b in plain[:20])}")

        # Try: IV = pkt[0:6], encrypted = pkt[6:], but the plaintext format
        # starts with cmd, attr, rand, rand
        cmd = plain[0]
        attr = plain[1]
        print(f"  cmd={cmd} attr={attr}")

        # Try other offsets
        for skip in [0, 1, 2, 3, 4, 5]:
            iv = pkt[skip:skip+6]
            enc = pkt[skip+6:]
            plain = decrypt(enc, iv)
            # Check if plaintext looks reasonable
            zero_count = sum(1 for b in plain[10:] if b == 0)
            total = len(plain) - 10
            zero_pct = zero_count / total * 100 if total > 0 else 0
            print(f"  skip={skip}: IV={iv.hex()} cmd={plain[0]:02x} "
                  f"plain[0:8]=[{' '.join(f'{b:02x}' for b in plain[:8])}] "
                  f"zeros_in_data={zero_count}/{total} ({zero_pct:.0f}%)")

    print("\nDONE")


if __name__ == "__main__":
    main()
