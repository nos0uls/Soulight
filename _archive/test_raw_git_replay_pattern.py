import os
import sys
import time

import serial

sys.path.insert(0, ".")

from soulight.protocol.raw_git_replay import RawGitReplayProtocol

COM_PORT = "COM7"
BAUD = 500000


def open_port():
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    return ser


def send_writes(ser, writes):
    count = 0
    for raw in writes:
        ser.write(raw)
        ser.flush()
        count += 1
        if len(raw) >= 239:
            time.sleep(0.033)
        else:
            time.sleep(0.005)
    return count


def make_pattern_colors():
    colors = []
    colors.extend([(255, 0, 0)] * 23)
    colors.extend([(0, 255, 0)] * 15)
    colors.extend([(0, 0, 255)] * 23)
    colors.extend([(255, 255, 0)] * 14)
    return colors


def main():
    protocol = RawGitReplayProtocol(repo_path=os.path.dirname(os.path.abspath(__file__)))

    print("=" * 60)
    print("RAW GIT REPLAY PATTERN TEST")
    print("=" * 60)
    print()

    print("--- PHASE 1: 4-color pattern ---")
    ser = open_port()
    try:
        count = send_writes(ser, protocol.iter_handshake_writes())
        print(f"[Test] handshake writes sent = {count}")
        count = send_writes(ser, protocol.iter_color_replay_writes(make_pattern_colors(), limit=120))
        print(f"[Test] pattern writes sent = {count}")
        time.sleep(2.5)
    finally:
        ser.close()
    time.sleep(0.5)

    print("--- PHASE 2: all white ---")
    ser = open_port()
    try:
        count = send_writes(ser, protocol.iter_handshake_writes())
        print(f"[Test] handshake writes sent = {count}")
        count = send_writes(ser, protocol.iter_color_replay_writes([(255, 255, 255)] * 75, limit=120))
        print(f"[Test] white writes sent = {count}")
        time.sleep(2.5)
    finally:
        ser.close()

    print("[Test] Done")


if __name__ == "__main__":
    main()
