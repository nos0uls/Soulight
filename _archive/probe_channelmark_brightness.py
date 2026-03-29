# -*- coding: utf-8 -*-
"""
probe_channelmark_brightness.py — Test if channelMark affects brightness byte.

Bridge sends channelMark=0, but .NET default is 0xFF.
Capture packets have brightness=0xFF. Coincidence?
"""
import os, sys, clr
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

BEELIGHT_EXE = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte, Activator
from System.Drawing import Color
import System

ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static


def derive_key(cipher, period):
    key = [0] * period
    for i in range(period):
        src = 2 + ((i - 2) % period)
        key[i] = cipher[src]
    return bytes(key)


def decrypt(cipher, key):
    period = len(key)
    return bytes(cipher[i] ^ key[i % period] for i in range(len(cipher)))


def main():
    asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
    ctrl = [t for t in asm.GetTypes() if t.Name == "LProtocolCtrl"][0]

    # Find Color[] overload
    color_overload = None
    for m in ctrl.GetMethods(ALL):
        if m.Name == "GenRGBTransferPackage":
            params = m.GetParameters()
            if len(params) == 2 and params[0].ParameterType.Name == "Color[]":
                color_overload = m
                break

    print("=" * 70)
    print("  channelMark vs brightness")
    print("=" * 70)

    colors75 = Array[Color]([Color.Red] * 75)

    for ch_mark in [0, 1, 127, 255]:
        # Generate 5 packets for each channelMark
        for trial in range(5):
            result = color_overload.Invoke(None, Array[System.Object]([
                colors75, Byte(ch_mark)
            ]))
            if result is None:
                print(f"  channelMark={ch_mark}: None"); continue

            pkt = bytes(result)
            payload = pkt[5:]
            pl = len(payload)
            period = pl - 235
            if period < 1:
                print(f"  channelMark={ch_mark}: payload={pl} INVALID PERIOD"); continue

            key = derive_key(payload, period)
            plain = decrypt(payload, key)
            cs = pl - 225

            # Find brightness (should be at cs-5)
            brightness = plain[cs - 5]
            hdr_bytes = plain[cs-7:cs]
            led0 = (plain[cs], plain[cs+1], plain[cs+2])

            if trial == 0:
                print(f"\n  channelMark={ch_mark} (0x{ch_mark:02x}):")
            print(f"    [{trial}] payload={pl} period={period} "
                  f"brightness=0x{brightness:02x} "
                  f"hdr[-7:]={hdr_bytes.hex()} "
                  f"LED0={led0}")

    # Also try GenColorPackage with different channelMark
    print(f"\n{'='*70}")
    print("  GenColorPackage channelMark test")
    print("=" * 70)

    gen_color = None
    for m in ctrl.GetMethods(ALL):
        if m.Name == "GenColorPackage":
            gen_color = m; break

    if gen_color:
        for ch_mark in [0, 255]:
            result = gen_color.Invoke(None, Array[System.Object]([
                Color.Red, Byte(ch_mark)
            ]))
            if result:
                pkt = bytes(result)
                print(f"  channelMark={ch_mark}: {pkt.hex()}")


if __name__ == "__main__":
    main()
