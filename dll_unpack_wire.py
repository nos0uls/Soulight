# -*- coding: utf-8 -*-
"""
dll_unpack_wire.py - Try to use DLL's unpack_package on wire captures.

Strategy:
  1. Read a wire capture payload (238 bytes)
  2. Wrap it in MgL header: [4D 67 4C len_hi len_lo] + payload
  3. Call unpack_package
  4. See if the result is valid plaintext

Also: try feeding the raw payload without MgL header.

Run with 32-bit Python!
"""
import sys
import os
import ctypes
import struct

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Novaya papka", "Dumps", "beelightLib.dll"
)
# Handle cyrillic path
DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll"
)

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

    ptr_size = struct.calcsize("P") * 8
    if ptr_size != 32:
        print("ERROR: Need 32-bit Python!")
        sys.exit(1)

    dll = ctypes.CDLL(DLL_PATH)
    print(f"DLL loaded")

    # Load some wire captures
    black_pairs = [(h, p) for h, p in parse_paired_writes(os.path.join(CSV_DIR, "black.csv"))
                   if len(p) == 238]
    print(f"BLACK 238-byte pairs: {len(black_pairs)}")

    if not black_pairs:
        print("No pairs found!")
        return

    # ================================================================
    # Test 1: Wrap wire payload in MgL header and call unpack_package
    # ================================================================
    print("\n=== Test 1: MgL header + wire payload ===")

    for pidx in range(3):
        hdr_frame, payload = black_pairs[pidx]

        # Wrap: [4D 67 4C] [len_hi] [len_lo] [payload]
        # length field = len(payload) + 10? or len(payload)?
        # From DLL: length = data_len + 10. For scen: data=6, field=16=0x10
        # So for our case: the encrypted region = len(payload)
        # encrypted = cmd(1) + attr(1) + rand(2) + data(N) = 4 + N
        # field = N + 10 = len(payload) - 6(IV) + 10 = len(payload) + 4
        # Or maybe field = len(payload) + 10? Let's try both.

        for field_val in [len(payload), len(payload) + 10, len(payload) + 4,
                          len(payload) - 6, len(payload) - 6 + 10]:
            mgl = bytearray(5 + len(payload))
            mgl[0] = 0x4D
            mgl[1] = 0x67
            mgl[2] = 0x4C
            mgl[3] = (field_val >> 8) & 0xFF
            mgl[4] = field_val & 0xFF
            mgl[5:] = payload

            buf = ctypes.create_string_buffer(bytes(mgl), len(mgl))
            result = dll.unpack_package(buf, len(mgl))

            unpacked = bytes(buf.raw[:len(mgl)])
            # Check if first bytes look like valid plaintext
            # For scen_package: [00 06 02 00 ...data...]
            # For screen: [?? ?? 01 01 ...data...]
            b0, b1, b2, b3 = unpacked[0], unpacked[1], unpacked[2], unpacked[3]

            looks_good = False
            if b2 in [1, 2, 5, 6] and b3 in [0, 1]:
                looks_good = True

            if looks_good or pidx == 0:
                print(f"  Pkt {pidx} field={field_val}: ret={result} "
                      f"first 12=[{' '.join(f'{b:02x}' for b in unpacked[:12])}] "
                      f"{'<-- GOOD?' if looks_good else ''}")

    # ================================================================
    # Test 2: Raw payload without MgL header
    # ================================================================
    print("\n=== Test 2: Raw payload (no MgL) ===")

    for pidx in range(3):
        hdr_frame, payload = black_pairs[pidx]
        buf = ctypes.create_string_buffer(bytes(payload), len(payload))
        result = dll.unpack_package(buf, len(payload))
        unpacked = bytes(buf.raw[:len(payload)])
        print(f"  Pkt {pidx}: ret={result} "
              f"first 20=[{' '.join(f'{b:02x}' for b in unpacked[:20])}]")

    # ================================================================
    # Test 3: Try full frame (55 AA 5A + payload)
    # ================================================================
    print("\n=== Test 3: Full frame (55 AA 5A + payload) ===")

    for pidx in range(3):
        hdr_frame, payload = black_pairs[pidx]
        full = hdr_frame + payload
        buf = ctypes.create_string_buffer(bytes(full), len(full))
        result = dll.unpack_package(buf, len(full))
        unpacked = bytes(buf.raw[:len(full)])
        print(f"  Pkt {pidx}: ret={result} "
              f"first 20=[{' '.join(f'{b:02x}' for b in unpacked[:20])}]")
        # Check at offset 5 (after 55 AA 5A xx 00)
        print(f"    offset 5: [{' '.join(f'{b:02x}' for b in unpacked[5:17])}]")

    # ================================================================
    # Test 4: Generate DLL packet and compare with wire format
    # ================================================================
    print("\n=== Test 4: DLL output structure analysis ===")

    # Generate a scen_package and examine BEFORE and AFTER encryption
    # Step 1: generate encrypted packet
    buf_enc = ctypes.create_string_buffer(256)
    dll.get_scen_package(0, 100, 0, 0, 0, 1, buf_enc)  # BLACK
    enc_raw = bytes(buf_enc.raw[:21])
    print(f"  Encrypted (BLACK): [{' '.join(f'{b:02x}' for b in enc_raw)}]")

    # Step 2: unpack it
    buf_dec = ctypes.create_string_buffer(bytes(enc_raw), len(enc_raw))
    result = dll.unpack_package(buf_dec, 21)
    dec_raw = bytes(buf_dec.raw[:21])
    print(f"  Unpacked:          [{' '.join(f'{b:02x}' for b in dec_raw)}]")
    print(f"  Return value: {result}")

    # Compare enc and dec to find which bytes changed
    print(f"  Changed bytes:")
    for i in range(21):
        if enc_raw[i] != dec_raw[i]:
            print(f"    [{i:2d}] 0x{enc_raw[i]:02x} -> 0x{dec_raw[i]:02x}")

    # The IV bytes [5:11] should remain same, encrypted [11:] should be decrypted
    print(f"\n  enc[5:11] (IV): [{enc_raw[5:11].hex()}]")
    print(f"  dec[5:11] (IV): [{dec_raw[5:11].hex()}]")
    print(f"  enc[0:5] (hdr): [{enc_raw[0:5].hex()}]")
    print(f"  dec[0:5] (hdr): [{dec_raw[0:5].hex()}]")

    # ================================================================
    # Test 5: Try generating screendata and see if format matches wire
    # ================================================================
    print("\n=== Test 5: Screen data packet analysis ===")

    img_size = 1920 * 1080 * 3
    img = ctypes.create_string_buffer(img_size)
    # All black
    buf_screen = ctypes.create_string_buffer(2048)
    result = dll.get_large_screendata_package(img, 1920, 1080, 3, buf_screen)

    if result == 1:
        screen_raw = bytes(buf_screen.raw[:1515])
        print(f"  Screen packet (BLACK): {len(screen_raw)} bytes")
        print(f"  Header [0:11]: [{' '.join(f'{b:02x}' for b in screen_raw[:11])}]")
        print(f"  IV [5:11]: [{screen_raw[5:11].hex()}]")

        # Unpack
        buf_unp = ctypes.create_string_buffer(bytes(screen_raw), len(screen_raw))
        result2 = dll.unpack_package(buf_unp, 1515)
        unp_raw = bytes(buf_unp.raw[:1515])
        print(f"  Unpacked header [0:11]: [{' '.join(f'{b:02x}' for b in unp_raw[:11])}]")
        print(f"  Unpacked [11:30]: [{' '.join(f'{b:02x}' for b in unp_raw[11:30])}]")

        # The plaintext data for BLACK screen should have lots of zeros
        data_start = 15  # 5 hdr + 6 IV + 4 (cmd+attr+2rand)
        data = unp_raw[data_start:]
        zeros = sum(1 for b in data[:225] if b == 0)
        print(f"  Data[15:240] zeros: {zeros}/225")

    print("\nDONE!")


if __name__ == "__main__":
    main()
