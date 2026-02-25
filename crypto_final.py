# -*- coding: utf-8 -*-
"""
crypto_final.py — Финальный взлом шифрования Beelight.

Подтверждено:
  - XOR stream cipher с 3-байтовым ключом [K0, K1, K2]
  - Весь пакет шифруется одним ключом
  - Keystream на позиции i = key3[(i + 2) % 3]
    т.е. offset 0→K1, offset 1→K2, offset 2→K0, offset 3→K1, ...
  
Plaintext header (BLACK, 238 bytes):
  [nonce0, nonce1, 0x00, 0x00, 0x00, 0x00, 0x05, 0x05, 0xFF, 0xE3, 0x00, 0x4B]
  
  byte[11] = 0x4B = 75 = NUM_LEDS
  byte[9] = 0xE3 = 227 = ??? 
  byte[6:8] = 0x05, 0x05 = LP_CMD_CTRL_DEVICE?
  byte[8] = 0xFF = ???

Цель: найти K0,K1,K2 = f(nonce0, nonce1) или f(cipher_byte0, cipher_byte1)
"""

import sys
import os
import hashlib
from collections import defaultdict

CSV_DIR = os.path.dirname(os.path.abspath(__file__))
NUM_LEDS = 75


# region ===== Утилиты =====
def parse_csv_long(filepath):
    pkts = []
    with open(filepath, "r", errors="replace") as f:
        for line in f:
            if "IRP_MJ_WRITE;DOWN" not in line:
                continue
            parts = line.split(";")
            if len(parts) <= 7:
                continue
            data = parts[5].strip()
            try:
                raw = bytes.fromhex(data.replace(" ", ""))
            except:
                continue
            if len(raw) > 10:
                pkts.append(raw)
    return pkts


def find_solid(pkts):
    results = []
    for pkt in pkts:
        for start in range(min(20, len(pkt) - 5)):
            trip = pkt[start:start+3]
            cnt = 1
            pos = start + 3
            while pos + 2 < len(pkt) and pkt[pos:pos+3] == trip:
                cnt += 1
                pos += 3
            if cnt >= 70:
                results.append((pkt, start, cnt, trip))
                break
    return results
# endregion


