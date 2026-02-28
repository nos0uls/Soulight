# -*- coding: utf-8 -*-
"""
analyze_wire_format.py — Reverse engineer the wire format from controller responses
and captured packets.

Key observations:
1. Controller responses start with 55 AA 5A [byte] [byte] ...
2. Wire captures (data packets) do NOT start with 55 AA 5A
3. DLL outputs MgL format which controller doesn't understand
4. So .NET code transforms MgL → wire format before sending

From BeelightProtocol.cs and LProtocolBase.cs:
  - Header: 55 AA 5A (Head0=0x55, Head1=0xAA, Head2=0x5A)
  - Commands: HEARTBEAT=0, FIRM=1, SYNCSTATUS=2, SYNCCONFIG=3, OTA=4,
              CTRL_DEVICE=5, CTRL_SYNC_RGB=6, CTRL_LOG=26
  - GenFramePackage wraps data in 55 AA 5A frame

The controller responses are in 55 AA 5A format. Our data captures DON'T have
55 AA 5A prefix. This means:
  - Heartbeats: 55 AA 5A [cmd] 00 (plain, 5 bytes)
  - Data: something else entirely

Wait - let me re-read the controller responses more carefully.
They contain MULTIPLE 55 AA 5A packets concatenated.

Controller response to heartbeat:
55aa5a 07 00 f63269d15069d1
55aa5a 0b 00 40360f7f668a150ddc0f7f
55aa5a 09 00 f9347b7bb2e6417b7b
...

Format: 55 AA 5A [len_byte] 00 [encrypted_data...]

Let me check: 55 AA 5A 07 00 = 5 bytes header, then 07-5=2? or 07=data_length?

If the 4th byte is length of remaining data:
  55 AA 5A 07 00 F6 32 69 D1 50 69 D1 = 3 header + 1 len + 7 data = 12? No...

Actually from heartbeat: 55 AA 5A F1 00 (len=5 total)
Response: 55 AA 5A 07 00 [7 more bytes] = 12 bytes total? 

Let me check: 55aa5a 07 00 f63269d150 69d1 = 3+1+1+7 = 12 bytes
Next: 55aa5a 0b 00 40360f7f668a150ddc0f7f = 3+1+1+11 = 16 bytes

So format is: 55 AA 5A [data_len] 00 [data_len bytes of data]

In our wire captures of data packets:
  238-byte packets DON'T start with 55 AA 5A
  
But what if the data packets ARE wrapped in 55 AA 5A but our serial capture
doesn't show the framing? Or what if only heartbeats use 55 AA 5A framing?

Actually wait - let me re-examine. The OUTGOING data in captures:
  Heartbeats: 55 AA 5A [EE-F5] 00 (5 bytes)
  Data: [238-245 bytes starting with random-looking bytes]

Maybe the 238-byte data IS the payload INSIDE a 55 AA 5A frame, but the
capture tool separates them because they're written as separate serial.write() calls?

Let me check the ORIGINAL csv more carefully.
"""
import sys
import os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_response_packets(response_hex):
    """Parse concatenated 55 AA 5A response packets."""
    data = bytes.fromhex(response_hex)
    packets = []
    i = 0
    while i < len(data) - 4:
        if data[i] == 0x55 and data[i+1] == 0xAA and data[i+2] == 0x5A:
            data_len = data[i+3]
            pkt = data[i:i+5+data_len]
            packets.append(pkt)
            i += 5 + data_len
        else:
            i += 1
    return packets


