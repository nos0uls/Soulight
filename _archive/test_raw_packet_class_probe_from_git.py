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


def is_238(raw):
    return len(raw) == 238


def is_239_245(raw):
    return 239 <= len(raw) <= 245


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


def send_filtered_sequence(ser, writes, allow_238, allow_239_245, limit=80):
    sent_238 = 0
    sent_239_245 = 0
    sent_5 = 0

    for raw in writes[:limit]:
        if len(raw) == 5:
            ser.write(raw)
            ser.flush()
            sent_5 += 1
            time.sleep(0.005)
            continue

        if is_238(raw):
            if not allow_238:
                continue
            ser.write(raw)
            ser.flush()
            sent_238 += 1
            time.sleep(0.033)
            continue

        if is_239_245(raw):
            if not allow_239_245:
                continue
            ser.write(raw)
            ser.flush()
            sent_239_245 += 1
            time.sleep(0.033)
            continue

        ser.write(raw)
        ser.flush()
        time.sleep(0.01)

    return sent_5, sent_238, sent_239_245


def run_phase(label, handshake_writes, green_writes, allow_238, allow_239_245):
    print(f"--- {label} ---")
    ser = open_port()
    try:
        send_handshake(ser, handshake_writes)
        sent_5, sent_238, sent_239_245 = send_filtered_sequence(
            ser,
            green_writes,
            allow_238=allow_238,
            allow_239_245=allow_239_245,
            limit=80,
        )
        print(f"[Probe] sent 5-byte = {sent_5}")
        print(f"[Probe] sent 238 = {sent_238}")
        print(f"[Probe] sent 239-245 = {sent_239_245}")
        time.sleep(2.0)
    finally:
        ser.close()
    time.sleep(0.5)


def main():
    print("=" * 60)
    print("RAW PACKET CLASS PROBE FROM GIT HISTORY")
    print("=" * 60)
    print()

    handshake_csv = load_deleted_file_from_git(REPO_PATH, SOURCE_REVISION, HANDSHAKE_FILE)
    green_csv = load_deleted_file_from_git(REPO_PATH, SOURCE_REVISION, GREEN_FILE)

    handshake_writes = parse_writes_from_csv_text(handshake_csv)[:114]
    green_writes = parse_writes_from_csv_text(green_csv)

    print(f"[Probe] Handshake writes = {len(handshake_writes)}")
    print(f"[Probe] Green writes = {len(green_writes)}")
    print(f"[Probe] Green 238 = {sum(1 for w in green_writes if is_238(w))}")
    print(f"[Probe] Green 239-245 = {sum(1 for w in green_writes if is_239_245(w))}")
    print()

    run_phase(
        "CONTROL: original mixed replay",
        handshake_writes,
        green_writes,
        allow_238=True,
        allow_239_245=True,
    )

    run_phase(
        "ONLY 238 payloads (+ 5-byte writes)",
        handshake_writes,
        green_writes,
        allow_238=True,
        allow_239_245=False,
    )

    run_phase(
        "ONLY 239-245 payloads (+ 5-byte writes)",
        handshake_writes,
        green_writes,
        allow_238=False,
        allow_239_245=True,
    )

    print("[Probe] Done")


if __name__ == "__main__":
    main()
