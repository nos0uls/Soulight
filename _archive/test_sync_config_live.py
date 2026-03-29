# test_sync_config_live.py — Живой тест SyncConfig/Ack через COM7
#
# Скрипт:
# 1) Загружает LProtocol из Beelight.exe
# 2) Вешает SyncConfigAck callback
# 3) Открывает COM7
# 4) Делает базовый handshake
# 5) Отправляет SyncConfigReq
# 6) Читает ответ контроллера и кормит его в ProtocolReciever
#
# Цель: проверить, получаем ли мы реальный SyncConfigAck,
# чтобы потом встроить это в driver для screen mirroring.

import sys
import time
import serial

sys.path.insert(0, ".")

import clr
from System.Reflection import Assembly, BindingFlags
from System import Array, Byte as NetByte

from soulight.protocol.bridge import BeelightBridge

ASM_PATH = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
PORT = "COM7"
BAUD = 500000


def main():
    print("[Test] Загружаю bridge...")
    bridge = BeelightBridge()
    if not bridge.init():
        print("[Test] ERROR: bridge.init() failed")
        return

    print("[Test] Загружаю assembly...")
    asm = Assembly.LoadFrom(ASM_PATH)
    flags = (
        BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Static
        | BindingFlags.Instance
    )

    lprotocol_type = None
    sync_ack_type = None
    for t in asm.GetTypes():
        if t.Name == "LProtocol":
            lprotocol_type = t
        elif t.Name == "LP_SyncConfigAck":
            sync_ack_type = t

    if lprotocol_type is None or sync_ack_type is None:
        print("[Test] ERROR: LProtocol or LP_SyncConfigAck not found")
        return

    print(f"[Test] LProtocol = {lprotocol_type.FullName}")
    print(f"[Test] LP_SyncConfigAck = {sync_ack_type.FullName}")

    from System import Activator, Int32
    protocol = None
    try:
        protocol = Activator.CreateInstance(lprotocol_type, True)
        print("[Test] LProtocol создан через nonPublic default ctor")
    except Exception as e:
        print(f"[Test] default ctor failed: {e}")

    if protocol is None:
        try:
            protocol = Activator.CreateInstance(lprotocol_type, Int32(0))
            print("[Test] LProtocol создан через Int32 ctor")
        except Exception as e:
            print(f"[Test] Int32 ctor failed: {e}")

    if protocol is None:
        print("[Test] ERROR: не удалось создать LProtocol")
        return

    ack_state = {
        "fired": False,
        "sumPixel": None,
        "channels": None,
        "channelPixel": None,
    }

    # Этот callback должен сработать, если контроллер ответит валидным SyncConfigAck.
    def on_sync_config_ack(o, sum_pixel, channels, channel_pixel):
        ack_state["fired"] = True
        ack_state["sumPixel"] = int(sum_pixel)
        ack_state["channels"] = int(channels)
        ack_state["channelPixel"] = [int(x) for x in channel_pixel]
        print(
            "[Test] SyncConfigAck fired: "
            f"sumPixel={ack_state['sumPixel']}, "
            f"channels={ack_state['channels']}, "
            f"channelPixel={ack_state['channelPixel']}"
        )

    print("[Test] Подписываюсь на SyncConfigAck...")
    # В pythonnet Python function обычно можно передать напрямую,
    # а маршалинг в delegate произойдёт автоматически.
    sync_ack_delegate = on_sync_config_ack
    protocol.AddSyncConfigAckListener(sync_ack_delegate)

    print(f"[Test] Открываю {PORT}...")
    ser = serial.Serial(PORT, BAUD, timeout=0.1, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(ser.in_waiting or 1)

    try:
        # Базовый handshake почти как в драйвере.
        hb = bridge.get_heartbeat()
        print("[Test] Отправляю heartbeat burst...")
        for _ in range(5):
            ser.write(hb)
            time.sleep(0.05)
        time.sleep(0.2)
        ser.read(ser.in_waiting or 1)

        print("[Test] Отправляю switch ON...")
        ser.write(bridge.make_switch_packet(True))
        time.sleep(0.05)

        print("[Test] Отправляю PC mode...")
        ser.write(bridge.make_workmode_pc_packet())
        time.sleep(0.1)

        print("[Test] Готовлю SyncConfigReq через LProtocol...")
        protocol.SendSyncConfigReq()
        queue_count = int(protocol.GetQueueCount())
        print(f"[Test] Queue count after SendSyncConfigReq = {queue_count}")

        req = protocol.GetSendBuffer()
        if req is None:
            print("[Test] ERROR: GetSendBuffer() returned None")
            return

        req_bytes = bytes(req)
        print(f"[Test] SyncConfigReq bytes = {len(req_bytes)}")
        print(f"[Test] SyncConfigReq head = {req_bytes[:24].hex(' ')}")

        print("[Test] Отправляю SyncConfigReq в COM7...")
        ser.write(req_bytes)
        time.sleep(0.1)

        print("[Test] Читаю ответ до 3 секунд...")
        deadline = time.time() + 3.0
        total_read = 0
        chunks = 0
        while time.time() < deadline and not ack_state["fired"]:
            data = ser.read(ser.in_waiting or 1)
            if data:
                chunks += 1
                total_read += len(data)
                print(f"[Test] RX chunk #{chunks}: {len(data)} bytes: {data.hex(' ')}")
                net_arr = Array[NetByte](data)
                protocol.ProtocolReciever(net_arr)
            else:
                time.sleep(0.02)

        print(f"[Test] Read total = {total_read} bytes, chunks = {chunks}")
        print(f"[Test] Ack fired = {ack_state['fired']}")
        if ack_state["fired"]:
            print("[Test] SUCCESS: SyncConfigAck получен")
        else:
            print("[Test] FAIL: SyncConfigAck не получен")
    finally:
        print("[Test] Закрываю порт...")
        try:
            ser.close()
        except Exception:
            pass


if __name__ == "__main__":
    main()
