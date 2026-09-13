# serial_driver.py — Управление COM-портом для отправки LP пакетов.
#
# Открывает serial соединение, выполняет handshake (heartbeat, switch ON,
# PC mode), и предоставляет метод set_color() для установки цвета.
# Background thread непрерывно отправляет brightness + color + heartbeat
# каждые ~70ms для стабильного горения (контроллер требует постоянного потока).

import os
import sys
import threading
import time
import serial  # pyserial
import serial.tools.list_ports

from soulight.protocol.bridge import BeelightBridge
from soulight.protocol.native_bridge import NativeBridge


def get_default_port():
    env_port = os.getenv("SOULIGHT_PORT")
    if env_port:
        return env_port
    if sys.platform != "win32":
        try:
            ports = [p.device for p in serial.tools.list_ports.comports()]
            for dev in ports:
                if "ttyUSB" in dev or "ttyACM" in dev:
                    return dev
            if ports:
                return ports[0]
        except Exception:
            pass
        return "/dev/ttyUSB0"
    return "COM7"


# Настройки по умолчанию
DEFAULT_PORT = get_default_port()
DEFAULT_BAUD = 500000

# Эти константы описывают только безопасную оценку practical throughput.
# Мы не ускоряем transport path, а лишь честно сообщаем UI,
# сколько кадров в секунду сейчас имеет смысл запрашивать.
PER_LED_FRAME_SLEEP_OVERHEAD = 0.005
MIRROR_FPS_SAFETY_MARGIN = 0.98


def _clamp_ch(v: float) -> int:
    return max(0, min(255, int(round(v))))


