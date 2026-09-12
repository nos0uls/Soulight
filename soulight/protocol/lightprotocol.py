# lightprotocol.py — Нативная реализация LightProtocol (Lytmi/Beelight LED).
#
# Полностью на Python, без Beelight.exe / .NET / pythonnet.
#
# Формат на проводе — точный, восстановлен из расшифрованного IL
# LProtocolBase.GenFramePackage / GetHash (Beelight.exe, de4dot)
# и проверен на 1491 пакете captures:
#
#   frame = 55 AA 5A <len_lo> <len_hi> + body, len = len(body)
#
#   body = [cksum] [pad^0x31] [key:pad] [attr^key0] [cmd^key1] [data^key...]
#
#   body[i] для i >= 2+pad: plain[i] XOR key[i % pad], где
#   plain = attr(1) + cmd(1) + data
#
#   cksum = sum(body[1:]) & 0xFF — контроллер отбрасывает пакет при
#   несовпадении (именно поэтому «случайный nonce»-модель не работала:
#   первые два байта body — это checksum и pad^0x31, а не nonce).
#
#   pad = Random.Next(3, 11) у оригинала → 3..10, key = Random.NextBytes.
#   Ключ случаен и не связан с данными — связность пакета обеспечивает
#   только checksum.
#
# Команды (data = ctrl + channelMark + len u16le + payload;
# attr=0 REQ, cmd=5 CTRL_DEVICE для всех ctrl-команд;
# раскладки восстановлены из расшифрованного IL LProtocolCtrl.*):
#   00 01                          — запрос device-info (attr=0, cmd=1 FIRM)
#   00 02                          — запрос каналов (attr=0, cmd=2 SYNCSTATUS)
#   00 03                          — запрос возможностей (attr=0, cmd=3 SYNCCONFIG)
#   01 00                          — heartbeat (attr=1, cmd=0 HEARTBEAT)
#   00 05 01 ff 01 00 <on>         — switcher ON/OFF (ctrl=1 SWITCHER)
#   00 05 02 ff 02 00 <u16le>      — аппаратная яркость (ctrl=2 BRIGHT),
#                                    dimmer 0..1000 (1000 в captures = max)
#   00 05 04 ff 03 00 R G B        — статичный цвет (ctrl=4 COLOR)
#   00 05 05 ff e3 00 4b 00 + 225B — RGB transfer (ctrl=5): 75 LED x RGB,
#                                    адресация с конца ленты
#   00 05 06 ff 03 00 <mode> 00 00 — work mode (ctrl=6): 0 = PC
#
# LP_CTRL: SWITCHER=1 BRIGHT=2 TEMPER=3 COLOR=4 RGB_TRANSFER=5 WORKMODE=6
# LP_CMD:  HEARTBEAT=0 FIRM=1 SYNCSTATUS=2 SYNCCONFIG=3 OTA=4
#          CTRL_DEVICE=5 CTRL_SYNC_RGB=6 CTRL_LOG=26
# LP_ATTR: REQ=0 ACK=1
# LP_WK_MODE: PC=0 TUYA=1 NRF=2 KEY=3

import random
from typing import Sequence, Tuple

FRAME_MAGIC = b"\x55\xaa\x5a"
PAD_XOR = 0x31

NUM_LEDS = 75
LED_DATA_BYTES = NUM_LEDS * 3  # 225

# Диапазон pad_len у оригинала: Random.Next(3, 11).
PAD_MIN = 3
PAD_MAX = 10

# Блобы команд: первый байт — attr, второй — cmd, дальше data.
_DATA_QUERY = {1: b"\x00\x01", 2: b"\x00\x02", 3: b"\x00\x03"}
_DATA_HEARTBEAT = b"\x01\x00"          # attr=ACK? нет: attr=1,cmd=0 — heartbeat
_DATA_SYNC_ON = b"\x00\x05\x01\xff\x01\x00\x01"
_DATA_RGB_HEADER = b"\x00\x05\x05\xff\xe3\x00\x4b\x00"  # e3=227=2+225, 4b=75 LED

_rng = random.SystemRandom()


def encrypt_payload(data: bytes,
                    pad_len: int | None = None,
                    key: bytes | None = None) -> bytes:
    """
    Шифрует data (=attr+cmd+payload) в wire-body:
    [cksum][pad^0x31][key][data XOR key].

    pad_len — длина ключа (3..10 у оригинала). key — pad_len случайных байт.
    """
    data = bytes(data)
    if pad_len is None:
        pad_len = _rng.randint(PAD_MIN, PAD_MAX)
    if pad_len < 2:
        raise ValueError("pad_len must be >= 2")
    if key is None:
        key = bytes(_rng.randrange(256) for _ in range(pad_len))
    if len(key) != pad_len:
        raise ValueError("key length must equal pad_len")

    enc = bytes(b ^ key[i % pad_len] for i, b in enumerate(data))
    body = bytearray(2 + pad_len + len(enc))
    body[1] = pad_len ^ PAD_XOR
    body[2:2 + pad_len] = key
    body[2 + pad_len:] = enc
    body[0] = sum(body[1:]) & 0xFF
    return bytes(body)


def decrypt_payload(payload: bytes) -> bytes:
    """
    Дешифрует wire-body → data (=attr+cmd+payload).
    Проверяет checksum; при несовпадении — ValueError.
    """
    if len(payload) < 3:
        raise ValueError("payload too short")
    cksum = payload[0]
    if cksum != (sum(payload[1:]) & 0xFF):
        raise ValueError(f"checksum mismatch: {cksum:02x}")
    pad_len = payload[1] ^ PAD_XOR
    if pad_len < 2 or 2 + pad_len > len(payload):
        raise ValueError("invalid pad_len for payload")
    key = payload[2:2 + pad_len]
    return bytes(b ^ key[i % pad_len] for i, b in enumerate(payload[2 + pad_len:]))


