# screen_capture.py — Чтение кадра экрана для Screen Mirroring.
#
# Здесь лежит минимальный и понятный слой захвата экрана.
# Он пока не знает ничего про LED layout и отправку в контроллер.
# Его задача простая: вернуть свежий кадр primary monitor в удобном виде.

import threading
from dataclasses import dataclass
from typing import Dict, Optional

# Пытаемся импортировать dxcam для быстрого DirectX capture.
# Если его нет — fallback на mss (медленнее, но работает везде).
try:
    import dxcam
    DXCAM_AVAILABLE = True
except ImportError:
    DXCAM_AVAILABLE = False


# Этот dataclass хранит уже готовый кадр экрана.
# bgra — numpy array shape (H, W, 4) dtype=uint8, порядок каналов BGRA.
# numpy позволяет делать быстрое среднее по зонам без Python-циклов.
@dataclass
class CaptureFrame:
    width: int
    height: int
    bgra: Optional[object] = None  # np.ndarray (H, W, 4) uint8
    edge_regions: Optional[Dict[str, "CaptureRegion"]] = None


# Этот dataclass описывает один уже захваченный edge strip.
# origin нужен, чтобы sampler мог перевести global screen rect
# в локальные координаты конкретного strip buffer.
@dataclass(frozen=True)
class CaptureRegion:
    left: int
    top: int
    width: int
    height: int
    bgra: object  # np.ndarray (H, W, 4) uint8


# Этот dataclass хранит геометрию выбранного монитора.
# Она нужна отдельно, чтобы layout строился по реальному размеру экрана.
@dataclass(frozen=True)
class MonitorGeometry:
    left: int
    top: int
    width: int
    height: int


