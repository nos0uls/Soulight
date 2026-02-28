# -*- coding: utf-8 -*-
"""
dll_init_state.py - Initialize DLL state by feeding controller response
into unpack_package, then try get_large_screendata_package.

The theory: the DLL maintains internal session state. After sending connect
and receiving the controller's response, unpack_package processes that response
and initializes internal variables (like LED count, session keys, etc).
Only after that can get_large_screendata_package generate valid output.

Run with 32-bit Python!
"""
import sys, os, ctypes, struct, time

# Установка кодировки вывода для поддержки UTF-8 в консоли
os.environ.setdefault('PYTHONIOENCODING', 'utf-8')
import io
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8', errors='replace')

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll")

try:
    import serial
except ImportError:
    print("pip install pyserial"); sys.exit(1)

COM_PORT = "COM7"
BAUD = 500000


def extract_mgl_payload(raw):
    """Get payload from MgL packet."""
    if raw[:3] == b'MgL':
        plen = raw[4]
        return raw[5:5+plen]
    return raw


def extract_wire_packets(raw):
    """Extract 55AA5A framed packets from raw data."""
    pkts = []
    i = 0
    while i < len(raw) - 4:
        if raw[i] == 0x55 and raw[i+1] == 0xAA and raw[i+2] == 0x5A:
            plen = raw[i+3]
            attr = raw[i+4]
            payload = raw[i+5:i+5+plen]
            pkts.append({'len': plen, 'attr': attr, 'payload': payload,
                         'raw': raw[i:i+5+plen]})
            i += 5 + plen
        else:
            i += 1
    return pkts