def frame(payload: bytes) -> bytes:
    """Оборачивает body в frame header 55 AA 5A <len u16le>."""
    if not 0 < len(payload) <= 0xFFFF:
        raise ValueError("payload length out of range")
    return FRAME_MAGIC + len(payload).to_bytes(2, "little") + payload


def packet(data: bytes, pad_len: int | None = None) -> bytes:
    """Полный wire-пакет (frame + зашифрованный body) для data-команды."""
    return frame(encrypt_payload(data, pad_len=pad_len))


# === Командные билдеры (возвращают полный wire-пакет) ===

def query_device_info() -> bytes:
    """00 01 — запрос device-info (ответ ~120 байт с ключом устройства)."""
    return packet(_DATA_QUERY[1])


def query_channels() -> bytes:
    """00 02 — запрос конфигурации каналов."""
    return packet(_DATA_QUERY[2])


def query_capabilities() -> bytes:
    """00 03 — запрос возможностей (в ответе 4b=75 LED)."""
    return packet(_DATA_QUERY[3])


def heartbeat() -> bytes:
    """01 00 — heartbeat."""
    return packet(_DATA_HEARTBEAT)


def sync_on() -> bytes:
    """00 05 01 ff 01 00 01 — вход в sync/PC-режим (подтверждено captures)."""
    return packet(_DATA_SYNC_ON)


def brightness(dimmer: int) -> bytes:
    """
    00 05 02 ff 02 00 <u16le> — аппаратная яркость (GenBrightPackage).

    dimmer — 0..1000 (оригинал клампит сверху на 1000; в captures 1000 = max).
    Контроллер умножает этот dimmer на RGB всех входящих режимов.
    """
    d = max(0, min(1000, int(dimmer)))
    return packet(b"\x00\x05\x02\xff\x02\x00" + d.to_bytes(2, "little"))


def color(r: int, g: int, b: int) -> bytes:
    """00 05 04 ff 03 00 R G B — статичный цвет (GenColorPackage)."""
    rgb = bytes(max(0, min(255, int(v))) for v in (r, g, b))
    return packet(b"\x00\x05\x04\xff\x03\x00" + rgb)


def set_work_mode(mode: int) -> bytes:
    """00 05 06 ff 03 00 <mode> 00 00 — режим работы. mode=0 = PC (LP_WK_MODE_PC)."""
    return packet(b"\x00\x05\x06\xff\x03\x00" + bytes([mode & 0xFF, 0, 0]))


def switch(on: bool) -> bytes:
    """00 05 01 ff 01 00 <01|00> — включение/выключение."""
    return packet(b"\x00\x05\x01\xff\x01\x00" + (b"\x01" if on else b"\x00"))


def rgb_transfer(colors_rgb: Sequence[Tuple[int, int, int]]) -> bytes:
    """
    00 05 05 ff e3 00 4b 00 + 225 байт LED — per-LED пакет.

    Список приводится к NUM_LEDS: хвост добивается чёрным, лишнее режется.
    Порядок реверсируется — контроллер адресует ленту с конца.
    """
    colors = [(max(0, min(255, int(r))), max(0, min(255, int(g))), max(0, min(255, int(b))))
              for r, g, b in list(colors_rgb)[:NUM_LEDS]]
    if len(colors) < NUM_LEDS:
        colors.extend([(0, 0, 0)] * (NUM_LEDS - len(colors)))
    colors.reverse()

    led = bytearray(LED_DATA_BYTES)
    for i, (r, g, b) in enumerate(colors):
        base = i * 3
        led[base] = r
        led[base + 1] = g
        led[base + 2] = b
    return packet(_DATA_RGB_HEADER + bytes(led))


def handshake_sequence() -> list[bytes]:
    """
    Стартовая последовательность, повторяющая оригинальное приложение:
    device-info → channels → sync_on → capabilities → heartbeats.
    Ответы устройства можно игнорировать (драйвер и так их вычитывает).
    """
    return [
        query_device_info(),
        query_channels(),
        sync_on(),
        query_capabilities(),
        heartbeat(),
        heartbeat(),
        heartbeat(),
    ]


# === Self-test ===

def _self_test() -> None:
    for data in (_DATA_HEARTBEAT, _DATA_SYNC_ON, b"\x00\x03"):
        for pad in range(2, 12):
            payload = encrypt_payload(data, pad_len=pad)
            assert decrypt_payload(payload) == data
    colors = [(255, 0, 0)] * NUM_LEDS
    wire = rgb_transfer(colors)
    assert wire[:3] == FRAME_MAGIC
    assert wire[5] == (sum(wire[6:]) & 0xFF)
    plain = decrypt_payload(wire[5:])
    assert plain[:8] == _DATA_RGB_HEADER
    assert plain[-LED_DATA_BYTES:] == bytes(led_bytes_from(colors))


def led_bytes_from(colors: Sequence[Tuple[int, int, int]]) -> bytes:
    """Хелпер для тестов: LED-байты в проводном порядке (как в rgb_transfer)."""
    c = list(colors)[:NUM_LEDS]
    if len(c) < NUM_LEDS:
        c += [(0, 0, 0)] * (NUM_LEDS - len(c))
    c.reverse()
    return bytes(v for rgb in c for v in rgb)


if __name__ == "__main__":
    _self_test()
    print("lightprotocol self-test OK")