# Этот класс отвечает только за захват экрана.
# Он автоматически выбирает лучший backend: dxcam (быстро) или mss (fallback).
# Orchestration (таймеры, потоки) живёт снаружи.
class ScreenCapturer:
    def __init__(self, monitor_index: int = 1, prefer_dxcam: bool = False):
        # В mss monitor[0] — это виртуальный общий desktop.
        # Для Ambilight нужен monitor[1+], то есть конкретный physical monitor.
        self._monitor_index = int(monitor_index)
        # Persistent mss instance для экономии ~1.2ms per frame.
        # Если возникнет ошибка — пересоздаём fresh и продолжаем.
        self._sct = None
        self._mss_error_count = 0
        # DXCam camera instance для быстрого DirectX capture.
        # Создаётся лениво при первом вызове, если доступен.
        self._dxcam_camera = None
        self._use_dxcam = DXCAM_AVAILABLE and prefer_dxcam
        self._dxcam_failed = False
        # Эти счётчики нужны только для диагностики.
        # Мы логируем первые удачные вызовы и любые ошибки,
        # чтобы потом было проще понять, где ломается capture lifecycle.
        self._geometry_reads = 0
        self._capture_attempts = 0

    @property
    def monitor_index(self) -> int:
        return self._monitor_index

    # Закрывает capture resources. Вызывать при остановке mirroring или смене монитора.
    def close(self):
        # Закрываем persistent mss instance.
        if self._sct is not None:
            try:
                self._sct.close()
            except Exception:
                pass
            self._sct = None
        # Освобождаем dxcam camera если был создан.
        if self._dxcam_camera is not None:
            try:
                self._dxcam_camera.release()
            except Exception:
                pass
            self._dxcam_camera = None

    # Этот helper печатает компактные диагностические сообщения.
    # Он нужен для расследования редких mss/thread проблем без бесконечного spam.
    def _debug_log(self, stage: str, message: str) -> None:
        print(
            f"[ScreenCapturer:{stage}] "
            f"monitor={self._monitor_index} thread={threading.get_ident()} {message}"
        )

    # Этот метод возвращает реальную геометрию выбранного монитора.
    def get_monitor_geometry(self) -> MonitorGeometry:
        import mss
        # Геометрию тоже читаем через временный mss(), чтобы lifecycle capture backend
        # был полностью локален одному вызову и не переносился между кадрами/потоками.
        self._geometry_reads += 1
        try:
            with mss.mss() as temp_sct:
                monitor = self._get_monitor(temp_sct)
        except Exception as e:
            self._debug_log(
                "geometry-error",
                f"attempt={self._geometry_reads} {type(e).__name__}: {e}",
            )
            raise
        if self._geometry_reads <= 3:
            self._debug_log(
                "geometry",
                f"attempt={self._geometry_reads} size={int(monitor['width'])}x{int(monitor['height'])}",
            )
        return MonitorGeometry(
            left=int(monitor["left"]),
            top=int(monitor["top"]),
            width=int(monitor["width"]),
            height=int(monitor["height"]),
        )

    # Этот метод делает один снимок экрана.
    # Возвращает numpy BGRA array для быстрого sampling.
    def capture(self) -> CaptureFrame:
        import mss
        import numpy as np

        self._capture_attempts += 1
        try:
            with mss.mss() as sct:
                monitor = self._get_monitor(sct)
                shot = sct.grab(monitor)
        except Exception as e:
            self._debug_log(
                "capture-error",
                f"attempt={self._capture_attempts} {type(e).__name__}: {e}",
            )
            raise
        # np.array(shot) — mss ScreenShot поддерживает __array_interface__,
        # что позволяет numpy создать array напрямую без промежуточного bytes().
        try:
            bgra = np.array(shot, dtype=np.uint8).reshape(
                (int(shot.height), int(shot.width), 4)
            )
        except Exception as e:
            self._debug_log(
                "convert-error",
                f"attempt={self._capture_attempts} {type(e).__name__}: {e}",
            )
            raise
        if self._capture_attempts <= 3:
            self._debug_log(
                "capture",
                f"attempt={self._capture_attempts} size={int(shot.width)}x{int(shot.height)}",
            )
        return CaptureFrame(
            width=int(shot.width),
            height=int(shot.height),
            bgra=bgra,
        )

    # Этот метод захватывает только нужные полосы по краям экрана.
    # Так мы резко уменьшаем объём пикселей, которые вообще попадают в numpy.
    def capture_edges(self, edge_depth: int) -> CaptureFrame:
        # Пытаемся использовать dxcam если доступен и не было ошибок.
        if self._use_dxcam and not self._dxcam_failed:
            try:
                return self._capture_edges_dxcam(edge_depth)
            except Exception as e:
                self._debug_log(
                    "dxcam-fallback",
                    f"DXCam failed, falling back to mss: {type(e).__name__}: {e}",
                )
                self._dxcam_failed = True
                # Fallback на mss
        
        # MSS fallback path
        return self._capture_edges_mss(edge_depth)

    # DXCam capture path — использует DirectX Desktop Duplication API.
    # В 10-20x быстрее mss благодаря прямому доступу к GPU framebuffer.
    def _capture_edges_dxcam(self, edge_depth: int) -> CaptureFrame:
        import numpy as np

        self._capture_attempts += 1
        
        # Лениво создаём dxcam camera при первом вызове.
        if self._dxcam_camera is None:
            # device_idx=0 обычно primary monitor, но dxcam нумерует с 0.
            # Наш monitor_index=1 для primary → device_idx=0 в dxcam.
            device_idx = max(0, self._monitor_index - 1)
            camera = dxcam.create(device_idx=device_idx, output_idx=device_idx)
            if camera is None:
                raise RuntimeError(f"Failed to create dxcam camera for device {device_idx}")
            self._dxcam_camera = camera
            self._debug_log("dxcam-init", f"device={device_idx}")
        
        # Получаем размер монитора из dxcam.
        monitor_width = self._dxcam_camera.width
        monitor_height = self._dxcam_camera.height
        edge_depth = max(1, min(int(edge_depth), monitor_width, monitor_height))
        
        # DXCam возвращает RGB (не BGRA), поэтому нужна конверсия.
        # Захватываем 4 edge regions отдельно.
        edge_regions = {}
        
        # Top strip
        region = (0, 0, monitor_width, edge_depth)
        frame_rgb = self._dxcam_camera.grab(region=region)
        if frame_rgb is None:
            raise RuntimeError("DXCam grab returned None")
        # Конвертируем RGB → BGRA для совместимости с sampler.
        bgra = self._rgb_to_bgra(frame_rgb)
        edge_regions["top"] = CaptureRegion(
            left=0, top=0, width=monitor_width, height=edge_depth, bgra=bgra
        )
        
        # Bottom strip
        region = (0, monitor_height - edge_depth, monitor_width, monitor_height)
        frame_rgb = self._dxcam_camera.grab(region=region)
        if frame_rgb is None:
            raise RuntimeError("DXCam grab returned None")
        bgra = self._rgb_to_bgra(frame_rgb)
        edge_regions["bottom"] = CaptureRegion(
            left=0, top=monitor_height - edge_depth,
            width=monitor_width, height=edge_depth, bgra=bgra
        )
        
        # Left strip
        region = (0, 0, edge_depth, monitor_height)
        frame_rgb = self._dxcam_camera.grab(region=region)
        if frame_rgb is None:
            raise RuntimeError("DXCam grab returned None")
        bgra = self._rgb_to_bgra(frame_rgb)
        edge_regions["left"] = CaptureRegion(
            left=0, top=0, width=edge_depth, height=monitor_height, bgra=bgra
        )
        
        # Right strip
        region = (monitor_width - edge_depth, 0, monitor_width, monitor_height)
        frame_rgb = self._dxcam_camera.grab(region=region)
        if frame_rgb is None:
            raise RuntimeError("DXCam grab returned None")
        bgra = self._rgb_to_bgra(frame_rgb)
        edge_regions["right"] = CaptureRegion(
            left=monitor_width - edge_depth, top=0,
            width=edge_depth, height=monitor_height, bgra=bgra
        )
        
        if self._capture_attempts <= 3:
            self._debug_log(
                "dxcam-edge-capture",
                (
                    f"attempt={self._capture_attempts} full={monitor_width}x{monitor_height} "
                    f"depth={edge_depth}"
                ),
            )
        
        return CaptureFrame(
            width=monitor_width,
            height=monitor_height,
            bgra=None,
            edge_regions=edge_regions,
        )

    # MSS capture path — медленный GDI backend, но работает везде.
    # Оптимизирован: reuse mss instance + single bounding box grab вместо 4 отдельных.
    def _capture_edges_mss(self, edge_depth: int) -> CaptureFrame:
        import mss
        import numpy as np

        self._capture_attempts += 1
        
        # Создаём fresh mss instance каждый раз.
        # Нет persistent state между кадрами — это избегает thread-local проблем.
        sct = mss.mss()
        try:
            return self._do_mss_capture_with_sct(sct, edge_depth)
        finally:
            try:
                sct.close()
            except Exception:
                pass

    # Внутренний helper для реального mss capture с переданным sct.
    def _do_mss_capture_with_sct(self, sct, edge_depth: int) -> CaptureFrame:
        import numpy as np

        try:
            monitor = self._get_monitor(sct)
            monitor_left = int(monitor["left"])
            monitor_top = int(monitor["top"])
            monitor_width = int(monitor["width"])
            monitor_height = int(monitor["height"])
            edge_depth = max(1, min(int(edge_depth), monitor_width, monitor_height))

            # Захватываем один большой bounding box, покрывающий все 4 края.
            # Это один syscall вместо 4, что экономит ~15-20ms.
            full_box = {
                "left": monitor_left,
                "top": monitor_top,
                "width": monitor_width,
                "height": monitor_height,
            }
            shot = sct.grab(full_box)
            # Конвертируем весь кадр в numpy один раз.
            full_bgra = np.array(shot, dtype=np.uint8).reshape(
                (int(shot.height), int(shot.width), 4)
            )
            
            # Теперь нарезаем edge strips из уже захваченного full frame.
            # Это быстрые numpy slices без дополнительных syscalls.
            edge_regions = {
                "top": CaptureRegion(
                    left=0,
                    top=0,
                    width=monitor_width,
                    height=edge_depth,
                    bgra=full_bgra[:edge_depth, :, :].copy(),
                ),
                "bottom": CaptureRegion(
                    left=0,
                    top=monitor_height - edge_depth,
                    width=monitor_width,
                    height=edge_depth,
                    bgra=full_bgra[monitor_height - edge_depth:, :, :].copy(),
                ),
                "left": CaptureRegion(
                    left=0,
                    top=0,
                    width=edge_depth,
                    height=monitor_height,
                    bgra=full_bgra[:, :edge_depth, :].copy(),
                ),
                "right": CaptureRegion(
                    left=monitor_width - edge_depth,
                    top=0,
                    width=edge_depth,
                    height=monitor_height,
                    bgra=full_bgra[:, monitor_width - edge_depth:, :].copy(),
                ),
            }
        except Exception as e:
            self._mss_error_count += 1
            self._debug_log(
                "mss-edge-capture-error",
                f"attempt={self._capture_attempts} depth={edge_depth} errors={self._mss_error_count} {type(e).__name__}: {e}",
            )
            raise

        if self._capture_attempts <= 3:
            self._debug_log(
                "mss-edge-capture",
                (
                    f"attempt={self._capture_attempts} full={monitor_width}x{monitor_height} "
                    f"depth={edge_depth} (single grab + numpy slicing)"
                ),
            )

        return CaptureFrame(
            width=monitor_width,
            height=monitor_height,
            bgra=None,
            edge_regions=edge_regions,
        )

    # Внутренний helper для выбора монитора.
    def _get_monitor(self, sct) -> dict:
        monitors = sct.monitors
        if self._monitor_index < 1 or self._monitor_index >= len(monitors):
            raise IndexError(
                f"Monitor index {self._monitor_index} is out of range; "
                f"available monitor count is {len(monitors) - 1}"
            )
        return monitors[self._monitor_index]

    # Конвертирует RGB numpy array в BGRA для совместимости с sampler.
    # DXCam возвращает RGB, а наш sampler ожидает BGRA.
    @staticmethod
    def _rgb_to_bgra(rgb_array):
        import numpy as np
        h, w, _ = rgb_array.shape
        bgra = np.zeros((h, w, 4), dtype=np.uint8)
        bgra[:, :, 0] = rgb_array[:, :, 2]  # B
        bgra[:, :, 1] = rgb_array[:, :, 1]  # G
        bgra[:, :, 2] = rgb_array[:, :, 0]  # R
        bgra[:, :, 3] = 255  # A
        return bgra