def main():
    print("Starting dll_init_state.py...")
    sys.stdout.flush()

    if struct.calcsize("P") * 8 != 32:
        print("Need 32-bit Python!"); return

    print(f"Loading DLL from: {DLL_PATH}")
    if not os.path.exists(DLL_PATH):
        print("DLL not found!"); return

    dll = ctypes.CDLL(DLL_PATH)
    print("DLL loaded")
    sys.stdout.flush()

    # ================================================================
    # 1. Open serial, send MgL connect, get response
    # ================================================================
    print(f"\n{'='*60}")
    print("1. MgL connect + capture response")
    print(f"{'='*60}")

    print(f"Opening {COM_PORT}...")
    sys.stdout.flush()
    ser = None
    for attempt in range(5):
        try:
            ser = serial.Serial(COM_PORT, BAUD, timeout=1.0)
            print(f"  Opened {COM_PORT} on attempt {attempt+1}")
            break
        except serial.SerialException as e:
            print(f"  Attempt {attempt+1}/5 failed: {e}")
            time.sleep(2.0)
    if ser is None:
        print("Cannot open COM7. Close Beelight or Serial Monitor first.")
        return

    ser.dtr = True; ser.rts = True
    time.sleep(0.3); ser.read(4096)

    # Send connect
    buf_conn = ctypes.create_string_buffer(512)
    dll.get_connect_package(buf_conn)
    end_conn = max((i for i in range(len(buf_conn.raw)) if buf_conn.raw[i]), default=0) + 1
    mgl_conn = buf_conn.raw[:end_conn]
    print(f"  Sending MgL connect: {mgl_conn.hex()}")
    ser.write(mgl_conn); ser.flush()

    time.sleep(1.0)
    resp = ser.read(4096)
    print(f"  Response: {len(resp)} bytes")

    if not resp:
        print("  No response! Trying again...")
        ser.write(mgl_conn); ser.flush()
        time.sleep(1.0)
        resp = ser.read(4096)
        print(f"  Response: {len(resp)} bytes")

    if not resp:
        print("  Still no response. Exiting.")
        ser.close()
        return

    # Parse response packets
    resp_pkts = extract_wire_packets(resp)
    print(f"  Parsed {len(resp_pkts)} wire packets from response:")
    for j, p in enumerate(resp_pkts):
        print(f"    pkt[{j}]: len={p['len']} attr={p['attr']:02x} "
              f"payload={p['payload'].hex()[:60]}")

    # ================================================================
    # 2. Feed response into unpack_package
    # ================================================================
    print(f"\n{'='*60}")
    print("2. Feed response into unpack_package")
    print(f"{'='*60}")

    # Try feeding each response packet into unpack_package
    for j, p in enumerate(resp_pkts):
        # Try various signatures
        wire_pkt = p['raw']
        payload = p['payload']

        # Convert wire packet to MgL format for unpack
        mgl_pkt = b'MgL\x00' + bytes([p['len']]) + payload

        for sig_desc, try_data in [
            ("raw wire bytes", wire_pkt),
            ("MgL-wrapped", mgl_pkt),
            ("payload only", payload),
        ]:
            out_buf = ctypes.create_string_buffer(512)
            in_buf = ctypes.create_string_buffer(try_data)
            try:
                ret = dll.unpack_package(in_buf, len(try_data), out_buf)
                out_raw = out_buf.raw
                out_end = 0
                for k in range(len(out_raw)-1, -1, -1):
                    if out_raw[k] != 0:
                        out_end = k + 1
                        break
                print(f"  pkt[{j}] {sig_desc}: ret={ret} out_len={out_end}")
                if out_end > 0:
                    print(f"    output: {out_raw[:min(40,out_end)].hex()}")
            except Exception as e:
                print(f"  pkt[{j}] {sig_desc}: ERROR: {e}")

    # Also try feeding the ENTIRE response as one blob
    print("\n  Feeding entire response:")
    for sig_desc, try_data in [
        ("full raw", resp),
    ]:
        out_buf = ctypes.create_string_buffer(1024)
        in_buf = ctypes.create_string_buffer(try_data)
        try:
            ret = dll.unpack_package(in_buf, len(try_data), out_buf)
            out_raw = out_buf.raw
            out_end = max((k for k in range(len(out_raw)) if out_raw[k]), default=0) + 1
            print(f"  {sig_desc}: ret={ret} out_len={out_end}")
            if out_end > 1:
                print(f"    output: {out_raw[:min(40,out_end)].hex()}")
        except Exception as e:
            print(f"  {sig_desc}: ERROR: {e}")

    # ================================================================
    # 3. Try unpack_package with 2-arg signature
    # ================================================================
    print(f"\n{'='*60}")
    print("3. unpack_package 2-arg signatures")
    print(f"{'='*60}")

    for j, p in enumerate(resp_pkts[:3]):
        wire_pkt = p['raw']
        mgl_pkt = b'MgL\x00' + bytes([p['len']]) + p['payload']

        for sig_desc, try_data in [
            ("MgL 2-arg", mgl_pkt),
            ("wire 2-arg", wire_pkt),
        ]:
            out_buf = ctypes.create_string_buffer(512)
            in_buf = ctypes.create_string_buffer(try_data)
            try:
                ret = dll.unpack_package(in_buf, out_buf)
                out_raw = out_buf.raw
                out_end = max((k for k in range(len(out_raw)) if out_raw[k]), default=0) + 1
                print(f"  pkt[{j}] {sig_desc}: ret={ret} out_len={out_end}")
                if out_end > 1:
                    print(f"    output: {out_raw[:min(40,out_end)].hex()}")
            except Exception as e:
                print(f"  pkt[{j}] {sig_desc}: ERROR: {e}")

    # ================================================================
    # 4. After unpack, try get_large_screendata_package again
    # ================================================================
    print(f"\n{'='*60}")
    print("4. get_large_screendata_package after unpack")
    print(f"{'='*60}")

    NUM_LEDS = 75
    pix = (ctypes.c_ubyte * (NUM_LEDS * 3))()
    for i in range(NUM_LEDS):
        pix[i*3] = 255; pix[i*3+1] = 0; pix[i*3+2] = 0

    out_sd = ctypes.create_string_buffer(2048)
    ret = dll.get_large_screendata_package(0, NUM_LEDS, pix, NUM_LEDS*3, out_sd)
    raw_sd = out_sd.raw
    end_sd = 0
    for k in range(min(1600, len(raw_sd))-1, -1, -1):
        if raw_sd[k] != 0:
            end_sd = k + 1
            break
    print(f"  ret={ret} end={end_sd}")
    if end_sd > 0:
        print(f"  first40: {raw_sd[:min(40,end_sd)].hex()}")
        is_mgl = raw_sd[:3] == b'MgL'
        print(f"  Is MgL: {is_mgl}")
    else:
        print("  Still empty. DLL state not initialized by unpack.")

    # ================================================================
    # 5. Try calling DLL functions in the order the .NET app would
    # ================================================================
    print(f"\n{'='*60}")
    print("5. Simulate .NET app sequence")
    print(f"{'='*60}")

    # The .NET app might call:
    # 1. get_connect_package -> send -> receive response -> unpack
    # 2. get_screen_size_setting_package(num_leds) -> send
    # 3. get_large_screendata_package

    # Try screen_size_setting with pointer arg
    print("  Trying get_screen_size_setting_package with pointer...")
    size_buf = ctypes.create_string_buffer(256)
    size_val = ctypes.c_int(75)
    try:
        ret = dll.get_screen_size_setting_package(ctypes.byref(size_val), size_buf)
        end2 = max((k for k in range(len(size_buf.raw)) if size_buf.raw[k]), default=0) + 1
        print(f"    ret={ret} len={end2} data={size_buf.raw[:min(30,end2)].hex()}")
    except Exception as e:
        print(f"    ERROR: {e}")

    # Try with different arg order
    try:
        ret = dll.get_screen_size_setting_package(size_buf, 75)
        end3 = max((k for k in range(len(size_buf.raw)) if size_buf.raw[k]), default=0) + 1
        print(f"    (buf, 75) ret={ret} len={end3} data={size_buf.raw[:min(30,end3)].hex()}")
    except Exception as e:
        print(f"    (buf, 75) ERROR: {e}")

    # Try get_setting_package which might set LED count
    set_buf = ctypes.create_string_buffer(256)
    for args in [(75, 0, set_buf), (0, 75, 0, set_buf), (75, 1, set_buf)]:
        set_buf2 = ctypes.create_string_buffer(256)
        try:
            ret = dll.get_setting_package(*args)
            end4 = max((k for k in range(len(set_buf2.raw)) if set_buf2.raw[k]), default=0) + 1
            # Check the buffer we passed
            print(f"    get_setting_package{args[:2]}: ret={ret}")
        except Exception as e:
            print(f"    get_setting_package{args[:2]}: ERROR: {e}")

    # ================================================================
    # 6. Keep connection alive with scen while testing
    # ================================================================
    print(f"\n{'='*60}")
    print("6. Continuous scen RED (check if LED works)")
    print(f"{'='*60}")

    start = time.time()
    n = 0
    while time.time() - start < 10.0:
        buf_s = ctypes.create_string_buffer(512)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf_s)
        end_s = max((k for k in range(len(buf_s.raw)) if buf_s.raw[k]), default=0) + 1
        ser.write(buf_s.raw[:end_s]); ser.flush()
        time.sleep(0.08)
        n += 1

        # Read any incoming data
        if n % 10 == 0:
            incoming = ser.read(1024)
            if incoming:
                print(f"  [{n}] incoming: {len(incoming)}b")

    print(f"  Sent {n} MgL scen RED packets. LED RED?")

    ser.close()
    print("\nDone!")


if __name__ == "__main__":
    main()
