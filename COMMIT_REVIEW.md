# Soulight — Построчный ревью коммита 670d94fc

> Ревью изменений коммита `670d94fc` от 2026-06-20.  
> Проверена каждая строка каждого изменённого файла.  
> Ниже — найденные баги, замечания и подтверждения корректности.

---

## Bugs (требуют исправления)

### BUG-1: `_on_audio_status_changed` полностью затирает сообщение об ошибке

**Файл:** `soulight/ui/main_window.py:1622-1637`  
**Серьёзность:** Высокая — пользователь видит "Idle" вместо сообщения об ошибке

**Суть:** В `engine.py:206-209` при ошибке эмитятся два сигнала подряд:

```python
self.error_occurred.emit(f"Audio error: {e}")  # #1 в очереди
self.status_changed.emit("Error")               # #2 в очереди
```

В UI thread обработка идёт по очереди:

1. **`_on_audio_error(msg)`** (из #1):
   - Ставит `"Error: Audio error: ..."` на label
   - Вызывает `_stop_audio()` → `stop()` → эмитит `status_changed("Stopped")` (queued #3)
   - `_stop_audio()` ставит `"Idle"` на label

2. **`_on_audio_status_changed("Error")`** (из #2):
   - `else`-ветка: `self._audio_status_label.setText("Error")` — затирает детальное сообщение

3. **`_on_audio_status_changed("Stopped")`** (из #3):
   - Ставит `"Idle"` на label

**Итог:** label заканчивается на `"Idle"` — детальное сообщение об ошибке полностью потеряно.

**Комментарий в коде (строка 1636):** «не перезаписываем сообщение об ошибке из _on_audio_error» — но код делает именно это.

**Фикс:**

```python
def _on_audio_status_changed(self, status):
    if status in ("Running", "Capturing..."):
        self._update_audio_status_running()
    elif status == "Stopped":
        if not self._audio_active:
            return  # _stop_audio уже установил "Idle"
        self._audio_status_label.setText("Idle")
        self._audio_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
    elif status == "Error":
        pass  # сообщение уже установлено _on_audio_error
    else:
        self._audio_status_label.setText(status)
```

---

### BUG-2: `capture_edges` отключает DXCam при первой же ошибке, игнорируя порог в 3 ошибки

**Файл:** `soulight/screen_mirroring/screen_capture.py:213-227`  
**Серьёзность:** Средняя — DXCam отключается навсегда при одном transient failure

**Суть:** В `_capture_edges_dxcam:300-318` реализована логика: `_dxcam_failed = True` только после `self._capture_errors >= 3`. Но `capture_edges` перехватывает `raise` и **безусловно** ставит `_dxcam_failed = True` на строке 223:

```python
except Exception as e:
    self._debug_log("dxcam-fallback", ...)
    self._dxcam_failed = True  # <-- первая же ошибка → permanent disable
```

Это делает 3-error порог в `_capture_edges_dxcam` (строки 313-314) **мёртвым кодом**. AUDIT_FINDINGS.md заявляет «только после 3 ошибок», но фактически отключение происходит при первой.

**Фикс:** Убрать `self._dxcam_failed = True` из `capture_edges`:

```python
except Exception as e:
    self._debug_log("dxcam-fallback", ...)
    # Не ставим _dxcam_failed — решение принимает _capture_edges_dxcam
```

---

### BUG-3: `run_silent.bat` — третья ветка fallback использует `pythonw`, которого уже нет в PATH

**Файл:** `run_silent.bat`  
**Серьёзность:** Низкая — fallback не сработает, если `pythonw` и `pyw` оба отсутствуют

**Суть:**

```bat
where pythonw >nul 2>nul          # проверка 1
if %errorlevel% == 0 (
    pythonw -m soulight           # запуск через pythonw
) else (
    where pyw >nul 2>nul          # проверка 2
    if %errorlevel% == 0 (
        pyw -3.12 -m soulight     # запуск через pyw
    ) else (
        start "" pythonw -m soulight   # BUG: pythonw уже не найден в проверке 1!
    )
)
```

Третья ветка доходит только если `where pythonw` **упал** (проверка 1) и `where pyw` тоже **упал** (проверка 2). Но `start "" pythonw` снова пытается запустить `pythonw`, которого нет в PATH.

**Фикс:** Использовать `python` (с консолью) или `py -3.12` как последний fallback:

```bat
) else (
    start "" python -m soulight
)
```

---

## Issues (не критичны, но стоит исправить)

### ISSUE-1: `_uninit_com_for_thread()` вызывает `CoUninitialize` без проверки, была ли успешна `CoInitializeEx`

**Файл:** `soulight/audio/engine.py:32-48, 163, 211`  
**Суть:** `_init_com_for_thread()` возвращает `True/False`, но возвращаемое значение игнорируется в `_run_loop`:

```python
def _run_loop(self):
    _init_com_for_thread()   # возвращаемое значение потеряно
    ...
    finally:
        _uninit_com_for_thread()  # всегда вызывается, даже если init упал
```

`CoUninitialize` без парного `CoInitializeEx` — undefined behavior (обычно безвредно, но некорректно).

**Фикс:**

```python
def _run_loop(self):
    com_owned = _init_com_for_thread()
    ...
    finally:
        if com_owned:
            _uninit_com_for_thread()
```

---

### ISSUE-2: `_init_dxcam_com()` не имеет парного `CoUninitialize`

**Файл:** `soulight/screen_mirroring/screen_capture.py:20-30, 105-119`  
**Суть:** COM инициализируется через `_init_dxcam_com()` при первом DXCam-захвате, но `close()` не вызывает `CoUninitialize`. При выходе из потока COM очистится ОС, но это resource leak.

**Фикс:** Добавить `CoUninitialize` в `close()`:

```python
def close(self):
    ...
    if getattr(_DXCAM_COM_THREAD, "initialized", False):
        try:
            ctypes.windll.ole32.CoUninitialize()
            _DXCAM_COM_THREAD.initialized = False
        except Exception:
            pass
```

---

### ISSUE-3: `get_monitor_geometry` создаёт fresh `mss.mss()` вместо использования `_get_sct()`

**Файл:** `soulight/screen_mirroring/screen_capture.py:142-166`  
**Суть:** После оптимизации с persistent mss instance через `_get_sct()`, `get_monitor_geometry` всё ещё создаёт временный `mss.mss()` через `with`-блок (строка 148). Это не критично (метод вызывается редко — только при `rebuild_layout`), но непоследовательно с остальным кодом.

---

### ISSUE-4: `_WAVE_CACHE` в `modes.py` никогда не очищается

**Файл:** `soulight/audio/modes.py:19`  
**Суть:** Module-level dict `_WAVE_CACHE` растёт, если `mags.size` или `led_count` меняются. На практике они фиксированы в рамках одной сессии, но при смене конфигурации LED или `block_size` старые записи остаются в памяти навсегда.

**Фикс:** Очищать кэш при смене параметров, или использовать `@functools.lru_cache` с лимитом.

---

### ISSUE-5: `wave` mode — первые ~10% LED получают одинаковое значение

**Файл:** `soulight/audio/modes.py:272-274`  
**Суть:** `log_x = np.logspace(0, 1, mags.size) / 10.0` даёт диапазон `[0.1, 1.0]`.  
`xi = np.linspace(0, 1, led_count)` начинается с `0.0`.  
`np.interp` клампит значения `xi < log_x[0]` (т.е. `xi < 0.1`) на первое значение `mags`.  
Для 75 LED первые ~7 получают одинаковый цвет.

**Фикс:**

```python
xi = np.linspace(log_x[0], log_x[-1], led_count)
```

---

### ISSUE-6: `electronic` и `pulse` режимы используют одинаковый ключ `"pulse"` в `_smooth` history

**Файл:** `soulight/audio/modes.py:155, 231`  
**Суть:** `electronic` mode: `pulse = _smooth(bass, "pulse", params, 0.25)`  
`pulse` mode: `pulse = _smooth(pulse_raw, "pulse", params, 0.45)`  

Оба используют ключ `"pulse"` в `params["history"]`. При переключении режима без перезапуска engine (если в будущем будет hot-swap), значение от предыдущего режима будет использовано как `prev` для нового режима. Самоисправляется за 1-2 кадра, но может вызвать кратковременный артефакт.

**Фикс:** Использовать уникальные ключи: `"elec_pulse"` и `"pulse_pulse"`.

---

### ISSUE-7: `ColorPreset.save()` обновляет `_last_saved` до записи на диск

**Файл:** `soulight/color_preset.py:39-49`  
**Суть:**

```python
def save(self):
    data = self._snapshot()
    if data == self._last_saved:
        return
    self._last_saved = data   # обновляем ДО записи
    try:
        with open(CONFIG_FILE, "w") as f:
            json.dump(data, f, indent=2)
    except Exception as e:
        print(f"[ColorPreset] Ошибка сохранения: {e}")
```

Если запись на диск падает, `_last_saved` уже обновлён. Следующий `save()` с теми же значениями не попытается записать снова. Не приводит к data loss (values в памяти корректны), но файл на диске может остаться устаревшим.

**Фикс:** Обновлять `_last_saved` после успешной записи:

```python
def save(self):
    data = self._snapshot()
    if data == self._last_saved:
        return
    try:
        with open(CONFIG_FILE, "w") as f:
            json.dump(data, f, indent=2)
        self._last_saved = data  # только после успеха
    except Exception as e:
        print(f"[ColorPreset] Ошибка сохранения: {e}")
```

---

## Построчная проверка по файлам

### `soulight/audio/engine.py` (212 строк)

| Строки | Что | Результат |
|--------|-----|----------|
| 32-40 | `_init_com_for_thread()` | **Корректно.** MTA через `CoInitializeEx(None, 0)`. Возвращает ownership flag. См. ISSUE-1 — флаг не используется. |
| 43-48 | `_uninit_com_for_thread()` | **Корректно** по логике, но вызывается безусловно. См. ISSUE-1. |
| 62-91 | `__init__` | **Корректно.** Все параметры валидируются (`max(1, ...)`, `max(1.0, min(60.0, ...))`). `_freq_bins` через `rfftfreq` — правильно. |
| 101-106 | `set_mode` | **Корректно.** Проверка `name in AUDIO_MODES`, `dict(params)` создаёт копию. |
| 108-117 | `set_sensitivity/gain/color_shift` | **Корректно.** Clamping: sensitivity 0.1..5.0, gain 0.0..3.0, color_shift `% 1.0`. |
| 119-121 | `set_fps` | **Корректно.** Clamping 1..60, `_interval` пересчитывается. |
| 126-140 | `start` | **Корректно.** `stop()` перед новым стартом, `_stop_event.clear()`, daemon thread. |
| 142-148 | `stop` | **Корректно.** `_stop_event.set()`, `join(timeout=2.0)`, emit `"Stopped"`. |
| 150-154 | `_compute_fft` | **Корректно.** Hanning window + `rfft` + `abs`. |
| 156-211 | `_run_loop` | **Корректно** по логике. COM init → recorder → loop → FFT → mode_fn → layout mask → emit → precise sleep. `finally` гарантирует COM uninit. См. ISSUE-1. |
| 193-196 | Layout mask | **Корректно.** Проверка `led.logical_index < len(colors)` предотвращает IndexError. |

### `soulight/audio/modes.py` (386 строк)

| Строки | Что | Результат |
|--------|-----|----------|
| 21-31 | `_clamp`, `_clampf`, `_hsv` | **Корректно.** Standard clamping + HSV→RGB. |
| 34-43 | `_smooth` | **Корректно.** Exponential smoothing: `prev + (new - prev) * factor`. `setdefault("history", {})` — один dict на все ключи. |
| 46-52 | `_normalize_magnitudes` | **Корректно.** `nan_to_num` + `maximum(0)`. |
| 55-72 | `_energy_band`, `_energy_sum` | **Корректно.** Mask по частотному диапазону, проверка empty. |
| 79-122 | `spectrum` | **Корректно.** `logspace` bins, `searchsorted` для O(n log n) поиска, `tanh` нормализация, `_smooth` per-LED, hue shift. Векторизация правильная. |
| 125-169 | `electronic` | **Корректно.** 3 band energy (bass/mid/treble), smoothing, pulse- intensity. См. ISSUE-6 — ключ `"pulse"` совпадает с `pulse` mode. |
| 172-209 | `lyricism` | **Корректно.** Spectral centroid + log hue + sine wave per-LED. |
| 212-249 | `pulse` | **Корректно.** Bass energy → smoothing → attack detection → edge fade. |
| 252-289 | `wave` | **Корректно** по логике. Кэш `(mags.size, led_count)` — правильный ключ. См. ISSUE-5 — clamping первых LED. |
| 292-321 | `bass` | **Корректно.** Bass + sub-bass с double-counting 20-100Hz (намеренно для веса sub-bass). |
| 324-360 | `disco` | **Корректно.** Beat detection через threshold + delta, `hue_acc` для смены цвета, smoothing flash. |
| 367-385 | `AUDIO_MODES`, `MODE_LABELS` | **Корректно.** 7 режимов, labels совпадают. |

### `soulight/screen_mirroring/screen_capture.py` (453 строки)

| Строки | Что | Результат |
|--------|-----|----------|
| 17-30 | `_init_dxcam_com` | **Корректно.** `threading.local()` для per-thread COM state. См. ISSUE-2 — нет `CoUninitialize`. |
| 35-39 | dxcam import | **Корректно.** Try/except, `DXCAM_AVAILABLE` flag. |
| 78-98 | `__init__` | **Корректно.** `_use_dxcam = DXCAM_AVAILABLE and prefer_dxcam` — правильное комбинирование. |
| 105-119 | `close` | **Корректно.** Закрывает mss и dxcam camera. См. ISSUE-2 — нет COM uninit. |
| 123-131 | `_get_sct` | **Корректно.** Lazy init persistent mss, raise при ошибке. |
| 135-139 | `_debug_log` | **Корректно.** Thread ID в логах для диагностики. |
| 142-166 | `get_monitor_geometry` | **Корректно.** См. ISSUE-3 — создаёт fresh mss вместо `_get_sct()`. |
| 170-209 | `capture` (mss full frame) | **Корректно.** Persistent sct, numpy reshape, error counter, reset on error. |
| 213-227 | `capture_edges` | **BUG-2.** `_dxcam_failed = True` на строке 223 — permanent disable при 1-й ошибке. |
| 231-335 | `_capture_edges_dxcam` | **Корректно** по логике. Lazy camera, RGB→BGRA, cache last frame, 4 edge regions. 3-error порог (строки 313-314) — мёртвый код из-за BUG-2. |
| 339-429 | `_capture_edges_mss` | **Корректно.** Single bounding box grab, numpy slicing для 4 edges. |
| 432-439 | `_get_monitor` | **Корректно.** Index validation, `IndexError` при выходе за пределы. |
| 443-452 | `_rgb_to_bgra` | **Корректно.** Channel swap RGB→BGRA, alpha=255. |

### `soulight/screen_mirroring/worker.py` (136 строк)

| Строки | Что | Результат |
|--------|-----|----------|
| 33-53 | `__init__` | **Корректно.** `prefer_dxcam` параметр, type conversion. |
| 63-69 | `shutdown` | **Корректно.** Закрывает engine в worker thread context. |
| 71-135 | `process_frame` | **Корректно.** Lazy engine creation в worker thread, first-frame retry с 100ms delay, error emission. |

### `soulight/screen_mirroring/engine.py` (122 строки)

| Строки | Что | Результат |
|--------|-----|----------|
| 41-55 | `__init__` | **Корректно.** `prefer_dxcam` передаётся в `ScreenCapturer`. |
| 59-60 | `close` | **Корректно.** Делегирует в capturer. |
| 76-85 | `rebuild_layout` | **Корректно.** Geometry → build_layout → smoother reset. |
| 89-106 | `process_next_frame` | **Корректно.** Auto-rebuild if needed, `capture_edges` + `sample_frame`. `rgb_bytes=None` — экономия CPU. |

### `soulight/protocol/serial_driver.py` (325 строк)

| Строки | Что | Результат |
|--------|-----|----------|
| 197-199 | `set_send_interval` | **Корректно.** Публичный setter, `max(0.001, ...)` — защита от нуля. |
| 243-306 | `_send_loop` | **Корректно.** Mode tracking + count reset (257-262), `_send_stop.wait` вместо `time.sleep` (268, 280, 290, 300, 302, 306), heartbeat every `hb_every`. |
| 259-262 | Mode reset | **Корректно.** `current_mode != last_mode` → `count = 0`. Предотвращает heartbeat drift. |
| 264-269 | Brightness update | **Корректно.** Dynamic brightness при любом изменении, даже в per_led. |
| 308-324 | `_safe_write` | **Корректно.** Lock, None/is_open checks, silent error handling. |

### `soulight/protocol/bridge.py` (325 строк)

| Строки | Что | Результат |
|--------|-----|----------|
| 14-21 | Constants | **Корректно.** `BRIDGE_DIR` удалён (был неиспользуемым). `BEELIGHT_DIR` — hardcoded путь. |
| Остальное | Без изменений | **Корректно.** Reflection, packet generation, heartbeat cache — не затронуто коммитом. |

### `soulight/color_preset.py` (72 строки)

| Строки | Что | Результат |
|--------|-----|----------|
| 24 | `_last_saved = None` | **Корректно.** Инициализация. |
| 26-27 | `_snapshot` | **Корректно.** dict с r/g/b/brightness. |
| 39-49 | `save` | **Корректно** по логике. См. ISSUE-7 — `_last_saved` обновляется до записи. |
| 51-67 | `load` | **Корректно.** `_last_saved = self._snapshot()` после load — предотвращает лишний save. |

### `soulight/led_config.py` (202 строки)

| Строки | Что | Результат |
|--------|-----|----------|
| 102-105 | `remaining` | **Корректно.** `max_total` параметр позволяет UI учитывать offset. |
| 107-115 | `set_count` | **Корректно.** `max_total` передаётся в `remaining`, `max(0, min(...))` — clamping. |
| Остальное | Без изменений | **Корректно.** |

### `soulight/scenes/engine.py` (124 строки)

| Строки | Что | Результат |
|--------|-----|----------|
| 44 | `_layout_leds = None` в `__init__` | **Корректно.** Заменяет `hasattr` check. |
| 110-113 | Layout mask | **Корректно.** Прямая проверка `self._layout_leds` вместо `hasattr(self, '_layout_leds')`. `logical_index < len(colors)` — защита IndexError. |

### `soulight/ui/led_config_widget.py` (704 строки, проверены 560-679)

| Строки | Что | Результат |
|--------|-----|----------|
| 570-577 | `_on_preview_toggled` | **Корректно.** `_maybe_send_preview()` только при включении. |
| 579-591 | `_on_count_changed` | **Корректно.** `max_total=self._effective_max()` — учитывает offset. SpinBox sync с `blockSignals`. |
| 593-603 | `_on_corner/dir_changed` | **Корректно.** `_maybe_send_preview` после изменения. |
| 605-612 | `_on_offset_changed` | **Корректно.** Offset устанавливается, `_update_total` обновляет лимиты. |
| 635-637 | `_effective_max` | **Корректно.** `MAX_LEDS - start_offset`. |
| 639-641 | `_is_valid_config` | **Корректно.** `total + offset <= MAX_LEDS`. |
| 643-653 | `_maybe_send_preview` | **Корректно.** Блокирует preview если невалиден, tooltip warning. |
| 655-669 | `_update_total` | **Корректно.** Red label если `total > eff_max`, SpinBox max update. |
| 671-679 | `_on_reset` | **Корректно.** `LEDConfig()` + `save()` + UI sync. |

### `soulight/ui/main_window.py` (1659 строк, проверены 600-629, 813-822, 1530-1659)

| Строки | Что | Результат |
|--------|-----|----------|
| 616-624 | DXCam checkbox | **Корректно.** `setChecked(True)`, `setEnabled(DXCAM_AVAILABLE)`, tooltip если нет. |
| 698-702 | `_mirror_prefer_dxcam` | **Корректно.** `DXCAM_AVAILABLE` check + checkbox state. |
| 813-822 | `_rebuild_mirror_engine` | **Корректно.** `prefer_dxcam` в dict для MirrorWorker. |
| 1534-1557 | Audio slider handlers | **Корректно.** Gain/shift/fps → engine setters. FPS update → status refresh. |
| 1559-1599 | `_start_audio` | **Корректно.** New AudioEngine per start, layout mask if not full LED, QThread + signal connections, sensitivity/gain/shift set after start. |
| 1601-1613 | `_stop_audio` | **Корректно.** `stop()` → `quit()` → `wait(2000)`, cleanup, UI reset. |
| 1615-1620 | `_on_audio_frame_ready` | **Корректно.** `_audio_active` + `_driver.connected` guard. TODO про hardware testing — честно. |
| 1622-1625 | `_on_audio_error` | **BUG-1.** Вызывает `_stop_audio()` который затирает error message. |
| 1627-1637 | `_on_audio_status_changed` | **BUG-1.** `else`-ветка перезаписывает error message. |
| 1643-1646 | `_on_speed_mode_clicked` | **Корректно.** `set_send_interval` — публичный setter. |
| 1650-1657 | `closeEvent` | **Корректно.** Stop mirroring/scenes/audio, disconnect. |

---

## Сводка

| # | Тип | Файл | Описание |
|---|------|------|----------|
| BUG-1 | Bug | `ui/main_window.py` | Error message полностью затирается: "Error" → "Idle" |
| BUG-2 | Bug | `screen_mirroring/screen_capture.py` | DXCam disable при 1-й ошибке, 3-error порог — мёртвый код |
| BUG-3 | Bug | `run_silent.bat` | Третий fallback использует `pythonw`, которого уже нет в PATH |
| ISSUE-1 | Minor | `audio/engine.py` | `CoUninitialize` без проверки успешности `CoInitializeEx` |
| ISSUE-2 | Minor | `screen_mirroring/screen_capture.py` | Нет `CoUninitialize` для DXCam COM |
| ISSUE-3 | Minor | `screen_mirroring/screen_capture.py` | `get_monitor_geometry` не использует persistent mss |
| ISSUE-4 | Minor | `audio/modes.py` | `_WAVE_CACHE` не очищается при смене параметров |
| ISSUE-5 | Minor | `audio/modes.py` | `wave` mode: первые ~10% LED получают одинаковый цвет |
| ISSUE-6 | Minor | `audio/modes.py` | `electronic` и `pulse` используют одинаковый ключ `"pulse"` в history |
| ISSUE-7 | Minor | `color_preset.py` | `_last_saved` обновляется до записи на диск |
