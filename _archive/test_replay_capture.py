# -*- coding: utf-8 -*-
"""
test_replay_capture.py — Воспроизводит ТОЧНУЮ последовательность из replay.csv.

Фазы:
  1) Replay capture handshake (6 пар frame+payload) + первые 100 color packets
  2) Replay capture handshake + .NET GenRGBTransferPackage (brightness-fixed)

Если фаза 1 работает → capture handshake = правильный
Если фаза 2 работает → можно генерировать свои пакеты!
"""
import os
import sys
import time

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

import serial

COM_PORT = "COM7"
BAUD = 500000
CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "replay.csv")


def parse_write_downs(filepath):
    """Парсит все IRP_MJ_WRITE;DOWN пакеты из capture."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f, 1):
            if line_no == 1:
                continue
            parts = line.split(";")
            if len(parts) < 6:
                continue
            func = parts[2].strip()
            direction = parts[3].strip()
            if "IRP_MJ_WRITE" not in func or direction != "DOWN":
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


def open_port():
    """Открывает COM порт с настройками контроллера."""
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    return ser


def send_write(ser, data, delay=0.01):
    """Отправляет один пакет и ждёт."""
    ser.write(data)
    ser.flush()
    time.sleep(delay)


def main():
    print("=" * 60)
    print("  REPLAY CAPTURE TEST")
    print("=" * 60)

    writes = parse_write_downs(CSV_PATH)
    print(f"\n  Total WRITE;DOWN packets: {len(writes)}")

    # Разделяем на handshake и color packets
    first_big_idx = None
    for i, w in enumerate(writes):
        if len(w) >= 238:
            first_big_idx = i
            break

    if first_big_idx is None:
        print("[FAIL] No big packets found!")
        return

    # Handshake = всё до первого большого пакета, ВКЛЮЧАЯ frame header
    # Но frame header перед первым большим = часть color data sequence
    # Handshake заканчивается за 1 write до first_big_idx (frame header для первого big)
    # На самом деле, frame header тоже может быть частью handshake
    # Берём все пакеты до first_big_idx
    handshake = writes[:first_big_idx]

    # Последний handshake пакет = frame header для первого color packet
    # Не включаем его в handshake, он идёт с color packet
    if handshake and handshake[-1][:3] == b'\x55\xAA\x5A':
        handshake_core = handshake[:-1]
        color_frame_start = first_big_idx - 1
    else:
        handshake_core = handshake
        color_frame_start = first_big_idx

    # Color packets = от color_frame_start до конца (включает 5-byte headers + payloads)
    color_writes = writes[color_frame_start:]

    print(f"  Handshake core: {len(handshake_core)} writes")
    print(f"  Color writes: {len(color_writes)} writes (first 200 used)")

    for i, w in enumerate(handshake_core):
        if w[:3] == b'\x55\xAA\x5A':
            print(f"    hs[{i}]: FRAME {w.hex()}")
        else:
            print(f"    hs[{i}]: DATA  len={len(w)} {w.hex()[:40]}...")

    # ================================================================
    # PHASE 1: Replay exact capture (handshake + color packets)
    # ================================================================
    print()
    print("--- PHASE 1: Replay capture handshake + first 200 color writes ---")

    ser = open_port()
    try:
        # Handshake
        for i, w in enumerate(handshake_core):
            delay = 0.05 if len(w) == 5 else 0.1
            send_write(ser, w, delay=delay)

        # Wait for controller to process handshake
        time.sleep(0.5)

        # Read any response
        resp = ser.read(4096)
        if resp:
            print(f"  Controller response after handshake: {len(resp)} bytes")
            print(f"    hex: {resp[:40].hex()}...")

        # Color packets (first 200)
        count = 0
        for w in color_writes[:200]:
            if len(w) == 5:
                # Frame header
                send_write(ser, w, delay=0.005)
            elif len(w) >= 238:
                # Color payload
                send_write(ser, w, delay=0.033)
                count += 1
            else:
                send_write(ser, w, delay=0.01)

        print(f"  Sent {count} color packets")
        time.sleep(3.0)

    finally:
        ser.close()
    time.sleep(1.0)

    # ================================================================
    # PHASE 2: Capture handshake + .NET GenRGBTransferPackage (bright-fixed)
    # ================================================================
    print()
    print("--- PHASE 2: Capture handshake + .NET RED (brightness=0xFF) ---")

    try:
        from soulight.protocol.bridge import BeelightBridge
        bridge = BeelightBridge()
        if not bridge.init():
            print("[FAIL] Bridge init failed")
            return

        ser = open_port()
        try:
            # Same handshake
            for w in handshake_core:
                delay = 0.05 if len(w) == 5 else 0.1
                send_write(ser, w, delay=delay)
            time.sleep(0.5)
            resp = ser.read(4096)
            if resp:
                print(f"  Response: {len(resp)} bytes")

            # Send .NET RED packets with brightness fix
            sent = 0
            for _ in range(100):
                pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
                if pkt is None:
                    break

                frame_hdr = pkt[:5]
                payload = bytearray(pkt[5:])

                # Fix brightness: XOR brightness byte with 0xFF
                brightness_pos = len(payload) - 230
                payload[brightness_pos] ^= 0xFF

                send_write(ser, frame_hdr, delay=0.005)
                send_write(ser, bytes(payload), delay=0.033)
                sent += 1

            print(f"  Sent {sent} .NET RED packets (brightness=0xFF)")
            time.sleep(3.0)

        finally:
            ser.close()
        time.sleep(1.0)

    except Exception as e:
        print(f"  Error: {e}")

    # ================================================================
    # PHASE 3: Historical handshake + capture color packets
    # ================================================================
    print()
    print("--- PHASE 3: Historical handshake + capture color packets ---")

    try:
        from soulight.protocol.raw_git_replay import RawGitReplayProtocol
        repo_path = os.path.dirname(os.path.abspath(__file__))
        protocol = RawGitReplayProtocol(repo_path=repo_path)

        ser = open_port()
        try:
            hs_count = 0
            for w in protocol.iter_handshake_writes():
                ser.write(w)
                ser.flush()
                hs_count += 1
                if len(w) >= 238:
                    time.sleep(0.033)
                elif len(w) == 5:
                    time.sleep(0.005)
                else:
                    time.sleep(0.01)
            print(f"  Historical handshake: {hs_count} writes")
            time.sleep(0.5)

            # Then capture color packets
            count = 0
            for w in color_writes[:200]:
                if len(w) == 5:
                    send_write(ser, w, delay=0.005)
                elif len(w) >= 238:
                    send_write(ser, w, delay=0.033)
                    count += 1
                else:
                    send_write(ser, w, delay=0.01)

            print(f"  Sent {count} capture color packets")
            time.sleep(3.0)
        finally:
            ser.close()

    except Exception as e:
        print(f"  Error: {e}")

    print()
    print("[Test] Done. Report what you saw:")
    print("  Phase 1 (capture hs + capture color):   ___")
    print("  Phase 2 (capture hs + .NET RED fixed):   ___")
    print("  Phase 3 (hist hs + capture color):       ___")


if __name__ == "__main__":
    main()