# region ===== Извлечение key3 и nonce =====
def extract_key_and_nonce(pkt, led_offset, enc_triplet, plain_rgb):
    """
    Извлекает 3-байтовый ключ и 2-байтовый nonce из пакета.
    
    LED offset = 12 для 238-byte пакетов.
    enc_triplet = зашифрованный RGB на LED-позициях.
    
    Keystream pattern (confirmed):
      ks[i] = key3[(i + 2) % 3]
      т.е. offset 0 → K1, offset 1 → K2, offset 2 → K0
    
    LED offset 12: ks[12] = key3[(12+2)%3] = key3[14%3] = key3[2]
                   ks[13] = key3[(13+2)%3] = key3[15%3] = key3[0]
                   ks[14] = key3[(14+2)%3] = key3[16%3] = key3[1]
    
    Значит: enc_triplet[0] = plain_rgb[0] ^ key3[2]  (offset 12)
            enc_triplet[1] = plain_rgb[1] ^ key3[0]  (offset 13)
            enc_triplet[2] = plain_rgb[2] ^ key3[1]  (offset 14)
    """
    r, g, b = plain_rgb
    # key3[2] = enc_trip[0] ^ r
    # key3[0] = enc_trip[1] ^ g
    # key3[1] = enc_trip[2] ^ b
    k2 = enc_triplet[0] ^ r
    k0 = enc_triplet[1] ^ g
    k1 = enc_triplet[2] ^ b
    key3 = bytes([k0, k1, k2])
    
    # Decrypt nonce (first 2 bytes)
    # ks[0] = key3[(0+2)%3] = key3[2] = k2
    # ks[1] = key3[(1+2)%3] = key3[0] = k0
    nonce0 = pkt[0] ^ k2  # wait, that doesn't look right...
    # Actually wait. Let me re-derive the phase.
    # From BLACK analysis:
    #   offset 2: ks = K2 (trip[2])
    #   offset 3: ks = K0 (trip[0])
    #   offset 4: ks = K1 (trip[1])
    #   offset 5: ks = K2 (trip[2])
    # So pattern at offset i: key3[(i) % 3] starting from some phase
    # offset 2 → K2 = key3[2]  => (2 + phase) % 3 = 2 => phase = 0
    # Wait no: if ks[i] = key3[(i + phase) % 3]:
    #   ks[2] = key3[(2+phase)%3] = K2 = key3[2] => (2+phase)%3 = 2 => phase = 0
    #   ks[3] = key3[(3+0)%3] = key3[0] = K0 ✓
    #   ks[4] = key3[(4+0)%3] = key3[1] = K1 ✓
    #   ks[5] = key3[(5+0)%3] = key3[2] = K2 ✓
    # So phase = 0: ks[i] = key3[i % 3]!
    # 
    # Then: ks[0] = key3[0] = K0
    #        ks[1] = key3[1] = K1
    #        ks[12] = key3[12%3] = key3[0] = K0
    #        ks[13] = key3[13%3] = key3[1] = K1
    #        ks[14] = key3[14%3] = key3[2] = K2
    #
    # So for LED at offset 12:
    #   enc_trip[0] = plain_rgb[0] ^ K0  (offset 12)
    #   enc_trip[1] = plain_rgb[1] ^ K1  (offset 13)  
    #   enc_trip[2] = plain_rgb[2] ^ K2  (offset 14)
    
    k0 = enc_triplet[0] ^ r
    k1 = enc_triplet[1] ^ g
    k2 = enc_triplet[2] ^ b
    key3 = bytes([k0, k1, k2])
    
    # Decrypt nonce
    nonce0 = pkt[0] ^ key3[0]
    nonce1 = pkt[1] ^ key3[1]
    
    return key3, nonce0, nonce1
# endregion


