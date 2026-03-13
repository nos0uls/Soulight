# main_window.py — Главное окно Soulight.
#
# Содержит Color Picker (палитра, RGB слайдеры, HEX ввод),
# Brightness slider, пресеты цветов, кнопку ON/OFF,
# и статус подключения.

from PyQt6.QtWidgets import (
    QMainWindow, QWidget, QVBoxLayout, QHBoxLayout,
    QLabel, QSlider, QPushButton, QLineEdit, QGridLayout,
    QGroupBox, QFrame, QSizePolicy, QMessageBox, QTabWidget,
)
from PyQt6.QtCore import Qt, QTimer, pyqtSignal
from PyQt6.QtGui import QColor, QPainter, QLinearGradient, QMouseEvent, QFont

from soulight.protocol.serial_driver import LEDDriver
from soulight.ui.led_config_widget import LEDConfigPanel
from soulight.color_preset import ColorPreset


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
        0 = Color tab: восстанавливаем solid color preset
        1 = LED Config tab: останавливаем solid color (лента гаснет или ждёт Live Preview)
        """
        prev_tab = self._current_tab
        self._current_tab = index

        if not self._driver.connected:
            return

        if index == 0:  # Возврат на Color tab
            if prev_tab == 1:  # Был в LED Config
                # Восстанавливаем последний solid color preset
                r, g, b = self._color_preset.as_tuple()
                self._driver.set_color(r, g, b)
                self._driver.set_brightness(self._color_preset.brightness)
        elif index == 1:  # Переход в LED Config
            # Останавливаем solid color (лента гаснет)
            self._driver.set_color(0, 0, 0)

    def _on_led_config_confirmed(self):
        """
        Вызывается при нажатии Confirm в LED Config.
        Пока per-LED не работает (требует SyncConfig handshake),
        просто гасим ленту после сохранения.
        """
        cfg = self._led_config_panel.config
        print(f"[UI] LED config saved: {cfg.total} LEDs")
        cfg.save()

        if not self._driver.connected:
            return

        # После Confirm гасим ленту (до выхода из LED Config)
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
        # Отправляем только если на Color tab
        if self._driver.connected and self._current_tab == 0:
            self._driver.set_color(self._r, self._g, self._b)

    # endregion

    def closeEvent(self, event):
        """При закрытии окна отключаемся от контроллера."""
        if self._driver.connected:
            self._driver.disconnect()
        event.accept()
# endregion
