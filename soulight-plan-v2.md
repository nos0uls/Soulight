# Soulight v2 — План разработки

## Что уже сделано

**Протокол полностью обойдён** без ручного реверса шифрования.

Beelight.exe — .NET сборка, обфусцированная CryptoObfuscator. При загрузке через `Assembly.LoadFrom()` обфускатор сам расшифровывает тела методов. Внутри обнаружен **LightProtocol** — второй слой протокола с чистыми методами для генерации wire-format пакетов.

Рабочие методы (вызываются через .NET reflection):
- `LProtocolCtrl.GenColorPackage(Color, channel)` → пакет цвета (26 байт)
- `LProtocolCtrl.GenBrightPackage(dimmer, channel)` → пакет яркости
- `LProtocolCtrl.GenSwitchPackage(on/off, channel)` → включение/выключение
- `LProtocolCtrl.GenWorkModePackage(LP_WK_MODE_PC, channel)` → режим PC
- `LProtocolBase.GenFramePackage(LP_ATTR_REQ, LP_CMD_HEARTBEAT, [])` → heartbeat
- `LProtocolCtrl.GenRGBTransferPackage(Color[], channel)` → per-LED (не тестировано)

Протестировано: произвольный RGB на максимальной яркости, стабильно, без мигания.

Артефакты:
- `Soulight.cs` — рабочий CLI: `Soulight.exe 255 0 255 30`
- `LPColorBridge.cs` — прототип (устарел)
- `ProtocolBridge.cs` — discovery-скрипт (устарел)

---

## Архитектура: что Python, что C#, как связаны

### Проблема
Шифрование протокола живёт внутри `Beelight.exe` (.NET). Пакеты генерируются методами типа `GenColorPackage()`, которые каждый раз создают уникальный nonce. Воспроизвести шифрование на Python без реверса невозможно — но и не нужно: можно вызывать .NET методы напрямую.

### Три варианта архитектуры

#### Вариант A: Python + pythonnet (рекомендуемый)
```
┌──────────────────────────────────────┐
│         Python приложение            │
│                                      │
│  PyQt6 UI ←→ App Logic (Python)      │
│       │              │               │
│       │         pythonnet            │
│       │         (.NET CLR)           │
│       │              │               │
│       │    Beelight.exe (reflection) │
│       │    GenColorPackage()         │
│       │    GenBrightPackage()        │
│       │              │               │
│       └──── pyserial ───→ COM7       │
└──────────────────────────────────────┘
```

**pythonnet** (`pip install pythonnet`) — библиотека, которая встраивает .NET CLR прямо в Python процесс. Позволяет делать:
```python
import clr
clr.AddReference(r"C:\...\Beelight.exe")
from LightProtocol.Vendor.LPVendor import LProtocolCtrl
packet = LProtocolCtrl.GenColorPackage(Color.FromArgb(255, 0, 255), 0)
```

Плюсы:
- Один процесс, нет overhead на subprocess
- Python управляет всем: UI, screen capture, serial, пакеты
- Простая отладка

Минусы:
- pythonnet иногда капризный при установке
- Зависимость от .NET Framework на машине (уже есть)

#### Вариант B: Python + subprocess C#
```
┌─────────────────────┐     stdin/stdout     ┌─────────────────┐
│  Python приложение   │ ←─────────────────→ │  Soulight.exe   │
│  PyQt6 UI            │     JSON команды    │  .NET reflection│
│  mss screen capture  │                     │  Serial output  │
└─────────────────────┘                      └─────────────────┘
```

Python запускает Soulight.exe как subprocess, отправляет команды через stdin (JSON), C# генерирует пакеты и пишет в serial.

Плюсы:
- Чистое разделение: Python не трогает .NET
- C# уже работает (Soulight.exe)

Минусы:
- Overhead на IPC (stdin/stdout) — ~1-5ms на команду
- Два процесса, сложнее отладка
- Нужно поддерживать протокол общения между ними

#### Вариант C: Чистый C# (WinForms/WPF)
```
┌──────────────────────────────────────┐
│         C# приложение (WPF)          │
│                                      │
│  XAML UI ←→ App Logic (C#)           │
│       │              │               │
│       │    Beelight.exe (reflection) │
│       │    GenColorPackage()         │
│       │              │               │
│       │  System.IO.Ports → COM7      │
│       │              │               │
│       │  DXGI Screen Capture         │
└──────────────────────────────────────┘
```

Плюсы:
- Нативный доступ к .NET, без мостов
- DXGI capture быстрее mss
- Один язык

Минусы:
- UI на WPF/WinForms медленнее разрабатывать чем PyQt6
- XAML verbose
- Компилятор v4.0 (csc.exe) не поддерживает C# 6+ (нет `?.`, `$""`, async/await pattern matching)
- Нужен Visual Studio или SDK для нормальной разработки

### Рекомендация

**Вариант A (pythonnet)** — лучший баланс. Если pythonnet не заведётся — fallback на **Вариант B (subprocess)**, где Soulight.exe уже готов.

---

## Фаза 1: Solid Color приложение

### 1.1 Инфраструктура
- Установить зависимости: `pythonnet`, `PyQt6`, `pyserial`
- Создать структуру проекта
- Протестировать pythonnet bridge: загрузка Beelight.exe, вызов GenColorPackage из Python

### 1.2 Protocol bridge (Python модуль)
```
soulight/
  protocol/
    bridge.py       # pythonnet загрузка Beelight.exe, обёртки над LP методами
    serial_driver.py # Открытие COM порта, отправка пакетов, heartbeat loop
```

