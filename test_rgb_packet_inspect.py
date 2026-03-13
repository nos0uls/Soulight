# test_rgb_packet_inspect.py — Проверка формата RGB transfer пакета

import sys
sys.path.insert(0, ".")

from soulight.protocol.bridge import BeelightBridge

def main():
    bridge = BeelightBridge()
    if not bridge.init():
        print("Bridge init failed")
        return

    # Генерируем simple test: первые 10 = red, остальные = black
    colors = [(255, 0, 0)] * 10 + [(0, 0, 0)] * 65
    
    print(f"[Test] Генерирую пакет для {len(colors)} LED...")
    pkt = bridge.make_rgb_transfer_packet(colors)
    
    if pkt is None:
        print("[Test] ERROR: пакет = None")
        return
    
    print(f"[Test] Размер пакета: {len(pkt)} байт")
    print(f"[Test] Начало (hex): {pkt[:20].hex(' ')}")
    print(f"[Test] Начинается с 55 AA 5A: {pkt[:3] == b'\\x55\\xAA\\x5A'}")
    
    # Сравним с solid color пакетом
    print("\n[Test] Для сравнения — solid color пакет:")
    color_pkt = bridge.make_color_packet(255, 0, 0)
    if color_pkt:
        print(f"  Размер: {len(color_pkt)} байт")
        print(f"  Начало: {color_pkt[:20].hex(' ')}")
    
    # Brightness пакет
    bright_pkt = bridge.make_bright_packet(255)
    if bright_pkt:
        print(f"\n[Test] Brightness пакет:")
        print(f"  Размер: {len(bright_pkt)} байт")
        print(f"  Начало: {bright_pkt[:20].hex(' ')}")

if __name__ == "__main__":
    main()
