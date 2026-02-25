# -*- coding: utf-8 -*-
"""
extract_b64_table.py — Извлечь Base64 строку и таблицу шифрования из beelightLib.dll.

Адреса из Ghidra:
  DAT_1003c4c0 — Base64-encoded string (source)
  DAT_1003f9c0 — Decoded table (destination, 521 bytes = 0x209)
  
DLL image base = 0x10000000
Section .data RVA нужно вычислить из PE headers.
"""
import sys
import struct
import base64
import os

DLL_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                        "Новая папка", "Dumps", "beelightLib.dll")

# Virtual addresses from Ghidra
VA_B64_STRING = 0x1003c4c0
VA_TABLE = 0x1003f9c0


def va_to_file_offset(dll_data, va):
    """Convert virtual address to file offset using PE section headers."""
    # Parse PE
    dos_magic = struct.unpack_from('<H', dll_data, 0)[0]
    assert dos_magic == 0x5A4D, "Not a PE file"
    
    pe_offset = struct.unpack_from('<I', dll_data, 0x3C)[0]
    pe_sig = struct.unpack_from('<I', dll_data, pe_offset)[0]
    assert pe_sig == 0x4550, "Invalid PE signature"
    
    # COFF header
    num_sections = struct.unpack_from('<H', dll_data, pe_offset + 6)[0]
    opt_hdr_size = struct.unpack_from('<H', dll_data, pe_offset + 20)[0]
    image_base = struct.unpack_from('<I', dll_data, pe_offset + 24 + 28)[0]
    
    # RVA = VA - image_base
    rva = va - image_base
    
    # Section headers start after optional header
    section_start = pe_offset + 24 + opt_hdr_size
    
    for i in range(num_sections):
        sec_offset = section_start + i * 40
        sec_name = dll_data[sec_offset:sec_offset + 8].rstrip(b'\x00').decode('ascii', errors='replace')
        sec_vsize = struct.unpack_from('<I', dll_data, sec_offset + 8)[0]
        sec_va = struct.unpack_from('<I', dll_data, sec_offset + 12)[0]
        sec_rawsize = struct.unpack_from('<I', dll_data, sec_offset + 16)[0]
        sec_rawptr = struct.unpack_from('<I', dll_data, sec_offset + 20)[0]
        
        print(f"  Section {sec_name:8s}: VA=0x{sec_va:08x} VSize=0x{sec_vsize:08x} "
              f"RawPtr=0x{sec_rawptr:08x} RawSize=0x{sec_rawsize:08x}")
        
        if sec_va <= rva < sec_va + sec_vsize:
            file_off = sec_rawptr + (rva - sec_va)
            return file_off
    
    return None


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    
    with open(DLL_PATH, "rb") as f:
        dll = f.read()
    
    print(f"DLL: {DLL_PATH}")
    print(f"Size: {len(dll)} bytes")
    print(f"\nSections:")
    
    # Get file offset for Base64 string
    b64_offset = va_to_file_offset(dll, VA_B64_STRING)
    print(f"\nVA 0x{VA_B64_STRING:08x} -> file offset 0x{b64_offset:08x}")
    
    # Read the Base64 string (null-terminated)
    end = dll.index(b'\x00', b64_offset)
    b64_raw = dll[b64_offset:end]
    b64_str = b64_raw.decode('ascii', errors='replace')
    
    print(f"\nBase64 string ({len(b64_str)} chars):")
    print(b64_str[:80] + "..." if len(b64_str) > 80 else b64_str)
    print(f"\nFull string:\n{b64_str}")
    
    # Decode
    try:
        table = base64.b64decode(b64_str)
        print(f"\nDecoded table: {len(table)} bytes (expected 521 = 0x209)")
        print(f"First 32 bytes: {' '.join(f'{b:02x}' for b in table[:32])}")
        print(f"Last  16 bytes: {' '.join(f'{b:02x}' for b in table[-16:])}")
        
        # Save table
        table_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cipher_table.bin")
        with open(table_path, "wb") as f:
            f.write(table)
        print(f"\nTable saved to: {table_path}")
        
        # Also save as Python literal
        py_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cipher_table.py")
        with open(py_path, "w") as f:
            f.write("# Auto-extracted from beelightLib.dll DAT_1003c4c0 -> base64 decode\n")
            f.write(f"# {len(table)} bytes\n")
            f.write("CIPHER_TABLE = bytes([\n")
            for i in range(0, len(table), 16):
                chunk = table[i:i+16]
                f.write("    " + ", ".join(f"0x{b:02x}" for b in chunk) + ",\n")
            f.write("])\n")
        print(f"Python literal saved to: {py_path}")
        
    except Exception as e:
        print(f"\nBase64 decode FAILED: {e}")
        print("Raw bytes around b64 start:")
        print(' '.join(f'{b:02x}' for b in dll[b64_offset:b64_offset+64]))


if __name__ == "__main__":
    main()
