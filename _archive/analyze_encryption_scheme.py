# -*- coding: utf-8 -*-
"""
analyze_encryption_scheme.py — Определение реальной схемы шифрования
для пакетов 238-245 bytes путём:
  1. Генерации .NET пакетов с known plaintext
  2. Вычисления keystream = cipher ^ plaintext
  3. Определения периода keystream
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from soulight.protocol.bridge import BeelightBridge


def analyze_keystream(bridge, color, name):
    """Генерирует пакет с known color, извлекает keystream."""
    pkt = bridge.make_rgb_transfer_packet([color] * 75)
    if pkt is None:
        print(f"  {name}: None")
        return None

    # Убираем frame header если есть
    if pkt[:3] == b'\x55\xAA\x5A':
        payload = pkt[5:]
    else:
        payload = pkt

    pkt_len = len(payload)
    color_start = pkt_len - 225

    # Plaintext для header: nonce[0], nonce[1], 0,0,0, ... (остальное неизвестно)
    # Plaintext для LED region: 75 * (R, G, B)
    r, g, b = color
    expected_led = bytes([r, g, b] * 75)

    # Извлекаем keystream для LED region
    cipher_led = payload[color_start:]
    keystream_led = bytes(c ^ p for c, p in zip(cipher_led, expected_led))

    print(f"\n  {name}: payload_len={pkt_len}, color_start={color_start}")
    print(f"    cipher header: {payload[:color_start].hex()}")
    print(f"    keystream LED[0:24]: {keystream_led[:24].hex()}")
    print(f"    keystream LED[222:225]: {keystream_led[222:225].hex()}")

    # Проверяем период keystream
    # Если 3-byte period: ks[0]=ks[3]=ks[6]=...
    period3 = all(keystream_led[i] == keystream_led[i % 3] for i in range(len(keystream_led)))

    # Проверяем другие периоды
    for period in range(1, 31):
        match = all(
            keystream_led[i] == keystream_led[i % period]
            for i in range(min(len(keystream_led), period * 10))
        )
        if match and period <= 15:
            print(f"    *** keystream period = {period} ***")
            print(f"    key = {keystream_led[:period].hex()}")
            break
    else:
        # Не нашли периода <= 30, покажем первые отличия
        print(f"    No simple period found (checked 1..30)")
        # Покажем через сколько повторяется первый байт
        first = keystream_led[0]
        repeats = [i for i in range(1, 60) if i < len(keystream_led) and keystream_led[i] == first]
        print(f"    First byte 0x{first:02x} repeats at: {repeats[:10]}")

    return payload, keystream_led, pkt_len, color_start


def compare_two_packets(bridge, c1, c2, name1, name2):
    """Сравнивает два пакета с разным known plaintext чтобы вычислить keystream."""
    p1 = bridge.make_rgb_transfer_packet([c1] * 75)
    p2 = bridge.make_rgb_transfer_packet([c2] * 75)
    if p1 is None or p2 is None:
        print("  Failed to generate packets")
        return

    # Убираем frame header
    pay1 = p1[5:] if p1[:3] == b'\x55\xAA\x5A' else p1
    pay2 = p2[5:] if p2[:3] == b'\x55\xAA\x5A' else p2

    if len(pay1) != len(pay2):
        print(f"  Different payload lengths: {len(pay1)} vs {len(pay2)}, can't compare directly")
        return

    cs1 = len(pay1) - 225
    cs2 = len(pay2) - 225

    # XOR двух ciphertext'ов = XOR двух plaintext'ов
    # cipher1 ^ cipher2 = (plain1 ^ key) ^ (plain2 ^ key) = plain1 ^ plain2
    print(f"\n  Comparing {name1} vs {name2} (both len={len(pay1)}):")
    print(f"    Header XOR: {bytes(a ^ b for a, b in zip(pay1[:cs1], pay2[:cs2])).hex()}")

    led_xor = bytes(a ^ b for a, b in zip(pay1[cs1:], pay2[cs2:]))
    r1, g1, b1 = c1
    r2, g2, b2 = c2
    expected_xor = bytes([r1 ^ r2, g1 ^ g2, b1 ^ b2] * 75)

    match = led_xor == expected_xor
    print(f"    LED XOR matches expected plain1^plain2: {match}")
    if not match:
        print(f"    Expected LED XOR[0:12]: {expected_xor[:12].hex()}")
        print(f"    Actual   LED XOR[0:12]: {led_xor[:12].hex()}")
        # Найдём первое расхождение
        for i in range(len(led_xor)):
            if led_xor[i] != expected_xor[i]:
                print(f"    First mismatch at LED byte [{i}]: "
                      f"expected 0x{expected_xor[i]:02x}, got 0x{led_xor[i]:02x}")
                break


def main():
    print("=" * 70)
    print("  ENCRYPTION SCHEME ANALYSIS")
    print("=" * 70)

    bridge = BeelightBridge()
    if not bridge.init():
        print("Bridge init failed")
        return

    # Генерируем пакеты с known colors и извлекаем keystream
    results = {}
    for name, color in [
        ("ALL_BLACK", (0, 0, 0)),
        ("ALL_RED", (255, 0, 0)),
        ("ALL_GREEN", (0, 255, 0)),
        ("ALL_BLUE", (0, 0, 255)),
        ("ALL_WHITE", (255, 255, 255)),
        ("R=1", (1, 0, 0)),
        ("G=1", (0, 1, 0)),
        ("B=1", (0, 0, 1)),
    ]:
        result = analyze_keystream(bridge, color, name)
        if result:
            results[name] = result

    # Сравним пакеты попарно
    print()
    print("=" * 70)
    print("  CROSS-PACKET COMPARISON (cipher1 XOR cipher2 = plain1 XOR plain2?)")
    print("=" * 70)

    # Генерируем 10 пакетов ALL_RED и проверяем, одинаковый ли keystream
    print("\n  --- 5 consecutive ALL_RED packets ---")
    red_packets = []
    for i in range(5):
        pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
        if pkt and pkt[:3] == b'\x55\xAA\x5A':
            pay = pkt[5:]
            red_packets.append(pay)
            cs = len(pay) - 225
            print(f"    [{i}] len={len(pay)} cipher[0:5]={pay[:5].hex()} "
                  f"cipher[cs:cs+6]={pay[cs:cs+6].hex()}")

    # Все ли одинаковой длины?
    lens = set(len(p) for p in red_packets)
    print(f"    Unique lengths: {lens}")

    # Keystream одинаковый?
    if len(red_packets) >= 2 and len(red_packets[0]) == len(red_packets[1]):
        xor01 = bytes(a ^ b for a, b in zip(red_packets[0], red_packets[1]))
        all_zero = all(b == 0 for b in xor01)
        print(f"    pkt[0] XOR pkt[1] all-zero (same keystream): {all_zero}")
        if not all_zero:
            # Keystream varies per packet (nonce-based)
            print(f"    First 20 bytes of XOR: {xor01[:20].hex()}")


if __name__ == "__main__":
    main()
