import sys, os, ctypes, struct

CSV_DIR = r"c:\Users\n0souls\Documents\GitHub\Soulight"
DLL_PATH = CSV_DIR + "\\" + "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430" + "\\Dumps\\beelightLib.dll"

print("DLL:", DLL_PATH)
print("exists:", os.path.exists(DLL_PATH))
dll = ctypes.CDLL(DLL_PATH)
print("dll ok")

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

# Читаем ответы контроллера из redish.csv
csv_path = CSV_DIR + "\\redish.csv"
reads = parse_reads(csv_path)
print("reads:", len(reads))
combined = b"".join(reads)
pkts = extract_wire(combined)
print("wire pkts:", len(pkts))

# Скармливаем каждый пакет в unpack_package
for j, pkt in enumerate(pkts[:12]):
    plen = pkt[3]
    out = ctypes.create_string_buffer(512)
    inp = ctypes.create_string_buffer(pkt)
    try:
        ret = dll.unpack_package(inp, len(pkt), out)
        raw_out = out.raw
        end = -1
        for k in range(len(raw_out)-1, -1, -1):
            if raw_out[k] != 0:
                end = k + 1
                break
        out_hex = raw_out[:min(16, end)].hex() if end > 0 else "empty"
        print("[" + str(j) + "] plen=" + str(plen) + " ret=" + str(ret) + " out=" + out_hex)
    except Exception as e:
        print("[" + str(j) + "] CRASH: " + str(e))

# Пробуем get_large_screendata_package
print("--- screendata ---")
NUM = 75
pix = (ctypes.c_ubyte * (NUM * 3))()
for i in range(NUM):
    pix[i*3] = 255
    pix[i*3+1] = 0
    pix[i*3+2] = 0
out = ctypes.create_string_buffer(2048)
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
    print("still empty")
print("DONE")
