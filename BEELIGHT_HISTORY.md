# История анализа протокола Beelight LED

## Цель
Создать Python-контроллер для управления светодиодной лентой Beelight в обход оригинального приложения.

---

## Этапы работы

### 1. Декомпиляция и статический анализ
**Результат**: Обфускация ConfuserEx (Anti-Dump + Control Flow) делает статический анализ C# невозможным.

**Проблемы:**
- `Beelight.exe` защищен ConfuserEx
- ILSpy дампы (`BeelightProtocol_dump2.cs`, `LProtocolBase_dump2.cs`) пустые (методы возвращают `null`)
- Код C# — только UI-обёртка, реальная логика в нативной DLL

**Выводы:**
- C# приложение вызывает `beelightLib.dll` через P/Invoke
- Шифрование и генерация пакетов происходят в нативном коде

---

### 2. Анализ протокола из захватов
**Инструменты**: USBPcap, Wireshark  
**Файлы**: `red_full.csv`, `surely_full_red.csv`, `black.csv`

**Открытия:**
1. **Wire формат**: `55 AA 5A [plen] 00 [payload]`
2. **Шифр**: 3-байтовый XOR с повторяющимся ключом `key3 = [K0, K1, K2]`
3. **Keystream**: `ks[i] = key3[i % 3]` (фаза 0)
4. **Plaintext для solid color** (238 байт):
   ```
   [nonce0, nonce1, 00, 00, 00, 00, 05, 05, FF, E3, 00, 4B(=75 LEDs), R,G,B × 75, 00]
   ```
5. **Извлечение key3** (подтверждено):
   ```python
   key3 = [cipher[3]^0xFF, cipher[4], cipher[5]]
   # где plain[3]=0xFF, plain[4]=0x00, plain[5]=0x00
   ```

**Проблема**: `key3` меняется для каждого пакета и не вычисляется простым PRNG.

---

### 3. Попытки брутфорса PRNG
**Алгоритмы**: C# `Random`, MSVC `rand()`, LCG mod 2^24, MD5

**Результат**: FAILED  
- Ни один алгоритм не дал совпадения с извлечённой последовательностью `key3`
- Seed-ы из timestamps, handshake байтов не подошли

**Вывод**: PRNG либо кастомный, либо использует неизвестный seed/state.

---

### 4. Ghidra анализ beelightLib.dll
**Инструмент**: Ghidra 12.0.3

**Найдено:**
- Экспорты: `get_connect_package`, `get_scen_package`, `get_large_screendata_package`, `unpack_package`
- Функция `get_scen_package` (0x100015b0):
  ```c
  do {
    bVar1 = *(byte *)(DAT_10018000 + uVar2);  // cipher table?
    *(byte *)((int)param_7 + uVar2) = param_4 ^ bVar1;  // XOR
    uVar2 = uVar2 + 1;
  } while (uVar2 < 3);  // 3-byte loop
  ```
- Внутренний формат DLL: `MgL\x00 [plen] [payload]`
- Magic bytes: `4D 67 4C 00` (`MgL\x00`)

**Вывод**: Весь PRNG и шифрование внутри нативной DLL.

---

### 5. Тест self-keyed гипотезы
**Скрипт**: `test_selfkeyed.py`  
**Метод**: Отправка пакетов с произвольным `key3`

**Результат**: FAILED  
- Лента не загорелась
- Контроллер **строго проверяет** `key3`

**Вывод**: Нужен правильный PRNG state sync через handshake.

---

### 6. Решение: DLL как чёрный ящик
**Стратегия**: Использовать `beelightLib.dll` через Python `ctypes` для генерации правильно зашифрованных пакетов.

**Реализация**:
```python
# 1. Загрузка DLL
lib = ctypes.CDLL('beelightLib.dll')

# 2. Вызов функции
buf = ctypes.create_string_buffer(2048)
lib.get_scen_package(scenid, dimmer, R, G, B, power, buf)

# 3. Извлечение payload (убрать MgL\x00 header)
payload = buf.raw[5:end]

# 4. Обёртка в wire protocol
packet = bytes([0x55, 0xAA, 0x5A, len(payload), 0x00]) + payload

# 5. Отправка в COM7 @ 115200 baud
```

**Скрипт**: `test_dll_direct.py`

---

## Этап 7: Диагностика формата DLL (27.02.2026, 16:00)

### Формат вывода DLL (подтверждено):
- Все функции DLL возвращают: `4D 67 4C 00` (MgL header) + payload
- Payload НЕ содержит `55 AA 5A` — wire header нужно добавлять отдельно
- `get_connect_package`: payload = **11 байт**, ret = 4
- `get_scen_package(0,100,255,0,0,1)`: payload = **17 байт**, ret = 1
- `get_eco_setting_package(0)`: payload = **12 байт**, ret = 5
- `get_reset_factory_package`: payload = **11 байт**, ret = 4
- `get_net_udp_scen_package(0,100,255,0,0,1)`: payload = **18 байт**, ret = 1

### Все 16 экспортов beelightLib.dll:
```
get_audiodata_package        get_bottom_start_line
get_connect_package          get_eco_setting_package
get_large_screenbuf_package_length   get_large_screendata_package
get_large_screendata_package_ex      get_middle_screenbuf_package_length
get_middle_screendata_package        get_net_udp_scen_package
get_reset_factory_package    get_scen_package
get_screen_size_setting_package      get_setting_package
get_top_start_line           unpack_package
```

