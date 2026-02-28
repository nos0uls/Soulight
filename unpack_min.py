import sys, os, ctypes, struct

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll")

dll = ctypes.CDLL(DLL_PATH)
print("dll ok")

# Parse READ ops from CSV (controller -> PC)
def parse_reads(csv_path):
    reads = []
    if not os.path.exists(csv_path):
        return reads
    with open(csv_path, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_READ" not in line or "UP" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
                if raw:
                    reads.append(raw)
            except:
                pass
    return reads

# Parse WRITE ops from CSV (PC -> controller)
def parse_writes(csv_path):
    writes = []
    if not os.path.exists(csv_path):
        return writes
    with open(csv_path, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
                if raw:
                    writes.append(raw)
            except:
                pass
    return writes

def extract_wire(raw):
    pkts = []
    i = 0
    while i < len(raw) - 4:
        if raw[i] == 0x55 and raw[i+1] == 0xAA and raw[i+2] == 0x5A:
            plen = raw[i+3]
            if i + 5 + plen <= len(raw):
                pkts.append(raw[i:i+5+plen])
                i += 5 + plen
            else:
                i += 1
        else:
            i += 1
    return pkts

# --- Feed READ packets into unpack_package ---
for fname in ["redish.csv", "__pycache__/surely_full_red.csv", "red_full.csv"]:
    path = os.path.join(CSV_DIR, fname)
    reads = parse_reads(path)
    if not reads:
        continue
    combined = b"".join(reads)
    pkts = extract_wire(combined)
    print(fname + ": " + str(len(reads)) + " reads, " + str(len(pkts)) + " wire pkts")
    for j, pkt in enumerate(pkts[:12]):
        plen = pkt[3]
        out = ctypes.create_string_buffer(512)
        inp = ctypes.create_string_buffer(pkt)
        try:
            ret = dll.unpack_package(inp, len(pkt), out)
            out_raw = out.raw
            end = -1
            for k in range(len(out_raw)-1, -1, -1):
                if out_raw[k] != 0:
                    end = k + 1
                    break
            out_hex = out_raw[:min(16, end)].hex() if end > 0 else "empty"
            print("  [" + str(j) + "] plen=" + str(plen) + " ret=" + str(ret) + " out_end=" + str(end) + " out=" + out_hex)
        except Exception as e:
            print("  [" + str(j) + "] ERROR: " + str(e))
    break

# --- Now try get_large_screendata_package ---
print("--- get_large_screendata_package ---")
NUM = 75
pix = (ctypes.c_ubyte * (NUM * 3))()
for i in range(NUM):
    pix[i*3] = 255
    pix[i*3+1] = 0
    pix[i*3+2] = 0
out = ctypes.create_string_buffer(2048)
try:
    ret = dll.get_large_screendata_package(0, NUM, pix, NUM*3, out)
    raw = out.raw
    end = -1
    for k in range(min(1600, len(raw))-1, -1, -1):
        if raw[k] != 0:
            end = k + 1
            break
    print("ret=" + str(ret) + " end=" + str(end))
    if end > 0:
        print("first40: " + raw[:min(40, end)].hex())
    else:
        print("empty output - DLL state not initialized by unpack")
except Exception as e:
    print("ERROR: " + str(e))

# --- Also check all CSV WRITE packets through unpack (sanity check) ---
print("--- unpack on WRITE packets (PC->controller) ---")
for fname in ["redish.csv", "__pycache__/surely_full_red.csv"]:
    path = os.path.join(CSV_DIR, fname)
    writes = parse_writes(path)
    if not writes:
        continue
    combined = b"".join(writes)
    pkts = extract_wire(combined)
    print(fname + ": " + str(len(pkts)) + " write wire pkts")
    for j, pkt in enumerate(pkts[:8]):
        plen = pkt[3]
        out = ctypes.create_string_buffer(512)
        inp = ctypes.create_string_buffer(pkt)
        try:
            ret = dll.unpack_package(inp, len(pkt), out)
            out_raw = out.raw
            end = -1
            for k in range(len(out_raw)-1, -1, -1):
                if out_raw[k] != 0:
                    end = k + 1
                    break
            out_hex = out_raw[:min(16, end)].hex() if end > 0 else "empty"
            print("  [" + str(j) + "] plen=" + str(plen) + " ret=" + str(ret) + " out=" + out_hex)
        except Exception as e:
            print("  [" + str(j) + "] ERROR: " + str(e))
    break

print("DONE")
