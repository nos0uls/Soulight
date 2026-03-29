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


def all_white():
    return [(255, 255, 255)] * 75


def half_white_half_green():
    return [(255, 255, 255)] * 38 + [(0, 255, 0)] * 37


def quarter_blocks():
    colors = []
    colors.extend([(255, 255, 255)] * 19)
    colors.extend([(255, 0, 0)] * 19)
    colors.extend([(0, 255, 0)] * 19)
    colors.extend([(0, 0, 255)] * 18)
    return colors


def single_led_white():
    colors = [(0, 255, 0)] * 75
    colors[0] = (255, 255, 255)
    return colors


def alternating_white_green():
    colors = []
    for i in range(75):
        if i % 2 == 0:
            colors.append((255, 255, 255))
        else:
            colors.append((0, 255, 0))
    return colors


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
    print("RAW 239-245 ACCEPTANCE PROBE")
    print("=" * 60)
    print()

    run_phase("PHASE 1: all white (known-good baseline)", protocol, all_white())
    run_phase("PHASE 2: half white / half green", protocol, half_white_half_green())
    run_phase("PHASE 3: quarter blocks", protocol, quarter_blocks())
    run_phase("PHASE 4: single LED white", protocol, single_led_white())
    run_phase("PHASE 5: alternating white/green", protocol, alternating_white_green())

    print("[Probe] Done")


if __name__ == "__main__":
    main()
