# main_window.py — Главное окно Soulight.
#
# Содержит Color Picker (палитра, RGB слайдеры, HEX ввод),
# Brightness slider, пресеты цветов, кнопку ON/OFF,
# и статус подключения.

from PyQt6.QtWidgets import (
    QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QLabel, QSlider, QPushButton, QLineEdit, QGridLayout,
    QGroupBox, QFrame, QSizePolicy, QMessageBox, QTabWidget,
    QComboBox,
)
from PyQt6.QtCore import Qt, QTimer, QThread, QMetaObject
from PyQt6.QtGui import QColor, QPainter, QLinearGradient, QMouseEvent, QFont

from soulight.led_config import SIDE_COLORS, MAX_LEDS
from soulight.protocol.serial_driver import LEDDriver
from soulight.ui.led_config_widget import LEDConfigPanel
from soulight.color_preset import ColorPreset
from soulight.screen_mirroring.layout import build_layout
from soulight.screen_mirroring.worker import MirrorWorker


# region Пресеты цветов — быстрые кнопки для частых цветов
COLOR_PRESETS = [
    ("Red",        255,   0,   0),
    ("Green",        0, 255,   0),
    ("Blue",         0,   0, 255),
    ("Purple",     255,   0, 255),
    ("Yellow",     255, 255,   0),
    ("Cyan",         0, 255, 255),
    ("Orange",     255, 128,   0),
    ("White",      255, 255, 255),
    ("Warm White", 255, 200, 100),
    ("Cool White", 200, 220, 255),
]
# endregion


# region ColorPreview — виджет предпросмотра текущего цвета
class ColorPreview(QFrame):
    """
    Прямоугольник, показывающий текущий выбранный цвет.
    Обновляется при каждом изменении RGB слайдеров.
    """

    def __init__(self, parent=None):
        super().__init__(parent)
        self._color = QColor(255, 0, 255)
        self.setMinimumSize(200, 80)
        self.setFrameShape(QFrame.Shape.Box)
        self.setStyleSheet("border: 2px solid #555; border-radius: 8px;")

    def set_color(self, r, g, b):
        """Обновляет цвет предпросмотра."""
        self._color = QColor(r, g, b)
        self.update()

    def paintEvent(self, event):
        """Рисует прямоугольник залитый текущим цветом."""
        p = QPainter(self)
        p.setRenderHint(QPainter.RenderHint.Antialiasing)
        p.setBrush(self._color)
        p.setPen(Qt.PenStyle.NoPen)
        p.drawRoundedRect(self.rect().adjusted(2, 2, -2, -2), 6, 6)
        p.end()
# endregion


