# test_probe_screen_capture_sync_rgb_compact.py — Компактный probe hidden ScreenCapture SyncRGB path
#
# Этот скрипт проверяет только самые правдоподобные варианты:
# - Initialize()
# - нормализованные Double параметры 0..1
# - явный Int32 для SetSwitch / SetIfon
#
# Цель: быстро понять, может ли high-level ScreenCapture path выдать
# рабочий packet там, где low-level LProtocolSyncRGB пока не помог.

import sys

sys.path.insert(0, ".")

import clr
clr.AddReference("System.Drawing")

from System import Array, Double, Int32
from System.Drawing import Color
from System.Reflection import Assembly, BindingFlags

ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"


# Ищем тип по простому имени.
def find_type(asm, name):
    for t in asm.GetTypes():
        if t.Name == name:
            return t
    return None


# Ищем метод по имени и числу параметров.
def find_method(t, name, count):
    flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
    for m in t.GetMethods(flags):
        if m.Name == name and len(m.GetParameters()) == count:
            return m
    return None


# Готовим заметный паттерн из 75 цветов.
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


# Печатаем только краткий результат вызова.
def try_call(label, method, target, args):
    try:
        result = method.Invoke(target, args)
        if result is None:
            print(f"[{label}] result=None")
            return None
        data = bytes(result)
        print(f"[{label}] len={len(data)} head={data[:20].hex(' ')}")
        return data
    except Exception as e:
        print(f"[{label}] error={e}")
        return None


# Этот main проверяет только компактный набор самых полезных вариантов.
def main():
    asm = Assembly.LoadFrom(ASM_PATH)
    screen_capture_type = find_type(asm, "ScreenCapture")
    if screen_capture_type is None:
        raise RuntimeError("ScreenCapture type not found")

    get_instance = find_method(screen_capture_type, "GetInstance", 0)
    initialize = find_method(screen_capture_type, "Initialize", 0)
    get_sync_2 = find_method(screen_capture_type, "GetLProtocolSyncRGB", 2)
    get_sync_3 = find_method(screen_capture_type, "GetLProtocolSyncRGB", 3)
    set_switch = find_method(screen_capture_type, "SetSwitch", 1)
    set_ifon = find_method(screen_capture_type, "SetIfon", 1)

    if get_instance is None or initialize is None or get_sync_2 is None or get_sync_3 is None:
        raise RuntimeError("Required ScreenCapture methods were not found")

    screen_capture = get_instance.Invoke(None, None)
    colors = make_colors(75)

    print("[CompactProbe] Before Initialize")
    try_call("BeforeInit-2-v1.0", get_sync_2, screen_capture, [colors, Double(1.0)])
    try_call("BeforeInit-3-d1.0-v1.0", get_sync_3, screen_capture, [colors, Double(1.0), Double(1.0)])
    try_call("BeforeInit-3-d0.5-v1.0", get_sync_3, screen_capture, [colors, Double(0.5), Double(1.0)])

    init_result = initialize.Invoke(screen_capture, None)
    print(f"[CompactProbe] Initialize() -> {init_result}")

    print("[CompactProbe] After Initialize")
    try_call("AfterInit-2-v1.0", get_sync_2, screen_capture, [colors, Double(1.0)])
    try_call("AfterInit-2-v0.5", get_sync_2, screen_capture, [colors, Double(0.5)])
    try_call("AfterInit-3-d1.0-v1.0", get_sync_3, screen_capture, [colors, Double(1.0), Double(1.0)])
    try_call("AfterInit-3-d0.5-v1.0", get_sync_3, screen_capture, [colors, Double(0.5), Double(1.0)])
    try_call("AfterInit-3-d1.0-v0.5", get_sync_3, screen_capture, [colors, Double(1.0), Double(0.5)])

    if set_switch is not None:
        for value in [Int32(0), Int32(1), Int32(2), Int32(3)]:
            switch_result = set_switch.Invoke(screen_capture, [value])
            print(f"[CompactProbe] SetSwitch({int(value)}) -> {switch_result}")
            try_call(f"AfterSetSwitch{int(value)}-2-v1.0", get_sync_2, screen_capture, [colors, Double(1.0)])
            try_call(f"AfterSetSwitch{int(value)}-3-d1.0-v1.0", get_sync_3, screen_capture, [colors, Double(1.0), Double(1.0)])

    if set_ifon is not None:
        for value in [Int32(0), Int32(1), Int32(2), Int32(3)]:
            set_ifon.Invoke(screen_capture, [value])
            print(f"[CompactProbe] SetIfon({int(value)}) called")
            try_call(f"AfterSetIfon{int(value)}-2-v1.0", get_sync_2, screen_capture, [colors, Double(1.0)])
            try_call(f"AfterSetIfon{int(value)}-3-d1.0-v1.0", get_sync_3, screen_capture, [colors, Double(1.0), Double(1.0)])


if __name__ == "__main__":
    main()
