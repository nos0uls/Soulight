# -*- coding: utf-8 -*-
"""
color_xor_test.py - Тест произвольных цветов через XOR-модификацию RED пакетов.

Метод: Replay RED handshake, затем модифицируем color пакеты через XOR.
ВАЖНО: пакеты отправляются ПОСЛЕДОВАТЕЛЬНО — каждая фаза продолжает
с того места, где остановилась предыдущая, чтобы PRNG контроллера
оставался синхронизированным с захваченными ключами.

COM7 @ 500000 baud. Запускать 32-bit Python!
"""
import sys
import os
import time

sys.stdout.reconfigure(encoding="utf-8")

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

# region ===== Конфигурация =====
COM_PORT = "COM7"
BAUD_RATE = 500000
CSV_PATH = os.path.join(os.path.dirname(os.path.abspath(__file__)), "surely_full_red.csv")
# endregion


# region ===== Парсинг CSV =====
def parse_all_writes(filepath):
    """Извлекает ВСЕ WRITE операции из CSV захвата USB трафика."""
    writes = []
    with open(filepath, "r", errors="replace") as f:
        for ln, line in enumerate(f):
            if "IRP_MJ_WRITE" not in line or "DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 5:
                continue
            try:
                raw = bytes.fromhex(parts[5].strip().replace(" ", ""))
            except ValueError:
                continue
            if raw:
                writes.append(raw)
    return writes
# endregion


# region ===== Модификация color пакетов =====
def modify_color_packet(raw, target_rgb):
    """
    Модифицирует захваченный RED(255,0,0) пакет для нового цвета.

    Математика: cipher_new[i] = cipher_old[i] ^ (plain_old[i] ^ plain_new[i])
    Keystream сокращается — работает для ЛЮБОГО keystream.

    Формат color data (bytes 12-236): [R, B, G] * 75 — НЕ RGB!
    Подтверждено экспериментально: GREEN(0,255,0) при RGB порядке
    давал BLUE на ленте → значит позиции 1 и 2 это B и G (не G и B).
    """
    r_new, g_new, b_new = target_rgb

    # Оригинальный plaintext RED в формате RBG: R=255, B=0, G=0
    # XOR маска для каждой позиции в формате [R, B, G]
    xor_r = 255 ^ r_new   # позиция 0: Red
    xor_b = 0 ^ b_new     # позиция 1: Blue (НЕ green!)
    xor_g = 0 ^ g_new     # позиция 2: Green (НЕ blue!)

    modified = bytearray(raw)
    # Применяем XOR к color data (bytes 12-236, 75 LED * 3 = 225 bytes)
    for i in range(12, 237):
        offset = (i - 12) % 3
        if offset == 0:
            modified[i] ^= xor_r  # R
        elif offset == 1:
            modified[i] ^= xor_b  # B
        else:
            modified[i] ^= xor_g  # G

    return bytes(modified)
# endregion


# region ===== Replay handshake =====
def replay_handshake(ser, writes):
    """
    Replay captured handshake — все writes до первого 238-byte пакета.
    Возвращает индекс первого color-пакета в списке writes.
    """
    handshake_end = len(writes)
    for i, raw in enumerate(writes):
        if len(raw) >= 238:
            # Включаем предшествующий 5-byte header (если есть)
            handshake_end = i - 1 if (i > 0 and len(writes[i - 1]) == 5) else i
            break

    handshake_writes = writes[:handshake_end]
    print("  Handshake: %d writes" % len(handshake_writes))

    for raw in handshake_writes:
        ser.write(raw)
        ser.flush()
        is_hdr = len(raw) == 5 and raw[:3] == b'\x55\xAA\x5A'
        if is_hdr:
            time.sleep(0.005)
        else:
            time.sleep(0.08)
            ser.read(4096)

    time.sleep(0.5)
    ser.read(8192)
    print("  Handshake done!")
    return handshake_end
