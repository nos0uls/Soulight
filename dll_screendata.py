# -*- coding: utf-8 -*-
"""
dll_screendata.py - Figure out correct signature for get_large_screendata_package
and send MgL color data to controller.

Key finding: controller accepts raw MgL format!

Run with 32-bit Python!
"""
import sys, os, ctypes, struct, time

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll")

try:
    import serial
except ImportError:
    print("pip install pyserial"); sys.exit(1)

COM_PORT = "COM7"
BAUD = 500000


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    if struct.calcsize("P") * 8 != 32:
        print("Need 32-bit Python!"); return

    dll = ctypes.CDLL(DLL_PATH)
    print("DLL loaded")

    # ================================================================
    # 1. get_large_screenbuf_package_length
    # ================================================================
    print(f"\n{'='*60}")
    print("1. get_large_screenbuf_package_length")
    print(f"{'='*60}")

    for n_leds in [1, 10, 50, 75, 100, 150]:
        try:
            ret = dll.get_large_screenbuf_package_length(n_leds)
            print(f"  n_leds={n_leds:3d} -> buf_len={ret}")
        except Exception as e:
            print(f"  n_leds={n_leds:3d} -> ERROR: {e}")

    # Also try with no args
    try:
        ret = dll.get_large_screenbuf_package_length()
        print(f"  no args -> {ret}")
    except:
        pass

    # ================================================================
    # 2. get_middle counterparts
    # ================================================================
    print(f"\n{'='*60}")
    print("2. get_middle_screenbuf_package_length")
    print(f"{'='*60}")

    for n_leds in [1, 10, 50, 75]:
        try:
            ret = dll.get_middle_screenbuf_package_length(n_leds)
            print(f"  n_leds={n_leds:3d} -> buf_len={ret}")
        except Exception as e:
            print(f"  n_leds={n_leds:3d} -> ERROR: {e}")

    # ================================================================
    # 3. get_screen_size_setting_package
    # ================================================================
    print(f"\n{'='*60}")
    print("3. get_screen_size_setting_package")
    print(f"{'='*60}")

    buf = ctypes.create_string_buffer(256)
    for n_leds in [75]:
        try:
            ret = dll.get_screen_size_setting_package(n_leds, buf)
            end = max((i for i in range(len(buf.raw)) if buf.raw[i]), default=0) + 1
            data = buf.raw[:end]
            print(f"  n_leds={n_leds} ret={ret} len={end} data={data.hex()}")
        except Exception as e:
            print(f"  n_leds={n_leds} ERROR: {e}")

    # ================================================================
    # 4. Try various signatures for get_large_screendata_package
    # ================================================================
    print(f"\n{'='*60}")
    print("4. get_large_screendata_package signature probing")
    print(f"{'='*60}")

    # Prepare pixel data: 75 LEDs RED
    NUM_LEDS = 75
    pix_arr = (ctypes.c_ubyte * (NUM_LEDS * 3))()
    for i in range(NUM_LEDS):
        pix_arr[i*3] = 255  # R
        pix_arr[i*3+1] = 0  # G
        pix_arr[i*3+2] = 0  # B

    # Get expected buffer length
    try:
        expected_len = dll.get_large_screenbuf_package_length(NUM_LEDS)
        print(f"  Expected output buffer: {expected_len} bytes")
    except:
        expected_len = 2048

    out_buf = ctypes.create_string_buffer(max(expected_len + 100, 2048))

    # Signature attempts
    attempts = [
        ("(0, num_leds, pix, pix_len, buf)", 
         lambda: dll.get_large_screendata_package(0, NUM_LEDS, pix_arr, NUM_LEDS*3, out_buf)),
        ("(num_leds, pix, pix_len, buf)",
         lambda: dll.get_large_screendata_package(NUM_LEDS, pix_arr, NUM_LEDS*3, out_buf)),
        ("(num_leds, pix, buf)",
         lambda: dll.get_large_screendata_package(NUM_LEDS, pix_arr, out_buf)),
        ("(pix, pix_len, num_leds, buf)",
         lambda: dll.get_large_screendata_package(pix_arr, NUM_LEDS*3, NUM_LEDS, out_buf)),
        ("(pix, num_leds, buf)",
         lambda: dll.get_large_screendata_package(pix_arr, NUM_LEDS, out_buf)),
        ("(0, num_leds, pix, buf)",
         lambda: dll.get_large_screendata_package(0, NUM_LEDS, pix_arr, out_buf)),
        ("(buf, num_leds, pix, pix_len)",
         lambda: dll.get_large_screendata_package(out_buf, NUM_LEDS, pix_arr, NUM_LEDS*3)),
    ]

    for desc, fn in attempts:
        out_buf2 = ctypes.create_string_buffer(max(expected_len + 100, 2048))
        try:
            ret = fn()
            raw = out_buf.raw
            end = 0
            for i in range(min(expected_len + 50, len(raw))-1, -1, -1):
                if raw[i] != 0:
                    end = i + 1
                    break
            print(f"\n  {desc}:")
            print(f"    ret={ret} end={end}")
            if end > 0:
                print(f"    first40: {raw[:min(40,end)].hex()}")
                is_mgl = raw[:3] == b'MgL'
                is_wire = raw[:3] == b'\x55\xAA\x5A'
                if is_mgl:
                    plen = raw[4]
                    print(f"    MgL! payload_len={plen}")
                elif is_wire:
                    plen = raw[3]
                    print(f"    Wire! payload_len={plen}")
                # Check if 0xFF appears (our RED value)
                ff_count = sum(1 for b in raw[:end] if b == 0xFF)
                print(f"    0xFF count: {ff_count}")
        except Exception as e:
            print(f"\n  {desc}: ERROR: {e}")

    # ================================================================
    # 5. Try get_large_screendata_package_ex
    # ================================================================
    print(f"\n{'='*60}")
    print("5. get_large_screendata_package_ex")
    print(f"{'='*60}")

    out_buf3 = ctypes.create_string_buffer(2048)
    attempts_ex = [
        ("(0, num_leds, pix, pix_len, buf)",
         lambda: dll.get_large_screendata_package_ex(0, NUM_LEDS, pix_arr, NUM_LEDS*3, out_buf3)),
        ("(num_leds, pix, pix_len, buf)",
         lambda: dll.get_large_screendata_package_ex(NUM_LEDS, pix_arr, NUM_LEDS*3, out_buf3)),
        ("(num_leds, pix, buf)",
         lambda: dll.get_large_screendata_package_ex(NUM_LEDS, pix_arr, out_buf3)),
        ("(0, num_leds, pix, pix_len, 0, buf)",
         lambda: dll.get_large_screendata_package_ex(0, NUM_LEDS, pix_arr, NUM_LEDS*3, 0, out_buf3)),
    ]

    for desc, fn in attempts_ex:
        out_buf3 = ctypes.create_string_buffer(2048)
        try:
            ret = fn()
            raw = out_buf3.raw
            end = 0
            for i in range(min(1600, len(raw))-1, -1, -1):
                if raw[i] != 0:
                    end = i + 1
                    break
            print(f"\n  {desc}:")
            print(f"    ret={ret} end={end}")
            if end > 0:
                print(f"    first40: {raw[:min(40,end)].hex()}")
        except Exception as e:
            print(f"\n  {desc}: ERROR: {e}")

    # ================================================================
    # 6. get_setting_package (might set LED count)
    # ================================================================
    print(f"\n{'='*60}")
    print("6. get_setting_package / get_eco_setting_package")
    print(f"{'='*60}")

    for func_name in ['get_setting_package', 'get_eco_setting_package']:
        buf4 = ctypes.create_string_buffer(256)
        func = getattr(dll, func_name)
        for args_desc, args in [
            ("(75, buf)", (75, buf4)),
            ("(buf)", (buf4,)),
            ("(0, 75, buf)", (0, 75, buf4)),
        ]:
            buf4 = ctypes.create_string_buffer(256)
            try:
                ret = func(*args)
                raw = buf4.raw
                end = max((i for i in range(len(raw)) if raw[i]), default=0) + 1
                print(f"  {func_name}{args_desc}: ret={ret} len={end} data={raw[:min(30,end)].hex()}")
            except Exception as e:
                print(f"  {func_name}{args_desc}: ERROR: {e}")

    # ================================================================
    # 7. get_top_start_line / get_bottom_start_line
    # ================================================================
    print(f"\n{'='*60}")
    print("7. start line functions")
    print(f"{'='*60}")

    for func_name in ['get_top_start_line', 'get_bottom_start_line']:
        func = getattr(dll, func_name)
        for args in [(), (75,), (0,)]:
            try:
                ret = func(*args)
                print(f"  {func_name}{args}: ret={ret}")
            except Exception as e:
                print(f"  {func_name}{args}: ERROR: {e}")

    # ================================================================
    # 8. SEND TO CONTROLLER: MgL connect + scen RED loop
    # ================================================================
    print(f"\n{'='*60}")
    print("8. SEND MgL: connect + scen(RED) to controller")
    print(f"{'='*60}")

    try:
        ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    except serial.SerialException:
        print("Cannot open COM7")
        return

    ser.dtr = True; ser.rts = True
    time.sleep(0.3); ser.read(4096)

    # Send connect
    conn = ctypes.create_string_buffer(512)
    dll.get_connect_package(conn)
    end = max((i for i in range(len(conn.raw)) if conn.raw[i]), default=0) + 1
    mgl_conn = conn.raw[:end]
    ser.write(mgl_conn); ser.flush()
    time.sleep(0.5)
    resp = ser.read(4096)
    print(f"  MgL connect sent ({end}b), response: {len(resp)}b")
    if resp:
        print(f"    resp first40: {resp[:40].hex()}")

    # Send scen RED for 15 seconds
    print("  Sending scen(RED) for 15 seconds...")
    start = time.time()
    n = 0
    while time.time() - start < 15.0:
        buf5 = ctypes.create_string_buffer(512)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf5)
        end5 = max((i for i in range(len(buf5.raw)) if buf5.raw[i]), default=0) + 1
        ser.write(buf5.raw[:end5]); ser.flush()
        time.sleep(0.08)
        n += 1

    print(f"  Sent {n} MgL scen RED. LED RED?")
    time.sleep(1)

    # Switch to GREEN
    print("  Sending scen(GREEN) for 5 seconds...")
    start = time.time()
    n = 0
    while time.time() - start < 5.0:
        buf6 = ctypes.create_string_buffer(512)
        dll.get_scen_package(0, 100, 0, 255, 0, 1, buf6)
        end6 = max((i for i in range(len(buf6.raw)) if buf6.raw[i]), default=0) + 1
        ser.write(buf6.raw[:end6]); ser.flush()
        time.sleep(0.08)
        n += 1

    print(f"  Sent {n} MgL scen GREEN. LED GREEN?")
    time.sleep(1)

    # Switch to BLUE
    print("  Sending scen(BLUE) for 5 seconds...")
    start = time.time()
    n = 0
    while time.time() - start < 5.0:
        buf7 = ctypes.create_string_buffer(512)
        dll.get_scen_package(0, 100, 0, 0, 255, 1, buf7)
        end7 = max((i for i in range(len(buf7.raw)) if buf7.raw[i]), default=0) + 1
        ser.write(buf7.raw[:end7]); ser.flush()
        time.sleep(0.08)
        n += 1

    print(f"  Sent {n} MgL scen BLUE. LED BLUE?")

    # OFF
    print("  Sending OFF...")
    for _ in range(20):
        buf8 = ctypes.create_string_buffer(512)
        dll.get_scen_package(0, 0, 0, 0, 0, 0, buf8)
        end8 = max((i for i in range(len(buf8.raw)) if buf8.raw[i]), default=0) + 1
        ser.write(buf8.raw[:end8]); ser.flush()
        time.sleep(0.08)

    ser.close()
    print("\nDone! Report: RED? GREEN? BLUE? OFF?")


if __name__ == "__main__":
    main()
