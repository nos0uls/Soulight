# screen_capture.py — Чтение кадра экрана для Screen Mirroring.
#
# Здесь лежит минимальный и понятный слой захвата экрана.
# Он пока не знает ничего про LED layout и отправку в контроллер.
# Его задача простая: вернуть свежий кадр primary monitor в удобном виде.

import ctypes
import os
import sys
import threading
import time
from dataclasses import dataclass
from typing import Dict, Optional

# Windows-only: DPI awareness нужен для bettercam, чтобы получать реальное
# разрешение монитора (1920x1080 вместо 1536x864 при 125% scaling).
if sys.platform == "win32":
    try:
        ctypes.windll.shcore.SetProcessDpiAwareness(2)
    except Exception:
        pass

# Windows-only: COM-инициализация для bettercam в worker thread.
# bettercam использует comtypes/DirectX Desktop Duplication; без COM в потоке
# создание camera может падать с COM-ошибками или возвращать None.
# ВАЖНО: нужно использовать comtypes.CoInitialize(), а не raw CoInitializeEx,
# иначе comtypes не отследит COM state и srcdc в threading.local будет отсутствовать.
_BETTERCAM_COM_THREAD = threading.local()


def _init_bettercam_com():
    if sys.platform != "win32":
        return
    if getattr(_BETTERCAM_COM_THREAD, "initialized", False):
        return
    try:
        import comtypes
        comtypes.CoInitialize()
        _BETTERCAM_COM_THREAD.initialized = True
    except Exception as e:
        print(f"[BetterCam-COM] CoInitialize FAILED: {type(e).__name__}: {e}")
        raise


def _ensure_gpu_preference():
    """На hybrid-системах (NVIDIA Optimus) Desktop Duplication API работает
    только на iGPU (Intel). Устанавливаем GpuPreference=1 (Power Saving)
    для текущего python.exe, если ещё не установлен."""
    if sys.platform != "win32":
        return
    try:
        import winreg
        exe = sys.executable
        key_path = r"Software\Microsoft\DirectX\UserGpuPreferences"
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key_path, 0, winreg.KEY_READ) as key:
            try:
                value, _ = winreg.QueryValueEx(key, exe)
                if "GpuPreference=1" in value:
                    return
            except FileNotFoundError:
                pass
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key_path, 0, winreg.KEY_WRITE) as key:
            winreg.SetValueEx(key, exe, 0, winreg.REG_SZ, "GpuPreference=1;")
        print(f"[BetterCam] Set GpuPreference=1 (Intel iGPU) for {exe}")
        print("[BetterCam] Restart required for GPU preference to take effect.")
    except Exception:
        pass


# Пытаемся импортировать bettercam для быстрого DirectX capture.
# Если его нет — fallback на mss (медленнее, но работает везде).
try:
    import bettercam
    BETTERCAM_AVAILABLE = True
    _ensure_gpu_preference()
except ImportError:
    BETTERCAM_AVAILABLE = False

# Обратная совместимость: DXCAM_AVAILABLE для внешних проверок.
DXCAM_AVAILABLE = BETTERCAM_AVAILABLE


def _is_wayland() -> bool:
    """Запущены ли мы под Wayland-сессией (mss там захватывает чёрное)."""
    return (
        os.environ.get("XDG_SESSION_TYPE", "").lower() == "wayland"
        or bool(os.environ.get("WAYLAND_DISPLAY"))
    )


# Этот dataclass хранит уже готовый кадр экрана.
# rgb — numpy array shape (H, W, 3) dtype=uint8, порядок каналов RGB.
# numpy позволяет делать быстрое среднее по зонам без Python-циклов.
@dataclass
class CaptureFrame:
    width: int
    height: int
    rgb: Optional[object] = None  # np.ndarray (H, W, 3) uint8
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
    rgb: object  # np.ndarray (H, W, 3) uint8


