# -*- coding: utf-8 -*-
"""
test_syncrgb_hardware.py — Hardware test for GenProtocolSyncRGB packets.

GenProtocolSyncRGB(rows, cols, Color[]) generates packets with LP_CMD=6
(different from GenRGBTransferPackage which uses LP_CMD=5).

Phases:
  A: Capture handshake + GenProtocolSyncRGB(1,75) RED
  B: Capture handshake + GenProtocolSyncRGB(1,75) 4-color pattern
  C: Historical handshake + GenProtocolSyncRGB(1,75) RED
  D: No handshake + GenProtocolSyncRGB(1,75) RED
  E: Capture handshake + GenProtocolSyncRGB(75,1) RED (different layout)
"""
import os, sys, time
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import serial, clr

BEELIGHT_EXE = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte, Activator
from System.Drawing import Color
import System

COM = "COM7"; BAUD = 500000; NUM = 75
CSV = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")
ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static


def load_writes(fp):
    w = []
    with open(fp, "r", errors="replace") as f:
        for n, line in enumerate(f, 1):
            if n == 1: continue
            p = line.split(";")
            if len(p) < 6: continue
            if "IRP_MJ_WRITE" not in p[2] or p[3].strip() != "DOWN": continue
            d = p[5].strip()
            if not d: continue
            try: raw = bytes.fromhex(d.replace(" ", ""))
            except: continue
            if raw: w.append(raw)
    return w

def get_capture_handshake(writes):
    fb = next((i for i, w in enumerate(writes) if len(w) >= 238), None)
    if fb is None: return writes
    hs = writes[:fb]
    if hs and hs[-1][:3] == b'\x55\xAA\x5A':
        return hs[:-1]
    return hs

def openp():
    s = serial.Serial(COM, BAUD, timeout=0.5, write_timeout=0.5)
    s.dtr = True; s.rts = True; time.sleep(0.3); s.read(4096); return s

def send_hs(s, hs):
    for w in hs:
        s.write(w); s.flush()
        time.sleep(0.05 if len(w) == 5 else 0.1)
    time.sleep(0.5); s.read(4096)

def setup_syncrgb():
    """Initialize LProtocolSyncRGB instance and get method reference."""
    asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
    sync_type = None
    for t in asm.GetTypes():
        if t.Name == "LProtocolSyncRGB":
            sync_type = t; break
    if not sync_type:
        return None, None
    instance = Activator.CreateInstance(sync_type)
    method = sync_type.GetMethod("GenProtocolSyncRGB", ALL)
    return instance, method

def gen_syncrgb(instance, method, rows, cols, colors_rgb):
    """Generate a SyncRGB packet. colors_rgb = list of (R,G,B) tuples."""
    n = rows * cols
    colors = Array[Color]([Color.FromArgb(r, g, b) for r, g, b in colors_rgb[:n]])
    result = method.Invoke(instance, Array[System.Object]([Byte(rows), Byte(cols), colors]))
    if result is None:
        return None
    return bytes(result)

def send_syncrgb_loop(s, instance, method, rows, cols, colors_rgb, duration=4.0):
    """Send SyncRGB packets in a loop."""
    start = time.time()
    sent = 0
    while time.time() - start < duration:
        pkt = gen_syncrgb(instance, method, rows, cols, colors_rgb)
        if pkt is None:
            print("  [!] GenProtocolSyncRGB returned None")
            break
        # Packet already includes 55AA5A frame header
        s.write(pkt)
        s.flush()
        sent += 1
        time.sleep(0.033)
    return sent


def main():
    print("=" * 60)
    print("  GenProtocolSyncRGB HARDWARE TEST")
    print("=" * 60)

    # Setup
    writes = load_writes(CSV)
    cap_hs = get_capture_handshake(writes)
    print(f"  Capture handshake: {len(cap_hs)} writes")

    instance, method = setup_syncrgb()
    if instance is None:
        print("[FAIL] Could not setup SyncRGB"); return
    print(f"  SyncRGB instance ready")

    # Test packet generation
    test_pkt = gen_syncrgb(instance, method, 1, 75, [(255, 0, 0)] * 75)
    print(f"  Test packet: {len(test_pkt)} bytes")

    # Colors
    red75 = [(255, 0, 0)] * 75
    green75 = [(0, 255, 0)] * 75
    pattern = ([(255,0,0)]*19 + [(0,255,0)]*19 + [(0,0,255)]*19 + [(255,255,0)]*18)

    # Historical handshake
    from soulight.protocol.raw_git_replay import RawGitReplayProtocol
    repo = os.path.dirname(os.path.abspath(__file__))
    protocol = RawGitReplayProtocol(repo_path=repo)

    # === Phase A: Capture hs + SyncRGB(1,75) RED ===
    print("\n--- A: Capture hs + SyncRGB(1,75) RED ---")
    s = openp()
    try:
        send_hs(s, cap_hs)
        n = send_syncrgb_loop(s, instance, method, 1, 75, red75, 4.0)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase B: Capture hs + SyncRGB(1,75) 4-color ===
    print("\n--- B: Capture hs + SyncRGB(1,75) 4-color ---")
    s = openp()
    try:
        send_hs(s, cap_hs)
        n = send_syncrgb_loop(s, instance, method, 1, 75, pattern, 4.0)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase C: Historical hs + SyncRGB(1,75) RED ===
    print("\n--- C: Historical hs + SyncRGB(1,75) RED ---")
    s = openp()
    try:
        for w in protocol.iter_handshake_writes():
            s.write(w); s.flush()
            time.sleep(0.005 if len(w) == 5 else 0.01)
        time.sleep(0.5); s.read(4096)
        n = send_syncrgb_loop(s, instance, method, 1, 75, red75, 4.0)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase D: No handshake + SyncRGB(1,75) RED ===
    print("\n--- D: No handshake + SyncRGB(1,75) RED ---")
    s = openp()
    try:
        n = send_syncrgb_loop(s, instance, method, 1, 75, red75, 4.0)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase E: Capture hs + SyncRGB(75,1) RED ===
    print("\n--- E: Capture hs + SyncRGB(75,1) RED ---")
    s = openp()
    try:
        send_hs(s, cap_hs)
        n = send_syncrgb_loop(s, instance, method, 75, 1, red75, 4.0)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()

    print("\n[Done] Report:")
    print("  A (cap hs + 1x75 RED):      ___")
    print("  B (cap hs + 1x75 4color):   ___")
    print("  C (hist hs + 1x75 RED):     ___")
    print("  D (no hs + 1x75 RED):       ___")
    print("  E (cap hs + 75x1 RED):      ___")

if __name__ == "__main__":
    main()
