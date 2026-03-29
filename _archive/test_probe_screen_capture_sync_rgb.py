# test_probe_screen_capture_sync_rgb.py — Узкий probe hidden ScreenCapture SyncRGB path
#
# Этот скрипт не трогает COM-порт.
# Он проверяет, какие предусловия нужны ScreenCapture.GetLProtocolSyncRGB(...),
# потому что прямой вызов сейчас возвращает None.

import sys

sys.path.insert(0, ".")

import clr
clr.AddReference("System.Drawing")

from System import Array
from System.Drawing import Color
from System.Reflection import Assembly, BindingFlags

ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"


# Ищем тип по простому имени.
def find_type(asm, name):
    for t in asm.GetTypes():
        if t.Name == name:
            return t
    return None


# Печатаем только интересующие методы с полными сигнатурами.
def dump_screen_capture_methods(screen_capture_type):
    flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
    print(f"=== TYPE: {screen_capture_type.FullName} ===")
    for m in screen_capture_type.GetMethods(flags):
        if m.Name in {"GetInstance", "Initialize", "SetSwitch", "GetLProtocolSyncRGB", "SetIfon"}:
            params = ", ".join([f"{p.Name}:{p.ParameterType.FullName}" for p in m.GetParameters()])
            print(f"METHOD {m.Name}({params}) -> {m.ReturnType.FullName}")


# Делаем небольшой тестовый массив цветов.
def make_colors(count):
    arr = Array.CreateInstance(Color, count)
    for i in range(count):
        if i % 4 == 0:
            arr[i] = Color.FromArgb(255, 0, 0)
        elif i % 4 == 1:
            arr[i] = Color.FromArgb(0, 255, 0)
        elif i % 4 == 2:
            arr[i] = Color.FromArgb(0, 0, 255)
        else:
            arr[i] = Color.FromArgb(255, 255, 0)
    return arr


# Этот helper вызывает метод и печатает результат без падения всего скрипта.
def try_call(label, method, target, args):
    try:
        result = method.Invoke(target, args)
        if result is None:
            print(f"[{label}] result = None")
            return None
        data = bytes(result)
        print(f"[{label}] len = {len(data)}")
        print(f"[{label}] head = {data[:24].hex(' ')}")
        return data
    except Exception as e:
        print(f"[{label}] error = {e}")
        return None


# Проверяем несколько возможных предусловий для high-level SyncRGB path.
def main():
    asm = Assembly.LoadFrom(ASM_PATH)
    screen_capture_type = find_type(asm, "ScreenCapture")
    if screen_capture_type is None:
        raise RuntimeError("ScreenCapture type not found")

    dump_screen_capture_methods(screen_capture_type)

    flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
    methods = list(screen_capture_type.GetMethods(flags))

    get_instance = next(m for m in methods if m.Name == "GetInstance" and len(m.GetParameters()) == 0)
    initialize = next(m for m in methods if m.Name == "Initialize" and len(m.GetParameters()) == 0)
    get_sync_2 = next(m for m in methods if m.Name == "GetLProtocolSyncRGB" and len(m.GetParameters()) == 2)
    get_sync_3 = next(m for m in methods if m.Name == "GetLProtocolSyncRGB" and len(m.GetParameters()) == 3)

    set_switch = None
    set_ifon = None
    for m in methods:
        if m.Name == "SetSwitch" and len(m.GetParameters()) == 1:
            set_switch = m
        elif m.Name == "SetIfon" and len(m.GetParameters()) == 1:
            set_ifon = m

    colors = make_colors(75)
    screen_capture = get_instance.Invoke(None, None)
    print(f"[Probe] instance = {screen_capture}")

    try_call("BeforeInit-2", get_sync_2, screen_capture, [colors, 1.0])
    try_call("BeforeInit-3", get_sync_3, screen_capture, [colors, 255.0, 1.0])

    try:
        init_result = initialize.Invoke(screen_capture, None)
        print(f"[Probe] Initialize() -> {init_result}")
    except Exception as e:
        print(f"[Probe] Initialize() error = {e}")

    try_call("AfterInit-2", get_sync_2, screen_capture, [colors, 1.0])
    try_call("AfterInit-3", get_sync_3, screen_capture, [colors, 255.0, 1.0])

    if set_switch is not None:
        for value in [0, 1, 2, 3]:
            try:
                switch_result = set_switch.Invoke(screen_capture, [value])
                print(f"[Probe] SetSwitch({value}) -> {switch_result}")
            except Exception as e:
                print(f"[Probe] SetSwitch({value}) error = {e}")
            try_call(f"AfterSetSwitch{value}-2", get_sync_2, screen_capture, [colors, 1.0])
            try_call(f"AfterSetSwitch{value}-3", get_sync_3, screen_capture, [colors, 255.0, 1.0])

    if set_ifon is not None:
        for value in [0, 1, 2, 3]:
            try:
                set_ifon_result = set_ifon.Invoke(screen_capture, [value])
                print(f"[Probe] SetIfon({value}) -> {set_ifon_result}")
            except Exception as e:
                print(f"[Probe] SetIfon({value}) error = {e}")
            try_call(f"AfterSetIfon{value}-2", get_sync_2, screen_capture, [colors, 1.0])
            try_call(f"AfterSetIfon{value}-3", get_sync_3, screen_capture, [colors, 255.0, 1.0])


if __name__ == "__main__":
    main()