class LEDDriver:
    """
    Драйвер LED ленты — управляет serial соединением и отправкой пакетов.

    Использует BeelightBridge для генерации wire-format пакетов
    и pyserial для отправки через COM порт.

    Типичное использование:
        driver = LEDDriver()
        driver.connect()
        driver.set_color(255, 0, 255)  # Purple
        driver.set_color(0, 255, 0)    # Green
        driver.disconnect()
    """

    def __init__(self, port=DEFAULT_PORT, baud=DEFAULT_BAUD, protocol=None):
        # Параметры serial соединения
        self._port_name = port
        self._baud = baud
        # pyserial объект
        self._serial = None
        # Генератор пакетов: BeelightBridge (.NET reflection) или
        # NativeBridge (чистый Python). Выбор: аргумент protocol
        # ("beelight"/"native"/"auto"), затем env SOULIGHT_PROTOCOL.
        self._protocol = (protocol or os.getenv("SOULIGHT_PROTOCOL") or "auto").lower()
        self._bridge = self._make_bridge()
        # Флаг подключения
        self._connected = False
        # Background send thread — непрерывно шлёт brightness+color+heartbeat
        self._send_thread = None
        self._send_stop = threading.Event()
        # Lock для потокобезопасной записи в serial
        self._write_lock = threading.Lock()
        # Текущий цвет (None = не задан, лента не горит активно).
        # Хранится как float-tuple: send-loop плавно лерпит его к
        # _target_color (fade ~0.3s при каждом set_color).
        self._current_color = None
        self._target_color = None
        # Per-LED цвета: список [(r, g, b), ...] для каждого LED
        # Если не None — используется вместо _current_color (приоритет)
        self._current_per_led = None
        # Версия буфера per-LED — растёт при каждом set_per_led_colors.
        # Send-loop шлёт пакет только при смене версии (иначе при audio 20 FPS
        # драйвер слал бы 66 одинаковых 240-байтных пакетов в секунду).
        self._per_led_version = 0
        # Яркость: текущее (float, лерпится) и целевое значение 0-255.
        # В wire-пакет конвертируется в hardware dimmer 0-1000 (_hw_dimmer).
        self._brightness = 255.0
        self._target_brightness = 255
        # Цветовая температура: множители каналов (r,g,b), 1.0 = без сдвига.
        # Применяется в send-loop при сборке пакета, не портит исходный буфер.
        self._temp_rgb = (1.0, 1.0, 1.0)
        self._target_temp_rgb = (1.0, 1.0, 1.0)
        # Вывод включён/выключен (команда switch). False → send-loop шлёт
        # только heartbeat — иначе поток color/RGB пакетов мгновенно
        # переключает ленту обратно и Off визуально не работает.
        self._output_on = True
        # Интервал между пакетами в секундах
        # 15ms позволяет отсылать до 66 пакетов в секунду (честные 60 FPS для mirroring)
        self._send_interval = 0.015
        # Heartbeat каждые N color пакетов
        self._hb_every = 10

    def _make_bridge(self):
        """
        Выбирает backend генерации пакетов.
        "native"   — всегда NativeBridge (чистый Python, кроссплатформен).
        "beelight" — всегда BeelightBridge (нужны Beelight.exe + .NET).
        "auto"     — BeelightBridge если pythonnet и Beelight.exe доступны,
                     иначе NativeBridge.
        """
        if self._protocol == "native":
            return NativeBridge()
        if self._protocol == "beelight":
            return BeelightBridge()
        # auto: BeelightBridge если окружение его поддерживает
        # (pythonnet + Beelight.exe — проверенный путь), иначе NativeBridge.
        try:
            from soulight.protocol import bridge as bm
            if bm.clr is not None and os.path.exists(bm.BEELIGHT_EXE):
                return BeelightBridge()
        except Exception:
            pass
        return NativeBridge()

    @property
    def connected(self):
        """Подключён ли драйвер к контроллеру."""
        return self._connected

    @property
    def bridge(self):
        """Доступ к BeelightBridge (для прямого вызова методов при необходимости)."""
        return self._bridge

    @property
    def practical_mirroring_max_fps(self):
        """
        Безопасная оценка practical FPS для screen mirroring.

        Здесь учитываем только текущую send cadence драйвера,
        чтобы UI не просил worker делать заметно больше кадров,
        чем transport path обычно успевает отправить на контроллер.
        """
        frame_time = self._send_interval + PER_LED_FRAME_SLEEP_OVERHEAD
        if frame_time <= 0:
            return 1
        estimated = int((1.0 / frame_time) * MIRROR_FPS_SAFETY_MARGIN)
        return max(1, estimated)

    def connect(self):
        """
        Подключается к контроллеру: инициализирует bridge, открывает
        serial порт, отправляет handshake (heartbeat + switch ON + PC mode).
        Запускает background heartbeat thread.
        Возвращает True при успехе.
        """
        if self._connected:
            return True

        # Инициализируем bridge (загрузка Beelight.exe)
        if not self._bridge.init():
            print("[Driver] Bridge инициализация провалилась")
            return False

        # Открываем serial порт. На POSIX просим exclusive доступ
        # (TIOCEXCL): без него вторая копия Soulight (например, headless
        # при запущенном GUI) открывает тот же порт и оба процесса пишут
        # пакеты попеременно — лента мерцает между двумя потоками кадров.
        open_kwargs = {}
        if sys.platform != "win32":
            open_kwargs["exclusive"] = True
        try:
            self._serial = serial.Serial(
                port=self._port_name,
                baudrate=self._baud,
                timeout=0.1,
                write_timeout=0.5,
                **open_kwargs,
            )
            # DTR и RTS нужны для пробуждения контроллера
            self._serial.dtr = True
            self._serial.rts = True
            time.sleep(0.3)
            # Очищаем входной буфер
            self._serial.read(self._serial.in_waiting or 1)
        except serial.SerialException as e:
            print(f"[Driver] Не удалось открыть {self._port_name}: {e}")
            if getattr(e, "errno", None) == 13 or "Permission denied" in str(e):
                print("[Driver] Нет прав на порт. Установи udev-правило:\n"
                      "  sudo cp 71-soulight-serial.rules /etc/udev/rules.d/\n"
                      "  sudo udevadm control --reload && sudo udevadm trigger")
            return False

        # Handshake: heartbeat burst → switch ON → PC mode
        self._handshake()

        # Handshake пишет dimmer=0 — синхронизируем текущее значение,
        # иначе первый пакет send-loop'а вспыхнул бы на старой яркости.
        # Яркость плавно доедет до _target_brightness (fade-in ~0.3s).
        self._brightness = 0.0

        # _connected поднимаем до старта потока — иначе set_* в этом окне
        # молча дропаются.
        self._connected = True

        # Запускаем background send loop (brightness + color + heartbeat)
        if self._send_thread is not None and self._send_thread.is_alive():
            # Старый поток завис и не умер — не плодим второй sender.
            print("[Driver] старый send thread ещё жив — переиспользуем")
        else:
            self._send_stop.clear()
            self._send_thread = threading.Thread(target=self._send_loop, daemon=True)
            self._send_thread.start()
        print(f"[Driver] Подключено к {self._port_name}")
        return True

    def disconnect(self):
        """
        Отключается: останавливает heartbeat, выключает ленту, закрывает порт.
        """
        if not self._connected:
            return

        # Останавливаем send thread. Если поток завис (write blocked),
        # флаг остаётся выставленным — иначе следующий connect() очистит
        # его и оживит второй send-loop параллельно с новым.
        self._send_stop.set()
        if self._send_thread is not None:
            self._send_thread.join(timeout=2.0)
            if self._send_thread.is_alive():
                print("[Driver] send thread не остановился — "
                      "флаг stop остаётся активным")

        # Выключаем ленту
        pkt = self._bridge.make_switch_packet(False)
        self._safe_write(pkt)
        time.sleep(0.1)

        # Закрываем порт
        if self._serial is not None and self._serial.is_open:
            self._serial.close()

        self._connected = False
        self._current_color = None
        self._target_color = None
        self._current_per_led = None
        print("[Driver] Отключено")

    def set_color(self, r, g, b):
        """
        Устанавливает единый цвет для всей ленты (RGB 0-255).
        Отключает per-LED режим. Переход плавный — send-loop лерпит
        текущий цвет к целевому.
        """
        if not self._connected:
            return
        # Уходим из per-LED режима: текущий solid-цвет протух (его ставили
        # до старта режима), поэтому фейд начинаем от усреднённого цвета
        # последнего кадра — иначе будет вспышка старого цвета.
        if self._current_per_led:
            n = len(self._current_per_led)
            ar = sum(c[0] for c in self._current_per_led) / n
            ag = sum(c[1] for c in self._current_per_led) / n
            ab = sum(c[2] for c in self._current_per_led) / n
            self._current_color = (ar, ag, ab)
        self._current_per_led = None
        self._target_color = (float(r), float(g), float(b))
        if self._current_color is None:
            self._current_color = self._target_color

    def set_per_led_colors(self, colors_rgb):
        """
        Устанавливает индивидуальные цвета для каждого LED.
        colors_rgb — список [(r, g, b), ...] длиной до 75.
        Перекрывает режим solid color.
        """
        if not self._connected:
            return
        try:
            buf = [(int(r), int(g), int(b)) for r, g, b in colors_rgb]
        except (TypeError, ValueError) as e:
            # Кривой кадр не должен убивать send-loop, но и не должен
            # исчезать молча — раньше это выглядело как «preview не работает».
            print(f"[Driver] set_per_led_colors: отброшен кадр: {e}")
            return
        self._current_per_led = buf
        self._per_led_version += 1

    def set_brightness(self, value):
        """
        Устанавливает целевую яркость (0-255). Send-loop плавно
        приближает текущую (fade ~0.3s) и шлёт dimmer-пакеты по мере
        изменения.
        """
        self._target_brightness = max(0, min(255, int(value)))

    def set_temperature(self, warmth: float):
        """
        Цветовая температура: warmth 0.0 = нейтрально, 1.0 = максимально
        тёплый свет (множители R=1.0, G~0.8, B~0.55). Лерпится в send-loop.
        """
        w = max(0.0, min(1.0, float(warmth)))
        self._target_temp_rgb = (1.0, 1.0 - 0.20 * w, 1.0 - 0.45 * w)

    def switch(self, on):
        """Включает (True) или выключает (False) ленту."""
        if not self._connected:
            return
        self._output_on = bool(on)
        pkt = self._bridge.make_switch_packet(on)
        self._safe_write(pkt)

    def set_send_interval(self, interval: float):
        """Публичный setter для интервала между пакетами (seconds).
        Пол <5мс — 240-байтный RGB-кадр при 500kbaud физически не уходит
        быстрее ~4-5мс, ниже только упираемся в write_timeout."""
        self._send_interval = max(0.005, float(interval))

    # === Внутренние методы ===

    def _hw_dimmer(self):
        """
        Конвертирует UI-яркость (0-255) в hardware dimmer контроллера (0-1000).
        Оба backend'а принимают hardware-единицу — так же, как оригинальное
        приложение шлёт GenBrightPackage(dimmer 0..1000).
        """
        return round(self._brightness * 1000 / 255)

    def _lerp_step(self, current, target, step=0.18):
        """Один шаг экспоненциального сближения (~0.3s при 66 итераций/с)."""
        return current + (target - current) * step

    def _handshake(self):
        """
        Отправляет начальную последовательность пакетов для пробуждения
        контроллера и переключения в PC mode.
        """
        hb = self._bridge.get_heartbeat()

        # Нативный backend повторяет полную стартовую последовательность
        # оригинального приложения (queries + sync_on + heartbeats).
        pre = getattr(self._bridge, "handshake_packets", None)
        if callable(pre):
            for pkt in pre():
                self._safe_write(pkt)
                time.sleep(0.05)
            try:
                self._serial.read(self._serial.in_waiting or 1)
            except Exception:
                pass

        # Heartbeat burst (5x) — пробуждение контроллера
        for _ in range(5):
            self._safe_write(hb)
            time.sleep(0.05)
        time.sleep(0.2)

        # Очищаем ответ контроллера
        try:
            self._serial.read(self._serial.in_waiting or 1)
        except Exception:
            pass

        # Яркость 0 ПЕРЕД включением — предотвращает вспышку при старте.
        # Контроллер запоминает последнее состояние; без этого switch ON
        # кратковременно показывает старый цвет на полной яркости.
        bright_zero = self._bridge.make_bright_packet(0)
        self._safe_write(bright_zero)
        time.sleep(0.02)

        # Switch ON (лента включается, но с яркостью 0 — темно)
        pkt = self._bridge.make_switch_packet(True)
        self._safe_write(pkt)
        time.sleep(0.02)

        # Повторяем яркость 0 для надёжности
        self._safe_write(bright_zero)
        time.sleep(0.02)

        # PC mode
        pkt = self._bridge.make_workmode_pc_packet()
        self._safe_write(pkt)
        time.sleep(0.05)

    def _send_loop(self):
        """
        Background thread: непрерывно отправляет brightness + color + heartbeat.
        Контроллер требует постоянного потока пакетов для стабильного горения.
        """
        hb = self._bridge.get_heartbeat()
        count = 0
        last_dimmer = None
        last_mode = None  # "per_led" | "solid" | "idle"
        last_sent_version = -1  # версия per-LED буфера, уже ушедшая на контроллер
        last_temp = None        # температура, при которой шёл последний кадр

        while not self._send_stop.is_set():
            try:
                # Плавные переходы: яркость, цвет и температура лерпятся
                # к целям (~0.3s). Диммер-пакет уходит при смене wire-значения.
                self._brightness = self._lerp_step(self._brightness, self._target_brightness)
                if abs(self._brightness - self._target_brightness) < 0.4:
                    self._brightness = float(self._target_brightness)
                tr, tg, tb = self._temp_rgb
                tt = self._target_temp_rgb
                # Snap при сходимости — иначе лерп асимптотически вязнет на
                # 0.9999… и per-led кадры вечно идут через scaled-ветку.
                def _temp_step(c, t):
                    v = self._lerp_step(c, t)
                    return t if abs(v - t) < 1e-3 else v
                self._temp_rgb = (
                    _temp_step(tr, tt[0]),
                    _temp_step(tg, tt[1]),
                    _temp_step(tb, tt[2]),
                )

                if not self._output_on:
                    # Лента выключена switch(False) — только heartbeat.
                    # Data-пакеты снова включили бы вывод, Off не работал бы.
                    self._safe_write(hb)
                    self._send_stop.wait(0.5)
                    continue

                # Версию читаем ДО списка: если producer обновит буфер между
                # чтениями, мы отправим новый список под старой версией и просто
                # пошлём его ещё раз — а не пропустим свежий кадр.
                version = self._per_led_version
                per_led = self._current_per_led
                color = self._current_color
                temp = self._temp_rgb

                # Определяем текущий режим и сбрасываем счётчик при смене,
                # чтобы heartbeat не дрейфовал между per_led/solid/none.
                current_mode = "per_led" if per_led is not None else ("solid" if color is not None else "idle")
                if current_mode != last_mode:
                    count = 0
                    last_mode = current_mode
                    last_sent_version = -1

                # Динамически отсылаем яркость при любом изменении (даже в per_led режиме)
                dimmer = self._hw_dimmer()
                if dimmer != last_dimmer:
                    bright_pkt = self._bridge.make_bright_packet(dimmer)
                    if bright_pkt is not None:
                        self._safe_write(bright_pkt)
                        self._send_stop.wait(0.005)
                        last_dimmer = dimmer

                if per_led is not None:
                    # Шлём RGB transfer только при новом кадре (версия сменилась)
                    # или смене температуры; периодический resend каждые ~40
                    # итераций — страховка от потерянных пакетов, heartbeat
                    # держит соединение.
                    if version != last_sent_version or temp != last_temp or count % 40 == 0:
                        if temp != (1.0, 1.0, 1.0):
                            scaled = [(_clamp_ch(r * temp[0]), _clamp_ch(g * temp[1]), _clamp_ch(b * temp[2]))
                                      for r, g, b in per_led]
                            rgb_pkt = self._bridge.make_rgb_transfer_packet(scaled)
                        else:
                            rgb_pkt = self._bridge.make_rgb_transfer_packet(per_led)
                        # Помечаем версию отправленной только при реальной
                        # посылке — иначе упавший пакет никогда не дойдёт.
                        if rgb_pkt is not None:
                            self._safe_write(rgb_pkt)
                            last_sent_version = version
                            last_temp = temp
                    count += 1

                    if count % self._hb_every == 0:
                        self._safe_write(hb)
                        self._send_stop.wait(0.005)

                    self._send_stop.wait(self._send_interval)

                elif color is not None:
                    # Плавный переход цвета: лерпим к цели, snap при сходимости.
                    tgt = self._target_color
                    if tgt is not None:
                        nr = self._lerp_step(color[0], tgt[0])
                        ng = self._lerp_step(color[1], tgt[1])
                        nb = self._lerp_step(color[2], tgt[2])
                        if abs(nr - tgt[0]) < 0.6 and abs(ng - tgt[1]) < 0.6 and abs(nb - tgt[2]) < 0.6:
                            nr, ng, nb = tgt
                        self._current_color = (nr, ng, nb)
                        color = self._current_color

                    # Solid Color режим:
                    # Контроллер сбрасывает dimmer иногда, поэтому дублируем яркость каждые 50 пакетов
                    if count % 50 == 0:
                        bright_pkt = self._bridge.make_bright_packet(dimmer)
                        if bright_pkt is not None:
                            self._safe_write(bright_pkt)
                            self._send_stop.wait(0.005)

                    # Color пакет (с учётом температуры)
                    r, g, b = color
                    color_pkt = self._bridge.make_color_packet(
                        _clamp_ch(r * temp[0]), _clamp_ch(g * temp[1]), _clamp_ch(b * temp[2]))
                    self._safe_write(color_pkt)
                    count += 1

                    # Heartbeat каждые N пакетов
                    if count % self._hb_every == 0:
                        self._safe_write(hb)
                        self._send_stop.wait(0.005)

                    self._send_stop.wait(self._send_interval)
                else:
                    # Нет цвета — только heartbeat раз в 500ms
                    self._safe_write(hb)
                    self._send_stop.wait(0.5)
            except Exception as e:
                # Исключение не должно убивать send-loop: без heartbeat'ов
                # контроллер выходит из PC mode и лента «сбрасывается».
                print(f"[Driver] send-loop error: {e}")
                self._send_stop.wait(0.1)

    def _safe_write(self, data):
        """
        Потокобезопасная отправка данных через serial.
        Игнорирует None data и ошибки записи.
        """
        if data is None or self._serial is None or not self._serial.is_open:
            return
        try:
            with self._write_lock:
                self._serial.write(data)
        except Exception:
            pass
