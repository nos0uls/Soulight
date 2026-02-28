# -*- coding: utf-8 -*-
"""
debug_serial.py — Диагностика serial порта.
Запускать через 32-bit Python!
"""
import sys
import struct

ptr_size = struct.calcsize("P") * 8
print(f"Python: {ptr_size}-bit")
print(f"Python path: {sys.executable}")

# Check pyserial
try:
    import serial
    print(f"pyserial: {serial.__version__}")
except ImportError:
    print("ERROR: pyserial NOT installed!")
    print(f"Fix: \"{sys.executable}\" -m pip install pyserial")
    sys.exit(1)

# List available ports
from serial.tools import list_ports
ports = list(list_ports.comports())
print(f"\nAvailable COM ports ({len(ports)}):")
for p in ports:
    print(f"  {p.device}: {p.description} [{p.hwid}]")

# Try to open COM7
port = "COM7"
baud = 500000
print(f"\nTrying to open {port} @ {baud}...")

try:
    ser = serial.Serial(port, baud, timeout=1)
    print(f"  ✓ Opened successfully!")
    print(f"  Port: {ser.port}")
    print(f"  Baudrate: {ser.baudrate}")
    print(f"  Is open: {ser.is_open}")
    
    # Toggle DTR
    ser.dtr = False
    import time
    time.sleep(0.05)
    ser.dtr = True
    time.sleep(0.05)
    
    # Send heartbeat
    hb = bytes([0x55, 0xAA, 0x5A, 0xF1, 0x00])
    print(f"\n  Sending heartbeat: {hb.hex()}")
    n = ser.write(hb)
    print(f"  Bytes written: {n}")
    ser.flush()
    print(f"  Flushed.")
    
    # Wait for response
    time.sleep(0.2)
    resp = ser.read(100)
    if resp:
        print(f"  Response ({len(resp)} bytes): {resp.hex()}")
    else:
        print(f"  No response (timeout)")
    
    # Send a few more heartbeats
    print(f"\n  Sending 5 more heartbeats...")
    for i in range(5):
        ser.write(hb)
        ser.flush()
        time.sleep(0.1)
        resp = ser.read(100)
        if resp:
            print(f"    [{i}] Response: {resp.hex()}")
        else:
            print(f"    [{i}] No response")
    
    # Now send a DLL packet (RED color)
    import ctypes
    import os
    dll_path = os.path.join(os.path.dirname(os.path.abspath(__file__)),
                            "Новая папка", "Dumps", "beelightLib.dll")
    dll = ctypes.CDLL(dll_path)
    
    buf = ctypes.create_string_buffer(256)
    dll.get_scen_package(0, 100, 255, 0, 0, 1, buf)
    raw = buf.raw
    last_nz = max((j for j in range(len(raw)) if raw[j] != 0), default=0)
    pkt = raw[:last_nz + 1]
    
    print(f"\n  Sending DLL scen packet (RED): {pkt.hex()}")
    n = ser.write(pkt)
    ser.flush()
    print(f"  Bytes written: {n}")
    
    time.sleep(0.2)
    resp = ser.read(100)
    if resp:
        print(f"  Response: {resp.hex()}")
    else:
        print(f"  No response")
    
    # Try sending heartbeat + DLL packet in a loop
    print(f"\n  Loop test: heartbeat + RED (10 iterations)...")
    for i in range(10):
        ser.write(hb)
        ser.flush()
        time.sleep(0.02)
        
        # Generate fresh packet each time (new random IV)
        buf2 = ctypes.create_string_buffer(256)
        dll.get_scen_package(0, 100, 255, 0, 0, 1, buf2)
        raw2 = buf2.raw
        last2 = max((j for j in range(len(raw2)) if raw2[j] != 0), default=0)
        pkt2 = raw2[:last2 + 1]
        
        ser.write(pkt2)
        ser.flush()
        time.sleep(0.08)
        
        resp = ser.read(100)
        if resp and i < 3:
            print(f"    [{i}] resp: {resp.hex()}")
    
    print(f"\n  Loop done. Check if LED changed color!")
    
    ser.close()
    print(f"  Port closed.")
    
except serial.SerialException as e:
    print(f"  ✗ FAILED: {e}")
    print(f"\n  Possible causes:")
    print(f"    1. Serial Port Monitor has exclusive lock on the port")
    print(f"    2. Beelight app is still running and using the port")
    print(f"    3. Wrong port name")
    print(f"\n  Try: close Serial Port Monitor and Beelight app, then re-run")

except Exception as e:
    print(f"  ✗ ERROR: {type(e).__name__}: {e}")

print("\nDONE")
