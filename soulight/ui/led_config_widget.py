# led_config_widget.py — Визуальный редактор расположения LED.
#
# Рисует схему монитора с LED-точками по периметру.
# Каждая сторона имеет свой цвет (как в оригинальном Beelight).
# Drag selection: удерживая мышь, можно выделять/снимать несколько LED подряд.
# Позволяет: выбрать начальный угол, направление обхода, смещение,
# количество LED на каждой стороне, включить/выключить отдельные LED.

from PyQt6.QtWidgets import (
    QWidget, QVBoxLayout, QHBoxLayout, QLabel,
    QPushButton, QSpinBox, QGroupBox, QGridLayout,
    QComboBox, QCheckBox, QSizePolicy, QFrame,
)
from PyQt6.QtCore import Qt, QRectF, QPointF, pyqtSignal
from PyQt6.QtGui import (
    QPainter, QColor, QPen, QBrush, QFont,
    QLinearGradient, QPainterPath,
)

from soulight.led_config import (
    LEDConfig, SIDE_TOP, SIDE_BOTTOM, SIDE_LEFT, SIDE_RIGHT,
    CORNER_NAMES, SIDE_COLORS, MAX_LEDS,
)


# region MonitorWidget — рисует монитор с LED точками

class MonitorWidget(QWidget):
    """
    Custom QWidget: рисует монитор с LED-точками по периметру.

    Каждая сторона имеет свой цвет (синий/красный/зелёный/жёлтый).
    Включённые LED показаны ярким цветом стороны, выключенные — серым.
    Drag: удерживая мышь, можно выделять/снимать LED непрерывно.
    Углы пронумерованы 1-4 как в оригинальном Beelight.
    """

    # Сигнал: что-то изменилось (для обновления UI панели)
    config_changed = pyqtSignal()

    # Минимальный и максимальный радиус LED (масштабируется с окном)
    LED_RADIUS_MIN = 4
    LED_RADIUS_MAX = 12
    # Отступ LED от края монитора (тоже масштабируется)
    LED_MARGIN_MIN = 14
    LED_MARGIN_MAX = 28
    # Размер области подставки монитора
    STAND_HEIGHT = 30

    def __init__(self, config: LEDConfig, parent=None):
        super().__init__(parent)
        self._config = config
        self.setMinimumSize(380, 300)
        self.setSizePolicy(QSizePolicy.Policy.Expanding, QSizePolicy.Policy.Expanding)
        # Кэш позиций LED для обработки кликов
        self._led_positions = []  # [(QPointF, side, index), ...]
        # Drag selection: режим рисования (True = включаем, False = выключаем)
        self._dragging = False
        self._drag_mode = True  # True = enable LEDs, False = disable LEDs
        # Набор уже обработанных LED во время текущего drag (чтобы не переключать дважды)
        self._drag_visited = set()

    def set_config(self, config: LEDConfig):
        """Обновляет конфигурацию и перерисовывает."""
        self._config = config
        self.update()

    def _scaled_radius(self):
        """Вычисляет радиус LED точки в зависимости от размера виджета."""
        scale = min(self.width(), self.height()) / 300.0
        r = self.LED_RADIUS_MIN + (self.LED_RADIUS_MAX - self.LED_RADIUS_MIN) * max(0, min(scale - 1, 1))
        return max(self.LED_RADIUS_MIN, min(self.LED_RADIUS_MAX, r))

    def _scaled_margin(self):
        """Вычисляет отступ LED от края монитора."""
        scale = min(self.width(), self.height()) / 300.0
        m = self.LED_MARGIN_MIN + (self.LED_MARGIN_MAX - self.LED_MARGIN_MIN) * max(0, min(scale - 1, 1))
        return max(self.LED_MARGIN_MIN, min(self.LED_MARGIN_MAX, m))

    def paintEvent(self, event):
        """Рисует монитор, LED точки, номера углов и стрелки направления."""
        p = QPainter(self)
        p.setRenderHint(QPainter.RenderHint.Antialiasing)

        w = self.width()
        h = self.height()

        # Динамический размер LED
        self._cur_radius = self._scaled_radius()
        self._cur_margin = self._scaled_margin()

        # Область монитора (с отступом для LED)
        margin = self._cur_margin + self._cur_radius + 10
        mon_x = margin + 20
        mon_y = margin + 10
        mon_w = w - 2 * mon_x
        mon_h = h - mon_y - margin - self.STAND_HEIGHT - 10

        if mon_w < 100 or mon_h < 60:
            p.end()
            return

        mon_rect = QRectF(mon_x, mon_y, mon_w, mon_h)

        # Рисуем подставку монитора
        self._draw_stand(p, mon_rect)

        # Рисуем корпус монитора (тёмный прямоугольник с рамкой)
        self._draw_monitor(p, mon_rect)

        # Рисуем LED точки по периметру (каждая сторона — свой цвет)
        self._led_positions = []
        self._draw_leds(p, mon_rect, SIDE_TOP)
        self._draw_leds(p, mon_rect, SIDE_BOTTOM)
        self._draw_leds(p, mon_rect, SIDE_LEFT)
        self._draw_leds(p, mon_rect, SIDE_RIGHT)

        # Рисуем номера углов
        self._draw_corners(p, mon_rect)

        # Рисуем стрелки направления
        self._draw_arrows(p, mon_rect)

        # Текст внутри монитора
        active = self._config.get_active_count()
        total = self._config.total
        p.setPen(QColor("#cdd6f4"))
        p.setFont(QFont("Segoe UI", 10))
        if active == total:
            text = f"All {total} LEDs selected"
        else:
            text = f"{active} / {total} LEDs active"
        p.drawText(mon_rect, Qt.AlignmentFlag.AlignCenter, text)

        p.end()

    def _draw_monitor(self, p: QPainter, rect: QRectF):
        """Рисует корпус монитора — тёмный прямоугольник с рамкой."""
        p.setPen(QPen(QColor("#585b70"), 2))
        p.setBrush(QColor("#11111b"))
        p.drawRoundedRect(rect, 8, 8)

        inner = rect.adjusted(6, 6, -6, -6)
        p.setPen(Qt.PenStyle.NoPen)
        p.setBrush(QColor("#181825"))
        p.drawRoundedRect(inner, 4, 4)

    def _draw_stand(self, p: QPainter, mon_rect: QRectF):
        """Рисует подставку монитора."""
        cx = mon_rect.center().x()
        bottom = mon_rect.bottom()

        p.setPen(Qt.PenStyle.NoPen)
        p.setBrush(QColor("#45475a"))
        p.drawRect(QRectF(cx - 15, bottom, 30, 20))
        p.drawRoundedRect(QRectF(cx - 50, bottom + 18, 100, 8), 3, 3)

    def _draw_leds(self, p: QPainter, mon_rect: QRectF, side: str):
        """
        Рисует LED точки вдоль одной стороны монитора.
        Включённые — яркий цвет стороны, выключенные — серый.
        """
        count = self._config.counts.get(side, 0)
        if count == 0:
            return

        enabled = self._config.enabled.get(side, [])
        positions = self._calc_led_positions(mon_rect, side, count)

        # Цвет стороны из SIDE_COLORS
        sr, sg, sb = SIDE_COLORS.get(side, (180, 180, 180))

        for i, pos in enumerate(positions):
            is_on = enabled[i] if i < len(enabled) else True

            if is_on:
                # Яркий цвет стороны
                color = QColor(sr, sg, sb)
                # Светлый бордер
                border = QColor(min(sr + 60, 255), min(sg + 60, 255), min(sb + 60, 255))
            else:
                # Тусклый серый (выключен)
                color = QColor("#45475a")
                border = QColor("#585b70")

            r = self._cur_radius if hasattr(self, '_cur_radius') else self.LED_RADIUS_MIN
            p.setPen(QPen(border, 1.5))
            p.setBrush(color)
            p.drawEllipse(pos, r, r)

            # Сохраняем позицию для обработки кликов
            self._led_positions.append((pos, side, i))

    def _calc_led_positions(self, mon_rect: QRectF, side: str, count: int):
        """Вычисляет позиции LED точек вдоль стороны монитора."""
        offset = self._cur_margin if hasattr(self, '_cur_margin') else self.LED_MARGIN_MIN
        positions = []

        if side == SIDE_TOP:
            y = mon_rect.top() - offset
            x_start = mon_rect.left()
            x_end = mon_rect.right()
            for i in range(count):
                t = (i + 0.5) / count
                positions.append(QPointF(x_start + t * (x_end - x_start), y))

        elif side == SIDE_BOTTOM:
            y = mon_rect.bottom() + offset
            x_start = mon_rect.left()
            x_end = mon_rect.right()
            for i in range(count):
                t = (i + 0.5) / count
                positions.append(QPointF(x_start + t * (x_end - x_start), y))

        elif side == SIDE_LEFT:
            x = mon_rect.left() - offset
            y_start = mon_rect.top()
            y_end = mon_rect.bottom()
            for i in range(count):
                t = (i + 0.5) / count
                positions.append(QPointF(x, y_start + t * (y_end - y_start)))

        elif side == SIDE_RIGHT:
            x = mon_rect.right() + offset
            y_start = mon_rect.top()
            y_end = mon_rect.bottom()
            for i in range(count):
                t = (i + 0.5) / count
                positions.append(QPointF(x, y_start + t * (y_end - y_start)))

        return positions

    def _draw_corners(self, p: QPainter, mon_rect: QRectF):
        """Рисует номера углов (1-4) около углов монитора."""
        font = QFont("Segoe UI", 9, QFont.Weight.Bold)
        p.setFont(font)

        led_margin = self._cur_margin if hasattr(self, '_cur_margin') else self.LED_MARGIN_MIN
        led_radius = self._cur_radius if hasattr(self, '_cur_radius') else self.LED_RADIUS_MIN
        corner_offset = led_margin + led_radius + 16

        corner_positions = {
            1: QPointF(mon_rect.right() + corner_offset, mon_rect.bottom() + corner_offset),
            2: QPointF(mon_rect.right() + corner_offset, mon_rect.top() - corner_offset + 6),
            3: QPointF(mon_rect.left() - corner_offset - 6, mon_rect.top() - corner_offset + 6),
            4: QPointF(mon_rect.left() - corner_offset - 6, mon_rect.bottom() + corner_offset),
        }

        for num, pos in corner_positions.items():
            if num == self._config.start_corner:
                p.setPen(QColor("#f9e2af"))
                p.setBrush(QColor("#f9e2af"))
                p.drawEllipse(pos, 10, 10)
                p.setPen(QColor("#1e1e2e"))
            else:
                p.setPen(QColor("#6c7086"))
                p.setBrush(Qt.BrushStyle.NoBrush)
                p.drawEllipse(pos, 10, 10)
                p.setPen(QColor("#6c7086"))

            p.drawText(QRectF(pos.x() - 10, pos.y() - 8, 20, 16),
                       Qt.AlignmentFlag.AlignCenter, str(num))

    def _draw_arrows(self, p: QPainter, mon_rect: QRectF):
        """Рисует стрелки направления на каждой стороне."""
        p.setPen(QPen(QColor("#89b4fa"), 2))
        arrow_size = 8

        cx = mon_rect.center().x()
        cy = mon_rect.center().y()
        led_margin = self._cur_margin if hasattr(self, '_cur_margin') else self.LED_MARGIN_MIN
        led_radius = self._cur_radius if hasattr(self, '_cur_radius') else self.LED_RADIUS_MIN
        offset = led_margin + led_radius + 6

        cw = self._config.clockwise

        ax = cx
        ay = mon_rect.top() - offset - 12
        self._draw_arrow_head(p, QPointF(ax, ay), "left" if cw else "right", arrow_size)

        ay = mon_rect.bottom() + offset + 12
        self._draw_arrow_head(p, QPointF(ax, ay), "right" if cw else "left", arrow_size)

        ax = mon_rect.left() - offset - 12
        ay = cy
        self._draw_arrow_head(p, QPointF(ax, ay), "down" if cw else "up", arrow_size)

        ax = mon_rect.right() + offset + 12
        self._draw_arrow_head(p, QPointF(ax, ay), "up" if cw else "down", arrow_size)

    def _draw_arrow_head(self, p: QPainter, center: QPointF, direction: str, size: float):
        """Рисует стрелку-треугольник в указанном направлении."""
        path = QPainterPath()
        cx, cy = center.x(), center.y()

        if direction == "right":
            path.moveTo(cx + size, cy)
            path.lineTo(cx - size, cy - size)
            path.lineTo(cx - size, cy + size)
        elif direction == "left":
            path.moveTo(cx - size, cy)
            path.lineTo(cx + size, cy - size)
            path.lineTo(cx + size, cy + size)
        elif direction == "up":
            path.moveTo(cx, cy - size)
            path.lineTo(cx - size, cy + size)
            path.lineTo(cx + size, cy + size)
        elif direction == "down":
            path.moveTo(cx, cy + size)
            path.lineTo(cx - size, cy - size)
            path.lineTo(cx + size, cy - size)

        path.closeSubpath()
        p.setBrush(QColor("#89b4fa"))
        p.drawPath(path)

    # region Drag selection — непрерывное выделение при удержании мыши

    def _find_led_at(self, point):
        """Находит LED под курсором. Возвращает (side, index) или None."""
        for pos, side, index in self._led_positions:
            dx = point.x() - pos.x()
            dy = point.y() - pos.y()
            r = self._cur_radius if hasattr(self, '_cur_radius') else self.LED_RADIUS_MIN
            if dx * dx + dy * dy <= (r + 5) ** 2:
                return (side, index)
        return None

    def mousePressEvent(self, event):
        """Начало drag: определяем режим (enable/disable) по первому LED."""
        hit = self._find_led_at(event.position())
        if hit is None:
            return

        side, index = hit
        self._dragging = True
        self._drag_visited = set()

        # Режим: если LED был включён — выключаем при drag, и наоборот
        enabled = self._config.enabled.get(side, [])
        current_state = enabled[index] if index < len(enabled) else True
        self._drag_mode = not current_state  # Инвертируем

        # Применяем к первому LED
        self._config.set_led(side, index, self._drag_mode)
        self._drag_visited.add((side, index))
        self.config_changed.emit()
        self.update()

    def mouseMoveEvent(self, event):
        """Продолжение drag: применяем режим ко всем LED под курсором."""
        if not self._dragging:
            return

        hit = self._find_led_at(event.position())
        if hit is None:
            return

        side, index = hit
        key = (side, index)
        if key in self._drag_visited:
            return

        self._config.set_led(side, index, self._drag_mode)
        self._drag_visited.add(key)
        self.config_changed.emit()
        self.update()

    def mouseReleaseEvent(self, event):
        """Конец drag."""
        self._dragging = False
        self._drag_visited = set()

    # endregion

