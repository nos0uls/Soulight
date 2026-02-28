# -*- coding: utf-8 -*-
"""
dll_blackbox.py - Black-box analysis of beelightLib.dll
Call exported functions with known inputs, observe outputs,
reverse-engineer the key generation.

Run with 32-bit Python!
"""
import sys, os, ctypes, struct, time

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
DLL_PATH = os.path.join(CSV_DIR, "\u041d\u043e\u0432\u0430\u044f \u043f\u0430\u043f\u043a\u0430", "Dumps", "beelightLib.dll")


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    if struct.calcsize("P") * 8 != 32:
        print("ERROR: Need 32-bit Python!"); return

    dll = ctypes.CDLL(DLL_PATH)
    print(f"DLL loaded: {DLL_PATH}")

    # ================================================================
    # 1. get_connect_package — simplest, no color params
    #    Signature: get_connect_package(byte* pout)
    # ================================================================
    print(f"\n{'='*70}")
    print("1. get_connect_package — multiple calls")
    print(f"{'='*70}")

    for call_n in range(5):
        buf = ctypes.create_string_buffer(512)
        ret = dll.get_connect_package(buf)
        raw = buf.raw
        # Find actual end (last non-zero byte)
        end = 0
        for i in range(len(raw)-1, -1, -1):
            if raw[i] != 0:
                end = i + 1
                break
        data = raw[:end]
        print(f"  call {call_n}: ret={ret} len={end} data={data.hex()}")

        # Check for 55AA5A frame header
        if data[:3] == b'\x55\xAA\x5A':
            plen = data[3]
            attr = data[4]
            payload = data[5:5+plen]
            print(f"    -> WIRE format! plen={plen} attr={attr:02x} payload={payload.hex()}")
            print(f"    -> byte[0]={payload[0]:02x} byte[1]={payload[1]:02x}")
        elif data[:3] == b'MgL':
            print(f"    -> MgL format!")
        time.sleep(0.01)

    # ================================================================
    # 2. get_scen_package — with BLACK (R=0,G=0,B=0)
    #    Signature: get_scen_package(scenid, dimmer, R, G, B, power, pout)
    # ================================================================
    print(f"\n{'='*70}")
    print("2. get_scen_package BLACK (R=0,G=0,B=0) — multiple calls")
    print(f"{'='*70}")

    black_outputs = []
    for call_n in range(5):
        buf = ctypes.create_string_buffer(512)
        ret = dll.get_scen_package(0, 100, 0, 0, 0, 1, buf)
        raw = buf.raw
        end = max((i for i in range(len(raw)) if raw[i]), default=0) + 1
        data = raw[:end]
        black_outputs.append(data)
        print(f"  call {call_n}: ret={ret} len={end} first20={data[:20].hex()}")

        if data[:3] == b'\x55\xAA\x5A':
            plen = data[3]
            payload = data[5:5+plen]
            print(f"    -> WIRE plen={plen} byte[0:4]={payload[:4].hex()}")
        elif data[:3] == b'MgL':
            # MgL format: header + encrypted
            print(f"    -> MgL format, total_len={end}")
            # Show structure
            print(f"    -> header: {data[:16].hex()}")
            print(f"    -> data:   {data[16:32].hex()}...")

    # ================================================================
    # 3. Compare BLACK calls: XOR consecutive outputs
    # ================================================================
    print(f"\n{'='*70}")
    print("3. XOR between consecutive BLACK outputs")
    print(f"{'='*70}")

    for i in range(len(black_outputs)-1):
        d1, d2 = black_outputs[i], black_outputs[i+1]
        min_l = min(len(d1), len(d2))
        xor = bytes(d1[j] ^ d2[j] for j in range(min_l))
        # Check period 3
        nonzero_start = next((j for j in range(min_l) if xor[j] != 0), min_l)
        if nonzero_start < min_l:
            is_p3 = all(xor[j] == xor[nonzero_start + (j-nonzero_start) % 3]
                        for j in range(nonzero_start, min_l) if xor[j] != 0)
        else:
            is_p3 = True
        print(f"  xor[{i}^{i+1}]: first_diff@{nonzero_start} period3={is_p3}")
        print(f"    xor[0:30]={xor[:30].hex()}")

    # ================================================================
    # 4. get_scen_package with RED (R=255,G=0,B=0)
    # ================================================================
    print(f"\n{'='*70}")
    print("4. get_scen_package RED (R=255,G=0,B=0)")
    print(f"{'='*70}")

    buf = ctypes.create_string_buffer(512)
    dll.get_scen_package(0, 100, 255, 0, 0, 1, buf)
    end = max((i for i in range(len(buf.raw)) if buf.raw[i]), default=0) + 1
    red_data = buf.raw[:end]
    print(f"  len={end} first20={red_data[:20].hex()}")

    # Compare RED vs last BLACK
    if black_outputs:
        last_black = black_outputs[-1]
        min_l = min(len(red_data), len(last_black))
        xor = bytes(red_data[j] ^ last_black[j] for j in range(min_l))
        print(f"\n  RED XOR BLACK:")
        print(f"    xor[0:40]={xor[:40].hex()}")

    # ================================================================
    # 5. get_scen_package with known colors to extract key
    # ================================================================
    print(f"\n{'='*70}")
    print("5. Extract key by comparing BLACK and known color")
    print(f"{'='*70}")

    # Two BLACK calls in a row — if nonce increments, key changes
    buf_b1 = ctypes.create_string_buffer(512)
    buf_b2 = ctypes.create_string_buffer(512)
    buf_r = ctypes.create_string_buffer(512)

    dll.get_scen_package(0, 100, 0, 0, 0, 1, buf_b1)
    dll.get_scen_package(0, 100, 0, 0, 0, 1, buf_b2)
    dll.get_scen_package(0, 100, 255, 0, 0, 1, buf_r)

    for name, buf in [("BLACK1", buf_b1), ("BLACK2", buf_b2), ("RED", buf_r)]:
        raw = buf.raw
        end = max((i for i in range(len(raw)) if raw[i]), default=0) + 1
        print(f"  {name}: len={end} data={raw[:min(40,end)].hex()}")

    # ================================================================
    # 6. Try unpack_package on a BLACK output
    # ================================================================
    print(f"\n{'='*70}")
    print("6. unpack_package on a BLACK output")
    print(f"{'='*70}")

    # First generate a fresh packet
    buf_in = ctypes.create_string_buffer(512)
    dll.get_scen_package(0, 100, 128, 64, 32, 1, buf_in)
    in_end = max((i for i in range(len(buf_in.raw)) if buf_in.raw[i]), default=0) + 1
    in_data = buf_in.raw[:in_end]
    print(f"  Input (scen R=128,G=64,B=32): len={in_end} data={in_data[:30].hex()}")

    # Try to unpack
    buf_out = ctypes.create_string_buffer(512)
    try:
        # Guess signature: unpack_package(byte* input, int input_len, byte* output)
        ret = dll.unpack_package(buf_in, in_end, buf_out)
        out_end = max((i for i in range(len(buf_out.raw)) if buf_out.raw[i]), default=0) + 1
        out_data = buf_out.raw[:out_end]
        print(f"  Output: ret={ret} len={out_end} data={out_data[:30].hex()}")

        # Check if output contains our R,G,B values
        for pos in range(out_end):
            if (out_data[pos] == 128 and pos + 2 < out_end
                    and out_data[pos+1] == 64 and out_data[pos+2] == 32):
                print(f"  FOUND R,G,B at offset {pos}!")
    except Exception as e:
        print(f"  Error: {e}")
        # Try alternative signatures
        try:
            ret = dll.unpack_package(buf_in, buf_out)
            out_end = max((i for i in range(len(buf_out.raw)) if buf_out.raw[i]), default=0) + 1
            print(f"  Alt sig ret={ret} len={out_end} data={buf_out.raw[:30].hex()}")
        except Exception as e2:
            print(f"  Alt sig error: {e2}")

    # ================================================================
    # 7. Call get_scen_package 20 times, track byte[0] and byte[1]
    # ================================================================
    print(f"\n{'='*70}")
    print("7. Nonce tracking across 20 calls")
    print(f"{'='*70}")

    for i in range(20):
        buf = ctypes.create_string_buffer(512)
        dll.get_scen_package(0, 100, 0, 0, 0, 1, buf)
        raw = buf.raw
        end = max((j for j in range(len(raw)) if raw[j]), default=0) + 1

        # Find 55AA5A or MgL header offset
        is_wire = raw[:3] == b'\x55\xAA\x5A'
        is_mgl = raw[:3] == b'MgL'

        if is_wire:
            plen = raw[3]
            payload = raw[5:5+plen]
            print(f"  [{i:2d}] WIRE plen={plen} b0={payload[0]:02x} b1={payload[1]:02x} "
                  f"b2={payload[2]:02x} b3={payload[3]:02x}")
        elif is_mgl:
            # MgL: "MgL" + length(4) + data
            mgl_len = struct.unpack_from("<I", raw, 3)[0] if end > 7 else 0
            print(f"  [{i:2d}] MgL len={mgl_len} b3={raw[3]:02x} b4={raw[4]:02x} "
                  f"b7={raw[7]:02x} b8={raw[8]:02x}")
        else:
            print(f"  [{i:2d}] UNK first8={raw[:8].hex()}")

    # ================================================================
    # 8. Detailed MgL format analysis
    # ================================================================
    print(f"\n{'='*70}")
    print("8. MgL format detail")
    print(f"{'='*70}")

    buf = ctypes.create_string_buffer(512)
    dll.get_scen_package(0, 100, 0, 0, 0, 1, buf)
    raw = buf.raw
    end = max((j for j in range(len(raw)) if raw[j]), default=0) + 1

    print(f"  Total length: {end}")
    print(f"  Hex dump:")
    for offset in range(0, min(end, 64), 16):
        hex_line = " ".join(f"{raw[offset+j]:02x}" for j in range(min(16, end-offset)))
        ascii_line = "".join(chr(raw[offset+j]) if 32 <= raw[offset+j] < 127 else '.'
                            for j in range(min(16, end-offset)))
        print(f"    {offset:04x}: {hex_line:<48s} {ascii_line}")

    print("\nDONE")


if __name__ == "__main__":
    main()
