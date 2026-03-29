# test_helper_sync_rgb_modes_live.py — Live probe двух вариантов rows/columns для SyncRGB
#
# Этот тест:
# 1) Загружает C# helper DLL
# 2) Делает базовый handshake с контроллером
# 3) Отправляет SyncConfigReq и ждёт SyncConfigAck
# 4) Пробует два варианта rows/columns для SyncRGB
# 5) Между режимами делает паузу, чтобы пользователь увидел разницу на ленте
#
# Варианты:
# - Mode A: rows=1, columns=sumPixel
# - Mode B: rows=channels, columns=max(channelPixel), colors padded до rows*columns

import os
import sys
import time

import serial
import clr

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


# Этот helper создаёт яркий и легко заметный паттерн.
# Цвета идут большими блоками, чтобы пользователь глазами различил режим.
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


# Этот helper переводит RGB-кортежи в плоский byte buffer.
# Такой формат принимает наш C# helper для CreateSyncRgbPacket.
def flatten_rgb(colors):
    out = bytearray()
    for r, g, b in colors:
        out.append(max(0, min(255, int(r))))
        out.append(max(0, min(255, int(g))))
        out.append(max(0, min(255, int(b))))
    return bytes(out)


# Для Mode B нужно сделать прямоугольную матрицу rows*columns.
# Если данных меньше, хвост заполняется чёрным.
def pad_colors(colors, total_count):
    colors = list(colors)
    if len(colors) >= total_count:
        return colors[:total_count]
    return colors + [(0, 0, 0)] * (total_count - len(colors))


# Кормим helper входящими serial-байтами до тех пор,
# пока не получим SyncConfigAck или не выйдет timeout.
def wait_for_sync_config(helper, ser, timeout=3.0):
    helper.ResetSyncConfigState()
    req = helper.CreateSyncConfigRequest()
    ser.write(bytes(req))

    deadline = time.time() + timeout
    total_read = 0
    while time.time() < deadline:
        data = ser.read(ser.in_waiting or 1)
        if data:
            total_read += len(data)
            helper.FeedReceivedBytes(data)
            if helper.HasSyncConfig:
                info = helper.LastSyncConfig
                print(
                    f"[Probe] SyncConfigAck: sumPixel={info.SumPixel}, "
                    f"channels={info.Channels}, channelPixel={list(info.ChannelPixel)}"
                )
                print(f"[Probe] RX total bytes before ack = {total_read}")
                return info
        else:
            time.sleep(0.02)

    raise TimeoutError("SyncConfigAck was not received within timeout")


# Отправляем один и тот же SyncRGB пакет несколько раз.
# Это нужно, чтобы контроллер успел стабильно показать картинку.
def send_packet_burst(ser, packet, repeat=24, interval=0.070):
    data = bytes(packet)
    for _ in range(repeat):
        ser.write(data)
        time.sleep(interval)


# Базовый handshake почти повторяет текущий driver.
# Это нужно, чтобы устройство вошло в рабочий PC mode до SyncConfig/SyncRGB.
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


# Этот main делает два последовательных режима,
# чтобы пользователь мог сравнить визуальный результат на ленте.
def main():
    print("[Probe] Loading helper...")
    helper = load_helper()

    print("[Probe] Loading legacy bridge for base handshake...")
    bridge = BeelightBridge()
    if not bridge.init():
        raise RuntimeError("bridge.init() failed")

    print(f"[Probe] Opening {PORT}...")
    ser = serial.Serial(PORT, BAUD, timeout=0.1, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(ser.in_waiting or 1)

    try:
        print("[Probe] Wake controller...")
        wake_controller(bridge, ser)

        print("[Probe] Waiting for SyncConfigAck...")
        info = wait_for_sync_config(helper, ser)

        base_colors = make_segmented_colors(info.SumPixel)
        mode_a_bytes = flatten_rgb(base_colors)
        mode_a_packet = helper.CreateSyncRgbPacket(1, info.SumPixel, mode_a_bytes)
        print(
            f"[Probe] Mode A ready: rows=1, columns={info.SumPixel}, "
            f"rgb_count={len(base_colors)}, packet_len={len(bytes(mode_a_packet))}"
        )

        mode_b_columns = max(list(info.ChannelPixel) or [info.SumPixel])
        mode_b_total = info.Channels * mode_b_columns
        mode_b_colors = pad_colors(base_colors, mode_b_total)
        mode_b_bytes = flatten_rgb(mode_b_colors)
        mode_b_packet = helper.CreateSyncRgbPacket(info.Channels, mode_b_columns, mode_b_bytes)
        print(
            f"[Probe] Mode B ready: rows={info.Channels}, columns={mode_b_columns}, "
            f"rgb_count={len(mode_b_colors)}, packet_len={len(bytes(mode_b_packet))}"
        )

        print("[Probe] Sending Mode A for ~1.7s...")
        send_packet_burst(ser, mode_a_packet)
        print("[Probe] Pause 1.5s...")
        time.sleep(1.5)

        print("[Probe] Sending Mode B for ~1.7s...")
        send_packet_burst(ser, mode_b_packet)
        print("[Probe] Done. Hold 2s...")
        time.sleep(2.0)
    finally:
        print("[Probe] Closing port...")
        ser.close()


if __name__ == "__main__":
    main()
