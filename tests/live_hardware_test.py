#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
live_hardware_test.py — живой тест нативного протокола на подключённом
контроллере. Без pyserial (raw os.write) и без Beelight.exe.

Использование:
    sudo chmod a+rw /dev/ttyACM0   # или добавить себя в dialout
    python3 tests/live_hardware_test.py [port]

Ожидаемое поведение ленты:
    handshake (устройство отвечает на запросы) -> красный ~1.5s ->
    зелёный ~1.5s -> синий ~1.5s -> белый ~1.5s -> выключение.
"""
import fcntl
import os
import select
import sys
import time

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))

from soulight.protocol import lightprotocol as lp

PORT = sys.argv[1] if len(sys.argv) > 1 else "/dev/ttyACM0"
HOLD_S = 1.5          # сколько держать каждый цвет
SEND_INTERVAL = 0.07  # как у драйвера (~14fps пакетов)

TIOCMGET = 0x5415
TIOCMSET = 0x5418
TIOCM_DTR = 0x002
TIOCM_RTS = 0x004


def set_dtr_rts(fd):
    """Поднимает DTR/RTS — драйвер делает это для пробуждения контроллера."""
    status = fcntl.ioctl(fd, TIOCMGET, b"\x00" * 4)
    flags = int.from_bytes(status, "little") | TIOCM_DTR | TIOCM_RTS
    fcntl.ioctl(fd, TIOCMSET, flags.to_bytes(4, "little"))


def wr(fd, pkt, settle=0.03):
    os.write(fd, pkt)
    time.sleep(settle)


def drain(fd, label="", max_wait=0.4):
    """Читает ответы устройства ограниченное время, печатает hex."""
    out = b""
    deadline = time.time() + max_wait
    while time.time() < deadline and len(out) < 8192:
        r, _, _ = select.select([fd], [], [], 0.05)
        if not r:
            continue
        try:
            chunk = os.read(fd, 4096)
        except OSError:
            break
        if not chunk:
            break
        out += chunk
    if out:
        print(f"  [{label}] <- {out.hex(' ')}")


def hold_color(fd, rgb, label):
    """Держит цвет HOLD_S: RGB-пакеты + heartbeat каждые ~10."""
    hb = lp.heartbeat()
    pkt = lp.rgb_transfer([rgb] * lp.NUM_LEDS)
    end = time.time() + HOLD_S
    n = 0
    while time.time() < end:
        wr(fd, pkt, SEND_INTERVAL)
        n += 1
        if n % 10 == 0:
            wr(fd, hb, 0.01)
    print(f"  {label}: sent {n} color packets")


def main():
    print(f"Opening {PORT} ...")
    fd = os.open(PORT, os.O_RDWR | os.O_NOCTTY)
    set_dtr_rts(fd)
    time.sleep(0.3)
    drain(fd, "post-dtr")

    print("Handshake (queries + sync_on + heartbeats):")
    for i, pkt in enumerate(lp.handshake_sequence()):
        wr(fd, pkt, 0.05)
        drain(fd, f"hs#{i}")

    print("Driver-style handshake (hb burst -> switch ON -> workmode PC):")
    for _ in range(5):
        wr(fd, lp.heartbeat(), 0.05)
    wr(fd, lp.switch(True), 0.05)
    drain(fd, "switch_on")
    wr(fd, lp.set_work_mode(0), 0.05)
    drain(fd, "workmode")

    print("Colors:")
    hold_color(fd, (255, 0, 0), "RED")
    hold_color(fd, (0, 255, 0), "GREEN")
    hold_color(fd, (0, 0, 255), "BLUE")
    hold_color(fd, (255, 255, 255), "WHITE")

    print("Off:")
    hold_color(fd, (0, 0, 0), "BLACK")
    wr(fd, lp.switch(False), 0.05)
    drain(fd, "switch_off")

    os.close(fd)
    print("Done.")


if __name__ == "__main__":
    main()
