# -*- coding: utf-8 -*-
"""
dump_table_ctypes.py — Загрузить beelightLib.dll через ctypes,
вызвать init (через get_connect_package) и прочитать таблицу DAT_1003f9c0 из памяти.

При загрузке DLL вызывается DllMain → FUN_10001000 → таблица заполняется.
После этого читаем 521 байт из нужного смещения.
"""
import sys
import ctypes
import os
import struct

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)

# Virtual address of the table (from Ghidra)
VA_TABLE = 0x1003f9c0
TABLE_SIZE = 0x209  # 521 bytes


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    print(f"Loading DLL: {DLL_PATH}")
    print(f"Expecting table at VA 0x{VA_TABLE:08x} ({TABLE_SIZE} bytes)")
    print()

    # Load the DLL — this triggers DllMain which calls FUN_10001000
    try:
        dll = ctypes.CDLL(DLL_PATH)
    except OSError as e:
        print(f"ERROR loading DLL: {e}")
        print("Make sure you're running 32-bit Python (the DLL is 32-bit)")
        print("Check: python -c \"import struct; print(struct.calcsize('P')*8, 'bit')\"")
        sys.exit(1)

    print("DLL loaded successfully!")

    # Get the DLL base address
    handle = dll._handle
    print(f"DLL handle (base address): 0x{handle:08x}")

    # Calculate the offset of the table within the DLL image
    # VA = image_base + RVA
    # Ghidra image_base = 0x10000000
    # So RVA = 0x1003f9c0 - 0x10000000 = 0x0003f9c0
    rva_table = VA_TABLE - 0x10000000  # = 0x3f9c0
    
    # But when loaded, the DLL might be at a different base
    # The actual address = handle + RVA
    actual_addr = handle + rva_table
    print(f"RVA of table: 0x{rva_table:08x}")
    print(f"Actual address: 0x{actual_addr:08x}")

    # Optionally call get_connect_package to ensure init happened
    # (DllMain should already have run, but just in case)
    try:
        buf = ctypes.create_string_buffer(512)
        dll.get_connect_package(buf)
        print("get_connect_package called successfully (ensures init)")
    except Exception as e:
        print(f"get_connect_package call failed: {e} (table may still be valid from DllMain)")

    # Read the table from memory
    table = (ctypes.c_ubyte * TABLE_SIZE).from_address(actual_addr)
    table_bytes = bytes(table)

    # Check if it's all zeros (init didn't run)
    if all(b == 0 for b in table_bytes):
        print("\nWARNING: Table is all zeros! Init may not have run.")
        print("Trying to trigger init by calling an export...")
        
        # Try calling get_connect_package
        try:
            buf = ctypes.create_string_buffer(512)
            dll.get_connect_package(buf)
            # Re-read
            table = (ctypes.c_ubyte * TABLE_SIZE).from_address(actual_addr)
            table_bytes = bytes(table)
        except Exception as e:
            print(f"Failed: {e}")
    
    if all(b == 0 for b in table_bytes):
        print("\nERROR: Table is still all zeros after init attempts.")
        print("The init function may have a different image base or relocation.")
        
        # Try scanning memory around the handle for non-zero data
        print("\nScanning for table near expected offset...")
        for delta in range(-0x1000, 0x1000, 0x100):
            addr = actual_addr + delta
            try:
                probe = (ctypes.c_ubyte * 16).from_address(addr)
                probe_bytes = bytes(probe)
                if not all(b == 0 for b in probe_bytes):
                    print(f"  Non-zero data at 0x{addr:08x} (delta={delta:+d}): "
                          f"{' '.join(f'{b:02x}' for b in probe_bytes)}")
            except:
                pass
        sys.exit(1)

    # Success!
    print(f"\nTable extracted: {len(table_bytes)} bytes")
    print(f"First 32: {' '.join(f'{b:02x}' for b in table_bytes[:32])}")
    print(f"Last  16: {' '.join(f'{b:02x}' for b in table_bytes[-16:])}")

    # Non-zero count
    nz = sum(1 for b in table_bytes if b != 0)
    print(f"Non-zero bytes: {nz}/{len(table_bytes)}")

    # Save
    out_bin = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cipher_table.bin")
    with open(out_bin, "wb") as f:
        f.write(table_bytes)
    print(f"\nSaved to: {out_bin}")

    # Save as Python literal
    out_py = os.path.join(os.path.dirname(os.path.abspath(__file__)), "cipher_table.py")
    with open(out_py, "w") as f:
        f.write("# Auto-extracted from beelightLib.dll runtime memory\n")
        f.write(f"# DAT_1003f9c0, {len(table_bytes)} bytes\n")
        f.write("CIPHER_TABLE = bytes([\n")
        for i in range(0, len(table_bytes), 16):
            chunk = table_bytes[i:i + 16]
            f.write("    " + ", ".join(f"0x{b:02x}" for b in chunk) + ",\n")
        f.write("])\n")
    print(f"Saved to: {out_py}")


if __name__ == "__main__":
    main()
