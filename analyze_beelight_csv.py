"""
analyze_encoding.py
-------------------
Берёт два лога (чёрный экран и белый экран) и сравнивает байты,
чтобы понять алгоритм трансформации цветов.

Как использовать:
1. Запусти Beelight в режиме Screen Mirroring
2. Сделай экран ПОЛНОСТЬЮ ЧЁРНЫМ (PowerPoint/Paint с чёрным фоном)
3. Запиши лог 3-5 секунд → сохрани как black.csv
4. Сделай экран ПОЛНОСТЬЮ БЕЛЫМ
5. Запиши лог 3-5 секунд → сохрани как white.csv
6. Запусти: python analyze_encoding.py black.csv white.csv
"""

import sys, csv, re

CANDIDATE_ENCODINGS = ["utf-8-sig", "utf-16", "cp1251", "cp1252", "latin-1"]

def read_text(path):
    raw = open(path, "rb").read()
    for enc in CANDIDATE_ENCODINGS:
        try:
            return raw.decode(enc)
        except UnicodeDecodeError:
            pass
    return raw.decode("latin-1", errors="replace")

def sniff_delimiter(sample):
    try:
        return csv.Sniffer().sniff(sample, delimiters=";,").delimiter
    except:
        return ";"

def get_write_packets(path):
    text = read_text(path)
    delim = sniff_delimiter("\n".join(text.splitlines()[:50]))
    reader = csv.DictReader(text.splitlines(), delimiter=delim)
    fields = {n.strip(): n for n in (reader.fieldnames or [])}
    col_func = fields.get("Function")
    col_data = fields.get("Data")
    packets = []
    for row in reader:
        func = (row.get(col_func) or "").strip()
        if func != "IRP_MJ_WRITE":
            continue
        raw = (row.get(col_data) or "").strip()
        raw = re.sub(r"[^0-9a-fA-F ]", "", raw)
        raw = re.sub(r"\s+", " ", raw).strip().upper()
        if len(raw) > 10:
            packets.append(raw)
    return packets

def to_bytes(hex_str):
    return bytes.fromhex(hex_str.replace(" ", ""))

def get_long_packets(packets, min_len=100):
    """Только длинные пакеты (данные цветов)"""
    result = []
    for p in packets:
        b = to_bytes(p)
        if len(b) >= min_len:
            result.append(b)
    return result

