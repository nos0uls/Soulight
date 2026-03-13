# test_helper_smoke.py — Быстрый smoke test для Soulight.ProtocolHelper.dll
#
# Этот тест проверяет только базовую связку Python -> helper DLL -> Beelight reflection.
# Он не открывает COM-порт и не трогает железо.

import os
import sys

sys.path.insert(0, ".")

import clr

DLL_PATH = os.path.join(
    os.path.dirname(__file__),
    "dotnet",
    "Soulight.ProtocolHelper",
    "bin",
    "Debug",
    "net48",
    "Soulight.ProtocolHelper.dll",
)


def main():
    # Сначала убеждаемся, что DLL реально собрана и лежит там, где мы ожидаем.
    if not os.path.exists(DLL_PATH):
        raise FileNotFoundError(f"Helper DLL not found: {DLL_PATH}")

    print(f"[Smoke] Loading DLL: {DLL_PATH}")
    clr.AddReference(DLL_PATH)

    # Импортируем helper уже после AddReference.
    # Так pythonnet увидит новый namespace и его типы.
    from Soulight.ProtocolHelper import BeelightProtocolHelper

    print("[Smoke] Creating helper instance...")
    helper = BeelightProtocolHelper()

    print("[Smoke] Initializing helper...")
    ok = helper.Initialize()
    print(f"[Smoke] Initialize() = {ok}")

    print("[Smoke] Creating SyncConfig request...")
    packet = helper.CreateSyncConfigRequest()
    packet_bytes = bytes(packet)
    print(f"[Smoke] SyncConfigReq len = {len(packet_bytes)}")
    print(f"[Smoke] SyncConfigReq head = {packet_bytes[:16].hex(' ')}")

    if len(packet_bytes) == 0:
        raise RuntimeError("Helper returned empty SyncConfig request")

    print("[Smoke] HasSyncConfig =", helper.HasSyncConfig)
    print("[Smoke] OK")


if __name__ == "__main__":
    main()
