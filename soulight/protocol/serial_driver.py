# serial_driver.py — Управление COM-портом для отправки LP пакетов.
#
# Открывает serial соединение, выполняет handshake (heartbeat, switch ON,
# PC mode), и предоставляет метод set_color() для установки цвета.
# Background thread непрерывно отправляет brightness + color + heartbeat
# каждые ~70ms для стабильного горения (контроллер требует постоянного потока).

import threading
import time
import serial  # pyserial

from soulight.protocol.bridge import BeelightBridge

# Настройки по умолчанию
DEFAULT_PORT = "COM7"
DEFAULT_BAUD = 500000

# Эти константы описывают только безопасную оценку practical throughput.
# Мы не ускоряем transport path, а лишь честно сообщаем UI,
# сколько кадров в секунду сейчас имеет смысл запрашивать.
PER_LED_FRAME_SLEEP_OVERHEAD = 0.005
MIRROR_FPS_SAFETY_MARGIN = 0.98


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

    def __init__(self, port=DEFAULT_PORT, baud=DEFAULT_BAUD):
        # Параметры serial соединения
        self._port_name = port
        self._baud = baud
        # pyserial объект
        self._serial = None
        # Bridge к Beelight.exe для генерации пакетов
        self._bridge = BeelightBridge()
        # Флаг подключения
        self._connected = False
        # Background send thread — непрерывно шлёт brightness+color+heartbeat
        self._send_thread = None
        self._send_stop = threading.Event()
        # Lock для потокобезопасной записи в serial
        self._write_lock = threading.Lock()
        # Текущий цвет (None = не задан, лента не горит активно)
        self._current_color = None
        # Per-LED цвета: список [(r, g, b), ...] для каждого LED
        # Если не None — используется вместо _current_color (приоритет)
        self._current_per_led = None
        # Текущая яркость (0-255)
        self._brightness = 255
        # Интервал между пакетами в секундах
        # 15ms позволяет отсылать до 66 пакетов в секунду (честные 60 FPS для mirroring)
        self._send_interval = 0.015
        # Heartbeat каждые N color пакетов
        self._hb_every = 10

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

        # Открываем serial порт
        try:
            self._serial = serial.Serial(
                port=self._port_name,
                baudrate=self._baud,
                timeout=0.1,
                write_timeout=0.5,
            )
            # DTR и RTS нужны для пробуждения контроллера
            self._serial.dtr = True
            self._serial.rts = True
            time.sleep(0.3)
            # Очищаем входной буфер
            self._serial.read(self._serial.in_waiting or 1)
        except serial.SerialException as e:
            print(f"[Driver] Не удалось открыть {self._port_name}: {e}")
            return False

        # Handshake: heartbeat burst → switch ON → PC mode
        self._handshake()

        # Запускаем background send loop (brightness + color + heartbeat)
        self._send_stop.clear()
        self._send_thread = threading.Thread(target=self._send_loop, daemon=True)
        self._send_thread.start()

        self._connected = True
        print(f"[Driver] Подключено к {self._port_name}")
        return True

    def disconnect(self):
        """
        Отключается: останавливает heartbeat, выключает ленту, закрывает порт.
        """
        if not self._connected:
            return

        # Останавливаем send thread
        self._send_stop.set()
        if self._send_thread is not None:
            self._send_thread.join(timeout=2.0)

        # Выключаем ленту
        pkt = self._bridge.make_switch_packet(False)
        self._safe_write(pkt)
        time.sleep(0.1)

        # Закрываем порт
        if self._serial is not None and self._serial.is_open:
            self._serial.close()

        self._connected = False
        self._current_color = None
        print("[Driver] Отключено")

    def set_color(self, r, g, b):
        """
        Устанавливает единый цвет для всей ленты (RGB 0-255).
        Отключает per-LED режим.
        """
        if not self._connected:
            return
        self._current_per_led = None
        self._current_color = (r, g, b)

    def set_per_led_colors(self, colors_rgb):
        """
        Устанавливает индивидуальные цвета для каждого LED.
        colors_rgb — список [(r, g, b), ...] длиной до 75.
        Перекрывает режим solid color.
        """
        if not self._connected:
            return
        self._current_per_led = list(colors_rgb)

    def set_brightness(self, value):
        """
        Устанавливает яркость (0-255).
        Применяется автоматически background send loop.
        """
        self._brightness = max(0, min(255, int(value)))

    def switch(self, on):
        """Включает (True) или выключает (False) ленту."""
        if not self._connected:
            return
        pkt = self._bridge.make_switch_packet(on)
        self._safe_write(pkt)

    def set_send_interval(self, interval: float):
        """Публичный setter для интервала между пакетами (seconds)."""
        self._send_interval = max(0.001, float(interval))

    # === Внутренние методы ===

    def _handshake(self):
        """
        Отправляет начальную последовательность пакетов для пробуждения
        контроллера и переключения в PC mode.
        """
        hb = self._bridge.get_heartbeat()

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
        last_bright = None
        last_mode = None  # "per_led" | "solid" | "idle"

        while not self._send_stop.is_set():
            per_led = self._current_per_led
            color = self._current_color

            # Определяем текущий режим и сбрасываем счётчик при смене,
            # чтобы heartbeat не дрейфовал между per_led/solid/none.
            current_mode = "per_led" if per_led is not None else ("solid" if color is not None else "idle")
            if current_mode != last_mode:
                count = 0
                last_mode = current_mode

            # Динамически отсылаем яркость при любом изменении (даже в per_led режиме)
            if self._brightness != last_bright:
                bright_pkt = self._bridge.make_bright_packet(self._brightness)
                self._safe_write(bright_pkt)
                self._send_stop.wait(0.005)
                last_bright = self._brightness

            if per_led is not None:
                # Per-LED режим: отправляем RGB transfer
                rgb_pkt = self._bridge.make_rgb_transfer_packet(per_led)
                self._safe_write(rgb_pkt)
                count += 1

                if count % self._hb_every == 0:
                    self._safe_write(hb)

                self._send_stop.wait(self._send_interval)

            elif color is not None:
                r, g, b = color

                # Solid Color режим:
                # Контроллер сбрасывает dimmer иногда, поэтому дублируем яркость каждые 50 пакетов
                if count % 50 == 0:
                    bright_pkt = self._bridge.make_bright_packet(self._brightness)
                    self._safe_write(bright_pkt)
                    self._send_stop.wait(0.005)

                # Color пакет
                color_pkt = self._bridge.make_color_packet(r, g, b)
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

    def _safe_write(self, data):
        """
        Потокобезопасная отправка данных через serial.
        Пакеты с frame header (55 AA 5A) отправляются двумя частями:
        сначала 5-байтный заголовок, потом payload — контроллер ожидает
        именно такую последовательность (подтверждено capture анализом).
        Игнорирует None data и ошибки записи.
        """
        if data is None or self._serial is None or not self._serial.is_open:
            return
        try:
            with self._write_lock:
                self._serial.write(data)
                # Убрали искусственное разделение заголовка и паузу 15мс (time.sleep(0.003)),
                # так как CH340 / USB должен сам справляться с фреймами, а пауза убивала FPS.
        except Exception:
            pass
