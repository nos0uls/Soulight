# test_sync_rgb_live.py — Тест GenProtocolSyncRGB на реальной ленте

import sys
sys.path.insert(0, ".")

import clr
clr.AddReference("System.Drawing")
from System.Reflection import Assembly, BindingFlags
from System.Drawing import Color
from System import Array, Byte as NetByte, Activator
import serial
import time

def main():
    # Загружаем assembly и находим LProtocolSyncRGB
    asm = Assembly.LoadFrom(r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe")
    
    sync_rgb_type = None
    for t in asm.GetTypes():
        if t.Name == "LProtocolSyncRGB":
            sync_rgb_type = t
            break
    
    if sync_rgb_type is None:
        print("LProtocolSyncRGB не найден")
        return
    
    # Находим метод
    flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
    method = None
    for m in sync_rgb_type.GetMethods(flags):
        if m.Name == "GenProtocolSyncRGB":
            method = m
            break
    
    if method is None:
        print("GenProtocolSyncRGB не найден")
        return
    
    # Создаём instance
    instance = Activator.CreateInstance(sync_rgb_type)
    
    # Формируем массив цветов: 23 red, 15 green, 23 blue, 14 yellow
    colors_arr = Array.CreateInstance(Color, 75)
    for i in range(75):
        if i < 23:
            colors_arr[i] = Color.FromArgb(255, 0, 0)  # Red
        elif i < 38:
            colors_arr[i] = Color.FromArgb(0, 255, 0)  # Green
        elif i < 61:
            colors_arr[i] = Color.FromArgb(0, 0, 255)  # Blue
        else:
            colors_arr[i] = Color.FromArgb(255, 255, 0)  # Yellow
    
    print("[Test] Генерирую screen mirroring пакет...")
    packet = method.Invoke(instance, [NetByte(1), NetByte(75), colors_arr])
    
    if packet is None:
        print("[Test] ERROR: packet = None")
        return
    
    data = bytes(packet)
    print(f"[Test] Размер: {len(data)} байт")
    print(f"[Test] Начало: {data[:20].hex(' ')}")
    
    # Открываем COM7
    print("[Test] Подключаюсь к COM7...")
    ser = serial.Serial("COM7", 500000, timeout=1)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.1)
    
    print("[Test] Отправляю пакет...")
    ser.write(data)
    time.sleep(0.05)
    
    # Повторяем несколько раз для стабильности
    for _ in range(20):
        ser.write(data)
        time.sleep(0.070)
    
    print("[Test] Отправлено. Жду 3 секунды...")
    time.sleep(3)
    
    print("[Test] Закрываю порт...")
    ser.close()
    print("[Test] Готово.")

if __name__ == "__main__":
    main()
