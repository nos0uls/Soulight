# -*- coding: utf-8 -*-
"""
analyze_startup.py - Analyze full session captures to find handshake.
"""
import sys
import os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_csv(filepath):
    """Parse all operations from CSV, preserving order."""
    ops = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            
            direction = ""
            if "IRP_MJ_WRITE" in line:
                if "DOWN" in line:
                    direction = "WRITE"
                else:
                    continue
            elif "IRP_MJ_READ" in line:
                if "UP" in line:
                    direction = "READ"
                else:
                    continue
            elif "IOCTL" in line:
                direction = "IOCTL"
            else:
                continue
            
            data = parts[5].strip() if len(parts) > 5 else ""
            try:
                raw = bytes.fromhex(data.replace(" ", "")) if data else b""
            except ValueError:
                raw = b""
            
            ops.append((line_no, direction, raw, line.strip()[:200]))
    return ops


def parse_wire_packets(raw):
    """Parse 55 AA 5A framed packets from raw bytes."""
    pkts = []
    i = 0
    while i < len(raw) - 4:
        if raw[i] == 0x55 and raw[i+1] == 0xAA and raw[i+2] == 0x5A:
            pkt_len = raw[i+3]
            attr = raw[i+4] if i+4 < len(raw) else 0xFF
            payload = raw[i+5:i+5+pkt_len] if i+5+pkt_len <= len(raw) else raw[i+5:]
            pkts.append({
                'offset': i,
                'len_byte': pkt_len,
                'attr': attr,
                'payload': payload,
                'full': raw[i:i+5+pkt_len]
            })
            i += 5 + pkt_len
        else:
            i += 1
    return pkts


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    
    files = [
        ("red_full.csv", os.path.join(CSV_DIR, "red_full.csv")),
        ("surely_full_red.csv", os.path.join(CSV_DIR, "__pycache__", "surely_full_red.csv")),
    ]
    
    for name, path in files:
        if not os.path.exists(path):
            print(f"SKIP: {name} not found")
            continue
        
        print(f"\n{'='*70}")
        print(f"FILE: {name}")
        print(f"{'='*70}")
        
        ops = parse_csv(path)
        print(f"Total operations: {len(ops)}")
        
        writes = [(ln, raw) for ln, d, raw, _ in ops if d == "WRITE"]
        reads = [(ln, raw) for ln, d, raw, _ in ops if d == "READ"]
        ioctls = [(ln, raw, txt) for ln, d, raw, txt in ops if d == "IOCTL"]
        
        print(f"  Writes: {len(writes)}, Reads: {len(reads)}, IOCTLs: {len(ioctls)}")
        
        # Show first IOCTLs (port config)
        if ioctls:
            print(f"\n  --- First IOCTLs (port setup) ---")
            for ln, raw, txt in ioctls[:5]:
                print(f"    line={ln}: {txt[:120]}")
        
        # Show ALL operations in order for first 100
        print(f"\n  --- First 100 operations (chronological) ---")
        for i, (ln, direction, raw, txt) in enumerate(ops[:100]):
            hex_str = raw.hex() if raw else "(empty)"
            if len(hex_str) > 80:
                hex_str = hex_str[:80] + "..."
            
            # Parse frame info
            info = ""
            if raw and len(raw) >= 5 and raw[0] == 0x55 and raw[1] == 0xAA and raw[2] == 0x5A:
                pkts = parse_wire_packets(raw)
                if pkts:
                    parts = []
                    for p in pkts[:3]:
                        parts.append(f"[len={p['len_byte']} attr={p['attr']:02x}]")
                    info = " PKTS:" + " ".join(parts)
                    if len(pkts) > 3:
                        info += f" +{len(pkts)-3}more"
            elif raw and len(raw) >= 3 and raw[0] == 0x4D and raw[1] == 0x67 and raw[2] == 0x4C:
                info = " MgL_FRAME"
            
            print(f"    [{i:3d}] {direction:5s} line={ln:5d} len={len(raw):4d}{info}")
            if len(raw) <= 30:
                print(f"          data={hex_str}")
            elif len(raw) <= 80:
                print(f"          data={raw[:30].hex()}...")
        
        # ================================================================
        # KEY ANALYSIS: Find the FIRST write that isn't a heartbeat
        # and the first SMALL write (potential handshake/mode-switch)
        # ================================================================
        print(f"\n  --- Write size distribution ---")
        size_counts = {}
        for _, raw in writes:
            sz = len(raw)
            size_counts[sz] = size_counts.get(sz, 0) + 1
        for sz in sorted(size_counts.keys()):
            c = size_counts[sz]
            # Show first example
            ex = next(raw for _, raw in writes if len(raw) == sz)
            ex_hex = ex.hex()[:40]
            print(f"    size={sz:4d}: {c:4d} writes  ex={ex_hex}...")
        
        # ================================================================
        # Look for small writes (handshake candidates)
        # ================================================================
        print(f"\n  --- Small writes (< 50 bytes) ---")
        small_writes = [(i, ln, raw) for i, (ln, raw) in enumerate(writes) if len(raw) < 50]
        for idx, ln, raw in small_writes[:20]:
            print(f"    write[{idx:3d}] line={ln:5d} len={len(raw)} data={raw.hex()}")
        if not small_writes:
            print(f"    (none found)")
        
        # ================================================================
        # Analyze READ responses - controller sends these
        # ================================================================
        print(f"\n  --- First 20 READ responses ---")
        for i, (ln, raw) in enumerate(reads[:20]):
            pkts = parse_wire_packets(raw)
            pkt_info = ""
            for p in pkts[:5]:
                pkt_info += f" [len={p['len_byte']} attr={p['attr']:02x}"
                if p['payload']:
                    pkt_info += f" d={p['payload'][:8].hex()}"
                pkt_info += "]"
            
            print(f"    read[{i:3d}] line={ln:5d} total_len={len(raw):4d}{pkt_info}")
    
    print("\nDONE")


if __name__ == "__main__":
    main()
