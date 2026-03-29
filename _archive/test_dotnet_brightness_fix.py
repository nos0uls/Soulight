# -*- coding: utf-8 -*-
"""
test_dotnet_brightness_fix.py — Критический hardware test.

Гипотеза: .NET GenRGBTransferPackage не работал потому что brightness byte = 0x00.
Исправляем XOR-ом brightness byte на 0xFF и проверяем на железе.

Фазы:
  0) Контроль: handshake + green replay (должен быть зелёный)
  1) .NET GenRGBTransferPackage solid RED, brightness=0xFF (FIXED)
  2) .NET GenRGBTransferPackage 4-color pattern, brightness=0xFF (FIXED)
  3) .NET GenRGBTransferPackage solid RED, brightness=0x00 (ORIGINAL, для сравнения)
"""
import os
import sys
import time

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import serial
from soulight.protocol.raw_git_replay import RawGitReplayProtocol
from soulight.protocol.bridge import BeelightBridge

COM_PORT = "COM7"
BAUD = 500000
NUM_LEDS = 75


def fix_brightness(pkt_with_header, brightness=0xFF):
    """
    Модифицирует brightness byte в зашифрованном .NET пакете.

    Структура plaintext: ... 05 05 [brightness] E3 00 4B 00 [225 LED bytes]
    brightness_pos = color_start - 5 = (payload_len - 225) - 5 = payload_len - 230

    Модификация: cipher[pos] ^= (target_brightness ^ current_brightness)
    Текущий brightness=0x00, поэтому delta = target_brightness.
    """
    # Убираем frame header (5 bytes: 55 AA 5A <len> 00)
    frame_hdr = pkt_with_header[:5]
    payload = bytearray(pkt_with_header[5:])

    brightness_pos = len(payload) - 230
    # XOR delta: от 0x00 к target brightness
    payload[brightness_pos] ^= brightness

    return bytes(frame_hdr) + bytes(payload)


def open_port():
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    return ser


def send_raw_writes(ser, writes):
    count = 0
    for raw in writes:
        ser.write(raw)
        ser.flush()
        count += 1
        if len(raw) >= 238:
            time.sleep(0.033)
        elif len(raw) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.01)
    return count


def send_packets_loop(ser, packet_fn, duration_sec=4.0):
    """Отправляет пакеты в цикле. packet_fn() -> bytes (full packet with frame header)."""
    start = time.time()
    sent = 0
    while time.time() - start < duration_sec:
        pkt = packet_fn()
        if pkt is None:
            print("[!] packet_fn returned None")
            break

        # Отправляем frame header и payload отдельно (как делает Beelight)
        frame_hdr = pkt[:5]
        payload = pkt[5:]
        ser.write(frame_hdr)
        ser.flush()
        time.sleep(0.005)
        ser.write(payload)
        ser.flush()
        sent += 1
        time.sleep(0.033)

    return sent


def main():
    print("=" * 60)
    print("  BRIGHTNESS FIX HARDWARE TEST")
    print("=" * 60)
    print()

    repo_path = os.path.dirname(os.path.abspath(__file__))
    protocol = RawGitReplayProtocol(repo_path=repo_path)
    bridge = BeelightBridge()
    if not bridge.init():
        print("[FAIL] Bridge init failed")
        return

    # Проверим fix_brightness
    test_pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * NUM_LEDS)
    fixed = fix_brightness(test_pkt, 0xFF)
    print(f"[OK] Test packet: {len(test_pkt)} bytes -> fixed: {len(fixed)} bytes")
    print()

    # ===== PHASE 0: Control — handshake + green replay =====
    print("--- PHASE 0: Control (handshake + green replay) ---")
    ser = open_port()
    try:
        hs = send_raw_writes(ser, protocol.iter_handshake_writes())
        gr = send_raw_writes(ser, protocol.iter_green_replay_writes(limit=80))
        print(f"  handshake={hs}, green={gr}")
        time.sleep(3.0)
    finally:
        ser.close()
    time.sleep(1.0)

    # ===== PHASE 1: handshake + .NET RED brightness=0xFF =====
    print("--- PHASE 1: Handshake + .NET RED brightness=0xFF (FIXED) ---")
    ser = open_port()
    try:
        send_raw_writes(ser, protocol.iter_handshake_writes())
        sent = send_packets_loop(
            ser,
            lambda: fix_brightness(
                bridge.make_rgb_transfer_packet([(255, 0, 0)] * NUM_LEDS), 0xFF
            ),
            duration_sec=4.0,
        )
        print(f"  sent={sent} fixed RED packets")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(1.0)

    # ===== PHASE 2: handshake + .NET 4-color brightness=0xFF =====
    print("--- PHASE 2: Handshake + .NET 4-color pattern brightness=0xFF (FIXED) ---")
    pattern = (
        [(255, 0, 0)] * 19      # RED
        + [(0, 255, 0)] * 19    # GREEN
        + [(0, 0, 255)] * 19    # BLUE
        + [(255, 255, 0)] * 18  # YELLOW
    )
    ser = open_port()
    try:
        send_raw_writes(ser, protocol.iter_handshake_writes())
        sent = send_packets_loop(
            ser,
            lambda: fix_brightness(
                bridge.make_rgb_transfer_packet(pattern), 0xFF
            ),
            duration_sec=4.0,
        )
        print(f"  sent={sent} fixed pattern packets")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(1.0)

    # ===== PHASE 3: handshake + .NET RED brightness=0x00 (ORIGINAL) =====
    print("--- PHASE 3: Handshake + .NET RED brightness=0x00 (ORIGINAL) ---")
    ser = open_port()
    try:
        send_raw_writes(ser, protocol.iter_handshake_writes())
        sent = send_packets_loop(
            ser,
            lambda: bridge.make_rgb_transfer_packet([(255, 0, 0)] * NUM_LEDS),
            duration_sec=4.0,
        )
        print(f"  sent={sent} original RED packets")
        time.sleep(2.0)
    finally:
        ser.close()

    print()
    print("[Test] Done. Report what you saw:")
    print("  Phase 0 (green replay):          ___")
    print("  Phase 1 (.NET RED, bright=FF):   ___")
    print("  Phase 2 (.NET 4-color, bright=FF): ___")
    print("  Phase 3 (.NET RED, bright=00):   ___")


if __name__ == "__main__":
    main()
