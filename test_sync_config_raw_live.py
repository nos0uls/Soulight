# test_sync_config_raw_live.py — Живой raw тест SyncConfigReq/RX через COM7
#
# Этот скрипт не пытается подписываться на .NET delegate.
# Его задача — отправить SyncConfigReq и снять сырые байты ответа,
# чтобы понять реальный ack пакет контроллера.

import sys
import time
import serial

sys.path.insert(0, ".")

import clr
from System.Reflection import Assembly, BindingFlags
from System import Activator, Int32

from soulight.protocol.bridge import BeelightBridge

ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
PORT = "COM7"
BAUD = 500000


def make_lprotocol(asm):
    lprotocol_type = None
    for t in asm.GetTypes():
        if t.Name == "LProtocol":
            lprotocol_type = t
            break

    if lprotocol_type is None:
        raise RuntimeError("LProtocol not found")

    try:
        return Activator.CreateInstance(lprotocol_type, Int32(0))
    except Exception as e:
        raise RuntimeError(f"Failed to create LProtocol(Int32): {e}")


def make_sync_config(asm, protocol):
    sync_type = None
    for t in asm.GetTypes():
        if t.Name == "LProtocolSyncConfig":
            sync_type = t
            break

    if sync_type is None:
        raise RuntimeError("LProtocolSyncConfig not found")

    try:
        return Activator.CreateInstance(sync_type, protocol)
    except Exception as e:
        raise RuntimeError(f"Failed to create LProtocolSyncConfig(protocol): {e}")


def main():
    print("[RawTest] Init bridge...")
    bridge = BeelightBridge()
    if not bridge.init():
        print("[RawTest] ERROR: bridge.init() failed")
        return

    asm = Assembly.LoadFrom(ASM_PATH)
    protocol = make_lprotocol(asm)
    sync_config = make_sync_config(asm, protocol)

    print(f"[RawTest] Open {PORT}...")
    ser = serial.Serial(PORT, BAUD, timeout=0.1, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(ser.in_waiting or 1)

    try:
        hb = bridge.get_heartbeat()
        print("[RawTest] Heartbeat burst...")
        for _ in range(5):
            ser.write(hb)
            time.sleep(0.05)
        time.sleep(0.2)
        ser.read(ser.in_waiting or 1)

        print("[RawTest] Switch ON...")
        ser.write(bridge.make_switch_packet(True))
        time.sleep(0.05)

        print("[RawTest] PC mode...")
        ser.write(bridge.make_workmode_pc_packet())
        time.sleep(0.1)
        ser.read(ser.in_waiting or 1)

        print("[RawTest] Generate SyncConfigReq via LProtocolSyncConfig...")
        req = sync_config.GenSyncConfigPackage()
        if req is None:
            print("[RawTest] ERROR: GenSyncConfigPackage() returned None")
            return

        req_bytes = bytes(req)
        print(f"[RawTest] TX SyncConfigReq len={len(req_bytes)}")
        print(f"[RawTest] TX head={req_bytes[:32].hex(' ')}")

        ser.write(req_bytes)
        time.sleep(0.05)

        print("[RawTest] Reading RX for 3 seconds...")
        deadline = time.time() + 3.0
        total = 0
        chunks = []
        while time.time() < deadline:
            data = ser.read(ser.in_waiting or 1)
            if data:
                total += len(data)
                chunks.append(data)
                print(f"[RawTest] RX chunk len={len(data)} : {data.hex(' ')}")
            else:
                time.sleep(0.02)

        print(f"[RawTest] RX total bytes = {total}")
        print(f"[RawTest] RX chunks = {len(chunks)}")

        if chunks:
            merged = b"".join(chunks)
            print(f"[RawTest] RX merged = {merged.hex(' ')}")
        else:
            print("[RawTest] RX empty")
    finally:
        print("[RawTest] Close port...")
        try:
            ser.close()
        except Exception:
            pass


if __name__ == "__main__":
    main()