### Критические находки:
- `unpack_package()` **ЗАВИСАЕТ** при вызове с захваченным ответом контроллера
- `get_setting_package(1)` → ACCESS VIOLATION (неправильная сигнатура)
- `get_screen_size_setting_package(75)` → ACCESS VIOLATION
- Buffer sizes: large=1515, middle=465
- top_start_line=0, bottom_start_line=0

---

## Этап 8: Анализ handshake из захвата

### Wire protocol (подтверждено):
- Header и Data отправляются **отдельными** `write()` вызовами
- Header (5 байт): `55 AA 5A [plen] 00`
- Data (plen байт): зашифрованный payload
- Heartbeat/frame sync: `55 AA 5A [0xEE-0xF5] 00` перед каждым color кадром

### Handshake последовательность (из surely_full_red.csv):
```
[0-1]  HDR 55aa5a0800 + DAT[8]   → Response: 118 bytes
[2-3]  HDR 55aa5a0d00 + DAT[13]  → Response: 114 bytes
[4-5]  HDR 55aa5a1000 + DAT[16]  → Response: 40 bytes
[6-7]  HDR 55aa5a0c00 + DAT[12]  → Response: 33 bytes
[8-9]  HDR 55aa5a0800 + DAT[8]   → Response: 39 bytes
[10-11] HDR 55aa5a0a00 + DAT[10] → Response: 39 bytes
[12-13] HDR 55aa5a0e00 + DAT[14] → Response: 35 bytes
[14-29] HDR + DAT[13] повторяется 8 раз (heartbeat/keepalive)
[30-113] Ещё handshake пакеты
```

### Ключевой вывод: Handshake НЕ генерируется DLL!
Размеры payload из захвата: 8, 13, 16, 12, 8, 10, 14, 13
Размеры DLL функций: 11, 12, 17, 18 — **НЕ совпадают**!
→ Handshake генерируется обфусцированным C# кодом в Beelight.exe

---

## Этап 9: Full replay тест (27.02.2026, 16:30)

**Скрипт**: `full_replay_test.py`
**Метод**: Полное воспроизведение ВСЕХ байтов из surely_full_red.csv
**Baud rate**: 500000 (исправлено с 115200!)

**Результаты коммуникации:**
- ✅ Контроллер ответил на ВСЕ 7 handshake пакетов
- ✅ Размеры ответов: 118, 114, 40, 33, 39, 39, 35 bytes
- ✅ 149 color кадров отправлено
- ✅ Финальный ответ получен (416 bytes)

**Визуальный результат**: ⏳ ОЖИДАНИЕ

---

## Текущий статус (27.02.2026, 16:35)

✅ **Wire протокол** — полностью понят (header + data отдельными write)
✅ **Baud rate** — 500000 (не 115200!)
✅ **Шифрование** — 3-byte XOR, формула key3 подтверждена
✅ **DLL формат** — MgL + payload (без 55 AA 5A)
✅ **Все 16 DLL экспортов** — протестированы
✅ **Handshake** — генерируется C# приложением, не DLL
✅ **Full replay** — контроллер отвечает на все пакеты
⏳ **Визуальное подтверждение** — ожидание

---

## Файлы проекта

**Диагностические скрипты:**
- `diag_dll_format.py` — проверка формата DLL вывода
- `diag_exports_safe.py` — тестирование всех 16 экспортов (subprocess isolation)
- `full_replay_test.py` — полный replay захваченного трафика

**Скрипты управления:**
- `test_dll_direct.py` — DLL black-box контроллер (НЕРАБОТАЮЩИЙ: baud 115200)
- `soulight_control.py` — DLL контроллер (baud 500000, НЕ тестировался)
- `debug_serial.py` — диагностика serial порта (baud 500000)
- `handshake_then_dll.py` — replay handshake + DLL packets
- `replay_handshake.py` — replay из CSV

**Аналитические скрипты:**
- `dll_call_32bit.py` — тестирование DLL функций
- `test_send_color.py` — отправка произвольного цвета (random key3)
- `test_selfkeyed.py` — тест self-keyed гипотезы (FAILED)

**Данные:**
- `surely_full_red.csv` — ПОЛНЫЙ захват RED сессии (788 writes, 5 reads)
- `red_full.csv`, `black.csv`, `blue.csv`, etc. — другие захваты
- `GHIDRA_GUIDE.md` — заметки по анализу в Ghidra
- `Новая папка\Dumps\beelightLib.dll` — нативная библиотека (32-bit)

**Технические детали:**
- COM порт: **COM7**
- Baud rate: **500000**
- DLL path: `c:\Users\n0souls\Documents\GitHub\Soulight\Новая папка\Dumps\beelightLib.dll`
- Python: 32-bit (для совместимости с 32-bit DLL)
- Handshake: 114 writes, 7 request-response пар
- Color frame: heartbeat (5 bytes) + encrypted data (238 bytes)

---

## Следующие шаги

1. ✅ Full replay test (surely_full_red.csv)
2. ⏳ Визуальное подтверждение replay
3. Если replay работает:
   - Replay handshake + генерация своих color пакетов
   - Анализ key3 sync после handshake
4. Если replay НЕ работает:
   - Нужен СВЕЖИЙ захват (новая сессия Beelight)
   - Возможно протокол session-bound

---

## Ключевые инсайты

1. **Baud rate = 500000** (не 115200 как было в первом тесте)
2. **Handshake генерируется C# кодом**, не DLL — это главное препятствие
3. **DLL только для шифрования/генерации color data** (scen, screen, audio)
4. **unpack_package зависает** — возможно ожидает специфический формат входа
5. **Header и Data** отправляются ОТДЕЛЬНЫМИ serial write
6. **Replay handshake работает** (контроллер отвечает) — протокол может быть stateless
