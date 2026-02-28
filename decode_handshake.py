# -*- coding: utf-8 -*-
"""
decode_handshake.py - Decode the handshake/startup sequence.

From red_full.csv analysis:
  - First 100 ops are IOCTLs (port setup)
  - Then small packets: handshake + config exchange
  - Then large 238-245 packets: color data

Goal: extract and decrypt the small packets, identify the mode-switch command.

LProtocolBase commands:
  LP_CMD_HEARTBEAT = 0
  LP_CMD_FIRM = 1
  LP_CMD_SYNCSTATUS = 2
  LP_CMD_SYNCCONFIG = 3
  LP_CMD_OTA = 4
  LP_CMD_CTRL_DEVICE = 5
  LP_CMD_CTRL_SYNC_RGB = 6
  LP_CMD_CTRL_LOG = 26

  LP_ATTR_REQ = 0
  LP_ATTR_ACK = 1

  LP_CTRL_SWITCHER = 1
  LP_CTRL_BRIGHT = 2
  LP_CTRL_TEMPER = 3
  LP_CTRL_COLOR = 4
  LP_CTRL_RGB_TRANSFER = 5
  LP_CTRL_WORKMODE = 6

  LP_WK_MODE_PC = 0
"""
import sys, os

CSV_DIR = os.path.dirname(os.path.abspath(__file__))


def parse_ops(filepath):
    """Parse all WRITE/READ ops in order, with timestamps if available."""
    ops = []
    with open(filepath, "r", errors="replace") as f:
        for line_no, line in enumerate(f):
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            direction = ""
            if "IRP_MJ_WRITE" in line and "DOWN" in line:
                direction = "W"
            elif "IRP_MJ_READ" in line and "UP" in line:
                direction = "R"
            else:
                continue
            data_str = parts[5].strip() if len(parts) > 5 else ""
            try:
                raw = bytes.fromhex(data_str.replace(" ", "")) if data_str else b""
            except ValueError:
                raw = b""
            if not raw:
                continue
            ops.append((line_no, direction, raw))
    return ops


def reassemble_frames(ops):
    """Pair frame headers with their data payloads (two consecutive writes)."""
    frames = []
    i = 0
    while i < len(ops):
        ln, d, raw = ops[i]
        
        if d == "R":
            # Parse READ as possibly multiple 55AA5A packets
            j = 0
            while j < len(raw) - 4:
                if raw[j] == 0x55 and raw[j+1] == 0xAA and raw[j+2] == 0x5A:
                    plen = raw[j+3]
                    payload = raw[j+5:j+5+plen]
                    attr = raw[j+4]
                    frames.append({
                        'line': ln, 'dir': 'R', 'attr': attr,
                        'payload_len': plen, 'payload': payload,
                        'hdr_raw': raw[j:j+5]
                    })
                    j += 5 + plen
                else:
                    j += 1
            i += 1
            continue
        
        # WRITE: check if this is a frame header
        if d == "W" and len(raw) == 5 and raw[0] == 0x55 and raw[1] == 0xAA and raw[2] == 0x5A:
            plen = raw[3]
            attr = raw[4]
            # Next write should be the data
            if i + 1 < len(ops) and ops[i+1][1] == "W":
                data = ops[i+1][2]
                frames.append({
                    'line': ln, 'dir': 'W', 'attr': attr,
                    'payload_len': plen, 'payload': data,
                    'hdr_raw': raw
                })
                i += 2
                continue
        
        # Single write without frame header (MgL or other)
        if d == "W":
            frames.append({
                'line': ln, 'dir': 'W', 'attr': -1,
                'payload_len': len(raw), 'payload': raw,
                'hdr_raw': b''
            })
        
        i += 1
    
    return frames


def try_decrypt_3xor(cipher, key):
    """Decrypt with 3-byte XOR key."""
    return bytes(c ^ key[i % 3] for i, c in enumerate(cipher))


