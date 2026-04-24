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
from PyQt6.QtCore import Qt, QTimer, QThread, pyqtSignal
from PyQt6.QtGui import QColor, QPainter, QLinearGradient, QMouseEvent, QFont

from soulight.led_config import SIDE_COLORS, MAX_LEDS
from soulight.protocol.serial_driver import LEDDriver
from soulight.ui.led_config_widget import LEDConfigPanel
from soulight.color_preset import ColorPreset
from soulight.screen_mirroring.layout import build_layout
from soulight.screen_mirroring.worker import MirrorWorker
from soulight.scenes.engine import SceneEngine
from soulight.scenes.patterns import PATTERN_LABELS
from soulight.audio.engine import AudioEngine
from soulight.audio.modes import MODE_LABELS


# region QSS Theme (Catppuccin Mocha)
DARK_STYLE = """
QMainWindow, QWidget {
    background-color: #1e1e2e;
    color: #cdd6f4;
    font-family: "Segoe UI", sans-serif;
    font-size: 13px;
}
QGroupBox {
    background-color: #181825;
    border: 1px solid #313244;
    border-radius: 8px;
    margin-top: 20px;
    padding-top: 10px;
    font-weight: bold;
}
QGroupBox::title {
    subcontrol-origin: margin;
    subcontrol-position: top left;
    left: 10px;
    top: 2px;
    color: #89b4fa;
    background-color: transparent;
}
QPushButton {
    background-color: #313244;
    border: 1px solid #45475a;
    border-radius: 6px;
    padding: 6px 12px;
    color: #cdd6f4;
    font-weight: bold;
}
QPushButton:hover {
    background-color: #45475a;
}
QPushButton:pressed {
    background-color: #585b70;
}
QPushButton:disabled {
    background-color: #181825;
    color: #585b70;
    border: 1px solid #313244;
}
QSlider::groove:horizontal {
    border: 1px solid #313244;
    height: 6px;
    background: #11111b;
    border-radius: 3px;
}
QSlider::sub-page:horizontal {
    background: #89b4fa;
    border-radius: 3px;
}
QSlider::handle:horizontal {
    background: #cdd6f4;
    border: 1px solid #11111b;
    width: 14px;
    margin: -5px 0;
    border-radius: 7px;
}
QSlider::handle:horizontal:hover {
    background: #b4befe;
}
QTabWidget::pane {
    border: 1px solid #313244;
    border-radius: 6px;
    top: -1px;
    background-color: #1e1e2e;
}
QTabBar::tab {
    background: #181825;
    color: #a6adc8;
    padding: 8px 16px;
    border: 1px solid #313244;
    border-bottom: none;
    border-top-left-radius: 6px;
    border-top-right-radius: 6px;
    margin-right: 2px;
}
QTabBar::tab:selected {
    background: #1e1e2e;
    color: #cdd6f4;
    font-weight: bold;
    border-bottom: 2px solid #89b4fa;
}
QTabBar::tab:hover:!selected {
    background: #313244;
}
QComboBox {
    background-color: #11111b;
    border: 1px solid #313244;
    border-radius: 4px;
    padding: 4px 8px;
    color: #cdd6f4;
}
QComboBox::drop-down {
    border: none;
    width: 20px;
}
QComboBox::down-arrow {
    image: none;
    border-left: 5px solid transparent;
    border-right: 5px solid transparent;
    border-top: 5px solid #a6adc8;
    margin-right: 5px;
}
QComboBox QAbstractItemView {
    background-color: #181825;
    border: 1px solid #313244;
    selection-background-color: #313244;
    color: #cdd6f4;
}
QLineEdit {
    background-color: #11111b;
    border: 1px solid #313244;
    border-radius: 4px;
    padding: 4px 8px;
    color: #cdd6f4;
}
"""
# endregion
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


# region Presets для Screen Mirroring

