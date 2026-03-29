# -*- coding: utf-8 -*-
"""
probe_runtime_il_dump.py — Runtime IL dump after method execution.

CryptoObfuscator decrypts method IL on first call.
Strategy: call the method FIRST, then read IL bytes.
Also try GetHash with full-size packets.
"""
import os, sys, clr

BEELIGHT_EXE = r"C:\Program Files (x86)\Beelight\Beelight V3.0\Beelight.exe"
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte, Activator
from System.Drawing import Color
import System

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from soulight.protocol.bridge import BeelightBridge

ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static


def get_il(method):
    try:
        body = method.GetMethodBody()
        if body is None: return None
        il = body.GetILAsByteArray()
        return bytes(il) if il else None
    except: return None


def dump_il(name, il_bytes, max_bytes=600):
    if il_bytes is None:
        print(f"  {name}: IL = None"); return
    print(f"  {name}: IL = {len(il_bytes)} bytes")
    for off in range(0, min(len(il_bytes), max_bytes), 16):
        chunk = il_bytes[off:off+16]
        h = " ".join(f"{b:02x}" for b in chunk)
        print(f"    {off:04x}: {h}")
    if len(il_bytes) > max_bytes:
        print(f"    ... ({len(il_bytes)-max_bytes} more bytes)")


def main():
    asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
    all_types = {t.Name: t for t in asm.GetTypes()}

    # ================================================================
    # STEP 1: Read IL BEFORE calling any methods
    # ================================================================
    print("=" * 70)
    print("  IL BEFORE method calls")
    print("=" * 70)

    ctrl = all_types.get("LProtocolCtrl")
    base = all_types.get("LProtocolBase")

    if ctrl:
        for m in ctrl.GetMethods(ALL | BindingFlags.DeclaredOnly):
            if "GenRGBTransfer" in m.Name:
                dump_il(f"BEFORE: {m.Name}", get_il(m))

    if base:
        for m in base.GetMethods(ALL | BindingFlags.DeclaredOnly):
            if m.Name in ("GenFramePackage", "GetHash"):
                dump_il(f"BEFORE: {m.Name}", get_il(m))

    # ================================================================
    # STEP 2: Call methods to trigger deobfuscation
    # ================================================================
    print(f"\n{'='*70}")
    print("  CALLING METHODS TO TRIGGER DEOBFUSCATION")
    print("=" * 70)

    # Call GenRGBTransferPackage
    bridge = BeelightBridge()
    bridge.init()
    pkt = bridge.make_rgb_transfer_packet([(255, 0, 0)] * 75)
    print(f"  GenRGBTransferPackage: returned {len(pkt) if pkt else 'None'} bytes")

    # Call GenFramePackage
    if base:
        gen_frame = base.GetMethod("GenFramePackage", ALL)
        if gen_frame:
            lp_attr_type = gen_frame.GetParameters()[0].ParameterType
            lp_cmd_type = gen_frame.GetParameters()[1].ParameterType
            attr_req = System.Enum.Parse(lp_attr_type, "LP_ATTR_REQ")
            cmd_hb = System.Enum.Parse(lp_cmd_type, "LP_CMD_HEARTBEAT")
            result = gen_frame.Invoke(None, Array[System.Object]([
                attr_req, cmd_hb, Array[Byte]([])
            ]))
            print(f"  GenFramePackage(HB): returned {len(bytes(result)) if result else 'None'} bytes")

    # ================================================================
    # STEP 3: Read IL AFTER method calls
    # ================================================================
    print(f"\n{'='*70}")
    print("  IL AFTER method calls (should be decrypted)")
    print("=" * 70)

    if ctrl:
        for m in ctrl.GetMethods(ALL | BindingFlags.DeclaredOnly):
            if "GenRGBTransfer" in m.Name or "Gen" in m.Name:
                dump_il(f"AFTER: {m.Name}", get_il(m))

    if base:
        for m in base.GetMethods(ALL | BindingFlags.DeclaredOnly):
            dump_il(f"AFTER: LProtocolBase.{m.Name}", get_il(m))

    # ================================================================
    # STEP 4: Try GetHash with proper-sized data
    # ================================================================
    print(f"\n{'='*70}")
    print("  GetHash WITH FULL-SIZE DATA")
    print("=" * 70)

    if base and pkt:
        get_hash = base.GetMethod("GetHash", ALL)
        if get_hash:
            # Try with the PAYLOAD (without frame header)
            payload = pkt[5:]
            for test_name, test_data in [
                ("full packet (with frame)", pkt),
                ("payload only", payload),
                ("payload[2:]", payload[2:]),
                ("238 zeros", bytes(238)),
                ("245 zeros", bytes(245)),
                ("100 zeros", bytes(100)),
                ("50 zeros", bytes(50)),
                ("30 zeros", bytes(30)),
                ("20 zeros", bytes(20)),
                ("15 zeros", bytes(15)),
            ]:
                arr = Array[Byte](test_data)
                try:
                    h = get_hash.Invoke(None, Array[System.Object]([arr]))
                    print(f"  GetHash({test_name}, len={len(test_data)}): "
                          f"{h} (0x{h & 0xFFFF:04x})")
                except Exception as e:
                    err = str(e).split('\n')[0][:80]
                    print(f"  GetHash({test_name}, len={len(test_data)}): ERROR {err}")

    # ================================================================
    # STEP 5: Also dump ALL types that have interesting methods
    # ================================================================
    print(f"\n{'='*70}")
    print("  ALL METHODS WITH NON-TRIVIAL IL (after deobfuscation)")
    print("=" * 70)

    for t in asm.GetTypes():
        if t.Namespace and "LightProtocol" in str(t.Namespace):
            methods = t.GetMethods(ALL | BindingFlags.DeclaredOnly)
            for m in methods:
                il = get_il(m)
                if il and len(il) > 10:
                    params = ", ".join(f"{p.ParameterType.Name} {p.Name}" for p in m.GetParameters())
                    print(f"\n  {t.Name}.{m.Name}({params}) -> {m.ReturnType.Name}: IL={len(il)}B")
                    # Show first 100 bytes
                    for off in range(0, min(len(il), 100), 16):
                        chunk = il[off:off+16]
                        h = " ".join(f"{b:02x}" for b in chunk)
                        print(f"    {off:04x}: {h}")
                    if len(il) > 100:
                        print(f"    ... +{len(il)-100}B more")


if __name__ == "__main__":
    main()
