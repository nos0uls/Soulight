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

## Headless / автозапуск

GUI не нужен — Qt вообще не импортируется (работает на systemd/SSH без X):

```bash
python -m soulight --headless --mode color --color FF3300 --brightness 128
python -m soulight --headless --mode scene --pattern fire --fps 25 --speed 1.5
python -m soulight --headless --mode audio --audio-mode spectrum --device <id>
python -m soulight --headless --list-devices     # id устройств вывода/ввода
python -m soulight --headless --mode off
```

Connect повторяется каждые 3с, пока контроллер не появится (флаг
`--no-retry` отключает). systemd unit:

```ini
[Unit]
Description=Soulight LED
After=dev-ttyACM0.device

[Service]
ExecStart=/path/.venv/bin/python -m soulight --headless --mode scene --pattern aurora
WorkingDirectory=/path/Soulight
Restart=on-failure

[Install]
WantedBy=multi-user.target
```

## Примечания

- Контроллер просыпается по DTR+RTS; на Linux нужен доступ к порту
  (`sudo usermod -aG dialout $USER`).
- Яркость — hardware dimmer контроллера (UI 0-255 → wire 0-1000),
  единый для всех режимов.
- Аудио: захват с выбранного устройства вывода (loopback/monitor source)
  или микрофона — выбирается на вкладке Audio.
- Screen capture: `bettercam` (Windows) → fallback `mss`.
- Настройки (цвет, LED-раскладка): `%APPDATA%/Soulight` на Windows,
  `~/.config/soulight` на Linux.
- `dotnet/SoulightBridge.dll` — опциональный fast-path для legacy backend.
- Тесты: `python -m unittest tests.test_native_protocol` (включая валидацию
  по реальному capture `tests/replay.csv`).
