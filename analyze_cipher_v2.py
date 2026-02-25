# -*- coding: utf-8 -*-
"""
analyze_cipher_v2.py — Пересмотр формата пакета.

Из FUN_10001880:
  1. Строится plaintext:
     [0-1]  = 0x674D (magic, LE)
     [2]    = (data_len + 10) & 0xFF
     [3]    = (data_len + 10) >> 8
     [4]    = not set explicitly (probably 0 or from stack)
     [5-10] = 6 random bytes (IV)
     [11]   = cmd
     [12]   = attr
     [13]   = rand
     [14]   = rand
     [15..] = data

  2. Encryption starts at offset 0x0b (byte 11), length = data_len + 4
     So bytes [0-10] are plaintext, bytes [11+] are encrypted

  3. BUT: in our captures byte[1] is CONSTANT per length
     238-byte: byte[1]=0x32=50
     If bytes[0-10] are plaintext, then byte[1] should be part of magic 0x674D
     0x674D LE = [0x4D, 0x67] — byte[1] should be 0x67
     But captured byte[1] = 0x32... NOT 0x67

  WAIT: *param_1 = 0x674d means we're writing a WORD (2 bytes) at param_1
  param_1 is undefined2* — so it writes 0x4D at [0] and 0x67 at [1] (little-endian)
  Then *(undefined1 *)(param_1 + 1) = 0x4C means writing 0x4C at byte offset 2
  (param_1+1 when param_1 is uint16* = byte offset 2)

  So actual layout is:
  [0] = 0x4D ('M')
  [1] = 0x67 ('g')  
  [2] = 0x4C ('L')
  [3] = (data_len+10) low
  [4] = (data_len+10) high

  That's "MgL" header!

  Encryption starts at offset 0x0b = 11.
  So bytes [0..10] are plaintext on the wire.
  
  But captured byte[1] = 0x32, not 0x67... 
  
  UNLESS: there's ANOTHER layer of processing between FUN_10001880 and the serial port.
  Or: the .NET code does additional transformation.
  
  Let me check: does the .NET side do something before sending?
  
  Actually, looking at captures more carefully:
  - byte[1] for 238 = 0x32
  - byte[1] for 239 = 0x35
  - byte[1] for 240 = 0x34
  - byte[1] for 241 = 0x37
  - byte[1] for 242 = 0x36
  - byte[1] for 243 = 0x39
  - byte[1] for 244 = 0x38
  - byte[1] for 245 = 0x3b
  
  0x32 = 50, 0x35 = 53, 0x34 = 52, 0x37 = 55, 0x36 = 54, 0x39 = 57, 0x38 = 56, 0x3b = 59
  
  238-byte packet: magic says length should be data_len + 10 + 5 (header overhead)
  
  Hmm wait, let me reconsider. Maybe the .NET wrapper adds 55 AA 5A prefix
  and what we see in captures is: 55 AA 5A [FUN_10001880_output]
  
  But heartbeats are also 55 AA 5A xx 00... and data packets DON'T start with 55 AA 5A.
  
  So data packets go through a different path. Let me just try: what if the ENTIRE
  packet (all bytes) is encrypted, including bytes 0-10?
  
  If the entire packet is encrypted with the same scheme (table + IV + rotate),
  we'd need to know the IV to decrypt. But the IV IS inside the packet...
  
  Circular dependency. Unless the IV is transmitted separately or derived differently.
  
  Actually - let me re-read FUN_10001880 more carefully.
  The IV bytes ARE written to the packet BEFORE encryption:
    *(byte *)((int)param_1 + iVar7 + 5) = (byte)iVar4;
  
  And encryption starts at offset 0x0b:
    pbVar8 = (byte *)((int)param_1 + 0xb);
  
  So bytes [5-10] (IV) are plaintext in the output!
  And bytes [0-4] (magic + length) are also plaintext!
  Only [11+] is encrypted.
  
  So bytes[0..10] should match:
    [0] = 0x4D
    [1] = 0x67
    [2] = 0x4C
    ...
  
  But they DON'T in the captures. That means there's an additional layer.

  Let me check: maybe the BEELIGHT APP wraps packets differently than the DLL.
  The .NET code calls get_large_screendata_package, not get_scen_package for LED data.
  
  Let me look at get_large_screendata_package signature.
"""
import sys
import os

