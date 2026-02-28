# -*- coding: utf-8 -*-
"""
soulight_control.py — Управление LED лентой Beelight через DLL + serial.

Использует beelightLib.dll для шифрования пакетов (через ctypes)
и pyserial для отправки на COM7 (500000 baud).

ЗАПУСКАТЬ ЧЕРЕЗ 32-BIT PYTHON!
  C:\\Python312-32\\python.exe soulight_control.py

Требования:
  pip install pyserial  (в 32-bit Python)
"""
import sys
import os
import ctypes
import struct
import time
import argparse

# ====================== CONFIG ======================
COM_PORT = "COM7"
BAUD_RATE = 500000
DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)

# Heartbeat packet (from captures)
HEARTBEAT = bytes([0x55, 0xAA, 0x5A, 0xF1, 0x00])


# ====================== DLL WRAPPER ======================
class BeelightDLL:
    def __init__(self, dll_path=DLL_PATH):
        self.dll = ctypes.CDLL(dll_path)
        self.handle = self.dll._handle
        print(f"[DLL] Loaded at 0x{self.handle:08x}")

    def get_connect_package(self):
        buf = ctypes.create_string_buffer(256)
        self.dll.get_connect_package(buf)
        return self._extract_packet(buf)

    def get_scen_package(self, scenid, dimmer, r, g, b, power=1):
        buf = ctypes.create_string_buffer(256)
        self.dll.get_scen_package(scenid, dimmer, r, g, b, power, buf)
        return self._extract_packet(buf)

    def get_large_screendata_package(self, image_data, rows, cols, channels=3):
        buf_size = self.dll.get_large_screenbuf_package_length() + 100
        img_buf = ctypes.create_string_buffer(bytes(image_data), len(image_data))
        out_buf = ctypes.create_string_buffer(buf_size)
        result = self.dll.get_large_screendata_package(img_buf, rows, cols, channels, out_buf)
        if result != 1:
            raise RuntimeError(f"get_large_screendata_package returned {result}")
        return self._extract_packet(out_buf)

    def get_audiodata_package(self, audio_data, length):
        buf = ctypes.create_string_buffer(2048)
        img_buf = ctypes.create_string_buffer(bytes(audio_data), len(audio_data))
        result = self.dll.get_audiodata_package(img_buf, length, buf)
        return self._extract_packet(buf)

    def _extract_packet(self, buf):
        """Extract actual packet from buffer (find last non-zero byte)."""
        raw = buf.raw
        # Find last non-zero
        last_nz = 0
        for i in range(len(raw) - 1, -1, -1):
            if raw[i] != 0:
                last_nz = i
                break
        return raw[:last_nz + 1]


# ====================== SERIAL SENDER ======================
class BeelightSerial:
    def __init__(self, port=COM_PORT, baud=BAUD_RATE):
        import serial
        self.ser = serial.Serial(port, baud, timeout=1)
        # Toggle DTR like the original app
        self.ser.dtr = False
        time.sleep(0.1)
        self.ser.dtr = True
        time.sleep(0.1)
        print(f"[SERIAL] Opened {port} @ {baud}")

    def send(self, data):
        self.ser.write(data)

    def send_heartbeat(self):
        self.send(HEARTBEAT)

    def read(self, size=256, timeout=0.5):
        self.ser.timeout = timeout
        return self.ser.read(size)

    def close(self):
        self.ser.close()


# ====================== MAIN CONTROL ======================
def send_color(dll, ser, r, g, b, dimmer=100, scenid=0, duration=5.0):
    """Send solid color to LED strip."""
    print(f"[COLOR] Sending R={r} G={g} B={b} dimmer={dimmer}")

    # Send connect first
    conn_pkt = dll.get_connect_package()
    print(f"[TX] Connect: {conn_pkt.hex()}")

    start = time.time()
    while time.time() - start < duration:
        # Heartbeat
        ser.send_heartbeat()
        time.sleep(0.02)

        # Color packet
        pkt = dll.get_scen_package(scenid, dimmer, r, g, b, 1)
        ser.send(pkt)
        time.sleep(0.08)

    print(f"[COLOR] Done ({duration:.1f}s)")


def send_screen(dll, ser, r, g, b, duration=5.0):
    """Send screen data (solid color) via get_large_screendata_package."""
    print(f"[SCREEN] Sending solid R={r} G={g} B={b}")

    # Create a 1920x1080 solid color image
    rows, cols, ch = 1080, 1920, 3
    img = bytearray(rows * cols * ch)
    for i in range(0, len(img), 3):
        img[i] = r
        img[i + 1] = g
        img[i + 2] = b

    start = time.time()
    while time.time() - start < duration:
        ser.send_heartbeat()
        time.sleep(0.02)

        pkt = dll.get_large_screendata_package(img, rows, cols, ch)
        ser.send(pkt)
        time.sleep(0.08)

    print(f"[SCREEN] Done ({duration:.1f}s)")