def analyze_two_logs(black_path, white_path):
    print(f"Читаю {black_path}...")
    black_pkts = get_long_packets(get_write_packets(black_path))
    print(f"  Длинных пакетов: {len(black_pkts)}")

    print(f"Читаю {white_path}...")
    white_pkts = get_long_packets(get_write_packets(white_path))
    print(f"  Длинных пакетов: {len(white_pkts)}")

    if not black_pkts or not white_pkts:
        print("Нет длинных пакетов!")
        return

    # Берём первый стабильный пакет из каждого
    # (берём из середины, чтобы не попасть на переходный)
    b_pkt = black_pkts[len(black_pkts)//2]
    w_pkt = white_pkts[len(white_pkts)//2]

    min_len = min(len(b_pkt), len(w_pkt))
    print(f"\nДлина чёрного пакета:  {len(b_pkt)} байт")
    print(f"Длина белого пакета:   {len(w_pkt)} байт")
    print(f"Сравниваем первые {min_len} байт\n")

    print("=== ПЕРВЫЕ 20 БАЙТ: Чёрный vs Белый ===")
    print(f"{'Позиция':>8}  {'Чёрный':>8}  {'Белый':>8}  {'Разница':>8}  {'XOR':>8}")
    for i in range(min(20, min_len)):
        bc = b_pkt[i]
        wc = w_pkt[i]
        diff = wc - bc
        xor  = bc ^ wc
        print(f"{i:>8}  {bc:>8}  {wc:>8}  {diff:>8}  {xor:>8X}")

    print("\n=== АНАЛИЗ ПОВТОРЯЮЩИХСЯ ПАТТЕРНОВ В ЧЁРНОМ ПАКЕТЕ ===")
    # Ищем паттерн (шаг повторения)
    for step in [1, 2, 3, 4]:
        values = [b_pkt[i] for i in range(0, min(60, len(b_pkt)), step)]
        unique = set(values)
        print(f"  Шаг {step}: {len(unique)} уникальных значений -> {sorted(unique)[:10]}")

    print("\n=== ГИПОТЕЗА: XOR-ключ ===")
    # Если данные XOR-ятся с ключом, то black XOR white = ключ XOR (0,0,0) XOR ключ XOR (255,255,255) = (255,255,255)
    # Т.е. все позиции должны давать одинаковый XOR
    xors = [b_pkt[i] ^ w_pkt[i] for i in range(min(60, min_len))]
    print(f"  XOR первых 60 байт: {[hex(x) for x in xors[:20]]}")
    unique_xors = set(xors[:60])
    if len(unique_xors) == 1:
        print(f"  ✅ XOR постоянный! Ключ = {hex(list(unique_xors)[0])}")
    elif len(unique_xors) <= 4:
        print(f"  🔶 XOR почти постоянный: {unique_xors}")
    else:
        print(f"  ❌ XOR непостоянный ({len(unique_xors)} разных значений) — простого XOR нет")

    print("\n=== СЫРЫЕ БАЙТЫ ПЕРВЫХ 5 ПАКЕТОВ (Чёрный) ===")
    for i, pkt in enumerate(black_pkts[:5]):
        print(f"  Пакет {i}: {pkt[:30].hex(' ')}")

    print("\n=== СЫРЫЕ БАЙТЫ ПЕРВЫХ 5 ПАКЕТОВ (Белый) ===")
    for i, pkt in enumerate(white_pkts[:5]):
        print(f"  Пакет {i}: {pkt[:30].hex(' ')}")

    # Записываем в файл для отправки
    with open("encoding_analysis.txt", "w", encoding="utf-8") as f:
        f.write(f"BLACK packet [{len(b_pkt)} bytes]:\n{b_pkt.hex(' ')}\n\n")
        f.write(f"WHITE packet [{len(w_pkt)} bytes]:\n{w_pkt.hex(' ')}\n\n")
        f.write("XOR positions 0-60:\n")
        f.write(" ".join(hex(x) for x in xors) + "\n")
    print("\n→ Полные пакеты сохранены в encoding_analysis.txt")


def analyze_single(path):
    """Если только один файл — просто показываем структуру"""
    print(f"Читаю {path}...")
    all_pkts = get_write_packets(path)
    long_pkts = get_long_packets(all_pkts)
    short_pkts = [to_bytes(p) for p in all_pkts if len(to_bytes(p)) < 20]

    print(f"\nКороткие пакеты (handshake):")
    for p in short_pkts[:15]:
        print(f"  {p.hex(' ')}")

    print(f"\nДлинные пакеты - первые 3:")
    for i, p in enumerate(long_pkts[:3]):
        print(f"  [{len(p)} bytes] {p[:40].hex(' ')}...")

    if long_pkts:
        with open("single_analysis.txt", "w", encoding="utf-8") as f:
            for i, p in enumerate(long_pkts[:10]):
                f.write(f"Packet {i} [{len(p)} bytes]:\n{p.hex(' ')}\n\n")
        print("\n→ Пакеты сохранены в single_analysis.txt")


if __name__ == "__main__":
    if len(sys.argv) == 3:
        analyze_two_logs(sys.argv[1], sys.argv[2])
    elif len(sys.argv) == 2:
        analyze_single(sys.argv[1])
    else:
        print("Usage:")
        print("  python analyze_encoding.py black.csv white.csv   # сравнить два лога")
        print("  python analyze_encoding.py log.csv               # анализ одного лога")