# Soulight

Управление LED-лентой Lytmi/Beelight (контроллер на Artery AT32, USB-CDC,
500000 baud) — solid color, per-LED, screen mirroring (ambilight), сцены,
аудио-режимы. PyQt6 GUI, Windows и Linux.

## Запуск

```bash
pip install -r requirements.txt
python -m soulight          # или ./run_soulight.sh (Linux) / run_silent.bat (Windows)
```

## Протокол (LightProtocol)

Wire-формат реконструирован из `Beelight.exe` (de4dot) и подтверждён на
железе — оригинальное приложение не требуется:

```
frame = 55 AA 5A <len u16le> + body
body  = [cksum][pad^0x31][key:pad][attr^key0][cmd^key1][data^key...]
cksum = sum(body[1:]) & 0xFF;  key случаен;  pad = 3..10
```

Реализация: `soulight/protocol/lightprotocol.py` (кодек) +
`native_bridge.py` (drop-in замена .NET-моста).

Выбор backend: `SOULIGHT_PROTOCOL=native|beelight|auto` (default `auto` —
native если нет pythonnet+Beelight.exe). Порт: `SOULIGHT_PORT` или
автодетект (`ttyUSB*`/`ttyACM*` на Linux, `COM7` на Windows).

## Примечания

- Контроллер просыпается по DTR+RTS; на Linux нужен доступ к порту
  (`sudo usermod -aG dialout $USER`).
- Screen capture: `bettercam` (Windows) → fallback `mss`.
- `dotnet/SoulightBridge.dll` — опциональный fast-path для legacy backend.
- Тесты: `python -m unittest tests.test_native_protocol` (включая валидацию
  по реальному capture `tests/replay.csv`).
