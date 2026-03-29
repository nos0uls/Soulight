import os
import subprocess
import sys
import time

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

REPO_PATH = os.path.dirname(os.path.abspath(__file__))
SOURCE_REVISION = "4f8511e68545bc071b36a32f4a48f0de181947a9^"
HANDSHAKE_FILE = "surely_full_red.csv"
GREEN_FILE = "green.csv"
COM_PORT = "COM7"
BAUD = 500000


def load_deleted_file_from_git(repo_path, revision, file_path):
    spec = f"{revision}:{file_path}"
    result = subprocess.run(
        ["git", "show", spec],
        cwd=repo_path,
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        check=True,
    )
    return result.stdout


def parse_writes_from_csv_text(text):
    writes = []
    for line in text.splitlines():
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


def is_color_packet(raw):
    return 239 <= len(raw) <= 245


def send_handshake(ser, handshake_writes):
    for raw in handshake_writes:
        ser.write(raw)
        ser.flush()
        time.sleep(0.005)
    time.sleep(0.2)


def send_color_sequence(ser, writes, modifier=None, limit=50):
    sent = 0
    for raw in writes[:limit]:
        if is_color_packet(raw):
            if modifier is not None:
                raw = modifier(raw)
            ser.write(raw)
            ser.flush()
            time.sleep(0.033)
            sent += 1
        elif len(raw) == 5:
            ser.write(raw)
            ser.flush()
            time.sleep(0.005)
        else:
            ser.write(raw)
            ser.flush()
            time.sleep(0.01)
    return sent


def set_color_from_green(raw, r, g, b):
    m = bytearray(raw)
    delta_r = r
    delta_g = g ^ 255
    delta_b = b
    color_start = len(m) - 225
    num_leds = 75

    for led in range(num_leds):
        base = color_start + led * 3
        if base + 2 < len(m):
            m[base + 0] ^= delta_r
            m[base + 1] ^= delta_g
            m[base + 2] ^= delta_b

    return bytes(m)


def open_port():
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    return ser


def main():
    print("=" * 60)
    print("RAW HANDSHAKE + COLOR FROM GIT HISTORY")
    print("=" * 60)
    print()

    handshake_csv = load_deleted_file_from_git(REPO_PATH, SOURCE_REVISION, HANDSHAKE_FILE)
    green_csv = load_deleted_file_from_git(REPO_PATH, SOURCE_REVISION, GREEN_FILE)

    red_writes = parse_writes_from_csv_text(handshake_csv)
    green_writes = parse_writes_from_csv_text(green_csv)
    handshake_writes = red_writes[:114]

    print(f"[Test] Handshake writes = {len(handshake_writes)}")
    print(f"[Test] Green writes = {len(green_writes)}")
    print(f"[Test] Green color packets = {sum(1 for w in green_writes if is_color_packet(w))}")
    print()

    print("--- PHASE 1: Handshake + GREEN original ---")
    ser = open_port()
    try:
        send_handshake(ser, handshake_writes)
        sent = send_color_sequence(ser, green_writes, modifier=None, limit=50)
        print(f"[Test] GREEN packets sent = {sent}")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(0.5)

    print("--- PHASE 2: Handshake + RED from GREEN capture ---")
    ser = open_port()
    try:
        send_handshake(ser, handshake_writes)
        sent = send_color_sequence(
            ser,
            green_writes,
            modifier=lambda w: set_color_from_green(w, 255, 0, 0),
            limit=50,
        )
        print(f"[Test] RED packets sent = {sent}")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(0.5)

    print("--- PHASE 3: Handshake + WHITE from GREEN capture ---")
    ser = open_port()
    try:
        send_handshake(ser, handshake_writes)
        sent = send_color_sequence(
            ser,
            green_writes,
            modifier=lambda w: set_color_from_green(w, 255, 255, 255),
            limit=50,
        )
        print(f"[Test] WHITE packets sent = {sent}")
        time.sleep(2.0)
    finally:
        ser.close()

    print("[Test] Done")


if __name__ == "__main__":
    main()
