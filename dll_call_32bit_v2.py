# -*- coding: utf-8 -*-
"""
dll_call_32bit_v2.py — Продолжение: вызвать get_large_screendata_package 
и другие экспорты с правильными параметрами.

Запускать 32-bit Python!
"""
import sys
import os
import ctypes
import struct

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)
OUT_DIR = os.path.dirname(os.path.abspath(__file__))


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
    # 1. get_large_screenbuf_package_length 
    # ================================================================
    print("\n=== get_large_screenbuf_package_length ===")
    try:
        pkg_len = dll.get_large_screenbuf_package_length()
        print(f"Expected buffer size: {pkg_len}")
    except Exception as e:
        print(f"Failed: {e}")
        pkg_len = 2048

    # ================================================================
    # 2. get_large_screendata_package with realistic image sizes
    # ================================================================
    # The DLL processes screen captures. Let's try standard resolutions.
    # The app captures a portion of the screen and sends it to the DLL.
    # Typical: 1920x1080, but also smaller.
    # 
    # The divide-by-zero likely happens when calculating LED zones from image dimensions.
    # Try larger images.

    sizes_to_try = [
        (1920, 1080, 3, "1920x1080"),
        (800, 600, 3, "800x600"),
        (320, 240, 3, "320x240"),
        (100, 100, 3, "100x100"),
        (75, 1, 3, "75x1 (one row of LEDs)"),
        (150, 1, 3, "150x1"),
        (1920, 1, 3, "1920x1 (one scanline)"),
    ]

    for rows, cols, ch, desc in sizes_to_try:
        print(f"\n=== get_large_screendata_package({desc}) ===")
        img_size = rows * cols * ch
        img = ctypes.create_string_buffer(img_size)
        
        # Fill with solid RED
        for i in range(0, img_size, 3):
            img[i] = 255   # R
            img[i+1] = 0   # G
            img[i+2] = 0   # B

        buf = ctypes.create_string_buffer(max(pkg_len + 100, 4096))
        
        try:
            # Signature: get_large_screendata_package(byte* pscreen, int row, int column, int channel, byte* pout)
            result = dll.get_large_screendata_package(img, rows, cols, ch, buf)
            print(f"  Return: {result}")
            
            if result == 1:
                raw = buf.raw
                # Find actual length
                last_nz = 0
                for j in range(len(raw) - 1, -1, -1):
                    if raw[j] != 0:
                        last_nz = j
                        break
                pkt_len = last_nz + 1
                pkt = raw[:pkt_len]
                
                print(f"  Output length: {pkt_len}")
                print(f"  First 40: {' '.join(f'{b:02x}' for b in pkt[:40])}")
                if pkt_len > 40:
                    print(f"  Bytes 40-60: {' '.join(f'{b:02x}' for b in pkt[40:60])}")
                    print(f"  Last  20: {' '.join(f'{b:02x}' for b in pkt[-20:])}")
                
                # Check magic
                if pkt[0:3] == b'\x4d\x67\x4c':
                    print(f"  *** Magic MgL confirmed! ***")
                
                # Save first successful one
                out_path = os.path.join(OUT_DIR, f"pkt_screen_{desc.split('(')[0].strip()}.bin")
                with open(out_path, "wb") as f:
                    f.write(pkt)
                print(f"  Saved: {out_path}")
                
                # Don't try remaining sizes, we got one working
                break
            else:
                print(f"  Returned {result} (likely failure)")
                
        except OSError as e:
            print(f"  CRASH: {e}")
            # Re-load DLL for next attempt
            dll = ctypes.CDLL(DLL_PATH)
            continue

    # ================================================================
    # 3. Try get_large_screendata_package_ex if it exists
    # ================================================================
    print(f"\n=== Trying _ex variant ===")
    try:
        # Check if the export exists
        func = getattr(dll, 'get_large_screendata_package_ex', None)
        if func is None:
            # Try explicit
            func = dll.get_large_screendata_package_ex
        
        # Try with 1920x1080
        img_size = 1920 * 1080 * 3
        img = ctypes.create_string_buffer(img_size)
        for i in range(0, img_size, 3):
            img[i] = 0     # R=0 (BLACK for easy verification)
            img[i+1] = 0   # G
            img[i+2] = 0   # B
        
        buf = ctypes.create_string_buffer(4096)
        result = func(img, 1920, 1080, 3, buf)
        print(f"  Return: {result}")
        
        if result == 1:
            raw = buf.raw
            last_nz = 0
            for j in range(len(raw) - 1, -1, -1):
                if raw[j] != 0:
                    last_nz = j
                    break
            pkt = raw[:last_nz + 1]
            print(f"  Output length: {len(pkt)}")
            print(f"  First 40: {' '.join(f'{b:02x}' for b in pkt[:40])}")
            
            with open(os.path.join(OUT_DIR, "pkt_screen_ex_black.bin"), "wb") as f:
                f.write(pkt)
    except (AttributeError, OSError) as e:
        print(f"  Failed: {e}")

    # ================================================================
    # 4. List all DLL exports
    # ================================================================
    print(f"\n=== All DLL exports ===")
    exports = [
        "get_connect_package",
        "get_scen_package", 
        "get_large_screendata_package",
        "get_large_screendata_package_ex",
        "get_large_screenbuf_package_length",
        "get_audiodata_package",
        "get_screenbuf_package_length",
        "get_screendata_package",
        "unpack_package",
        "set_crc_check",
        "get_device_string",
    ]
    
    for name in exports:
        try:
            func = getattr(dll, name)
            print(f"  {name}: OK (at 0x{ctypes.cast(func, ctypes.c_void_p).value:08x})")
        except AttributeError:
            print(f"  {name}: NOT FOUND")

    # ================================================================
    # 5. get_device_string — might give us the device key
    # ================================================================
    print(f"\n=== get_device_string ===")
    try:
        buf_dev = ctypes.create_string_buffer(256)
        dll.get_device_string(buf_dev)
        dev_str = buf_dev.value.decode('ascii', errors='replace')
        print(f"  Device string: '{dev_str}'")
    except Exception as e:
        print(f"  Failed: {e}")

    # ================================================================
    # 6. Multiple get_scen_package calls — analyze structure
    # ================================================================
    print(f"\n=== get_scen_package analysis ===")
    print("Calling 5 times with same params (RED, brightness=100):")
    for i in range(5):
        buf = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf)
        raw = buf.raw
        last_nz = 0
        for j in range(len(raw) - 1, -1, -1):
            if raw[j] != 0:
                last_nz = j
                break
        pkt = raw[:last_nz + 1]
        hex_str = ' '.join(f'{b:02x}' for b in pkt)
        print(f"  [{i}] len={len(pkt):3d} [{hex_str}]")
    
    # BLACK
    print("\nCalling with BLACK (R=0,G=0,B=0):")
    for i in range(3):
        buf = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, 0, 0, 0, 1, buf)
        raw = buf.raw
        last_nz = 0
        for j in range(len(raw) - 1, -1, -1):
            if raw[j] != 0:
                last_nz = j
                break
        pkt = raw[:last_nz + 1]
        hex_str = ' '.join(f'{b:02x}' for b in pkt)
        print(f"  [{i}] len={len(pkt):3d} [{hex_str}]")

    print("\nDONE!")


if __name__ == "__main__":
    main()
