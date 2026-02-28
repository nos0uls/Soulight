# unpack_test.py - minimal unpack test, no stdout wrapping
import sys, os, ctypes, struct

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "Novaya papka", "Dumps", "beelightLib.dll")
# Try alternate path with cyrillic
if not os.path.exists(DLL_PATH):
    DLL_PATH = os.path.join(CSV_DIR, "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll")

print("bits:", struct.calcsize("P")*8)
print("dll:", DLL_PATH, os.path.exists(DLL_PATH))

dll = ctypes.CDLL(DLL_PATH)
print("dll loaded")

def parse_reads(csv_path):
    reads = []
    if not os.path.exists(csv_path): return reads
    with open(csv_path, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_READ" not in line or "UP" not in line: continue
            parts = line.split(";")
            if len(parts) <= 5: continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ",""))
                if raw: reads.append(raw)
            except: pass
    return reads

def extract_wire(raw):
    pkts = []
    i = 0
    while i < len(raw)-4:
        if raw[i]==0x55 and raw[i+1]==0xAA and raw[i+2]==0x5A:
            plen = raw[i+3]
            if i+5+plen <= len(raw):
                pkts.append(raw[i:i+5+plen])
                i += 5+plen
            else: i+=1
        else: i+=1
    return pkts

# Load from captures
for fname in ["redish.csv", "__pycache__/surely_full_red.csv", "red_full.csv"]:
    path = os.path.join(CSV_DIR, fname)
    reads = parse_reads(path)
    if not reads: continue
    combined = b"".join(reads)
    pkts = extract_wire(combined)
    print(f"\n{fname}: {len(reads)} read chunks, {len(pkts)} wire packets")
    for j, pkt in enumerate(pkts[:8]):
        plen = pkt[3]
        out = ctypes.create_string_buffer(512)
        inp = ctypes.create_string_buffer(pkt)
        try:
            ret = dll.unpack_package(inp, len(pkt), out)
            end = max((k for k in range(len(out.raw)) if out.raw[k]), default=-1)+1
            print(f"  [{j:2d}] len={plen:3d} unpack ret={ret} out_end={end} out={out.raw[:min(16,end)].hex() if end>0 else 'empty'}")
        except Exception as e:
            print(f"  [{j:2d}] len={plen:3d} ERROR: {e}")
    break

# After feeding all, try screendata
print("\n--- get_large_screendata_package after unpack ---")
NUM = 75
pix = (ctypes.c_ubyte*(NUM*3))()
for i in range(NUM):
    pix[i*3]=255; pix[i*3+1]=0; pix[i*3+2]=0
out = ctypes.create_string_buffer(2048)
ret = dll.get_large_screendata_package(0, NUM, pix, NUM*3, out)
raw = out.raw
end = max((k for k in range(min(1600,len(raw))) if raw[k]), default=-1)+1
print(f"ret={ret} end={end}")
if end>0:
    print(f"first40: {raw[:min(40,end)].hex()}")

print("DONE")
