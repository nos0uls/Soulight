# test_raw_replay_from_git.py — Raw replay через green.csv из git history
#
# Этот скрипт не требует восстанавливать большой green.csv в рабочее дерево.
# Вместо этого он читает удалённый файл прямо из указанного commit через git show,
# парсит WRITE-операции и может отправить их в контроллер как есть.
#
# Это полезно как первый шаг для true raw path:
# 1) проверяем, что исторический replay вообще снова светит
# 2) потом добавляем selective XOR modification поверх тех же packet

import os
import subprocess
import sys
import time

try:
    import serial
except ImportError:
    print("pip install pyserial")
    sys.exit(1)

# region ===== Настройки =====
REPO_PATH = os.path.dirname(os.path.abspath(__file__))
SOURCE_REVISION = "4f8511e68545bc071b36a32f4a48f0de181947a9^"
SOURCE_FILE = "green.csv"
COM_PORT = "COM7"
BAUD = 500000
# endregion


# region ===== Загрузка CSV из git history =====
def load_deleted_file_from_git(repo_path, revision, file_path):
    # Этот helper читает содержимое удалённого файла прямо из commit history.
    # Так нам не нужно физически восстанавливать большой CSV в проект.
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


# Этот parser извлекает только WRITE-пакеты из capture CSV.
# Нам нужны именно реальные байты, которые уходили в COM-порт.
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
# endregion


# region ===== Replay =====
def replay_writes(writes):
    # Этот replay повторяет исторические пакеты с теми же задержками по классам длины.
    # Логика взята из старых raw-скриптов: длинные пакеты идут как кадры,
    # heartbeat — быстрее.
    ser = serial.Serial(COM_PORT, BAUD, timeout=0.5, write_timeout=0.5)
    ser.dtr = True
    ser.rts = True
    time.sleep(0.3)
    ser.read(4096)

    try:
        for packet in writes:
            ser.write(packet)
            ser.flush()

            if len(packet) >= 238:
                time.sleep(0.033)
            elif len(packet) == 5:
                time.sleep(0.005)
            else:
                time.sleep(0.05)
    finally:
        ser.close()
# endregion


# region ===== Main =====
def main():
    print("=" * 60)
    print("RAW REPLAY FROM GIT HISTORY")
    print("=" * 60)
    print()
    print(f"Repo: {REPO_PATH}")
    print(f"Source revision: {SOURCE_REVISION}")
    print(f"Source file: {SOURCE_FILE}")
    print()

    csv_text = load_deleted_file_from_git(REPO_PATH, SOURCE_REVISION, SOURCE_FILE)
    writes = parse_writes_from_csv_text(csv_text)

    count_5 = sum(1 for w in writes if len(w) == 5)
    count_238 = sum(1 for w in writes if len(w) == 238)
    count_239_245 = sum(1 for w in writes if 239 <= len(w) <= 245)

    print(f"[Replay] total writes = {len(writes)}")
    print(f"[Replay] 5-byte packets = {count_5}")
    print(f"[Replay] 238-byte packets = {count_238}")
    print(f"[Replay] 239-245 packets = {count_239_245}")
    print()

    if not writes:
        raise RuntimeError("No write packets parsed from git history CSV")

    print("[Replay] Opening COM and replaying packets...")
    replay_writes(writes)
    print("[Replay] Done")
# endregion


if __name__ == "__main__":
    main()
