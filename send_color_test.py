# -*- coding: utf-8 -*-
"""
send_color_test.py - Отправка цветных пакетов с ПРОИЗВОЛЬНЫМ key3
после replay handshake из red_full.csv.

Гипотеза: контроллер НЕ проверяет key3, а просто дешифрует пакет
используя key3 из самого пакета (cipher[2:5]).
Значит мы можем выбрать ЛЮБОЙ key3 и правильно зашифровать plaintext.

Plaintext формат (plen=238, 75 LEDs):
[nonce0, byte1=0x32, 00,00,00,00, 05,05, FF,E3, 00, 4B, R,G,B x75, 00]

Run with 32-bit Python!
"""
import sys, os, struct, time, random

try:
    import serial
except ImportError:
    print("pip install pyserial"); sys.exit(1)

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
COM_PORT = "COM7"
BAUD = 500000


def build_color_packet(r, g, b, key3=None, nonce=None):
    """
    Строит wire-пакет для solid color (75 LEDs, plen=238).
    
    Plaintext: [nonce, 0x32, 00,00,00,00, 05,05, FF,E3, 00, 4B, R,G,B x75, 00]
    Encrypted: plaintext XOR key3 (3-byte, starting from byte 2)
    Wire: 55 AA 5A EE 00 + encrypted_payload
    """
    if key3 is None:
        key3 = [random.randint(0, 255) for _ in range(3)]
    if nonce is None:
        nonce = random.randint(0, 255)

    # Plaintext (238 bytes)
    plain = bytearray(238)
    plain[0] = nonce          # nonce (не шифруется)
    plain[1] = 0x32           # byte1 для plen=238 (не шифруется)
    # bytes[2:6] = 00,00,00,00 (padding/reserved)
    plain[6] = 0x05           # unknown constant
    plain[7] = 0x05           # unknown constant
    plain[8] = 0xFF           # unknown constant (maybe brightness?)
    plain[9] = 0xE3           # unknown constant
    plain[10] = 0x00          # unknown
    plain[11] = 0x4B          # = 75, LED count
    # bytes[12:237] = R,G,B x 75
    for i in range(75):
        plain[12 + i*3] = r
        plain[12 + i*3 + 1] = g
        plain[12 + i*3 + 2] = b
    plain[237] = 0x00          # tail byte

    # Шифруем bytes[2:] XOR key3 (3-byte repeat)
    encrypted = bytearray(238)
    encrypted[0] = plain[0]   # nonce не шифруется
    encrypted[1] = plain[1]   # byte1 не шифруется
    for i in range(2, 238):
        encrypted[i] = plain[i] ^ key3[(i - 2) % 3]

    # Wire header: 55 AA 5A plen=0xEE attr=0x00
    wire = bytes([0x55, 0xAA, 0x5A, 0xEE, 0x00]) + bytes(encrypted)
    return wire


def replay_handshake(ser, csv_path):
    """Воспроизводит handshake из CSV-файла (маленькие пакеты до первого большого)."""
    hs_data = []
    with open(csv_path, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
            except:
                continue
            if not raw:
                continue
            # Прекращаем при достижении большого пакета (>100 байт = цветовые данные)
            if len(raw) > 100:
                break
            hs_data.append(raw)

    print("Replaying " + str(len(hs_data)) + " handshake packets...")
    for pkt in hs_data:
        ser.write(pkt)
        ser.flush()
        time.sleep(0.01)

    time.sleep(0.5)
    resp = ser.read(4096)
    print("Handshake response: " + str(len(resp)) + " bytes")
    return resp


def main():
    print("send_color_test.py")

    # Открываем порт
    ser = None
    for attempt in range(5):
        try:
            ser = serial.Serial(COM_PORT, BAUD, timeout=1.0)
            print("COM7 opened")
            break
        except serial.SerialException as e:
            print("Attempt " + str(attempt+1) + ": " + str(e))
            time.sleep(2)
    if ser is None:
        print("Cannot open COM7")
        return

    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)

    # 1. Replay handshake из red_full.csv
    hs_path = os.path.join(CSV_DIR, "red_full.csv")
    if not os.path.exists(hs_path):
        print("red_full.csv not found!")
        ser.close()
        return

    replay_handshake(ser, hs_path)

    # Ждём пока контроллер обработает handshake
    time.sleep(1.0)
    ser.read(4096)

    # 2. Отправляем КРАСНЫЙ с random key3
    print("\n--- Sending RED (random key3) for 15s ---")
    t0 = time.time()
    n = 0
    while time.time() - t0 < 15.0:
        wire = build_color_packet(0xFF, 0x00, 0x00)
        # Отправляем header + payload раздельно (как в оригинале)
        ser.write(wire[:5])
        ser.flush()
        time.sleep(0.005)
        ser.write(wire[5:])
        ser.flush()
        time.sleep(0.06)
        n += 1
    print("Sent " + str(n) + " RED packets. LED RED?")
    time.sleep(2)

    # 3. ЗЕЛЁНЫЙ
    print("\n--- Sending GREEN for 10s ---")
    t0 = time.time()
    n = 0
    while time.time() - t0 < 10.0:
        wire = build_color_packet(0x00, 0xFF, 0x00)
        ser.write(wire[:5])
        ser.flush()
        time.sleep(0.005)
        ser.write(wire[5:])
        ser.flush()
        time.sleep(0.06)
        n += 1
    print("Sent " + str(n) + " GREEN packets. LED GREEN?")
    time.sleep(2)

    # 4. СИНИЙ
    print("\n--- Sending BLUE for 10s ---")
    t0 = time.time()
    n = 0
    while time.time() - t0 < 10.0:
        wire = build_color_packet(0x00, 0x00, 0xFF)
        ser.write(wire[:5])
        ser.flush()
        time.sleep(0.005)
        ser.write(wire[5:])
        ser.flush()
        time.sleep(0.06)
        n += 1
    print("Sent " + str(n) + " BLUE packets. LED BLUE?")
    time.sleep(2)

    # 5. БЕЛЫЙ
    print("\n--- Sending WHITE for 5s ---")
    t0 = time.time()
    n = 0
    while time.time() - t0 < 5.0:
        wire = build_color_packet(0xFF, 0xFF, 0xFF)
        ser.write(wire[:5])
        ser.flush()
        time.sleep(0.005)
        ser.write(wire[5:])
        ser.flush()
        time.sleep(0.06)
        n += 1
    print("Sent " + str(n) + " WHITE packets. LED WHITE?")

    # 6. OFF
    print("\n--- Sending BLACK (off) for 3s ---")
    t0 = time.time()
    while time.time() - t0 < 3.0:
        wire = build_color_packet(0x00, 0x00, 0x00)
        ser.write(wire[:5])
        ser.flush()
        time.sleep(0.005)
        ser.write(wire[5:])
        ser.flush()
        time.sleep(0.06)

    ser.close()
    print("\nDone! Report colors: RED? GREEN? BLUE? WHITE? OFF?")


if __name__ == "__main__":
    main()
