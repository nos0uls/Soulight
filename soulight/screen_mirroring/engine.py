# engine.py — Высокоуровневый pipeline для Screen Mirroring.
#
# Этот модуль собирает foundation-части в один рабочий поток:
# 1) читаем размер монитора
# 2) строим layout по LEDConfig
# 3) захватываем кадр
# 4) усредняем цвета по зонам
# 5) получаем готовый RGB buffer для отправки в протокол
#
# Здесь пока нет потоков, таймеров и UI.
# Это намеренно: orchestration уровня приложения появится позже.

from dataclasses import dataclass
from typing import Optional

from soulight.led_config import LEDConfig
from soulight.screen_mirroring.layout import ScreenMirrorLayout, build_layout
from soulight.screen_mirroring.sampler import (
    FrameSmoother,
    SampledColors,
    sample_frame,
)
from soulight.screen_mirroring.screen_capture import ScreenCapturer


# Этот dataclass хранит результат полного pipeline за один кадр.
# Так следующий слой сразу получает и layout, и цвета, и плоский RGB buffer.
@dataclass(frozen=True)
class MirroringFrameResult:
    layout: ScreenMirrorLayout
    sampled: SampledColors
    # Этот буфер пока оставлен для совместимости структуры результата.
    # В текущем worker/UI пути он не используется, поэтому в hot path
    # мы больше не тратим CPU на его сборку каждый кадр.
    rgb_bytes: Optional[bytes]


# Этот класс — главный алгоритмический фасад для screen mirroring.
# Позже UI или driver смогут вызывать его по таймеру и брать готовый буфер.
class ScreenMirrorEngine:
    def __init__(
        self,
        config: LEDConfig,
        monitor_index: int = 1,
        edge_fraction: float = 0.08,
        smoothing_factor: float = 0.35,
        saturation_boost: float = 1.3,
        prefer_dxcam: bool = True,
    ):
        self._config = config
        self._capturer = ScreenCapturer(monitor_index=monitor_index, prefer_dxcam=prefer_dxcam)
        self._edge_fraction = float(edge_fraction)
        self._smoother = FrameSmoother(factor=smoothing_factor)
        self._saturation_boost = float(saturation_boost)
        self._layout: Optional[ScreenMirrorLayout] = None

    # Закрывает ресурсы (mss instance внутри capturer).
    # Вызывать при остановке mirroring.
    def close(self):
        self._capturer.close()

    @property
    def layout(self) -> Optional[ScreenMirrorLayout]:
        return self._layout

    @property
    def capturer(self) -> ScreenCapturer:
        return self._capturer

    @property
    def smoother(self) -> FrameSmoother:
        return self._smoother

    # Этот метод нужно вызвать при смене LEDConfig, монитора или режима.
    # Он пересчитывает зоны и сбрасывает smoothing history.
    def rebuild_layout(self) -> ScreenMirrorLayout:
        geometry = self._capturer.get_monitor_geometry()
        self._layout = build_layout(
            config=self._config,
            capture_width=geometry.width,
            capture_height=geometry.height,
            edge_fraction=self._edge_fraction,
        )
        self._smoother.reset()
        return self._layout

    # Этот метод делает полный цикл для одного кадра.
    # Если layout ещё не готов, он будет построен автоматически.
    def process_next_frame(self) -> MirroringFrameResult:
        if self._layout is None:
            self.rebuild_layout()

        # Захватываем только полосы по краям, потому что layout sample_rect
        # никогда не требует пиксели из центра экрана.
        frame = self._capturer.capture_edges(edge_depth=self._layout.edge_depth)
        sampled = sample_frame(
            frame=frame,
            layout=self._layout,
            smoother=self._smoother,
            saturation_boost=self._saturation_boost,
        )
        return MirroringFrameResult(
            layout=self._layout,
            sampled=sampled,
            rgb_bytes=None,
        )

    # Этот метод позволяет на лету обновить силу сглаживания.
    # UI потом сможет просто звать его при изменении настройки.
    def set_smoothing_factor(self, value: float) -> None:
        self._smoother.factor = value

    # Этот метод обновляет толщину sampling-зон по краям экрана.
    # После смены параметра layout надо перестроить.
    def set_edge_fraction(self, value: float) -> None:
        self._edge_fraction = float(value)
        self.rebuild_layout()

    # Насыщенность: 1.0 = без изменений, >1.0 = насыщеннее, <1.0 = бледнее.
    def set_saturation_boost(self, value: float) -> None:
        self._saturation_boost = float(value)
