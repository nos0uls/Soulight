# -*- coding: utf-8 -*-
"""
test_channelmark_ff_hardware.py — THE critical test.

GenRGBTransferPackage(colors, channelMark=0xFF) produces packets
identical to capture. channelMark=0 was the bug all along.

Phases:
  A: Historical hs + GenRGBTransferPackage(RED, channelMark=0xFF) — split sends
  B: Historical hs + GenRGBTransferPackage(4-color, channelMark=0xFF) — per-LED!
  C: Historical hs + GenRGBTransferPackage(RED, channelMark=0) — control (old bug)
"""
import os, sys, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import serial, clr

BEELIGHT_EXE = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte
from System.Drawing import Color
import System

COM = "COM7"; BAUD = 500000; NUM = 75
ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static


def openp():
    s = serial.Serial(COM, BAUD, timeout=0.5, write_timeout=0.5)
    s.dtr = True; s.rts = True; time.sleep(0.3); s.read(4096); return s


def send_split(s, pkt, delay=0.033):
    """Send frame header + payload separately."""
    if pkt[:3] == b'\x55\xAA\x5A':
        s.write(pkt[:5]); s.flush()
        time.sleep(0.005)
        s.write(pkt[5:]); s.flush()
    else:
        s.write(pkt); s.flush()
    time.sleep(delay)


# Setup .NET
asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
ctrl = [t for t in asm.GetTypes() if t.Name == "LProtocolCtrl"][0]
color_overload = None
for m in ctrl.GetMethods(ALL):
    if m.Name == "GenRGBTransferPackage":
        params = m.GetParameters()
        if len(params) == 2 and params[0].ParameterType.Name == "Color[]":
            color_overload = m; break


def gen_rgb(colors_rgb, channel_mark=0xFF):
    """Generate GenRGBTransferPackage with specified channelMark."""
    colors = Array[Color]([Color.FromArgb(r, g, b) for r, g, b in colors_rgb])
    result = color_overload.Invoke(None, Array[System.Object]([colors, Byte(channel_mark)]))
    return bytes(result) if result else None


def main():
    from soulight.protocol.raw_git_replay import RawGitReplayProtocol
    repo = os.path.dirname(os.path.abspath(__file__))
    protocol = RawGitReplayProtocol(repo_path=repo)

    red75 = [(255, 0, 0)] * NUM
    pattern = [(255,0,0)]*19 + [(0,255,0)]*19 + [(0,0,255)]*19 + [(255,255,0)]*18

    # Verify packet generation
    pkt_ff = gen_rgb(red75, 0xFF)
    pkt_00 = gen_rgb(red75, 0x00)
    print(f"channelMark=0xFF: {len(pkt_ff)}B")
    print(f"channelMark=0x00: {len(pkt_00)}B")

    # === Phase A: Historical hs + RED channelMark=0xFF ===
    print("\n--- A: Historical hs + RED channelMark=0xFF ---")
    s = openp()
    try:
        for w in protocol.iter_handshake_writes():
            s.write(w); s.flush()
            time.sleep(0.005 if len(w) == 5 else 0.01)
        time.sleep(0.5); s.read(4096)

        sent = 0
        for _ in range(120):
            pkt = gen_rgb(red75, 0xFF)
            if pkt:
                send_split(s, pkt)
                sent += 1
        print(f"  sent={sent}")
        time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase B: Historical hs + 4-color pattern channelMark=0xFF ===
    print("\n--- B: Historical hs + 4-COLOR PATTERN channelMark=0xFF ---")
    s = openp()
    try:
        for w in protocol.iter_handshake_writes():
            s.write(w); s.flush()
            time.sleep(0.005 if len(w) == 5 else 0.01)
        time.sleep(0.5); s.read(4096)

        sent = 0
        for _ in range(120):
            pkt = gen_rgb(pattern, 0xFF)
            if pkt:
                send_split(s, pkt)
                sent += 1
        print(f"  sent={sent}")
        time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase C: Historical hs + RED channelMark=0 (control — should fail) ===
    print("\n--- C: Historical hs + RED channelMark=0 (CONTROL — old bug) ---")
    s = openp()
    try:
        for w in protocol.iter_handshake_writes():
            s.write(w); s.flush()
            time.sleep(0.005 if len(w) == 5 else 0.01)
        time.sleep(0.5); s.read(4096)

        sent = 0
        for _ in range(120):
            pkt = gen_rgb(red75, 0x00)
            if pkt:
                send_split(s, pkt)
                sent += 1
        print(f"  sent={sent}")
        time.sleep(3)
    finally: s.close()

    print("\n[Done] Report:")
    print("  A (hist hs + RED ch=0xFF):      ___")
    print("  B (hist hs + 4-color ch=0xFF):  ___")
    print("  C (hist hs + RED ch=0x00):      ___  (control, should be dark)")


if __name__ == "__main__":
    main()
