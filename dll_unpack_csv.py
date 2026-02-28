# -*- coding: utf-8 -*-
"""
dll_unpack_csv.py - Feed captured controller responses from CSV into
unpack_package, then test get_large_screendata_package.

No COM port needed - uses data from redish.csv.

Run with 32-bit Python!
"""
import sys, os, ctypes, struct, time, io

# Установка кодировки для нормального отображения UTF-8
os.environ.setdefault('PYTHONIOENCODING', 'utf-8')
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8', errors='replace')

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "Новая папка", "Dumps", "beelightLib.dll")

print("Starting dll_unpack_csv.py...")
sys.stdout.flush()

if struct.calcsize("P") * 8 != 32:
    print("Need 32-bit Python!"); sys.exit(1)

dll = ctypes.CDLL(DLL_PATH)
print("DLL loaded")


# ================================================================
# Парсинг CSV: извлекаем все READ операции (ответы контроллера)
# ================================================================
def parse_csv_reads(csv_path):
    """Читает CSV и возвращает список байтовых строк из операций чтения (READ - ответы контроллера)."""
    reads = []
    if not os.path.exists(csv_path):
        print(f"  NOT FOUND: {csv_path}")
        return reads
    with open(csv_path, "r", errors="replace") as f:
        for line in f:
            # READ = данные от контроллера к ПК
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
                continue
    return reads


def parse_csv_writes(csv_path):
    """Читает CSV и возвращает список байтовых строк из операций записи (WRITE - данные от ПК)."""
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
                continue
    return writes


def extract_wire_packets(raw):
    """Разбирает raw байты на wire пакеты формата 55AA5A."""
    pkts = []
    i = 0
    while i < len(raw) - 4:
        if raw[i] == 0x55 and raw[i+1] == 0xAA and raw[i+2] == 0x5A:
            plen = raw[i+3]
            if i + 5 + plen <= len(raw):
                payload = raw[i+5:i+5+plen]
                pkts.append({
                    'len': plen,
                    'attr': raw[i+4],
                    'payload': payload,
                    'raw': raw[i:i+5+plen]
                })
                i += 5 + plen
            else:
                i += 1
        else:
            i += 1
    return pkts


# ================================================================
# 1. Загружаем ответы контроллера из redish.csv
# ================================================================
print(f"\n{'='*60}")
print("1. Load controller responses from CSVs")
print(f"{'='*60}")

csv_files = ["redish.csv", "__pycache__/surely_full_red.csv", "red_full.csv"]
all_reads = {}
all_writes = {}

for fname in csv_files:
    path = os.path.join(CSV_DIR, fname)
    r = parse_csv_reads(path)
    w = parse_csv_writes(path)
    all_reads[fname] = r
    all_writes[fname] = w
    print(f"  {fname}: {len(r)} reads, {len(w)} writes")

# Объединяем все READ данные в один поток для поиска пакетов
for fname, reads in all_reads.items():
    if not reads:
        continue
    combined = b"".join(reads)
    pkts = extract_wire_packets(combined)
    print(f"\n  {fname} wire packets: {len(pkts)}")
    for j, p in enumerate(pkts[:10]):
        print(f"    pkt[{j:2d}]: len={p['len']:3d} attr={p['attr']:02x} "
              f"payload={p['payload'][:20].hex()}")

# ================================================================
# 2. Скармливаем ответы контроллера в unpack_package
# ================================================================
print(f"\n{'='*60}")
print("2. Feed controller responses into unpack_package")
print(f"{'='*60}")


def try_unpack(data, desc):
    """Пробует разные сигнатуры unpack_package и возвращает результат."""
    results = []

    # Сигнатура 1: (data_ptr, data_len, out_ptr)
    out = ctypes.create_string_buffer(512)
    inp = ctypes.create_string_buffer(data)
    try:
        ret = dll.unpack_package(inp, len(data), out)
        out_raw = out.raw
        out_end = max((k for k in range(len(out_raw)) if out_raw[k]), default=-1) + 1
        results.append(f"  3-arg: ret={ret} out_len={out_end} "
                       f"out={out_raw[:min(20, out_end)].hex() if out_end > 0 else 'empty'}")
    except Exception as e:
        results.append(f"  3-arg: ERROR: {e}")

    # Сигнатура 2: (data_ptr, out_ptr)
    out2 = ctypes.create_string_buffer(512)
    inp2 = ctypes.create_string_buffer(data)
    try:
        ret2 = dll.unpack_package(inp2, out2)
        out_raw2 = out2.raw
        out_end2 = max((k for k in range(len(out_raw2)) if out_raw2[k]), default=-1) + 1
        results.append(f"  2-arg: ret={ret2} out_len={out_end2} "
                       f"out={out_raw2[:min(20, out_end2)].hex() if out_end2 > 0 else 'empty'}")
    except Exception as e:
        results.append(f"  2-arg: ERROR: {e}")

    return results


# Берём пакеты из первого доступного CSV
for fname in csv_files:
    reads = all_reads.get(fname, [])
    if not reads:
        continue
    combined = b"".join(reads)
    pkts = extract_wire_packets(combined)
    if not pkts:
        continue

    print(f"\n  From {fname}:")
    for j, p in enumerate(pkts[:5]):
        print(f"\n  pkt[{j}] len={p['len']} payload={p['payload'][:10].hex()}...")
        # Пробуем: raw wire, payload only, MgL-wrapped
        for data, label in [
            (p['raw'], "wire"),
            (p['payload'], "payload"),
            (b'MgL\x00' + bytes([p['len']]) + p['payload'], "MgL-wrapped"),
        ]:
            res = try_unpack(data, label)
            for r in res:
                print(f"    [{label}] {r}")
    break

