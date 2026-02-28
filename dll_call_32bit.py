# -*- coding: utf-8 -*-
"""
dll_call_32bit.py — Вызов beelightLib.dll через 32-bit Python ctypes.

ТРЕБОВАНИЯ: Запускать ТОЛЬКО через 32-bit Python!
Пример: "C:\Python312-32\python.exe" dll_call_32bit.py

Этот скрипт:
1. Загружает DLL (DllMain заполняет таблицу шифрования)
2. Дампит таблицу DAT_1003f9c0 (521 байт) из памяти
3. Вызывает get_connect_package и get_scen_package для получения эталонных пакетов
4. Сохраняет всё в файлы для анализа
"""
import sys
import os
import ctypes
import struct

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)

# Addresses from Ghidra (RVA = VA - 0x10000000)
RVA_TABLE = 0x0003f9c0       # DAT_1003f9c0, 521 bytes
RVA_MAGIC = 0x0003fbd0       # _DAT_1003fbd0 (magic "MgL")
TABLE_SIZE = 0x209            # 521

OUT_DIR = os.path.dirname(os.path.abspath(__file__))


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    # Check we're 32-bit
    ptr_size = struct.calcsize("P") * 8
    print(f"Python: {ptr_size}-bit")
    if ptr_size != 32:
        print("ERROR: This script MUST be run with 32-bit Python!")
        print("Download from: https://www.python.org/downloads/")
        print("Choose 'Windows installer (32-bit)'")
        print(f"Then run: path\\to\\python32.exe {os.path.abspath(__file__)}")
        sys.exit(1)

    print(f"Loading DLL: {DLL_PATH}")
    try:
        dll = ctypes.CDLL(DLL_PATH)
    except OSError as e:
        print(f"ERROR: {e}")
        sys.exit(1)

    handle = dll._handle
    print(f"DLL loaded at base: 0x{handle:08x}")

    # ================================================================
    # 1. Dump cipher table from memory
    # ================================================================
    table_addr = handle + RVA_TABLE
    print(f"\nReading cipher table at 0x{table_addr:08x} ({TABLE_SIZE} bytes)")

    table = (ctypes.c_ubyte * TABLE_SIZE).from_address(table_addr)
    table_bytes = bytes(table)

    nz = sum(1 for b in table_bytes if b != 0)
    print(f"Non-zero bytes: {nz}/{TABLE_SIZE}")

    if nz == 0:
        print("WARNING: Table is all zeros!")
        # Try triggering init by calling get_connect_package
        buf = ctypes.create_string_buffer(2048)
        try:
            dll.get_connect_package(buf)
            print("Called get_connect_package to trigger init")
        except Exception as e:
            print(f"Call failed: {e}")
        
        table = (ctypes.c_ubyte * TABLE_SIZE).from_address(table_addr)
        table_bytes = bytes(table)
        nz = sum(1 for b in table_bytes if b != 0)
        print(f"After init: Non-zero bytes: {nz}/{TABLE_SIZE}")

    print(f"First 32: {' '.join(f'{b:02x}' for b in table_bytes[:32])}")
    print(f"Last  16: {' '.join(f'{b:02x}' for b in table_bytes[-16:])}")

    # Save table
    table_bin_path = os.path.join(OUT_DIR, "cipher_table_runtime.bin")
    with open(table_bin_path, "wb") as f:
        f.write(table_bytes)
    print(f"Saved: {table_bin_path}")

    # Save as Python
    table_py_path = os.path.join(OUT_DIR, "cipher_table_runtime.py")
    with open(table_py_path, "w") as f:
        f.write("# Extracted from beelightLib.dll runtime memory\n")
        f.write(f"# Address: 0x{table_addr:08x}, {len(table_bytes)} bytes\n")
        f.write("CIPHER_TABLE = bytes([\n")
        for i in range(0, len(table_bytes), 16):
            chunk = table_bytes[i:i + 16]
            f.write("    " + ", ".join(f"0x{b:02x}" for b in chunk) + ",\n")
        f.write("])\n")
    print(f"Saved: {table_py_path}")

    # ================================================================
    # 2. Read magic bytes
    # ================================================================
    magic_addr = handle + RVA_MAGIC
    magic = (ctypes.c_ubyte * 8).from_address(magic_addr)
    magic_bytes = bytes(magic)
    print(f"\nMagic at 0x{magic_addr:08x}: {' '.join(f'{b:02x}' for b in magic_bytes)}")
    print(f"  As ASCII: {''.join(chr(b) if 32 <= b < 127 else '.' for b in magic_bytes)}")

    # ================================================================
    # 3. Call get_connect_package
    # ================================================================
    print("\n" + "=" * 60)
    print("get_connect_package")
    print("=" * 60)

    buf = ctypes.create_string_buffer(2048)
    dll.get_connect_package(buf)
    
    # Find actual length (look for where data ends)
    raw = buf.raw
    # The packet should start with magic 0x4D67
    # Find last non-zero byte (rough)
    last_nz = 0
    for i in range(len(raw) - 1, -1, -1):
        if raw[i] != 0:
            last_nz = i
            break
    pkt_len = last_nz + 1
    pkt = raw[:pkt_len]
    
    print(f"Output length (to last non-zero): {pkt_len}")
    print(f"First 40: {' '.join(f'{b:02x}' for b in pkt[:40])}")
    if pkt_len > 40:
        print(f"Last  20: {' '.join(f'{b:02x}' for b in pkt[-20:])}")

    # Save
    with open(os.path.join(OUT_DIR, "pkt_connect.bin"), "wb") as f:
        f.write(pkt)
    print(f"Saved: pkt_connect.bin")

    # ================================================================
    # 4. Call get_scen_package (RED color, full brightness)
    # ================================================================
    print("\n" + "=" * 60)
    print("get_scen_package(scenid=0, dimmer=100, R=255, G=0, B=0, power=1)")
    print("=" * 60)

    buf2 = ctypes.create_string_buffer(2048)
    # int get_scen_package(byte scenid, byte dimmer, byte red, byte green, byte blue, byte power, byte* pout)
    result = dll.get_scen_package(0, 100, 255, 0, 0, 1, buf2)
    print(f"Return value: {result}")

    raw2 = buf2.raw
    last_nz2 = 0
    for i in range(len(raw2) - 1, -1, -1):
        if raw2[i] != 0:
            last_nz2 = i
            break
    pkt2_len = last_nz2 + 1
    pkt2 = raw2[:pkt2_len]

    print(f"Output length: {pkt2_len}")
    print(f"Packet: {' '.join(f'{b:02x}' for b in pkt2[:60])}")

    with open(os.path.join(OUT_DIR, "pkt_scen_red.bin"), "wb") as f:
        f.write(pkt2)
    print(f"Saved: pkt_scen_red.bin")

    # ================================================================
    # 5. Call get_large_screendata_package (solid RED, 75 LEDs)
    # ================================================================
    print("\n" + "=" * 60)
    print("get_large_screendata_package (solid RED)")
    print("=" * 60)

    # Signature: int get_large_screendata_package(byte* pscreen, int row, int column, int channel, byte* pout)
    # pscreen = pointer to RGB pixel data (row * column * channel bytes)
    # For solid RED: all pixels = (255, 0, 0)
    # The DLL processes this into 75 LED colors and encrypts

    # Create a simple 10x10 RED image (channel=3)
    rows, cols, channels = 10, 10, 3
    img_size = rows * cols * channels
    img = ctypes.create_string_buffer(img_size)
    # Fill with RED (R=255, G=0, B=0)
    for i in range(0, img_size, 3):
        img[i] = 255  # R
        img[i + 1] = 0  # G
        img[i + 2] = 0  # B

    buf3 = ctypes.create_string_buffer(2048)
    result3 = dll.get_large_screendata_package(img, rows, cols, channels, buf3)
    print(f"Return value: {result3}")

    raw3 = buf3.raw
    last_nz3 = 0
    for i in range(len(raw3) - 1, -1, -1):
        if raw3[i] != 0:
            last_nz3 = i
            break
    pkt3_len = last_nz3 + 1
    pkt3 = raw3[:pkt3_len]

    print(f"Output length: {pkt3_len}")
    print(f"First 40: {' '.join(f'{b:02x}' for b in pkt3[:40])}")
    if pkt3_len > 40:
        print(f"Last  20: {' '.join(f'{b:02x}' for b in pkt3[-20:])}")

    with open(os.path.join(OUT_DIR, "pkt_screen_red.bin"), "wb") as f:
        f.write(pkt3)
    print(f"Saved: pkt_screen_red.bin")

    # ================================================================
    # 6. Call get_large_screenbuf_package_length
    # ================================================================
    print("\n" + "=" * 60)
    print("get_large_screenbuf_package_length")
    print("=" * 60)
    try:
        pkt_size = dll.get_large_screenbuf_package_length()
        print(f"Expected buffer size: {pkt_size}")
    except Exception as e:
        print(f"Call failed: {e}")

    # ================================================================
    # 7. Multiple calls to check randomness
    # ================================================================
    print("\n" + "=" * 60)
    print("Multiple get_scen_package calls (checking randomness)")
    print("=" * 60)
    for i in range(5):
        buf_m = ctypes.create_string_buffer(2048)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf_m)
        raw_m = buf_m.raw
        # Find length
        for j in range(len(raw_m) - 1, -1, -1):
            if raw_m[j] != 0:
                break
        pkt_m = raw_m[:j + 1]
        print(f"  Call {i}: len={len(pkt_m)} [{' '.join(f'{b:02x}' for b in pkt_m[:25])}]")

    print("\n\nDONE! Check output files in:", OUT_DIR)


if __name__ == "__main__":
    main()
