# test_per_led_live.py — Тест per-LED на реальной ленте
# Отправляет 4-цветный паттерн: R/G/B/Y по сторонам

import sys
sys.path.insert(0, ".")

from soulight.protocol.serial_driver import LEDDriver
import time

def main():
    driver = LEDDriver()
    
    print("[Test] Подключаюсь к COM7...")
    if not driver.connect():
        print("[Test] ОШИБКА: не удалось подключиться")
        return
    
    print("[Test] Подключено. Отправляю per-LED цвета...")
    
    # Формируем массив: 23 red, 15 green, 23 blue, 14 yellow
    colors = []
    colors.extend([(255, 0, 0)] * 23)      # Top = Red
    colors.extend([(0, 255, 0)] * 15)      # Right = Green
    colors.extend([(0, 0, 255)] * 23)      # Bottom = Blue
    colors.extend([(255, 255, 0)] * 14)    # Left = Yellow
    
    print(f"[Test] Отправляю {len(colors)} LED...")
    driver.set_per_led_colors(colors)
    
    print("[Test] Жду 5 секунд...")
    time.sleep(5)
    
    print("[Test] Меняю на все белые...")
    driver.set_per_led_colors([(255, 255, 255)] * 75)
    time.sleep(3)
    
    print("[Test] Отключаюсь...")
    driver.disconnect()
    print("[Test] Готово.")

if __name__ == "__main__":
    main()
