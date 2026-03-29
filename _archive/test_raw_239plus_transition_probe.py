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


def make_two_blocks():
    return [(255, 255, 255)] * 38 + [(0, 255, 0)] * 37


def make_three_blocks():
    return [(255, 255, 255)] * 25 + [(0, 255, 0)] * 25 + [(255, 255, 255)] * 25


def make_four_blocks():
    return [(255, 255, 255)] * 19 + [(0, 255, 0)] * 19 + [(255, 255, 255)] * 19 + [(0, 255, 0)] * 18


def run_phase(label, protocol, colors):
    print(f"--- {label} ---")
    ser = open_port()
    try:
        count = send_writes(ser, protocol.iter_handshake_writes())
        print(f"[Probe] handshake writes sent = {count}")
        count = send_writes(ser, protocol.iter_color_replay_writes(colors, limit=120))
        print(f"[Probe] color writes sent = {count}")
        time.sleep(2.2)
    finally:
        ser.close()
    time.sleep(0.5)


def main():
    protocol = RawGitReplayProtocol(repo_path=os.path.dirname(os.path.abspath(__file__)))

    print("=" * 60)
    print("RAW 239-245 TRANSITION PROBE")
    print("=" * 60)
    print()

    run_phase("PHASE 1: 2 blocks (known good baseline)", protocol, make_two_blocks())
    run_phase("PHASE 2: 3 blocks (same two colors)", protocol, make_three_blocks())
    run_phase("PHASE 3: 4 blocks (same two colors)", protocol, make_four_blocks())

    print("[Probe] Done")


if __name__ == "__main__":
    main()
