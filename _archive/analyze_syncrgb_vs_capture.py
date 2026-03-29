# -*- coding: utf-8 -*-
"""
analyze_syncrgb_vs_capture.py — Compare GenProtocolSyncRGB packets
with capture packets to find structural differences.

1. Generate 30 GenProtocolSyncRGB packets, analyze payload sizes and structure
2. Compare decrypted headers with capture packets
3. Check if packet sizes overlap with capture range (238-245)
4. Also compare with GenRGBTransferPackage for reference
"""
import os, sys, clr
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

BEELIGHT_EXE = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte, Activator
from System.Drawing import Color
import System
from collections import Counter

ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
CSV = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")


def derive_key(cipher, period):
    """Derive XOR key from cipher assuming plain[2:2+period]=0."""
    key = [0] * period
    for i in range(period):
        src = 2 + ((i - 2) % period)
        key[i] = cipher[src]
    return bytes(key)


def decrypt(cipher, key):
    period = len(key)
    return bytes(cipher[i] ^ key[i % period] for i in range(len(cipher)))


def main():
    # Setup .NET
    asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
    sync_type = [t for t in asm.GetTypes() if t.Name == "LProtocolSyncRGB"][0]
    sync_inst = Activator.CreateInstance(sync_type)
    gen_sync = sync_type.GetMethod("GenProtocolSyncRGB", ALL)

    ctrl_type = [t for t in asm.GetTypes() if t.Name == "LProtocolCtrl"][0]
    # Find RGB[] overload
    from soulight.protocol.bridge import BeelightBridge
    bridge = BeelightBridge()
    bridge.init()

    # ================================================================
    # 1. Generate 30 GenProtocolSyncRGB packets (all RED)
    # ================================================================
    print("=" * 70)
    print("  GenProtocolSyncRGB PACKET ANALYSIS")
    print("=" * 70)

    sync_sizes = Counter()
    sync_pkts = []
    for i in range(30):
        colors = Array[Color]([Color.Red] * 75)
        for rows, cols in [(1, 75), (75, 1), (3, 25)]:
            result = gen_sync.Invoke(sync_inst, Array[System.Object]([
                Byte(rows), Byte(cols), colors
            ]))
            if result:
                pkt = bytes(result)
                payload = pkt[5:]
                sync_sizes[len(payload)] += 1
                sync_pkts.append({"rows": rows, "cols": cols, "pkt": pkt, "payload": payload})

    print(f"\n  Generated {len(sync_pkts)} GenProtocolSyncRGB packets")
    print(f"  Payload size distribution:")
    for sz, cnt in sorted(sync_sizes.items()):
        in_range = "IN RANGE" if 238 <= sz <= 245 else "OUT OF RANGE"
        period = sz - 235
        print(f"    payload={sz}: {cnt} packets (period={period}) [{in_range}]")

    # ================================================================
    # 2. Decrypt GenProtocolSyncRGB headers
    # ================================================================
    print(f"\n  --- GenProtocolSyncRGB decrypted headers ---")
    for i, sp in enumerate(sync_pkts[:6]):
        payload = sp["payload"]
        pl = len(payload)
        period = pl - 235
        if period < 1:
            print(f"  [{i}] payload={pl} period={period} INVALID"); continue
        color_start = pl - 225

        # Try decryption
        key = derive_key(payload, period)
        plain = decrypt(payload, key)

        print(f"\n  [{i}] rows={sp['rows']} cols={sp['cols']} payload={pl} period={period}")
        print(f"    key = {key.hex()}")
        print(f"    plain header ({color_start}B) = {plain[:color_start].hex()}")
        print(f"    LED[0:3] = ({plain[color_start]},{plain[color_start+1]},{plain[color_start+2]})")

        # Check for 05 05 pattern
        for pos in range(2, color_start - 1):
            if plain[pos] == 0x05 and plain[pos + 1] == 0x05:
                print(f"    05 05 at [{pos},{pos+1}]")
                if pos + 5 < color_start:
                    print(f"    brightness [{pos+2}] = 0x{plain[pos+2]:02x}")
                    print(f"    byte [{pos+3}] = 0x{plain[pos+3]:02x}")
                    print(f"    LED count [{pos+5}] = 0x{plain[pos+5]:02x} ({plain[pos+5]})")
                break
        else:
            # No 05 05 found, dump full header
            print(f"    NO 05 05 pattern found!")

    # ================================================================
    # 3. GenRGBTransferPackage headers for comparison
    # ================================================================
    print(f"\n" + "=" * 70)
    print(f"  GenRGBTransferPackage COMPARISON (RED)")
    print(f"=" * 70)

    for i in range(3):
        pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
        if pkt:
            payload = pkt[5:]
            pl = len(payload)
            period = pl - 235
            key = derive_key(payload, period)
            plain = decrypt(payload, key)
            color_start = pl - 225
            print(f"\n  [{i}] payload={pl} period={period}")
            print(f"    plain header = {plain[:color_start].hex()}")
            print(f"    LED[0:3] = ({plain[color_start]},{plain[color_start+1]},{plain[color_start+2]})")

    # ================================================================
    # 4. Capture packets for comparison
    # ================================================================
    print(f"\n" + "=" * 70)
    print(f"  CAPTURE PACKET COMPARISON")
    print(f"=" * 70)

    writes = []
    with open(CSV, "r", errors="replace") as f:
        for n, line in enumerate(f, 1):
            if n == 1: continue
            p = line.split(";")
            if len(p) < 6: continue
            if "IRP_MJ_WRITE" not in p[2] or p[3].strip() != "DOWN": continue
            d = p[5].strip()
            if not d: continue
            try: raw = bytes.fromhex(d.replace(" ", ""))
            except: continue
            if 238 <= len(raw) <= 245:
                writes.append(raw)

    for i, raw in enumerate(writes[:6]):
        pl = len(raw)
        period = pl - 235
        key = derive_key(raw, period)
        plain = decrypt(raw, key)
        color_start = pl - 225
        print(f"\n  [{i}] payload={pl} period={period}")
        print(f"    plain header = {plain[:color_start].hex()}")
        print(f"    LED[0:6] = ({plain[color_start]},{plain[color_start+1]},{plain[color_start+2]}) "
              f"({plain[color_start+3]},{plain[color_start+4]},{plain[color_start+5]})")

        # Check for 05 05
        for pos in range(2, color_start - 1):
            if plain[pos] == 0x05 and plain[pos + 1] == 0x05:
                print(f"    05 05 at [{pos},{pos+1}]")
                if pos + 5 < color_start:
                    print(f"    brightness [{pos+2}] = 0x{plain[pos+2]:02x}")
                    print(f"    E3 byte [{pos+3}] = 0x{plain[pos+3]:02x}")
                    print(f"    LED count [{pos+5}] = 0x{plain[pos+5]:02x} ({plain[pos+5]})")
                break

    # ================================================================
    # 5. KEY STRUCTURAL COMPARISON
    # ================================================================
    print(f"\n" + "=" * 70)
    print(f"  STRUCTURAL COMPARISON SUMMARY")
    print(f"=" * 70)

    # Compare: nonce bytes, header patterns, LP_CMD encoding
    # For each type, show the plaintext byte at specific positions

    print(f"\n  Position analysis (relative to color_start):")
    print(f"  {'Type':<20} {'PayloadLen':<12} {'Header[-7]':<10} {'Header[-6:-4]':<14} {'Header[-5]':<10} {'Header[-4]':<10} {'Header[-2:-1]':<12}")

    # GenProtocolSyncRGB samples
    for sp in sync_pkts[:3]:
        payload = sp["payload"]
        pl = len(payload)
        period = pl - 235
        if period < 1: continue
        key = derive_key(payload, period)
        plain = decrypt(payload, key)
        cs = pl - 225
        # Show relative positions from color_start
        print(f"  {'SyncRGB':<20} {pl:<12} "
              f"0x{plain[cs-7]:02x}       "
              f"{plain[cs-6]:02x} {plain[cs-5]:02x}       "
              f"0x{plain[cs-4]:02x}       "
              f"0x{plain[cs-3]:02x}       "
              f"{plain[cs-2]:02x} {plain[cs-1]:02x}")

    # GenRGBTransferPackage samples
    for i in range(3):
        pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
        if pkt:
            payload = pkt[5:]
            pl = len(payload)
            period = pl - 235
            key = derive_key(payload, period)
            plain = decrypt(payload, key)
            cs = pl - 225
            print(f"  {'RGBTransfer':<20} {pl:<12} "
                  f"0x{plain[cs-7]:02x}       "
                  f"{plain[cs-6]:02x} {plain[cs-5]:02x}       "
                  f"0x{plain[cs-4]:02x}       "
                  f"0x{plain[cs-3]:02x}       "
                  f"{plain[cs-2]:02x} {plain[cs-1]:02x}")

    # Capture samples
    for raw in writes[:3]:
        pl = len(raw)
        period = pl - 235
        key = derive_key(raw, period)
        plain = decrypt(raw, key)
        cs = pl - 225
        print(f"  {'Capture':<20} {pl:<12} "
              f"0x{plain[cs-7]:02x}       "
              f"{plain[cs-6]:02x} {plain[cs-5]:02x}       "
              f"0x{plain[cs-4]:02x}       "
              f"0x{plain[cs-3]:02x}       "
              f"{plain[cs-2]:02x} {plain[cs-1]:02x}")


if __name__ == "__main__":
    main()
