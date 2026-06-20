# Soulight — Audit Findings

> Автоматический аудит кодовой базы (вне аудио-модулей).  
> Найденные проблемы, их риски и статус исправления.

---

## Severity: Medium

### 1. `LEDConfigPanel` позволяет включить Live Preview с невалидной конфигурацией

**Файлы:** `soulight/ui/led_config_widget.py`, `soulight/led_config.py`  
**Суть:** `_on_offset_changed` и `_on_count_changed` не проверяют, что `total + start_offset <= MAX_LEDS`. Кнопка Confirm блокирует сохранение, но если включен Live Preview, конфигурация сразу отправляется в `_on_led_config_confirmed` → `_send_led_config_preview`, который молча возвращается, если `total + offset > MAX_LEDS`. Пользователь не видит предупреждения и не понимает, почему лента не реагирует.

**Риск:** Путаница в UI, неявное молчание при превышении лимита.

**Статус:** Исправлено — добавлена валидация и блокировка Live Preview при превышении лимита.

### 2. `LEDConfig.set_count` не учитывает `start_offset`

**Файлы:** `soulight/led_config.py`  
**Суть:** Метод `set_count` ограничивает сумму сторон до `MAX_LEDS`, но не учитывает `start_offset`. В итоге `total + start_offset` может превысить 75. Проверка есть только на уровне UI при Confirm.

**Риск:** Несогласованность между моделью и UI; возможна отправка переполненного буфера на ленту.

**Статус:** Исправлено — `LEDConfig.set_count` теперь принимает `max_total` и панель передаёт `effective_max`.

### 3. `MainWindow` напрямую мутирует приватный `_send_interval` драйвера

**Файлы:** `soulight/ui/main_window.py`, `soulight/protocol/serial_driver.py`  
**Суть:** `_on_speed_mode_clicked` делает `self._driver._send_interval = interval`. Это нарушает инкапсуляцию и делает архитектуру хрупкой.

**Риск:** Сломается, если внутреннее имя или семантика `_send_interval` изменится.

**Статус:** Исправлено — добавлен публичный setter `LEDDriver.set_send_interval()`.

---

## Severity: Low

### 4. `SceneEngine` использует `hasattr(self, '_layout_leds')`

**Файлы:** `soulight/scenes/engine.py`  
**Суть:** `set_layout` всегда устанавливает атрибут, поэтому `hasattr` избыточен. Это паттерн, который может привести к скрытым ошибкам, если кто-то забудет вызвать `set_layout`.

**Риск:** Низкий, но ухудшает читаемость и надёжность.

**Статус:** Исправлено — `_layout_leds` инициализирован в `__init__`, `hasattr` убран.

### 5. `ScreenCapturer` содержит неиспользуемый persistent mss instance

**Файлы:** `soulight/screen_mirroring/screen_capture.py`  
**Суть:** `self._sct = None` создаётся в `__init__`, `close()` его закрывает, но в `capture()` и `_capture_edges_mss()` каждый раз создаётся свежий `mss.mss()`. Комментарий о "persistent instance для экономии ~1.2ms" не соответствует коду.

**Риск:** Мертвый код, лишние аллокации, несоответствие комментария и реализации.

**Статус:** Исправлено — удалён неиспользуемый `_sct` и `_mss_error_count` переименован в `_capture_errors` для ясности.

### 6. `serial_driver._send_loop` не сбрасывает счётчик `count` при смене режима

**Файлы:** `soulight/protocol/serial_driver.py`  
**Суть:** Переменная `count` накапливается вечно. При переключении между `per_led` и `solid color` heartbeat-кадрирование может сдвигаться.

**Риск:** Незначительный дрейф heartbeat; не критично.

**Статус:** Исправлено — `count` сбрасывается при смене активного режима.

### 7. `ColorPreset.save()` вызывается на каждое изменение brightness