# Этот dataclass хранит геометрию выбранного монитора.
# Она нужна отдельно, чтобы layout строился по реальному размеру экрана.
@dataclass(frozen=True)
class MonitorGeometry:
    left: int
    top: int
    width: int
    height: int


# Этот класс отвечает только за захват экрана.
# Он автоматически выбирает лучший backend: bettercam (быстро) или mss (fallback).
# Orchestration (таймеры, потоки) живёт снаружи.
class ScreenCapturer:
    def __init__(self, monitor_index: int = 1, prefer_dxcam: bool = False):
        # В mss monitor[0] — это виртуальный общий desktop.
        # Для Ambilight нужен monitor[1+], то есть конкретный physical monitor.
        self._monitor_index = int(monitor_index)
        # Persistent mss instance для экономии ~1-2ms per frame.
        # Создаётся лениво в _get_sct() и живёт в том же потоке, что и используется.
        self._sct = None
        # Счётчик ошибок capture нужен только для диагностики.
        self._capture_errors = 0
        # DXCam camera instance для быстрого DirectX capture.
        # Создаётся лениво при первом вызове, если доступен.
        self._bettercam_camera = None
        self._last_bettercam_rgb = None
        self._last_edge_regions = None
        self._use_bettercam = BETTERCAM_AVAILABLE and prefer_dxcam
        self._bettercam_failed = False
        # Wayland/KWin backend: mss на Wayland видит только пустой
        # XWayland-слой (чёрный кадр). Вместо него используется
        # org.kde.KWin.ScreenShot2 через session D-Bus (raw пиксели
        # в pipe, без диалогов). Требует авторизации вызывающего
        # процесса — см. install_desktop.sh (python-soulight shim).
        self._kwin_iface = None
        self._kwin_bus = None
        self._kwin_fail_count = 0
        self._kwin_disabled_until = 0.0  # backoff после серии ошибок
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
        # Отключаем KWin D-Bus соединение этого потока.
        if self._kwin_bus is not None:
            try:
                self._kwin_bus.disconnectFromBus(self._kwin_bus.name())
            except Exception:
                pass
            self._kwin_bus = None
            self._kwin_iface = None
        # Освобождаем bettercam camera если был создан.
        if self._bettercam_camera is not None:
            try:
                self._bettercam_camera.release()
            except Exception:
                pass
            self._bettercam_camera = None
        # Деинициализируем COM для bettercam, если был инициализирован в этом потоке.
        if getattr(_BETTERCAM_COM_THREAD, "initialized", False):
            try:
                import comtypes
                comtypes.CoUninitialize()
                _BETTERCAM_COM_THREAD.initialized = False
            except Exception:
                pass

    # Ленивый доступ к persistent mss instance.
    # При первой ошибке сбрасываем и создаём заново.
    def _get_sct(self):
        import mss
        if self._sct is None:
            try:
                self._sct = mss.mss()
            except Exception as e:
                self._debug_log("mss-init-error", f"{type(e).__name__}: {e}")
                raise
        return self._sct

    # Этот helper печатает компактные диагностические сообщения.
    # Он нужен для расследования редких mss/thread проблем без бесконечного spam.
    def _debug_log(self, stage: str, message: str) -> None:
        print(
            f"[ScreenCapturer:{stage}] "
            f"monitor={self._monitor_index} thread={threading.get_ident()} {message}"
        )

    # Этот метод возвращает реальную геометрию выбранного монитора.
    def get_monitor_geometry(self) -> MonitorGeometry:
        # Используем persistent mss instance для консистентности с capture path.
        self._geometry_reads += 1
        try:
            sct = self._get_sct()
            monitor = self._get_monitor(sct)
        except Exception as e:
            self._debug_log(
                "geometry-error",
                f"attempt={self._geometry_reads} {type(e).__name__}: {e}",
            )
            self._sct = None
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
    # Возвращает numpy RGB array для быстрого sampling.
    def capture(self) -> CaptureFrame:
        import numpy as np

        self._capture_attempts += 1
        try:
            sct = self._get_sct()
            monitor = self._get_monitor(sct)
            shot = sct.grab(monitor)
        except Exception as e:
            self._capture_errors += 1
            self._debug_log(
                "capture-error",
                f"attempt={self._capture_attempts} errors={self._capture_errors} {type(e).__name__}: {e}",
            )
            # Сбрасываем persistent instance при ошибке — создадим заново при следующем вызове.
            self._sct = None
            raise
        # np.array(shot) — mss ScreenShot поддерживает __array_interface__,
        # что позволяет numpy создать array напрямую без промежуточного bytes().
        try:
            bgra = np.array(shot, dtype=np.uint8).reshape(
                (int(shot.height), int(shot.width), 4)
            )
            rgb = bgra[:, :, :3][:, :, ::-1].copy()  # BGRA → RGB
        except Exception as e:
            self._capture_errors += 1
            self._debug_log(
                "convert-error",
                f"attempt={self._capture_attempts} errors={self._capture_errors} {type(e).__name__}: {e}",
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
            rgb=rgb,
        )

    # Этот метод захватывает только нужные полосы по краям экрана.
    # Так мы резко уменьшаем объём пикселей, которые вообще попадают в numpy.
    def capture_edges(self, edge_depth: int) -> CaptureFrame:
        # Пытаемся использовать bettercam если доступен и не было ошибок.
        if self._use_bettercam and not self._bettercam_failed:
            try:
                return self._capture_edges_bettercam(edge_depth)
            except Exception as e:
                self._debug_log(
                    "bettercam-fallback",
                    f"BetterCam failed, falling back to mss: {type(e).__name__}: {e}",
                )
                # Но сбрасываем MSS instance, если COM был инициализирован —
                # comtypes.CoInitialize мог сломать persistent MSS state (srcdc).
                if getattr(_BETTERCAM_COM_THREAD, "initialized", False):
                    if self._sct is not None:
                        try:
                            self._sct.close()
                        except Exception:
                            pass
                        self._sct = None
        
        # Wayland: mss видит только XWayland-слой (чёрный кадр) —
        # пробуем KWin ScreenShot2, если он доступен и процесс
        # авторизован (desktop-файл с X-KDE-DBUS-Restricted-Interfaces).
        # Отключаем только после серии ошибок подряд и с backoff — разовый
        # хик KWin (lock screen, D-Bus timeout) не должен навсегда
        # отправлять нас в чёрный mss-кадр.
        if (_is_wayland() and sys.platform != "win32"
                and time.monotonic() >= self._kwin_disabled_until):
            try:
                frame = self._capture_edges_kwin(edge_depth)
                self._kwin_fail_count = 0
                return frame
            except Exception as e:
                self._kwin_fail_count += 1
                if self._kwin_fail_count >= 3:
                    self._kwin_disabled_until = time.monotonic() + 30.0
                    self._debug_log(
                        "kwin-unavailable",
                        f"KWin capture failed x{self._kwin_fail_count} "
                        f"({type(e).__name__}: {e}); fallback на mss 30с — "
                        "на Wayland это чёрный кадр: для mirroring запустите "
                        "приложение через ./install_desktop.sh ярлык",
                    )

        # MSS fallback path
        return self._capture_edges_mss(edge_depth)

    # BetterCam capture path — использует DirectX Desktop Duplication API.
    # В 10-20x быстрее mss благодаря прямому доступу к GPU framebuffer.
    def _capture_edges_bettercam(self, edge_depth: int) -> CaptureFrame:
        import numpy as np

        self._capture_attempts += 1
        device_idx = max(0, self._monitor_index - 1)

        try:
            # bettercam/comtypes требует COM в потоке. Инициализируем перед созданием camera.
            _init_bettercam_com()

            # Сбрасываем persistent MSS instance — он мог быть создан до CoInitialize
            # (например, в get_geometry), и его srcdc больше не валиден после смены COM state.
            if self._sct is not None:
                try:
                    self._sct.close()
                except Exception:
                    pass
                self._sct = None

            # Лениво создаём bettercam camera при первом вызове.
            if self._bettercam_camera is None:
                camera = bettercam.create(device_idx=device_idx, output_idx=device_idx)
                if camera is None:
                    raise RuntimeError(
                        f"bettercam.create({device_idx},{device_idx}) returned None; "
                        "Desktop Duplication недоступен для этого монитора/GPU."
                    )
                self._bettercam_camera = camera
                self._debug_log(
                    "bettercam-init",
                    f"device={device_idx} size={camera.width}x{camera.height}",
                )

            # Получаем размер монитора из bettercam.
            monitor_width = self._bettercam_camera.width
            monitor_height = self._bettercam_camera.height
            edge_depth = max(1, min(int(edge_depth), monitor_width, monitor_height))

            # Делаем один захват всего экрана.
            # BetterCam возвращает RGB numpy array или None (если экран не изменился).
            frame_rgb = self._bettercam_camera.grab()

            if frame_rgb is None:
                # Экран не обновился. Используем кэш edge_regions.
                if self._last_edge_regions is None:
                    # Ещё нет ни одного кадра — fallback на MSS.
                    return self._capture_edges_mss(edge_depth)
                edge_regions = self._last_edge_regions
            else:
                # Нарезаем 4 edge regions через numpy slices (view, без .copy()).
                # frame_rgb — view на shared memory; .copy() не нужен т.к. next grab
                # вернёт новый array, а этот останется в кэше до следующего изменения.
                rgb = np.ascontiguousarray(frame_rgb)
                edge_regions = {
                    "top": CaptureRegion(
                        left=0, top=0, width=monitor_width, height=edge_depth,
                        rgb=rgb[:edge_depth, :, :]
                    ),
                    "bottom": CaptureRegion(
                        left=0, top=monitor_height - edge_depth, width=monitor_width, height=edge_depth,
                        rgb=rgb[monitor_height - edge_depth:, :, :]
                    ),
                    "left": CaptureRegion(
                        left=0, top=0, width=edge_depth, height=monitor_height,
                        rgb=rgb[:, :edge_depth, :]
                    ),
                    "right": CaptureRegion(
                        left=monitor_width - edge_depth, top=0, width=edge_depth, height=monitor_height,
                        rgb=rgb[:, monitor_width - edge_depth:, :]
                    ),
                }
                self._last_edge_regions = edge_regions

        except Exception as e:
            # Детальное логирование, чтобы понять реальную причину фейла.
            import traceback
            self._capture_errors += 1
            self._debug_log(
                "bettercam-error",
                f"device={device_idx} errors={self._capture_errors} {type(e).__name__}: {e}",
            )
            if self._capture_errors <= 1:
                traceback.print_exc()
            if self._capture_errors >= 3:
                self._bettercam_failed = True
                self._debug_log(
                    "bettercam-disabled",
                    f"device={device_idx} disabled after {self._capture_errors} errors, fallback to MSS",
                )
                if getattr(_BETTERCAM_COM_THREAD, "initialized", False):
                    try:
                        import comtypes
                        comtypes.CoUninitialize()
                        _BETTERCAM_COM_THREAD.initialized = False
                        self._debug_log("bettercam-com-uninit", "CoUninitialize after disable")
                    except Exception:
                        pass
                if self._sct is not None:
                    try:
                        self._sct.close()
                    except Exception:
                        pass
                    self._sct = None
                    self._debug_log("mss-reset", "persistent mss reset after BetterCam COM uninit")
            raise

        if self._capture_attempts <= 3:
            self._debug_log(
                "bettercam-edge-capture",
                (
                    f"attempt={self._capture_attempts} full={monitor_width}x{monitor_height} "
                    f"depth={edge_depth} (single grab + caching)"
                ),
            )

        return CaptureFrame(
            width=monitor_width,
            height=monitor_height,
            rgb=None,
            edge_regions=edge_regions,
        )

    # MSS capture path — медленный GDI backend, но работает везде.
    # Оптимизирован: reuse persistent mss instance + single bounding box grab.
    def _capture_edges_mss(self, edge_depth: int) -> CaptureFrame:
        self._capture_attempts += 1
        try:
            sct = self._get_sct()
            return self._do_mss_capture_with_sct(sct, edge_depth)
        except Exception:
            # При ошибке сбрасываем persistent instance — следующий кадр создаст новый.
            self._sct = None
            raise

    # Внутренний helper для реального mss capture с переданным sct.
    # Делает 4 узких grab'а по краям вместо одного full-screen: на Linux/X11
    # (XGetImage) это ~60x дешевле — 1MB данных вместо 8MB на кадр,
    # и BGRA→RGB конвертируются только полосы, а не весь экран.
    def _do_mss_capture_with_sct(self, sct, edge_depth: int) -> CaptureFrame:
        import numpy as np

        try:
            monitor = self._get_monitor(sct)
            monitor_left = int(monitor["left"])
            monitor_top = int(monitor["top"])
            monitor_width = int(monitor["width"])
            monitor_height = int(monitor["height"])
            edge_depth = max(1, min(int(edge_depth), monitor_width, monitor_height))

            boxes = {
                "top": {
                    "left": monitor_left, "top": monitor_top,
                    "width": monitor_width, "height": edge_depth,
                },
                "bottom": {
                    "left": monitor_left, "top": monitor_top + monitor_height - edge_depth,
                    "width": monitor_width, "height": edge_depth,
                },
                "left": {
                    "left": monitor_left, "top": monitor_top,
                    "width": edge_depth, "height": monitor_height,
                },
                "right": {
                    "left": monitor_left + monitor_width - edge_depth, "top": monitor_top,
                    "width": edge_depth, "height": monitor_height,
                },
            }
            edge_regions = {}
            for side, box in boxes.items():
                shot = sct.grab(box)
                # np.frombuffer(shot.raw) — ~6x быстрее np.array(shot)
                # (нет копии через __array_interface__). RGB — strided view
                # на reversed BGR каналы: .copy() не нужен, shot.raw живёт
                # вместе с array через .base.
                rgb = np.frombuffer(shot.raw, dtype=np.uint8).reshape(
                    (int(shot.height), int(shot.width), 4)
                )[:, :, 2::-1]
                edge_regions[side] = CaptureRegion(
                    left=box["left"] - monitor_left,
                    top=box["top"] - monitor_top,
                    width=int(shot.width),
                    height=int(shot.height),
                    rgb=rgb,
                )
        except Exception as e:
            self._capture_errors += 1
            self._debug_log(
                "mss-edge-capture-error",
                f"attempt={self._capture_attempts} depth={edge_depth} errors={self._capture_errors} {type(e).__name__}: {e}",
            )
            raise

        if self._capture_attempts <= 3:
            self._debug_log(
                "mss-edge-capture",
                (
                    f"attempt={self._capture_attempts} full={monitor_width}x{monitor_height} "
                    f"depth={edge_depth} (4 strip grabs)"
                ),
            )

        return CaptureFrame(
            width=monitor_width,
            height=monitor_height,
            rgb=None,
            edge_regions=edge_regions,
        )

    # KWin ScreenShot2 (Wayland/KDE): один D-Bus вызов CaptureActiveScreen
    # пишет raw пиксели (ARGB32_Premultiplied, LE = BGRA) в наш pipe.
    # native-resolution=True → размер в физических пикселях совпадает
    # с геометрией, которую вернул mss (X11 видит physical размер).
    # ~40ms на 1920x1080 — примерно 25 FPS, что достаточно для ambilight.
    # Ограничение: только активный экран (single-monitor сценарий).
    def _capture_edges_kwin(self, edge_depth: int) -> CaptureFrame:
        import numpy as np

        self._capture_attempts += 1
        iface = self._get_kwin_iface()

        r_fd, w_fd = os.pipe2(os.O_CLOEXEC)
        chunks: list[bytes] = []

        def _drain():
            try:
                while True:
                    b = os.read(r_fd, 1 << 20)
                    if not b:
                        break
                    chunks.append(b)
            except OSError:
                pass

        reader = threading.Thread(target=_drain, daemon=True)
        reader.start()
        try:
            from PyQt6.QtDBus import QDBus, QDBusUnixFileDescriptor
            reply = iface.call(
                QDBus.CallMode.Block, "CaptureActiveScreen",
                {"native-resolution": True}, QDBusUnixFileDescriptor(w_fd),
            )
        finally:
            os.close(w_fd)
        reader.join(timeout=5.0)
        os.close(r_fd)

        from PyQt6.QtDBus import QDBusMessage
        if reply.type() == QDBusMessage.MessageType.ErrorMessage:
            raise RuntimeError(reply.arguments() or ["KWin screenshot failed"])

        info = reply.arguments()[0]
        fw = int(info["width"])
        fh = int(info["height"])
        stride = int(info["stride"])
        if info.get("type") != "raw" or int(info["format"]) not in (4, 5, 6):
            raise RuntimeError(f"KWin returned unsupported frame: {info}")

        buf = b"".join(chunks)
        if len(buf) < stride * fh:
            raise RuntimeError(f"KWin frame truncated: {len(buf)} < {stride * fh}")

        # BGRA → RGB view на reversed каналах; buf живёт через .base.
        bgra = np.frombuffer(buf, dtype=np.uint8).reshape(fh, stride)
        rgb = bgra[:, : fw * 4].reshape(fh, fw, 4)[:, :, 2::-1]
        d = max(1, min(int(edge_depth), fw, fh))
        edge_regions = {
            "top": CaptureRegion(0, 0, fw, d, rgb[:d, :]),
            "bottom": CaptureRegion(0, fh - d, fw, d, rgb[fh - d:, :]),
            "left": CaptureRegion(0, 0, d, fh, rgb[:, :d]),
            "right": CaptureRegion(fw - d, 0, d, fh, rgb[:, fw - d:]),
        }

        if self._capture_attempts <= 3:
            self._debug_log(
                "kwin-edge-capture",
                (
                    f"attempt={self._capture_attempts} full={fw}x{fh} "
                    f"depth={d} screen={info.get('screen')} (ScreenShot2 raw)"
                ),
            )

        return CaptureFrame(width=fw, height=fh, rgb=None, edge_regions=edge_regions)

    # Ленивое создание D-Bus интерфейса к KWin в ТЕКУЩЕМ потоке.
    # QDBusConnection привязана к потоку создания — поэтому именно
    # connectToBus с уникальным именем, а не sessionBus() (main-thread only).
    def _get_kwin_iface(self):
        if self._kwin_iface is not None:
            return self._kwin_iface
        from PyQt6.QtDBus import QDBusConnection, QDBusInterface
        name = f"soulight-cap-{id(self)}-{threading.get_ident()}"
        bus = QDBusConnection.connectToBus(
            QDBusConnection.BusType.SessionBus, name
        )
        if not bus.isConnected():
            raise RuntimeError("session D-Bus unavailable")
        iface = QDBusInterface(
            "org.kde.KWin.ScreenShot2",
            "/org/kde/KWin/ScreenShot2",
            "org.kde.KWin.ScreenShot2",
            bus,
        )
        if not iface.isValid():
            raise RuntimeError("org.kde.KWin.ScreenShot2 not found (not KWin?)")
        self._kwin_bus = bus
        self._kwin_iface = iface
        return iface

    # Внутренний helper для выбора монитора.
    def _get_monitor(self, sct) -> dict:
        monitors = sct.monitors
        if self._monitor_index < 1 or self._monitor_index >= len(monitors):
            raise IndexError(
                f"Monitor index {self._monitor_index} is out of range; "
                f"available monitor count is {len(monitors) - 1}"
            )
        return monitors[self._monitor_index]