def interactive_mode(dll, ser):
    """Interactive color control."""
    print("\n=== Soulight Interactive Mode ===")
    print("Commands:")
    print("  red / green / blue / white / off")
    print("  rgb R G B  (e.g. 'rgb 255 128 0')")
    print("  dim N      (brightness 0-100)")
    print("  screen R G B  (via screen mirroring path)")
    print("  quit")
    print()

    dimmer = 100

    while True:
        try:
            cmd = input("> ").strip().lower()
        except (EOFError, KeyboardInterrupt):
            break

        if not cmd:
            continue

        parts = cmd.split()

        if parts[0] == "quit":
            break
        elif parts[0] == "red":
            send_color(dll, ser, 255, 0, 0, dimmer)
        elif parts[0] == "green":
            send_color(dll, ser, 0, 255, 0, dimmer)
        elif parts[0] == "blue":
            send_color(dll, ser, 0, 0, 255, dimmer)
        elif parts[0] == "white":
            send_color(dll, ser, 255, 255, 255, dimmer)
        elif parts[0] == "off":
            send_color(dll, ser, 0, 0, 0, 0)
        elif parts[0] == "rgb" and len(parts) >= 4:
            r, g, b = int(parts[1]), int(parts[2]), int(parts[3])
            send_color(dll, ser, r, g, b, dimmer)
        elif parts[0] == "dim" and len(parts) >= 2:
            dimmer = int(parts[1])
            print(f"Brightness set to {dimmer}")
        elif parts[0] == "screen" and len(parts) >= 4:
            r, g, b = int(parts[1]), int(parts[2]), int(parts[3])
            send_screen(dll, ser, r, g, b)
        else:
            print(f"Unknown command: {cmd}")


def main():
    # Check 32-bit
    ptr_size = struct.calcsize("P") * 8
    if ptr_size != 32:
        print(f"ERROR: This script requires 32-bit Python! (current: {ptr_size}-bit)")
        print("Run: C:\\Python312-32\\python.exe soulight_control.py")
        sys.exit(1)

    parser = argparse.ArgumentParser(description="Soulight LED Control")
    parser.add_argument("--port", default=COM_PORT, help=f"Serial port (default: {COM_PORT})")
    parser.add_argument("--color", nargs=3, type=int, metavar=("R", "G", "B"),
                        help="Send solid color and exit")
    parser.add_argument("--screen", nargs=3, type=int, metavar=("R", "G", "B"),
                        help="Send via screen mirroring path")
    parser.add_argument("--duration", type=float, default=10.0,
                        help="Duration in seconds (default: 10)")
    parser.add_argument("--dimmer", type=int, default=100,
                        help="Brightness 0-100 (default: 100)")
    parser.add_argument("--test", action="store_true",
                        help="Quick test: RED 3s -> GREEN 3s -> BLUE 3s")
    parser.add_argument("--interactive", action="store_true",
                        help="Interactive mode")
    args = parser.parse_args()

    # Initialize
    print("Soulight LED Controller v1.0")
    print(f"DLL: {DLL_PATH}")
    print(f"Port: {args.port}")
    print()

    dll = BeelightDLL()

    try:
        import serial
    except ImportError:
        print("ERROR: pyserial not installed!")
        print(f"Run: C:\\Python312-32\\python.exe -m pip install pyserial")
        sys.exit(1)

    ser = BeelightSerial(args.port)

    try:
        if args.test:
            print("=== Quick Test: RED -> GREEN -> BLUE ===")
            send_color(dll, ser, 255, 0, 0, args.dimmer, duration=3)
            send_color(dll, ser, 0, 255, 0, args.dimmer, duration=3)
            send_color(dll, ser, 0, 0, 255, args.dimmer, duration=3)
            send_color(dll, ser, 0, 0, 0, 0, duration=1)  # off
        elif args.color:
            r, g, b = args.color
            send_color(dll, ser, r, g, b, args.dimmer, duration=args.duration)
        elif args.screen:
            r, g, b = args.screen
            send_screen(dll, ser, r, g, b, duration=args.duration)
        elif args.interactive:
            interactive_mode(dll, ser)
        else:
            # Default: interactive
            interactive_mode(dll, ser)
    except KeyboardInterrupt:
        print("\nStopping...")
    finally:
        ser.close()
        print("Done.")


if __name__ == "__main__":
    main()
