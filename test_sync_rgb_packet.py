# test_sync_rgb_packet.py — Тест LProtocolSyncRGB.GenProtocolSyncRGB

import sys
sys.path.insert(0, ".")

import clr
clr.AddReference("System.Drawing")
from System.Reflection import Assembly, BindingFlags
from System.Drawing import Color
from System import Array

def main():
    asm = Assembly.LoadFrom(r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe")
    
    # Находим LProtocolSyncRGB
    sync_rgb_type = None
    for t in asm.GetTypes():
        if t.Name == "LProtocolSyncRGB":
            sync_rgb_type = t
            break
    
    if sync_rgb_type is None:
        print("LProtocolSyncRGB не найден")
        return
    
    print(f"Найден: {sync_rgb_type.FullName}")
    
    # Ищем метод GenProtocolSyncRGB
    flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
    method = None
    for m in sync_rgb_type.GetMethods(flags):
        if m.Name == "GenProtocolSyncRGB":
            method = m
            print(f"\nМетод: {m.Name}")
            params = m.GetParameters()
            for p in params:
                print(f"  {p.Name}: {p.ParameterType}")
            print(f"  Return: {m.ReturnType}")
    
    if method is None:
        print("GenProtocolSyncRGB не найден")
        return
    
    # Пробуем вызвать с параметрами для 75 LED в виде 1 row × 75 columns
    try:
        from System import Byte as NetByte, Activator
        
        # Создаём экземпляр LProtocolSyncRGB
        print("\nСоздаём экземпляр LProtocolSyncRGB...")
        instance = Activator.CreateInstance(sync_rgb_type)
        
        colors_arr = Array.CreateInstance(Color, 75)
        
        # Заполняем: первые 10 = red, остальные = black
        for i in range(75):
            if i < 10:
                colors_arr[i] = Color.FromArgb(255, 0, 0)
            else:
                colors_arr[i] = Color.FromArgb(0, 0, 0)
        
        # Вызываем: rows=1, columns=75, colors
        print(f"Вызываем GenProtocolSyncRGB(rows=1, columns=75, colors[75])...")
        result = method.Invoke(instance, [NetByte(1), NetByte(75), colors_arr])
        
        if result is None:
            print("Результат: NULL")
        else:
            data = bytes(result)
            print(f"Результат: {len(data)} байт")
            print(f"Начало (hex): {data[:20].hex(' ')}")
            print(f"Начинается с 55 AA 5A: {data[:3] == b'\\x55\\xAA\\x5A'}")
            
            # Проверим ожидаемый размер для screen mirroring (238-245)
            if 238 <= len(data) <= 245:
                print("  ✓ Размер соответствует screen mirroring packet!")
    
    except Exception as e:
        print(f"ОШИБКА: {e}")
        import traceback
        traceback.print_exc()

if __name__ == "__main__":
    main()
