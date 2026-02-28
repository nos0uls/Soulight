# -*- coding: utf-8 -*-
"""
diag_exports_safe.py - Test each DLL export in a SEPARATE subprocess.
Native crashes in one test won't kill the whole script.
Run with 32-bit Python!
"""
import sys, os, subprocess, struct

sys.stdout.reconfigure(encoding="utf-8")
PYTHON = sys.executable
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))

# region ===== Тесты для каждого экспорта =====
# Каждый тест — отдельный Python-скрипт запущенный как subprocess
TESTS = {
    "get_connect_package": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_connect_package(buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
mgl = pkt[:3] == b'MgL'
if mgl: print("payload_after_mgl=%d hex=%s" % (len(pkt)-4, pkt[4:].hex()))
""",

    "get_scen_package": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_scen_package(0, 100, 255, 0, 0, 1, buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
if pkt[:3] == b'MgL': print("payload=%d" % (len(pkt)-4))
""",

    "get_setting_package(0)": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_setting_package(0, buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
if pkt[:3] == b'MgL': print("payload=%d" % (len(pkt)-4))
""",

    "get_setting_package(1)": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_setting_package(1, buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
if pkt[:3] == b'MgL': print("payload=%d" % (len(pkt)-4))
""",

    "get_eco_setting_package(0)": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_eco_setting_package(0, buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
if pkt[:3] == b'MgL': print("payload=%d" % (len(pkt)-4))
""",

    "get_screen_size_setting_package(75)": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_screen_size_setting_package(75, buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
if pkt[:3] == b'MgL': print("payload=%d" % (len(pkt)-4))
""",

    "get_reset_factory_package": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_reset_factory_package(buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
if pkt[:3] == b'MgL': print("payload=%d" % (len(pkt)-4))
""",

    "get_net_udp_scen_package(0,100,255,0,0,1)": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
buf = ctypes.create_string_buffer(2048)
ret = dll.get_net_udp_scen_package(0, 100, 255, 0, 0, 1, buf)
raw = buf.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d len=%d hex=%s" % (ret, len(pkt), pkt.hex()))
if pkt[:3] == b'MgL': print("payload=%d" % (len(pkt)-4))
""",

    "buffer_sizes": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
print("large=%d" % dll.get_large_screenbuf_package_length())
print("middle=%d" % dll.get_middle_screenbuf_package_length())
print("top=%d" % dll.get_top_start_line())
print("bottom=%d" % dll.get_bottom_start_line())
""",

    "unpack_package(captured_resp)": """
import ctypes, os
dll = ctypes.CDLL(os.path.join(r'{dir}', '\\u041d\\u043e\\u0432\\u0430\\u044f \\u043f\\u0430\\u043f\\u043a\\u0430', 'Dumps', 'beelightLib.dll'))
resp = bytes.fromhex("55aa5a76009f39f083e3fa8ec9b1b0f182e2fa8fc8b0b0f1828793f7e4d5869ce5948ae6b0dfdf98")
ib = ctypes.create_string_buffer(resp, len(resp))
ob = ctypes.create_string_buffer(2048)
ret = dll.unpack_package(ib, ob)
raw = ob.raw
last = max((i for i in range(len(raw)) if raw[i] != 0), default=0)
pkt = raw[:last+1]
print("ret=%d outlen=%d hex=%s" % (ret, len(pkt), pkt[:40].hex()))
""",
}
# endregion

# region ===== Запуск тестов =====
print("=" * 60)
print("Testing ALL beelightLib.dll exports (each in subprocess)")
print("=" * 60)

for name, code in TESTS.items():
    code_filled = code.replace("{dir}", SCRIPT_DIR)
    print("\n--- %s ---" % name)
    try:
        result = subprocess.run(
            [PYTHON, "-c", code_filled],
            capture_output=True, text=True, timeout=5,
            cwd=SCRIPT_DIR
        )
        if result.stdout.strip():
            for line in result.stdout.strip().split("\n"):
                print("  %s" % line)
        if result.returncode != 0:
            print("  EXIT CODE: %d" % result.returncode)
            if result.stderr.strip():
                # Показываем только последнюю строку ошибки
                err_lines = result.stderr.strip().split("\n")
                print("  ERROR: %s" % err_lines[-1])
    except subprocess.TimeoutExpired:
        print("  TIMEOUT (5s)")
    except Exception as e:
        print("  FAILED: %s" % e)

print("\nAll tests complete.")
# endregion
