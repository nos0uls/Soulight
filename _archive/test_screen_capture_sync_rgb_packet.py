# test_screen_capture_sync_rgb_packet.py — Сравнение hidden ScreenCapture path и low-level SyncRGB
#
# Этот скрипт ничего не отправляет в COM-порт.
# Он только сравнивает два packet generator path внутри Beelight.exe:
# - ScreenCapture.GetLProtocolSyncRGB(...)
# - LProtocolSyncRGB.GenProtocolSyncRGB(...)
#
# Если high-level path даёт другой wire-format, это может объяснить,
# почему low-level helper packet пока не зажигает ленту.

import sys

sys.path.insert(0, ".")

import clr
clr.AddReference("System.Drawing")

from System import Array, Byte as NetByte, Activator
from System.Drawing import Color
from System.Reflection import Assembly, BindingFlags

ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"


# Делаем простой и яркий паттерн, чтобы оба packet path получили одинаковые данные.
def make_colors(count):
    arr = Array.CreateInstance(Color, count)
    for i in range(count):
        t = i / max(1, count)
        if t < 0.25:
            arr[i] = Color.FromArgb(255, 0, 0)
        elif t < 0.50:
            arr[i] = Color.FromArgb(0, 255, 0)
        elif t < 0.75:
            arr[i] = Color.FromArgb(0, 0, 255)
        else:
            arr[i] = Color.FromArgb(255, 255, 0)
    return arr


# Ищем тип по простому имени.
def find_type(asm, name):
    for t in asm.GetTypes():
        if t.Name == name:
            return t
    return None


# Ищем метод по имени и числу параметров.
def find_method(t, name, n_params):
    flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
    for m in t.GetMethods(flags):
        if m.Name == name and len(m.GetParameters()) == n_params:
            return m
    return None


# Красиво печатаем краткую информацию о пакете.
def dump_packet(label, data):
    print(f"[{label}] len = {len(data)}")
    print(f"[{label}] head = {data[:32].hex(' ')}")
    print(f"[{label}] tail = {data[-32:].hex(' ')}")


# Сравниваем первые отличающиеся байты между двумя пакетами.
def dump_diff(a, b):
    limit = min(len(a), len(b))
    first_diffs = []
    for i in range(limit):
        if a[i] != b[i]:
            first_diffs.append((i, a[i], b[i]))
            if len(first_diffs) >= 16:
                break

    if not first_diffs and len(a) == len(b):
        print("[Diff] packets are identical")
        return

    print(f"[Diff] len(a)={len(a)}, len(b)={len(b)}")
    for idx, av, bv in first_diffs:
        print(f"[Diff] @ {idx}: {av:02x} != {bv:02x}")


# Этот main вызывает оба варианта packet generation и сравнивает результат.
def main():
    asm = Assembly.LoadFrom(ASM_PATH)

    screen_capture_type = find_type(asm, "ScreenCapture")
    sync_rgb_type = find_type(asm, "LProtocolSyncRGB")

    if screen_capture_type is None:
        raise RuntimeError("ScreenCapture type not found")
    if sync_rgb_type is None:
        raise RuntimeError("LProtocolSyncRGB type not found")

    get_instance = find_method(screen_capture_type, "GetInstance", 0)
    get_sync_rgb_2 = find_method(screen_capture_type, "GetLProtocolSyncRGB", 2)
    get_sync_rgb_3 = find_method(screen_capture_type, "GetLProtocolSyncRGB", 3)
    gen_sync_rgb = find_method(sync_rgb_type, "GenProtocolSyncRGB", 3)

    if get_instance is None:
        raise RuntimeError("ScreenCapture.GetInstance() not found")
    if get_sync_rgb_2 is None or get_sync_rgb_3 is None:
        raise RuntimeError("ScreenCapture.GetLProtocolSyncRGB overloads not found")
    if gen_sync_rgb is None:
        raise RuntimeError("LProtocolSyncRGB.GenProtocolSyncRGB() not found")

    colors = make_colors(75)

    screen_capture = get_instance.Invoke(None, None)
    sync_rgb = Activator.CreateInstance(sync_rgb_type)

    low_level_packet = bytes(gen_sync_rgb.Invoke(sync_rgb, [NetByte(1), NetByte(75), colors]))
    high_level_packet_2 = bytes(get_sync_rgb_2.Invoke(screen_capture, [colors, 1.0]))
    high_level_packet_3 = bytes(get_sync_rgb_3.Invoke(screen_capture, [colors, 255.0, 1.0]))

    dump_packet("LowLevel", low_level_packet)
    dump_packet("HighLevel2", high_level_packet_2)
    dump_packet("HighLevel3", high_level_packet_3)

    print()
    print("=== LowLevel vs HighLevel2 ===")
    dump_diff(low_level_packet, high_level_packet_2)

    print()
    print("=== LowLevel vs HighLevel3 ===")
    dump_diff(low_level_packet, high_level_packet_3)

    print()
    print("=== HighLevel2 vs HighLevel3 ===")
    dump_diff(high_level_packet_2, high_level_packet_3)


if __name__ == "__main__":
    main()
