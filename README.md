# Soulight

Замена Beelight для управления LED лентой Lytmi/Beelight.

Использует LightProtocol команды через .NET reflection из оригинального `Beelight.exe` — полный контроль цвета и яркости без реверса шифрования.

## Возможности

- **Solid Color** — произвольный RGB цвет на всю ленту
- **Brightness** — регулировка яркости (0-255)
- **Color Presets** — быстрые пресеты (Red, Green, Blue, Purple, Yellow, Cyan, Orange, White, Warm/Cool White)
- **HEX ввод** — цвет через #RRGGBB
- **Dark UI** — тёмная тема (Catppuccin-style)

## Установка

```bash
pip install -r requirements.txt
```

Требования:
- Python 3.10+
- Windows (нужен .NET Framework 4.x)
- `Beelight.exe` установлен в `C:\Program Files (x86)\Beelight\Beelight V3.0\`
- LED контроллер подключён к COM7

## Запуск

```bash
# GUI приложение
python -m soulight

# CLI (fallback)
Soulight.exe 255 0 255 30
```

## Структура

```
soulight/
  protocol/
    bridge.py         # pythonnet мост к Beelight.exe
    serial_driver.py  # Serial + continuous send loop
  ui/
    main_window.py    # PyQt6 главное окно
  app.py              # Точка входа
```

## Архитектура

```
PyQt6 UI  →  LEDDriver  →  BeelightBridge (pythonnet/.NET)  →  Serial COM7
                                    ↓
                            Beelight.exe (reflection)
                            GenColorPackage()
                            GenBrightPackage()
```

Beelight.exe обфусцирован CryptoObfuscator, но расшифровывает method bodies при загрузке через `Assembly.LoadFrom()`. Методы LightProtocol генерируют готовые wire-format пакеты (`55 AA 5A ...`).
