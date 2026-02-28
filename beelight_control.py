# -*- coding: utf-8 -*-
"""
beelight_control.py - Управление лентой Beelight через DLL.

Протокол (выяснено реверс-инжинирингом):
- DLL генерирует MgL-пакеты (4d674c 00 plen payload...)
- Контроллер принимает wire-формат: 55 AA 5A plen 00 payload
- MgL payload == wire payload (одно и то же!)
- Для начала сессии нужен connect + ответ контроллера

Run: python beelight_control.py (32-bit Python!)
"""
import sys, os, ctypes, struct, time

# Пути к DLL и COM-порту
CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll")
COM_PORT = "COM7"
BAUD = 500000

try:
    import serial
except ImportError:
    print("pip install pyserial"); sys.exit(1)


# ================================================================
# Вспомогательные функции работы с DLL
# ================================================================

def dll_call(dll, fn_name, *args):
    """Вызывает функцию DLL, возвращает MgL-пакет (bytes)."""
    buf = ctypes.create_string_buffer(512)
    getattr(dll, fn_name)(*args, buf)
    raw = buf.raw
    end = max((i for i in range(len(raw)) if raw[i]), default=0) + 1
    return raw[:end]


def mgl_to_wire(mgl_data):
    """Конвертирует MgL пакет (4d674c 00 plen payload) в wire (55AA5A plen 00 payload)."""
    if mgl_data[:3] != b'MgL':
        return None
    plen = mgl_data[4]
    payload = mgl_data[5:5 + plen]
    return bytes([0x55, 0xAA, 0x5A, plen, 0x00]) + payload


def send_wire(ser, wire_pkt):
    """Отправляет wire-пакет: сначала 5-байтовый заголовок, потом payload."""
    ser.write(wire_pkt[:5])
    ser.flush()
    time.sleep(0.005)
    ser.write(wire_pkt[5:])
    ser.flush()


def send_mgl(ser, mgl_pkt):
    """Отправляет MgL-пакет напрямую (контроллер принимает оба формата)."""
    ser.write(mgl_pkt)
    ser.flush()


# ================================================================
# Главная логика
# ================================================================

