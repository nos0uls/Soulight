# -*- coding: utf-8 -*-
"""
test_syncrgb_split.py — GenProtocolSyncRGB with SPLIT sends + heartbeats.

Key fixes vs previous test:
1. Send frame header (5 bytes) and payload SEPARATELY (as Beelight does)
2. Add periodic heartbeat packets (every 10 frames)
3. Try different row/col combos (75x1 gives period=4, within normal range)
4. Try reading controller responses between packets
5. Try with SyncStatus packet first
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

def send_hs_with_reads(s, hs):
    """Send handshake, reading responses between frame+payload pairs."""
    i = 0
    while i < len(hs):
        w = hs[i]
        s.write(w); s.flush()
        if len(w) == 5 and w[:3] == b'\x55\xAA\x5A':
            # Frame header, send payload next
            time.sleep(0.005)
            if i + 1 < len(hs):
                i += 1
                s.write(hs[i]); s.flush()
                time.sleep(0.1)
                # Read response
                resp = s.read(4096)
                if resp:
                    print(f"    hs response: {len(resp)} bytes")
        else:
            time.sleep(0.05)
        i += 1
    time.sleep(0.3)
    resp = s.read(4096)
    if resp:
        print(f"    final hs response: {len(resp)} bytes")


# Setup .NET SyncRGB
asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
sync_type = [t for t in asm.GetTypes() if t.Name == "LProtocolSyncRGB"][0]
base_type = [t for t in asm.GetTypes() if t.Name == "LProtocolBase"][0]
sync_status_type = [t for t in asm.GetTypes() if t.Name == "LProtocolSyncStatus"][0]

sync_instance = Activator.CreateInstance(sync_type)
gen_sync_rgb = sync_type.GetMethod("GenProtocolSyncRGB", ALL)
gen_frame = base_type.GetMethod("GenFramePackage", ALL)

# Get enum types
lp_attr_type = gen_frame.GetParameters()[0].ParameterType
lp_cmd_type = gen_frame.GetParameters()[1].ParameterType
LP_ATTR_REQ = System.Enum.Parse(lp_attr_type, "LP_ATTR_REQ")
LP_CMD_HEARTBEAT = System.Enum.Parse(lp_cmd_type, "LP_CMD_HEARTBEAT")

# GenSyncStatusPackage
gen_sync_status = sync_status_type.GetMethod("GenSyncStatusPackage", ALL)


def make_heartbeat():
    """Generate heartbeat packet."""
    result = gen_frame.Invoke(None, Array[System.Object]([
        LP_ATTR_REQ, LP_CMD_HEARTBEAT, Array[Byte]([])
    ]))
    return bytes(result) if result else None


def make_sync_status():
    """Generate SyncStatus packet."""
    result = gen_sync_status.Invoke(None, Array[System.Object]([]))
    return bytes(result) if result else None


def make_syncrgb(rows, cols, colors_rgb):
    """Generate SyncRGB packet."""
    n = rows * cols
    colors = Array[Color]([Color.FromArgb(r, g, b) for r, g, b in colors_rgb[:n]])
    result = gen_sync_rgb.Invoke(sync_instance, Array[System.Object]([
        Byte(rows), Byte(cols), colors
    ]))
    return bytes(result) if result else None


def send_split(s, pkt, delay=0.033):
    """Send packet split: frame header (5 bytes) then payload."""
    if pkt[:3] == b'\x55\xAA\x5A':
        frame = pkt[:5]
        payload = pkt[5:]
        s.write(frame); s.flush()
        time.sleep(0.005)
        s.write(payload); s.flush()
    else:
        s.write(pkt); s.flush()
    time.sleep(delay)


def send_syncrgb_loop_split(s, rows, cols, colors_rgb, duration=4.0, heartbeat_interval=10):
    """Send SyncRGB packets split, with periodic heartbeats."""
    start = time.time()
    sent = 0; hb_count = 0
    while time.time() - start < duration:
        # Heartbeat every N frames
        if heartbeat_interval and sent > 0 and sent % heartbeat_interval == 0:
            hb = make_heartbeat()
            if hb:
                send_split(s, hb, delay=0.01)
                hb_count += 1
            # Read any response
            s.read(4096)

        pkt = make_syncrgb(rows, cols, colors_rgb)
        if pkt is None:
            print("  [!] GenProtocolSyncRGB returned None"); break
        send_split(s, pkt)
        sent += 1
    return sent, hb_count


def main():
    writes = load_writes(CSV)
    cap_hs = get_capture_handshake(writes)

    red75 = [(255, 0, 0)] * NUM
    pattern = [(255,0,0)]*19 + [(0,255,0)]*19 + [(0,0,255)]*19 + [(255,255,0)]*18

    # Test packets
    pkt_1x75 = make_syncrgb(1, 75, red75)
    pkt_75x1 = make_syncrgb(75, 1, red75)
    print(f"SyncRGB(1,75): {len(pkt_1x75)}B, payload={len(pkt_1x75)-5}")
    print(f"SyncRGB(75,1): {len(pkt_75x1)}B, payload={len(pkt_75x1)-5}")

    hb = make_heartbeat()
    ss = make_sync_status()
    print(f"Heartbeat: {len(hb) if hb else 'None'}B")
    print(f"SyncStatus: {len(ss) if ss else 'None'}B")

    from soulight.protocol.raw_git_replay import RawGitReplayProtocol
    repo = os.path.dirname(os.path.abspath(__file__))
    protocol = RawGitReplayProtocol(repo_path=repo)

    # === Phase A: Capture hs (with response reads) + split SyncRGB(75,1) RED ===
    print("\n--- A: Capture hs + split SyncRGB(75,1) RED + heartbeats ---")
    s = openp()
    try:
        send_hs_with_reads(s, cap_hs)
        n, hb = send_syncrgb_loop_split(s, 75, 1, red75, 4.0, heartbeat_interval=10)
        print(f"  sent={n} heartbeats={hb}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase B: Historical hs + split SyncRGB(75,1) RED + heartbeats ===
    print("\n--- B: Historical hs + split SyncRGB(75,1) RED + heartbeats ---")
    s = openp()
    try:
        for w in protocol.iter_handshake_writes():
            s.write(w); s.flush()
            time.sleep(0.005 if len(w) == 5 else 0.01)
        time.sleep(0.5); s.read(4096)
        n, hb = send_syncrgb_loop_split(s, 75, 1, red75, 4.0, heartbeat_interval=10)
        print(f"  sent={n} heartbeats={hb}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase C: Capture hs + SyncStatus + split SyncRGB(75,1) RED ===
    print("\n--- C: Capture hs + SyncStatus + split SyncRGB(75,1) RED ---")
    s = openp()
    try:
        send_hs_with_reads(s, cap_hs)
        # Send SyncStatus packet
        if ss:
            send_split(s, ss, delay=0.1)
            resp = s.read(4096)
            if resp: print(f"  SyncStatus response: {len(resp)} bytes")
        n, hb = send_syncrgb_loop_split(s, 75, 1, red75, 4.0, heartbeat_interval=10)
        print(f"  sent={n} heartbeats={hb}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase D: Capture hs + split SyncRGB(1,75) RED (period=1 payload) ===
    print("\n--- D: Capture hs + split SyncRGB(1,75) RED ---")
    s = openp()
    try:
        send_hs_with_reads(s, cap_hs)
        n, hb = send_syncrgb_loop_split(s, 1, 75, red75, 4.0, heartbeat_interval=10)
        print(f"  sent={n} heartbeats={hb}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # === Phase E: Capture hs + 60 ORIGINAL capture + split SyncRGB(75,1) ===
    print("\n--- E: Capture hs + 60 orig capture + split SyncRGB(75,1) RED ---")
    s = openp()
    try:
        send_hs_with_reads(s, cap_hs)
        # Send 60 original capture color writes
        fb = next((i for i, w in enumerate(writes) if len(w) >= 238), len(writes))
        color_start = fb - 1 if writes[fb-1][:3] == b'\x55\xAA\x5A' else fb
        for w in writes[color_start:color_start+120]:  # ~60 pairs
            s.write(w); s.flush()
            if len(w) == 5: time.sleep(0.005)
            elif len(w) >= 238: time.sleep(0.033)
            else: time.sleep(0.01)
        time.sleep(0.5)
        # Then SyncRGB
        n, hb = send_syncrgb_loop_split(s, 75, 1, red75, 4.0, heartbeat_interval=10)
        print(f"  sent={n} heartbeats={hb}"); time.sleep(3)
    finally: s.close()

    print("\n[Done] Report:")
    print("  A (cap hs+split 75x1):         ___")
    print("  B (hist hs+split 75x1):        ___")
    print("  C (cap hs+SyncStatus+75x1):    ___")
    print("  D (cap hs+split 1x75):         ___")
    print("  E (cap hs+60orig+split 75x1):  ___")

if __name__ == "__main__":
    main()
