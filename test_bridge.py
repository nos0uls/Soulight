# test_bridge.py — Тест pythonnet bridge к Beelight.exe.
# Проверяет загрузку assembly, генерацию пакетов, и вывод hex.

import sys
sys.path.insert(0, ".")

from soulight.protocol.bridge import BeelightBridge

def hex_str(data):
    """Форматирует bytes в hex строку для отладки."""
    if data is None:
        return "NULL"
    return f"{len(data)}b: {data[:40].hex(' ')}"

def main():
    bridge = BeelightBridge()

    print("=== Инициализация bridge ===")
    ok = bridge.init()
    if not ok:
        print("ОШИБКА: bridge не инициализирован")
        return

    print(f"Ready: {bridge.ready}")

    print("\n=== Генерация пакетов ===")

    # Heartbeat (предгенерированный)
    hb = bridge.get_heartbeat()
    print(f"Heartbeat:     {hex_str(hb)}")

    # Color: Purple
    pkt = bridge.make_color_packet(255, 0, 255)
    print(f"Color(Purple): {hex_str(pkt)}")

    # Color: Red
    pkt = bridge.make_color_packet(255, 0, 0)
    print(f"Color(Red):    {hex_str(pkt)}")

    # Color: Green
    pkt = bridge.make_color_packet(0, 255, 0)
    print(f"Color(Green):  {hex_str(pkt)}")

    # Brightness 255
    pkt = bridge.make_bright_packet(255)
    print(f"Bright(255):   {hex_str(pkt)}")

    # Brightness 0
    pkt = bridge.make_bright_packet(0)
    print(f"Bright(0):     {hex_str(pkt)}")

    # Switch ON
    pkt = bridge.make_switch_packet(True)
    print(f"Switch(ON):    {hex_str(pkt)}")

    # Switch OFF
    pkt = bridge.make_switch_packet(False)
    print(f"Switch(OFF):   {hex_str(pkt)}")

    # WorkMode PC
    pkt = bridge.make_workmode_pc_packet()
    print(f"WorkMode(PC):  {hex_str(pkt)}")

    # Verify uniqueness: two color packets should have different nonces
    print("\n=== Проверка уникальности nonce ===")
    p1 = bridge.make_color_packet(255, 0, 255)
    p2 = bridge.make_color_packet(255, 0, 255)
    if p1 and p2:
        same = p1 == p2
        print(f"Два Purple пакета {'ОДИНАКОВЫЕ (плохо!)' if same else 'РАЗНЫЕ (ok, уникальный nonce)'}")
        print(f"  pkt1: {p1.hex(' ')}")
        print(f"  pkt2: {p2.hex(' ')}")

    print("\nТест завершён.")

if __name__ == "__main__":
    main()