# region MainWindow — главное окно приложения
class MainWindow(QMainWindow):
    """
    Главное окно Soulight.

    Содержит:
    - Color Preview (предпросмотр текущего цвета)
    - RGB слайдеры (0-255 каждый)
    - HEX ввод (#FF00FF)
    - Brightness slider (0-255)
    - Пресеты цветов (кнопки)
    - Кнопки Connect / Disconnect / ON / OFF
    - Статус подключения
    """

    def __init__(self):
        super().__init__()
        self.setWindowTitle("Soulight")
        self.setMinimumSize(520, 680)

        # LED driver (protocol bridge + serial)
        self._driver = LEDDriver()
        # Color preset (save/load last color)
        self._color_preset = ColorPreset()
        self._color_preset.load()
        # Track which tab is active
        self._current_tab = 0  # 0=Color, 1=LED Config
        # Счётчик и режим автоподключения при старте приложения.
        # Эти поля помогают аккуратно сделать до 5 попыток без блокировки UI.
        self._auto_connect_attempt = 0
        self._auto_connect_max_attempts = 5
        self._auto_connect_delay_ms = 1000
        self._is_auto_connecting = False
        # Screen mirroring: engine + background thread для capture/sample
        self._screen_mirror_engine = None
        self._screen_mirroring_active = False
        self._screen_mirror_timer = QTimer()
        self._screen_mirror_timer.timeout.connect(self._tick_screen_mirroring)
        self._mirror_restart_timer = QTimer()
        self._mirror_restart_timer.setSingleShot(True)
        self._mirror_restart_timer.setInterval(150)
        self._mirror_restart_timer.timeout.connect(self._restart_screen_mirroring)
        # Background thread: capture + sampling выполняются здесь, не в UI thread
        self._mirror_thread = None   # QThread
        self._mirror_worker = None   # MirrorWorker
        self._mirror_frame_pending = False  # Защита от накопления запросов
        # Текущие RGB значения
        self._r = 255
        self._g = 0
        self._b = 255

        # Таймер для debounce отправки цвета (чтобы слайдеры не спамили)
        self._send_timer = QTimer()
        self._send_timer.setSingleShot(True)
        self._send_timer.setInterval(50)  # 50ms debounce
        self._send_timer.timeout.connect(self._send_current_color)

        self._init_ui()
        # Load saved preset into UI (after widgets are created)
        self._load_preset_to_ui()
        self._update_preview()
        # Небольшая задержка даёт окну успеть показаться,
        # после чего запускаем автоподключение.
        QTimer.singleShot(200, self._start_auto_connect)

    def _init_ui(self):
        """Создаёт все виджеты и layout."""
        central = QWidget()
        self.setCentralWidget(central)
        layout = QVBoxLayout(central)
        layout.setSpacing(12)
        layout.setContentsMargins(16, 16, 16, 16)

        # === Статус и подключение (всегда видно, над табами) ===
        conn_layout = QHBoxLayout()
        self._status_label = QLabel("Disconnected")
        self._status_label.setStyleSheet("color: #cc3333; font-weight: bold;")
        conn_layout.addWidget(self._status_label)
        conn_layout.addStretch()

        self._btn_connect = QPushButton("Connect")
        self._btn_connect.clicked.connect(self._on_connect)
        conn_layout.addWidget(self._btn_connect)

        self._btn_disconnect = QPushButton("Disconnect")
        self._btn_disconnect.clicked.connect(self._on_disconnect)
        self._btn_disconnect.setEnabled(False)
        conn_layout.addWidget(self._btn_disconnect)
        layout.addLayout(conn_layout)

        # === Табы ===
        self._tabs = QTabWidget()
        layout.addWidget(self._tabs)

        # Вкладка 1: Color
        color_page = QWidget()
        color_layout = QVBoxLayout(color_page)
        color_layout.setSpacing(12)
        color_layout.setContentsMargins(8, 8, 8, 8)
        self._build_color_tab(color_layout)
        self._tabs.addTab(color_page, "Color")

        # Вкладка 2: LED Config
        self._led_config_panel = LEDConfigPanel()
        self._led_config_panel.config_confirmed.connect(self._on_led_config_confirmed)
        self._tabs.addTab(self._led_config_panel, "LED Config")

        # Вкладка 3: Screen Mirror
        screen_page = QWidget()
        screen_layout = QVBoxLayout(screen_page)
        screen_layout.setSpacing(12)
        screen_layout.setContentsMargins(8, 8, 8, 8)
        self._build_screen_mirror_tab(screen_layout)
        self._tabs.addTab(screen_page, "Screen Mirror")

        # Tab change handler для управления состоянием ленты
        self._tabs.currentChanged.connect(self._on_tab_changed)

    def _build_color_tab(self, layout):
        """Строит содержимое вкладки Color."""
        # === Color Preview ===
        self._preview = ColorPreview()
        layout.addWidget(self._preview)

        # === HEX ввод ===
        hex_layout = QHBoxLayout()
        hex_layout.addWidget(QLabel("HEX:"))
        self._hex_input = QLineEdit("#FF00FF")
        self._hex_input.setMaxLength(7)
        self._hex_input.setFixedWidth(100)
        self._hex_input.setFont(QFont("Consolas", 11))
        self._hex_input.returnPressed.connect(self._on_hex_changed)
        hex_layout.addWidget(self._hex_input)
        hex_layout.addStretch()
        layout.addLayout(hex_layout)

        # === RGB Слайдеры ===
        rgb_group = QGroupBox("Color (RGB)")
        rgb_layout = QVBoxLayout(rgb_group)

        self._slider_r, self._label_r = self._make_slider("R", 255, self._on_slider_changed)
        self._slider_g, self._label_g = self._make_slider("G", 0, self._on_slider_changed)
        self._slider_b, self._label_b = self._make_slider("B", 255, self._on_slider_changed)

        for label_name, slider, value_label in [
            ("R", self._slider_r, self._label_r),
            ("G", self._slider_g, self._label_g),
            ("B", self._slider_b, self._label_b),
        ]:
            row = QHBoxLayout()
            lbl = QLabel(label_name)
            lbl.setFixedWidth(20)
            lbl.setFont(QFont("Consolas", 11, QFont.Weight.Bold))
            row.addWidget(lbl)
            row.addWidget(slider)
            value_label.setFixedWidth(35)
            value_label.setAlignment(Qt.AlignmentFlag.AlignRight)
            row.addWidget(value_label)
            rgb_layout.addLayout(row)

        layout.addWidget(rgb_group)

        # === Brightness Slider ===
        bright_group = QGroupBox("Brightness")
        bright_layout = QHBoxLayout(bright_group)
        self._slider_bright = QSlider(Qt.Orientation.Horizontal)
        self._slider_bright.setRange(0, 255)
        self._slider_bright.setValue(255)
        self._slider_bright.valueChanged.connect(self._on_brightness_changed)
        self._label_bright = QLabel("255")
        self._label_bright.setFixedWidth(35)
        self._label_bright.setAlignment(Qt.AlignmentFlag.AlignRight)
        bright_layout.addWidget(self._slider_bright)
        bright_layout.addWidget(self._label_bright)
        layout.addWidget(bright_group)

        # === Пресеты цветов ===
        presets_group = QGroupBox("Presets")
        presets_layout = QGridLayout(presets_group)
        presets_layout.setSpacing(6)
        for i, (name, r, g, b) in enumerate(COLOR_PRESETS):
            btn = QPushButton(name)
            btn.setFixedHeight(30)
            # Цвет фона кнопки приближен к пресету
            text_color = "#000" if (r + g + b) > 380 else "#fff"
            btn.setStyleSheet(
                f"background-color: rgb({r},{g},{b}); color: {text_color}; "
                f"border: 1px solid #666; border-radius: 4px; font-weight: bold;"
            )
            btn.clicked.connect(lambda checked, rr=r, gg=g, bb=b: self._set_rgb(rr, gg, bb))
            presets_layout.addWidget(btn, i // 5, i % 5)
        layout.addWidget(presets_group)

        # === ON/OFF кнопки ===
        onoff_layout = QHBoxLayout()
        self._btn_on = QPushButton("LED ON")
        self._btn_on.setFixedHeight(36)
        self._btn_on.setStyleSheet("background-color: #2d8c2d; color: white; font-weight: bold; border-radius: 6px;")
        self._btn_on.clicked.connect(lambda: self._driver.switch(True))
        onoff_layout.addWidget(self._btn_on)

        self._btn_off = QPushButton("LED OFF")
        self._btn_off.setFixedHeight(36)
        self._btn_off.setStyleSheet("background-color: #cc3333; color: white; font-weight: bold; border-radius: 6px;")
        self._btn_off.clicked.connect(lambda: self._driver.switch(False))
        onoff_layout.addWidget(self._btn_off)
        layout.addLayout(onoff_layout)

        layout.addStretch()

    def _build_screen_mirror_tab(self, layout):
        """Строит вкладку Screen Mirror: выбор монитора, tuning, start/stop."""
        # Статус — Idle / Running / Error
        self._mirror_status_label = QLabel("Idle")
        self._mirror_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        layout.addWidget(self._mirror_status_label)

        # region Capture source — выбор монитора
        source_group = QGroupBox("Capture")
        source_layout = QVBoxLayout(source_group)

        monitor_row = QHBoxLayout()
        monitor_row.addWidget(QLabel("Monitor:"))
        self._mirror_monitor_combo = QComboBox()
        self._mirror_monitor_combo.currentIndexChanged.connect(
            self._on_mirror_monitor_changed
        )
        monitor_row.addWidget(self._mirror_monitor_combo, stretch=1)
        btn_refresh = QPushButton("Refresh")
        btn_refresh.clicked.connect(self._populate_monitor_combo)
        monitor_row.addWidget(btn_refresh)
        source_layout.addLayout(monitor_row)
        layout.addWidget(source_group)
        # endregion

        # region Tuning — edge depth, smoothing, fps
        tuning_group = QGroupBox("Tuning")
        tuning_layout = QVBoxLayout(tuning_group)

        # Edge depth — толщина полосы по краю экрана для sampling
        self._mirror_edge_slider, self._mirror_edge_label = self._make_slider(
            "Edge", 8, self._on_mirror_edge_changed
        )
        self._mirror_edge_slider.setRange(2, 20)
        self._mirror_edge_label.setText("8%")
        edge_row = QHBoxLayout()
        edge_row.addWidget(QLabel("Edge %"))
        edge_row.addWidget(self._mirror_edge_slider)
        self._mirror_edge_label.setFixedWidth(48)
        self._mirror_edge_label.setAlignment(Qt.AlignmentFlag.AlignRight)
        edge_row.addWidget(self._mirror_edge_label)
        tuning_layout.addLayout(edge_row)

        # Smoothing — сглаживание между кадрами
        self._mirror_smooth_slider, self._mirror_smooth_label = self._make_slider(
            "Smooth", 35, self._on_mirror_smooth_changed
        )
        self._mirror_smooth_slider.setRange(0, 95)
        self._mirror_smooth_label.setText("35%")
        smooth_row = QHBoxLayout()
        smooth_row.addWidget(QLabel("Smooth %"))
        smooth_row.addWidget(self._mirror_smooth_slider)
        self._mirror_smooth_label.setFixedWidth(48)
        self._mirror_smooth_label.setAlignment(Qt.AlignmentFlag.AlignRight)
        smooth_row.addWidget(self._mirror_smooth_label)
        tuning_layout.addLayout(smooth_row)

        # Saturation — усиление насыщенности (план 3.4)
        self._mirror_sat_slider, self._mirror_sat_label = self._make_slider(
            "Sat", 130, self._on_mirror_sat_changed
        )
        self._mirror_sat_slider.setRange(50, 250)
        self._mirror_sat_label.setText("1.3x")
        sat_row = QHBoxLayout()
        sat_row.addWidget(QLabel("Saturation"))
        sat_row.addWidget(self._mirror_sat_slider)
        self._mirror_sat_label.setFixedWidth(48)
        self._mirror_sat_label.setAlignment(Qt.AlignmentFlag.AlignRight)
        sat_row.addWidget(self._mirror_sat_label)
        tuning_layout.addLayout(sat_row)

        # FPS — частота кадров mirroring
        self._mirror_fps_slider, self._mirror_fps_label = self._make_slider(
            "FPS", 15, self._on_mirror_fps_changed
        )
        self._mirror_fps_slider.setRange(5, 30)
        self._mirror_fps_label.setText("15")
        fps_row = QHBoxLayout()
        fps_row.addWidget(QLabel("FPS"))
        fps_row.addWidget(self._mirror_fps_slider)
        self._mirror_fps_label.setFixedWidth(48)
        self._mirror_fps_label.setAlignment(Qt.AlignmentFlag.AlignRight)
        fps_row.addWidget(self._mirror_fps_label)
        tuning_layout.addLayout(fps_row)

        layout.addWidget(tuning_group)
        # endregion

        # region Start / Stop кнопки
        btn_row = QHBoxLayout()
        self._btn_mirror_start = QPushButton("Start")
        self._btn_mirror_start.setFixedHeight(36)
        self._btn_mirror_start.setStyleSheet(
            "background-color: #2d8c2d; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_mirror_start.clicked.connect(self._start_screen_mirroring)
        btn_row.addWidget(self._btn_mirror_start)

        self._btn_mirror_stop = QPushButton("Stop")
        self._btn_mirror_stop.setFixedHeight(36)
        self._btn_mirror_stop.setStyleSheet(
            "background-color: #cc3333; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_mirror_stop.setEnabled(False)
        self._btn_mirror_stop.clicked.connect(self._stop_screen_mirroring)
        btn_row.addWidget(self._btn_mirror_stop)
        layout.addLayout(btn_row)
        # endregion

        layout.addStretch()
        # Заполняем список мониторов при создании вкладки
        self._populate_monitor_combo()

    # region Screen Mirror helpers

    def _populate_monitor_combo(self):
        """Обновляет список доступных мониторов через mss."""
        prev = self._mirror_monitor_combo.currentData()
        self._mirror_monitor_combo.blockSignals(True)
        self._mirror_monitor_combo.clear()
        try:
            import mss
            with mss.mss() as sct:
                for idx, mon in enumerate(sct.monitors[1:], start=1):
                    w = int(mon.get("width", 0))
                    h = int(mon.get("height", 0))
                    self._mirror_monitor_combo.addItem(
                        f"{idx}: {w}x{h}", idx
                    )
        except Exception as e:
            self._mirror_monitor_combo.addItem(f"Error: {e}", 1)
        # Восстанавливаем предыдущий выбор если возможно
        if prev is not None:
            for i in range(self._mirror_monitor_combo.count()):
                if self._mirror_monitor_combo.itemData(i) == prev:
                    self._mirror_monitor_combo.setCurrentIndex(i)
                    break
        self._mirror_monitor_combo.blockSignals(False)

    def _mirror_monitor_index(self):
        """Выбранный monitor index (1-based, как в mss)."""
        d = self._mirror_monitor_combo.currentData()
        return int(d) if d is not None else 1

    def _mirror_edge_fraction(self):
        """Текущее значение edge depth как доля (0.02..0.20)."""
        return self._mirror_edge_slider.value() / 100.0

    def _mirror_smoothing_factor(self):
        """Текущее значение smoothing (0.0..0.95)."""
        return self._mirror_smooth_slider.value() / 100.0

    def _mirror_saturation_boost(self):
        """Текущее значение saturation boost (0.5..2.5)."""
        return self._mirror_sat_slider.value() / 100.0

    def _mirror_interval_ms(self):
        """Интервал таймера mirroring по текущему FPS."""
        fps = max(1, int(self._mirror_fps_slider.value()))
        return max(1, int(1000 / fps))

    def _update_mirror_status(self, text, color="#9399b2"):
        """Обновляет статус-label на вкладке Screen Mirror."""
        self._mirror_status_label.setText(text)
        self._mirror_status_label.setStyleSheet(
            f"color: {color}; font-weight: bold;"
        )

    def _rebuild_mirror_engine(self):
        """Собирает snapshot текущих mirroring-настроек для worker thread."""
        return {
            "config": self._led_config_panel.config,
            "monitor_index": self._mirror_monitor_index(),
            "edge_fraction": self._mirror_edge_fraction(),
            "smoothing_factor": self._mirror_smoothing_factor(),
            "saturation_boost": self._mirror_saturation_boost(),
        }

    def _restart_screen_mirroring(self):
        """
        Безопасно перезапускает mirroring.
        Это нужно для настроек, которые требуют новый engine/monitor/layout.
        """
        if not self._screen_mirroring_active:
            return
        self._stop_screen_mirroring(restore_output=False)
        self._start_screen_mirroring()

    def _queue_mirror_restart(self):
        """
        Планирует короткий debounce-restart mirroring.
        Это убирает race между UI thread и worker thread при rebuild layout/engine,
        и не дёргает stop/start на каждый тик слайдера.
        """
        if not self._screen_mirroring_active:
            return
        self._mirror_restart_timer.start()

    def _send_led_config_preview(self):
        """
        Отправляет на ленту preview раскладки из LED Config.
        Цвет каждой стороны берётся из SIDE_COLORS, выключенные LED — чёрные.
        """
        if not self._driver.connected:
            return

        cfg = self._led_config_panel.config
        if cfg.total + cfg.start_offset > MAX_LEDS:
            return

        # Для preview нужен только порядок LED, поэтому геометрия экрана условная.
        layout = build_layout(cfg, capture_width=100, capture_height=100, edge_fraction=0.08)
        colors = [(0, 0, 0)] * layout.physical_led_count
        for led in layout.leds:
            if led.enabled:
                colors[led.physical_index] = SIDE_COLORS.get(led.side, (255, 255, 255))
        self._driver.set_per_led_colors(colors)

    def _start_screen_mirroring(self):
        """
        Запускает screen mirroring: создаёт engine, background thread, таймер.
        Capture + sampling выполняются в отдельном потоке через MirrorWorker,
        чтобы UI оставался отзывчивым.
        """
        if not self._driver.connected:
            QMessageBox.warning(self, "Not connected",
                                "Connect to the controller first.")
            return
        # Создаём background thread и worker
        self._mirror_thread = QThread()
        worker_args = self._rebuild_mirror_engine()
        self._mirror_worker = MirrorWorker(**worker_args)
        self._mirror_worker.moveToThread(self._mirror_thread)
        # Сигналы: worker → UI thread (thread-safe через Qt signal queue)
        self._mirror_worker.frame_ready.connect(self._on_mirror_frame_ready)
        self._mirror_worker.error_occurred.connect(self._on_mirror_error)
        self._mirror_thread.finished.connect(self._mirror_worker.deleteLater)
        self._mirror_thread.start()

        self._screen_mirroring_active = True
        self._mirror_frame_pending = False
        # Убираем фокус с editable widgets, чтобы мигающий caret не попадал в capture.
        focus_widget = self.focusWidget()
        if focus_widget is not None:
            focus_widget.clearFocus()
        # Убираем solid color, чтобы per-LED не конфликтовал
        self._driver.set_color(0, 0, 0)
        self._screen_mirror_timer.start(self._mirror_interval_ms())
        self._btn_mirror_start.setEnabled(False)
        self._btn_mirror_stop.setEnabled(True)
        self._update_mirror_status("Running", "#2d8c2d")
        self._btn_mirror_stop.setFocus()
        # Первый кадр сразу
        self._tick_screen_mirroring()

    def _stop_screen_mirroring(self, restore_output=True):
        """Останавливает screen mirroring: таймер, thread, engine."""
        self._mirror_restart_timer.stop()
        self._screen_mirror_timer.stop()
        self._screen_mirroring_active = False

        # Останавливаем background thread
        if self._mirror_thread is not None:
            if self._mirror_worker is not None:
                QMetaObject.invokeMethod(self._mirror_worker, "shutdown")
            self._mirror_thread.quit()
            self._mirror_thread.wait(2000)
            self._mirror_thread = None
        self._mirror_worker = None
        self._mirror_frame_pending = False
        self._btn_mirror_start.setEnabled(True)
        self._btn_mirror_stop.setEnabled(False)
        self._update_mirror_status("Idle")
        if restore_output and self._driver.connected:
            if self._current_tab == 0:
                self._driver.set_color(self._r, self._g, self._b)
            else:
                self._driver.set_color(0, 0, 0)

    def _tick_screen_mirroring(self):
        """
        Таймер tick: запрашивает новый кадр у background worker.
        Если предыдущий кадр ещё обрабатывается — пропускаем (frame drop).
        """
        if not self._screen_mirroring_active or self._mirror_worker is None:
            return
        if self._mirror_frame_pending:
            return  # Worker ещё обрабатывает предыдущий кадр
        self._mirror_frame_pending = True
        # invokeMethod с QueuedConnection гарантирует выполнение в worker thread
        QMetaObject.invokeMethod(self._mirror_worker, "process_frame")

    def _on_mirror_frame_ready(self, physical_colors):
        """
        Slot: вызывается из worker thread когда кадр готов.
        Отправляет цвета в LED driver (быстро, не блокирует UI).
        """
        self._mirror_frame_pending = False
        if not self._screen_mirroring_active:
            return
        self._driver.set_per_led_colors(physical_colors)

    def _on_mirror_error(self, error_msg):
        """Slot: worker поймал ошибку при capture/sample."""
        self._mirror_frame_pending = False
        self._stop_screen_mirroring(restore_output=False)
        self._update_mirror_status(f"Error: {error_msg}", "#f38ba8")

    def _on_mirror_monitor_changed(self, index):
        """При смене монитора перестраиваем engine если mirroring активен."""
        if self._screen_mirroring_active:
            try:
                self._queue_mirror_restart()
            except Exception:
                self._stop_screen_mirroring(restore_output=False)

    def _on_mirror_edge_changed(self):
        """Обновляет label и live-применяет edge depth в engine."""
        self._mirror_edge_label.setText(f"{self._mirror_edge_slider.value()}%")
        if self._screen_mirroring_active:
            try:
                self._queue_mirror_restart()
            except Exception:
                pass

    def _on_mirror_smooth_changed(self):
        """Обновляет label и live-применяет smoothing в engine."""
        self._mirror_smooth_label.setText(f"{self._mirror_smooth_slider.value()}%")
        if self._screen_mirroring_active:
            self._queue_mirror_restart()

    def _on_mirror_sat_changed(self):
        """Обновляет label и live-применяет saturation boost в engine."""
        val = self._mirror_sat_slider.value() / 100.0
        self._mirror_sat_label.setText(f"{val:.1f}x")
        if self._screen_mirroring_active:
            self._queue_mirror_restart()

    def _on_mirror_fps_changed(self):
        """Обновляет label и перестраивает интервал таймера."""
        self._mirror_fps_label.setText(str(self._mirror_fps_slider.value()))
        if self._screen_mirroring_active:
            self._screen_mirror_timer.start(self._mirror_interval_ms())

    # endregion

    def _load_preset_to_ui(self):
        """Загружает сохранённый preset в UI (цвет, яркость, sliders)."""
        r, g, b = self._color_preset.as_tuple()
        self._r = r
        self._g = g
        self._b = b
        # Сразу синхронизируем brightness в драйвер,
        # даже если подключения ещё нет.
        # Это важно, чтобы при первом успешном connect лента не вспыхивала на 255.
        self._driver.set_brightness(self._color_preset.brightness)
        self._slider_bright.setValue(self._color_preset.brightness)
        # Обновляем sliders и UI
        self._slider_r.setValue(r)
        self._slider_g.setValue(g)
        self._slider_b.setValue(b)
        self._label_r.setText(str(r))
        self._label_g.setText(str(g))
        self._label_b.setText(str(b))

    def _on_tab_changed(self, index):
        """
        Обработчик переключения вкладок.
        0 = Color: восстанавливаем solid color preset
        1 = LED Config: гасим вывод, ждём Live Preview / Confirm
        2 = Screen Mirror: гасим solid, mirroring стартует кнопкой
        """
        prev_tab = self._current_tab
        self._current_tab = index

        if not self._driver.connected:
            return

        # При уходе с вкладки Screen Mirror — останавливаем mirroring
        if prev_tab == 2 and index != 2:
            self._stop_screen_mirroring(restore_output=False)

        if index == 0:  # Возврат на Color tab
            r, g, b = self._color_preset.as_tuple()
            self._driver.set_color(r, g, b)
            self._driver.set_brightness(self._color_preset.brightness)
        elif index == 1:  # Переход в LED Config
            if self._led_config_panel.live_preview:
                self._send_led_config_preview()
            else:
                self._driver.set_color(0, 0, 0)
        elif index == 2:  # Переход в Screen Mirror
            self._driver.set_color(0, 0, 0)

    def _on_led_config_confirmed(self):
        """
        Вызывается при нажатии Confirm в LED Config.
        Сохраняет конфиг. Если mirroring активен — перестраивает layout.
        """
        cfg = self._led_config_panel.config
        print(f"[UI] LED config saved: {cfg.total} LEDs")
        cfg.save()

        if not self._driver.connected:
            return

        # Если mirroring активен — пересобираем engine с новым конфигом
        if self._screen_mirroring_active:
            try:
                self._queue_mirror_restart()
            except Exception:
                pass
            return

        # На LED Config вкладке показываем живой preview раскладки.
        if self._current_tab == 1:
            self._send_led_config_preview()
            return

        # Иначе гасим ленту (до выхода из LED Config)
        self._driver.set_color(0, 0, 0)

    def _make_slider(self, name, initial, callback):
        """Создаёт горизонтальный слайдер 0-255 с label значения."""
        slider = QSlider(Qt.Orientation.Horizontal)
        slider.setRange(0, 255)
        slider.setValue(initial)
        label = QLabel(str(initial))
        slider.valueChanged.connect(callback)
        return slider, label

    # region Обработчики событий

    def _on_slider_changed(self):
        """Вызывается при изменении любого RGB слайдера."""
        self._r = self._slider_r.value()
        self._g = self._slider_g.value()
        self._b = self._slider_b.value()
        self._label_r.setText(str(self._r))
        self._label_g.setText(str(self._g))
        self._label_b.setText(str(self._b))
        self._update_preview()
        # Debounce: отправляем цвет через 50ms после последнего изменения
        self._send_timer.start()

    def _on_hex_changed(self):
        """Вызывается при нажатии Enter в HEX поле."""
        text = self._hex_input.text().strip()
        if not text.startswith("#"):
            text = "#" + text
        try:
            color = QColor(text)
            if color.isValid():
                self._set_rgb(color.red(), color.green(), color.blue())
        except Exception:
            pass

    def _on_brightness_changed(self, value):
        """Изменилась яркость."""
        self._label_bright.setText(str(value))
        # Сохраняем preset
        self._color_preset.set_brightness(value)
        self._color_preset.save()
        # Всегда синхронизируем яркость в драйвер.
        # Так сохранённое значение уже лежит в памяти драйвера
        # к моменту первого подключения.
        self._driver.set_brightness(value)

    def _on_connect(self):
        """Подключение к контроллеру."""
        self._is_auto_connecting = False
        self._auto_connect_attempt = 0
        self._status_label.setText("Connecting...")
        self._status_label.setStyleSheet("color: #cc9933; font-weight: bold;")
        self._btn_connect.setEnabled(False)

        # Подключаемся (загрузка assembly может занять ~1-2 сек)
        QTimer.singleShot(100, lambda: self._do_connect(is_auto=False))

    def _start_auto_connect(self):
        """
        Запускает серию автопопыток подключения при старте приложения.
        Если контроллер недоступен, пользователь увидит popup только после 5 неудач.
        """
        if self._driver.connected or self._is_auto_connecting:
            return
        self._is_auto_connecting = True
        self._auto_connect_attempt = 0
        self._status_label.setText("Auto connecting... (1/5)")
        self._status_label.setStyleSheet("color: #cc9933; font-weight: bold;")
        self._btn_connect.setEnabled(False)
        QTimer.singleShot(100, lambda: self._do_connect(is_auto=True))

    def _do_connect(self, is_auto=False):
        """Выполняет подключение (вызывается из таймера для обновления UI)."""
        if is_auto:
            self._auto_connect_attempt += 1
        ok = self._driver.connect()
        if ok:
            self._is_auto_connecting = False
            self._status_label.setText("Connected")
            self._status_label.setStyleSheet("color: #2d8c2d; font-weight: bold;")
            self._btn_connect.setEnabled(False)
            self._btn_disconnect.setEnabled(True)
            # Сначала повторно применяем brightness из UI,
            # затем отправляем цвет. Такой порядок нужен,
            # потому что контроллер может сбрасывать dimmer после handshake.
            self._driver.set_brightness(self._slider_bright.value())
            self._send_current_color()
        else:
            if is_auto and self._auto_connect_attempt < self._auto_connect_max_attempts:
                next_attempt = self._auto_connect_attempt + 1
                self._status_label.setText(
                    f"Auto connecting... ({next_attempt}/{self._auto_connect_max_attempts})"
                )
                self._status_label.setStyleSheet("color: #cc9933; font-weight: bold;")
                QTimer.singleShot(self._auto_connect_delay_ms, lambda: self._do_connect(is_auto=True))
                return

            self._is_auto_connecting = False
            self._status_label.setText("Connection failed")
            self._status_label.setStyleSheet("color: #cc3333; font-weight: bold;")
            self._btn_connect.setEnabled(True)
            if is_auto:
                QMessageBox.warning(
                    self,
                    "Auto connect failed",
                    "Soulight не смог подключиться к контроллеру после 5 попыток.\n"
                    "Можно попробовать ещё раз кнопкой Connect."
                )

    def _on_disconnect(self):
        """Отключение от контроллера."""
        self._is_auto_connecting = False
        self._stop_screen_mirroring(restore_output=False)
        self._driver.disconnect()
        self._status_label.setText("Disconnected")
        self._status_label.setStyleSheet("color: #cc3333; font-weight: bold;")
        self._btn_connect.setEnabled(True)
        self._btn_disconnect.setEnabled(False)

    # endregion

    # region Вспомогательные методы

    def _set_rgb(self, r, g, b):
        """Устанавливает RGB значения: обновляет слайдеры, preview, HEX, и отправляет."""
        # Блокируем сигналы слайдеров чтобы не вызвать рекурсию
        self._slider_r.blockSignals(True)
        self._slider_g.blockSignals(True)
        self._slider_b.blockSignals(True)

        self._slider_r.setValue(r)
        self._slider_g.setValue(g)
        self._slider_b.setValue(b)
        self._r = r
        self._g = g
        self._b = b

        self._slider_r.blockSignals(False)
        self._slider_g.blockSignals(False)
        self._slider_b.blockSignals(False)

        self._label_r.setText(str(r))
        self._label_g.setText(str(g))
        self._label_b.setText(str(b))

        self._update_preview()
        self._send_current_color()

    def _update_preview(self):
        """Обновляет preview и HEX поле из текущих RGB."""
        self._preview.set_color(self._r, self._g, self._b)
        self._hex_input.setText(f"#{self._r:02X}{self._g:02X}{self._b:02X}")

    def _send_current_color(self):
        """
        Отправляет текущий цвет на LED ленту (если подключено).
        Отправляется только если на вкладке Color.
        """
        # Сохраняем preset
        self._color_preset.set_color(self._r, self._g, self._b)
        self._color_preset.save()
        # Отправляем только если на Color tab и mirroring не активен
        if self._driver.connected and self._current_tab == 0 and not self._screen_mirroring_active:
            self._driver.set_color(self._r, self._g, self._b)

    # endregion

    def closeEvent(self, event):
        """При закрытии окна останавливаем mirroring и отключаемся."""
        self._stop_screen_mirroring(restore_output=False)
        if self._driver.connected:
            self._driver.disconnect()
        event.accept()
# endregion
