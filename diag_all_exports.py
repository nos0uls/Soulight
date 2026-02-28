# -*- coding: utf-8 -*-
"""
diag_all_exports.py - Test ALL 16 exports of beelightLib.dll
Run with 32-bit Python!
"""
import sys, os, ctypes, struct

sys.stdout.reconfigure(encoding="utf-8")
if struct.calcsize("P") * 8 != 32:
    print("ERROR: need 32-bit Python"); sys.exit(1)

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll"
)
dll = ctypes.CDLL(DLL_PATH)
print("DLL loaded at 0x%08x" % dll._handle)

def extract(buf):
    raw = buf.raw
    last_nz = 0
    for i in range(len(raw) - 1, -1, -1):
        if raw[i] != 0:
            last_nz = i
            break
    return raw[:last_nz + 1]

def hexstr(data, n=30):
    return " ".join("%02x" % b for b in data[:n])

def show(pkt, label):
    has_mgl = pkt[:3] == b"MgL"
    pay = pkt[4:] if has_mgl else pkt
    tag = " [MgL]" if has_mgl else ""
    print("  %s: total=%d payload=%d%s" % (label, len(pkt), len(pay), tag))
    print("    RAW: %s" % hexstr(pkt, 40))
    if has_mgl:
        print("    PAY: %s" % hexstr(pay, 30))

def safe_call(name, func, *args):
    try:
        buf = ctypes.create_string_buffer(2048)
        ret = func(*args, buf)
        pkt = extract(buf)
        show(pkt, "%s ret=%s" % (name, ret))
        return pkt
    except Exception as e:
        print("  %s: ERROR %s" % (name, e))
        return None

print("\n" + "=" * 60)
print("1. get_connect_package")
safe_call("connect", dll.get_connect_package)

print("\n" + "=" * 60)
print("2. get_scen_package(0,100,255,0,0,1)")
safe_call("scen", dll.get_scen_package, 0, 100, 255, 0, 0, 1)

print("\n" + "=" * 60)
print("3. get_setting_package - probe signatures")
for sig, args in [("(0,buf)", [0]), ("(1,buf)", [1]), ("(0,0,buf)", [0, 0]),
                  ("(0,100,buf)", [0, 100]), ("(1,0,0,buf)", [1, 0, 0])]:
    try:
        buf = ctypes.create_string_buffer(2048)
        ret = dll.get_setting_package(*args, buf)
        pkt = extract(buf)
        if len(pkt) > 1:
            show(pkt, "setting%s ret=%s" % (sig, ret))
            break
    except Exception as e:
        print("  setting%s: %s" % (sig, type(e).__name__))

print("\n" + "=" * 60)
print("4. get_eco_setting_package - probe signatures")
for sig, args in [("(0,buf)", [0]), ("(1,buf)", [1]), ("(0,0,buf)", [0, 0])]:
    try:
        buf = ctypes.create_string_buffer(2048)
        ret = dll.get_eco_setting_package(*args, buf)
        pkt = extract(buf)
        if len(pkt) > 1:
            show(pkt, "eco%s ret=%s" % (sig, ret))
            break
    except Exception as e:
        print("  eco%s: %s" % (sig, type(e).__name__))

print("\n" + "=" * 60)
print("5. get_screen_size_setting_package - probe signatures")
for sig, args in [("(75,buf)", [75]), ("(0,buf)", [0]), ("(75,1,buf)", [75, 1]),
                  ("(1920,1080,buf)", [1920, 1080])]:
    try:
        buf = ctypes.create_string_buffer(2048)
        ret = dll.get_screen_size_setting_package(*args, buf)
        pkt = extract(buf)
        if len(pkt) > 1:
            show(pkt, "screen_size%s ret=%s" % (sig, ret))
            break
    except Exception as e:
        print("  screen_size%s: %s" % (sig, type(e).__name__))

print("\n" + "=" * 60)
print("6. get_reset_factory_package")
safe_call("reset", dll.get_reset_factory_package)

print("\n" + "=" * 60)
print("7. get_net_udp_scen_package - probe")
for sig, args in [("(0,100,255,0,0,1,buf)", [0, 100, 255, 0, 0, 1]),
                  ("(0,100,255,0,0,buf)", [0, 100, 255, 0, 0])]:
    try:
        buf = ctypes.create_string_buffer(2048)
        ret = dll.get_net_udp_scen_package(*args, buf)
        pkt = extract(buf)
        if len(pkt) > 1:
            show(pkt, "udp_scen%s ret=%s" % (sig, ret))
            break
    except Exception as e:
        print("  udp_scen%s: %s" % (sig, type(e).__name__))

print("\n" + "=" * 60)
print("8-9. Buffer sizes")
try:
    print("  large_screenbuf_length = %d" % dll.get_large_screenbuf_package_length())
except Exception as e:
    print("  large: %s" % e)
try:
    print("  middle_screenbuf_length = %d" % dll.get_middle_screenbuf_package_length())
except Exception as e:
    print("  middle: %s" % e)

print("\n" + "=" * 60)
print("10. Start lines")
try:
    print("  top = %d" % dll.get_top_start_line())
    print("  bottom = %d" % dll.get_bottom_start_line())
except Exception as e:
    print("  error: %s" % e)

print("\n" + "=" * 60)
print("11. unpack_package - test with captured response")
fake_resp = bytes.fromhex(
    "55aa5a76009f39f083e3fa8ec9b1b0f182e2fa8fc8b0b0f182"
    "8793f7e4d5869ce5948ae6b0dfdf98"
)
for sig, call_args in [
    ("(in,out)", lambda ib, ob: dll.unpack_package(ib, ob)),
    ("(in,len,out)", lambda ib, ob: dll.unpack_package(ib, len(fake_resp), ob)),
]:
    try:
        ib = ctypes.create_string_buffer(fake_resp, len(fake_resp))
        ob = ctypes.create_string_buffer(2048)
        ret = call_args(ib, ob)
        pkt = extract(ob)
        print("  unpack%s: ret=%s outlen=%d" % (sig, ret, len(pkt)))
        if len(pkt) > 1:
            print("    OUT: %s" % hexstr(pkt, 30))
    except Exception as e:
        print("  unpack%s: %s" % (sig, type(e).__name__))

print("\n" + "=" * 60)
print("SUMMARY: payload sizes after MgL strip")
print("  Handshake from capture: 8, 13, 16, 12, 8, 10, 14, 13")
print("  DLL connect payload: 11")
print("  DLL scen payload: 17")

print("\nDONE")
