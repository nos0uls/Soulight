# test_find_sync_rgb_methods.py — Ищем методы, связанные с SYNC_RGB

import sys
sys.path.insert(0, ".")

import clr
from System.Reflection import Assembly, BindingFlags

def main():
    asm = Assembly.LoadFrom(r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe")
    
    flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
    
    print("=== Методы с RGB/Transfer/Sync в имени ===\n")
    
    for t in asm.GetTypes():
        for m in t.GetMethods(flags):
            name = m.Name.lower()
            if any(kw in name for kw in ['rgb', 'transfer', 'sync']):
                params = m.GetParameters()
                param_str = ", ".join([f"{p.Name}:{p.ParameterType.Name}" for p in params])
                print(f"{t.Name}.{m.Name}({param_str}) -> {m.ReturnType.Name}")
    
    print("\n=== Проверка GenColorFrame vs GenRGBTransferPackage ===")
    # Возможно есть метод GenColorFrame или GenSyncRGBFrame
    for t in asm.GetTypes():
        if "Protocol" in t.Name:
            for m in t.GetMethods(flags):
                if "Frame" in m.Name or "Package" in m.Name:
                    params = m.GetParameters()
                    if len(params) > 0:
                        param_str = ", ".join([f"{p.ParameterType.Name}" for p in params])
                        print(f"  {t.Name}.{m.Name}({param_str})")

if __name__ == "__main__":
    main()
