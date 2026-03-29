# -*- coding: utf-8 -*-
"""
analyze_correct_decrypt.py — Расшифровка capture пакетов правильным ключом.

Теперь мы знаем:
  keystream_period = pkt_len - 235
  key = cipher[2 : 2 + period]  (т.к. plain[2:2+period] = 0x00)

Проверяем:
  1. Правильная расшифровка LED data из capture
  2. Правильная расшифровка header structure
  3. Стабильные header bytes для каждого размера
"""
import os
import sys
from collections import defaultdict

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")


def correct_decrypt(pkt):
    """Расшифровка с правильным keystream_period = len(pkt) - 235."""
    pkt_len = len(pkt)
    if pkt_len < 238:
        return pkt
    period = pkt_len - 235
    # Key = cipher[2 : 2 + period], потому что plain[2:2+period] = 0x00
    key = pkt[2:2 + period]
    return bytes(pkt[i] ^ key[i % period] for i in range(pkt_len))


def parse_writes(filepath):
    writes = []
    with open(filepath, "r", errors="replace") as f:
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
    return writes


def main():
    writes = parse_writes(CSV_PATH)
    big_pkts = [w for w in writes if 238 <= len(w) <= 245]
    by_len = defaultdict(list)
    for pkt in big_pkts:
        by_len[len(pkt)].append(pkt)

    # ============================================================
    # 1. Correct decryption — header stability per size
    # ============================================================
    print("=" * 70)
    print("  CORRECT DECRYPTION — HEADER STABILITY")
    print("=" * 70)

    for pkt_len in sorted(by_len.keys()):
        pkts = by_len[pkt_len]
        period = pkt_len - 235
        color_start = pkt_len - 225

        plains = [correct_decrypt(p) for p in pkts]

        print(f"\n  len={pkt_len}, period={period}, color_start={color_start}, "
              f"n={len(pkts)}")

        # Стабильность каждого header byte
        stability = []
        for pos in range(color_start):
            vals = set(p[pos] for p in plains)
            if len(vals) == 1:
                stability.append(f"{list(vals)[0]:02x}")
            else:
                stability.append("**")

        print(f"    Stability: {' '.join(stability)}")

        # Первый пакет — полный header
        p0 = plains[0]
        print(f"    Header[0]: {p0[:color_start].hex()}")

        # Показываем первые LED
        leds = p0[color_start:]
        print(f"    First 5 LEDs: ", end="")
        for j in range(0, 15, 3):
            r, g, b = leds[j], leds[j + 1], leds[j + 2]
            print(f"({r},{g},{b}) ", end="")
        print()
        print(f"    Last LED[74]: ({leds[222]},{leds[223]},{leds[224]})")

    # ============================================================
    # 2. Universal header format — compare across all sizes
    # ============================================================
    print()
    print("=" * 70)
    print("  UNIVERSAL HEADER FORMAT")
    print("=" * 70)

    # Для каждого размера, покажем stable header pattern
    for pkt_len in sorted(by_len.keys()):
        pkts = by_len[pkt_len]
        period = pkt_len - 235
        color_start = pkt_len - 225
        plains = [correct_decrypt(p) for p in pkts]

        # Стабильные байты
        stable_map = {}
        for pos in range(color_start):
            vals = set(p[pos] for p in plains)
            if len(vals) == 1:
                stable_map[pos] = list(vals)[0]

        print(f"\n  len={pkt_len}: stable bytes at positions: "
              f"{dict((k, f'0x{v:02x}') for k, v in sorted(stable_map.items()))}")

    # ============================================================
    # 3. Verify .NET GenRGBTransferPackage with correct key
    # ============================================================
    print()
    print("=" * 70)
    print("  .NET GenRGBTransferPackage — CORRECT DECRYPTION")
    print("=" * 70)

    try:
        from soulight.protocol.bridge import BeelightBridge
        bridge = BeelightBridge()
        if not bridge.init():
            print("  Bridge init failed")
            return

        for name, color in [("RED", (255, 0, 0)), ("GREEN", (0, 255, 0)),
                            ("BLUE", (0, 0, 255)), ("WHITE", (255, 255, 255))]:
            pkt = bridge.make_rgb_transfer_packet([color] * 75)
            if pkt is None:
                print(f"  {name}: None")
                continue

            payload = pkt[5:] if pkt[:3] == b'\x55\xAA\x5A' else pkt
            plain = correct_decrypt(payload)
            pkt_len = len(payload)
            color_start = pkt_len - 225

            print(f"\n  .NET {name}: payload_len={pkt_len}, color_start={color_start}")
            print(f"    plain header: {plain[:color_start].hex()}")

            # LED data
            leds = plain[color_start:]
            led0 = (leds[0], leds[1], leds[2])
            led1 = (leds[3], leds[4], leds[5])
            led74 = (leds[222], leds[223], leds[224])
            print(f"    LED[0]={led0} LED[1]={led1} LED[74]={led74}")

            # Все LED одинаковые?
            expected = bytes([color[0], color[1], color[2]] * 75)
            match = leds == expected
            print(f"    All LEDs = {color}: {match}")
    except Exception as e:
        print(f"  Error: {e}")
        import traceback
        traceback.print_exc()

    # ============================================================
    # 4. Can we build packets from scratch?
    # ============================================================
    print()
    print("=" * 70)
    print("  PACKET CONSTRUCTION TEST")
    print("=" * 70)

    # Возьмём 238-byte capture packet, расшифруем, изменим LED data, зашифруем обратно
    test_pkt = by_len[238][0]
    plain = correct_decrypt(test_pkt)
    period = 3
    color_start = 13

    print(f"  Original 238-byte packet header: {plain[:color_start].hex()}")
    print(f"  Original LED[0:3]: ({plain[13]},{plain[14]},{plain[15]})")

    # Заменяем LED data на solid red
    new_plain = bytearray(plain)
    for i in range(75):
        new_plain[color_start + i * 3] = 255
        new_plain[color_start + i * 3 + 1] = 0
        new_plain[color_start + i * 3 + 2] = 0

    # Зашифровываем обратно тем же ключом
    key = test_pkt[2:2 + period]
    new_cipher = bytes(new_plain[i] ^ key[i % period] for i in range(len(new_plain)))

    # Проверяем: расшифровка нового пакета должна дать red
    verify = correct_decrypt(new_cipher)
    print(f"  Modified LED[0:3]: ({verify[13]},{verify[14]},{verify[15]})")
    print(f"  Modified LED[74]: ({verify[-3]},{verify[-2]},{verify[-1]})")
    print(f"  Key preserved: {new_cipher[2:5].hex() == test_pkt[2:5].hex()}")


if __name__ == "__main__":
    main()