# ================================================================
# 3. Проверяем что возвращает unpack_package на данных из захвата
# ================================================================
print(f"\n{'='*60}")
print("3. Check unpack return values in detail")
print(f"{'='*60}")

# Согласно документации DLL, unpack должна вернуть тип пакета
# Проверяем все возможные коды возврата
type_names = {
    0: "UNKNOWN/ERROR",
    1: "SCEN",
    2: "SCREEN_DATA",
    3: "CONNECT/RESPONSE",
    4: "CONNECT_ACK",
    5: "SETTING",
    6: "ECO",
}

for fname in csv_files:
    reads = all_reads.get(fname, [])
    if not reads:
        continue
    combined = b"".join(reads)
    pkts = extract_wire_packets(combined)
    if not pkts:
        continue

    print(f"\n  All packets from {fname}:")
    for j, p in enumerate(pkts):
        out = ctypes.create_string_buffer(512)
        inp = ctypes.create_string_buffer(p['raw'])
        try:
            ret = dll.unpack_package(inp, len(p['raw']), out)
            out_raw = out.raw
            out_end = max((k for k in range(len(out_raw)) if out_raw[k]), default=-1) + 1
            tname = type_names.get(ret, f"TYPE_{ret}")
            print(f"    pkt[{j:2d}] plen={p['len']:3d} -> ret={ret} ({tname}) "
                  f"out={out_raw[:min(16, out_end)].hex() if out_end > 0 else 'empty'}")
        except Exception as e:
            print(f"    pkt[{j:2d}] plen={p['len']:3d} -> ERROR: {e}")
    break

# ================================================================
# 4. После unpack пробуем get_large_screendata_package
# ================================================================
print(f"\n{'='*60}")
print("4. get_large_screendata_package after feeding all packets")
print(f"{'='*60}")

# Скормим все пакеты из захвата подряд
for fname in csv_files:
    reads = all_reads.get(fname, [])
    if not reads:
        continue
    combined = b"".join(reads)
    pkts = extract_wire_packets(combined)
    if not pkts:
        continue

    print(f"  Feeding {len(pkts)} packets from {fname} into unpack_package...")
    for p in pkts:
        out = ctypes.create_string_buffer(512)
        inp = ctypes.create_string_buffer(p['raw'])
        try:
            dll.unpack_package(inp, len(p['raw']), out)
        except:
            pass

    # Теперь пробуем get_large_screendata_package
    NUM_LEDS = 75
    pix = (ctypes.c_ubyte * (NUM_LEDS * 3))()
    for i in range(NUM_LEDS):
        pix[i*3] = 255; pix[i*3+1] = 0; pix[i*3+2] = 0

    out_sd = ctypes.create_string_buffer(2048)
    try:
        ret = dll.get_large_screendata_package(0, NUM_LEDS, pix, NUM_LEDS*3, out_sd)
        raw_sd = out_sd.raw
        end_sd = max((k for k in range(min(1600, len(raw_sd))) if raw_sd[k]), default=-1) + 1
        print(f"  ret={ret} output_end={end_sd}")
        if end_sd > 0:
            print(f"  first40: {raw_sd[:min(40,end_sd)].hex()}")
            if raw_sd[:3] == b'MgL':
                print(f"  -> MgL format! payload_len={raw_sd[4]}")
            elif raw_sd[:3] == b'\x55\xAA\x5A':
                print(f"  -> Wire format! payload_len={raw_sd[3]}")
        else:
            print("  -> Still empty output")
    except Exception as e:
        print(f"  ERROR: {e}")
    break

# ================================================================
# 5. Анализируем структуру ответа контроллера на connect
# ================================================================
print(f"\n{'='*60}")
print("5. Analyze controller connect response structure")
print(f"{'='*60}")

# В предыдущем запуске контроллер ответил 153 байтами на MgL connect
# resp: 55aa5a09005834496864e777496855aa5a0c002039426d85f40053734a426d55aa5a0a0035379941...
# Разберём эти пакеты
sample_resp = bytes.fromhex(
    "55aa5a09005834496864e777496855aa5a0c002039426d85f40053734a426d55aa5a0a0035379941"
    # Это только первые 40 байт, полный ответ был 153 байта — не имеем полностью
)
print(f"  Sample response (first 40b): {sample_resp.hex()}")
pkts_sample = extract_wire_packets(sample_resp)
print(f"  Parsed {len(pkts_sample)} packets:")
for j, p in enumerate(pkts_sample):
    print(f"    pkt[{j}]: len={p['len']} attr={p['attr']:02x} payload={p['payload'].hex()}")
    # Попробуем unpack
    out = ctypes.create_string_buffer(512)
    inp = ctypes.create_string_buffer(p['raw'])
    try:
        ret = dll.unpack_package(inp, len(p['raw']), out)
        out_end = max((k for k in range(len(out.raw)) if out.raw[k]), default=-1) + 1
        print(f"      unpack ret={ret} out_len={out_end} out={out.raw[:min(16,out_end)].hex()}")
    except Exception as e:
        print(f"      unpack ERROR: {e}")

print("\nDone!")
sys.stdout.flush()
