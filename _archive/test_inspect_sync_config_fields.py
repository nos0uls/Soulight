# test_inspect_sync_config_fields.py — Поля и внутреннее состояние LProtocolSyncConfig

import sys
sys.path.insert(0, ".")

import clr
from System.Reflection import Assembly, BindingFlags
from System import Activator

ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"


def main():
    asm = Assembly.LoadFrom(ASM_PATH)
    flags = (
        BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Static
        | BindingFlags.Instance
    )

    target = None
    for t in asm.GetTypes():
        if t.Name == "LProtocolSyncConfig":
            target = t
            break

    if target is None:
        print("LProtocolSyncConfig not found")
        return

    print(f"TYPE: {target.FullName}")
    print("\n=== Constructors ===")
    for ctor in target.GetConstructors(flags):
        params = ", ".join([p.ParameterType.Name for p in ctor.GetParameters()])
        print(f"CTOR({params})")

    instance = None
    try:
        instance = Activator.CreateInstance(target, True)
        print("\nInstance created with nonPublic/default ctor")
    except Exception as e:
        print(f"\nCreateInstance default failed: {e}")

    print("\n=== Fields ===")
    for f in target.GetFields(flags):
        print(f"{f.Name}: {f.FieldType}")
        if instance is not None:
            try:
                print(f"  value = {f.GetValue(instance)}")
            except Exception as e:
                print(f"  value error = {e}")

    print("\n=== Properties ===")
    for p in target.GetProperties(flags):
        print(f"{p.Name}: {p.PropertyType}")
        if instance is not None and p.CanRead:
            try:
                print(f"  value = {p.GetValue(instance, None)}")
            except Exception as e:
                print(f"  value error = {e}")


if __name__ == "__main__":
    main()
