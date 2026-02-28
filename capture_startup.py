# -*- coding: utf-8 -*-
"""
capture_startup.py - Capture serial traffic while Beelight app connects.

Instructions:
1. Close Beelight app
2. Run this script (it will monitor COM7)
3. Open Beelight app and set a solid color
4. Press Ctrl+C to stop capture
5. The script saves all traffic to startup_capture.bin

This uses 64-bit Python (no DLL needed).
"""
import sys
import os
import time

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

COM_PORT = "COM7"
BAUD_RATE = 500000
OUT_DIR = os.path.dirname(os.path.abspath(__file__))


def main():
    print("=== Startup Capture Tool ===")
    print(f"Port: {COM_PORT} @ {BAUD_RATE}")
    print()
    print("This script will sniff serial traffic.")
    print("BUT: it can't sniff while another app has the port.")
    print()
    print("ALTERNATIVE APPROACH:")
    print("1. Open Serial Port Monitor (HHD)")
    print("2. Start monitoring COM7")
    print("3. Open Beelight app")
    print("4. Set a solid RED color")
    print("5. Wait 5 seconds")
    print("6. Close Beelight app")
    print("7. Stop monitoring")
    print("8. Export as CSV (same format as your other captures)")
    print("9. Save as 'startup.csv' in the Soulight folder")
    print()
    print("Then we can analyze the startup sequence!")
    print()
    
    # Alternative: we can try to capture by being the first to open the port,
    # then Beelight will fail to connect. Instead, let's send our packets
    # right after Beelight disconnects (controller stays in PC mode briefly).
    
    print("=" * 60)
    print("Quick test: Send packets right after opening port")
    print("If controller is already in PC mode, this will work.")
    print("=" * 60)
    
    try:
        ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=0.5)
    except serial.SerialException as e:
        print(f"Can't open port: {e}")
        print("Close other apps using COM7 and try again.")
        return
    
    # Don't toggle DTR - just send data immediately
    time.sleep(0.1)
    ser.read(4096)  # drain
    
    # Send heartbeats first
    hb = bytes([0x55, 0xAA, 0x5A, 0x00, 0x00])
    for _ in range(5):
        ser.write(hb)
        ser.flush()
        time.sleep(0.05)
    
    resp = ser.read(4096)
    if resp:
        print(f"Controller responds: {len(resp)} bytes")
        # Parse responses
        i = 0
        pkt_num = 0
        while i < len(resp) - 4:
            if resp[i] == 0x55 and resp[i+1] == 0xAA and resp[i+2] == 0x5A:
                pkt_len = resp[i+3]
                pkt_data = resp[i+4:i+5+pkt_len]
                print(f"  Response[{pkt_num}]: cmd/len=0x{resp[i+3]:02x} data={pkt_data.hex()}")
                i += 5 + pkt_len
                pkt_num += 1
            else:
                i += 1
    
    # Now send RED
    print("\nSending RED color packets (10 seconds)...")
    print("(If controller is in PC mode, LED should turn RED)")
    
    import random
    start = time.time()
    count = 0
    while time.time() - start < 10.0:
        key = [random.randint(1, 254) for _ in range(3)]
        k0, k1, k2 = key
        
        plain = bytearray(238)
        plain[0] = random.randint(0, 255)
        plain[1] = random.randint(0, 255)
        plain[6] = 0x05
        plain[7] = 0x05
        plain[8] = 0xFF
        plain[9] = 0xE3
        plain[11] = 0x4B
        for j in range(75):
            base = 12 + j * 3
            plain[base] = 255  # R
            plain[base + 1] = 0  # G
            plain[base + 2] = 0  # B
        
        cipher = bytearray(238)
        for j in range(238):
            cipher[j] = plain[j] ^ key[j % 3]
        
        frame_hdr = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00])
        ser.write(frame_hdr)
        ser.flush()
        time.sleep(0.005)
        ser.write(bytes(cipher))
        ser.flush()
        time.sleep(0.075)
        count += 1
    
    print(f"Sent {count} RED packets")
    print("Did LED turn RED?")
    
    ser.close()


if __name__ == "__main__":
    main()
