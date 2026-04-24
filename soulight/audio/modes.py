# modes.py — Алгоритмы аудио-реакции для LED.
#
# Превращают FFT-спектр в RGB массив для ленты.
# Три режима: Spectrum, Electronic, Lyricism.

import math
from typing import List, Tuple

import numpy as np


def _clamp(v: int) -> int:
    return max(0, min(255, int(v)))


def _hsv(h: float, s: float, v: float) -> Tuple[int, int, int]:
    import colorsys
    r, g, b = colorsys.hsv_to_rgb(h % 1.0, s, v)
    return (_clamp(r * 255), _clamp(g * 255), _clamp(b * 255))


def _energy_band(magnitudes: np.ndarray, freq_bins: np.ndarray, low_hz: float, high_hz: float) -> float:
    """Суммарная энергия в заданном частотном диапазоне."""
    mask = (freq_bins >= low_hz) & (freq_bins <= high_hz)
    if not np.any(mask):
        return 0.0
    return float(np.mean(magnitudes[mask]))


def spectrum(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Spectrum: частотный анализ.
    Низкие частоты → красный/оранжевый, средние → зелёный, высокие → синий/фиолетовый.
    """
    sensitivity = params.get("sensitivity", 1.5)
    bass = _energy_band(magnitudes, freq_bins, 20, 150)
    mid = _energy_band(magnitudes, freq_bins, 150, 2000)
    treble = _energy_band(magnitudes, freq_bins, 2000, 16000)

    bass = min(1.0, bass * sensitivity)
    mid = min(1.0, mid * sensitivity)
    treble = min(1.0, treble * sensitivity)

    # bass → красный, mid → зелёный, treble → синий
    colors = []
    for i in range(led_count):
        t = i / max(1, led_count - 1)
        if t < 0.33:
            intensity = bass * (1.0 - t / 0.33) + mid * (t / 0.33)
            colors.append((_clamp(intensity * 255), _clamp(intensity * 40), 0))
        elif t < 0.66:
            intensity = mid
            colors.append((_clamp(intensity * 40), _clamp(intensity * 255), _clamp(intensity * 40)))
        else:
            intensity = treble
            colors.append((_clamp(intensity * 80), _clamp(intensity * 40), _clamp(intensity * 255)))
    return colors


def electronic(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Electronic: пульсация под бит.
    Фокус на bass + kick, резкие яркие всплески.
    """
    sensitivity = params.get("sensitivity", 2.0)
    bass = _energy_band(magnitudes, freq_bins, 30, 250)
    beat = min(1.0, bass * sensitivity)

    # Бит определяет общую яркость, цвет смещается по Hue
    hue = params.get("base_hue", 0.0)
    colors = []
    for i in range(led_count):
        local_beat = beat * (0.7 + 0.3 * math.sin(i * 0.5))
        v = local_beat
        colors.append(_hsv(hue, 1.0, v))
    return colors


def lyricism(
    magnitudes: np.ndarray,
    freq_bins: np.ndarray,
    led_count: int,
    params: dict,
) -> List[Tuple[int, int, int]]:
    """
    Lyricism: плавные переходы под мелодию.
    Мягкое реагирование на общую громкость, медленные Hue-сдвиги.
    """
    sensitivity = params.get("sensitivity", 1.2)
    total_energy = float(np.mean(magnitudes)) * sensitivity
    total_energy = min(1.0, total_energy)

    # Hue плавно дрейфует
    hue_offset = params.get("hue_offset", 0.0)
    hue = (hue_offset + total_energy * 0.2) % 1.0

    colors = []
    for i in range(led_count):
        wave = math.sin(i * 0.15) * 0.15
        local_v = total_energy * (0.6 + wave)
        colors.append(_hsv((hue + i * 0.02) % 1.0, 0.85, local_v))
    return colors


# region Реестр режимов

AUDIO_MODES = {
    "spectrum": spectrum,
    "electronic": electronic,
    "lyricism": lyricism,
}

MODE_LABELS = {
    "spectrum": "Spectrum",
    "electronic": "Electronic",
    "lyricism": "Lyricism",
}

# endregion
