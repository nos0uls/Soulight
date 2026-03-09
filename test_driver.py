# test_driver.py — Тест LEDDriver: подключение, цвета, отключение.
# Отправляет Purple → Red → Green → Blue → White → OFF.

import time
import sys
sys.path.insert(0, ".")

from soulight.protocol.serial_driver import LEDDriver

def main():
    driver = LEDDriver()

    print("=== Подключение ===")
    if not driver.connect():
        print("Не удалось подключиться!")
        return

    colors = [
        (255, 0, 255, "Purple"),
        (255, 0, 0,   "Red"),
        (0, 255, 0,   "Green"),
        (0, 0, 255,   "Blue"),
        (255, 255, 255,"White"),
    ]

    for r, g, b, name in colors:
        print(f"\n  {name}: RGB({r}, {g}, {b})")
        driver.set_color(r, g, b)
        time.sleep(3)

    print("\n=== Отключение ===")
    driver.disconnect()
    print("Тест завершён.")

if __name__ == "__main__":
    main()
