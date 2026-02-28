# -*- coding: utf-8 -*-
"""
analyze_session_start.py - Analyze the beginning of a capture session.
Look for handshake/connect packets that switch the controller mode.

From LProtocolBase:
  LP_CMD_HEARTBEAT = 0
  LP_CMD_FIRM = 1
  LP_CMD_SYNCSTATUS = 2
  LP_CMD_SYNCCONFIG = 3
  LP_CMD_OTA = 4
  LP_CMD_CTRL_DEVICE = 5
  LP_CMD_CTRL_SYNC_RGB = 6
  LP_CMD_CTRL_LOG = 26

  LP_CTRL_WORKMODE = 6
  LP_WK_MODE_PC = 0

The app probably sends: CTRL_DEVICE(WORKMODE=PC) to take over the strip.

GenFramePackage wraps: [55 AA 5A] [len] [attr] [cmd] [data...]
Wait - attr is byte[4], not always 0x00!
Heartbeat: [55 AA 5A F1 00] - here F1 might be cmd, not len!

Let me re-examine the heartbeat format.
"""
import sys
import os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_all_writes(filepath):
    """Parse ALL write operations in order."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
            if "IRP_MJ_WRITE;DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            data = parts[5].strip()
            try:
                raw = bytes.fromhex(data.replace(" ", ""))
            except ValueError:
                continue
            writes.append((line_no, raw))
    return writes


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    # Analyze the beginning of each capture file
    for color in ["black", "red", "green", "blue", "white"]:
        path = os.path.join(CSV_DIR, f"{color}.csv")
        if not os.path.exists(path):
            continue

        writes = parse_all_writes(path)
        print(f"\n{'='*60}")
        print(f"{color.upper()} - First 30 writes")
        print(f"{'='*60}")

        for i, (line_no, raw) in enumerate(writes[:30]):
            hex_str = raw.hex()
            if len(hex_str) > 60:
                hex_str = hex_str[:60] + "..."

            pkt_type = "?"
            if len(raw) == 5 and raw[0] == 0x55 and raw[1] == 0xAA and raw[2] == 0x5A:
                pkt_type = f"FRAME_HDR len={raw[3]} attr={raw[4]:02x}"
            elif len(raw) > 5 and raw[0] == 0x55 and raw[1] == 0xAA and raw[2] == 0x5A:
                pkt_type = f"FULL_FRAME len_byte={raw[3]} total={len(raw)}"
            else:
                pkt_type = f"DATA len={len(raw)}"

            print(f"  [{i:3d}] line={line_no:5d} {pkt_type:30s} [{hex_str}]")

    # Also analyze the controller's FIRST responses
    print(f"\n{'='*60}")
    print("Analyzing controller responses (READ operations)")
    print(f"{'='*60}")

    for color in ["black", "red"]:
        path = os.path.join(CSV_DIR, f"{color}.csv")
        if not os.path.exists(path):
            continue

        print(f"\n--- {color.upper()} first 20 READs ---")
        reads = []
        with open(path, "r", errors="replace") as f:
            for line_no, line in enumerate(f):
                if "IRP_MJ_READ" not in line:
                    continue
                parts = line.split(";")
                if len(parts) <= 7:
                    continue
                data = parts[5].strip()
                try:
                    raw = bytes.fromhex(data.replace(" ", ""))
                except ValueError:
                    continue
                reads.append((line_no, raw))
                if len(reads) >= 20:
                    break

        for i, (line_no, raw) in enumerate(reads):
            hex_str = raw.hex()
            if len(hex_str) > 80:
                hex_str = hex_str[:80] + "..."
            
            # Parse 55 AA 5A response packets
            pkts_info = []
            j = 0
            while j < len(raw) - 4:
                if raw[j] == 0x55 and raw[j+1] == 0xAA and raw[j+2] == 0x5A:
                    pkt_len = raw[j+3]
                    attr = raw[j+4] if j+4 < len(raw) else 0
                    pkts_info.append(f"[len={pkt_len} attr={attr:02x}]")
                    j += 5 + pkt_len
                else:
                    j += 1
            
            print(f"  [{i:3d}] line={line_no:5d} len={len(raw):4d} "
                  f"pkts={'  '.join(pkts_info[:3])}")

    # ================================================================
    # Check: what does the Beelight app send DIFFERENTLY from our script?
    # Our heartbeat: 55 AA 5A F1 00
    # From captures, the "heartbeat" byte[3] varies: EE-F5
    # But that's for data frames, not pure heartbeats.
    #
    # Are there any SMALL writes that aren't 5-byte frames?
    # ================================================================
    print(f"\n{'='*60}")
    print("All unique write patterns in BLACK capture")
    print(f"{'='*60}")

    all_writes = parse_all_writes(os.path.join(CSV_DIR, "black.csv"))
    by_len = {}
    for _, raw in all_writes:
        plen = len(raw)
        if plen not in by_len:
            by_len[plen] = []
        by_len[plen].append(raw)

    for plen in sorted(by_len.keys()):
        pkts = by_len[plen]
        print(f"\n  len={plen}: {len(pkts)} writes")
        # Show first few unique values
        unique = set()
        for p in pkts[:20]:
            if len(p) <= 10:
                unique.add(p.hex())
        if unique:
            for u in sorted(unique):
                count = sum(1 for p in pkts if p.hex() == u)
                print(f"    {u} x{count}")
        else:
            # Show first 3
            for p in pkts[:3]:
                print(f"    {p[:20].hex()}{'...' if len(p) > 20 else ''}")

    # ================================================================
    # Check: the frame header byte[4] - is it always 0x00?
    # Or does it carry the attr/cmd info?
    # ================================================================
    print(f"\n{'='*60}")
    print("Frame header byte[4] analysis (BLACK)")
    print(f"{'='*60}")

    frame_hdrs = [raw for _, raw in all_writes if len(raw) == 5
                  and raw[0] == 0x55 and raw[1] == 0xAA and raw[2] == 0x5A]

    byte4_vals = set(h[4] for h in frame_hdrs)
    print(f"  Unique byte[4] values: {[f'{v:02x}' for v in sorted(byte4_vals)]}")

    byte3_vals = set(h[3] for h in frame_hdrs)
    print(f"  Unique byte[3] values: {sorted(byte3_vals)} = {[f'{v:02x}' for v in sorted(byte3_vals)]}")

    # ================================================================
    # KEY: What if byte[3] is NOT the payload length but the CMD?
    # Heartbeat: 55 AA 5A F1 00 -> cmd=0xF1? But that's not in LP_CMD enum.
    # Or maybe GenFramePackage format is:
    #   [55 AA 5A] [data_len] [attr] [cmd] [data...]
    # where attr=0 (LP_ATTR_REQ) and cmd is in the data?
    # 
    # Wait - we confirmed that byte[3] = following payload length (238=0xEE etc).
    # For "heartbeats": byte[3] ranges EE-F5 which = 238-245.
    # These ARE frame headers for data packets!
    #
    # But there are also writes where byte[3] is small (7, 8, 9, etc)?
    # Let me check.
    # ================================================================

    small_frames = [h for h in frame_hdrs if h[3] < 50]
    if small_frames:
        print(f"\n  Small frame headers (len < 50): {len(small_frames)}")
        for h in small_frames[:10]:
            print(f"    {h.hex()}")
    else:
        print(f"\n  No small frame headers found")
        # Then ALL frame headers are data (len 238-245)?
        # The actual heartbeat must be embedded differently
        # Or the heartbeat IS just a raw 5-byte packet without 55AA5A frame

    # Check if there are pure heartbeat-like writes
    non_frame_small = [raw for _, raw in all_writes 
                       if len(raw) <= 10 and not (len(raw) == 5 and raw[0] == 0x55)]
    if non_frame_small:
        print(f"\n  Non-frame small writes: {len(non_frame_small)}")
        for p in non_frame_small[:5]:
            print(f"    {p.hex()}")

    print("\nDONE")


if __name__ == "__main__":
    main()