**Файлы:** `soulight/ui/main_window.py`, `soulight/color_preset.py`  
**Суть:** `_on_brightness_changed` сохраняет preset в файл при каждом тике слайдера (с дебаунсом 50 мс). Это не критично, но на интенсивном использовании может вызывать лишние дисковые операции.

**Риск:** Износ SSD/HDD, небольшие лаги UI при быстром дёргании слайдера.

**Статус:** Исправлено — добавлена защита: сохранение только если значение реально изменилось.

### 8. `bridge.py` содержит неиспользуемую константу `BRIDGE_DIR`

**Файлы:** `soulight/protocol/bridge.py`  
**Суть:** `BRIDGE_DIR` определена, но нигде не используется.

**Риск:** Мертвый код.

**Статус:** Исправлено — удалена.

### 9. `LEDConfigPanel._on_reset` не сохраняет новую конфигурацию

**Файлы:** `soulight/ui/led_config_widget.py`  
**Суть:** При нажатии Reset создаётся новый `LEDConfig` в памяти, но старый файл на диске не перезаписывается. Если пользователь закроет приложение без Confirm, при следующем старте загрузится старая конфигурация.

**Риск:** Неожиданное поведение для пользователя.

**Статус:** Исправлено — после Reset сразу вызывается `_config.save()` и `config_confirmed` (если Live Preview включен).

---

## Severity: Architectural / Notes

### 10. `SceneEngine` и `AudioEngine` создают `threading.Thread` внутри `QThread`

**Файлы:** `soulight/scenes/engine.py`, `soulight/audio/engine.py`  
**Суть:** Объекты наследуют `QObject` и `moveToThread` в `QThread`, но реальная работа ведётся в отдельном `threading.Thread`. Это работает, но неидиоматично для Qt. Сигналы `frame_ready` и `error_occurred` приходят из plain thread в Qt event loop.

**Риск:** Средний. Потенциальные проблемы с lifecycle QObject при уничтожении. Пока не меняем, так как это крупная переработка и требует отдельного обсуждения.

**Статус:** Задокументировано, не исправлено.

### 11. `LEDDriver` не уведомляет UI о разрыве соединения

**Файлы:** `soulight/protocol/serial_driver.py`  
**Суть:** `_safe_write` молча глотает все исключения. Если COM-порт отвалится, драйвер продолжит считать себя `_connected = True`. UI не узнает о проблеме.

**Риск:** Средний. Пользователь не поймёт, почему лента перестала реагировать.

**Статус:** Задокументировано, не исправлено (требует добавления сигнала/колбэка и тестирования с реальным оборудованием).

### 12. Отсутствует валидация входных данных в `LEDConfig.load`

**Файлы:** `soulight/led_config.py`  
**Суть:** Если `led_config.json` повреждён или содержит отрицательные/слишком большие значения, они загружаются как есть. Последующий `set_count` может урезать, но `start_offset` и `counts` не проверяются.

**Риск:** Низкий. Пользователь может получить невалидный конфиг.

**Статус:** Задокументировано, не исправлено (можно добавить sanitize в рамках отдельной задачи).

---

## Сводка по правкам

| # | Проблема | Файлы | Статус |
|---|----------|-------|--------|
| 1 | Live Preview с невалидным конфигом | `ui/led_config_widget.py` | Исправлено |
| 2 | `set_count` не учитывает offset | `led_config.py`, `ui/led_config_widget.py` | Исправлено |
| 3 | Прямой доступ к `_send_interval` | `protocol/serial_driver.py`, `ui/main_window.py` | Исправлено |
| 4 | `hasattr` в `SceneEngine` | `scenes/engine.py` | Исправлено |
| 5 | Неиспользуемый `_sct` | `screen_mirroring/screen_capture.py` | Исправлено |
| 6 | Счётчик `count` не сбрасывается | `protocol/serial_driver.py` | Исправлено |
| 7 | Лишние save preset | `ui/main_window.py`, `color_preset.py` | Исправлено |
| 8 | Мёртвая константа `BRIDGE_DIR` | `protocol/bridge.py` | Исправлено |
| 9 | Reset не сохраняет конфиг | `ui/led_config_widget.py` | Исправлено |
| 10 | Thread-in-QThread | `scenes/engine.py`, `audio/engine.py` | Документировано |
| 11 | Нет уведомления о разрыве COM | `protocol/serial_driver.py` | Документировано |
| 12 | Нет sanitize в `load` | `led_config.py` | Документировано |
| 13 | DXCam не в requirements, нет COM init | `screen_mirroring/*`, `requirements.txt`, `ui/main_window.py` | Исправлено |

