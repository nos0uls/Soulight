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
NUM_LEDS = 75
COLOR_START_238 = 13
COLOR_BYTES = NUM_LEDS * 3


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


def is_238(raw):
    return len(raw) == 238


def open_port():
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)
    return ser


def send_handshake(ser, handshake_writes):
    for raw in handshake_writes:
        ser.write(raw)
        ser.flush()
        time.sleep(0.005)
    time.sleep(0.2)


def extract_key3_from_238(pkt):
    return bytes([pkt[3], pkt[4], pkt[2]])


def decrypt_238(pkt):
    key3 = extract_key3_from_238(pkt)
    plain = bytes(pkt[i] ^ key3[i % 3] for i in range(len(pkt)))
    return plain, key3


def encrypt_238(plain, key3):
    return bytes(plain[i] ^ key3[i % 3] for i in range(len(plain)))


def set_solid_color_238(pkt, r, g, b):
    plain, key3 = decrypt_238(pkt)
    modified = bytearray(plain)

    for led in range(NUM_LEDS):
        base = COLOR_START_238 + led * 3
        modified[base + 0] = r
        modified[base + 1] = g
        modified[base + 2] = b

    return encrypt_238(bytes(modified), key3)


def send_238_sequence(ser, writes, modifier=None, limit=120):
    sent_5 = 0
    sent_238 = 0

    for raw in writes[:limit]:
        if len(raw) == 5:
            ser.write(raw)
            ser.flush()
            sent_5 += 1
            time.sleep(0.005)
            continue

        if is_238(raw):
            if modifier is not None:
                raw = modifier(raw)
            ser.write(raw)
            ser.flush()
            sent_238 += 1
            time.sleep(0.033)
            continue

    return sent_5, sent_238


def run_phase(label, handshake_writes, green_writes, modifier):
    print(f"--- {label} ---")
    ser = open_port()
    try:
        send_handshake(ser, handshake_writes)
        sent_5, sent_238 = send_238_sequence(ser, green_writes, modifier=modifier, limit=120)
        print(f"[Test] sent 5-byte = {sent_5}")
        print(f"[Test] sent 238 = {sent_238}")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(0.5)


def main():
    print("=" * 60)
    print("RAW 238 COLOR TEST FROM GIT HISTORY")
    print("=" * 60)
    print()

    handshake_csv = load_deleted_file_from_git(REPO_PATH, SOURCE_REVISION, HANDSHAKE_FILE)
    green_csv = load_deleted_file_from_git(REPO_PATH, SOURCE_REVISION, GREEN_FILE)

    handshake_writes = parse_writes_from_csv_text(handshake_csv)[:114]
    green_writes = parse_writes_from_csv_text(green_csv)

    print(f"[Test] Handshake writes = {len(handshake_writes)}")
    print(f"[Test] Green writes = {len(green_writes)}")
    print(f"[Test] Green 238 packets = {sum(1 for w in green_writes if is_238(w))}")
    print()

    run_phase("GREEN original 238-only", handshake_writes, green_writes, modifier=None)
    run_phase("RED 238-only", handshake_writes, green_writes, modifier=lambda raw: set_solid_color_238(raw, 255, 0, 0))
    run_phase("WHITE 238-only", handshake_writes, green_writes, modifier=lambda raw: set_solid_color_238(raw, 255, 255, 255))

    print("[Test] Done")


if __name__ == "__main__":
    main()
