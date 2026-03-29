# -*- coding: utf-8 -*-
"""
test_handshake_then_dotnet_perled.py

Гипотеза: historical raw handshake устанавливает сессию,
после чего .NET GenRGBTransferPackage per-LED пакеты
могут быть приняты контроллером.

Фазы:
  0) Контроль: handshake + green replay (должен быть зелёный)
  1) handshake + .NET GenRGBTransferPackage solid RED
  2) handshake + .NET GenRGBTransferPackage 4-color pattern
  3) handshake + .NET LP GenColorPackage solid BLUE (known-good baseline)
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


def open_port():
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    return ser


def send_raw_writes(ser, writes):
    """Отправка raw-пакетов из handshake/green replay."""
    count = 0
    for raw in writes:
        ser.write(raw)
        ser.flush()
        count += 1
        if len(raw) >= 239:
            time.sleep(0.033)
        elif len(raw) == 5:
            time.sleep(0.005)
        else:
            time.sleep(0.01)
    return count


def send_dotnet_packets(ser, bridge, packets_fn, duration_sec=3.0):
    """Генерирует и отправляет .NET пакеты в цикле duration_sec секунд."""
    hb = bridge.get_heartbeat()
    bright = bridge.make_bright_packet(255)

    start = time.time()
    sent = 0
    while time.time() - start < duration_sec:
        pkt = packets_fn()
        if pkt is None:
            print("[!] packets_fn returned None")
            break

        # Яркость перед каждым цветовым пакетом
        if bright:
            ser.write(bright)
            ser.flush()
            time.sleep(0.005)

        ser.write(pkt)
        ser.flush()
        sent += 1
        time.sleep(0.033)

        # Heartbeat каждые ~10 пакетов
        if sent % 10 == 0 and hb:
            ser.write(hb)
            ser.flush()
            time.sleep(0.005)

    return sent


def main():
    print("=" * 60)
    print("HANDSHAKE + .NET PER-LED TEST")
    print("=" * 60)
    print()

    # Инициализация
    repo_path = os.path.dirname(os.path.abspath(__file__))
    protocol = RawGitReplayProtocol(repo_path=repo_path)
    bridge = BeelightBridge()
    if not bridge.init():
        print("[FAIL] Bridge init failed")
        return

    # Проверим, что GenRGBTransferPackage вообще генерирует пакет
    test_pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * NUM_LEDS)
    if test_pkt is None:
        print("[FAIL] GenRGBTransferPackage returned None")
        return
    print(f"[OK] GenRGBTransferPackage test packet: {len(test_pkt)} bytes")
    print(f"     First 20 bytes: {test_pkt[:20].hex()}")
    print()

    # ===== PHASE 0: Control — handshake + green replay =====
    print("--- PHASE 0: Control (handshake + green replay) ---")
    ser = open_port()
    try:
        hs_count = send_raw_writes(ser, protocol.iter_handshake_writes())
        gr_count = send_raw_writes(ser, protocol.iter_green_replay_writes(limit=80))
        print(f"[Phase 0] handshake={hs_count}, green={gr_count}")
        time.sleep(2.5)
    finally:
        ser.close()
    time.sleep(0.5)

    # ===== PHASE 1: handshake + .NET solid RED =====
    print("--- PHASE 1: Handshake + .NET GenRGBTransfer solid RED ---")
    ser = open_port()
    try:
        send_raw_writes(ser, protocol.iter_handshake_writes())
        sent = send_dotnet_packets(
            ser, bridge,
            lambda: bridge.make_rgb_transfer_packet([(255, 0, 0)] * NUM_LEDS),
            duration_sec=3.0,
        )
        print(f"[Phase 1] .NET RED packets sent = {sent}")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(0.5)

    # ===== PHASE 2: handshake + .NET 4-color pattern =====
    print("--- PHASE 2: Handshake + .NET GenRGBTransfer 4-color pattern ---")
    pattern = (
        [(255, 0, 0)] * 19
        + [(0, 255, 0)] * 19
        + [(0, 0, 255)] * 19
        + [(255, 255, 0)] * 18
    )
    ser = open_port()
    try:
        send_raw_writes(ser, protocol.iter_handshake_writes())
        sent = send_dotnet_packets(
            ser, bridge,
            lambda: bridge.make_rgb_transfer_packet(pattern),
            duration_sec=3.0,
        )
        print(f"[Phase 2] .NET pattern packets sent = {sent}")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(0.5)

    # ===== PHASE 3: handshake + LP GenColorPackage solid BLUE (baseline) =====
    print("--- PHASE 3: Handshake + LP GenColorPackage solid BLUE ---")
    ser = open_port()
    try:
        send_raw_writes(ser, protocol.iter_handshake_writes())
        sent = send_dotnet_packets(
            ser, bridge,
            lambda: bridge.make_color_packet(0, 0, 255),
            duration_sec=3.0,
        )
        print(f"[Phase 3] .NET BLUE packets sent = {sent}")
        time.sleep(2.0)
    finally:
        ser.close()

    print()
    print("[Test] Done. Report what you saw:")
    print("  Phase 0 (green replay):    ___")
    print("  Phase 1 (.NET solid RED):  ___")
    print("  Phase 2 (.NET 4-color):    ___")
    print("  Phase 3 (.NET solid BLUE): ___")


if __name__ == "__main__":
    main()