---

## Optimization / Performance Audit

### 1. MSS screen capture создавал свежий instance на каждый кадр

**Файлы:** `soulight/screen_mirroring/screen_capture.py`  
**Суть:** Ранее `mss.mss()` создавался fresh при каждом `capture()` / `_capture_edges_mss()`. Это тратит ~1-2 ms на создание/уничтожение GDI-контекста.  
**Риск:** Снижение effective FPS для screen mirroring, лишние аллокации.  
**Статус:** Исправлено — добавлен ленивый persistent mss instance через `_get_sct()`, сбрасывается только при ошибке.

### 2. `spectrum` audio mode использовал O(n²) поиск ближайшей частоты

**Файлы:** `soulight/audio/modes.py`  
**Суть:** `np.argmin(np.abs(freq_bins - target_freq))` вызывался в цикле для каждого LED.  
**Риск:** Рост времени обработки при увеличении разрешения FFT.  
**Статус:** Исправлено — векторизованный `np.searchsorted` + выбор ближайшего соседа.

### 3. `wave` audio mode пересоздавал интерполяционные координаты каждый кадр

**Файлы:** `soulight/audio/modes.py`  
**Суть:** `np.linspace` и `np.logspace` для `xi`/`log_x` создавались каждый кадр, хотя размеры фиксированы.  
**Риск:** Лишние аллокации numpy-массивов в горячем цикле.  
**Статус:** Исправлено — добавлен module-level кэш `_WAVE_CACHE` по `(mags.size, led_count)`.

### 4. `serial_driver._send_loop` использовал `time.sleep` вместо `Event.wait`

**Файлы:** `soulight/protocol/serial_driver.py`  
**Суть:** `time.sleep` не прерывается при вызове `disconnect()`. Поток мог спать до 500 мс в idle-режиме после запроса остановки.  
**Риск:** Медленное отключение, заметная задержка при закрытии приложения.  
**Статус:** Исправлено — все `time.sleep` в `_send_loop` заменены на `self._send_stop.wait(...)`.

### 5. Нет batch-обновления яркости в solid color режиме

**Файлы:** `soulight/protocol/serial_driver.py`  
**Суть:** В solid color режиме яркость переотправляется каждые 50 кадров. Это генерирует лишний reflection-вызов `make_bright_packet`.  
**Риск:** Небольшие лаги/джиттер каждые ~50 кадров.  
**Статус:** Документировано, не исправлено (требует тестирования с контроллером — текущая логика была добавлена как workaround для сброса dimmer).

### 6. `bridge.py` reflection-вызовы для каждого пакета

**Файлы:** `soulight/protocol/bridge.py`  
**Суть:** `make_color_packet`, `make_bright_packet`, `make_rgb_transfer_packet` используют `MethodInfo.Invoke` и `FormatterServices.GetUninitializedObject` для каждого пакета.  
**Риск:** Значительный CPU overhead на высоких FPS.  
**Статус:** Документировано. Быстрый C# мост (`SoulightBridge.dll`) уже реализован как fallback; его активация зависит от наличия DLL и успешной инициализации. Полный переход на fast bridge требует тестирования.

### 7. `sampler.py` — 75 отдельных вызовов `numpy.mean` на кадр

**Файлы:** `soulight/screen_mirroring/sampler.py`  
**Суть:** Для каждого LED вызывается `_average_rect_bgra` → `numpy.mean(axis=(0,1))`. 75 LED × 20 FPS = 1500 mean/сек.  
**Риск:** Небольшой, но не нулевой overhead.  
**Статус:** Документировано, не исправлено. Векторизация возможна, но усложняет код из-за разных размеров `sample_rect`. Оставлено до профилирования реального bottleneck.