def main():
    sys.stdout.reconfigure(encoding="utf-8")

    # Parse controller responses from debug_serial.py output
    print("=== Controller Response Analysis ===\n")
    
    resp1 = "55aa5a0700f63269d15069d155aa5a0b0040360f7f668a150ddc0f7f55aa5a0900f9347b7bb2e6417b7b55aa5a0e00573be8e467f77ec2ed057480e8e455aa5a0d00f438f3ca1e19c1809cbf6ff3ca55aa5a0800bc35fac5fd0cfac555aa5a080099352b"
    
    pkts = parse_response_packets(resp1)
    print(f"Response packets: {len(pkts)}")
    
    for i, pkt in enumerate(pkts):
        hdr = pkt[:5]
        data_len = pkt[3]
        payload = pkt[5:]
        print(f"  [{i}] hdr=[{hdr.hex()}] data_len={data_len} "
              f"payload=[{payload.hex()}] total={len(pkt)}")
    
    # Key: the 4th byte is the payload length.
    # For heartbeat OUT: 55 AA 5A F1 00 → byte[3]=0xF1=241... that's NOT a length
    # Heartbeats OUT: F1-F5 cycle, these are command/status bytes, not lengths
    
    # For responses: byte[3] IS the data length
    # Response: 55 AA 5A 07 00 [7 bytes] = total 12 bytes
    
    # But for OUR heartbeats: 55 AA 5A F1 00
    # F1 = 241 ≠ 0 bytes data. So byte[3] isn't always length.
    # Maybe byte[3] = cmd, byte[4] = attr?
    # For heartbeat: cmd=0xF1, attr=0x00
    # For response:  cmd=0x07, attr=0x00, then data
    
    # Let's check response cmd values
    print("\nResponse cmd values:")
    cmd_vals = set()
    for pkt in pkts:
        cmd_vals.add(pkt[3])
    print(f"  {sorted(cmd_vals)}")
    print(f"  hex: {[f'0x{v:02x}' for v in sorted(cmd_vals)]}")
    
    # Now the REAL question: how does .NET wrap the DLL output for sending?
    # 
    # From LProtocolBase.cs: GenFramePackage builds 55 AA 5A frames
    # From BeelightProtocol.cs: it calls DLL, then probably calls GenFramePackage
    #
    # The wire data packets DON'T have 55 AA 5A prefix.
    # But maybe they're written in TWO serial.write() calls:
    #   1. Write: 55 AA 5A [cmd] [attr] (5 bytes)  ← appears as heartbeat
    #   2. Write: [encrypted payload] (238 bytes)   ← appears as data
    #
    # That would explain why captures show alternating heartbeat+data!
    # The "heartbeat" with cmd=0xEE-0xF5 might actually be the FRAME HEADER
    # for the following data packet!
    
    print("\n" + "=" * 60)
    print("HYPOTHESIS: 55 AA 5A [cmd] [attr] + [data] are ONE packet")
    print("=" * 60)
    
    # Check: heartbeat cmd values in captures
    # From analyze_framing.py: 55 AA 5A [EE-F5] 00
    # 0xEE=238, 0xEF=239, 0xF0=240, 0xF1=241, 0xF2=242, 0xF3=243, 0xF4=244, 0xF5=245
    
    print("\nHeartbeat byte[3] values vs data packet lengths:")
    mapping = {
        0xEE: 238, 0xEF: 239, 0xF0: 240, 0xF1: 241,
        0xF2: 242, 0xF3: 243, 0xF4: 244, 0xF5: 245
    }
    for hb_byte, data_len in sorted(mapping.items()):
        print(f"  0x{hb_byte:02x} ({hb_byte}) → data_len={data_len}")
    
    print("\n*** byte[3] of 'heartbeat' = LENGTH of following data packet! ***")
    print("*** This is NOT a heartbeat - it's a frame header! ***")
    
    # Verify on captures
    print("\n=== Verifying on BLACK captures ===")
    
    black_path = os.path.join(CSV_DIR, "black.csv")
    if os.path.exists(black_path):
        writes = []
        with open(black_path, "r", errors="replace") as f:
            for line in f:
                if "IRP_MJ_WRITE;DOWN" not in line:
                    continue
                parts = line.split(";")
                if len(parts) <= 7:
                    continue
                data = parts[5].strip()
                try:
                    raw = bytes.fromhex(data.replace(" ", ""))
                except ValueError:
                    continue
                writes.append(raw)
        
        # Check pairs: [55 AA 5A xx 00] followed by [xx bytes]
        matches = 0
        mismatches = 0
        for i in range(len(writes) - 1):
            w1 = writes[i]
            w2 = writes[i + 1]
            if len(w1) == 5 and w1[0] == 0x55 and w1[1] == 0xAA and w1[2] == 0x5A:
                expected_len = w1[3]
                actual_len = len(w2)
                if expected_len == actual_len:
                    matches += 1
                else:
                    mismatches += 1
                    if mismatches <= 3:
                        print(f"  MISMATCH at {i}: header says {expected_len}, data is {actual_len}")
        
        print(f"  Matches: {matches}, Mismatches: {mismatches}")
        
        if matches > 0 and mismatches == 0:
            print("\n  ✓✓✓ CONFIRMED: 55 AA 5A [len] 00 is the frame header!")
            print("  Wire format: [55 AA 5A len 00] + [len bytes of encrypted data]")
            print("  These are sent as TWO separate serial.write() calls")
            print("\n  To send our DLL packets, we need to:")
            print("  1. Generate DLL packet (MgL format)")
            print("  2. Strip MgL header (first 5 bytes)")
            print("  3. The remaining bytes = encrypted payload")
            print("  4. Send: 55 AA 5A [payload_len] 00")
            print("  5. Send: [payload]")
            print("  OR: the .NET code re-encrypts differently")
    
    # Now the big question: what IS the encrypted data in the wire captures?
    # DLL: [MgL][len_hi][len_lo][IV 6B][encrypted cmd+attr+rand+rand+data]
    # Wire: [55 AA 5A][len][00][???encrypted???]
    #
    # If .NET strips MgL header: wire_data = DLL[5:] = [IV 6B][encrypted...]
    # For get_scen_package: DLL output = 21 bytes, DLL[5:] = 16 bytes
    # But our wire data packets are 238-245 bytes, not 16.
    # So .NET does NOT use get_scen_package for these!
    
    # For get_large_screendata_package: DLL output = 1515 bytes
    # DLL[5:] = 1510 bytes. But wire = 238-245.
    # 1510 ≠ 238. So .NET doesn't just strip the header.
    
    # WAIT: maybe the actual app doesn't use get_large_screendata_package either!
    # Maybe it uses the .NET LProtocolSyncRGB.GenProtocolSyncRGB() which builds
    # its own packet format and encrypts with .NET code, not the DLL.
    #
    # In that case, the DLL is only used for firmware updates or other features,
    # and the actual color/screen protocol is implemented in .NET (obfuscated).
    
    # But we KNOW the protocol works with 55 AA 5A framing.
    # We KNOW cmd=6 (CTRL_SYNC_RGB) is for screen mirroring.
    # 
    # Let me check: in unpack_package output, the plaintext was:
    # 00 06 02 00 00 64 ff 00 00 01
    # Where 06 is at position 1... could this be the frame byte[3]?
    # No, 06 in unpack = data_len field in internal format.
    
    # Let me think about this differently.
    # The controller understands 55 AA 5A [cmd] [attr] format (it responds to heartbeats).
    # The data packets DON'T have 55 AA 5A.
    # But byte[3] of the preceding "heartbeat" = length of following data.
    # 
    # So the full wire packet is: 55 AA 5A [len] 00 [len bytes of payload]
    # And the payload is encrypted.
    #
    # The question is: what encryption does the payload use?
    # From our crypto analysis: 3-byte XOR (approximately).
    # Let's check if the DLL cipher with runtime table matches the wire captures
    # when applied to the payload portion.
    
    print("\n" + "=" * 60)
    print("Testing DLL cipher on wire payload")
    print("=" * 60)
    
    from cipher_table_runtime import CIPHER_TABLE
    
    def rotate_iv_left_1bit(iv6):
        val = int.from_bytes(iv6, 'little')
        val = ((val << 1) | (val >> 47)) & ((1 << 48) - 1)
        return val.to_bytes(6, 'little')
    
    def decrypt(data, iv6):
        iv = bytearray(iv6)
        result = bytearray(data)
        table_idx = 0
        iv_idx = 0
        for i in range(len(result)):
            result[i] ^= CIPHER_TABLE[table_idx] ^ iv[iv_idx]
            table_idx = (table_idx + 1) % len(CIPHER_TABLE)
            iv_idx += 1
            if iv_idx >= 6:
                iv = bytearray(rotate_iv_left_1bit(bytes(iv)))
                iv_idx = 0
        return bytes(result)
    
    # Try: wire payload = [IV 6B][encrypted data]
    # Or: wire payload has no explicit IV and uses something else
    
    # Get paired writes (header + data)
    if writes:
        pairs = []
        for i in range(len(writes) - 1):
            w1 = writes[i]
            w2 = writes[i + 1]
            if (len(w1) == 5 and w1[0] == 0x55 and w1[1] == 0xAA 
                and w1[2] == 0x5A and w1[3] == len(w2)):
                pairs.append((w1, w2))
        
        print(f"\nFound {len(pairs)} header+data pairs")
        
        for pidx, (hdr, payload) in enumerate(pairs[:3]):
            print(f"\n  Pair {pidx}: hdr=[{hdr.hex()}] payload_len={len(payload)}")
            print(f"  Payload[0:20]: {' '.join(f'{b:02x}' for b in payload[:20])}")
            
            # Try: first 6 bytes of payload = IV
            iv = payload[:6]
            enc = payload[6:]
            plain = decrypt(enc, iv)
            print(f"  Try IV=payload[0:6]: plain[0:15]=[{' '.join(f'{b:02x}' for b in plain[:15])}]")
            
            # Check for known plaintext patterns
            # cmd=1,attr=1 for screen data
            if plain[0] in [1, 2, 5, 6] and plain[1] in [0, 1]:
                print(f"    *** cmd={plain[0]} attr={plain[1]} — POSSIBLE MATCH! ***")


if __name__ == "__main__":
    main()