# endregion


# region LEDConfigPanel — панель настроек LED

class LEDConfigPanel(QWidget):
    """
    Полная панель настройки LED: MonitorWidget + контролы.

    Содержит:
    - Визуальный монитор с LED (MonitorWidget)
    - SpinBox для количества LED на каждой стороне (с лимитом 75 total)
    - Выбор начального угла (ComboBox)
    - Направление (Clockwise/Counter-clockwise)
    - Start offset (с какого LED начинается отсчёт)
    - Select All / Deselect All
    - Enable/disable по сторонам
    - Reset / Confirm
    """

    # Сигнал: конфигурация изменена и подтверждена
    config_confirmed = pyqtSignal()
    # Сигнал: изменился режим Live Preview (bool)
    live_preview_changed = pyqtSignal(bool)

    def __init__(self, config: LEDConfig = None, parent=None):
        super().__init__(parent)
        self._config = config or LEDConfig()
        self._config.load()
        self._live_preview = False  # По умолчанию manual mode
        self._init_ui()
        # Инициализируем total сразу после построения UI
        self._update_total()

    @property
    def config(self):
        return self._config

    @property
    def live_preview(self):
        """Режим Live Preview: обновлять ленту при каждом изменении."""
        return self._live_preview

    def _init_ui(self):
        """Создаёт layout: монитор сверху, контролы снизу."""
        layout = QVBoxLayout(self)
        layout.setSpacing(10)
        layout.setContentsMargins(12, 12, 12, 12)

        # === Live Preview checkbox (вверху) ===
        preview_layout = QHBoxLayout()
        self._preview_checkbox = QCheckBox("Live Preview")
        self._preview_checkbox.setChecked(False)
        self._preview_checkbox.setToolTip(
            "When enabled: LED strip updates immediately as you change settings.\n"
            "When disabled: LED strip updates only on Confirm."
        )
        self._preview_checkbox.stateChanged.connect(self._on_preview_toggled)
        preview_layout.addWidget(self._preview_checkbox)
        preview_layout.addStretch()
        layout.addLayout(preview_layout)

        # === Монитор с LED ===
        self._monitor = MonitorWidget(self._config)
        self._monitor.config_changed.connect(self._on_monitor_changed)
        layout.addWidget(self._monitor, stretch=3)

        # === Контролы ===
        controls_layout = QHBoxLayout()

        # Левая колонка: количество LED по сторонам
        counts_group = QGroupBox("LED Count")
        counts_layout = QGridLayout(counts_group)
        counts_layout.setSpacing(6)

        self._spin = {}
        for i, (side, label) in enumerate([
            (SIDE_TOP, "Top"),
            (SIDE_BOTTOM, "Bottom"),
            (SIDE_LEFT, "Left"),
            (SIDE_RIGHT, "Right"),
        ]):
            lbl = QLabel(label)
            # Цветная метка стороны
            sr, sg, sb = SIDE_COLORS.get(side, (180, 180, 180))
            lbl.setStyleSheet(f"color: rgb({sr},{sg},{sb}); font-weight: bold;")
            spin = QSpinBox()
            spin.setRange(0, MAX_LEDS)
            spin.setValue(self._config.counts[side])
            spin.valueChanged.connect(lambda val, s=side: self._on_count_changed(s, val))
            counts_layout.addWidget(lbl, i, 0)
            counts_layout.addWidget(spin, i, 1)
            self._spin[side] = spin

        # Общее количество (readonly)
        total_lbl = QLabel("Total:")
        total_lbl.setStyleSheet("font-weight: bold;")
        self._total_label = QLabel(f"{self._config.total} / {MAX_LEDS}")
        self._total_label.setStyleSheet("font-weight: bold; color: #89b4fa;")
        counts_layout.addWidget(total_lbl, 4, 0)
        counts_layout.addWidget(self._total_label, 4, 1)

        controls_layout.addWidget(counts_group)

        # Правая колонка: угол, направление, offset, кнопки
        settings_group = QGroupBox("Settings")
        settings_layout = QVBoxLayout(settings_group)

        # Start corner
        corner_layout = QHBoxLayout()
        corner_layout.addWidget(QLabel("Start:"))
        self._corner_combo = QComboBox()
        for num, name in CORNER_NAMES.items():
            self._corner_combo.addItem(f"{num} — {name}", num)
        self._corner_combo.setCurrentIndex(self._config.start_corner - 1)
        self._corner_combo.currentIndexChanged.connect(self._on_corner_changed)
        corner_layout.addWidget(self._corner_combo)
        settings_layout.addLayout(corner_layout)

        # Direction
        dir_layout = QHBoxLayout()
        dir_layout.addWidget(QLabel("Direction:"))
        self._dir_combo = QComboBox()
        self._dir_combo.addItem("Clockwise", True)
        self._dir_combo.addItem("Counter-clockwise", False)
        self._dir_combo.setCurrentIndex(0 if self._config.clockwise else 1)
        self._dir_combo.currentIndexChanged.connect(self._on_dir_changed)
        dir_layout.addWidget(self._dir_combo)
        settings_layout.addLayout(dir_layout)

        # Start offset (с какого LED начинается отсчёт)
        offset_layout = QHBoxLayout()
        offset_layout.addWidget(QLabel("Offset:"))
        self._offset_spin = QSpinBox()
        self._offset_spin.setRange(0, MAX_LEDS - 1)
        self._offset_spin.setValue(self._config.start_offset)
        self._offset_spin.setToolTip("С какого физического LED начинается отсчёт")
        self._offset_spin.valueChanged.connect(self._on_offset_changed)
        offset_layout.addWidget(self._offset_spin)
        settings_layout.addLayout(offset_layout)

        # Side enable/disable buttons (цветные)
        sides_layout = QHBoxLayout()
        for side, label in [(SIDE_TOP, "T"), (SIDE_BOTTOM, "B"), (SIDE_LEFT, "L"), (SIDE_RIGHT, "R")]:
            btn = QPushButton(label)
            btn.setFixedSize(32, 28)
            sr, sg, sb = SIDE_COLORS.get(side, (180, 180, 180))
            btn.setToolTip(f"Toggle all LEDs on {side} side")
            btn.setStyleSheet(
                f"background-color: rgb({sr},{sg},{sb}); color: #1e1e2e; "
                f"font-weight: bold; border-radius: 4px;"
            )
            btn.clicked.connect(lambda checked, s=side: self._on_side_toggle(s))
            sides_layout.addWidget(btn)
        settings_layout.addLayout(sides_layout)

        # Select All / Deselect All
        sel_layout = QHBoxLayout()
        btn_all = QPushButton("Select All")
        btn_all.clicked.connect(lambda: self._select_all(True))
        sel_layout.addWidget(btn_all)
        btn_none = QPushButton("Deselect All")
        btn_none.clicked.connect(lambda: self._select_all(False))
        sel_layout.addWidget(btn_none)
        settings_layout.addLayout(sel_layout)

        controls_layout.addWidget(settings_group)
        layout.addLayout(controls_layout)

        # === Reset / Confirm (с hover эффектами) ===
        btn_layout = QHBoxLayout()
        btn_reset = QPushButton("Reset")
        btn_reset.setStyleSheet(
            "QPushButton { background-color: #cc3333; color: white; "
            "font-weight: bold; border-radius: 6px; padding: 8px; }"
            "QPushButton:hover { background-color: #a32929; }"
            "QPushButton:pressed { background-color: #8a2222; }"
        )
        btn_reset.clicked.connect(self._on_reset)
        btn_layout.addWidget(btn_reset)

        btn_confirm = QPushButton("Confirm")
        btn_confirm.setStyleSheet(
            "QPushButton { background-color: #2d8c2d; color: white; "
            "font-weight: bold; border-radius: 6px; padding: 8px; }"
            "QPushButton:hover { background-color: #247024; }"
            "QPushButton:pressed { background-color: #1c5a1c; }"
        )
        btn_confirm.clicked.connect(self._on_confirm)
        btn_layout.addWidget(btn_confirm)
        layout.addLayout(btn_layout)

    # region Обработчики событий

    def _on_preview_toggled(self, state):
        """Live Preview checkbox переключён."""
        self._live_preview = (state == Qt.CheckState.Checked.value)
        self.live_preview_changed.emit(self._live_preview)
        # При включении Live Preview сразу отправляем текущую конфигурацию,
        # но только если она валидна.
        if self._live_preview:
            self._maybe_send_preview()

    def _on_count_changed(self, side, value):
        """Изменилось количество LED на стороне (с учётом лимита 75 и offset)."""
        self._config.set_count(side, value, max_total=self._effective_max())
        # SpinBox мог быть ограничен — обновляем отображение
        actual = self._config.counts[side]
        if actual != value:
            spin = self._spin[side]
            spin.blockSignals(True)
            spin.setValue(actual)
            spin.blockSignals(False)
        self._update_total()
        self._monitor.update()
        self._maybe_send_preview()

    def _on_corner_changed(self, index):
        """Изменился начальный угол."""
        self._config.start_corner = self._corner_combo.currentData()
        self._monitor.update()
        self._maybe_send_preview()

    def _on_dir_changed(self, index):
        """Изменилось направление обхода."""
        self._config.clockwise = self._dir_combo.currentData()
        self._monitor.update()
        self._maybe_send_preview()

    def _on_offset_changed(self, value):
        """
        Изменилось смещение начального LED.
        Offset уменьшает доступный лимит: total + offset ≤ MAX_LEDS.
        """
        self._config.start_offset = value
        self._update_total()
        self._maybe_send_preview()

    def _on_monitor_changed(self):
        """Монитор-виджет изменил состояние LED (drag/click)."""
        self._update_total()
        self._maybe_send_preview()

    def _on_side_toggle(self, side):
        """Переключает все LED на стороне (если все включены — выключаем, иначе включаем)."""
        enabled = self._config.enabled.get(side, [])
        all_on = all(enabled) if enabled else False
        self._config.set_side(side, not all_on)
        self._update_total()
        self._monitor.update()
        self._maybe_send_preview()

    def _select_all(self, on):
        """Включает/выключает все LED."""
        self._config.set_all(on)
        self._update_total()
        self._monitor.update()
        self._maybe_send_preview()

    def _effective_max(self):
        """Эффективный лимит LED с учётом offset: MAX_LEDS - offset."""
        return MAX_LEDS - self._config.start_offset

    def _is_valid_config(self):
        """True, если total + offset не превышает MAX_LEDS."""
        return self._config.total + self._config.start_offset <= MAX_LEDS

    def _maybe_send_preview(self):
        """Отправляет Live Preview только если конфигурация валидна."""
        if not self._live_preview:
            return
        if self._is_valid_config():
            self._total_label.setToolTip("")
            self.config_confirmed.emit()
        else:
            self._total_label.setToolTip(
                "Total + offset exceeds MAX_LEDS. Live preview blocked."
            )

    def _update_total(self):
        """Обновляет отображение общего количества LED и ограничивает SpinBox."""
        total = self._config.total
        eff_max = self._effective_max()
        # Показываем effective limit с учётом offset
        color = "#f38ba8" if total > eff_max else "#89b4fa"  # Красный если превышен
        self._total_label.setText(f"{total} / {eff_max}")
        self._total_label.setStyleSheet(f"font-weight: bold; color: {color};")
        # Обновляем максимумы SpinBox с учётом offset
        for side, spin in self._spin.items():
            other_used = sum(c for s, c in self._config.counts.items() if s != side)
            max_val = max(0, eff_max - other_used)
            spin.blockSignals(True)
            spin.setMaximum(max_val)
            spin.blockSignals(False)

    def _on_reset(self):
        """Сбрасывает конфигурацию к значениям по умолчанию и сохраняет её."""
        self._config = LEDConfig()
        self._config.save()
        self._monitor.set_config(self._config)
        for side, spin in self._spin.items():
            spin.blockSignals(True)
            spin.setValue(self._config.counts[side])
            spin.blockSignals(False)
        self._update_total()
        self._corner_combo.setCurrentIndex(0)
        self._dir_combo.setCurrentIndex(0)
        self._offset_spin.setValue(0)
        self._maybe_send_preview()

    def _on_confirm(self):
        """Сохраняет конфигурацию. Блокирует если total + offset > MAX_LEDS."""
        eff_max = self._effective_max()
        if self._config.total > eff_max:
            from PyQt6.QtWidgets import QMessageBox
            QMessageBox.warning(
                self, "Invalid config",
                f"Total LEDs ({self._config.total}) + offset ({self._config.start_offset}) "
                f"exceeds hardware limit ({MAX_LEDS}).\n"
                f"Reduce LED count or offset."
            )
            return
        self._config.save()
        self.config_confirmed.emit()

    # endregion

# endregion
