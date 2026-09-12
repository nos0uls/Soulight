# -*- coding: utf-8 -*-
"""
test_native_protocol.py — Тесты нативного кодека lightprotocol и NativeBridge.

Формат подтверждён расшифрованным IL LProtocolBase.GenFramePackage
(Beelight.exe) и checksum-валидацией 1491 пакета captures:

  body = [cksum][pad^0x31][key:pad][attr^key0][cmd^key1][data^key]
  cksum = sum(body[1:]) & 0xFF

Проверяем оффлайн (без железа):
- структура frame header (55 AA 5A len u16le)
- checksum всех captured пакетов
- key-XOR roundtrip при всех pad_len
- размеры RGB-пакетов совпадают с captures
- порядок LED (реверс, адресация с конца ленты)
- plaintext-команды после дешифровки
- NativeBridge: интерфейс drop-in для BeelightBridge
"""
import os
import sys
import unittest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))

from soulight.protocol import lightprotocol as lp
from soulight.protocol.native_bridge import NativeBridge

REPLAY_CSV = os.path.join(os.path.dirname(__file__), "replay.csv")


def parse_capture_writes(path):
    """IRP_MJ_WRITE/DOWN из CSV capture -> список bytes."""
    writes = []
    with open(path, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE" not in line or ";DOWN;" not in line:
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


def pair_frames(writes):
    """Склеивает frame header (55 AA 5A len u16le) со следующим payload write."""
    frames = []
    i = 0
    while i < len(writes):
        w = writes[i]
        if w[:3] == lp.FRAME_MAGIC and len(w) == 5 and i + 1 < len(writes):
            frames.append(writes[i + 1])
            i += 2
        else:
            i += 1
    return frames


class FrameTests(unittest.TestCase):
    def test_frame_header(self):
        wire = lp.frame(b"\x01\x02\x03")
        self.assertEqual(wire[:5], b"\x55\xaa\x5a\x03\x00")
        self.assertEqual(wire[5:], b"\x01\x02\x03")

    def test_frame_16bit_len(self):
        wire = lp.frame(bytes(300))
        self.assertEqual(wire[3:5], (300).to_bytes(2, "little"))


class CryptoTests(unittest.TestCase):
    def test_roundtrip_all_pads(self):
        data = bytes(range(1, 60))
        for pad in range(2, 16):
            payload = lp.encrypt_payload(data, pad_len=pad)
            self.assertEqual(len(payload), 2 + pad + len(data))
            self.assertEqual(payload[1] ^ lp.PAD_XOR, pad)
            self.assertEqual(lp.decrypt_payload(payload), data)

    def test_key_is_cleartext(self):
        key = b"\xde\xad\xbe\xef\x42"
        payload = lp.encrypt_payload(b"\xaa\xbb", pad_len=5, key=key)
        self.assertEqual(payload[2:7], key)

    def test_checksum(self):
        payload = lp.encrypt_payload(b"\x00\x05\x01\xff\x01\x00\x01", pad_len=4,
                                     key=b"\x01\x02\x03\x04")
        self.assertEqual(payload[0], sum(payload[1:]) & 0xFF)

    def test_checksum_rejects_corruption(self):
        payload = bytearray(lp.encrypt_payload(b"\x01\x00", pad_len=3))
        payload[-1] ^= 0xFF  # портим данные — checksum должен сломаться
        with self.assertRaises(ValueError):
            lp.decrypt_payload(bytes(payload))


class RgbTransferTests(unittest.TestCase):
    def test_wire_size_in_capture_range(self):
        for _ in range(50):
            wire = lp.rgb_transfer([(255, 0, 0)] * 75)
            payload_len = wire[3] | wire[4] << 8
            self.assertEqual(len(wire), 5 + payload_len)
            # payload = 2 + pad + 8 + 225, pad 3..10 → 238..245
            self.assertGreaterEqual(payload_len, 235 + lp.PAD_MIN)
            self.assertLessEqual(payload_len, 235 + lp.PAD_MAX)

    def test_decrypted_structure(self):
        colors = [(i % 256, (i * 2) % 256, (i * 3) % 256) for i in range(75)]
        wire = lp.rgb_transfer(colors)
        data = lp.decrypt_payload(wire[5:])
        self.assertEqual(data[:8], lp._DATA_RGB_HEADER)
        # LED0 (первый в списке) должен быть последним на проводе
        led = data[8:]
        self.assertEqual(led[-3:], bytes(colors[0]))
        self.assertEqual(led[:3], bytes(colors[-1]))

    def test_pads_to_75(self):
        wire = lp.rgb_transfer([(1, 2, 3)])
        data = lp.decrypt_payload(wire[5:])
        self.assertEqual(len(data), 8 + 225)


class CommandBuilderTests(unittest.TestCase):
    def test_heartbeat(self):
        self.assertEqual(lp.decrypt_payload(lp.heartbeat()[5:]), b"\x01\x00")

    def test_sync_on(self):
        self.assertEqual(lp.decrypt_payload(lp.sync_on()[5:]),
                         b"\x00\x05\x01\xff\x01\x00\x01")

    def test_switch_on_off(self):
        for on, tail in ((True, b"\x01"), (False, b"\x00")):
            self.assertEqual(lp.decrypt_payload(lp.switch(on)[5:]),
                             b"\x00\x05\x01\xff\x01\x00" + tail)

    def test_work_mode(self):
        self.assertEqual(lp.decrypt_payload(lp.set_work_mode(0)[5:]),
                         b"\x00\x05\x06\xff\x03" + b"\x00" * 4)

    def test_brightness(self):
        """GenBrightPackage: [2, ch, 2, 0, dimmer u16le], dimmer 0..1000."""
        self.assertEqual(lp.decrypt_payload(lp.brightness(1000)[5:]),
                         b"\x00\x05\x02\xff\x02\x00" + (1000).to_bytes(2, "little"))
        self.assertEqual(lp.decrypt_payload(lp.brightness(0)[5:]),
                         b"\x00\x05\x02\xff\x02\x00\x00\x00")

    def test_brightness_clamps_to_1000(self):
        self.assertEqual(lp.decrypt_payload(lp.brightness(9999)[5:]),
                         b"\x00\x05\x02\xff\x02\x00" + (1000).to_bytes(2, "little"))

    def test_color(self):
        """GenColorPackage: [4, ch, 3, 0, R, G, B]."""
        self.assertEqual(lp.decrypt_payload(lp.color(255, 0, 128)[5:]),
                         b"\x00\x05\x04\xff\x03\x00\xff\x00\x80")


@unittest.skipUnless(os.path.exists(REPLAY_CSV), "replay.csv not found")
class CaptureCompatibilityTests(unittest.TestCase):
    """Декодер должен валидировать и расшифровывать реальные captured пакеты."""

    @classmethod
    def setUpClass(cls):
        cls.payloads = pair_frames(parse_capture_writes(REPLAY_CSV))
        cls.rgb = [p for p in cls.payloads if len(p) >= 235]

    def test_capture_has_rgb_packets(self):
        self.assertGreater(len(self.rgb), 0, "no RGB-sized payloads in capture")

    def test_all_payloads_checksum(self):
        """Все captured payload проходят checksum — контроллер это проверяет."""
        ok = sum(1 for p in self.payloads
                 if len(p) >= 3 and p[0] == (sum(p[1:]) & 0xFF))
        self.assertEqual(ok, len(self.payloads),
                         f"{len(self.payloads) - ok} payloads failed checksum")

    def test_rgb_payloads_decrypt(self):
        ok = 0
        for p in self.rgb:
            try:
                data = lp.decrypt_payload(p)
            except ValueError:
                continue
            if data[:8] == lp._DATA_RGB_HEADER:
                ok += 1
        self.assertEqual(ok, len(self.rgb),
                         f"{len(self.rgb) - ok} RGB packets failed decrypt")

    def test_small_payloads_decrypt(self):
        known = (b"\x00\x01", b"\x00\x02", b"\x00\x03", b"\x01\x00", b"\x00\x05")
        checked = 0
        for p in self.payloads:
            if len(p) >= 235 or len(p) < 5:
                continue
            try:
                data = lp.decrypt_payload(p)
            except ValueError:
                continue
            if any(data.startswith(k) for k in known):
                checked += 1
        self.assertGreater(checked, 0, "no small packets decrypted")

    def test_our_packets_indistinguishable(self):
        """Наш пакет дешифруется тем же способом что и captured."""
        wire = lp.rgb_transfer([(10, 20, 30)] * 75)
        data = lp.decrypt_payload(wire[5:])
        self.assertEqual(data[:8], lp._DATA_RGB_HEADER)


class NativeBridgeTests(unittest.TestCase):
    def setUp(self):
        self.b = NativeBridge()
        self.b.init()

    def test_interface_matches_beelight_bridge(self):
        for name in ("init", "ready", "make_color_packet", "make_bright_packet",
                     "make_switch_packet", "make_workmode_pc_packet",
                     "make_rgb_transfer_packet", "get_heartbeat"):
            self.assertTrue(hasattr(self.b, name), f"missing {name}")

    def test_packets_before_ready(self):
        b = NativeBridge()
        self.assertIsNone(b.make_color_packet(255, 0, 0))
        self.assertIsNone(b.make_rgb_transfer_packet([(1, 2, 3)]))

    def test_color_packet_decodes(self):
        wire = self.b.make_color_packet(200, 100, 50)
        data = lp.decrypt_payload(wire[5:])
        led = data[8:]
        self.assertEqual(len(led), 225)
        self.assertEqual(set(led), {200, 100, 50})

    def test_brightness_is_hardware_packet(self):
        """Яркость — настоящий wire-пакет (ctrl=2), НЕ software-скейлинг RGB."""
        pkt = self.b.make_bright_packet(500)
        self.assertIsNotNone(pkt)
        self.assertEqual(lp.decrypt_payload(pkt[5:]),
                         b"\x00\x05\x02\xff\x02\x00" + (500).to_bytes(2, "little"))

    def test_brightness_does_not_scale_rgb(self):
        """Регрессия: после make_bright_packet цвет остаётся без software-скейлинга
        (dimmer применяет сам контроллер)."""
        self.b.make_bright_packet(0)
        wire = self.b.make_color_packet(255, 255, 255)
        data = lp.decrypt_payload(wire[5:])
        self.assertEqual(set(data[8:]), {255})

    def test_heartbeat(self):
        hb = self.b.get_heartbeat()
        self.assertIsNotNone(hb)
        self.assertEqual(lp.decrypt_payload(hb[5:]), b"\x01\x00")

    def test_handshake_sequence_nonempty(self):
        seq = self.b.handshake_packets()
        self.assertGreater(len(seq), 0)
        for pkt in seq:
            self.assertEqual(pkt[:3], lp.FRAME_MAGIC)


class DriverBrightnessMappingTests(unittest.TestCase):
    """UI-яркость 0-255 конвертируется в hardware dimmer 0-1000."""

    def test_hw_dimmer_mapping(self):
        try:
            from soulight.protocol.serial_driver import LEDDriver
        except ImportError:
            self.skipTest("pyserial not installed")
        d = LEDDriver(protocol="native")
        for ui, hw in ((0, 0), (255, 1000), (128, 501)):
            d.set_brightness(ui)
            self.assertEqual(d._hw_dimmer(), hw)


if __name__ == "__main__":
    unittest.main()
