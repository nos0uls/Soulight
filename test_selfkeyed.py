#!/usr/bin/env python3
"""
Тест: контроллер Beelight self-keyed?
Отправляем пакет RED с ПРОИЗВОЛЬНЫМ key3,
зашифрованным по правильной формуле:
  byte[0:2] = unencrypted (nonce, type)
  byte[2+i] = plain[i] XOR key3[i % 3]

Если лента загорится красным — контроллер self-keyed
и мы можем использовать любой key3!
"""

import serial
import time
import sys

# ============================================================
#region Настройки
PORT = "COM7"
BAUD = 500000
NUM_LEDS = 75   # Количество светодиодов (0x4B)
#endregion

# ============================================================
#region Handshake данные (реплей из red_full.csv)
# Первые ~250 пакетов handshake из рабочего capture
HANDSHAKE_HEX = [
    "55aa5a0c004c390a3a60ec0005c8710a3b",
    "55aa5a0e00213b9db8b6ffcd2a4cde92d29dba",
    "55aa5a13001e3b9db8b6ffcd2a4cde92d29dbdb700cc2a4d",
    "55aa5a0d00c33898d5d8bdc6fee194e298d6",
    "55aa5a0c00cb398c263346d7394b5b8c25",
    "55aa5a0a00873759f6410c1d4959f5",
    "55aa5a0e00d43be1861e25222ca936f864e185",
    "55aa5a070092326be1c76be2",
    "55aa5a0d00fd38250e533c31ee91b968240e",
    "55aa5a0a000d3771d280db768070d2",
    "55aa5a0a000d3771d280db768070d2",
    "55aa5a0a000d3771d280db768070d2",
]
#endregion

# ============================================================
#region Функция построения wire пакета
def build_wire_packet(payload_bytes):
    """Оборачиваем payload в wire формат: 55 AA 5A [plen] 00 [payload]"""
    plen = len(payload_bytes)
    header = bytes([0x55, 0xAA, 0x5A, plen, 0x00])
    return header + bytes(payload_bytes)

def build_red_packet(nonce, key3):
    """
    Строим пакет RED solid color с заданным nonce и key3.
    
    Plaintext (236 bytes encrypted + 2 bytes unencrypted header):
      byte[0] = nonce (не зашифрован)
      byte[1] = 0x32 (тип data, не зашифрован)
      byte[2:] = зашифрованные данные
    
    Plaintext данных (236 bytes):
      [00, FF, 00, 00, FA, 05, FF, 1C, 00, 4B, (FF,00,00)x75, FF]
    """
    # Формируем plaintext данных (то что шифруется)
    plain_data = [
        0x00, 0xFF, 0x00, 0x00,  # header bytes
        0xFA, 0x05,              # mode/submode  
        0xFF, 0x1C,              # brightness? flags?
        0x00, 0x4B,              # 0x4B = 75 LEDs
    ]
    # 75 пикселей красного (R=FF, G=00, B=00)
    for _ in range(NUM_LEDS):
        plain_data.extend([0xFF, 0x00, 0x00])
    # Завершающий байт
    plain_data.append(0xFF)
    
    # Шифруем: cipher[i] = plain_data[i] XOR key3[i % 3]
    cipher_data = [plain_data[i] ^ key3[i % 3] for i in range(len(plain_data))]
    
    # Собираем payload: [nonce, type, cipher_data...]
    payload = [nonce, 0x32] + cipher_data
    
    return build_wire_packet(payload)

def build_keepalive():
    """Heartbeat пакет (не зашифрован)"""
    return bytes([0x55, 0xAA, 0x5A, 0xF0, 0x00])
#endregion

# ============================================================
#region Основной тест
def main():
    print("=== Beelight Self-Keyed Test ===")
    print(f"Port: {PORT}, Baud: {BAUD}")
    
    ser = serial.Serial(PORT, BAUD, timeout=1)
    time.sleep(0.5)
    
    # Шаг 1: Отправляем handshake (реплей)
    print("\n[1] Sending handshake...")
    for i, hex_str in enumerate(HANDSHAKE_HEX):
        pkt = bytes.fromhex(hex_str)
        ser.write(pkt)
        time.sleep(0.05)
        # Читаем ответ
        resp = ser.read(ser.in_waiting or 1)
        if resp:
            print(f"  [{i}] Sent {len(pkt)}b, Response: {len(resp)}b")
        else:
            print(f"  [{i}] Sent {len(pkt)}b")
    
    print("\nWaiting 2s after handshake...")
    time.sleep(2)
    
    # Шаг 2: Отправляем RED пакеты с ПРОИЗВОЛЬНЫМ key3
    key3 = [0xAA, 0xBB, 0xCC]  # Произвольный ключ
    print(f"\n[2] Sending RED packets with arbitrary key3={[hex(b) for b in key3]}")
    
    for i in range(100):
        nonce = i & 0xFF
        pkt = build_red_packet(nonce, key3)
        ser.write(pkt)
        
        # Keepalive каждые 10 пакетов
        if i % 10 == 9:
            ser.write(build_keepalive())
        
        time.sleep(0.03)
        
        if i < 5 or i % 20 == 0:
            print(f"  Sent packet {i}, nonce={hex(nonce)}, len={len(pkt)}")
    
    print("\n[3] Holding for 5 seconds (sending keepalive)...")
    for _ in range(50):
        ser.write(build_keepalive())
        time.sleep(0.1)
    
    # Шаг 3: Отключение
    print("\n[4] Done. Close serial.")
    ser.close()
    print("Check LED strip - did it turn RED?")

if __name__ == "__main__":
    main()
#endregion