# Just print the analysis and conclusions
def main():
    sys.stdout.reconfigure(encoding="utf-8")
    
    # Key insight: look at byte[1] pattern
    # len=238 -> b1=0x32, len=239 -> b1=0x35, len=240 -> b1=0x34
    # len=241 -> b1=0x37, len=242 -> b1=0x36, len=243 -> b1=0x39
    # len=244 -> b1=0x38, len=245 -> b1=0x3b
    
    mapping = {
        238: 0x32, 239: 0x35, 240: 0x34, 241: 0x37,
        242: 0x36, 243: 0x39, 244: 0x38, 245: 0x3b
    }
    
    print("Length -> byte[1] mapping:")
    for length, b1 in sorted(mapping.items()):
        # XOR with various values
        print(f"  len={length} b1=0x{b1:02x}({b1}) "
              f"b1^0x67=0x{b1^0x67:02x} b1^0x4D=0x{b1^0x4D:02x} "
              f"b1^len_low=0x{b1^(length&0xFF):02x}")
    
    print()
    
    # Check: b1 XOR something = constant?
    # len=238=0xEE: b1=0x32, 0x32^0xEE=0xDC
    # len=239=0xEF: b1=0x35, 0x35^0xEF=0xDA
    # Nope, not constant
    
    # Check: b1 relates to (length - some_offset)?
    # 238 -> 50, 239 -> 53, 240 -> 52, 241 -> 55
    # Differences: 238->50, 239->53 (diff=3), 240->52 (diff=-1), 241->55 (diff=3)
    # Pattern: 50, 53, 52, 55, 54, 57, 56, 59
    # That's: 50, 53, 52, 55, 54, 57, 56, 59
    # In binary: 110010, 110101, 110100, 110111, 110110, 111001, 111000, 111011
    
    print("byte[1] values in binary:")
    for length, b1 in sorted(mapping.items()):
        # Lower 3 bits of length and b1
        l3 = length & 7
        b3 = b1 & 7
        print(f"  len={length} ({length&7}) b1=0x{b1:02x} ({b1&7}) "
              f"len_bin={length:08b} b1_bin={b1:08b}")
    
    print()
    
    # 238 = 0b11101110, b1=0x32=0b00110010
    # 239 = 0b11101111, b1=0x35=0b00110101  
    # 240 = 0b11110000, b1=0x34=0b00110100
    # 241 = 0b11110001, b1=0x37=0b00110111
    # 242 = 0b11110010, b1=0x36=0b00110110
    # 243 = 0b11110011, b1=0x39=0b00111001
    # 244 = 0b11110100, b1=0x38=0b00111000
    # 245 = 0b11110101, b1=0x3b=0b00111011
    
    # b1 low nibble: 2,5,4,7,6,9,8,b = 2,5,4,7,6,9,8,11
    # That's: +3, -1, +3, -1, +3, -1, +3
    # OR: pairs (2,5), (4,7), (6,9), (8,11)
    # Each pair: low=even, high=low+3
    
    # This could be: b1 = 0x30 + some_function_of(len)
    # For 238: 0x32 = 0x30 + 2
    # For 239: 0x35 = 0x30 + 5  
    # For 240: 0x34 = 0x30 + 4
    # For 241: 0x37 = 0x30 + 7
    
    # Digit pattern: 2,5,4,7,6,9,8,B
    # These are ASCII digits! 0x32='2', 0x35='5', 0x34='4', 0x37='7'...
    # These spell: "2", "5", "4", "7", "6", "9", "8", ";"
    
    # Wait... packet_size - 5 (header overhead) = payload
    # 238-5=233, 239-5=234...
    # FUN_10001880 param_5 = data_len, total = data_len + 15 (5 hdr + 6 IV + 4 overhead)
    # So data_len = total_len - 15 = 238-15 = 223
    # length field = data_len + 10 = 233 = 0xE9
    # But b1 = 0x32... not 0xE9
    
    # Actually for get_large_screendata_package the structure may be different!
    # Let me check what FUN_10001880 actually writes as length
    
    # *(char *)((int)param_1 + 3) = (char)((uint)(param_5 + 10) >> 8);
    # *(char *)(param_1 + 2) = (char)param_5 + '\n';  // param_5 + 10
    
    # For get_scen_package: param_5=6, so length = 16, fits in 1 byte
    # For screen data: param_5=225 (75 LEDs × 3), length = 235 = 0xEB
    # byte[3] = 0xEB >> 8 = 0x00
    # byte[2+...wait, param_1+2 is byte offset 4 (param_1 is uint16*)
    
    # WAIT. param_1 is undefined2* (2-byte pointer)
    # param_1 + 2 = adds 2 * sizeof(undefined2) = 4 bytes
    # So *(char *)(param_1 + 2) writes at byte offset 4!
    # And *(char *)((int)param_1 + 3) writes at byte offset 3!
    
    # So:
    # [0-1] = 0x674D (via *param_1 = 0x674D) -> [0]=0x4D, [1]=0x67
    # [2]   = 0x4C (via *(undefined1*)(param_1+1) = 0x4C) 
    #         param_1+1 = byte_offset 2 (pointer arithmetic on uint16*)
    # [3]   = (param_5 + 10) >> 8
    # [4]   = (param_5 + 10) & 0xFF
    
    # Hmm, actually: *(char *)((int)param_1 + 3) uses (int)param_1, 
    # which casts to byte pointer! So it's byte offset 3.
    # And *(char *)(param_1 + 2) — param_1 is undefined2*, 
    # so param_1+2 is byte offset 4.
    
    # Final layout:
    # [0] = 0x4D
    # [1] = 0x67  
    # [2] = 0x4C
    # [3] = high byte of (param_5 + 10)
    # [4] = low byte of (param_5 + 10)
    
    # For 238-byte captured packet, what's the DLL param_5?
    # DLL outputs: 5 header + 6 IV + param_5 + 4 = param_5 + 15
    # But we don't know the total DLL output size for a 238-capture packet
    # Because .NET might prepend/append bytes
    
    # Let's check what get_large_screendata_package does
    print("CONCLUSION: Need to see get_large_screendata_package pseudo-C")
    print("This is the function that generates screen mirroring packets (our captures)")
    print()
    print("Also need to check: does .NET code transform DLL output before serial write?")
    print("The BeelightProtocol class may add framing bytes")

if __name__ == "__main__":
    main()
