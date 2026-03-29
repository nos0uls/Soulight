# test_helper_sync_rgb_bright_live.py — Live test SyncRGB с brightness + heartbeat
#
# Этот тест проверяет самый практичный сценарий для screen mirroring:
# - базовый handshake
# - SyncConfigReq / SyncConfigAck
# - затем brightness перед каждым SyncRGB
# - heartbeat каждые N пакетов
#
# Цель теста: понять, не нужен ли SyncRGB тот же самый keep-alive pattern,
# который уже требуется для solid color режима.

import os
import sys
import time

import clr
import serial

sys.path.insert(0, ".")

from soulight.protocol.bridge import BeelightBridge

DLL_PATH = os.path.join(
    os.path.dirname(__file__),
    "dotnet",
    "Soulight.ProtocolHelper",
    "bin",
    "Debug",
    "net48",
    "Soulight.ProtocolHelper.dll",
)
PORT = "COM7"
BAUD = 500000


def load_helper():
    if not os.path.exists(DLL_PATH):
        raise FileNotFoundError(f"Helper DLL not found: {DLL_PATH}")

    clr.AddReference(DLL_PATH)
    from Soulight.ProtocolHelper import BeelightProtocolHelper

    helper = BeelightProtocolHelper()
    helper.Initialize()
    return helper


# Делаем заметный 4-сегментный паттерн по всей длине ленты.
# Если режим заработает, пользователь сразу увидит явные цветовые блоки.
def make_segmented_colors(count):
    colors = []
    for i in range(count):
        t = i / max(1, count)
        if t < 0.25:
            colors.append((255, 0, 0))
        elif t < 0.50:
            colors.append((0, 255, 0))
        elif t < 0.75:
            colors.append((0, 0, 255))
        else:
            colors.append((255, 255, 0))
    return colors


# Плоский RGB buffer для helper CreateSyncRgbPacket().
def flatten_rgb(colors):
    out = bytearray()
    for r, g, b in colors:
        out.append(max(0, min(255, int(r))))
        out.append(max(0, min(255, int(g))))
        out.append(max(0, min(255, int(b))))
    return bytes(out)


# Базовый wake-up, как у текущего driver.
def wake_controller(bridge, ser):
    hb = bridge.get_heartbeat()
    for _ in range(5):
        ser.write(hb)
        time.sleep(0.05)
    time.sleep(0.2)
    ser.read(ser.in_waiting or 1)

    ser.write(bridge.make_switch_packet(True))
    time.sleep(0.05)
    ser.write(bridge.make_workmode_pc_packet())
    time.sleep(0.10)
    ser.read(ser.in_waiting or 1)


# Ждём SyncConfigAck и параллельно кормим helper входящими байтами из COM-порта.
def wait_for_sync_config(helper, ser, timeout=3.0):
    helper.ResetSyncConfigState()
    req = helper.CreateSyncConfigRequest()
    ser.write(bytes(req))

    deadline = time.time() + timeout
    while time.time() < deadline:
        data = ser.read(ser.in_waiting or 1)
        if data:
            helper.FeedReceivedBytes(data)
            if helper.HasSyncConfig:
                return helper.LastSyncConfig
        else:
            time.sleep(0.02)

    raise TimeoutError("SyncConfigAck was not received within timeout")


# Главный цикл теста: brightness + SyncRGB + heartbeat.
# Это максимально похоже на текущий рабочий send loop для solid color.
def main():
    print("[BrightTest] Loading helper...")
    helper = load_helper()

    print("[BrightTest] Loading bridge...")
    bridge = BeelightBridge()
    if not bridge.init():
        raise RuntimeError("bridge.init() failed")

    print(f"[BrightTest] Opening {PORT}...")
    ser = serial.Serial(PORT, BAUD, timeout=0.1, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(ser.in_waiting or 1)

    try:
        print("[BrightTest] Wake controller...")
        wake_controller(bridge, ser)

        print("[BrightTest] Waiting for SyncConfigAck...")
        info = wait_for_sync_config(helper, ser)
        print(
            f"[BrightTest] SyncConfigAck: sumPixel={info.SumPixel}, "
            f"channels={info.Channels}, channelPixel={list(info.ChannelPixel)}"
        )

        colors = make_segmented_colors(info.SumPixel)
        rgb_bytes = flatten_rgb(colors)
        packet = helper.CreateSyncRgbPacket(1, info.SumPixel, rgb_bytes)
        packet_bytes = bytes(packet)
        print(f"[BrightTest] SyncRGB packet len = {len(packet_bytes)}")

        hb = bridge.get_heartbeat()
        count = 0
        start = time.time()
        while time.time() - start < 3.0:
            bright_pkt = bridge.make_bright_packet(255)
            ser.write(bright_pkt)
            time.sleep(0.005)

            ser.write(packet_bytes)
            count += 1

            if count % 10 == 0:
                ser.write(hb)
                time.sleep(0.005)

            # Продолжаем читать входящие байты, чтобы не забивать входной буфер.
            data = ser.read(ser.in_waiting or 1)
            if data:
                helper.FeedReceivedBytes(data)

            time.sleep(0.070)

        print(f"[BrightTest] Sent {count} SyncRGB packets")
        print("[BrightTest] Hold 2s...")
        time.sleep(2.0)
    finally:
        print("[BrightTest] Closing port...")
        ser.close()


if __name__ == "__main__":
    main()
