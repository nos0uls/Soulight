import ctypes
import serial
import time
import os
import sys

# Путь к 32-битной библиотеке
DLL_PATH = r'c:\Users\n0souls\Documents\GitHub\Soulight\Новая папка\Dumps\beelightLib.dll'
if not os.path.exists(DLL_PATH):
    print("Ошибка: beelightLib.dll не найдена!")
    sys.exit(1)

# Загружаем библиотеку
lib = ctypes.CDLL(DLL_PATH)

def get_payload(func, *args):
    """Вызывает функцию из DLL, извлекает результат и убирает заголовок MgL\x00"""
    buf = ctypes.create_string_buffer(2048)
    res = func(*args, buf)
    raw = buf.raw
    
    # Ищем конец данных
    last_nz = 0
    for j in range(len(raw) - 1, -1, -1):
        if raw[j] != 0:
            last_nz = j
            break
    
    pkt = raw[:last_nz + 1]
    
    # Убираем служебный заголовок MgL\x00 (4D 67 4C 00)
    if pkt.startswith(b'MgL\x00'):
        return pkt[4:]
    return pkt

def wrap_packet(payload):
    """Оборачивает зашифрованный payload в протокол 55 AA 5A"""
    plen = len(payload)
    return bytes([0x55, 0xAA, 0x5A, plen, 0x00]) + payload

def main():
    try:
        # Открываем COM7. Если у вас другой порт, измените его здесь.
        port = serial.Serial('COM7', 115200, timeout=0.5)
        print("Подключено к", port.portstr)
        
        # 1. Отправляем Handshake (Connect)
        # Это синхронизирует PRNG в DLL с устройством
        print("\n--- Этап 1: Handshake ---")
        conn_payload = get_payload(lib.get_connect_package)
        pkt_conn = wrap_packet(conn_payload)
        print("Отправляем:", pkt_conn.hex(' '))
        port.write(pkt_conn)
        
        # Ждем и читаем ответ от контроллера
        resp = port.read(100)
        print("Ответ:", resp.hex(' '))
        
        # В оригинале ответ передается в unpack_package для обновления ключей?
        # Попробуем без этого, просто сгенерировать следующий пакет
        
        time.sleep(0.5)

        # 2. Отправляем Зеленый цвет
        print("\n--- Этап 2: Установка Зеленого цвета ---")
        # get_scen_package(scenid, dimmer, R, G, B, power, pout)
        scen_payload = get_payload(lib.get_scen_package, 0, 100, 0, 255, 0, 1)
        pkt_scen = wrap_packet(scen_payload)
        print("Отправляем:", pkt_scen.hex(' '))
        port.write(pkt_scen)
        resp = port.read(100)
        print("Ответ:", resp.hex(' '))
        
        time.sleep(1)

        # 3. Отправляем Синий цвет
        print("\n--- Этап 3: Установка Синего цвета ---")
        scen_payload2 = get_payload(lib.get_scen_package, 0, 100, 0, 0, 255, 1)
        pkt_scen2 = wrap_packet(scen_payload2)
        print("Отправляем:", pkt_scen2.hex(' '))
        port.write(pkt_scen2)
        resp = port.read(100)
        print("Ответ:", resp.hex(' '))
        
        port.close()
        print("\nУспешно завершено!")
        
    except serial.SerialException as e:
        print(f"\nОшибка COM-порта: {e}")
        print("Убедитесь, что оригинальное приложение Beelight полностью закрыто (проверьте системный трей).")
    except Exception as e:
        print(f"\nОшибка: {e}")

if __name__ == '__main__':
    main()