def analyze_small_packet(payload, direction):
    """Try to guess plaintext of a small encrypted packet.
    
    For a 3-byte XOR cipher, if we know ANY 3 consecutive plaintext bytes 
    at a position divisible by 3, we can recover the key.
    
    Hypothesis for GenFramePackage plaintext format:
    Option A: [nonce0, nonce1, pad0, pad1, attr, cmd, data...]
    Option B: [nonce0, nonce1, attr, cmd, data...]  
    Option C: [attr, cmd, data...] (no nonce in small packets?)
    """
    results = []
    
    if len(payload) < 3:
        return results
    
    # For READ responses (from controller), attr=LP_ATTR_ACK=1
    # For WRITE requests (from app), attr=LP_ATTR_REQ=0
    
    # Try different plaintext assumptions for first bytes
    # Hypothesis: plaintext[2]=0x00 (padding), plaintext[3]=0x00 (padding)
    # Then: key = [cipher[0]^n0, cipher[1]^n1, cipher[2]^0x00]
    # So k2 = cipher[2], and we test if decrypted rest makes sense
    
    for cmd_guess in range(7):  # LP_CMD values 0-6
        for attr_guess in [0, 1]:
            # Try format: [n0, n1, 0x00, 0x00, attr, cmd, data...]
            if len(payload) >= 6:
                k2 = payload[2] ^ 0x00  # pad=0
                k0 = payload[3] ^ 0x00  # pad=0
                k1 = payload[4] ^ attr_guess
                # Verify: cipher[5] ^ k2 should = cmd
                if len(payload) > 5:
                    decoded_cmd = payload[5] ^ k2
                    if decoded_cmd == cmd_guess:
                        key = [k0, k1, k2]
                        plain = try_decrypt_3xor(payload, key)
                        results.append(('A', key, plain, attr_guess, cmd_guess))
            
            # Try format: [n0, n1, attr, cmd, data...]
            if len(payload) >= 4:
                k2 = payload[2] ^ attr_guess
                # cipher[3] ^ k0 = cmd? But k0 = cipher[0]^n0 (unknown)
                # Can't solve without knowing nonce
                pass
    
    return results


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    
    files = [
        ("red_full.csv", os.path.join(CSV_DIR, "red_full.csv")),
        ("surely_full_red.csv", os.path.join(CSV_DIR, "__pycache__", "surely_full_red.csv")),
    ]
    
    for name, path in files:
        if not os.path.exists(path):
            continue
        
        print(f"\n{'='*70}")
        print(f"HANDSHAKE ANALYSIS: {name}")
        print(f"{'='*70}")
        
        ops = parse_ops(path)
        frames = reassemble_frames(ops)
        
        print(f"Total frames: {len(frames)}")
        
        # Find where big packets start
        first_big = None
        for i, f in enumerate(frames):
            if f['payload_len'] >= 238:
                first_big = i
                break
        
        if first_big is None:
            first_big = len(frames)
        
        print(f"First big packet (>=238) at frame index {first_big}")
        print(f"\n--- HANDSHAKE SEQUENCE (frames 0..{first_big-1}) ---\n")
        
        for i, f in enumerate(frames[:first_big + 5]):
            marker = ">>>" if i == first_big else "   "
            d = f['dir']
            plen = f['payload_len']
            payload = f['payload']
            
            hex_data = payload.hex() if payload else "(empty)"
            if len(hex_data) > 80:
                hex_data = hex_data[:80] + "..."
            
            print(f"{marker} [{i:3d}] {d} line={f['line']:5d} len={plen:3d} "
                  f"attr={f['attr']:02x} data={hex_data}")
            
            # Try decryption on small packets
            if plen < 50 and payload:
                guesses = analyze_small_packet(payload, d)
                for fmt, key, plain, attr_g, cmd_g in guesses[:2]:
                    cmd_name = {0:'HEARTBEAT',1:'FIRM',2:'SYNCSTATUS',
                                3:'SYNCCONFIG',4:'OTA',5:'CTRL_DEVICE',
                                6:'CTRL_SYNC_RGB',26:'CTRL_LOG'}.get(cmd_g, f'?{cmd_g}')
                    attr_name = 'REQ' if attr_g == 0 else 'ACK'
                    print(f"          -> fmt={fmt} key=[{key[0]:02x},{key[1]:02x},{key[2]:02x}] "
                          f"attr={attr_name} cmd={cmd_name}")
                    print(f"             plain={plain.hex()}")
        
        # ================================================================
        # Cross-reference: match W/R pairs to identify request-response
        # ================================================================
        print(f"\n--- REQUEST-RESPONSE PAIRING ---")
        handshake = frames[:first_big]
        
        # Group by proximity (W followed by R)
        pairs = []
        i = 0
        while i < len(handshake):
            if handshake[i]['dir'] == 'W':
                # Look for next R
                for j in range(i+1, min(i+10, len(handshake))):
                    if handshake[j]['dir'] == 'R':
                        pairs.append((handshake[i], handshake[j]))
                        break
            i += 1
        
        for w, r in pairs[:10]:
            print(f"\n  W len={w['payload_len']:3d}: {w['payload'].hex()[:40]}")
            print(f"  R len={r['payload_len']:3d}: {r['payload'].hex()[:40]}")
            
            # If both have same length, XOR them to see relationship
            wp = w['payload']
            rp = r['payload']
            if len(wp) == len(rp) and len(wp) > 0:
                xor = bytes(a ^ b for a, b in zip(wp, rp))
                print(f"  XOR:          {xor.hex()[:40]}")
        
        # ================================================================
        # CRITICAL: Compare byte[0] and byte[1] patterns across ALL packets
        # For 3-byte XOR, byte[0] and byte[1] are nonce XOR key.
        # If key is constant for some packets, nonce varies.
        # ================================================================
        print(f"\n--- BYTE PATTERNS IN SMALL PACKETS ---")
        small_w = [f for f in handshake if f['dir'] == 'W' and len(f['payload']) >= 2]
        
        if small_w:
            b0_list = [f'{fr["payload"][0]:02x}' for fr in small_w]
            b1_list = [f'{fr["payload"][1]:02x}' for fr in small_w]
            print(f"  byte[0] values (WRITE): {b0_list}")
            print(f"  byte[1] values (WRITE): {b1_list}")
            
            # Check if byte[1] has the same relationship as big packets
            # In big packets: byte[1] XOR key[1] = nonce1, and nonce1 XOR key1 = 0x32
            # So byte[1] = nonce1 XOR key[1] = (key1 XOR 0x32) XOR key[1]
            # If same key: byte[1] constant? No, nonce varies.
            
            # Check byte[1] values
            b1_vals = [f['payload'][1] for f in small_w]
            print(f"  byte[1] unique: {sorted(set(f'{v:02x}' for v in b1_vals))}")
            
            # For big packets in the same file, check byte[1]
            big_w = [f for f in frames if f['dir'] == 'W' and len(f['payload']) >= 238]
            if big_w:
                big_b1 = [f['payload'][1] for f in big_w[:10]]
                print(f"  byte[1] in big packets: {[f'{v:02x}' for v in big_b1]}")
        
        # ================================================================
        # KEY INSIGHT: The repeated 13-byte writes - these are likely the
        # same command sent multiple times (retries or periodic)
        # ================================================================
        print(f"\n--- REPEATED PATTERNS ---")
        from collections import Counter
        payload_counts = Counter()
        for f in handshake:
            if f['dir'] == 'W':
                payload_counts[f['payload'].hex()] += 1
        
        for hex_val, count in payload_counts.most_common(10):
            if count > 1:
                print(f"  {count}x: len={len(bytes.fromhex(hex_val))} data={hex_val}")
    
    print("\nDONE")


if __name__ == "__main__":
    main()