# endregion


# region ===== Отправка color пакетов (ПОСЛЕДОВАТЕЛЬНАЯ) =====
def send_color_sequential(ser, writes, current_idx, target_rgb, num_frames=6):
    """
    Отправляет num_frames модифицированных color пакетов, начиная с current_idx.

    КРИТИЧНО: продолжает с того места, где остановилась предыдущая фаза!
    Это сохраняет синхронизацию PRNG контроллера с захваченными ключами.

    Возвращает: новый current_idx (для следующей фазы).
    """
    r, g, b = target_rgb
    is_red = (r == 255 and g == 0 and b == 0)
    print("  Sending RGB(%d,%d,%d) — %d frames from idx %d%s" % (
        r, g, b, num_frames, current_idx,
        " (original)" if is_red else " (XOR modified)"))

    sent = 0
    idx = current_idx

    while sent < num_frames and idx < len(writes):
        raw = writes[idx]
        idx += 1

        if len(raw) == 238:
            # Color packet — модифицируем (или оставляем как есть для RED)
            if is_red:
                ser.write(raw)
            else:
                modified = modify_color_packet(raw, target_rgb)
                ser.write(modified)
            ser.flush()
            time.sleep(0.033)  # ~30fps
            sent += 1
        else:
            # Header/heartbeat — отправляем как есть
            ser.write(raw)
            ser.flush()
            time.sleep(0.005)

    print("  Sent %d frames, next_idx=%d" % (sent, idx))
    return idx
# endregion


# region ===== Основной тест =====
def main():
    print("=" * 60)
    print("COLOR XOR TEST v2 - Sequential PRNG-sync'd color test")
    print("=" * 60)

    # Загружаем захват
    writes = parse_all_writes(CSV_PATH)
    total_238 = sum(1 for w in writes if len(w) == 238)
    print("Loaded %d writes (%d color packets) from %s" % (
        len(writes), total_238, os.path.basename(CSV_PATH)))

    # Рассчитываем: 42 пакета / 7 фаз = 6 фреймов на фазу
    frames_per_phase = total_238 // 7
    if frames_per_phase < 3:
        frames_per_phase = 3
    print("Using %d frames per phase (%d phases)" % (frames_per_phase, 7))

    # Открываем порт
    ser = serial.Serial(COM_PORT, BAUD_RATE, timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    print("Port: %s @ %d\n" % (ser.port, ser.baudrate))

    # ФАЗА 1: Handshake
    print("--- PHASE 1: Handshake ---")
    idx = replay_handshake(ser, writes)

    # 42 пакета / 3 фазы = 14 фреймов (~0.5 сек) — хорошо видно!
    # Формат: RBG (не RGB!) — исправлен выше
    tests = [
        ("RED (original, проверка)",  (255, 0, 0)),
        ("GREEN (RBG fix)",           (0, 255, 0)),
        ("BLUE (RBG fix)",            (0, 0, 255)),
    ]

    for phase_num, (name, rgb) in enumerate(tests, start=2):
        print("\n--- PHASE %d: %s ---" % (phase_num, name))
        if idx >= len(writes):
            print("  NO MORE PACKETS! Stopped.")
            break
        idx = send_color_sequential(ser, writes, idx, rgb, num_frames=14)
        time.sleep(0.5)  # пауза 0.5 сек между фазами

    ser.close()
    print("\n" + "=" * 60)
    print("ТЕСТ ЗАВЕРШЕН!")
    print("=" * 60)
    print("\nЧто ты видел? Каждая фаза ~0.5 сек:")
    print("  ФАЗА 2 (RED):   ___ (должен быть КРАСНЫЙ)")
    print("  ФАЗА 3 (GREEN): ___ (должен быть ЗЕЛЁНЫЙ)")
    print("  ФАЗА 4 (BLUE):  ___ (должен быть СИНИЙ)")
# endregion


if __name__ == "__main__":
    main()
