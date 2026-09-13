# modes.py — Алгоритмы аудио-реакции для LED.
#
# Превращают FFT-спектр в RGB массив для ленты.
# Режимы: Spectrum, Electronic, Lyricism, Pulse, Wave, Bass, Disco.

import colorsys
import math
from typing import List, Tuple

import numpy as np


# ---------------------------------------------------------------------------
# Утилиты
# ---------------------------------------------------------------------------

# Кэш для wave mode: интерполяционные координаты зависят только от
# размеров magnitudes и led_count, которые не меняются в runtime.
# Словарь может содержать только одну запись — последнюю.
_WAVE_CACHE = {}


def clear_wave_cache():
    """Очищает кэш wave mode при смене параметров (block_size, led_count)."""
    _WAVE_CACHE.clear()

def _clamp(v: float) -> int:
    return max(0, min(255, int(v)))


def _clampf(v: float) -> float:
    return max(0.0, min(1.0, float(v)))


def _hsv(h: float, s: float, v: float) -> Tuple[int, int, int]:
    r, g, b = colorsys.hsv_to_rgb(h % 1.0, _clampf(s), _clampf(v))
    return (_clamp(r * 255), _clamp(g * 255), _clamp(b * 255))


def _smooth(new_value: float, state_key: str, params: dict, factor: float) -> float:
    """
    Простая экспоненциальная сглаживалка между кадрами.
    state_key должен быть уникальным для каждого сглаживаемого значения.
    """
    history = params.setdefault("history", {})
    prev = history.get(state_key, new_value)
    result = prev + (new_value - prev) * factor
    history[state_key] = result
    return result


def _normalize_magnitudes(magnitudes: np.ndarray) -> np.ndarray:
    """Убирает NaN/Inf и базовый шум, возвращает безопасный массив."""
    if magnitudes is None or magnitudes.size == 0:
        return np.array([0.0])
    mags = np.nan_to_num(magnitudes, nan=0.0, posinf=0.0, neginf=0.0)
    mags = np.maximum(mags, 0.0)
    return mags


def _energy_band(magnitudes: np.ndarray, freq_bins: np.ndarray, low_hz: float, high_hz: float) -> float:
    """Средняя энергия в заданном частотном диапазоне."""
    if magnitudes.size == 0 or freq_bins.size == 0:
        return 0.0
    mask = (freq_bins >= low_hz) & (freq_bins <= high_hz)
    if not np.any(mask):
        return 0.0
    return float(np.mean(magnitudes[mask]))


def _energy_sum(magnitudes: np.ndarray, freq_bins: np.ndarray, low_hz: float, high_hz: float) -> float:
    """Суммарная энергия в заданном частотном диапазоне."""
    if magnitudes.size == 0 or freq_bins.size == 0:
        return 0.0
    mask = (freq_bins >= low_hz) & (freq_bins <= high_hz)
    if not np.any(mask):
        return 0.0
    return float(np.sum(magnitudes[mask]))


def _auto_gain(value: float, key: str, params: dict,
               floor: float = 20.0, decay: float = 0.985) -> float:
    """
    Нормировка энергии на бегущий пик: 1.0 = уровень недавнего максимума.
    Делает режимы независимыми от абсолютной громкости источника —
    реагируют на динамику, а не на raw FFT-шкалу (которая у реального
    loopback оказалась в ~10x ниже, чем ждали старые делители).

    floor — шумовой гейт: значения ниже него отдают 0 и НЕ обновляют
    пик. Иначе пик за пару секунд тишины затухал до шумовой полки,
    и лента «светилась» на шум — именно это выглядело как реакция
    на музыку после её остановки.
    Показатель 1.25 придавливает середину: тихие звуки не заливаются
    в полную яркость, чувствительность мягче.
    """
    hist = params.setdefault("history", {})
    if value < floor:
        return 0.0
    peak = max(hist.get(key, 0.0) * decay, value, floor)
    hist[key] = peak
    return _clampf((value / peak) ** 1.25)


