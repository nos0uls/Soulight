# test_enum_workmodes.py — Перечисление всех LP_WK_MODE значений

import sys
sys.path.insert(0, ".")

from soulight.protocol.bridge import BeelightBridge
from System.Reflection import Assembly, BindingFlags
from System import Enum as SysEnum

def main():
    # Загружаем assembly
    asm = Assembly.LoadFrom(r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe")
    
    # Ищем LP_WK_MODE enum
    wk_mode_type = None
    for t in asm.GetTypes():
        if t.Name == "LP_WK_MODE":
            wk_mode_type = t
            break
    
    if wk_mode_type is None:
        print("LP_WK_MODE не найден")
        return
    
    print("=== LP_WK_MODE enum ===")
    for name in SysEnum.GetNames(wk_mode_type):
        val = SysEnum.Parse(wk_mode_type, name)
        print(f"  {name} = {int(val)}")
    
    # Также проверим LP_CTRL
    print("\n=== LP_CTRL enum ===")
    for t in asm.GetTypes():
        if t.Name == "LP_CTRL":
            for name in SysEnum.GetNames(t):
                val = SysEnum.Parse(t, name)
                print(f"  {name} = {int(val)}")
            break
    
    # LP_CMD
    print("\n=== LP_CMD enum ===")
    for t in asm.GetTypes():
        if t.Name == "LP_CMD":
            for name in SysEnum.GetNames(t):
                val = SysEnum.Parse(t, name)
                print(f"  {name} = {int(val)}")
            break

if __name__ == "__main__":
    main()