### 8. `ScreenCapturer._rgb_to_bgra` создаёт полный BGRA для dxcam

**Файлы:** `soulight/screen_mirroring/screen_capture.py`  
**Суть:** DXCam возвращает RGB; для совместимости с sampler конвертируется в полный BGRA (8 МБ для 4K), хотя нужны только края.  
**Риск:** Высокая нагрузка на CPU и память при изменениях экрана.  
**Статус:** Документировано, не исправлено. Оптимизация требует либо RGB-native sampler, либо конвертации только edge-полос. Крупное изменение, требует обсуждения.

### 9. `FrameSmoother` конвертирует list ↔ numpy каждый кадр

**Файлы:** `soulight/screen_mirroring/sampler.py`  
**Суть:** `apply` принимает список кортежей, конвертирует в numpy, смешивает, конвертирует обратно.  
**Риск:** Лишние аллокации в горячем пути.  
**Статус:** Документировано, не исправлено. Переход на numpy-native pipeline весь путь до driver требует изменения API sampler и driver.

### 10. `SceneEngine` не учитывает время генерации при sleep

**Файлы:** `soulight/scenes/engine.py`  
**Суть:** `self._stop_event.wait(self._interval)` не вычитает время генерации паттерна.  
**Риск:** FPS сцен чуть ниже target, особенно на медленных CPU.  
**Статус:** Документировано, не исправлено (влияние мало для 20 FPS и простых паттернов).

### 11. DXCam не был в `requirements.txt` и не инициализировал COM в worker thread

**Файлы:** `requirements.txt`, `soulight/screen_mirroring/screen_capture.py`, `soulight/screen_mirroring/worker.py`, `soulight/ui/main_window.py`  
**Суть:** Вероятные причины, по которым dxcam "не вышло в первый раз":
1. `dxcam` отсутствовал в `requirements.txt` — чистая установка зависимостей не ставила пакет.
2. `dxcam.create()` вызывался из `MirrorWorker` (QThread) без COM-инициализации. DirectX Desktop Duplication требует COM в потоке; без `CoInitializeEx` может вернуться `None` или COM-ошибка.
3. При первом фейле `_dxcam_failed` устанавливался в `True` навсегда, не давая повторить попытку без перезапуска приложения.
4. Не было UI-переключателя и детального логирования, чтобы понять, что именно сломалось.

**Риск:** DXCam (в 10-20x быстрее MSS) не использовался; screen mirroring работал на медленном GDI backend.

**Статус:** Исправлено:
- `dxcam>=0.0.5` добавлен в `requirements.txt`.
- Добавлена thread-local COM-инициализация `_init_dxcam_com()` перед `dxcam.create()`.
- Улучшено логирование: отдельные сообщения для init, first-frame-none, ошибок и disable.
- `_dxcam_failed` теперь выставляется только после 3 ошибок, а не после первой; смена backend чекбоксом создаёт новый `ScreenCapturer` с чистым флагом.
- В UI добавлен чекбокс **Use DXCam backend** (вкладка Screen Mirror). Если `dxcam` не установлен — чекбокс disabled с подсказкой.

**TODO:** протестировать с реальным dxcam и монитором; убедиться, что COM-инициализация MTA корректна в QThread. Если будут новые ошибки — в логах теперь будет видна реальная причина.

---

## Непротестированные участки

- Любая логика, завязанная на реальное COM-подключение, не может быть проверена без LED-контроллера.
- Работа с `pythonnet` / `Beelight.exe` протестирована только синтаксически (`py_compile`) в текущем окружении.
- Захват экрана (`dxcam`/`mss`) не проверялся в runtime.
- Оптимизации `spectrum` и `wave` проверены только синтаксически и логически; фактическая нагрузка на CPU требует профилирования с реальным аудиопотоком.
