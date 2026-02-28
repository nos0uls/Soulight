# -*- coding: utf-8 -*-
"""
dll_direct.py - Use DLL to generate ALL packets, convert MgL->wire, send.

MgL format: "MgL\x00" + len + payload
Wire format: "\x55\xAA\x5A" + len + "\x00" + payload

The DLL generates properly encrypted packets. We just need to re-frame them.

Also tries sending raw MgL (in case controller accepts both formats).

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


def mgl_to_wire(mgl_data):
    """Convert MgL packet to wire format (55AA5A framing)."""
    if mgl_data[:3] != b'MgL':
        return None
    # MgL: [4d 67 4c 00] [len] [payload...]
    plen = mgl_data[4]
    payload = mgl_data[5:5+plen]
    # Wire: [55 AA 5A] [len] [00] [payload...]
    return bytes([0x55, 0xAA, 0x5A, plen, 0x00]) + payload


def dll_get_connect(dll):
    """Get connect package from DLL."""
    buf = ctypes.create_string_buffer(512)
    dll.get_connect_package(buf)
    raw = buf.raw
    end = max((i for i in range(len(raw)) if raw[i]), default=0) + 1
    return raw[:end]


def dll_get_scen(dll, scenid, dimmer, r, g, b, power):
    """Get scen package from DLL."""
    buf = ctypes.create_string_buffer(512)
    dll.get_scen_package(scenid, dimmer, r, g, b, power, buf)
    raw = buf.raw
    end = max((i for i in range(len(raw)) if raw[i]), default=0) + 1
    return raw[:end]


def dll_get_screendata(dll, num_leds, pixels):
    """Try to get large screendata package from DLL."""
    # pixels should be ctypes array of R,G,B bytes
    buf = ctypes.create_string_buffer(2048)
    try:
        ret = dll.get_large_screendata_package(1, num_leds, pixels, len(pixels), buf)
        raw = buf.raw
        end = max((i for i in range(min(1600, len(raw))) if raw[i]), default=0) + 1
        return raw[:end], ret
    except Exception as e:
        return None, str(e)


def list_dll_exports(dll_path):
    """List DLL exports using ctypes/PE."""
    try:
        import ctypes.wintypes
        import struct as st
        with open(dll_path, "rb") as f:
            data = f.read()
        
        # PE header
        pe_offset = st.unpack_from("<I", data, 0x3C)[0]
        # Export directory RVA
        export_rva = st.unpack_from("<I", data, pe_offset + 0x78)[0]
        export_size = st.unpack_from("<I", data, pe_offset + 0x7C)[0]
        
        if export_rva == 0:
            return ["No exports"]
        
        # Find section containing export directory
        num_sections = st.unpack_from("<H", data, pe_offset + 6)[0]
        opt_header_size = st.unpack_from("<H", data, pe_offset + 0x14)[0]
        sections_offset = pe_offset + 0x18 + opt_header_size
        
        file_offset = 0
        for i in range(num_sections):
            sec = sections_offset + i * 40
            sec_rva = st.unpack_from("<I", data, sec + 12)[0]
            sec_size = st.unpack_from("<I", data, sec + 8)[0]
            sec_raw = st.unpack_from("<I", data, sec + 20)[0]
            if sec_rva <= export_rva < sec_rva + sec_size:
                file_offset = export_rva - sec_rva + sec_raw
                delta = sec_raw - sec_rva
                break
        
        if file_offset == 0:
            return ["Cannot find export section"]
        
        num_names = st.unpack_from("<I", data, file_offset + 24)[0]
        names_rva = st.unpack_from("<I", data, file_offset + 32)[0]
        
        names = []
        for i in range(num_names):
            name_rva = st.unpack_from("<I", data, names_rva + delta + i * 4)[0]
            name_offset = name_rva + delta
            end = data.index(b'\x00', name_offset)
            names.append(data[name_offset:end].decode('ascii', errors='replace'))
        
        return names
    except Exception as e:
        return [f"Error: {e}"]


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    if struct.calcsize("P") * 8 != 32:
        print("Need 32-bit Python!"); return

    dll = ctypes.CDLL(DLL_PATH)
    print("DLL loaded")

    # ================================================================
    # 0. List all DLL exports
    # ================================================================
    print(f"\n{'='*60}")
    print("DLL EXPORTS")
    print(f"{'='*60}")
    exports = list_dll_exports(DLL_PATH)
    for e in exports:
        print(f"  {e}")

    # ================================================================
    # 1. Test MgL -> Wire conversion
    # ================================================================
    print(f"\n{'='*60}")
    print("MgL -> Wire conversion test")
    print(f"{'='*60}")

    conn_mgl = dll_get_connect(dll)
    conn_wire = mgl_to_wire(conn_mgl)
    print(f"  connect MgL:  {conn_mgl.hex()}")
    print(f"  connect wire: {conn_wire.hex() if conn_wire else 'FAIL'}")

    scen_mgl = dll_get_scen(dll, 0, 100, 255, 0, 0, 1)
    scen_wire = mgl_to_wire(scen_mgl)
    print(f"  scen MgL:  {scen_mgl.hex()}")
    print(f"  scen wire: {scen_wire.hex() if scen_wire else 'FAIL'}")

    # ================================================================
    # 2. Try get_large_screendata_package
    # ================================================================
    print(f"\n{'='*60}")
    print("get_large_screendata_package test")
    print(f"{'='*60}")

    # 75 LEDs, all RED
    pix = (ctypes.c_ubyte * 225)()
    for i in range(75):
        pix[i*3] = 255; pix[i*3+1] = 0; pix[i*3+2] = 0
    
    sd_data, sd_ret = dll_get_screendata(dll, 75, pix)
    if sd_data:
        print(f"  ret={sd_ret} len={len(sd_data)}")
        print(f"  first 40: {sd_data[:40].hex()}")
        if sd_data[:3] == b'MgL':
            mgl_len = sd_data[4]
            print(f"  MgL payload len: {mgl_len}")
            wire = mgl_to_wire(sd_data)
            if wire:
                print(f"  wire len: {len(wire)} (header 5 + payload {len(wire)-5})")
        elif sd_data[:3] == b'\x55\xAA\x5A':
            print(f"  Already wire format! len={sd_data[3]}")
    else:
        print(f"  Failed: {sd_ret}")
        # Try different signatures
        for sig_desc, call_fn in [
            ("(num_leds, pixels, pix_len, buf)", 
             lambda: dll.get_large_screendata_package(75, pix, 225, ctypes.create_string_buffer(2048))),
            ("(pixels, pix_len, buf)",
             lambda: dll.get_large_screendata_package(pix, 225, ctypes.create_string_buffer(2048))),
        ]:
            try:
                buf_test = ctypes.create_string_buffer(2048)
                # Re-try with different approaches
                print(f"  Trying {sig_desc}...")
            except:
                pass

    # ================================================================
    # 3. Open port and send DLL packets
    # ================================================================
    print(f"\n{'='*60}")
    print("SENDING DLL PACKETS TO CONTROLLER")
    print(f"{'='*60}")

    try:
        ser = serial.Serial(COM_PORT, BAUD, timeout=0.5)
    except serial.SerialException:
        print("ERROR: Cannot open COM7. Close other apps first.")
        return

    ser.dtr = True; ser.rts = True
    time.sleep(0.3); ser.read(4096)

    # TEST A: Send connect package first, then scen RED
    print("\n--- TEST A: connect + scen(RED) ---")
    conn = mgl_to_wire(dll_get_connect(dll))
    # Split wire packet: header (5 bytes) + payload
    ser.write(conn[:5]); ser.flush(); time.sleep(0.005)
    ser.write(conn[5:]); ser.flush(); time.sleep(0.3)
    resp = ser.read(4096)
    print(f"  connect sent, response: {len(resp)} bytes")
    if resp:
        print(f"    resp: {resp[:40].hex()}")

    # Send scen RED repeatedly
    for i in range(30):
        scen = mgl_to_wire(dll_get_scen(dll, 0, 100, 255, 0, 0, 1))
        ser.write(scen[:5]); ser.flush(); time.sleep(0.005)
        ser.write(scen[5:]); ser.flush(); time.sleep(0.08)
    print("  Sent 30 scen RED. LED RED?")
    time.sleep(1)

    # TEST B: Send raw MgL (no conversion)
    print("\n--- TEST B: Raw MgL connect + scen(GREEN) ---")
    mgl_conn = dll_get_connect(dll)
    ser.write(mgl_conn); ser.flush(); time.sleep(0.3)
    resp = ser.read(4096)
    print(f"  MgL connect sent, response: {len(resp)} bytes")

    for i in range(30):
        mgl_scen = dll_get_scen(dll, 0, 100, 0, 255, 0, 1)
        ser.write(mgl_scen); ser.flush(); time.sleep(0.08)
    print("  Sent 30 MgL scen GREEN. LED GREEN?")
    time.sleep(1)

    # TEST C: First replay handshake, THEN send DLL wire packets
    print("\n--- TEST C: Replay handshake + DLL wire scen(BLUE) ---")
    cap = os.path.join(CSV_DIR, "__pycache__", "surely_full_red.csv")
    if os.path.exists(cap):
        writes = []
        with open(cap, "r", errors="replace") as f:
            for line in f:
                if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                    continue
                parts = line.split(";")
                if len(parts) <= 5: continue
                try:
                    raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
                except: continue
                if raw:
                    if len(raw) >= 238: break
                    writes.append(raw)

        for raw in writes:
            ser.write(raw); ser.flush()
            is_hdr = len(raw) == 5 and raw[0:3] == b'\x55\xAA\x5A'
            time.sleep(0.005 if is_hdr else 0.08)
        time.sleep(0.5); ser.read(8192)
        print(f"  Handshake replayed ({len(writes)} writes)")

        # Now send DLL scen BLUE
        for i in range(60):
            scen = mgl_to_wire(dll_get_scen(dll, 0, 100, 0, 0, 255, 1))
            ser.write(scen[:5]); ser.flush(); time.sleep(0.005)
            ser.write(scen[5:]); ser.flush(); time.sleep(0.08)
        print("  Sent 60 DLL scen BLUE. LED BLUE?")
        time.sleep(1)

    # TEST D: After handshake, try large screendata
    if sd_data:
        print("\n--- TEST D: Replay handshake + DLL screendata RED ---")
        for i in range(30):
            sd = dll_get_screendata(dll, 75, pix)[0]
            if sd:
                wire = mgl_to_wire(sd)
                if wire:
                    ser.write(wire[:5]); ser.flush(); time.sleep(0.005)
                    ser.write(wire[5:]); ser.flush(); time.sleep(0.08)
        print("  Sent screendata RED. LED RED?")

    ser.close()
    print("\nDone! Report which test (A/B/C/D) worked.")


if __name__ == "__main__":
    main()
