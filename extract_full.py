# -*- coding: utf-8 -*-
"""
extract_full.py — Извлечь кастомный Base64 алфавит и декодировать таблицу шифрования.

Addresses from Ghidra:
  DAT_1003e890 — Custom Base64 alphabet (64 chars)
  DAT_1003c4c0 — Encoded string
  DAT_1003f9c0 — Destination table (521 bytes)
"""
import sys
import struct
import os

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)


def va_to_file_offset(dll_data, va, image_base=0x10000000):
    """Convert VA to file offset using PE section headers."""
    pe_offset = struct.unpack_from('<I', dll_data, 0x3C)[0]
    num_sections = struct.unpack_from('<H', dll_data, pe_offset + 6)[0]
    opt_hdr_size = struct.unpack_from('<H', dll_data, pe_offset + 20)[0]
    section_start = pe_offset + 24 + opt_hdr_size

    rva = va - image_base

    for i in range(num_sections):
        so = section_start + i * 40
        sec_va = struct.unpack_from('<I', dll_data, so + 12)[0]
        sec_vsize = struct.unpack_from('<I', dll_data, so + 8)[0]
        sec_rawptr = struct.unpack_from('<I', dll_data, so + 20)[0]

        if sec_va <= rva < sec_va + sec_vsize:
            return sec_rawptr + (rva - sec_va)
    return None


def read_cstring(data, offset, max_len=2048):
    """Read null-terminated ASCII string."""
    end = data.index(b'\x00', offset)
    if end - offset > max_len:
        end = offset + max_len
    return data[offset:end].decode('ascii', errors='replace')


def custom_b64_decode(encoded, alphabet):
    """
    Decode using custom Base64 alphabet.
    Reimplements FUN_10001000 logic:
    - Read 4 chars at a time
    - Look up each char's index in 64-char alphabet (6 bits)
    - Pack 4×6 = 24 bits = 3 bytes
    - '=' means padding (stop)
    """
    result = bytearray()
    i = 0
    while i + 3 < len(encoded):
        chunk = encoded[i:i+4]
        i += 4

        bits = 0
        pad_pos = None
        shift = 0
        for ci, ch in enumerate(chunk):
            if ch == '=':
                pad_pos = ci
                break
            idx = alphabet.find(ch)
            if idx < 0:
                idx = 0  # fallback
            bits |= (idx & 0x3F) << shift
            shift += 6

        # Extract 3 bytes (little-endian bit packing)
        b0 = bits & 0xFF
        b1 = (bits >> 8) & 0xFF
        b2 = (bits >> 16) & 0xFF

        if pad_pos is not None:
            # Padding — stop
            # But still output partial bytes
            if pad_pos >= 2:
                result.append(b0)
            if pad_pos >= 3:
                result.append(b1)
            break
        else:
            result.append(b0)
            result.append(b1)
            result.append(b2)

    return bytes(result)


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    with open(DLL_PATH, "rb") as f:
        dll = f.read()

    print(f"DLL size: {len(dll)} bytes\n")

    # Extract custom alphabet from DAT_1003e890
    alpha_off = va_to_file_offset(dll, 0x1003e890)
    print(f"Alphabet VA=0x1003e890 -> file offset 0x{alpha_off:08x}")

    # Read string at that offset
    alphabet = read_cstring(dll, alpha_off)
    print(f"Alphabet ({len(alphabet)} chars): {alphabet}")

    if len(alphabet) < 64:
        # Maybe it's not a simple null-terminated string
        # Read raw 64 bytes
        raw = dll[alpha_off:alpha_off + 80]
        print(f"Raw bytes: {' '.join(f'{b:02x}' for b in raw)}")
        print(f"Raw ascii: {''.join(chr(b) if 32 <= b < 127 else '.' for b in raw)}")

    # Extract encoded string from DAT_1003c4c0
    enc_off = va_to_file_offset(dll, 0x1003c4c0)
    encoded = read_cstring(dll, enc_off)
    print(f"\nEncoded string VA=0x1003c4c0 -> file offset 0x{enc_off:08x}")
    print(f"Length: {len(encoded)} chars")
    print(f"String: {encoded[:100]}...")
    print(f"Full:   {encoded}")

    # Decode with custom alphabet
    if len(alphabet) >= 64:
        alpha64 = alphabet[:64]
    else:
        # Try reading more — the alphabet might be split or have nulls
        # Let's read raw 64 bytes and filter
        raw64 = dll[alpha_off:alpha_off + 128]
        # Find all printable chars
        alpha64 = ''.join(chr(b) for b in raw64 if 32 <= b < 127)[:64]
        print(f"\nReconstructed alphabet ({len(alpha64)} chars): {alpha64}")

    print(f"\nUsing alphabet: {alpha64}")
    table = custom_b64_decode(encoded, alpha64)
    print(f"\nDecoded table: {len(table)} bytes")
    print(f"First 32: {' '.join(f'{b:02x}' for b in table[:32])}")
    print(f"Last  16: {' '.join(f'{b:02x}' for b in table[-16:])}")

    nz = sum(1 for b in table if b != 0)
    print(f"Non-zero: {nz}/{len(table)}")

    # Save
    out_bin = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cipher_table.bin")
    with open(out_bin, "wb") as f:
        f.write(table)
    print(f"\nSaved binary: {out_bin}")

    out_py = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cipher_table.py")
    with open(out_py, "w") as f:
        f.write("# Auto-extracted from beelightLib.dll\n")
        f.write(f"# Custom Base64 decoded from DAT_1003c4c0 using alphabet from DAT_1003e890\n")
        f.write(f"# {len(table)} bytes\n")
        f.write("CIPHER_TABLE = bytes([\n")
        for i in range(0, len(table), 16):
            chunk = table[i:i + 16]
            f.write("    " + ", ".join(f"0x{b:02x}" for b in chunk) + ",\n")
        f.write("])\n")
    print(f"Saved Python: {out_py}")


if __name__ == "__main__":
    main()
