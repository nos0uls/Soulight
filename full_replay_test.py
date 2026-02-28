# -*- coding: utf-8 -*-
"""
full_replay_test.py - Полный replay захваченного трафика из surely_full_red.csv.
Воспроизводит ТОЧНЫЕ байты handshake + color data с правильными таймингами.

Цель: проверить, принимает ли контроллер реплей захваченных пакетов.
Если LED загорится красным - протокол работает, можно двигаться дальше.

COM7 @ 500000 baud. Запускать через 32-bit Python!
"""
import sys
import os
import time
import struct

sys.stdout.reconfigure(encoding="utf-8")

# region ===== Проверки =====
try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

COM_PORT = "COM7"
BAUD_RATE = 500000
CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "surely_full_red.csv")
if not os.path.exists(CSV_PATH):
    CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "red_full.csv")
# endregion

# region ===== Парсинг CSV захвата =====
def parse_all_writes(filepath):
    """
    Извлекает ВСЕ WRITE операции из CSV лога Serial Port Monitor.
    Возвращает список (line_number, raw_bytes).
    """
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for ln, line in enumerate(f):
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            data_str = parts[5].strip()
            try:
                raw = bytes.fromhex(data_str.replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append((ln, raw))
    return writes
# endregion

# region ===== Основной тест =====
def main():
    print("=" * 60)
    print("FULL REPLAY TEST")
    print("Воспроизведение захваченного трафика из CSV")
    print("=" * 60)
    print("CSV: %s" % os.path.basename(CSV_PATH))
    print("Port: %s @ %d baud" % (COM_PORT, BAUD_RATE))
    print()

    # Парсим все WRITE операции
    writes = parse_all_writes(CSV_PATH)
    print("Всего WRITE операций: %d" % len(writes))

    # Разделяем handshake и color data
    # Handshake: все пакеты до первого 238-байтового payload
    handshake_end = len(writes)
    for i, (ln, raw) in enumerate(writes):
        if len(raw) >= 238:
            # Включаем предыдущий header (5 байт)
            handshake_end = i - 1 if (i > 0 and len(writes[i-1][1]) == 5) else i
            break

    handshake_writes = writes[:handshake_end]
    color_writes = writes[handshake_end:]

    print("Handshake writes: %d" % len(handshake_writes))
    print("Color writes: %d" % len(color_writes))

    # Показываем handshake
    print("\n--- Handshake sequence ---")
    for i, (ln, raw) in enumerate(handshake_writes[:20]):
        is_hdr = len(raw) == 5 and raw[:3] == b'\x55\xAA\x5A'
        tag = "HDR" if is_hdr else "DAT[%d]" % len(raw)
        print("  [%2d] %s %s" % (i, tag, raw.hex()))

    if len(handshake_writes) > 20:
        print("  ... + %d more" % (len(handshake_writes) - 20))

    # Открываем порт
    print("\n--- Открываем порт ---")
    try:
        ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=0.5)
    except serial.SerialException as e:
        print("ОШИБКА: %s" % e)
        print("Закройте Beelight приложение и Serial Port Monitor!")
        return

    # DTR toggle (как в оригинальном приложении)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)  # очистка буфера
    print("Порт открыт: %s @ %d" % (ser.port, ser.baudrate))

    # ================================================================
    # ФАЗА 1: Replay handshake
    # ================================================================
    print("\n" + "=" * 60)
    print("ФАЗА 1: Replay handshake (%d writes)" % len(handshake_writes))
    print("=" * 60)

    for i, (ln, raw) in enumerate(handshake_writes):
        is_hdr = len(raw) == 5 and raw[:3] == b'\x55\xAA\x5A'

        ser.write(raw)
        ser.flush()

        if is_hdr:
            # Короткая пауза после header
            time.sleep(0.005)
        else:
            # Пауза после data — даём контроллеру время обработать
            time.sleep(0.08)

            # Читаем ответ
            resp = ser.read(4096)
            if resp and i < 30:
                print("  [%2d] Sent DAT[%d], Response: %d bytes: %s" % (
                    i, len(raw), len(resp), resp[:20].hex()))

    # Дочитываем оставшиеся ответы
    time.sleep(0.5)
    remaining = ser.read(8192)
    if remaining:
        print("  Remaining: %d bytes" % len(remaining))

    print("Handshake replayed!")

    # ================================================================
    # ФАЗА 2: Replay captured color packets (RED)
    # ================================================================
    print("\n" + "=" * 60)
    print("ФАЗА 2: Replay captured RED color (%d writes, ~5 sec)" % min(120, len(color_writes)))
    print("=" * 60)

    # Отправляем первые 120 writes (~60 кадров: header + data)
    sent = 0
    for i, (ln, raw) in enumerate(color_writes[:120]):
        ser.write(raw)
        ser.flush()

        if len(raw) >= 238:
            # Пауза после color data
            time.sleep(0.033)  # ~30 fps
            sent += 1
        else:
            # Пауза после header
            time.sleep(0.005)

    print("Отправлено %d color кадров" % sent)
    print(">>> ПРОВЕРЬ ЛЕНТУ - ЗАГОРЕЛАСЬ КРАСНЫМ? <<<")

    # Ждём 3 секунды для наблюдения
    time.sleep(3)

    # ================================================================
    # ФАЗА 3: Продолжаем color packets (если лента загорелась)
    # ================================================================
    print("\n" + "=" * 60)
    print("ФАЗА 3: Продолжение color replay (ещё ~5 sec)")
    print("=" * 60)

    sent2 = 0
    for i, (ln, raw) in enumerate(color_writes[120:300]):
        ser.write(raw)
        ser.flush()

        if len(raw) >= 238:
            time.sleep(0.033)
            sent2 += 1
        else:
            time.sleep(0.005)

    print("Дополнительно отправлено %d кадров" % sent2)

    # Читаем финальные ответы
    time.sleep(0.5)
    final_resp = ser.read(4096)
    if final_resp:
        print("Финальный ответ: %d bytes: %s" % (
            len(final_resp), final_resp[:30].hex()))

    ser.close()
    print("\n" + "=" * 60)
    print("ТЕСТ ЗАВЕРШЕН")
    print("=" * 60)
    print("Результат:")
    print("  Загорелась RED? -> ДА / НЕТ")
    print("  Если ДА - протокол работает, replay принимается")
    print("  Если НЕТ - нужен свежий захват или session-bound протокол")
# endregion


if __name__ == "__main__":
    main()
