# -*- coding: utf-8 -*-
import sys, os
sys.stdout.reconfigure(encoding="utf-8")
BASE = r"c:\Users\n0souls\Documents\GitHub\Soulight"

csvs = ["black", "green", "blue", "white", "surely_full_red", "red", "red_full", "redish"]

for name in csvs:
    path = os.path.join(BASE, name + ".csv")
    if not os.path.exists(path):
        continue
    writes = []
    with open(path, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append(raw)

    n238 = sum(1 for w in writes if len(w) == 238)
    hs = next((i for i, r in enumerate(writes) if len(r) >= 238), len(writes))
    print("%s: %d writes, %d color, handshake=%d" % (name, len(writes), n238, hs))