`bridge.py` — тонкая обёртка:
```python
class BeelightBridge:
    def __init__(self):
        # Загрузка assembly через pythonnet
    def make_color_packet(self, r, g, b) -> bytes
    def make_bright_packet(self, dimmer) -> bytes
    def make_switch_packet(self, on: bool) -> bytes
    def make_heartbeat() -> bytes
```

`serial_driver.py` — управление соединением:
```python
class LEDDriver:
    def __init__(self, port="COM7", baud=500000)
    def connect(self)          # Open + handshake + switch ON + PC mode
    def set_color(self, r, g, b, brightness=255)
    def disconnect(self)
    # Внутри: background thread для heartbeat каждые ~500ms
```

### 1.3 UI: Главное окно
```
soulight/
  ui/
    main_window.py    # Главное окно с табами
    color_picker.py   # Палитра + RGB слайдеры + hex input + пресеты
    settings.py       # COM порт, яркость, autoconnect
```

Функционал Solid Color:
- Палитра цветов (color wheel или grid)
- RGB слайдеры (0-255 каждый)
- HEX ввод (#FF00FF)
- Slider яркости (0-100%)
- Пресеты: Red, Green, Blue, Purple, Yellow, White, Warm White, Cool White
- Кнопка ON/OFF
- Статус подключения

### 1.4 Точка входа
```
soulight/
  app.py            # QApplication + main window
  __main__.py       # python -m soulight
```

### 1.5 Сборка
```
requirements.txt    # pythonnet, PyQt6, pyserial
README.md
```

## Фаза 2: Режимы и расширенный функционал (три в отдельных менюшках\на разных кнопках) 

### 2.1 Режимы реакции на звук (Audio Modes)

Оригинальное Beelight имело "Music" раздел с режимами реакции на аудио:

- **Spectrum** — частотный анализ (высокие/низкие частоты → разные цвета)
- **Electronic** — пульсация под бит (транс/техно)
- **Lyricism** — плавные переходы под мелодию

**Реализация:**
- Захват аудио: `sounddevice` или `pyaudio`
- FFT анализ в реальном времени
- Параметрические режимы (разные алгоритмы маппинга частот → цвет)

### 2.2 Сценические режимы (Scenes)

Предустановленные анимации без привязки к экрану/звуку:

| Сцена | Описание |
|-------|----------|
| Rainbow | Радуга, плавный перебор Hue |
| Fire | Огонь: красный→оранжевый, мерцание |
| Vitality | Энергичная смена цветов |
| Firework | Имитация фейерверка (вспышки) |
| Seasons | Медленные сезонные палитры |
| Warm | Тёплый жёлтый, имитация свечи |
| Aurora | Полярное сияние (зелёный→фиолетовый) |
| Romance | Медленные розовые переходы |
| Flow | Плавное течение цвета по ленте |
| Chase | Бегущие огни |

**Реализация:**
- Фоновый thread с паттерн-генераторами
- `GenRGBTransferPackage` — per-LED контроль для "бегущих" эффектов
- Настраиваемая скорость (Speed slider)

### 2.3 LED Configuration UI

Визуальная настройка расположения LED по периметру экрана (как на скриншоте):

**Элементы:**
- Представление монитора с 4 сторонами
- Выбор начальной точки (1-4 угла)
- Направление (по часовой/против)
- Количество LED на каждой стороне (top, bottom, left, right)
- Enable/disable отдельных LED (checkbox на каждом с возможностью выделения нескольких сразу)
- "All selected" — выбрать всё
- Reset / Confirm кнопки
- LED загораются, когда ты их выбираешь, зеленым синим красным белый на каждой из сторон, соответственно отображению в приложении. 

**Реализация:**
- Custom QWidget с отрисовкой схемы
- Drag-select или click-select для LED
- Сохранение конфигурации в JSON

### 2.4 Smooth/Normal/Fast режимы

Управляют двумя параметрами:
- **Target FPS** — частота обновления ленты
  - Smooth: ~15 fps (медленно, плавно)
  - Normal: ~25 fps
  - Fast: ~40 fps (быстро, реактивно)
- **Smoothing factor** — сглаживание переходов (0.0-1.0)

**Реализация:**
- UI: 3 кнопки с иконками спидометра
- Влияет на `_send_interval` в LEDDriver

---

## Фаза 3: Screen Mirroring (Ambilight)

### 3.0 Предварительный тест
Перед началом: протестировать `GenRGBTransferPackage(Color[], channel)` — если один вызов отправляет все 75 LED в одном пакете, screen mirroring реален через LP.

### 3.1 Screen Capture
- `mss` библиотека, 30fps целевой 
- Захват только нужной области (primary monitor)

### 3.2 LED Sampling
- Разбиение краёв экрана на зоны согласно LED Configuration
- numpy для быстрого усреднения RGB

### 3.3 Smoothing
- Линейная интерполяция между кадрами (predictive, не reactive)
- Настраиваемый smoothing factor

### 3.4 UI для mirroring
- Превью монитора с LED по периметру
- Start/Stop, FPS counter
- Настройки: brightness, saturation, delay compensation

---

## Порядок работы

1. ✅ **pythonnet bridge** — загрузка Beelight.exe из Python, тест GenColorPackage
2. ✅ **serial_driver** — подключение, heartbeat thread, set_color()
3. ✅ **UI: Solid Color** — color picker, RGB слайдеры, пресеты
4. **Тест GenRGBTransferPackage** — feasibility per-LED контроля
5. **UI: LED Configuration** — визуальная настройка LED по сторонам
6. **UI: Scenes** — паттерн-режимы (Rainbow, Fire, Aurora...)
7. **UI: Audio Modes** — Spectrum, Electronic, Lyricism...
8. **Screen mirroring** — mss capture + LED sampling