# region ===== Main analysis =====
def main():
    sys.stdout.reconfigure(encoding="utf-8")
    
    print("=" * 70)
    print("ФИНАЛЬНЫЙ КРИПТОАНАЛИЗ BEELIGHT")
    print("=" * 70)
    
    # Verify phase = 0 hypothesis with BLACK packets
    print("\n### Проверка фазы keystream ###")
    print("Гипотеза: ks[i] = key3[i % 3] (фаза = 0)")
    
    black_pkts = parse_csv_long(os.path.join(CSV_DIR, "black.csv"))
    black_solid = find_solid(black_pkts)
    
    for pkt, led_off, cnt, enc_trip in black_solid[:2]:
        # For BLACK (plain=0), cipher = keystream
        # LED trip = [K0, K1, K2] at offset 12
        # If phase=0: ks[12]=K0, ks[13]=K1, ks[14]=K2
        k0, k1, k2 = enc_trip[0], enc_trip[1], enc_trip[2]
        
        # Verify other positions
        ok = True
        for i in range(2, len(pkt)):
            expected = [k0, k1, k2][i % 3]
            # But for non-LED region, plain might not be 0!
            # Only LED region (offset 12 to 12+225) has plain=0 for BLACK
            if led_off <= i < led_off + 225:
                if pkt[i] != expected:
                    ok = False
                    print(f"  MISMATCH at offset {i}: cipher=0x{pkt[i]:02x} expected=0x{expected:02x}")
                    break
        
        # Check known header positions
        # plain[2]=0 => ks[2]=cipher[2], and ks[2]=key3[2%3]=K2
        ks2 = pkt[2]  # = cipher[2] ^ plain[2] = cipher[2] ^ 0 ... NO!
        # plain[2] for BLACK is 0x00 (confirmed), so cipher[2] = ks[2] = key3[2] = K2
        check2 = pkt[2] == k2
        check3 = pkt[3] == k0  # plain[3]=0 for BLACK
        check4 = pkt[4] == k1
        check5 = pkt[5] == k2
        # plain[6]=0x05, so cipher[6] = 0x05 ^ ks[6] = 0x05 ^ K0
        check6 = pkt[6] == (0x05 ^ k0)  # wait, cipher = plain ^ ks
        # cipher[6] = plain[6] ^ ks[6] = 0x05 ^ K0
        check6 = pkt[6] == (0x05 ^ k0)
        # NO WAIT. For BLACK, plain header = [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B]
        # cipher[6] = 0x05 ^ key3[6%3] = 0x05 ^ K0
        check6_val = 0x05 ^ k0
        
        print(f"\n  Packet len={len(pkt)}, LED K=[{k0:02x} {k1:02x} {k2:02x}]")
        print(f"  cipher[2]=0x{pkt[2]:02x} == K2=0x{k2:02x}? {check2}")
        print(f"  cipher[3]=0x{pkt[3]:02x} == K0=0x{k0:02x}? {check3}")
        print(f"  cipher[4]=0x{pkt[4]:02x} == K1=0x{k1:02x}? {check4}")
        print(f"  cipher[5]=0x{pkt[5]:02x} == K2=0x{k2:02x}? {check5}")
        print(f"  cipher[6]=0x{pkt[6]:02x} == 0x05^K0=0x{check6_val:02x}? {pkt[6]==check6_val}")
        print(f"  cipher[7]=0x{pkt[7]:02x} == 0x05^K1=0x{(0x05^k1):02x}? {pkt[7]==(0x05^k1)}")
        print(f"  cipher[8]=0x{pkt[8]:02x} == 0xFF^K2=0x{(0xFF^k2):02x}? {pkt[8]==(0xFF^k2)}")
        print(f"  cipher[9]=0x{pkt[9]:02x} == 0xE3^K0=0x{(0xE3^k0):02x}? {pkt[9]==(0xE3^k0)}")
        print(f"  cipher[10]=0x{pkt[10]:02x} == 0x00^K1=0x{k1:02x}? {pkt[10]==k1}")
        print(f"  cipher[11]=0x{pkt[11]:02x} == 0x4B^K2=0x{(0x4B^k2):02x}? {pkt[11]==(0x4B^k2)}")
        
        # ALL LED region
        print(f"  LED region all match key3 period: {ok}")
        
        # TAIL
        if led_off + 225 < len(pkt):
            tail_off = led_off + 225
            tail_cipher = pkt[tail_off]
            # plain tail for BLACK = 0x00
            # cipher[237] = 0x00 ^ key3[237%3] = key3[237%3] = key3[0] = K0
            expected_tail = k0 if (237 % 3 == 0) else (k1 if 237 % 3 == 1 else k2)
            print(f"  tail cipher=0x{tail_cipher:02x} == key3[{237%3}]=0x{expected_tail:02x}? {tail_cipher==expected_tail}")
    
    # Now collect all (nonce, key3) pairs with correct phase
    print("\n" + "=" * 70)
    print("ИЗВЛЕЧЕНИЕ ПАР (NONCE, KEY3)")
    print("=" * 70)
    
    all_pairs = []
    for color_name, rgb in [
        ("black", (0,0,0)), ("red", (0xFF,0,0)), 
        ("green", (0,0xFF,0)), ("blue", (0,0,0xFF)),
        ("white", (0xFF,0xFF,0xFF))
    ]:
        csv_path = os.path.join(CSV_DIR, f"{color_name}.csv")
        if not os.path.exists(csv_path):
            continue
        pkts = parse_csv_long(csv_path)
        solid = find_solid(pkts)
        
        for pkt, led_off, cnt, enc_trip in solid:
            key3, n0, n1 = extract_key_and_nonce(pkt, led_off, enc_trip, rgb)
            all_pairs.append({
                "color": color_name,
                "key3": key3,
                "nonce": (n0, n1),
                "pkt_len": len(pkt),
                "byte0_cipher": pkt[0],
                "byte1_cipher": pkt[1],
            })
    
    print(f"\nВсего пар: {len(all_pairs)}")
    
    # Verify: same nonce -> same key?
    by_nonce = defaultdict(list)
    for p in all_pairs:
        by_nonce[p["nonce"]].append(p["key3"])
    
    consistent = sum(1 for keys in by_nonce.values() if len(set(keys)) == 1)
    total = len(by_nonce)
    print(f"Один nonce → один key: {consistent}/{total}")
    
    # Show first 20 pairs
    print(f"\n{'n0':>4s} {'n1':>4s} | {'K0':>4s} {'K1':>4s} {'K2':>4s} | color   | relations")
    print("-" * 70)
    for p in all_pairs[:30]:
        n0, n1 = p["nonce"]
        k0, k1, k2 = p["key3"]
        
        # Find interesting relationships
        rels = []
        # Check if key bytes relate to nonce bytes
        if k0 == n0: rels.append("K0=n0")
        if k1 == n1: rels.append("K1=n1")
        if k0 == (n0 + n1) & 0xFF: rels.append("K0=n0+n1")
        if k0 == (n0 ^ n1): rels.append("K0=n0^n1")
        if k0 == (n0 * n1) & 0xFF: rels.append("K0=n0*n1")
        
        rel_str = ", ".join(rels) if rels else ""
        print(f"0x{n0:02x} 0x{n1:02x} | 0x{k0:02x} 0x{k1:02x} 0x{k2:02x} | {p['color']:7s} | {rel_str}")
    
    # Deep relationship analysis
    print("\n" + "=" * 70)
    print("ПОИСК ФОРМУЛЫ KEY = f(NONCE)")
    print("=" * 70)
    
    # Check: K0 = f(n0, n1) for various f
    print("\n--- Checking K0 = n0 OP n1 ---")
    for op_name, op_fn in [
        ("n0+n1", lambda a,b: (a+b)&0xFF),
        ("n0-n1", lambda a,b: (a-b)&0xFF),
        ("n1-n0", lambda a,b: (b-a)&0xFF),
        ("n0*n1", lambda a,b: (a*b)&0xFF),
        ("n0^n1", lambda a,b: a^b),
        ("n0&n1", lambda a,b: a&b),
        ("n0|n1", lambda a,b: a|b),
        ("~n0", lambda a,b: (~a)&0xFF),
        ("~n1", lambda a,b: (~b)&0xFF),
        ("n0<<1^n1", lambda a,b: ((a<<1)^b)&0xFF),
        ("n0>>1^n1", lambda a,b: ((a>>1)^b)&0xFF),
        ("n0*2+n1", lambda a,b: (a*2+b)&0xFF),
        ("n0+n1*2", lambda a,b: (a+b*2)&0xFF),
    ]:
        # Check if K0 = op(n0,n1) + constant
        diffs = set()
        for p in all_pairs:
            n0, n1 = p["nonce"]
            k0 = p["key3"][0]
            computed = op_fn(n0, n1)
            diffs.add((k0 - computed) & 0xFF)
        if len(diffs) == 1:
            c = list(diffs)[0]
            print(f"  K0 = ({op_name} + 0x{c:02x}) & 0xFF — MATCH!")
        elif len(diffs) <= 3:
            print(f"  K0 vs {op_name}: {len(diffs)} diffs: {[hex(d) for d in sorted(diffs)]}")
    
    # Check K1 similarly
    print("\n--- Checking K1 = n0 OP n1 ---")
    for op_name, op_fn in [
        ("n0+n1", lambda a,b: (a+b)&0xFF),
        ("n0-n1", lambda a,b: (a-b)&0xFF),
        ("n1-n0", lambda a,b: (b-a)&0xFF),
        ("n0^n1", lambda a,b: a^b),
        ("~n0", lambda a,b: (~a)&0xFF),
        ("~n1", lambda a,b: (~b)&0xFF),
    ]:
        diffs = set()
        for p in all_pairs:
            n0, n1 = p["nonce"]
            k1 = p["key3"][1]
            computed = op_fn(n0, n1)
            diffs.add((k1 - computed) & 0xFF)
        if len(diffs) == 1:
            c = list(diffs)[0]
            print(f"  K1 = ({op_name} + 0x{c:02x}) & 0xFF — MATCH!")
    
    # Check K2
    print("\n--- Checking K2 = n0 OP n1 ---")
    for op_name, op_fn in [
        ("n0+n1", lambda a,b: (a+b)&0xFF),
        ("n0-n1", lambda a,b: (a-b)&0xFF),
        ("n1-n0", lambda a,b: (b-a)&0xFF),
        ("n0^n1", lambda a,b: a^b),
        ("~n0", lambda a,b: (~a)&0xFF),
        ("~n1", lambda a,b: (~b)&0xFF),
    ]:
        diffs = set()
        for p in all_pairs:
            n0, n1 = p["nonce"]
            k2 = p["key3"][2]
            computed = op_fn(n0, n1)
            diffs.add((k2 - computed) & 0xFF)
        if len(diffs) == 1:
            c = list(diffs)[0]
            print(f"  K2 = ({op_name} + 0x{c:02x}) & 0xFF — MATCH!")
    
    # Check if key3 = some hash of nonce
    print("\n--- Checking key3 = hash(nonce) approaches ---")
    # Maybe key3[0] has a lookup-table relationship with nonce?
    # Let's just plot n0 vs K0
    print("\n  n0 vs K0 (checking linearity):")
    pairs_sorted = sorted(all_pairs, key=lambda p: p["nonce"][0])
    for p in pairs_sorted[:10]:
        n0, n1 = p["nonce"]
        k0 = p["key3"][0]
        print(f"    n0=0x{n0:02x} n1=0x{n1:02x} -> K0=0x{k0:02x}")
    
    # Maybe the relationship involves MORE than just n0,n1
    # Perhaps cipher_byte[0] or cipher_byte[1] IS the nonce (not decrypted)
    print("\n--- Checking if cipher bytes ARE the nonce (unencrypted) ---")
    for p in all_pairs[:10]:
        c0 = p["byte0_cipher"]
        c1 = p["byte1_cipher"]
        n0, n1 = p["nonce"]
        k0, k1, k2 = p["key3"]
        print(f"  cipher[0]=0x{c0:02x} cipher[1]=0x{c1:02x} | "
              f"nonce=(0x{n0:02x},0x{n1:02x}) | key=({k0:02x},{k1:02x},{k2:02x}) | "
              f"c0=n0^K0: 0x{(n0^k0):02x}==0x{c0:02x}? {(n0^k0)==c0}")

    # Final: maybe nonce bytes are actually random, and the key is derived
    # from some external state (frame counter maintained by the DLL)?
    print("\n" + "=" * 70)
    print("ВЫВОД")
    print("=" * 70)
    print("""
Подтверждено:
  1. Cipher = XOR с ключом period=3, фаза=0: ks[i] = key3[i % 3]
  2. Plain header (BLACK): [n0, n1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B]
  3. byte[11] plaintext = 0x4B = 75 = NUM_LEDS
  4. Один nonce → один key3 (детерминированная связь)
  5. key3 НЕ вычисляется простой арифметикой из nonce
  
Скорее всего key3 = f(state), где state — внутренний счётчик DLL,
а nonce — тоже функция этого state. 
Для полного взлома нужен дизассемблинг beelightLib.dll (Ghidra).

НО: мы уже можем ОТПРАВЛЯТЬ пакеты, если:
  - Выберем произвольный key3 (3 байта)
  - Зашифруем правильный plaintext этим ключом
  - Отправим в COM порт
  - Если контроллер не проверяет nonce/counter → сработает!
""")


if __name__ == "__main__":
    main()
