# main_window.py — Главное окно Soulight.
#
# Содержит Color Picker (палитра, RGB слайдеры, HEX ввод),
# Brightness slider, пресеты цветов, кнопку ON/OFF,
# и статус подключения.

from PyQt6.QtWidgets import (
    QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QLabel, QSlider, QPushButton, QLineEdit, QGridLayout,
    QGroupBox, QFrame, QMessageBox, QTabWidget,
    QComboBox, QCheckBox, QSpinBox, QDoubleSpinBox, QTimeEdit,
    QColorDialog, QProgressBar,
)
from PyQt6.QtCore import Qt, QTimer, QThread, pyqtSignal, QTime
from PyQt6.QtGui import QColor, QPainter, QFont

from soulight.led_config import SIDE_COLORS, MAX_LEDS
from soulight.protocol.serial_driver import LEDDriver
from soulight.ui.led_config_widget import LEDConfigPanel
from soulight.color_preset import ColorPreset
from soulight.app_settings import AppSettings
from soulight.auto_brightness import AutoBrightnessService, CV2_AVAILABLE
from soulight.screen_mirroring.layout import build_layout
from soulight.screen_mirroring.screen_capture import BETTERCAM_AVAILABLE
from soulight.screen_mirroring.worker import MirrorWorker
from soulight.scenes.engine import SceneEngine
from soulight.scenes.patterns import PATTERN_LABELS, PATTERNS
from soulight.audio.engine import AudioEngine, list_capture_devices
from soulight.audio.modes import MODE_LABELS, AUDIO_MODES


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
    # Статус сервиса автояркости → UI (сервис зовёт из своего потока)
    _auto_status_signal = pyqtSignal(str)

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
        self._audio_error_msg = None
        # Настройки приложения + сервис автояркости (вкладка Auto)
        self._settings = AppSettings()
        self._auto_service = AutoBrightnessService(
            self._driver, self._settings.auto_params())
        self._mode_restored = False  # восстановление режима — один раз при старте
        # Активный режим ленты: None | "color" | "mirror" | "scene" | "audio".
        # Режим меняется только кнопкой (Re)Start — переключение вкладок
        # ничего не запускает и не останавливает.
        self._active_mode = None
        # Программный setValue слайдера яркости (загрузка пресета) не
        # должен ставить автояркость на паузу как ручное изменение.
        self._suppress_manual_pause = False
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
        self._led_config_panel.preview_requested.connect(self._on_led_config_preview)
        self._led_config_panel.live_preview_changed.connect(self._on_live_preview_toggled)
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

        # Вкладка 6: Auto — автояркость (ambient + время) и автоматизация.
        # Последней, чтобы не сдвигать индексы существующих вкладок.
        auto_page = QWidget()
        auto_layout = QVBoxLayout(auto_page)
        auto_layout.setSpacing(12)
        auto_layout.setContentsMargins(8, 8, 8, 8)
        self._build_auto_tab(auto_layout)
        self._tabs.addTab(auto_page, "Auto")

        # Tab change handler: только выбор того, ЧТО запустит (Re)Start.
        # Никаких действий с лентой при переключении вкладок.
        self._tabs.currentChanged.connect(self._on_tab_changed)

        # === Общая панель управления режимом (вне вкладок) ===
        ctrl_layout = QHBoxLayout()
        self._btn_mode_start = QPushButton("(Re)Start")
        self._btn_mode_start.setFixedHeight(36)
        self._btn_mode_start.setStyleSheet(
            "background-color: #2d8c2d; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_mode_start.setToolTip(
            "Запустить режим текущей вкладки (Color / Mirror / Scenes / Audio)"
        )
        self._btn_mode_start.clicked.connect(self._on_mode_start)
        ctrl_layout.addWidget(self._btn_mode_start)

        self._btn_mode_stop = QPushButton("Stop")
        self._btn_mode_stop.setFixedHeight(36)
        self._btn_mode_stop.setStyleSheet(
            "background-color: #cc3333; color: white; font-weight: bold; border-radius: 6px;"
        )
        self._btn_mode_stop.setEnabled(False)
        self._btn_mode_stop.setToolTip("Остановить режим — лента гаснет")
        self._btn_mode_stop.clicked.connect(self._on_mode_stop)
        ctrl_layout.addWidget(self._btn_mode_stop)

        # Быстрая пауза веб-камеры автояркости — без рестарта сервиса.
        self._btn_cam_pause = QPushButton("Cam ⏸")
        self._btn_cam_pause.setCheckable(True)
        self._btn_cam_pause.setToolTip(
            "Пауза веб-камеры автояркости (камера освобождается, "
            "кривая день/ночь продолжает работать)"
        )
        self._btn_cam_pause.toggled.connect(self._on_cam_pause_toggled)
        ctrl_layout.addWidget(self._btn_cam_pause)
        layout.addLayout(ctrl_layout)

        # Статусная строка: подключение + активный режим
        self._mode_status_label = QLabel("Mode: off")
        self._mode_status_label.setStyleSheet("color: #9399b2;")
        layout.addWidget(self._mode_status_label)
        self._update_mode_status()

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
        self._btn_pick_color = QPushButton("Pick a color")
        self._btn_pick_color.setToolTip(
            "Открыть системный выбор цвета — новый цвет сохранится"
        )
        self._btn_pick_color.clicked.connect(self._on_pick_color)
        hex_layout.addWidget(self._btn_pick_color)
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
        self._btn_on.setToolTip(
            "Hardware-выключатель ленты: OFF — реальное выключение "
            "на контроллере, движок продолжает писать кадры. "
            "ON возвращает вывод."
        )
        self._btn_on.clicked.connect(lambda: self._driver.switch(True))
        onoff_layout.addWidget(self._btn_on)

        self._btn_off = QPushButton("LED OFF")
        self._btn_off.setFixedHeight(36)
        self._btn_off.setStyleSheet("background-color: #cc3333; color: white; font-weight: bold; border-radius: 6px;")
        self._btn_off.setToolTip(
            "Выключает вывод ленты на уровне контроллера "
            "(движок продолжает работать — см. Stop внизу)."
        )
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
        # На Wayland захват идёт через KWin CaptureActiveScreen —
        # индекс монитора игнорируется, захватывается активный экран.
        from soulight.screen_mirroring.screen_capture import _is_wayland
        if _is_wayland():
            self._mirror_monitor_combo.setEnabled(False)
            self._mirror_monitor_combo.setToolTip(
                "На Wayland захватывается активный экран (KWin)"
            )
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
        # Дефолт — Custom: реальные значения слайдеров,
        # чтобы combo не обещал пресет, который не применён.
        self._mirror_preset_combo.setCurrentIndex(0)
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

        # Brightness — аппаратная яркость LED (hardware dimmer контроллера).
        # Это alias мастер-слайдера с вкладки Color: они двунаправленно синхронны.
        self._mirror_brightness_slider, self._mirror_brightness_label = self._make_slider(
            "Brightness", 255, self._on_mirror_brightness_changed
        )
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

        # Backend selector — BetterCam (быстрый DirectX) или MSS (совместимый GDI).
        # Доступен только если bettercam установлен; иначе disabled и подсказка.
        self._mirror_bettercam_checkbox = QCheckBox("Use BetterCam backend")
        self._mirror_bettercam_checkbox.setChecked(True)
        self._mirror_bettercam_checkbox.setEnabled(BETTERCAM_AVAILABLE)
        if not BETTERCAM_AVAILABLE:
            self._mirror_bettercam_checkbox.setToolTip(
                "bettercam not installed. Run: pip install bettercam"
            )
        self._mirror_bettercam_checkbox.stateChanged.connect(self._on_mirror_bettercam_changed)
        tuning_layout.addWidget(self._mirror_bettercam_checkbox)

        layout.addWidget(tuning_group)
        # endregion

        hint = QLabel("Запуск/остановка — общей кнопкой (Re)Start внизу окна.")
        hint.setStyleSheet("color: #6c7086;")
        layout.addWidget(hint)
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

    def _mirror_prefer_bettercam(self):
        """Пользователь выбрал BetterCam backend?"""
        if not BETTERCAM_AVAILABLE:
            return False
        return self._mirror_bettercam_checkbox.isChecked()

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
        """Собирает snapshot текущих mirroring-настроек для worker thread.
        Конфиг копируем — виджет может мутировать его во время работы worker."""
        import copy
        return {
            "config": copy.deepcopy(self._led_config_panel.config),
            "monitor_index": self._mirror_monitor_index(),
            "edge_fraction": self._mirror_edge_fraction(),
            "smoothing_factor": self._mirror_smoothing_factor(),
            "saturation_boost": self._mirror_saturation_boost(),
            "prefer_dxcam": self._mirror_prefer_bettercam(),
        }

    def _restart_screen_mirroring(self):
        """
        Безопасно перезапускает mirroring.
        Это нужно для настроек, которые требуют новый engine/monitor/layout.
        """
        if not self._screen_mirroring_active:
            return
        self._stop_screen_mirroring(restore_output=False)
        # Лента держит последний кадр/resting color до первого нового —
        # per-led перезаписывает буфер целиком, чёрная прослойка не нужна.
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
        Preview не должен перебивать работающий режим.
        """
        if not self._driver.connected or self._active_mode is not None:
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
        Лента держит текущий кадр до первого нового — без чёрной прослойки.
        """
        if not self._driver.connected:
            QMessageBox.warning(self, "Not connected",
                                "Connect to the controller first.")
            return
        cfg = self._led_config_panel.config
        if cfg.total + cfg.start_offset > MAX_LEDS:
            QMessageBox.warning(
                self, "Invalid LED config",
                f"Total LEDs ({cfg.total}) + offset ({cfg.start_offset}) "
                f"exceeds hardware limit ({MAX_LEDS}).\n"
                "Исправьте раскладку на вкладке LED Config."
            )
            return
        self._stop_all_modes()
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
        # Hardware dimmer — из основного слайдера (mirror-слайдер ему синхронен).
        self._driver.set_brightness(self._slider_bright.value())
        self._screen_mirror_timer.start(self._mirror_interval_ms())
        self._active_mode = "mirror"
        self._btn_mode_stop.setEnabled(True)
        self._save_mode("mirror")
        self._update_mirror_status(
            f"Running · {self._mirror_effective_fps()} FPS effective",
            "#2d8c2d",
        )
        # Первый кадр просим не мгновенно, а на следующем тике event loop.
        # Это уменьшает шанс стартовой гонки между UI thread и worker thread.
        # Увеличили до 100ms — на медленных системах thread старт может занимать 50-80ms.
        QTimer.singleShot(100, self._tick_screen_mirroring)

    def _stop_screen_mirroring(self, restore_output=True):
        """Останавливает screen mirroring: таймер, thread, engine."""
        self._mirror_restart_timer.stop()
        self._screen_mirror_timer.stop()
        self._screen_mirroring_active = False

        worker = self._mirror_worker
        thread = self._mirror_thread
        self._mirror_worker = None
        self._mirror_thread = None
        self._mirror_frame_pending = False

        if worker is not None:
            # Отвязываем worker от UI-слотов до остановки потока —
            # его поздние кадры/ошибки не должны добивать новый engine.
            try:
                worker.frame_ready.disconnect(self._on_mirror_frame_ready)
                worker.error_occurred.disconnect(self._on_mirror_error)
            except (TypeError, RuntimeError):
                pass
            self.mirror_shutdown_requested.emit()

        if thread is not None:
            thread.quit()
            if thread.wait(2000):
                # Поток завершился — queued shutdown мог не доставиться
                # (event loop уже вышел). Освобождаем ресурсы напрямую,
                # это безопасно: поток мёртв, гонки с capture нет.
                if worker is not None:
                    try:
                        worker.shutdown()
                    except RuntimeError:
                        pass
            else:
                # Поток не уложился в 2с — не теряем его: worker уже
                # отвязан от UI-сигналов, поток удалится по finished.
                self._debug_log_mirror("mirror thread did not stop in 2s")

        self._update_mirror_status("Idle")
        if restore_output:
            self._mode_stopped("mirror")

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

    def _debug_log_mirror(self, msg):
        print(f"[mirror] {msg}")

    def _on_mirror_frame_ready(self, physical_colors):
        """
        Slot: вызывается из worker thread когда кадр готов.
        Отправляет цвета в LED driver (быстро, не блокирует UI).
        """
        # Игнорируем кадры от старого worker'а — после restart они
        # относятся к предыдущей раскладке и могут прийти с задержкой.
        if self.sender() is not self._mirror_worker:
            return
        self._mirror_frame_pending = False
        if not self._screen_mirroring_active:
            return
        self._driver.set_per_led_colors(physical_colors)

    def _on_mirror_error(self, error_msg):
        """Slot: worker поймал ошибку при capture/sample."""
        # Ошибка от уже остановленного worker'а не должна убивать новый.
        if self.sender() is not self._mirror_worker:
            return
        self._mirror_frame_pending = False
        self._stop_screen_mirroring(restore_output=True)
        self._update_mirror_status(f"Error: {error_msg}", "#f38ba8")

    def _on_mirror_monitor_changed(self, index):
        """При смене монитора перестраиваем engine если mirroring активен."""
        if self._screen_mirroring_active:
            try:
                self._queue_mirror_restart()
            except Exception:
                self._stop_screen_mirroring(restore_output=False)

    def _on_mirror_bettercam_changed(self, state):
        """При смене backend перезапускаем mirroring, чтобы применить выбор."""
        if not self._screen_mirroring_active:
            return
        self._queue_mirror_restart()

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
        # setValue мастер-слайдера вызовет _on_brightness_changed → драйвер.
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
            self._update_mode_status()

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
        self._suppress_manual_pause = True
        try:
            self._slider_bright.setValue(self._color_preset.brightness)
        finally:
            self._suppress_manual_pause = False
        # Обновляем sliders и UI
        self._slider_r.setValue(r)
        self._slider_g.setValue(g)
        self._slider_b.setValue(b)
        self._label_r.setText(str(r))
        self._label_g.setText(str(g))
        self._label_b.setText(str(b))

    # Вкладки, с которых (Re)Start запускает режим ленты.
    # LED Config (1) и Auto (5) — настройки, не режимы.
    _MODE_TABS = (0, 2, 3, 4)

    def _on_tab_changed(self, index):
        """
        Переключение вкладки — только выбор того, что запустит (Re)Start.
        Ленту не трогаем: активный режим продолжает работать при смене
        вкладки, новый запускается только кнопкой.
        """
        self._current_tab = index
        self._update_mode_status()

    def _mode_name(self) -> str:
        """Человекочитаемое имя активного режима для статусной строки."""
        if self._active_mode == "color":
            return f"Color #{self._r:02X}{self._g:02X}{self._b:02X}"
        if self._active_mode == "mirror":
            return f"Screen Mirror · {self._mirror_effective_fps()} FPS"
        if self._active_mode == "scene":
            p = self._scene_pattern_combo.currentData()
            return f"Scene: {PATTERN_LABELS.get(p, p)}"
        if self._active_mode == "audio":
            m = self._audio_mode_combo.currentData()
            return f"Audio: {MODE_LABELS.get(m, m)}"
        # idle — движок не запущен, лента показывает resting color.
        return "idle"

    def _update_mode_status(self):
        """Строка под кнопками: подключение + активный режим."""
        conn = "connected" if self._driver.connected else "disconnected"
        tab_names = {0: "Color", 2: "Screen Mirror", 3: "Scenes", 4: "Audio"}
        selected = tab_names.get(self._current_tab)
        sel = f" · selected: {selected}" if selected else ""
        self._mode_status_label.setText(
            f"LED: {conn} · mode: {self._mode_name()}{sel}"
        )
        self._btn_mode_start.setEnabled(
            self._driver.connected and self._current_tab in self._MODE_TABS
        )
        # Stop активен когда есть что остановить — движок может жить
        # при временно отвалившемся serial.
        self._btn_mode_stop.setEnabled(self._active_mode is not None)
        # LED ON/OFF — hardware-выключатель, только при подключении.
        if hasattr(self, "_btn_on"):
            self._btn_on.setEnabled(self._driver.connected)
            self._btn_off.setEnabled(self._driver.connected)
        self._sync_cam_pause_button()

    def _sync_cam_pause_button(self):
        """Cam ⏸ имеет смысл только когда автояркость с камерой включена."""
        btn = getattr(self, "_btn_cam_pause", None)
        amb = getattr(self, "_auto_ambient_cb", None)
        auto = getattr(self, "_auto_enable_cb", None)
        if btn is None or amb is None or auto is None:
            return
        cam_ok = auto.isChecked() and amb.isChecked() and CV2_AVAILABLE
        if not cam_ok and btn.isChecked():
            # Отпускаем паузу, чтобы она не зависла невидимой.
            btn.setChecked(False)
        btn.setEnabled(cam_ok)

    def _on_mode_start(self):
        """(Re)Start: запускает режим открытой вкладки."""
        if not self._driver.connected:
            QMessageBox.warning(self, "Not connected",
                                "Connect to the controller first.")
            return
        # Если лента выключена кнопкой LED OFF — возвращаем вывод,
        # иначе запущенный режим останется тёмным.
        self._driver.switch(True)
        tab = self._current_tab
        if tab == 0:
            self._stop_all_modes()
            self._active_mode = "color"
            self._save_mode("color")
            # Сохранит preset и отправит цвет на ленту.
            self._send_current_color()
        elif tab == 2:
            self._start_screen_mirroring()
        elif tab == 3:
            self._start_scenes(self._scene_pattern_combo.currentData())
        elif tab == 4:
            self._start_audio(self._audio_mode_combo.currentData(),
                              self._audio_source_combo.currentData())
        self._update_mode_status()

    def _on_mode_stop(self):
        """Stop: останавливает режим и возвращает ленте статичный цвет.
        Для полного выключения есть кнопка LED OFF."""
        self._stop_all_modes()
        self._active_mode = None
        self._save_mode("off")
        if self._driver.connected:
            if self._preview_owns_strip():
                self._send_led_config_preview()
            else:
                self._driver.set_color(self._r, self._g, self._b)
        self._update_mode_status()

    def _stop_all_modes(self):
        """Останавливает все динамические режимы без восстановления цвета.
        Сначала сбрасываем _active_mode — тогда _mode_stopped внутри
        stop-путей не шлёт промежуточный чёрный кадр при переключении."""
        self._active_mode = None
        self._stop_screen_mirroring(restore_output=False)
        self._stop_scenes()
        self._stop_audio()

    def _on_cam_pause_toggled(self, paused: bool):
        """Пауза веб-камеры автояркости без рестарта сервиса."""
        self._auto_service.set_camera_paused(paused)
        self._btn_cam_pause.setText("Cam ▶" if paused else "Cam ⏸")

    def _on_pick_color(self):
        """Диалог выбора цвета; выбранный цвет сохраняется в preset."""
        initial = QColor(self._r, self._g, self._b)
        color = QColorDialog.getColor(initial, self, "Pick a color")
        if not color.isValid():
            return
        self._slider_r.setValue(color.red())
        self._slider_g.setValue(color.green())
        self._slider_b.setValue(color.blue())
        # setValue триггерит _on_slider_changed → preview + preset save.
        # Если активен режим Color — цвет уйдёт на ленту сразу.

    def _preview_owns_strip(self):
        """Live Preview из LED Config владеет лентой в idle-режиме.
        Пока он включён, solid-color пакеты (set_color) не должны
        сбивать per-led буфер — иначе лента моргает между preview
        и сохранённым цветом при каждом касании цветовых контролов."""
        return (self._active_mode is None
                and self._led_config_panel.live_preview)

    def _on_led_config_preview(self):
        """Live Preview из LED Config — только когда нет активного режима."""
        self._send_led_config_preview()

    def _on_live_preview_toggled(self, enabled: bool):
        """При выключении Live Preview возвращаем ленте статичный цвет,
        если никакой режим не запущен (preview-кадр иначе застрянет)."""
        if (not enabled and self._active_mode is None
                and self._driver.connected):
            self._driver.set_color(self._r, self._g, self._b)

    def _on_led_config_confirmed(self):
        """
        Confirm/Reset в LED Config: конфиг уже сохранён виджетом.
        Пере-применяем раскладку к работающим движкам; активный режим
        preview'ем не перебиваем.
        """
        cfg = self._led_config_panel.config
        print(f"[UI] LED config saved: {cfg.total} LEDs")

        # Работающие scene/audio движки — новый count + маска монитора.
        if self._scene_active and self._scene_engine is not None:
            self._apply_full_led(self._scene_engine,
                                 self._scene_full_led_cb.isChecked())
        if self._audio_active and self._audio_engine is not None:
            self._apply_full_led(self._audio_engine,
                                 self._audio_full_led_cb.isChecked())

        # Mirroring — пересобираем engine с новым конфигом (без вспышки).
        if self._screen_mirroring_active:
            try:
                self._queue_mirror_restart()
            except Exception:
                pass
            return

        # Режима нет — показываем раскладку на ленте как preview.
        if self._active_mode is None:
            self._send_led_config_preview()

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
        # Сохраняем preset + settings (headless --restore читает settings)
        self._color_preset.set_brightness(value)
        self._color_preset.save()
        self._settings.set("brightness", value)
        # Всегда синхронизируем яркость в драйвер.
        self._driver.set_brightness(value)
        # Ручное изменение ставит автояркость на паузу
        if not self._suppress_manual_pause:
            self._auto_service.notify_manual_adjustment()
        
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
        try:
            ok = self._driver.connect()
        except Exception as e:
            # Исключение в connect (serial/bridge) не должно ронять
            # слот таймера — считаем это неудачной попыткой.
            print(f"[connect] {type(e).__name__}: {e}")
            ok = False
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
            self._maybe_restore_mode()
            self._update_mode_status()
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
            self._update_mode_status()
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
        self._stop_all_modes()
        self._active_mode = None
        self._driver.disconnect()
        self._status_label.setText("Disconnected")
        self._status_label.setStyleSheet("color: #cc3333; font-weight: bold;")
        self._btn_connect.setEnabled(True)
        self._btn_disconnect.setEnabled(False)
        self._update_mode_status()

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
        Сохраняет текущий цвет в preset и app settings; отправляет на ленту
        только когда активен режим Color (запускается общей кнопкой (Re)Start).
        last_color синхронизируем всегда — иначе headless --restore поднимет
        устаревший цвет после выбора цвета под активным другим режимом.
        """
        self._color_preset.set_color(self._r, self._g, self._b)
        self._color_preset.save()
        self._settings.update(
            last_color=[self._r, self._g, self._b],
            brightness=self._slider_bright.value(),
        )
        # Resting color: лента показывает сохранённый цвет в idle и в
        # режиме Color. Во время динамических режимов не трогаем драйвер —
        # set_color очистил бы per-led буфер и убил бы работающий режим.
        if (self._driver.connected and self._active_mode in (None, "color")
                and not self._preview_owns_strip()):
            self._driver.set_color(self._r, self._g, self._b)
            self._update_mode_status()

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
        self._scene_pattern_combo.currentIndexChanged.connect(self._on_scene_pattern_changed)
        pattern_layout.addWidget(self._scene_pattern_combo)
        layout.addWidget(pattern_group)

        speed_group = QGroupBox("Options")
        speed_layout = QVBoxLayout(speed_group)
        
        # Speed row
        speed_row = QHBoxLayout()
        speed_row.addWidget(QLabel("Speed:"))
        self._scene_speed_slider = QSlider(Qt.Orientation.Horizontal)
        self._scene_speed_slider.setRange(25, 400)
        self._scene_speed_slider.setValue(100)
        self._scene_speed_slider.setToolTip("Animation speed (25% - 400%)")
        self._scene_speed_label = QLabel("100%")
        self._scene_speed_slider.valueChanged.connect(self._on_scene_speed_changed)
        speed_row.addWidget(self._scene_speed_slider)
        self._scene_speed_label.setFixedWidth(40)
        speed_row.addWidget(self._scene_speed_label)
        speed_layout.addLayout(speed_row)

        # Full LED Checkbox — сохраняется между запусками, применяется на лету.
        self._scene_full_led_cb = QCheckBox("Full LED (Использовать все светодиоды)")
        self._scene_full_led_cb.setChecked(
            bool(self._settings.get("scene_full_led", True)))
        self._scene_full_led_cb.setToolTip("Если выключено, анимация будет игнорировать углы без ленты (как в Screen Mirroring).")
        self._scene_full_led_cb.toggled.connect(self._on_scene_full_led_toggled)
        speed_layout.addWidget(self._scene_full_led_cb)
        
        layout.addWidget(speed_group)

        hint = QLabel("Запуск/остановка — общей кнопкой (Re)Start внизу окна.")
        hint.setStyleSheet("color: #6c7086;")
        layout.addWidget(hint)
        layout.addStretch()

    def _on_scene_speed_changed(self):
        v = self._scene_speed_slider.value()
        self._scene_speed_label.setText(f"{v}%")
        if self._scene_engine is not None:
            self._scene_engine.set_speed(v / 100.0)

    def _on_scene_pattern_changed(self, index):
        """Live-переключение паттерна без остановки engine."""
        pattern = self._scene_pattern_combo.currentData()
        if self._scene_active and self._scene_engine is not None and pattern in PATTERNS:
            self._scene_engine.set_pattern(pattern)
            self._scene_status_label.setText(f"Running: {PATTERN_LABELS.get(pattern, pattern)}")
        self._update_mode_status()

    def _apply_full_led(self, engine, checked: bool):
        """Live-переключение Full LED: число LED + маска монитора
        применяются атомарно, чтобы ни один кадр не ушёл в
        рассогласованной конфигурации."""
        count = self._mode_led_count(checked)
        leds = None
        if not checked:
            layout_data = build_layout(self._led_config_panel.config, 100, 100, 0.08)
            leds = layout_data.leds
        if hasattr(engine, "set_output_config"):
            engine.set_output_config(led_count=count, layout_leds=leds)
        else:
            engine.set_led_count(count)
            engine.set_layout(leds)

    def _on_scene_full_led_toggled(self, checked: bool):
        self._settings.update(scene_full_led=checked)
        if self._scene_active and self._scene_engine is not None:
            self._apply_full_led(self._scene_engine, checked)

    def _mode_led_count(self, full_led: bool) -> int:
        """Кол-во LED для движка: Full LED — вся физическая лента
        (включая start_offset-диапазон), иначе — логические по монитору."""
        cfg = self._led_config_panel.config
        if full_led:
            return min(MAX_LEDS, cfg.total + cfg.start_offset)
        return cfg.total

    def _start_scenes(self, pattern_name: str):
        self._stop_all_modes()
        self._scene_thread = QThread()
        full_led = self._scene_full_led_cb.isChecked()
        self._scene_engine = SceneEngine(
            led_count=self._mode_led_count(full_led), fps=20)

        if not full_led:
            layout_data = build_layout(self._led_config_panel.config, 100, 100, 0.08)
            self._scene_engine.set_layout(layout_data.leds)

        engine = self._scene_engine
        engine.moveToThread(self._scene_thread)
        engine.frame_ready.connect(self._on_scene_frame_ready)
        engine.error_occurred.connect(self._on_scene_error)
        # Захватываем engine локально: если stop/старт успеет заменить
        # self._scene_engine до срабатывания started, старый engine
        # всё равно получит свой start (и будет остановлен при cleanup).
        self._scene_thread.started.connect(lambda: engine.start(pattern_name))
        self._scene_thread.start()
        self._scene_active = True
        self._active_mode = "scene"
        self._scene_engine.set_speed(self._scene_speed_slider.value() / 100.0)
        self._scene_status_label.setText(f"Running: {PATTERN_LABELS.get(pattern_name, pattern_name)}")
        self._scene_status_label.setStyleSheet("color: #2d8c2d; font-weight: bold;")
        self._btn_mode_stop.setEnabled(True)
        self._save_mode("scene", scene_pattern=pattern_name,
                        scene_speed=self._scene_speed_slider.value() / 100.0,
                        scene_full_led=full_led)

    def _stop_scenes(self):
        if self._scene_engine is not None:
            self._scene_engine.stop()
            self._scene_engine = None
        if self._scene_thread is not None:
            self._scene_thread.quit()
            self._scene_thread.wait(2000)
            self._scene_thread = None
        was_active = self._scene_active
        self._scene_active = False
        self._scene_status_label.setText("Idle")
        self._scene_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        if was_active:
            self._mode_stopped("scene")

    def _mode_stopped(self, mode: str):
        """Режим завершился (стоп или ошибка): сбрасываем _active_mode
        и возвращаем ленте сохранённый статичный цвет — иначе она
        замораживается на последнем кадре движка.
        Только если остановлен именно текущий режим — при переключении
        режимов _stop_all_modes уже сбросил _active_mode, и промежуточный
        кадр не нужен."""
        if self._active_mode != mode:
            return
        self._active_mode = None
        if self._driver.connected:
            if self._preview_owns_strip():
                self._send_led_config_preview()
            else:
                self._driver.set_color(self._r, self._g, self._b)
        self._update_mode_status()

    def _on_scene_frame_ready(self, colors):
        if self.sender() is not self._scene_engine:
            return
        if self._scene_active and self._driver.connected:
            self._driver.set_per_led_colors(colors)

    def _on_scene_error(self, msg):
        if self.sender() is not self._scene_engine:
            return
        # Сначала стопаем (статус "Idle"), потом показываем ошибку —
        # иначе _stop_scenes затирает текст.
        self._stop_scenes()
        self._scene_status_label.setText(f"Error: {msg}")
        self._scene_status_label.setStyleSheet("color: #f38ba8; font-weight: bold;")

    # endregion

    # region Audio tab

    def _build_audio_tab(self, layout):
        """Строит вкладку Audio: source, mode, sensitivity, gain, color shift, fps, start/stop."""
        self._audio_status_label = QLabel("Idle")
        self._audio_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        layout.addWidget(self._audio_status_label)

        mode_group = QGroupBox("Audio Settings")
        mode_layout = QVBoxLayout(mode_group)

        # Audio Source — конкретное устройство вывода (loopback) или микрофон
        source_row = QHBoxLayout()
        source_row.addWidget(QLabel("Source:"))
        self._audio_source_combo = QComboBox()
        self._audio_source_combo.setSizeAdjustPolicy(
            QComboBox.SizeAdjustPolicy.AdjustToMinimumContentsLengthWithIcon
        )
        source_row.addWidget(self._audio_source_combo, stretch=1)
        btn_audio_refresh = QPushButton("Refresh")
        btn_audio_refresh.setToolTip("Перечислить аудио-устройства заново")
        btn_audio_refresh.clicked.connect(self._populate_audio_sources)
        source_row.addWidget(btn_audio_refresh)
        mode_layout.addLayout(source_row)
        self._populate_audio_sources()

        # Mode — переключается на лету, без остановки захвата
        mode_row = QHBoxLayout()
        mode_row.addWidget(QLabel("Mode:"))
        self._audio_mode_combo = QComboBox()
        for key, label in MODE_LABELS.items():
            self._audio_mode_combo.addItem(label, key)
        self._audio_mode_combo.currentIndexChanged.connect(self._on_audio_mode_changed)
        mode_row.addWidget(self._audio_mode_combo)
        mode_layout.addLayout(mode_row)

        # Input level — показывает, слышит ли выбранный источник звук.
        # Плоский бар: если он на нуле — причина «молчит лента» в источнике,
        # а не в режиме/движке.
        level_row = QHBoxLayout()
        level_row.addWidget(QLabel("Input:"))
        self._audio_level_bar = QProgressBar()
        self._audio_level_bar.setRange(0, 100)
        self._audio_level_bar.setValue(0)
        self._audio_level_bar.setTextVisible(False)
        self._audio_level_bar.setFixedHeight(10)
        self._audio_level_bar.setToolTip(
            "Уровень входного сигнала. На нуле — источник не слышит звук:\n"
            "выберите Output-монитор ваших колонок или другой микрофон."
        )
        level_row.addWidget(self._audio_level_bar, stretch=1)
        mode_layout.addLayout(level_row)

        layout.addWidget(mode_group)

        sens_group = QGroupBox("Options")
        sens_layout = QVBoxLayout(sens_group)

        # Sensitivity
        sens_row = QHBoxLayout()
        sens_row.addWidget(QLabel("Sensitivity:"))
        self._audio_sens_slider = QSlider(Qt.Orientation.Horizontal)
        self._audio_sens_slider.setRange(10, 500)
        self._audio_sens_slider.setValue(150)
        self._audio_sens_label = QLabel("150%")
        self._audio_sens_slider.valueChanged.connect(self._on_audio_sens_changed)
        sens_row.addWidget(self._audio_sens_slider)
        self._audio_sens_label.setFixedWidth(40)
        sens_row.addWidget(self._audio_sens_label)
        sens_layout.addLayout(sens_row)

        # Gain / Brightness
        gain_row = QHBoxLayout()
        gain_row.addWidget(QLabel("Gain:"))
        self._audio_gain_slider = QSlider(Qt.Orientation.Horizontal)
        self._audio_gain_slider.setRange(0, 300)
        self._audio_gain_slider.setValue(100)
        self._audio_gain_slider.setToolTip("Software brightness gain for audio effect")
        self._audio_gain_label = QLabel("100%")
        self._audio_gain_slider.valueChanged.connect(self._on_audio_gain_changed)
        gain_row.addWidget(self._audio_gain_slider)
        self._audio_gain_label.setFixedWidth(40)
        gain_row.addWidget(self._audio_gain_label)
        sens_layout.addLayout(gain_row)

        # Color Shift
        shift_row = QHBoxLayout()
        shift_row.addWidget(QLabel("Color Shift:"))
        self._audio_shift_slider = QSlider(Qt.Orientation.Horizontal)
        self._audio_shift_slider.setRange(0, 100)
        self._audio_shift_slider.setValue(0)
        self._audio_shift_slider.setToolTip("Rotate base hue of audio modes")
        self._audio_shift_label = QLabel("0%")
        self._audio_shift_slider.valueChanged.connect(self._on_audio_shift_changed)
        shift_row.addWidget(self._audio_shift_slider)
        self._audio_shift_label.setFixedWidth(40)
        shift_row.addWidget(self._audio_shift_label)
        sens_layout.addLayout(shift_row)

        # FPS
        fps_row = QHBoxLayout()
        fps_row.addWidget(QLabel("FPS:"))
        self._audio_fps_slider = QSlider(Qt.Orientation.Horizontal)
        self._audio_fps_slider.setRange(5, 60)
        self._audio_fps_slider.setValue(20)
        self._audio_fps_slider.setToolTip("Target frames per second")
        self._audio_fps_label = QLabel("20")
        self._audio_fps_slider.valueChanged.connect(self._on_audio_fps_changed)
        fps_row.addWidget(self._audio_fps_slider)
        self._audio_fps_label.setFixedWidth(40)
        fps_row.addWidget(self._audio_fps_label)
        sens_layout.addLayout(fps_row)

        # Full LED Checkbox — сохраняется, применяется на лету.
        self._audio_full_led_cb = QCheckBox("Full LED (Использовать все светодиоды)")
        self._audio_full_led_cb.setChecked(
            bool(self._settings.get("audio_full_led", True)))
        self._audio_full_led_cb.setToolTip("Если выключено, анимация будет игнорировать углы без ленты (как в Screen Mirroring).")
        self._audio_full_led_cb.toggled.connect(self._on_audio_full_led_toggled)
        sens_layout.addWidget(self._audio_full_led_cb)

        layout.addWidget(sens_group)

        hint = QLabel("Запуск/остановка — общей кнопкой (Re)Start внизу окна.")
        hint.setStyleSheet("color: #6c7086;")
        layout.addWidget(hint)
        layout.addStretch()

    def _on_audio_sens_changed(self):
        v = self._audio_sens_slider.value()
        self._audio_sens_label.setText(f"{v}%")
        if self._audio_engine is not None:
            self._audio_engine.set_sensitivity(v / 100.0)

    def _on_audio_gain_changed(self):
        v = self._audio_gain_slider.value()
        self._audio_gain_label.setText(f"{v}%")
        if self._audio_engine is not None:
            self._audio_engine.set_gain(v / 100.0)

    def _on_audio_shift_changed(self):
        v = self._audio_shift_slider.value()
        self._audio_shift_label.setText(f"{v}%")
        if self._audio_engine is not None:
            self._audio_engine.set_color_shift(v / 100.0)

    def _on_audio_fps_changed(self):
        v = self._audio_fps_slider.value()
        self._audio_fps_label.setText(str(v))
        if self._audio_engine is not None:
            self._audio_engine.set_fps(v)
            self._update_audio_status_running()

    def _populate_audio_sources(self):
        """Перечисляет устройства захвата: выходы (loopback) и микрофоны."""
        prev = self._audio_source_combo.currentData()
        self._audio_source_combo.blockSignals(True)
        self._audio_source_combo.clear()
        for dev_id, label, _is_lb in list_capture_devices():
            self._audio_source_combo.addItem(label, dev_id)
        if prev is not None:
            for i in range(self._audio_source_combo.count()):
                if self._audio_source_combo.itemData(i) == prev:
                    self._audio_source_combo.setCurrentIndex(i)
                    break
        self._audio_source_combo.blockSignals(False)

    def _on_audio_mode_changed(self, index):
        """Live-переключение аудио-режима без остановки захвата."""
        mode = self._audio_mode_combo.currentData()
        if self._audio_active and self._audio_engine is not None and mode in AUDIO_MODES:
            self._audio_engine.set_mode(mode)
            self._update_audio_status_running()
            self._update_mode_status()

    def _on_audio_full_led_toggled(self, checked: bool):
        self._settings.update(audio_full_led=checked)
        if self._audio_active and self._audio_engine is not None:
            self._apply_full_led(self._audio_engine, checked)

    def _update_audio_status_running(self):
        if self._audio_active:
            mode = self._audio_mode_combo.currentData()
            fps = self._audio_fps_slider.value()
            self._audio_status_label.setText(f"Running: {MODE_LABELS.get(mode, mode)} · {fps} FPS")
            self._audio_status_label.setStyleSheet("color: #2d8c2d; font-weight: bold;")

    def _start_audio(self, mode_name: str, device_id):
        self._stop_all_modes()
        self._audio_error_msg = None
        self._audio_thread = QThread()
        full_led = self._audio_full_led_cb.isChecked()
        fps = self._audio_fps_slider.value()
        self._audio_engine = AudioEngine(
            led_count=self._mode_led_count(full_led), fps=fps)

        if not full_led:
            layout_data = build_layout(self._led_config_panel.config, 100, 100, 0.08)
            self._audio_engine.set_layout(layout_data.leds)

        engine = self._audio_engine
        engine.moveToThread(self._audio_thread)
        engine.frame_ready.connect(self._on_audio_frame_ready)
        engine.error_occurred.connect(self._on_audio_error)
        engine.status_changed.connect(self._on_audio_status_changed)
        engine.level_changed.connect(self._on_audio_level)
        self._audio_thread.started.connect(lambda: engine.start(mode_name, device_id=device_id))
        self._audio_thread.start()
        self._audio_active = True
        self._active_mode = "audio"
        self._save_mode("audio", audio_mode=mode_name,
                        audio_device=device_id,
                        audio_fps=self._audio_fps_slider.value(),
                        audio_full_led=full_led)
        self._audio_engine.set_sensitivity(self._audio_sens_slider.value() / 100.0)
        self._audio_engine.set_gain(self._audio_gain_slider.value() / 100.0)
        self._audio_engine.set_color_shift(self._audio_shift_slider.value() / 100.0)
        self._audio_status_label.setText(f"Starting: {MODE_LABELS.get(mode_name, mode_name)}...")
        self._audio_status_label.setStyleSheet("color: #cc9933; font-weight: bold;")
        self._btn_mode_stop.setEnabled(True)

    def _stop_audio(self):
        if self._audio_engine is not None:
            self._audio_engine.stop()
            self._audio_engine = None
        if self._audio_thread is not None:
            self._audio_thread.quit()
            self._audio_thread.wait(2000)
            self._audio_thread = None
        was_active = self._audio_active
        self._audio_active = False
        self._audio_level_bar.setValue(0)
        if not getattr(self, "_audio_error_msg", None):
            self._audio_status_label.setText("Idle")
            self._audio_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        if was_active:
            self._mode_stopped("audio")

    def _on_audio_frame_ready(self, colors):
        if self.sender() is not self._audio_engine:
            return
        if self._audio_active and self._driver.connected:
            self._driver.set_per_led_colors(colors)

    def _on_audio_error(self, msg):
        if self.sender() is not self._audio_engine:
            return
        self._audio_error_msg = f"Error: {msg}"
        self._audio_status_label.setText(self._audio_error_msg)
        self._audio_status_label.setStyleSheet("color: #f38ba8; font-weight: bold;")
        self._stop_audio()
        # Восстанавливаем сообщение после _stop_audio, который ставит "Idle"
        self._audio_status_label.setText(self._audio_error_msg)
        self._audio_status_label.setStyleSheet("color: #f38ba8; font-weight: bold;")

    def _on_audio_level(self, level: float):
        """Индикатор входного уровня — обновляется из audio thread."""
        self._audio_level_bar.setValue(int(level * 100))

    def _on_audio_status_changed(self, status):
        if self.sender() is not self._audio_engine:
            return
        # "Running" / "Capturing..." / "Stopped" / "Error"
        if status in ("Running", "Capturing..."):
            self._audio_error_msg = None
            self._update_audio_status_running()
        elif status == "Stopped":
            if getattr(self, "_audio_error_msg", None):
                return  # Не перезаписываем сообщение об ошибке
            self._audio_status_label.setText("Idle")
            self._audio_status_label.setStyleSheet("color: #9399b2; font-weight: bold;")
        elif status == "Error":
            pass  # Сообщение уже установлено _on_audio_error
        else:
            self._audio_status_label.setText(status)

    # endregion

    # region Auto (автояркость + автоматизация)

    def _build_auto_tab(self, layout):
        """Вкладка Auto: автояркость по камере и времени + restore."""
        s = self._settings

        self._auto_enable_cb = QCheckBox("Enable auto brightness")
        self._auto_enable_cb.setChecked(s.get("auto_enabled"))
        self._auto_enable_cb.toggled.connect(self._on_auto_enable_toggled)
        layout.addWidget(self._auto_enable_cb)

        self._auto_status_label = QLabel("—")
        self._auto_status_label.setStyleSheet("color: #9399b2;")
        layout.addWidget(self._auto_status_label)
        note = QLabel("Ручное движение слайдера Brightness ставит авто-режим на паузу (5 мин).")
        note.setStyleSheet("color: #6c7086; font-size: 11px;")
        note.setWordWrap(True)
        layout.addWidget(note)

        # --- Ambient (веб-камера) ---
        amb_group = QGroupBox("Room light (webcam)")
        amb = QGridLayout(amb_group)

        self._auto_ambient_cb = QCheckBox("Measure room brightness via webcam")
        self._auto_ambient_cb.setChecked(s.get("auto_ambient_enabled"))
        self._auto_ambient_cb.setEnabled(CV2_AVAILABLE)
        self._auto_ambient_cb.toggled.connect(self._on_auto_param_changed)
        amb.addWidget(self._auto_ambient_cb, 0, 0, 1, 2)
        if not CV2_AVAILABLE:
            no_cv = QLabel("opencv-python-headless не установлен")
            no_cv.setStyleSheet("color: #f38ba8; font-size: 11px;")
            amb.addWidget(no_cv, 0, 2)

        amb.addWidget(QLabel("Camera index:"), 1, 0)
        self._auto_camera_spin = QSpinBox()
        self._auto_camera_spin.setRange(0, 9)
        self._auto_camera_spin.setValue(int(s.get("auto_camera_index")))
        self._auto_camera_spin.valueChanged.connect(self._on_auto_param_changed)
        amb.addWidget(self._auto_camera_spin, 1, 1)

        amb.addWidget(QLabel("Poll, s:"), 1, 2)
        self._auto_poll_spin = QDoubleSpinBox()
        self._auto_poll_spin.setRange(0.5, 60.0)
        self._auto_poll_spin.setSingleStep(0.5)
        self._auto_poll_spin.setValue(float(s.get("auto_poll_interval")))
        self._auto_poll_spin.valueChanged.connect(self._on_auto_param_changed)
        amb.addWidget(self._auto_poll_spin, 1, 3)

        self._auto_luma_dark, self._auto_luma_dark_l = self._make_slider(
            "Dark", int(s.get("auto_luma_dark")), self._on_auto_param_changed)
        self._auto_luma_bright, self._auto_luma_bright_l = self._make_slider(
            "Bright", int(s.get("auto_luma_bright")), self._on_auto_param_changed)
        self._auto_min_level, self._auto_min_level_l = self._make_slider(
            "Min", int(s.get("auto_min_level")), self._on_auto_param_changed)
        self._auto_max_level, self._auto_max_level_l = self._make_slider(
            "Max", int(s.get("auto_max_level")), self._on_auto_param_changed)
        amb.addWidget(QLabel("Luma «dark»:"), 2, 0)
        amb.addWidget(self._auto_luma_dark, 2, 1, 1, 2)
        amb.addWidget(self._auto_luma_dark_l, 2, 3)
        amb.addWidget(QLabel("Luma «bright»:"), 3, 0)
        amb.addWidget(self._auto_luma_bright, 3, 1, 1, 2)
        amb.addWidget(self._auto_luma_bright_l, 3, 3)
        amb.addWidget(QLabel("Strip min:"), 4, 0)
        amb.addWidget(self._auto_min_level, 4, 1, 1, 2)
        amb.addWidget(self._auto_min_level_l, 4, 3)
        amb.addWidget(QLabel("Strip max:"), 5, 0)
        amb.addWidget(self._auto_max_level, 5, 1, 1, 2)
        amb.addWidget(self._auto_max_level_l, 5, 3)
        layout.addWidget(amb_group)

        # --- Время суток ---
        time_group = QGroupBox("Day / night curve")
        tg = QGridLayout(time_group)

        self._auto_time_cb = QCheckBox("Enable day/night schedule")
        self._auto_time_cb.setChecked(s.get("auto_time_enabled"))
        self._auto_time_cb.toggled.connect(self._on_auto_param_changed)
        tg.addWidget(self._auto_time_cb, 0, 0, 1, 2)

        def _time_edit(hour):
            e = QTimeEdit()
            h = int(hour)
            e.setTime(QTime(h, int(round((hour - h) * 60))))
            e.setDisplayFormat("HH:mm")
            e.timeChanged.connect(self._on_auto_param_changed)
            return e

        tg.addWidget(QLabel("Day starts:"), 1, 0)
        self._auto_day_start = _time_edit(float(s.get("auto_day_start")))
        tg.addWidget(self._auto_day_start, 1, 1)
        tg.addWidget(QLabel("Night starts:"), 1, 2)
        self._auto_night_start = _time_edit(float(s.get("auto_night_start")))
        tg.addWidget(self._auto_night_start, 1, 3)

        self._auto_day_cap, self._auto_day_cap_l = self._make_slider(
            "Day", int(s.get("auto_day_cap")), self._on_auto_param_changed)
        self._auto_night_cap, self._auto_night_cap_l = self._make_slider(
            "Night", int(s.get("auto_night_cap")), self._on_auto_param_changed)
        tg.addWidget(QLabel("Day cap:"), 2, 0)
        tg.addWidget(self._auto_day_cap, 2, 1, 1, 2)
        tg.addWidget(self._auto_day_cap_l, 2, 3)
        tg.addWidget(QLabel("Night cap:"), 3, 0)
        tg.addWidget(self._auto_night_cap, 3, 1, 1, 2)
        tg.addWidget(self._auto_night_cap_l, 3, 3)

        tg.addWidget(QLabel("Transition, min:"), 4, 0)
        self._auto_transition_spin = QSpinBox()
        self._auto_transition_spin.setRange(5, 180)
        self._auto_transition_spin.setValue(int(s.get("auto_transition_min")))
        self._auto_transition_spin.valueChanged.connect(self._on_auto_param_changed)
        tg.addWidget(self._auto_transition_spin, 4, 1)

        tg.addWidget(QLabel("Night warmth:"), 4, 2)
        self._auto_warmth, self._auto_warmth_l = self._make_slider(
            "Warm", int(s.get("auto_warmth_night") * 100), self._on_auto_param_changed)
        self._auto_warmth.setRange(0, 100)
        tg.addWidget(self._auto_warmth, 4, 3)
        layout.addWidget(time_group)

        # --- Автозапуск режима ---
        self._restore_mode_cb = QCheckBox("Restore last mode on startup")
        self._restore_mode_cb.setChecked(s.get("restore_mode"))
        self._restore_mode_cb.toggled.connect(
            lambda v: self._settings.set("restore_mode", bool(v)))
        layout.addWidget(self._restore_mode_cb)

        layout.addStretch()

        # Индикатор сервиса → UI (вызывается из потока сервиса — через сигнал)
        self._auto_service.status_cb = self._auto_status_signal.emit
        self._auto_status_signal.connect(self._auto_status_label.setText)

        if self._auto_enable_cb.isChecked():
            self._auto_service.start()

    def _on_auto_enable_toggled(self, checked):
        self._settings.set("auto_enabled", bool(checked))
        if checked:
            self._apply_auto_params()
            self._auto_service.start()
        else:
            self._auto_service.stop()
            # Возвращаем ручную яркость и нейтральную температуру
            if self._driver.connected:
                self._driver.set_temperature(0.0)
                self._driver.set_brightness(self._slider_bright.value())
        self._sync_cam_pause_button()

    def _on_auto_param_changed(self, *args):
        """Сохраняет параметры и передаёт их в сервис."""
        self._apply_auto_params()
        self._sync_cam_pause_button()

    def _apply_auto_params(self):
        """UI → settings → service params."""
        def hhmm(e):
            t = e.time()
            return t.hour() + t.minute() / 60.0

        self._settings.update(
            auto_ambient_enabled=self._auto_ambient_cb.isChecked(),
            auto_camera_index=self._auto_camera_spin.value(),
            auto_poll_interval=self._auto_poll_spin.value(),
            auto_luma_dark=float(self._auto_luma_dark.value()),
            auto_luma_bright=float(self._auto_luma_bright.value()),
            auto_min_level=self._auto_min_level.value(),
            auto_max_level=self._auto_max_level.value(),
            auto_time_enabled=self._auto_time_cb.isChecked(),
            auto_day_start=hhmm(self._auto_day_start),
            auto_night_start=hhmm(self._auto_night_start),
            auto_day_cap=self._auto_day_cap.value(),
            auto_night_cap=self._auto_night_cap.value(),
            auto_transition_min=self._auto_transition_spin.value(),
            auto_warmth_night=self._auto_warmth.value() / 100.0,
        )
        self._auto_service.update_params(**self._settings.auto_params())

    def _maybe_restore_mode(self):
        """Восстанавливает последний активный режим — один раз при старте."""
        if self._mode_restored:
            return
        self._mode_restored = True
        if not self._settings.get("restore_mode"):
            return
        mode = self._settings.get("last_mode", "color")
        if mode == "scene":
            pattern = self._settings.get("scene_pattern", "rainbow")
            for i in range(self._scene_pattern_combo.count()):
                if self._scene_pattern_combo.itemData(i) == pattern:
                    self._scene_pattern_combo.setCurrentIndex(i)
                    break
            self._scene_speed_slider.setValue(
                int(self._settings.get("scene_speed", 1.0) * 100))
            self._tabs.setCurrentIndex(3)
            self._start_scenes(pattern)
        elif mode == "audio":
            am = self._settings.get("audio_mode", "spectrum")
            for i in range(self._audio_mode_combo.count()):
                if self._audio_mode_combo.itemData(i) == am:
                    self._audio_mode_combo.setCurrentIndex(i)
                    break
            self._audio_fps_slider.setValue(int(self._settings.get("audio_fps", 30)))
            dev = self._settings.get("audio_device")
            if dev is not None:
                for i in range(self._audio_source_combo.count()):
                    if self._audio_source_combo.itemData(i) == dev:
                        self._audio_source_combo.setCurrentIndex(i)
                        break
            self._tabs.setCurrentIndex(4)
            self._start_audio(am, self._audio_source_combo.currentData())
        elif mode == "mirror":
            self._tabs.setCurrentIndex(2)
            self._start_screen_mirroring()
        elif mode == "color":
            self._active_mode = "color"
            self._send_current_color()

    def _save_mode(self, name, **extra):
        self._settings.update(last_mode=name, **extra)

    # endregion

    # region Speed mode

    def _on_speed_mode_clicked(self, interval, label):
        for lb, btn in self._speed_buttons.items():
            btn.setChecked(lb == label)
        self._driver.set_send_interval(interval)

    # endregion

    def closeEvent(self, event):
        """При закрытии окна останавливаем mirroring, scenes, audio и отключаемся."""
        # Флашим отложенный debounce-цвет — иначе последний выбор
        # пользователя может не сохраниться.
        self._send_timer.stop()
        self._send_current_color()
        self._stop_screen_mirroring(restore_output=False)
        self._stop_scenes()
        self._stop_audio()
        self._auto_service.stop()
        if self._driver.connected:
            self._driver.disconnect()
        event.accept()
# endregion