def main():
    print("beelight_control.py starting...")

    if struct.calcsize("P") * 8 != 32:
        print("ERROR: Need 32-bit Python!")
        return

    if not os.path.exists(DLL_PATH):
        print("ERROR: DLL not found: " + DLL_PATH)
        return

    dll = ctypes.CDLL(DLL_PATH)
    print("DLL loaded: " + DLL_PATH)

    # Открываем COM-порт с повторными попытками
    ser = None
    for attempt in range(8):
        try:
            ser = serial.Serial(COM_PORT, BAUD, timeout=1.0)
            print("COM" + COM_PORT[-1] + " opened (attempt " + str(attempt + 1) + ")")
            break
        except serial.SerialException as e:
            print("Attempt " + str(attempt + 1) + "/8: " + str(e))
            time.sleep(2.0)

    if ser is None:
        print("Cannot open " + COM_PORT + ". Close Serial Monitor first!")
        return

    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)  # Очищаем буфер

    # ================================================================
    # ТЕСТ 1: Wire-формат connect + scen RED
    # ================================================================
    print("\n--- TEST 1: Wire connect + scen RED ---")

    mgl_conn = dll_call(dll, 'get_connect_package')
    wire_conn = mgl_to_wire(mgl_conn)
    print("Wire connect: " + wire_conn.hex())
    send_wire(ser, wire_conn)
    time.sleep(0.8)
    resp = ser.read(4096)
    print("Response: " + str(len(resp)) + " bytes" + (" -> " + resp[:40].hex() if resp else ""))

    # Отправляем RED 15 секунд
    print("Sending scen RED (wire) for 15s...")
    t0 = time.time()
    n = 0
    while time.time() - t0 < 15.0:
        mgl = dll_call(dll, 'get_scen_package', 0, 100, 255, 0, 0, 1)
        wire = mgl_to_wire(mgl)
        send_wire(ser, wire)
        time.sleep(0.08)
        n += 1
    print("Sent " + str(n) + " RED packets via wire. LED RED? (waiting 3s...)")
    time.sleep(3)

    # ================================================================
    # ТЕСТ 2: MgL-формат напрямую (без конвертации)
    # ================================================================
    print("\n--- TEST 2: Raw MgL connect + scen GREEN ---")

    mgl_conn2 = dll_call(dll, 'get_connect_package')
    print("MgL connect: " + mgl_conn2.hex())
    send_mgl(ser, mgl_conn2)
    time.sleep(0.8)
    resp2 = ser.read(4096)
    print("Response: " + str(len(resp2)) + " bytes" + (" -> " + resp2[:40].hex() if resp2 else ""))

    print("Sending scen GREEN (MgL) for 10s...")
    t0 = time.time()
    n = 0
    while time.time() - t0 < 10.0:
        mgl = dll_call(dll, 'get_scen_package', 0, 100, 0, 255, 0, 1)
        send_mgl(ser, mgl)
        time.sleep(0.08)
        n += 1
    print("Sent " + str(n) + " GREEN packets via MgL. LED GREEN? (waiting 3s...)")
    time.sleep(3)

    # ================================================================
    # ТЕСТ 3: Replay captured handshake + wire scen BLUE
    # ================================================================
    print("\n--- TEST 3: Replayed handshake + wire scen BLUE ---")

    cap_path = os.path.join(CSV_DIR, "surely_full_red.csv")
    if os.path.exists(cap_path):
        hs_packets = []
        with open(cap_path, "r", errors="replace") as f:
            for line in f:
                if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                    continue
                parts = line.split(";")
                if len(parts) <= 5:
                    continue
                try:
                    raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
                    if raw and len(raw) < 50:  # только маленькие (handshake)
                        hs_packets.append(raw)
                    elif len(raw) >= 240:
                        break  # дошли до цветовых данных
                except:
                    continue

        print("Replaying " + str(len(hs_packets)) + " handshake packets...")
        for pkt in hs_packets:
            ser.write(pkt)
            ser.flush()
            time.sleep(0.02)
        time.sleep(0.5)
        resp3 = ser.read(4096)
        print("Handshake response: " + str(len(resp3)) + " bytes")

        print("Sending scen BLUE (wire) for 10s...")
        t0 = time.time()
        n = 0
        while time.time() - t0 < 10.0:
            mgl = dll_call(dll, 'get_scen_package', 0, 100, 0, 0, 255, 1)
            wire = mgl_to_wire(mgl)
            send_wire(ser, wire)
            time.sleep(0.08)
            n += 1
        print("Sent " + str(n) + " BLUE packets. LED BLUE? (waiting 3s...)")
        time.sleep(3)
    else:
        print("surely_full_red.csv not found, skipping test 3")

    # ================================================================
    # ТЕСТ 4: Воспроизвести ОРИГИНАЛЬНЫЕ цветовые пакеты из захвата
    # ================================================================
    print("\n--- TEST 4: Replay ORIGINAL captured RED packets ---")

    cap_path2 = os.path.join(CSV_DIR, "red_full.csv")
    if os.path.exists(cap_path2):
        all_pkts = []
        with open(cap_path2, "r", errors="replace") as f:
            for line in f:
                if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                    continue
                parts = line.split(";")
                if len(parts) <= 5:
                    continue
                try:
                    raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
                    if raw:
                        all_pkts.append(raw)
                except:
                    continue

        print("Total write packets: " + str(len(all_pkts)))
        print("Replaying ALL packets from red_full.csv (handshake + color data)...")

        for i, pkt in enumerate(all_pkts):
            ser.write(pkt)
            ser.flush()
            is_hdr = len(pkt) == 5 and pkt[0:3] == b'\x55\xAA\x5A'
            time.sleep(0.005 if is_hdr else 0.04)

            if i % 50 == 0:
                incoming = ser.read(1024)
                if incoming:
                    print("  [" + str(i) + "] Response: " + str(len(incoming)) + "b")

        print("Replay done. LED RED?")
    else:
        print("red_full.csv not found")

    ser.close()
    print("\nAll tests done!")


if __name__ == "__main__":
    main()