# Эти пресеты меняют только безопасные UI/processing параметры.
# Transport cadence драйвера они не ускоряют, поэтому effective FPS
# всё равно вычисляется отдельно и честно показывается в интерфейсе.
MIRROR_PRESETS = {
    "performance": {
        "label": "Performance",
        "edge": 6,
        "smooth": 10,
        "sat": 100,
        "fps": "practical",
    },
    "balanced": {
        "label": "Balanced",
        "edge": 8,
        "smooth": 35,
        "sat": 130,
        "fps": 15,
    },
    "quality": {
        "label": "Quality",
        "edge": 12,
        "smooth": 55,
        "sat": 150,
        "fps": 20,
    },
}

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
        self.setStyleSheet("border: 2px solid #313244; border-radius: 8px; background-color: transparent;")

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
    # Сигналы для общения с worker thread.
    # Используем их вместо invokeMethod, чтобы Qt гарантированно делал queued call.
    mirror_frame_requested = pyqtSignal()
    mirror_shutdown_requested = pyqtSignal()

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
        self.setStyleSheet(DARK_STYLE)

        # LED driver (protocol bridge + serial)
        self._driver = LEDDriver()
        # Color preset (save/load last color)
        self._color_preset = ColorPreset()
        self._color_preset.load()
        # Track which tab is active
        self._current_tab = 0  # 0=Color, 1=LED Config, 2=Screen Mirror, 3=Scenes, 4=Audio
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
        # Scene engine: паттерн-режимы (Rainbow, Fire, Aurora...)
        self._scene_engine = None
        self._scene_active = False
        self._scene_thread = None
        # Audio engine: FFT аудио-режимы (Spectrum, Electronic, Lyricism)
        self._audio_engine = None
        self._audio_active = False
        self._audio_thread = None
        # Этот флаг нужен, чтобы bulk-обновление слайдеров из пресета
        # не переводило preset selector в Custom посреди применения.
        self._applying_mirror_preset = False
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

        # Вкладка 4: Scenes
        scenes_page = QWidget()
        scenes_layout = QVBoxLayout(scenes_page)
        scenes_layout.setSpacing(12)
        scenes_layout.setContentsMargins(8, 8, 8, 8)
        self._build_scenes_tab(scenes_layout)
        self._tabs.addTab(scenes_page, "Scenes")

        # Вкладка 5: Audio
        audio_page = QWidget()
        audio_layout = QVBoxLayout(audio_page)
        audio_layout.setSpacing(12)
        audio_layout.setContentsMargins(8, 8, 8, 8)
        self._build_audio_tab(audio_layout)
        self._tabs.addTab(audio_page, "Audio")

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

        # === Speed Mode — быстрый выбор частоты обновления ===
        # Управляет интервалом отправки пакетов в serial driver.
        # Влияет на все режимы: solid color, mirroring, scenes, audio.
        speed_group = QGroupBox("Speed Mode")
        speed_layout = QHBoxLayout(speed_group)
        self._speed_buttons = {}
        for label, interval, tooltip in [
            ("Smooth", 0.070, "Плавно, экономно (~14 FPS)"),
            ("Normal", 0.040, "Баланс (~25 FPS)"),
            ("Fast",   0.015, "Максимум (~60 FPS)"),
        ]:
            btn = QPushButton(label)
            btn.setCheckable(True)
            btn.setFixedHeight(32)
            btn.setToolTip(tooltip)
            btn.clicked.connect(lambda checked, iv=interval, lb=label: self._on_speed_mode_clicked(iv, lb))
            speed_layout.addWidget(btn)
            self._speed_buttons[label] = btn
        # Fast по умолчанию — не перезаписываем _send_interval,
        # он уже инициализирован в serial_driver как 0.015
        self._speed_buttons["Fast"].setChecked(True)
        layout.addWidget(speed_group)

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

        # region Preset selector — быстрый выбор quality/perf баланса
        preset_group = QGroupBox("Preset")
        preset_layout = QHBoxLayout(preset_group)
        preset_layout.addWidget(QLabel("Profile:"))
        self._mirror_preset_combo = QComboBox()
        self._mirror_preset_combo.addItem("Custom", "custom")
        for preset_key, preset in MIRROR_PRESETS.items():
            self._mirror_preset_combo.addItem(preset["label"], preset_key)
        self._mirror_preset_combo.setCurrentIndex(2)
        self._mirror_preset_combo.currentIndexChanged.connect(self._on_mirror_preset_changed)
        preset_layout.addWidget(self._mirror_preset_combo, stretch=1)
        layout.addWidget(preset_group)
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

        # Brightness — аппаратная яркость LED при mirroring (0-255)
        self._mirror_brightness_slider, self._mirror_brightness_label = self._make_slider(
            "Brightness", 255, self._on_mirror_brightness_changed
        )
        # Теперь это напрямую управляет Hardware Dimmer контроллера
        self._mirror_brightness_slider.setRange(0, 255)
        self._mirror_brightness_label.setText("255")
        brightness_row = QHBoxLayout()
        brightness_row.addWidget(QLabel("Brightness"))
        brightness_row.addWidget(self._mirror_brightness_slider)
        self._mirror_brightness_label.setFixedWidth(48)
        self._mirror_brightness_label.setAlignment(Qt.AlignmentFlag.AlignRight)
        brightness_row.addWidget(self._mirror_brightness_label)
        tuning_layout.addLayout(brightness_row)

        # FPS — частота кадров mirroring
        self._mirror_fps_slider, self._mirror_fps_label = self._make_slider(
            "FPS", 60, self._on_mirror_fps_changed
        )
        self._mirror_fps_slider.setRange(5, 60)
        self._mirror_fps_label.setText("60")
        fps_row = QHBoxLayout()
        fps_row.addWidget(QLabel("FPS"))
        fps_row.addWidget(self._mirror_fps_slider)
        self._mirror_fps_label.setFixedWidth(48)
        self._mirror_fps_label.setAlignment(Qt.AlignmentFlag.AlignRight)
        fps_row.addWidget(self._mirror_fps_label)
        tuning_layout.addLayout(fps_row)

        # Этот label показывает честный effective FPS после safe cap.
        # Так пользователь видит, когда requested FPS выше practical throughput.
        self._mirror_fps_hint_label = QLabel("")
        self._mirror_fps_hint_label.setStyleSheet("color: #9399b2;")
        tuning_layout.addWidget(self._mirror_fps_hint_label)

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
        self._refresh_mirror_tuning_labels()

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
        """Интервал таймера mirroring по effective FPS после safe cap."""
        fps = self._mirror_effective_fps()
        return max(1, int(1000 / fps))

    def _mirror_requested_fps(self):
        """Requested FPS из UI до применения safe cap."""
        return max(1, int(self._mirror_fps_slider.value()))

    def _mirror_practical_max_fps(self):
        """Practical throughput текущего transport path без ускорения драйвера."""
        return max(1, int(self._driver.practical_mirroring_max_fps))

    def _mirror_effective_fps(self):
        """Effective FPS, который реально имеет смысл просить у worker."""
        return min(self._mirror_requested_fps(), self._mirror_practical_max_fps())

    def _mirror_brightness_gain(self):
        """Software brightness gain для mirroring-картинки."""
        return max(0.0, float(self._mirror_brightness_slider.value()) / 100.0)

    def _mirror_preset_target_fps(self, preset):
        """Переводит preset policy в конкретное requested FPS значение."""
        target = preset["fps"]
        if target == "practical":
            return self._mirror_practical_max_fps()
        return max(1, int(target))

    def _set_mirror_preset_combo_value(self, preset_key):
        """Аккуратно меняет preset selector без лишних сигналов."""
        for index in range(self._mirror_preset_combo.count()):
            if self._mirror_preset_combo.itemData(index) == preset_key:
                self._mirror_preset_combo.blockSignals(True)
                self._mirror_preset_combo.setCurrentIndex(index)
                self._mirror_preset_combo.blockSignals(False)
                return

    def _refresh_mirror_tuning_labels(self):
        """
        Синхронизирует текстовые label после любого изменения tuning.

        Здесь мы отдельно показываем requested и effective FPS,
        чтобы cap не выглядел как скрытая магия.
        """
        self._mirror_edge_label.setText(f"{self._mirror_edge_slider.value()}%")
        self._mirror_smooth_label.setText(f"{self._mirror_smooth_slider.value()}%")
        sat_value = self._mirror_sat_slider.value() / 100.0
        self._mirror_sat_label.setText(f"{sat_value:.1f}x")
        self._mirror_brightness_label.setText(f"{self._mirror_brightness_slider.value()}%")

        requested = self._mirror_requested_fps()
        effective = self._mirror_effective_fps()
        practical = self._mirror_practical_max_fps()
        self._mirror_fps_label.setText(str(requested))
        if requested > effective:
            self._mirror_fps_hint_label.setText(
                f"Effective: {effective} FPS (safe cap, transport ~{practical} FPS)"
            )
            self._mirror_fps_hint_label.setStyleSheet("color: #f9e2af;")
        else:
            self._mirror_fps_hint_label.setText(
                f"Effective: {effective} FPS (transport ~{practical} FPS)"
            )
            self._mirror_fps_hint_label.setStyleSheet("color: #9399b2;")

    def _apply_mirror_preset(self, preset_key):
        """
        Применяет preset как одну атомарную операцию.

        Это важно, чтобы не дёргать restart несколько раз подряд,
        пока UI только выставляет значения в слайдеры.
        """
        preset = MIRROR_PRESETS.get(preset_key)
        if preset is None:
            return

        self._applying_mirror_preset = True
        sliders = (
            self._mirror_edge_slider,
            self._mirror_smooth_slider,
            self._mirror_sat_slider,
            self._mirror_fps_slider,
        )
        for slider in sliders:
            slider.blockSignals(True)

        self._mirror_edge_slider.setValue(int(preset["edge"]))
        self._mirror_smooth_slider.setValue(int(preset["smooth"]))
        self._mirror_sat_slider.setValue(int(preset["sat"]))
        self._mirror_fps_slider.setValue(self._mirror_preset_target_fps(preset))

        for slider in sliders:
            slider.blockSignals(False)
        self._applying_mirror_preset = False

        self._set_mirror_preset_combo_value(preset_key)
        self._refresh_mirror_tuning_labels()

        if self._screen_mirroring_active:
            self._queue_mirror_restart()
            self._screen_mirror_timer.start(self._mirror_interval_ms())

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
        self.mirror_frame_requested.connect(self._mirror_worker.process_frame)
        self.mirror_shutdown_requested.connect(self._mirror_worker.shutdown)
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
        # Во время mirroring master brightness берём из основного global slider.
        # Отдельный mirror brightness теперь управляет именно gain sampled цветов.
        self._driver.set_brightness(self._slider_bright.value())
        self._screen_mirror_timer.start(self._mirror_interval_ms())
        self._btn_mirror_start.setEnabled(False)
        self._btn_mirror_stop.setEnabled(True)
        self._update_mirror_status(
            f"Running · {self._mirror_effective_fps()} FPS effective",
            "#2d8c2d",
        )
        self._btn_mirror_stop.setFocus()
        # Первый кадр просим не мгновенно, а на следующем тике event loop.
        # Это уменьшает шанс стартовой гонки между UI thread и worker thread.
        # Увеличили до 100ms — на медленных системах thread старт может занимать 50-80ms.
        QTimer.singleShot(100, self._tick_screen_mirroring)

    def _stop_screen_mirroring(self, restore_output=True):
        """Останавливает screen mirroring: таймер, thread, engine."""
        self._mirror_restart_timer.stop()
        self._screen_mirror_timer.stop()
        self._screen_mirroring_active = False

        # Останавливаем background thread
        if self._mirror_thread is not None:
            if self._mirror_worker is not None:
                self.mirror_shutdown_requested.emit()
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
        # queued signal-slot гарантирует выполнение process_frame в worker thread
        self.mirror_frame_requested.emit()

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
        self._refresh_mirror_tuning_labels()
        if not self._applying_mirror_preset:
            self._set_mirror_preset_combo_value("custom")
        if self._screen_mirroring_active:
            try:
                self._queue_mirror_restart()
            except Exception:
                pass

    def _on_mirror_smooth_changed(self):
        """Обновляет label и live-применяет smoothing в engine."""
        self._refresh_mirror_tuning_labels()
        if not self._applying_mirror_preset:
            self._set_mirror_preset_combo_value("custom")
        if self._screen_mirroring_active:
            self._queue_mirror_restart()

    def _on_mirror_sat_changed(self):
        """Обновляет label и live-применяет saturation boost в engine."""
        self._refresh_mirror_tuning_labels()
        if not self._applying_mirror_preset:
            self._set_mirror_preset_combo_value("custom")
        if self._screen_mirroring_active:
            self._queue_mirror_restart()

    def _on_mirror_brightness_changed(self):
        """Синхронизирует ползунок Mirroring с основным Hardware Brightness."""
        val = self._mirror_brightness_slider.value()
        self._mirror_brightness_label.setText(str(val))
        if not self._applying_mirror_preset:
            self._set_mirror_preset_combo_value("custom")
        
        # Блокируем сигналы мастер-слайдера, чтобы избежать рекурсии, если понадобится,
        # но в данном случае достаточно просто вызвать setValue
        self._slider_bright.setValue(val)

    def _on_mirror_fps_changed(self):
        """Обновляет label и перестраивает интервал таймера."""
        self._refresh_mirror_tuning_labels()
        if not self._applying_mirror_preset:
            self._set_mirror_preset_combo_value("custom")
        if self._screen_mirroring_active:
            self._screen_mirror_timer.start(self._mirror_interval_ms())
            self._update_mirror_status(
                f"Running · {self._mirror_effective_fps()} FPS effective",
                "#2d8c2d",
            )

    def _on_mirror_preset_changed(self, index):
        """Применяет выбранный preset или оставляет ручной режим Custom."""
        preset_key = self._mirror_preset_combo.currentData()
        if preset_key in MIRROR_PRESETS:
            self._apply_mirror_preset(preset_key)

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
        3 = Scenes: гасим solid, scenes стартуют кнопкой
        4 = Audio: гасим solid, audio стартует кнопкой
        """
        prev_tab = self._current_tab
        self._current_tab = index

        # При уходе с активных вкладок — останавливаем их потоки
        if prev_tab == 2 and index != 2:
            self._stop_screen_mirroring(restore_output=False)
        if prev_tab == 3 and index != 3:
            self._stop_scenes()
        if prev_tab == 4 and index != 4:
            self._stop_audio()

        if not self._driver.connected:
            return

        if index == 0:  # Возврат на Color tab
            r, g, b = self._color_preset.as_tuple()
            self._driver.set_color(r, g, b)
            self._driver.set_brightness(self._color_preset.brightness)
        elif index == 1:  # Переход в LED Config
            if self._led_config_panel.live_preview:
                self._send_led_config_preview()
            else:
                self._driver.set_color(0, 0, 0)
        elif index in (2, 3, 4):  # Screen Mirror / Scenes / Audio
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
        """Изменилась мастер-яркость."""
        self._label_bright.setText(str(value))
        # Сохраняем preset
        self._color_preset.set_brightness(value)
        self._color_preset.save()
        # Всегда синхронизируем яркость в драйвер.
        self._driver.set_brightness(value)
        
        # Синхронизируем Mirroring слайдер, чтобы они не разъезжались
        if self._mirror_brightness_slider.value() != value:
            self._mirror_brightness_slider.blockSignals(True)
            self._mirror_brightness_slider.setValue(value)
            self._mirror_brightness_label.setText(str(value))
            self._mirror_brightness_slider.blockSignals(False)

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
        self._stop_scenes()
        self._stop_audio()
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
        Отправляется только если на вкладке Color и нет активных режимов.
        """
        # Сохраняем preset
        self._color_preset.set_color(self._r, self._g, self._b)
        self._color_preset.save()
        # Отправляем только если на Color tab и ничто другое не активно
        if (self._driver.connected
                and self._current_tab == 0
                and not self._screen_mirroring_active
                and not self._scene_active
                and not self._audio_active):
            self._driver.set_color(self._r, self._g, self._b)

    # endregion

    # region Scenes tab

    def _build_scenes_tab(self, layout):
        """Строит вкладку Scenes: выбор паттерна, speed, start/stop."""
        self._scene_status_label = QLabel("Idle")
        self._scene_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        layout.addWidget(self._scene_status_label)

        pattern_group = QGroupBox("Pattern")
        pattern_layout = QVBoxLayout(pattern_group)
        self._scene_pattern_combo = QComboBox()
        for key, label in PATTERN_LABELS.items():
            self._scene_pattern_combo.addItem(label, key)
        pattern_layout.addWidget(self._scene_pattern_combo)
        layout.addWidget(pattern_group)

        speed_group = QGroupBox("Speed")
        speed_layout = QHBoxLayout(speed_group)
        self._scene_speed_slider = QSlider(Qt.Orientation.Horizontal)
        self._scene_speed_slider.setRange(25, 400)
        self._scene_speed_slider.setValue(100)
        self._scene_speed_slider.setToolTip("Animation speed (25% - 400%)")
        self._scene_speed_label = QLabel("100%")
        self._scene_speed_slider.valueChanged.connect(self._on_scene_speed_changed)
        speed_layout.addWidget(self._scene_speed_slider)
        self._scene_speed_label.setFixedWidth(40)
        speed_layout.addWidget(self._scene_speed_label)
        layout.addWidget(speed_group)

        btn_row = QHBoxLayout()
        self._btn_scene_start = QPushButton("Start")
        self._btn_scene_start.setFixedHeight(36)
        self._btn_scene_start.setStyleSheet(
            "background-color: #2d8c2d; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_scene_start.clicked.connect(self._on_scene_start_clicked)
        btn_row.addWidget(self._btn_scene_start)
        self._btn_scene_stop = QPushButton("Stop")
        self._btn_scene_stop.setFixedHeight(36)
        self._btn_scene_stop.setStyleSheet(
            "background-color: #cc3333; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_scene_stop.setEnabled(False)
        self._btn_scene_stop.clicked.connect(self._stop_scenes)
        btn_row.addWidget(self._btn_scene_stop)
        layout.addLayout(btn_row)
        layout.addStretch()

    def _on_scene_speed_changed(self):
        v = self._scene_speed_slider.value()
        self._scene_speed_label.setText(f"{v}%")
        if self._scene_engine is not None:
            self._scene_engine.set_speed(v / 100.0)

    def _on_scene_start_clicked(self):
        if not self._driver.connected:
            QMessageBox.warning(self, "Not connected", "Connect to the controller first.")
            return
        self._start_scenes(self._scene_pattern_combo.currentData())

    def _start_scenes(self, pattern_name: str):
        self._stop_scenes()
        self._scene_thread = QThread()
        # led_count берём из актуального LED конфига, а не хардкодим
        actual_led_count = self._led_config_panel.config.total
        self._scene_engine = SceneEngine(led_count=actual_led_count, fps=20)
        self._scene_engine.moveToThread(self._scene_thread)
        self._scene_engine.frame_ready.connect(self._on_scene_frame_ready)
        self._scene_engine.error_occurred.connect(self._on_scene_error)
        self._scene_thread.started.connect(lambda: self._scene_engine.start(pattern_name))
        self._scene_thread.start()
        self._scene_active = True
        self._scene_engine.set_speed(self._scene_speed_slider.value() / 100.0)
        self._scene_status_label.setText(f"Running: {PATTERN_LABELS.get(pattern_name, pattern_name)}")
        self._scene_status_label.setStyleSheet("color: #2d8c2d; font-weight: bold;")
        self._btn_scene_start.setEnabled(False)
        self._btn_scene_stop.setEnabled(True)
        self._driver.set_color(0, 0, 0)

    def _stop_scenes(self):
        if self._scene_engine is not None:
            self._scene_engine.stop()
            self._scene_engine = None
        if self._scene_thread is not None:
            self._scene_thread.quit()
            self._scene_thread.wait(2000)
            self._scene_thread = None
        self._scene_active = False
        self._scene_status_label.setText("Idle")
        self._scene_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        self._btn_scene_start.setEnabled(True)
        self._btn_scene_stop.setEnabled(False)

    def _on_scene_frame_ready(self, colors):
        if self._scene_active and self._driver.connected:
            self._driver.set_per_led_colors(colors)

    def _on_scene_error(self, msg):
        self._scene_status_label.setText(f"Error: {msg}")
        self._scene_status_label.setStyleSheet("color: #f38ba8; font-weight: bold;")
        self._stop_scenes()

    # endregion

    # region Audio tab

    def _build_audio_tab(self, layout):
        """Строит вкладку Audio: выбор режима, sensitivity, start/stop."""
        self._audio_status_label = QLabel("Idle")
        self._audio_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        layout.addWidget(self._audio_status_label)

        mode_group = QGroupBox("Mode")
        mode_layout = QVBoxLayout(mode_group)
        self._audio_mode_combo = QComboBox()
        for key, label in MODE_LABELS.items():
            self._audio_mode_combo.addItem(label, key)
        mode_layout.addWidget(self._audio_mode_combo)
        layout.addWidget(mode_group)

        sens_group = QGroupBox("Sensitivity")
        sens_layout = QHBoxLayout(sens_group)
        self._audio_sens_slider = QSlider(Qt.Orientation.Horizontal)
        self._audio_sens_slider.setRange(10, 500)
        self._audio_sens_slider.setValue(150)
        self._audio_sens_label = QLabel("150%")
        self._audio_sens_slider.valueChanged.connect(self._on_audio_sens_changed)
        sens_layout.addWidget(self._audio_sens_slider)
        self._audio_sens_label.setFixedWidth(40)
        sens_layout.addWidget(self._audio_sens_label)
        layout.addWidget(sens_group)

        btn_row = QHBoxLayout()
        self._btn_audio_start = QPushButton("Start")
        self._btn_audio_start.setFixedHeight(36)
        self._btn_audio_start.setStyleSheet(
            "background-color: #2d8c2d; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_audio_start.clicked.connect(self._on_audio_start_clicked)
        btn_row.addWidget(self._btn_audio_start)
        self._btn_audio_stop = QPushButton("Stop")
        self._btn_audio_stop.setFixedHeight(36)
        self._btn_audio_stop.setStyleSheet(
            "background-color: #cc3333; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_audio_stop.setEnabled(False)
        self._btn_audio_stop.clicked.connect(self._stop_audio)
        btn_row.addWidget(self._btn_audio_stop)
        layout.addLayout(btn_row)
        layout.addStretch()

    def _on_audio_sens_changed(self):
        v = self._audio_sens_slider.value()
        self._audio_sens_label.setText(f"{v}%")
        if self._audio_engine is not None:
            self._audio_engine.set_sensitivity(v / 100.0)

    def _on_audio_start_clicked(self):
        if not self._driver.connected:
            QMessageBox.warning(self, "Not connected", "Connect to the controller first.")
            return
        self._start_audio(self._audio_mode_combo.currentData())

    def _start_audio(self, mode_name: str):
        self._stop_audio()
        self._audio_thread = QThread()
        # led_count берём из актуального LED конфига, а не хардкодим
        actual_led_count = self._led_config_panel.config.total
        self._audio_engine = AudioEngine(led_count=actual_led_count, fps=20)
        self._audio_engine.moveToThread(self._audio_thread)
        self._audio_engine.frame_ready.connect(self._on_audio_frame_ready)
        self._audio_engine.error_occurred.connect(self._on_audio_error)
        self._audio_engine.status_changed.connect(self._on_audio_status_changed)
        self._audio_thread.started.connect(lambda: self._audio_engine.start(mode_name))
        self._audio_thread.start()
        self._audio_active = True
        self._audio_engine.set_sensitivity(self._audio_sens_slider.value() / 100.0)
        self._audio_status_label.setText(f"Starting: {MODE_LABELS.get(mode_name, mode_name)}...")
        self._audio_status_label.setStyleSheet("color: #cc9933; font-weight: bold;")
        self._btn_audio_start.setEnabled(False)
        self._btn_audio_stop.setEnabled(True)
        self._driver.set_color(0, 0, 0)

    def _stop_audio(self):
        if self._audio_engine is not None:
            self._audio_engine.stop()
            self._audio_engine = None
        if self._audio_thread is not None:
            self._audio_thread.quit()
            self._audio_thread.wait(2000)
            self._audio_thread = None
        self._audio_active = False
        self._audio_status_label.setText("Idle")
        self._audio_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        self._btn_audio_start.setEnabled(True)
        self._btn_audio_stop.setEnabled(False)

    def _on_audio_frame_ready(self, colors):
        if self._audio_active and self._driver.connected:
            self._driver.set_per_led_colors(colors)

    def _on_audio_error(self, msg):
        self._audio_status_label.setText(f"Error: {msg}")
        self._audio_status_label.setStyleSheet("color: #f38ba8; font-weight: bold;")
        self._stop_audio()

    def _on_audio_status_changed(self, status):
        self._audio_status_label.setText(status)

    # endregion

    # region Speed mode

    def _on_speed_mode_clicked(self, interval, label):
        for lb, btn in self._speed_buttons.items():
            btn.setChecked(lb == label)
        self._driver._send_interval = interval

    # endregion

    def closeEvent(self, event):
        """При закрытии окна останавливаем mirroring, scenes, audio и отключаемся."""
        self._stop_screen_mirroring(restore_output=False)
        self._stop_scenes()
        self._stop_audio()
        if self._driver.connected:
            self._driver.disconnect()
        event.accept()
# endregion
