# -*- coding: utf-8 -*-
"""
test_warmup_perled.py — Warmup + per-LED control tests.

Phase A: 60 orig + 60 uniform RED (base=black) -> confirm Phase D result
Phase B: 60 orig + 60 NON-UNIFORM 4-color (base=black) -> per-LED?
Phase C: 20 orig + 80 uniform RED -> minimal warmup
Phase D: 5 orig + 95 uniform RED -> very minimal warmup
Phase E: 60 orig + 60 uniform GREEN (base=black) -> different color test
"""
import os, sys, time
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

def make_pairs(cw):
    ps = []; i = 0
    while i < len(cw):
        w = cw[i]
        if len(w) == 5 and w[:3] == b'\x55\xAA\x5A' and i+1 < len(cw) and len(cw[i+1]) >= 238:
            ps.append((w, cw[i+1])); i += 2
        else: i += 1
    return ps

def xor_uniform(pkt, target, base=(0,0,0)):
    n = len(pkt); cs = n - 225; r = bytearray(pkt)
    dr,dg,db = target[0]^base[0], target[1]^base[1], target[2]^base[2]
    for i in range(NUM):
        p = cs + i*3
        r[p] ^= dr; r[p+1] ^= dg; r[p+2] ^= db
    return bytes(r)

def xor_perled(pkt, targets, base=(0,0,0)):
    n = len(pkt); cs = n - 225; r = bytearray(pkt)
    br,bg,bb = base
    for i in range(NUM):
        p = cs + i*3
        tr,tg,tb = targets[i]
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

def send_pairs(s, ps, limit=999):
    c = 0
    for fh, pl in ps[:limit]:
        s.write(fh); s.flush(); time.sleep(0.005)
        s.write(pl); s.flush(); time.sleep(0.033); c += 1
    return c

def run_phase(name, hs, ps, warmup, mod_fn, total=100):
    print(f"\n--- {name} (warmup={warmup}) ---")
    s = openp()
    try:
        send_hs(s, hs)
        # Warmup: send original packets
        if warmup > 0:
            send_pairs(s, ps[:warmup])
        # Modified packets
        mod_start = warmup
        mod_pairs = [(fh, mod_fn(pl)) for fh, pl in ps[mod_start:mod_start + (total - warmup)]]
        n = send_pairs(s, mod_pairs)
        print(f"  warmup={warmup} modified={n}")
        time.sleep(3)
    finally:
        s.close()
    time.sleep(1.5)

def main():
    writes = load_writes(CSV)
    hs, cw = split(writes)
    ps = make_pairs(cw)
    print(f"hs={len(hs)} pairs={len(ps)}")

    # 4-color pattern for non-uniform test
    pattern = (
        [(255,0,0)]*19 + [(0,255,0)]*19 +
        [(0,0,255)]*19 + [(255,255,0)]*18
    )

    # Phase A: 60 orig + 40 uniform RED
    run_phase("A: 60 orig + RED", hs, ps, 60,
              lambda pl: xor_uniform(pl, (255,0,0)))

    # Phase B: 60 orig + 40 NON-UNIFORM 4-color
    run_phase("B: 60 orig + 4-color pattern", hs, ps, 60,
              lambda pl: xor_perled(pl, pattern))

    # Phase C: 20 orig + 80 RED
    run_phase("C: 20 orig + RED", hs, ps, 20,
              lambda pl: xor_uniform(pl, (255,0,0)))

    # Phase D: 5 orig + 95 RED
    run_phase("D: 5 orig + RED", hs, ps, 5,
              lambda pl: xor_uniform(pl, (255,0,0)))

    # Phase E: 60 orig + 40 GREEN
    run_phase("E: 60 orig + GREEN", hs, ps, 60,
              lambda pl: xor_uniform(pl, (0,255,0)))

    print("\n[Done] Report:")
    print("  A (60+RED):        ___")
    print("  B (60+4color):     ___")
    print("  C (20+RED):        ___")
    print("  D (5+RED):         ___")
    print("  E (60+GREEN):      ___")

if __name__ == "__main__":
    main()
