# test_inspect_sync_config_api.py — Диагностика SyncConfig / SyncRGB API в Beelight.exe
#
# Этот скрипт ничего не отправляет в COM-порт.
# Он только через reflection показывает, какие типы и методы есть
# для SyncConfig, SyncRGB и ack listener.

import sys
sys.path.insert(0, ".")

import clr
from System.Reflection import Assembly, BindingFlags


ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"


def dump_type(t, flags):
    print(f"\n=== TYPE: {t.FullName} ===")

    try:
        ctors = t.GetConstructors(flags)
        for ctor in ctors:
            params = ", ".join([p.ParameterType.Name for p in ctor.GetParameters()])
            print(f"CTOR({params})")
    except Exception:
        pass

    for m in t.GetMethods(flags):
        params = ", ".join([f"{p.Name}:{p.ParameterType.Name}" for p in m.GetParameters()])
        print(f"{m.Name}({params}) -> {m.ReturnType.Name}")


def main():
    asm = Assembly.LoadFrom(ASM_PATH)
    flags = (
        BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Static
        | BindingFlags.Instance
    )

    wanted_type_names = {
        "LProtocol",
        "LProtocolSyncConfig",
        "LProtocolSyncRGB",
        "LP_SyncConfigAck",
        "LP_SwitcherAck",
        "LP_BrightAck",
        "LP_WorkModeAck",
    }

    found = []
    for t in asm.GetTypes():
        if t.Name in wanted_type_names:
            found.append(t)

    for t in found:
        dump_type(t, flags)

    print("\n=== EXTRA: delegate Invoke signatures ===")
    for t in found:
        if "MulticastDelegate" in str(t.BaseType):
            invoke = t.GetMethod("Invoke")
            if invoke is None:
                continue
            params = ", ".join([f"{p.Name}:{p.ParameterType.Name}" for p in invoke.GetParameters()])
            print(f"{t.Name}.Invoke({params}) -> {invoke.ReturnType.Name}")


if __name__ == "__main__":
    main()
