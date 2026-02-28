# -*- coding: utf-8 -*-
"""
analyze_key_exchange.py - Analyze the session key exchange.

The handshake replay works (LED turns on), but colors are wrong because
the cipher key is session-specific. The controller's FIRST response likely
contains session key material.

We need to:
1. Look at the controller's first response after the first handshake write
2. Compare responses between sessions (surely_full_red vs redish)
3. Find what bytes change (session-specific) vs stay fixed (protocol)
4. The session key is likely derived from those changing bytes
"""
import sys, os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_all_ops(filepath):
    """Parse WRITE/READ operations in order."""
    ops = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            d = ""
            if "IRP_MJ_WRITE" in line and "DOWN" in line:
                d = "W"
            elif "IRP_MJ_READ" in line and "UP" in line:
                d = "R"
            else:
                continue
            data_str = parts[5].strip()
            try:
                raw = bytes.fromhex(data_str.replace(" ", "")) if data_str else b""
            except ValueError:
                raw = b""
            if raw:
                ops.append((line_no, d, raw))
    return ops


def parse_55aa5a_packets(raw):
    """Extract individual 55AA5A-framed packets from a raw read."""
    pkts = []
    i = 0
    while i < len(raw) - 4:
        if raw[i] == 0x55 and raw[i+1] == 0xAA and raw[i+2] == 0x5A:
            plen = raw[i+3]
            attr = raw[i+4]
            payload = raw[i+5:i+5+plen]
            pkts.append({'len': plen, 'attr': attr, 'payload': payload, 'offset': i})
            i += 5 + plen
        else:
            i += 1
    return pkts


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    files = {
        'surely_full_red': os.path.join(CSV_DIR, "__pycache__", "surely_full_red.csv"),
        'red_full': os.path.join(CSV_DIR, "red_full.csv"),
        'redish': os.path.join(CSV_DIR, "redish.csv"),
    }

    sessions = {}
    for name, path in files.items():
        if not os.path.exists(path):
            continue
        ops = parse_all_ops(path)
        sessions[name] = ops
        print(f"{name}: {len(ops)} ops")

    # ================================================================
    # Extract the FIRST controller response from each session
    # ================================================================
    print(f"\n{'='*70}")
    print("FIRST CONTROLLER RESPONSE IN EACH SESSION")
    print(f"{'='*70}")

    first_responses = {}
    for name, ops in sessions.items():
        reads = [(ln, raw) for ln, d, raw in ops if d == 'R']
        if reads:
            ln, raw = reads[0]
            pkts = parse_55aa5a_packets(raw)
            first_responses[name] = (raw, pkts)
            print(f"\n  {name}: first READ at line {ln}, {len(raw)} bytes, {len(pkts)} sub-packets")
            for j, p in enumerate(pkts[:5]):
                print(f"    pkt[{j}] len={p['len']} attr={p['attr']:02x} "
                      f"payload={p['payload'].hex()}")
        else:
            print(f"\n  {name}: NO reads found!")

    # ================================================================
    # Compare first responses between sessions
    # ================================================================
    print(f"\n{'='*70}")
    print("COMPARING FIRST RESPONSES")
    print(f"{'='*70}")

    resp_names = list(first_responses.keys())
    for i in range(len(resp_names)):
        for j in range(i+1, len(resp_names)):
            n1, n2 = resp_names[i], resp_names[j]
            raw1, pkts1 = first_responses[n1]
            raw2, pkts2 = first_responses[n2]

            print(f"\n  {n1} vs {n2}:")
            print(f"    Sizes: {len(raw1)} vs {len(raw2)}")
            print(f"    Sub-packet counts: {len(pkts1)} vs {len(pkts2)}")

            # Compare corresponding sub-packets
            for k in range(min(len(pkts1), len(pkts2))):
                p1 = pkts1[k]['payload']
                p2 = pkts2[k]['payload']
                if len(p1) == len(p2):
                    if p1 == p2:
                        print(f"    sub[{k}] len={len(p1)} IDENTICAL")
                    else:
                        xor = bytes(a ^ b for a, b in zip(p1, p2))
                        same_pos = [m for m in range(len(p1)) if p1[m] == p2[m]]
                        diff_pos = [m for m in range(len(p1)) if p1[m] != p2[m]]
                        print(f"    sub[{k}] len={len(p1)}")
                        print(f"      p1={p1.hex()}")
                        print(f"      p2={p2.hex()}")
                        print(f"      xor={xor.hex()}")
                        print(f"      same@{same_pos}")
                        print(f"      diff@{diff_pos}")
                else:
                    print(f"    sub[{k}] LEN MISMATCH: {len(p1)} vs {len(p2)}")

    # ================================================================
    # Full handshake exchange: interleave W/R to see the conversation
    # ================================================================
    print(f"\n{'='*70}")
    print("FULL HANDSHAKE CONVERSATION (redish.csv)")
    print(f"{'='*70}")

    if 'redish' in sessions:
        ops = sessions['redish']
        # Find first big write
        first_big_line = None
        for ln, d, raw in ops:
            if d == 'W' and len(raw) >= 238:
                first_big_line = ln
                break

        print(f"  First big write at line {first_big_line}")
        print()

        msg_count = 0
        for ln, d, raw in ops:
            if first_big_line and ln >= first_big_line:
                break

            if d == 'W':
                is_hdr = len(raw) == 5 and raw[0:3] == b'\x55\xAA\x5A'
                if is_hdr:
                    print(f"  [{msg_count:3d}] W HDR  {raw.hex()}")
                else:
                    print(f"  [{msg_count:3d}] W DAT  len={len(raw):3d} {raw.hex()[:60]}")
                msg_count += 1

            elif d == 'R':
                pkts = parse_55aa5a_packets(raw)
                for p in pkts:
                    print(f"  [{msg_count:3d}] R PKT  len={p['len']:3d} "
                          f"attr={p['attr']:02x} {p['payload'].hex()[:60]}")
                    msg_count += 1

    # ================================================================
    # KEY ANALYSIS: Look at the first write + first response in detail
    # The first handshake packet likely sends our "hello" and the
    # controller responds with session parameters including key material
    # ================================================================
    print(f"\n{'='*70}")
    print("FIRST EXCHANGE DETAIL (per session)")
    print(f"{'='*70}")

    for name, ops in sessions.items():
        writes = [(ln, raw) for ln, d, raw in ops if d == 'W']
        reads = [(ln, raw) for ln, d, raw in ops if d == 'R']

        if len(writes) < 2 or not reads:
            continue

        # First write pair: header + data
        w_hdr = writes[0][1] if writes[0][1][0:3] == b'\x55\xAA\x5A' else None
        w_data = writes[1][1] if len(writes) > 1 else None

        print(f"\n  {name}:")
        if w_hdr:
            print(f"    First W header: {w_hdr.hex()}")
        if w_data:
            print(f"    First W data:   {w_data.hex()} (len={len(w_data)})")

        # First read
        r_raw = reads[0][1]
        r_pkts = parse_55aa5a_packets(r_raw)
        print(f"    First R: {len(r_raw)} bytes, {len(r_pkts)} sub-packets")
        if r_pkts:
            p = r_pkts[0]
            print(f"      First sub-pkt: len={p['len']} payload={p['payload'].hex()}")

    # ================================================================
    # Check if any response bytes correlate with the byte[1] values
    # in subsequent data packets
    # ================================================================
    print(f"\n{'='*70}")
    print("CORRELATION: RESPONSE BYTES vs DATA BYTE[1]")
    print(f"{'='*70}")

    for name, ops in sessions.items():
        reads = [(ln, raw) for ln, d, raw in ops if d == 'R']
        if not reads:
            continue
        
        first_r_pkts = parse_55aa5a_packets(reads[0][1])
        if not first_r_pkts:
            continue
        
        resp_payload = first_r_pkts[0]['payload']
        
        # Get byte[1] from first few data writes
        writes = [(ln, raw) for ln, d, raw in ops if d == 'W' and len(raw) >= 238]
        if writes:
            b1_vals = set(raw[1] for _, raw in writes[:10])
            print(f"  {name}:")
            print(f"    Response[0:16]: {resp_payload[:16].hex()}")
            print(f"    Data byte[1]: {[f'{v:02x}' for v in sorted(b1_vals)]}")
            
            # Check if response contains any of the byte[1] values
            for v in sorted(b1_vals):
                positions = [k for k in range(len(resp_payload)) if resp_payload[k] == v]
                if positions:
                    print(f"    0x{v:02x} found in response at positions: {positions}")

    print("\nDONE")


if __name__ == "__main__":
    main()
