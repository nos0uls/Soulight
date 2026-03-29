# -*- coding: utf-8 -*-
"""
test_capture_isolate.py — Изолирующий тест: какой тип пакетов управляет LEDs?

Phase A: Capture hs + ONLY 238-byte packets → что покажет?
Phase B: Capture hs + ONLY 239-245 byte packets → что покажет?
Phase C: Capture hs + modified 238-byte packets (LED=RED, brightness=0xFF) → цвет?
Phase D: Capture hs + modified 238-byte packets (LED=RED, brightness=0x00) → тёмно?
Phase E: Capture hs + XOR delta 239-245 to uniform RED → работает?

Это определит, какой пакет-класс управляет лентой и можно ли менять цвет.
"""
import os
import sys
import time
from collections import defaultdict

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import serial

COM_PORT = "COM7"
BAUD = 500000
CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")
NUM_LEDS = 75


def parse_write_downs(filepath):
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f, 1):
            if line_no == 1:
                continue
            parts = line.split(";")
            if len(parts) < 6:
                continue
            if "IRP_MJ_WRITE" not in parts[2] or parts[3].strip() != "DOWN":
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


def extract_handshake_and_color(writes):
    """Разделяет writes на handshake и color packets."""
    first_big = None
    for i, w in enumerate(writes):
        if len(w) >= 238:
            first_big = i
            break
    if first_big is None:
        return writes, []

    # Handshake core (без frame header последнего)
    hs = writes[:first_big]
    if hs and hs[-1][:3] == b'\x55\xAA\x5A':
        hs_core = hs[:-1]
        color_start = first_big - 1
    else:
        hs_core = hs
        color_start = first_big
    return hs_core, writes[color_start:]


def pair_frame_and_payload(color_writes):
    """Собирает пары (frame_header, payload) из color writes."""
    pairs = []
    i = 0
    while i < len(color_writes):
        w = color_writes[i]
        if len(w) == 5 and w[:3] == b'\x55\xAA\x5A':
            if i + 1 < len(color_writes) and len(color_writes[i + 1]) >= 238:
                pairs.append((w, color_writes[i + 1]))
                i += 2
                continue
        i += 1
    return pairs


def open_port():
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    return ser


def send_handshake(ser, hs_core):
    for w in hs_core:
        ser.write(w)
        ser.flush()
        time.sleep(0.05 if len(w) == 5 else 0.1)
    time.sleep(0.5)
    ser.read(4096)


def send_pairs(ser, pairs, limit=80):
    """Отправляет пары frame_header + payload."""
    sent = 0
    for fh, payload in pairs[:limit]:
        ser.write(fh)
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        time.sleep(0.033)
        sent += 1
    return sent


def key3_for_238(pkt):
    """Извлекает полный 3-byte ключ для 238-byte пакета."""
    return bytes([pkt[3], pkt[4], pkt[2]])


def modify_238_leds(pkt, led_colors, brightness=0xFF):
    """
    Модифицирует LED data и brightness в 238-byte capture пакете.

    pkt: 238-byte cipher packet
    led_colors: list of 75 (R,G,B) tuples
    brightness: 0x00-0xFF
    """
    key3 = key3_for_238(pkt)
    # Расшифровываем
    plain = bytearray(pkt[i] ^ key3[i % 3] for i in range(238))

    # Устанавливаем brightness (position 8 = color_start - 5)
    plain[8] = brightness

    # Устанавливаем LED data (position 13..237)
    for i in range(NUM_LEDS):
        r, g, b = led_colors[i]
        plain[13 + i * 3] = r
        plain[13 + i * 3 + 1] = g
        plain[13 + i * 3 + 2] = b

    # Зашифровываем обратно
    return bytes(plain[i] ^ key3[i % 3] for i in range(238))


def modify_big_pkt_uniform_xor(pkt, target_rgb, base_rgb=(0, 0, 0)):
    """
    Модифицирует LED data через XOR delta для пакета любого размера.
    Работает только для UNIFORM color (все LED одинаковый цвет).

    XOR delta не зависит от ключа!
    new_cipher[i] = old_cipher[i] XOR (new_plain[i] XOR old_plain[i])
    """
    pkt_len = len(pkt)
    color_start = pkt_len - 225
    result = bytearray(pkt)

    tr, tg, tb = target_rgb
    br, bg, bb = base_rgb

    for i in range(NUM_LEDS):
        pos = color_start + i * 3
        result[pos] ^= (tr ^ br)
        result[pos + 1] ^= (tg ^ bg)
        result[pos + 2] ^= (tb ^ bb)

    return bytes(result)


