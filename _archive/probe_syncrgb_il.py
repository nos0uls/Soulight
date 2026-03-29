# -*- coding: utf-8 -*-
"""
probe_syncrgb_il.py — Deep reflection probe of LProtocolSyncRGB.

Goals:
1. Find GenProtocolSyncRGB method signature and all overloads
2. Explore LProtocolSyncRGB instance state (fields, properties)
3. Try to create an instance, set state, and call GenProtocolSyncRGB
4. Also probe GenRGBTransferPackage IL to understand encryption
5. Extract method body IL bytecode for decompilation
"""
import os, sys, clr

BEELIGHT_DIR = r"C:\Program Files (x86)\Beelight\Beelight V3.0"
BEELIGHT_EXE = os.path.join(BEELIGHT_DIR, "Beelight.exe")
clr.AddReference(BEELIGHT_EXE)

from System.Reflection import BindingFlags
from System import Array, Byte, Type
import System

ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static


def inspect_type(t, depth=0):
    """Inspect a .NET type: fields, properties, methods, constructors."""
    prefix = "  " * depth
    print(f"{prefix}TYPE: {t.FullName}")
    print(f"{prefix}  Base: {t.BaseType}")
    print(f"{prefix}  IsAbstract={t.IsAbstract} IsSealed={t.IsSealed} IsStatic={t.IsAbstract and t.IsSealed}")

    # Constructors
    ctors = t.GetConstructors(ALL)
    if ctors:
        print(f"{prefix}  CONSTRUCTORS ({len(ctors)}):")
        for c in ctors:
            params = ", ".join(f"{p.ParameterType.Name} {p.Name}" for p in c.GetParameters())
            print(f"{prefix}    ({params})")

    # Fields
    fields = t.GetFields(ALL)
    if fields:
        print(f"{prefix}  FIELDS ({len(fields)}):")
        for f in fields:
            static = " [static]" if f.IsStatic else ""
            print(f"{prefix}    {f.FieldType.Name} {f.Name}{static}")

    # Properties
    props = t.GetProperties(ALL)
    if props:
        print(f"{prefix}  PROPERTIES ({len(props)}):")
        for p in props:
            print(f"{prefix}    {p.PropertyType.Name} {p.Name}")

    # Methods (non-inherited)
    methods = t.GetMethods(ALL | BindingFlags.DeclaredOnly)
    if methods:
        print(f"{prefix}  METHODS ({len(methods)}):")
        for m in methods:
            params = ", ".join(f"{p.ParameterType.Name} {p.Name}" for p in m.GetParameters())
            static = " [static]" if m.IsStatic else ""
            ret = m.ReturnType.Name if m.ReturnType else "void"
            print(f"{prefix}    {ret} {m.Name}({params}){static}")


def get_il_bytes(method_info):
    """Extract IL bytecodes from a MethodInfo."""
    try:
        body = method_info.GetMethodBody()
        if body is None:
            return None
        il = body.GetILAsByteArray()
        if il is None:
            return None
        return bytes(il)
    except Exception as e:
        return None


def main():
    asm = System.Reflection.Assembly.LoadFrom(BEELIGHT_EXE)

    all_types = asm.GetTypes()

    # Find SyncRGB-related types
    print("=" * 70)
    print("  SYNCRGB-RELATED TYPES")
    print("=" * 70)

    sync_types = [t for t in all_types if "Sync" in t.Name or "sync" in t.Name.lower()]
    for t in sync_types:
        inspect_type(t)
        print()

    # Find LProtocolSyncRGB specifically
    print("=" * 70)
    print("  LProtocolSyncRGB DETAIL")
    print("=" * 70)

    sync_rgb = None
    for t in all_types:
        if t.Name == "LProtocolSyncRGB":
            sync_rgb = t
            break

    if sync_rgb:
        inspect_type(sync_rgb)

        # Get IL for GenProtocolSyncRGB
        methods = sync_rgb.GetMethods(ALL | BindingFlags.DeclaredOnly)
        for m in methods:
            if "Gen" in m.Name or "Encrypt" in m.Name or "Pack" in m.Name:
                il = get_il_bytes(m)
                if il:
                    print(f"\n  IL for {m.Name}: {len(il)} bytes")
                    # Show hex dump
                    for offset in range(0, min(len(il), 500), 16):
                        chunk = il[offset:offset + 16]
                        hex_str = " ".join(f"{b:02x}" for b in chunk)
                        print(f"    {offset:04x}: {hex_str}")
                    if len(il) > 500:
                        print(f"    ... ({len(il) - 500} more bytes)")

    # Find LProtocolCtrl (has GenRGBTransferPackage)
    print("\n" + "=" * 70)
    print("  LProtocolCtrl — GenRGBTransferPackage IL")
    print("=" * 70)

    ctrl = None
    for t in all_types:
        if t.Name == "LProtocolCtrl":
            ctrl = t
            break

    if ctrl:
        methods = ctrl.GetMethods(ALL | BindingFlags.DeclaredOnly)
        for m in methods:
            if "RGB" in m.Name or "Encrypt" in m.Name or "encrypt" in m.Name:
                il = get_il_bytes(m)
                params = ", ".join(f"{p.ParameterType.Name} {p.Name}" for p in m.GetParameters())
                print(f"\n  {m.ReturnType.Name} {m.Name}({params})")
                if il:
                    print(f"  IL: {len(il)} bytes")
                    for offset in range(0, min(len(il), 500), 16):
                        chunk = il[offset:offset + 16]
                        hex_str = " ".join(f"{b:02x}" for b in chunk)
                        print(f"    {offset:04x}: {hex_str}")
                    if len(il) > 500:
                        print(f"    ... ({len(il) - 500} more bytes)")

    # Find any encryption-related methods
    print("\n" + "=" * 70)
    print("  ENCRYPTION-RELATED METHODS (all types)")
    print("=" * 70)

    for t in all_types:
        methods = t.GetMethods(ALL | BindingFlags.DeclaredOnly)
        for m in methods:
            name_lower = m.Name.lower()
            if any(k in name_lower for k in ["encrypt", "crypt", "cipher", "xor", "checksum", "crc", "hash", "sign"]):
                params = ", ".join(f"{p.ParameterType.Name} {p.Name}" for p in m.GetParameters())
                static = " [static]" if m.IsStatic else ""
                print(f"  {t.Name}.{m.Name}({params}) -> {m.ReturnType.Name}{static}")
                il = get_il_bytes(m)
                if il:
                    print(f"    IL: {len(il)} bytes")

    # Find LProtocolBase (base class, likely has encryption)
    print("\n" + "=" * 70)
    print("  LProtocolBase DETAIL")
    print("=" * 70)

    base_type = None
    for t in all_types:
        if t.Name == "LProtocolBase":
            base_type = t
            break

    if base_type:
        inspect_type(base_type)
        methods = base_type.GetMethods(ALL | BindingFlags.DeclaredOnly)
        for m in methods:
            il = get_il_bytes(m)
            if il and len(il) > 20:
                params = ", ".join(f"{p.ParameterType.Name} {p.Name}" for p in m.GetParameters())
                print(f"\n  {m.ReturnType.Name} {m.Name}({params}): IL={len(il)} bytes")
                for offset in range(0, min(len(il), 300), 16):
                    chunk = il[offset:offset + 16]
                    hex_str = " ".join(f"{b:02x}" for b in chunk)
                    print(f"    {offset:04x}: {hex_str}")
                if len(il) > 300:
                    print(f"    ... ({len(il) - 300} more bytes)")


if __name__ == "__main__":
    main()
