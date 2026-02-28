# -*- coding: utf-8 -*-
"""
try_direct_dll.py — Attempt to call beelightLib.dll via a 32-bit subprocess.

Since our Python is 64-bit and the DLL is 32-bit, we create a small 32-bit 
helper program. But first, let's try a completely different approach:

APPROACH: Use the DLL file to extract the table at runtime by creating
a small C program that loads the DLL, reads the table, and dumps it.

We can use the MSVC compiler or gcc if available.

Actually, the simplest approach: write a small .NET program (which can be 
AnyCPU/32-bit) that loads beelightLib.dll via DllImport and calls 
get_connect_package, then reads the result.

Even simpler: use ctypes with 32-bit Python from python.org.

But let me first try the approach that needs NO extra tools:
Parse the DLL's custom Base64 properly by re-implementing FUN_10001110 + FUN_10001000.
"""
import sys
import struct
import os

DLL_PATH = os.path.join(
    os.path.dirname(os.path.abspath(__file__)),
    "Новая папка", "Dumps", "beelightLib.dll"
)


def va_to_file_offset(dll_data, va, image_base=0x10000000):
    pe_offset = struct.unpack_from('<I', dll_data, 0x3C)[0]
    num_sections = struct.unpack_from('<H', dll_data, pe_offset + 6)[0]
    opt_hdr_size = struct.unpack_from('<H', dll_data, pe_offset + 20)[0]
    section_start = pe_offset + 24 + opt_hdr_size
    rva = va - image_base
    for i in range(num_sections):
        so = section_start + i * 40
        sec_va = struct.unpack_from('<I', dll_data, so + 12)[0]
        sec_vsize = struct.unpack_from('<I', dll_data, so + 8)[0]
        sec_rawptr = struct.unpack_from('<I', dll_data, so + 20)[0]
        if sec_va <= rva < sec_va + sec_vsize:
            return sec_rawptr + (rva - sec_va)
    return None


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    
    with open(DLL_PATH, "rb") as f:
        dll = f.read()
    
    # Read the alphabet from multiple null-separated segments at DAT_1003e890
    alpha_off = va_to_file_offset(dll, 0x1003e890)
    
    # The Ghidra output showed the raw data has null separators between groups:
    # @#$%^&*()-x[];!:|<>?, \0\0\0 asdfghjklZXCVBNM \0\0\0\0 123457890qwertyui \0\0\0 QWERTYUIOP
    # That's 4 groups separated by nulls. FUN_10001110 assembles them into one 64-char array.
    
    # Read raw 256 bytes from alphabet area
    raw = dll[alpha_off:alpha_off + 256]
    
    # Extract all printable ASCII runs
    runs = []
    current_run = []
    for b in raw:
        if 32 <= b < 127:
            current_run.append(chr(b))
        else:
            if current_run:
                runs.append(''.join(current_run))
                current_run = []
    if current_run:
        runs.append(''.join(current_run))
    
    print("Alphabet segments from DLL:")
    total_chars = 0
    for i, run in enumerate(runs):
        print(f"  Segment {i}: '{run}' ({len(run)} chars)")
        total_chars += len(run)
    print(f"  Total: {total_chars} chars")
    
    # The FUN_10001110 pseudo-C initializes a 64-char array.
    # From disassembly at 10001128: MOV EDX, DAT_1003e890 (= '@')
    # Then it builds the alphabet by scanning through the data.
    # 
    # Looking at the segments:
    #   Segment 0: @#$%^&*()-x[];!:|<>?, (21 chars)
    #   Segment 1: asdfghjklZXCVBNM      (17 chars) 
    #   Segment 2: 123457890qwertyui     (17 chars)
    #   Segment 3: QWERTYUIOP            (10 chars)
    # Total: 65 chars... we need exactly 64 (one char might be a terminator)
    
    # Let me look at it differently. Maybe it's NOT concatenated segments.
    # FUN_10001110 builds the alphabet by some transformation.
    # Let me check if the segments in order give us 64 chars:
    
    alphabet = ''.join(runs[:4])
    print(f"\nConcatenated: '{alphabet}' ({len(alphabet)} chars)")
    
    # If 65, drop last char (or first)
    if len(alphabet) == 65:
        # '@' at the start might be a marker, not part of alphabet
        # Standard Base64 has 64 chars, let's try without '@'
        alphabet_no_at = alphabet[1:]
        print(f"Without '@': '{alphabet_no_at}' ({len(alphabet_no_at)} chars)")
    
    # Now read the encoded string
    enc_off = va_to_file_offset(dll, 0x1003c4c0)
    end = dll.index(b'\x00', enc_off)
    encoded = dll[enc_off:end].decode('ascii')
    print(f"\nEncoded string: {len(encoded)} chars")
    
    # Try both alphabets
    for name, alpha in [("full_65", alphabet), ("no_at_64", alphabet[1:] if len(alphabet) > 64 else alphabet)]:
        if len(alpha) < 64:
            print(f"\n{name}: only {len(alpha)} chars, skipping")
            continue
        alpha = alpha[:64]
        print(f"\n=== Trying alphabet '{name}': '{alpha}' ===")
        
        # Custom Base64 decode (matching FUN_10001000 logic)
        # FUN_10001000 processes 4 chars at a time, produces 3 bytes
        # Bit packing: shift by 0,6,12,18 (LSB first)
        # '=' means end
        result = bytearray()
        i = 0
        stopped = False
        while i + 3 < len(encoded) and not stopped:
            chunk = encoded[i:i+4]
            i += 4
            
            bits = 0
            shift = 0
            pad = False
            for ch in chunk:
                if ch == '=':
                    pad = True
                    stopped = True
                    break
                idx = alpha.find(ch)
                if idx < 0:
                    idx = 0  # Unknown char
                bits |= (idx & 0x3F) << shift
                shift += 6
            
            # Output bytes
            b0 = bits & 0xFF
            b1 = (bits >> 8) & 0xFF
            b2 = (bits >> 16) & 0xFF
            
            if pad:
                # Check how many chars we got before pad
                if shift >= 12:  # at least 2 chars decoded
                    result.append(b0)
                if shift >= 18:  # at least 3 chars decoded
                    result.append(b1)
            else:
                result.append(b0)
                result.append(b1)
                result.append(b2)
        
        table = bytes(result)
        print(f"  Decoded: {len(table)} bytes")
        print(f"  First 16: {' '.join(f'{b:02x}' for b in table[:16])}")
        print(f"  Last  16: {' '.join(f'{b:02x}' for b in table[-16:])}")
        
        # Now verify: for a 238-byte BLACK packet, if the DLL format has
        # bytes[0-10] as plaintext and bytes[11+] encrypted with this table,
        # let's check if decryption produces sensible results.
        #
        # But we established that the captured packets DON'T have
        # the DLL header format. So let me try a completely different hypothesis:
        #
        # What if the ENTIRE packet is encrypted, including header?
        # That would mean IV is transmitted separately or derived differently.
        #
        # OR: the .NET code builds its own packet format without using FUN_10001880.
        # In that case, the simpler 3-byte XOR IS the real cipher, 
        # but derived from the table somehow.
        
        # Let me check: for BLACK packets, the "key3" from LED region
        # Let's see if key3 can be derived from the table + some per-packet index
        
        from collections import Counter
        csv_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "black.csv")
        pkts = []
        with open(csv_path, "r", errors="replace") as f:
            for line in f:
                if "IRP_MJ_WRITE;DOWN" not in line: continue
                parts = line.split(";")
                if len(parts) <= 7: continue
                data = parts[5].strip()
                try: raw = bytes.fromhex(data.replace(" ", ""))
                except: continue
                if len(raw) == 238: pkts.append(raw)
        
        print(f"\n  Testing on {len(pkts)} BLACK 238-byte packets")
        
        # For BLACK 238-byte: key3 = [pkt[12], pkt[13], pkt[14]] (since plain LED = 0)
        # Check if key3 bytes appear consecutively in the table
        for pidx, pkt in enumerate(pkts[:5]):
            k0, k1, k2 = pkt[12], pkt[13], pkt[14]
            # Check consecutive triplets in table
            for t in range(len(table) - 2):
                if table[t] == k0 and table[t+1] == k1 and table[t+2] == k2:
                    print(f"    Pkt {pidx}: key3=[{k0:02x},{k1:02x},{k2:02x}] MATCH at table[{t}]")
                    break
            else:
                # Check if k0 XOR k1 XOR k2 appears in table, etc
                # Check table[t] XOR table[t+1] XOR table[t+2]
                pass
                print(f"    Pkt {pidx}: key3=[{k0:02x},{k1:02x},{k2:02x}] no consecutive match")

    # Also: let's check if any exported function produces 238-byte output
    # get_large_screenbuf_package_length returns the expected buffer size
    # We need to check what it returns. Since we can't call it, let's look
    # for the constant in the DLL.
    
    print(f"\n{'='*70}")
    print("Searching DLL for constants related to packet size")
    print(f"{'='*70}")
    
    # Search for 238 (0xEE), 1500 (0x5DC), 1515
    for val, name in [(238, "238"), (1500, "1500/0x5DC"), (1515, "1515"), 
                       (225, "225=75*3"), (75, "75=NUM_LEDS"),
                       (0x674D, "magic 0x674D")]:
        # Search as little-endian 32-bit
        needle32 = struct.pack('<I', val)
        positions = []
        start = 0
        while True:
            pos = dll.find(needle32, start)
            if pos < 0: break
            positions.append(pos)
            start = pos + 1
        if positions:
            print(f"  {name} (32-bit LE): found at file offsets {[f'0x{p:08x}' for p in positions[:5]]}")
    
    # Also search for byte value 0x4B (75) as immediate in code
    # And 0xEE (238)


if __name__ == "__main__":
    main()
