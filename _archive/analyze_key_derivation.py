# -*- coding: utf-8 -*-
"""
analyze_key_derivation.py — Определяем точную формулу извлечения ключа.

Мы знаем:
  - period = pkt_len - 235
  - plain[2:5] = 0 (всегда), значит key[2%P], key[3%P], key[4%P] = cipher[2], cipher[3], cipher[4]
  - Для .NET пакетов plain[2:2+period] = 0 (подтверждено)

Проверяем формулу: key[i] = cipher[2 + ((i-2) % period)]
"""
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))


def derive_key_from_dotnet(cipher, period):
    """Для .NET пакетов (plain[2:2+period]=0): извлекает ключ через rotation."""
    key = [0] * period
    for i in range(period):
        src = 2 + ((i - 2) % period)
        key[i] = cipher[src]
    return bytes(key)


def decrypt_with_key(cipher, key):
    """Расшифровывает пакет данным ключом."""
    period = len(key)
    return bytes(cipher[i] ^ key[i % period] for i in range(len(cipher)))


def main():
    print("=" * 70)
    print("  KEY DERIVATION VERIFICATION")
    print("=" * 70)

    from soulight.protocol.bridge import BeelightBridge
    bridge = BeelightBridge()
    if not bridge.init():
        print("Bridge init failed")
        return

    for name, color in [
        ("BLACK", (0, 0, 0)),
        ("RED", (255, 0, 0)),
        ("GREEN", (0, 255, 0)),
        ("WHITE", (255, 255, 255)),
    ]:
        pkt = bridge.make_rgb_transfer_packet([color] * 75)
        if pkt is None:
            continue

        payload = pkt[5:] if pkt[:3] == b'\x55\xAA\x5A' else pkt
        pkt_len = len(payload)
        period = pkt_len - 235
        color_start = pkt_len - 225

        # Извлекаем ключ
        key = derive_key_from_dotnet(payload, period)

        # Расшифровываем
        plain = decrypt_with_key(payload, key)

        # Проверяем LED data
        expected_led = bytes([color[0], color[1], color[2]] * 75)
        actual_led = plain[color_start:]
        led_ok = actual_led == expected_led

        # Проверяем header structure
        header = plain[:color_start]

        print(f"\n  {name}: len={pkt_len}, period={period}, color_start={color_start}")
        print(f"    key = {key.hex()}")
        print(f"    plain header = {header.hex()}")
        print(f"    LED data correct: {led_ok}")

        if led_ok:
            # Анализируем header
            # Ищем 05 05 pattern
            for pos in range(2, color_start - 1):
                if header[pos] == 0x05 and header[pos + 1] == 0x05:
                    brightness_pos = pos + 2
                    e3_pos = pos + 3
                    led_count_pos = pos + 5
                    print(f"    05 05 at [{pos},{pos + 1}]")
                    print(f"    brightness byte [{brightness_pos}] = 0x{header[brightness_pos]:02x}")
                    print(f"    E3 byte [{e3_pos}] = 0x{header[e3_pos]:02x}")
                    print(f"    LED count byte [{led_count_pos}] = 0x{header[led_count_pos]:02x} ({header[led_count_pos]})")
                    break

    # ================================================================
    # Capture 238-byte packets: verify with old key3 approach
    # ================================================================
    print()
    print("=" * 70)
    print("  CAPTURE 238-byte VERIFICATION (key3 = old approach)")
    print("=" * 70)

    CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")
    writes = []
    with open(CSV_PATH, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            data_str = parts[5].strip()
            if not data_str:
                continue
            try:
                raw = bytes.fromhex(data_str.replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append(raw)

    cap238 = [w for w in writes if len(w) == 238]
    print(f"  {len(cap238)} capture 238-byte packets")

    for i, pkt in enumerate(cap238[:3]):
        # period=3, key derived same way
        key = derive_key_from_dotnet(pkt, 3)
        plain = decrypt_with_key(pkt, key)
        color_start = 13
        print(f"\n  Capture 238[{i}]:")
        print(f"    key = {key.hex()}")
        print(f"    plain header = {plain[:color_start].hex()}")
        print(f"    LED[0:3] = ({plain[13]},{plain[14]},{plain[15]})")

        # Check 05 05 position
        for pos in range(2, color_start - 1):
            if plain[pos] == 0x05 and plain[pos + 1] == 0x05:
                print(f"    05 05 at [{pos},{pos + 1}]")
                print(f"    brightness byte [{pos + 2}] = 0x{plain[pos + 2]:02x}")
                print(f"    E3 byte [{pos + 3}] = 0x{plain[pos + 3]:02x}")
                print(f"    LED count [{pos + 5}] = 0x{plain[pos + 5]:02x} ({plain[pos + 5]})")
                break

    # ================================================================
    # BRIGHTNESS HYPOTHESIS: compare capture vs .NET
    # ================================================================
    print()
    print("=" * 70)
    print("  BRIGHTNESS HYPOTHESIS")
    print("=" * 70)

    print("\n  .NET packets have brightness = 0x00 at color_start - 5")
    print("  Capture 238 packets have brightness = 0xFF at color_start - 5")
    print()
    print("  If we XOR the brightness byte with 0xFF, the controller")
    print("  should see brightness=255 and accept the packet!")
    print()

    # Generate a .NET RED packet, modify brightness, show result
    pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
    payload = pkt[5:]
    pkt_len = len(payload)
    period = pkt_len - 235
    key = derive_key_from_dotnet(payload, period)
    color_start = pkt_len - 225
    brightness_pos = color_start - 5

    print(f"  .NET RED: len={pkt_len}, brightness_pos={brightness_pos}")
    print(f"    Before: cipher[{brightness_pos}]=0x{payload[brightness_pos]:02x}")

    modified = bytearray(payload)
    modified[brightness_pos] ^= 0xFF

    print(f"    After:  cipher[{brightness_pos}]=0x{modified[brightness_pos]:02x}")

    # Verify decryption
    plain_mod = decrypt_with_key(bytes(modified), key)
    print(f"    Decrypted brightness: 0x{plain_mod[brightness_pos]:02x}")
    print(f"    LED[0] still correct: ({plain_mod[color_start]},{plain_mod[color_start + 1]},{plain_mod[color_start + 2]})")


if __name__ == "__main__":
    main()
