# -*- coding: utf-8 -*-
"""
diag_dll_format.py — Диагностика формата вывода beelightLib.dll
Показывает СЫРОЙ вывод DLL без какой-либо обработки.
Запускать через 32-bit Python!
"""
import sys, os, ctypes, struct

# region ===== Проверки =====
sys.stdout.reconfigure(encoding="utf-8")
ptr = struct.calcsize("P") * 8
if ptr != 32:
    print(f"ОШИБКА: нужен 32-bit Python (сейчас {ptr}-bit)")
    sys.exit(1)

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)
dll = ctypes.CDLL(DLL_PATH)
print(f"DLL loaded at 0x{dll._handle:08x}")
# endregion

# region ===== Помощник для hex-дампа =====
def hexdump(data, label=""):
    """Печатает hex-дамп данных с ASCII-представлением"""
    if label:
        print(f"\n{'='*60}")
        print(f"  {label}")
        print(f"  Длина: {len(data)} байт")
        print(f"{'='*60}")
    for off in range(0, len(data), 16):
        chunk = data[off:off+16]
        hx = ' '.join(f'{b:02x}' for b in chunk)
        asc = ''.join(chr(b) if 32 <= b < 127 else '.' for b in chunk)
        print(f"  {off:04x}: {hx:<48s}  {asc}")
# endregion

# region ===== Извлечение пакета из буфера DLL =====
def extract_raw(buf, buf_size=256):
    """Извлекает данные из буфера DLL — ищет последний ненулевой байт"""
    raw = buf.raw
    last_nz = 0
    for i in range(min(buf_size, len(raw)) - 1, -1, -1):
        if raw[i] != 0:
            last_nz = i
            break
    return raw[:last_nz + 1]
# endregion

# region ===== Тест 1: get_connect_package =====
print("\n" + "#"*60)
print("# ТЕСТ 1: get_connect_package")
print("#"*60)

buf1 = ctypes.create_string_buffer(256)
ret1 = dll.get_connect_package(buf1)
pkt1 = extract_raw(buf1)
print(f"Return value: {ret1}")
hexdump(pkt1, "RAW DLL output (connect)")

# Проверяем: начинается с MgL?
if pkt1[:3] == b'MgL':
    print("\n  >>> Начинается с MgL header!")
    print(f"  >>> Байт 3 (после MgL): 0x{pkt1[3]:02x}")
    print(f"  >>> Payload после MgL\\x00: {pkt1[4:].hex(' ')}")
    # Проверяем: есть ли 55 AA 5A внутри payload?
    if b'\x55\xAA\x5A' in pkt1[4:]:
        idx = pkt1.index(b'\x55\xAA\x5A', 4)
        print(f"  >>> 55 AA 5A найден на позиции {idx}!")
    else:
        print(f"  >>> 55 AA 5A НЕ найден в payload")
elif pkt1[:3] == b'\x55\xAA\x5A':
    print("\n  >>> Начинается с 55 AA 5A (wire format)!")
else:
    print(f"\n  >>> Неизвестный формат. Первые 4 байта: {pkt1[:4].hex(' ')}")
# endregion

# region ===== Тест 2: get_scen_package (RED) =====
print("\n" + "#"*60)
print("# ТЕСТ 2: get_scen_package(0, 100, 255, 0, 0, 1)")
print("#"*60)

buf2 = ctypes.create_string_buffer(256)
ret2 = dll.get_scen_package(0, 100, 255, 0, 0, 1, buf2)
pkt2 = extract_raw(buf2)
print(f"Return value: {ret2}")
hexdump(pkt2, "RAW DLL output (scen RED)")

if pkt2[:3] == b'MgL':
    print(f"\n  >>> MgL header. Payload после MgL\\x00: {len(pkt2)-4} байт")
    payload = pkt2[4:]
    # Ищем 55 AA 5A внутри
    if payload[:3] == b'\x55\xAA\x5A':
        print(f"  >>> Payload НАЧИНАЕТСЯ с 55 AA 5A!")
        print(f"  >>> plen byte: 0x{payload[3]:02x} = {payload[3]}")
    else:
        print(f"  >>> Payload НЕ начинается с 55 AA 5A")
        print(f"  >>> Первые байты payload: {payload[:8].hex(' ')}")
# endregion

# region ===== Тест 3: Несколько вызовов — смотрим меняются ли пакеты =====
print("\n" + "#"*60)
print("# ТЕСТ 3: 5 вызовов get_scen_package (GREEN) — randomness")
print("#"*60)

for i in range(5):
    buf = ctypes.create_string_buffer(256)
    dll.get_scen_package(0, 100, 0, 255, 0, 1, buf)
    pkt = extract_raw(buf)
    # Показываем только первые 20 байт
    print(f"  [{i}] len={len(pkt):3d}  {pkt[:20].hex(' ')}")
# endregion

# region ===== Тест 4: get_connect второй раз — меняется ли? =====
print("\n" + "#"*60)
print("# ТЕСТ 4: get_connect_package (второй вызов)")
print("#"*60)

buf4 = ctypes.create_string_buffer(256)
dll.get_connect_package(buf4)
pkt4 = extract_raw(buf4)
hexdump(pkt4, "RAW DLL output (connect #2)")
# endregion

# region ===== Тест 5: unpack_package — что делает? =====
print("\n" + "#"*60)
print("# ТЕСТ 5: unpack_package — проверка API")
print("#"*60)

# unpack_package скорее всего принимает ответ контроллера
# Попробуем передать ему пустой буфер
try:
    in_buf = ctypes.create_string_buffer(256)
    out_buf = ctypes.create_string_buffer(256)
    # Попробуем разные сигнатуры
    # Вариант 1: unpack_package(input, output)
    ret5 = dll.unpack_package(in_buf, out_buf)
    print(f"  unpack_package(empty, out) = {ret5}")
    out5 = extract_raw(out_buf)
    if out5:
        hexdump(out5, "unpack output")
except Exception as e:
    print(f"  Ошибка: {e}")
# endregion

print("\n\nДиагностика завершена.")
