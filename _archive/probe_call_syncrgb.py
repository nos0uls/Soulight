# -*- coding: utf-8 -*-
"""
probe_call_syncrgb.py — Try calling GenProtocolSyncRGB and GetHash.

GenProtocolSyncRGB(Byte rows, Byte columns, Color[] colors) -> Byte[]
GetHash(Byte[] data) -> Int32

Also probe LP_ATTR and LP_CMD enums.
"""
import os, sys, clr

BEELIGHT_DIR = r"C:\Program Files (x86)\Beelight\Beelight V3.0"
BEELIGHT_EXE = os.path.join(BEELIGHT_DIR, "Beelight.exe")
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte, Type, Int32
from System.Drawing import Color
import System

ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static


def main():
    asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)
    all_types = asm.GetTypes()

    # ================================================================
    # 1. Enumerate LP_ATTR and LP_CMD
    # ================================================================
    print("=" * 60)
    print("  LP_ATTR AND LP_CMD ENUMS")
    print("=" * 60)

    for t in all_types:
        if t.Name in ("LP_ATTR", "LP_CMD"):
            print(f"\n  {t.Name} (IsEnum={t.IsEnum}):")
            if t.IsEnum:
                for name in System.Enum.GetNames(t):
                    val = System.Enum.Parse(t, name)
                    print(f"    {name} = {System.Convert.ToInt32(val)}")
            else:
                # Maybe a class with constants
                for f in t.GetFields(ALL):
                    if f.IsStatic and f.IsLiteral:
                        print(f"    {f.Name} = {f.GetRawConstantValue()}")

    # ================================================================
    # 2. Try GetHash
    # ================================================================
    print("\n" + "=" * 60)
    print("  GetHash TEST")
    print("=" * 60)

    base_type = None
    for t in all_types:
        if t.Name == "LProtocolBase":
            base_type = t
            break

    if base_type:
        get_hash = base_type.GetMethod("GetHash", ALL)
        if get_hash:
            # Test with simple data
            for test_data in [
                bytes([0] * 10),
                bytes([1] * 10),
                bytes([255] * 10),
                bytes(range(10)),
                bytes([0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B, 0x00]),
            ]:
                arr = Array[Byte](test_data)
                try:
                    h = get_hash.Invoke(None, Array[System.Object]([arr]))
                    print(f"  GetHash({test_data.hex()}) = {h} (0x{h:08x})")
                except Exception as e:
                    print(f"  GetHash({test_data.hex()}) ERROR: {e}")

    # ================================================================
    # 3. Try GenProtocolSyncRGB
    # ================================================================
    print("\n" + "=" * 60)
    print("  GenProtocolSyncRGB TEST")
    print("=" * 60)

    sync_rgb_type = None
    for t in all_types:
        if t.Name == "LProtocolSyncRGB":
            sync_rgb_type = t
            break

    if sync_rgb_type:
        # Create instance
        try:
            instance = System.Activator.CreateInstance(sync_rgb_type)
            print(f"  Instance created: {instance}")
        except Exception as e:
            print(f"  Instance creation failed: {e}")
            instance = None

        if instance:
            gen_method = sync_rgb_type.GetMethod("GenProtocolSyncRGB", ALL)
            if gen_method:
                print(f"  Method: {gen_method}")
                print(f"  Params: {', '.join(f'{p.ParameterType.Name} {p.Name}' for p in gen_method.GetParameters())}")

                # Try various rows/columns combos
                for rows, cols in [(1, 75), (75, 1), (3, 25), (5, 15), (1, 1)]:
                    num = rows * cols
                    colors = Array[Color]([Color.Red] * num)
                    try:
                        result = gen_method.Invoke(
                            instance,
                            Array[System.Object]([
                                Byte(rows),
                                Byte(cols),
                                colors
                            ])
                        )
                        if result is not None:
                            pkt = bytes(result)
                            print(f"\n  rows={rows} cols={cols}: GOT PACKET! len={len(pkt)}")
                            print(f"    hex[0:20]: {pkt[:20].hex()}")
                            print(f"    hex[-10:]: {pkt[-10:].hex()}")
                        else:
                            print(f"  rows={rows} cols={cols}: returned None")
                    except Exception as e:
                        err_msg = str(e)[:100]
                        print(f"  rows={rows} cols={cols}: ERROR: {err_msg}")

    # ================================================================
    # 4. Try GenFramePackage
    # ================================================================
    print("\n" + "=" * 60)
    print("  GenFramePackage TEST")
    print("=" * 60)

    if base_type:
        gen_frame = base_type.GetMethod("GenFramePackage", ALL)
        if gen_frame:
            print(f"  Method: {gen_frame}")
            params = gen_frame.GetParameters()
            print(f"  Params: {', '.join(f'{p.ParameterType.Name} {p.Name}' for p in params)}")

            # Find LP_ATTR and LP_CMD types
            lp_attr_type = params[0].ParameterType
            lp_cmd_type = params[1].ParameterType

            print(f"\n  LP_ATTR type: {lp_attr_type.FullName}")
            if lp_attr_type.IsEnum:
                for name in System.Enum.GetNames(lp_attr_type):
                    val = System.Enum.Parse(lp_attr_type, name)
                    print(f"    {name} = {System.Convert.ToInt32(val)}")

            print(f"\n  LP_CMD type: {lp_cmd_type.FullName}")
            if lp_cmd_type.IsEnum:
                for name in System.Enum.GetNames(lp_cmd_type):
                    val = System.Enum.Parse(lp_cmd_type, name)
                    print(f"    {name} = {System.Convert.ToInt32(val)}")

            # Try calling with SyncRGB command
            try:
                test_data = Array[Byte](bytes([0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B, 0x00] + [0xFF] * 225))
                # Try to find SyncRGB enum value
                for name in System.Enum.GetNames(lp_cmd_type):
                    if "sync" in name.lower() or "rgb" in name.lower():
                        cmd_val = System.Enum.Parse(lp_cmd_type, name)
                        # Use first ATTR value
                        attr_names = System.Enum.GetNames(lp_attr_type)
                        for aname in attr_names:
                            attr_val = System.Enum.Parse(lp_attr_type, aname)
                            try:
                                result = gen_frame.Invoke(None, Array[System.Object]([attr_val, cmd_val, test_data]))
                                if result is not None:
                                    pkt = bytes(result)
                                    print(f"\n  GenFramePackage(ATTR={aname}, CMD={name}): len={len(pkt)}")
                                    print(f"    hex[0:20]: {pkt[:20].hex()}")
                                    break
                            except Exception as e:
                                pass
            except Exception as e:
                print(f"  GenFramePackage test error: {str(e)[:100]}")

    # ================================================================
    # 5. Also try GenRGBTransferPackage with same Color[] for comparison
    # ================================================================
    print("\n" + "=" * 60)
    print("  GenRGBTransferPackage COMPARISON")
    print("=" * 60)

    ctrl_type = None
    for t in all_types:
        if t.Name == "LProtocolCtrl":
            ctrl_type = t
            break

    if ctrl_type:
        # Find the Color[] overload
        methods = ctrl_type.GetMethods(ALL)
        for m in methods:
            if m.Name == "GenRGBTransferPackage":
                params = m.GetParameters()
                ptypes = [p.ParameterType.Name for p in params]
                if "Color[]" in ptypes:
                    colors = Array[Color]([Color.Red] * 75)
                    try:
                        result = m.Invoke(None, Array[System.Object]([colors, Byte(0)]))
                        if result is not None:
                            pkt = bytes(result)
                            print(f"  GenRGBTransferPackage(Color[], 0): len={len(pkt)}")
                            print(f"    hex[0:20]: {pkt[:20].hex()}")
                        else:
                            print(f"  GenRGBTransferPackage(Color[], 0): returned None")
                    except Exception as e:
                        print(f"  ERROR: {str(e)[:100]}")


if __name__ == "__main__":
    main()