# ---------------------------------------------------------------------------
# Режимы
# ---------------------------------------------------------------------------

def spectrum(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Spectrum: частотный анализ.
    Низкие частоты слева, высокие справа; логарифмическая шкала,
    плавная интерполяция, нормализация и сглаживание.
    """
    sensitivity = float(params.get("sensitivity", 1.5))
    gain = float(params.get("gain", 1.0))
    mags = _normalize_magnitudes(magnitudes)
    if led_count <= 0:
        return []
    if freq_bins.size < 2 or mags.size < 2:
        return [(0, 0, 0)] * led_count

    min_freq = max(20.0, freq_bins[1])
    max_freq = max(8000.0, freq_bins[-1])
    log_bins = np.logspace(math.log10(min_freq), math.log10(max_freq), led_count)

    # Векторизованный поиск ближайшего freq_bin для каждого log_bin.
    # O(led_count * log(freq_bins.size)) вместо O(led_count * freq_bins.size).
    indices = np.searchsorted(freq_bins, log_bins, side="left")
    lower = np.maximum(indices - 1, 0)
    upper = np.minimum(indices, len(freq_bins) - 1)
    nearest = np.where(
        np.abs(freq_bins[lower] - log_bins) <= np.abs(freq_bins[upper] - log_bins),
        lower,
        upper,
    )
    sampled_mags = mags[nearest] * sensitivity * gain

    # Адаптивная шкала: tanh нормируется на бегущий пик спектра,
    # а не на фиксированные 60 — иначе тихий/громкий источник даёт
    # стабильно тусклую или стабильно пересвеченную картину.
    # Ниже гейта (тишина) — сразу тёмная лента, без residual-мерцания.
    spec_max = float(np.max(sampled_mags))
    if spec_max < 5.0:
        return [(0, 0, 0)] * led_count
    _auto_gain(spec_max, "spec_peak", params, floor=5.0, decay=0.985)
    peak = max(float(params["history"]["spec_peak"]), 1e-3)

    colors = []
    hue_shift = float(params.get("color_shift", 0.0))
    for i, mag in enumerate(sampled_mags):
        value = math.tanh(mag / peak * 2.0)
        value = _smooth(value, f"spec_{i}", params, 0.35)
        hue = (i / led_count + hue_shift) % 1.0
        sat = 0.9 + value * 0.1
        colors.append(_hsv(hue, sat, value))
    return colors


def electronic(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Electronic: пульсация под бит.
    Фокус на bass + kick, сглаженные energy bands, beat-swell.
    """
    sensitivity = float(params.get("sensitivity", 2.0))
    gain = float(params.get("gain", 1.0))
    mags = _normalize_magnitudes(magnitudes)
    if led_count <= 0:
        return []
    if freq_bins.size < 2 or mags.size < 2:
        return [(0, 0, 0)] * led_count

    bass = _auto_gain(_energy_sum(mags, freq_bins, 20, 250) * sensitivity * gain,
                      "bass_raw", params)
    mid = _auto_gain(_energy_sum(mags, freq_bins, 250, 4000) * sensitivity * gain,
                     "mid_raw", params)
    treble = _auto_gain(
        _energy_sum(mags, freq_bins, 4000, max(8000.0, freq_bins[-1])) * sensitivity * gain,
        "treble_raw", params)

    bass = _smooth(bass, "bass", params, 0.35)
    mid = _smooth(mid, "mid", params, 0.35)
    treble = _smooth(treble, "treble", params, 0.35)

    pulse = _smooth(bass, "elec_pulse", params, 0.25)
    colors = []
    for i in range(led_count):
        t = abs((i / max(1, led_count - 1)) - 0.5) * 2.0
        intensity = max(0.0, pulse * (1.0 - t))
        r = _clamp((bass * 0.8 + pulse * 0.2) * 255)
        g = _clamp(mid * 255)
        b = _clamp(treble * 255)
        color = (
            _clamp(r * intensity + treble * 0.1 * 255),
            _clamp(g * intensity),
            _clamp(b * intensity + mid * 0.1 * 255),
        )
        colors.append(color)
    return colors


def lyricism(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Lyricism: плавные переходы под мелодию.
    Spectral centroid + сглаживание + волна по ленте.
    """
    sensitivity = float(params.get("sensitivity", 1.2))
    gain = float(params.get("gain", 1.0))
    mags = _normalize_magnitudes(magnitudes)
    if led_count <= 0:
        return []
    if freq_bins.size < 2 or mags.size < 2:
        return [(0, 0, 0)] * led_count

    total = float(np.sum(mags))
    if total == 0:
        centroid = 1000.0
    else:
        centroid = float(np.sum(freq_bins * mags) / total)

    # Средняя энергия по всем бинам — тихая величина (большинство
    # бинов ~0 даже в музыке), поэтому гейт ниже, чем у band-сумм.
    avg = float(np.mean(mags)) * sensitivity * gain
    energy = _auto_gain(avg, "avg_raw", params, floor=0.8)
    energy = _smooth(energy, "energy", params, 0.25)

    hue = (math.log10(max(100.0, centroid)) - 2.0) / 2.0
    hue = _clampf(hue)
    hue = _smooth(hue, "hue", params, 0.15)

    colors = []
    for i in range(led_count):
        t = abs((i / max(1, led_count - 1)) - 0.5) * 2.0
        v = energy * (1.0 - t * 0.5)
        colors.append(_hsv(hue + 0.05 * math.sin(i * 0.2), 0.85, v))
    return colors


def pulse(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Pulse: весь strip пульсирует единым цветом в такт басу.
    """
    sensitivity = float(params.get("sensitivity", 1.5))
    gain = float(params.get("gain", 1.0))
    mags = _normalize_magnitudes(magnitudes)
    if led_count <= 0:
        return []
    if freq_bins.size < 2 or mags.size < 2:
        return [(0, 0, 0)] * led_count

    bass = _auto_gain(_energy_sum(mags, freq_bins, 20, 250) * sensitivity * gain,
                      "bass_raw", params)
    pulse_raw = bass
    pulse = _smooth(pulse_raw, "pulse", params, 0.45)

    history = params.setdefault("history", {})
    prev = history.get("prev_pulse", pulse)
    attack = max(0.0, pulse - prev)
    history["prev_pulse"] = pulse

    mid = _energy_sum(mags, freq_bins, 250, 4000)
    hue = 0.0 if (bass + mid) < 1e-6 else bass / (bass + mid)
    hue = _smooth(hue, "pulse_hue", params, 0.2)
    hue = (hue + float(params.get("color_shift", 0.0))) % 1.0

    value = _clampf(pulse + attack * 0.5)
    colors = []
    for i in range(led_count):
        edge = abs((i / max(1, led_count - 1)) - 0.5) * 2.0
        v = value * (1.0 - edge * 0.2)
        colors.append(_hsv(hue, 0.9, v))
    return colors


def wave(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Wave: FFT bins напрямую отображаются на ленту как огибающая волны.
    """
    sensitivity = float(params.get("sensitivity", 1.5))
    gain = float(params.get("gain", 1.0))
    mags = _normalize_magnitudes(magnitudes)
    if led_count <= 0:
        return []
    if freq_bins.size < 2 or mags.size < 2:
        return [(0, 0, 0)] * led_count

    # Кэшируем координаты интерполяции, так как mags.size и led_count постоянны.
    cache_key = (mags.size, led_count)
    if cache_key not in _WAVE_CACHE:
        _WAVE_CACHE.clear()
        log_x = np.logspace(0, 1, mags.size) / 10.0
        _WAVE_CACHE[cache_key] = (
            np.linspace(log_x[0], log_x[-1], led_count),
            log_x,
        )
    xi, log_x = _WAVE_CACHE[cache_key]
    sampled = np.interp(xi, log_x, mags * sensitivity * gain)
    sampled = np.maximum(sampled - np.mean(sampled) * 0.3, 0.0)
    peak = float(np.max(sampled))
    # Без порога self-normalize разгоняет шумовую полку тишины до
    # полной яркости — лента светится в отсутствие звука.
    if peak < 1.0:
        return [(0, 0, 0)] * led_count
    sampled = sampled / peak

    colors = []
    hue_shift = float(params.get("color_shift", 0.0))
    for i, val in enumerate(sampled):
        v = _clampf(val)
        v = _smooth(v, f"wave_{i}", params, 0.4)
        hue = (i / led_count * 0.7 + hue_shift) % 1.0
        colors.append(_hsv(hue, 0.85, v))
    return colors


def bass(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Bass: низкие частоты управляют яркостью, цвет от красного к жёлтому/зелёному.
    """
    sensitivity = float(params.get("sensitivity", 1.5))
    gain = float(params.get("gain", 1.0))
    mags = _normalize_magnitudes(magnitudes)
    if led_count <= 0:
        return []
    if freq_bins.size < 2 or mags.size < 2:
        return [(0, 0, 0)] * led_count

    bass = _auto_gain(_energy_sum(mags, freq_bins, 20, 250) * sensitivity * gain,
                      "bass_raw", params)
    sub = _auto_gain(_energy_sum(mags, freq_bins, 20, 100) * sensitivity * gain,
                     "sub_raw", params)

    energy = _clampf(bass + sub * 0.5)
    energy = _smooth(energy, "bass_energy", params, 0.3)

    hue = _clampf(energy * 0.22 + float(params.get("color_shift", 0.0)))
    colors = []
    for i in range(led_count):
        t = abs((i / max(1, led_count - 1)) - 0.5) * 2.0
        v = energy * (1.0 - t * 0.5)
        colors.append(_hsv(hue, 0.95, v))
    return colors


def disco(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Disco: резкие вспышки на bass-транзиентах, цвет меняется каждый beat.
    """
    sensitivity = float(params.get("sensitivity", 1.5))
    gain = float(params.get("gain", 1.0))
    mags = _normalize_magnitudes(magnitudes)
    if led_count <= 0:
        return []
    if freq_bins.size < 2 or mags.size < 2:
        return [(0, 0, 0)] * led_count

    raw_bass = _energy_sum(mags, freq_bins, 20, 250) * sensitivity * gain
    bass = _auto_gain(raw_bass, "bass_raw", params)
    history = params.setdefault("history", {})
    # Вспышка на транзиенте: мгновенная raw-энергия выше сглаженного
    # фона. Нормированный bass тут только как индикатор «музыка играет» —
    # delta-условие по нему не работает: auto-gain держит его у ~1.0.
    baseline = _smooth(raw_bass, "disco_base", params, 0.25)
    flash = 1.0 if (bass > 0.5 and raw_bass > baseline * 1.35) else 0.0
    flash = _smooth(flash, "flash", params, 0.55)

    hue_acc = (history.get("hue_acc", 0.0) + bass * 0.015) % 1.0
    history["hue_acc"] = hue_acc
    hue = (hue_acc + float(params.get("color_shift", 0.0))) % 1.0

    colors = []
    for i in range(led_count):
        v = flash
        h = (hue + (i / led_count) * 0.15) % 1.0
        colors.append(_hsv(h, 0.9, v))
    return colors


# ---------------------------------------------------------------------------
# Реестр режимов
# ---------------------------------------------------------------------------

AUDIO_MODES = {
    "spectrum": spectrum,
    "electronic": electronic,
    "lyricism": lyricism,
    "pulse": pulse,
    "wave": wave,
    "bass": bass,
    "disco": disco,
}

MODE_LABELS = {
    "spectrum": "Spectrum",
    "electronic": "Electronic",
    "lyricism": "Lyricism",
    "pulse": "Pulse",
    "wave": "Wave",
    "bass": "Bass",
    "disco": "Disco",
}
