# -*- coding: utf-8 -*-
"""
test_xor_delta_patterns.py — XOR delta tests on capture packets.

Phase A: Invert all channels (XOR 255,255,255) on all packets
Phase B: XOR delta only on 239-245, skip 238
Phase C: Non-uniform 4-color pattern via XOR delta
Phase D: Send original capture first, then XOR-delta modified stream
"""
import os, sys, time
from collections import defaultdict
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import serial

COM = "COM7"; BAUD = 500000; NUM = 75
CSV = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")

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

def split(writes):
    fb = next((i for i, w in enumerate(writes) if len(w) >= 238), None)
    if fb is None: return writes, []
    hs = writes[:fb]
    if hs and hs[-1][:3] == b'\x55\xAA\x5A':
        return hs[:-1], writes[fb-1:]
    return hs, writes[fb:]

def pairs(cw):
    ps = []; i = 0
    while i < len(cw):
        w = cw[i]
        if len(w) == 5 and w[:3] == b'\x55\xAA\x5A' and i+1 < len(cw) and len(cw[i+1]) >= 238:
            ps.append((w, cw[i+1])); i += 2
        else: i += 1
    return ps

def xor_delta_uniform(pkt, target, base):
    n = len(pkt); cs = n - 225; r = bytearray(pkt)
    tr,tg,tb = target; br,bg,bb = base
    dr,dg,db = tr^br, tg^bg, tb^bb
    for i in range(NUM):
        p = cs + i*3
        r[p] ^= dr; r[p+1] ^= dg; r[p+2] ^= db
    return bytes(r)

def xor_delta_perled(pkt, targets, bases):
    n = len(pkt); cs = n - 225; r = bytearray(pkt)
    for i in range(NUM):
        p = cs + i*3
        tr,tg,tb = targets[i]; br,bg,bb = bases[i]
        r[p] ^= (tr^br); r[p+1] ^= (tg^bg); r[p+2] ^= (tb^bb)
    return bytes(r)

def openp():
    s = serial.Serial(COM, BAUD, timeout=0.5, write_timeout=0.5)
    s.dtr = True; s.rts = True; time.sleep(0.3); s.read(4096); return s

def send_hs(s, hs):
    for w in hs:
        s.write(w); s.flush()
        time.sleep(0.05 if len(w)==5 else 0.1)
    time.sleep(0.5); s.read(4096)

def send_pairs(s, ps, limit=80):
    c = 0
    for fh, pl in ps[:limit]:
        s.write(fh); s.flush(); time.sleep(0.005)
        s.write(pl); s.flush(); time.sleep(0.033); c += 1
    return c

def main():
    writes = load_writes(CSV)
    hs, cw = split(writes)
    ps = pairs(cw)
    ps238 = [(f,p) for f,p in ps if len(p)==238]
    psbig = [(f,p) for f,p in ps if 239<=len(p)<=245]
    print(f"hs={len(hs)} pairs={len(ps)} 238={len(ps238)} big={len(psbig)}")

    # Phase A: Invert all channels on ALL packets
    print("\n--- A: Invert all (XOR 255,255,255) ---")
    s = openp()
    try:
        send_hs(s, hs)
        mod = [(fh, xor_delta_uniform(pl, (0,0,0), (255,255,255))) for fh,pl in ps]
        n = send_pairs(s, mod, 100)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # Phase B: XOR delta only 239-245 -> RED, base=black, skip 238
    print("\n--- B: Only 239-245, XOR to RED (base=black) ---")
    s = openp()
    try:
        send_hs(s, hs)
        mod = [(fh, xor_delta_uniform(pl, (255,0,0), (0,0,0))) for fh,pl in psbig]
        n = send_pairs(s, mod, 100)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # Phase C: Non-uniform 4-color pattern on ALL packets
    print("\n--- C: Non-uniform 4-color pattern ---")
    pattern = ([(255,0,0)]*19 + [(0,255,0)]*19 + [(0,0,255)]*19 + [(255,255,0)]*18)
    base_black = [(0,0,0)]*NUM
    s = openp()
    try:
        send_hs(s, hs)
        mod = [(fh, xor_delta_perled(pl, pattern, base_black)) for fh,pl in ps]
        n = send_pairs(s, mod, 100)
        print(f"  sent={n}"); time.sleep(3)
    finally: s.close()
    time.sleep(1.5)

    # Phase D: Original capture first (60 frames), then switch to XOR-delta RED
    print("\n--- D: Original 60 frames, then XOR-delta RED 60 frames ---")
    s = openp()
    try:
        send_hs(s, hs)
        send_pairs(s, ps, 60)
        time.sleep(0.5)
        mod = [(fh, xor_delta_uniform(pl, (255,0,0), (0,0,0))) for fh,pl in ps[60:]]
        n = send_pairs(s, mod, 60)
        print(f"  orig=60 mod={n}"); time.sleep(3)
    finally: s.close()

    print("\n[Done] Report:")
    print("  A (invert all):     ___")
    print("  B (239-245 RED):    ___")
    print("  C (4-color pattern):___")
    print("  D (orig then RED):  ___")

if __name__ == "__main__":
    main()