def main():
    print("=" * 60)
    print("  CAPTURE ISOLATE TEST")
    print("=" * 60)

    writes = parse_write_downs(CSV_PATH)
    hs_core, color_writes = extract_handshake_and_color(writes)
    pairs = pair_frame_and_payload(color_writes)

    # Разделяем по размерам
    pairs_238 = [(fh, pl) for fh, pl in pairs if len(pl) == 238]
    pairs_big = [(fh, pl) for fh, pl in pairs if 239 <= len(pl) <= 245]

    print(f"  Handshake: {len(hs_core)} writes")
    print(f"  Total pairs: {len(pairs)}")
    print(f"  238-byte pairs: {len(pairs_238)}")
    print(f"  239-245 pairs: {len(pairs_big)}")

    # ================================================================
    # Phase A: ONLY 238-byte packets
    # ================================================================
    print("\n--- Phase A: Capture hs + ONLY 238-byte packets ---")
    ser = open_port()
    try:
        send_handshake(ser, hs_core)
        sent = send_pairs(ser, pairs_238, limit=80)
        print(f"  Sent {sent} 238-byte packets")
        time.sleep(3.0)
    finally:
        ser.close()
    time.sleep(1.5)

    # ================================================================
    # Phase B: ONLY 239-245 byte packets
    # ================================================================
    print("\n--- Phase B: Capture hs + ONLY 239-245 byte packets ---")
    ser = open_port()
    try:
        send_handshake(ser, hs_core)
        sent = send_pairs(ser, pairs_big, limit=80)
        print(f"  Sent {sent} big packets")
        time.sleep(3.0)
    finally:
        ser.close()
    time.sleep(1.5)

    # ================================================================
    # Phase C: Modified 238-byte → LED=RED, brightness=0xFF
    # ================================================================
    print("\n--- Phase C: Capture hs + modified 238-byte (RED, bright=FF) ---")
    ser = open_port()
    try:
        send_handshake(ser, hs_core)
        red_colors = [(255, 0, 0)] * NUM_LEDS
        sent = 0
        for fh, pl in pairs_238[:80]:
            modified = modify_238_leds(pl, red_colors, brightness=0xFF)
            ser.write(fh)
            ser.flush()
            time.sleep(0.005)
            ser.write(modified)
            ser.flush()
            time.sleep(0.033)
            sent += 1
        print(f"  Sent {sent} modified 238-byte RED packets")
        time.sleep(3.0)
    finally:
        ser.close()
    time.sleep(1.5)

    # ================================================================
    # Phase D: Modified 238-byte → LED=RED, brightness=0x00
    # ================================================================
    print("\n--- Phase D: Capture hs + modified 238-byte (RED, bright=0x00) ---")
    ser = open_port()
    try:
        send_handshake(ser, hs_core)
        sent = 0
        for fh, pl in pairs_238[:80]:
            modified = modify_238_leds(pl, red_colors, brightness=0x00)
            ser.write(fh)
            ser.flush()
            time.sleep(0.005)
            ser.write(modified)
            ser.flush()
            time.sleep(0.033)
            sent += 1
        print(f"  Sent {sent} modified 238-byte RED bright=0 packets")
        time.sleep(3.0)
    finally:
        ser.close()
    time.sleep(1.5)

    # ================================================================
    # Phase E: XOR delta on ALL capture pairs → uniform RED
    # (Assumes base color = white (255,255,255) based on user report)
    # ================================================================
    print("\n--- Phase E: XOR delta ALL capture packets -> uniform RED ---")
    print("    (assuming base=white, target=red)")
    ser = open_port()
    try:
        send_handshake(ser, hs_core)
        sent = 0
        for fh, pl in pairs[:100]:
            # Для 238-byte: base=black (0,0,0), target=red
            if len(pl) == 238:
                modified = modify_big_pkt_uniform_xor(pl, (255, 0, 0), (0, 0, 0))
            else:
                # Для 239-245: base=white (255,255,255), target=red
                modified = modify_big_pkt_uniform_xor(pl, (255, 0, 0), (255, 255, 255))
            ser.write(fh)
            ser.flush()
            time.sleep(0.005)
            ser.write(modified)
            ser.flush()
            time.sleep(0.033)
            sent += 1
        print(f"  Sent {sent} XOR-delta packets")
        time.sleep(3.0)
    finally:
        ser.close()

    print()
    print("[Test] Done. Report what you saw:")
    print("  Phase A (ONLY 238):                ___")
    print("  Phase B (ONLY 239-245):            ___")
    print("  Phase C (238 RED bright=FF):       ___")
    print("  Phase D (238 RED bright=0x00):     ___")
    print("  Phase E (XOR delta -> RED):         ___")


if __name__ == "__main__":
    main()
