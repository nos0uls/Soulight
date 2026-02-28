# -*- coding: utf-8 -*-
"""
dll_call_32bit_v3.py — Дополнительные тесты:
1. unpack_package (расшифровка) — проверить обратное преобразование
2. get_connect_package — для handshake
3. Изучить .NET framing: вызвать DLL и сравнить с wire captures

Также: попробовать вызвать get_scen_package с параметрами из реальных захватов
и посмотреть как .NET мог бы трансформировать пакеты.

Запускать 32-bit Python!
"""
import sys
import os
import ctypes
import struct
import time

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)
OUT_DIR = os.path.dirname(os.path.abspath(__file__))

# Runtime table RVA
RVA_TABLE = 0x0003f9c0
TABLE_SIZE = 0x209  # 521


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    ptr_size = struct.calcsize("P") * 8
    if ptr_size != 32:
        print("ERROR: Need 32-bit Python!")
        sys.exit(1)

    dll = ctypes.CDLL(DLL_PATH)
    handle = dll._handle
    print(f"DLL loaded at 0x{handle:08x}")

    # ================================================================
    # 1. unpack_package — test decryption
    # ================================================================
    print("\n=== unpack_package test ===")
    
    # First create a packet
    buf_enc = ctypes.create_string_buffer(256)
    dll.get_scen_package(0, 100, 255, 0, 0, 1, buf_enc)
    
    # Copy the encrypted packet
    pkt_raw = buf_enc.raw[:21]
    print(f"Encrypted: {' '.join(f'{b:02x}' for b in pkt_raw)}")
    
    # Now try unpack_package
    # Signature guess: int unpack_package(byte* packet, ...)
    # Let's try a few signatures
    buf_dec = ctypes.create_string_buffer(256)
    
    # Copy encrypted packet to new buffer for unpack
    for i in range(21):
        buf_dec[i] = pkt_raw[i]
    
    try:
        # Try: unpack_package(buf, length)
        result = dll.unpack_package(buf_dec, 21)
        print(f"unpack_package(buf, 21) returned: {result}")
        dec_raw = buf_dec.raw[:21]
        print(f"After unpack: {' '.join(f'{b:02x}' for b in dec_raw)}")
        # Check if bytes [11:] are now decrypted
        if dec_raw[11] == 0x02:  # cmd should be 2
            print("  ✓ Decryption looks correct! cmd=2")
    except Exception as e:
        print(f"  Failed with (buf, len): {e}")
    
    # ================================================================
    # 2. Check all global data areas for wire format clues
    # ================================================================
    print("\n=== Reading DAT_10041f10 and DAT_100407a0 after get_large_screendata_package ===")
    
    # These are intermediate buffers used by get_large_screendata_package
    rva_41f10 = 0x00041f10
    rva_407a0 = 0x000407a0
    rva_401c0 = 0x000401c0  # data buffer passed to FUN_10001880
    
    # First call get_large_screendata_package to fill buffers
    img_size = 1920 * 1080 * 3
    img = ctypes.create_string_buffer(img_size)
    for i in range(0, img_size, 3):
        img[i] = 255  # R
    
    buf_screen = ctypes.create_string_buffer(2048)
    dll.get_large_screendata_package(img, 1920, 1080, 3, buf_screen)
    
    # Read intermediate buffer DAT_100401c0 (data passed to FUN_10001880)
    addr_401c0 = handle + rva_401c0
    data_buf = (ctypes.c_ubyte * 1500).from_address(addr_401c0)
    data_bytes = bytes(data_buf)
    
    print(f"DAT_100401c0 (data buffer, 1500 bytes):")
    print(f"  First 40: {' '.join(f'{b:02x}' for b in data_bytes[:40])}")
    print(f"  Bytes 225-240: {' '.join(f'{b:02x}' for b in data_bytes[225:240])}")
    
    # This buffer contains the processed LED data BEFORE encryption
    # For RED: should contain the LED strip color data
    
    # Count 0xFF and 0x00 occurrences
    ff_count = sum(1 for b in data_bytes if b == 0xFF)
    zero_count = sum(1 for b in data_bytes if b == 0x00)
    print(f"  0xFF count: {ff_count}, 0x00 count: {zero_count}")
    
    # ================================================================
    # 3. Read DAT_10041f10 buffer (used in get_large_screendata_package)
    # ================================================================
    addr_41f10 = handle + rva_41f10
    buf_41f10 = (ctypes.c_ubyte * 0x5dc).from_address(addr_41f10)
    buf_41f10_bytes = bytes(buf_41f10)
    
    print(f"\nDAT_10041f10 (intermediate buffer, 0x5dc bytes):")
    print(f"  First 40: {' '.join(f'{b:02x}' for b in buf_41f10_bytes[:40])}")
    nz = sum(1 for b in buf_41f10_bytes if b != 0)
    print(f"  Non-zero: {nz}/{len(buf_41f10_bytes)}")
    
    # ================================================================
    # 4. Read DAT_100407a0 buffer
    # ================================================================
    addr_407a0 = handle + rva_407a0
    buf_407a0 = (ctypes.c_ubyte * 6000).from_address(addr_407a0)
    buf_407a0_bytes = bytes(buf_407a0)
    
    print(f"\nDAT_100407a0 (screen buffer, 6000 bytes):")
    print(f"  First 40: {' '.join(f'{b:02x}' for b in buf_407a0_bytes[:40])}")
    nz = sum(1 for b in buf_407a0_bytes if b != 0)
    print(f"  Non-zero: {nz}/{len(buf_407a0_bytes)}")
    
    # ================================================================
    # 5. Read global config values
    # ================================================================
    print("\n=== Global config values ===")
    globals_to_read = [
        (0x000401bc, "DAT_100401bc", 4),  # used in screendata
        (0x000424f0, "DAT_100424f0", 4),
        (0x000424ec, "DAT_100424ec", 4),
        (0x0004079c, "DAT_1004079c", 4),
    ]
    
    for rva, name, size in globals_to_read:
        addr = handle + rva
        val_arr = (ctypes.c_ubyte * size).from_address(addr)
        val_bytes = bytes(val_arr)
        val_int = int.from_bytes(val_bytes, 'little')
        print(f"  {name}: 0x{val_int:08x} ({val_int})")

    # ================================================================
    # 6. Send DLL packet directly over serial (test if controller accepts it)
    # ================================================================
    print("\n=== Generating test packets for serial ===")
    
    # Generate heartbeat-like connect package
    buf_conn = ctypes.create_string_buffer(256)
    dll.get_connect_package(buf_conn)
    conn_raw = buf_conn.raw
    last_nz = max((i for i in range(len(conn_raw)) if conn_raw[i] != 0), default=0)
    conn_pkt = conn_raw[:last_nz + 1]
    print(f"Connect packet: [{' '.join(f'{b:02x}' for b in conn_pkt)}]")
    
    # Generate multiple scen packets for testing
    colors = [
        ("RED",   255, 0,   0),
        ("GREEN", 0,   255, 0),
        ("BLUE",  0,   0,   255),
        ("WHITE", 255, 255, 255),
    ]
    
    for cname, r, g, b in colors:
        buf = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, r, g, b, 1, buf)
        raw = buf.raw
        last_nz = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
        pkt = raw[:last_nz + 1]
        
        # Save for later use
        fname = f"pkt_scen_{cname.lower()}.bin"
        with open(os.path.join(OUT_DIR, fname), "wb") as f:
            f.write(pkt)
        print(f"  {cname}: [{' '.join(f'{b:02x}' for b in pkt)}] -> {fname}")
    
    # ================================================================
    # 7. Table consistency check: call DLL multiple times, check table stays same
    # ================================================================
    print("\n=== Table consistency check ===")
    
    # Read table before
    table1 = bytes((ctypes.c_ubyte * TABLE_SIZE).from_address(handle + RVA_TABLE))
    
    # Make several calls
    for _ in range(10):
        buf = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf)
    
    # Read table after
    table2 = bytes((ctypes.c_ubyte * TABLE_SIZE).from_address(handle + RVA_TABLE))
    
    if table1 == table2:
        print("  ✓ Table is CONSTANT across calls (not modified by encryption)")
    else:
        diffs = sum(1 for a, b in zip(table1, table2) if a != b)
        print(f"  ✗ Table CHANGED! {diffs} bytes differ")
    
    print("\nDONE!")


if __name__ == "__main__":
    main()
