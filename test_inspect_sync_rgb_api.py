# test_inspect_sync_rgb_api.py — Узкий reflection probe вокруг hidden SyncRGB path
#
# Этот скрипт не трогает COM-порт и ничего не отправляет в устройство.
# Он только смотрит в Beelight.exe через reflection и печатает:
# - конструкторы
# - свойства
# - поля
# - методы
# для типов, которые похожи на реальный внутренний screen mirroring pipeline.

import sys

sys.path.insert(0, ".")

import clr
from System.Reflection import Assembly, BindingFlags

ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"


# Этот helper красиво печатает сигнатуры и структуру типа.
# Он нужен, чтобы быстро увидеть возможные точки входа без ручного перебора в C# decompiler.
def dump_type(t, flags):
    print(f"\n=== TYPE: {t.FullName} ===")

    try:
        for ctor in t.GetConstructors(flags):
            params = ", ".join([f"{p.Name}:{p.ParameterType.Name}" for p in ctor.GetParameters()])
            print(f"CTOR({params})")
    except Exception as e:
        print(f"[ctors error] {e}")

    try:
        for prop in t.GetProperties(flags):
            print(f"PROP {prop.Name}: {prop.PropertyType.Name}")
    except Exception as e:
        print(f"[props error] {e}")

    try:
        for field in t.GetFields(flags):
            print(f"FIELD {field.Name}: {field.FieldType.Name}")
    except Exception as e:
        print(f"[fields error] {e}")

    try:
        for method in t.GetMethods(flags):
            params = ", ".join([f"{p.Name}:{p.ParameterType.Name}" for p in method.GetParameters()])
            print(f"METHOD {method.Name}({params}) -> {method.ReturnType.Name}")
    except Exception as e:
        print(f"[methods error] {e}")


# Этот main ищет только те типы, которые уже всплыли в предыдущем probe.
# Так вывод остаётся узким и не тонет в шуме всей assembly.
def main():
    asm = Assembly.LoadFrom(ASM_PATH)
    flags = (
        BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Static
        | BindingFlags.Instance
    )

    wanted_exact = {
        "ScreenCapture",
        "LProtocolSyncStatus",
        "IDevice",
        "DeviceLight",
        "DeviceLamp",
        "DeviceBulb",
        "DeviceFloorLamp",
    }

    found = []
    for t in asm.GetTypes():
        if t.Name in wanted_exact:
            found.append(t)

    if not found:
        print("No target types found")
        return

    for t in found:
        dump_type(t, flags)


if __name__ == "__main__":
    main()
