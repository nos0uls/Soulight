# -*- coding: utf-8 -*-
"""
analyze_framing.py — Понять как 1515-байтный вывод DLL превращается в 238-245 byte пакеты.

DLL: get_large_screendata_package → FUN_10001880(pout, cmd=1, attr=1, data_buf, 0x5dc)
  param_5 = 0x5dc = 1500 bytes data
  Total DLL output = 5 (magic+len) + 6 (IV) + 4 (cmd+attr+2rand) + 1500 = 1515 bytes

Captures: 238-245 byte packets, interleaved with 5-byte heartbeats.

Hypothesis: .NET splits the 1515-byte DLL output into ~6-7 chunks of ~238 bytes each,
possibly adding its own framing.

Let's check: for one "frame" (between heartbeats), how many data packets are there?
And do their total sizes add up to ~1515?
"""
import sys
import os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_all_writes_ordered(filepath):
    """Parse CSV preserving order, returning (length, raw_bytes) for each write."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
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
            if len(raw) >= 5:
                writes.append(raw)
    return writes


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    for color in ["black", "red"]:
        path = os.path.join(CSV_DIR, f"{color}.csv")
        writes = parse_all_writes_ordered(path)

        print(f"{'='*70}")
        print(f"{color.upper()}: {len(writes)} total writes")
        print(f"{'='*70}")

        # Group writes into frames: each frame = [heartbeat] + [data packets until next heartbeat]
        frames = []
        current_frame = []

        for w in writes:
            is_hb = (len(w) == 5 and w[0] == 0x55 and w[1] == 0xAA and w[2] == 0x5A)
            if is_hb:
                if current_frame:
                    frames.append(current_frame)
                current_frame = []
            else:
                current_frame.append(w)

        if current_frame:
            frames.append(current_frame)

        print(f"Frames (groups between heartbeats): {len(frames)}")

        # Analyze frame sizes
        for fidx, frame in enumerate(frames[:10]):
            sizes = [len(p) for p in frame]
            total = sum(sizes)
            size_str = "+".join(str(s) for s in sizes)
            print(f"  Frame {fidx}: {len(frame)} packets, sizes=[{size_str}] total={total}")

        # Statistics
        if frames:
            pkt_counts = [len(f) for f in frames]
            totals = [sum(len(p) for p in f) for f in frames]
            print(f"\n  Packets per frame: min={min(pkt_counts)} max={max(pkt_counts)} "
                  f"avg={sum(pkt_counts)/len(pkt_counts):.1f}")
            print(f"  Bytes per frame:   min={min(totals)} max={max(totals)} "
                  f"avg={sum(totals)/len(totals):.1f}")
            print(f"  Expected DLL output: 1515 bytes")

        # Check: is each frame exactly 1 packet?
        single_pkt_frames = sum(1 for f in frames if len(f) == 1)
        print(f"  Single-packet frames: {single_pkt_frames}/{len(frames)}")

        # Check byte[1] pattern more carefully
        print(f"\n  Byte[1] by packet length (all data packets):")
        from collections import defaultdict
        b1_by_len = defaultdict(set)
        for frame in frames:
            for pkt in frame:
                b1_by_len[len(pkt)].add(pkt[1])

        for plen in sorted(b1_by_len.keys()):
            vals = b1_by_len[plen]
            vals_hex = ', '.join(f'0x{v:02x}' for v in sorted(vals))
            print(f"    len={plen}: byte[1] = [{vals_hex}]")

        # Check if consecutive packets in a frame share any structure
        print(f"\n  Consecutive packet analysis (frame 0):")
        if frames:
            frame = frames[0]
            for i, pkt in enumerate(frame):
                hdr = ' '.join(f'{b:02x}' for b in pkt[:15])
                print(f"    [{i}] len={len(pkt):3d} [{hdr}]")

        print()


if __name__ == "__main__":
    main()
