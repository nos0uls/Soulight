# app.py — Точка входа Soulight.
#
# Создаёт QApplication и показывает главное окно.
# Запуск: python -m soulight  или  python soulight/app.py

import sys
from PyQt6.QtWidgets import QApplication
from PyQt6.QtGui import QFont
from soulight.ui.main_window import MainWindow


def main():
    app = QApplication(sys.argv)
    app.setApplicationName("Soulight")

    # Тёмная тема (базовый стиль)
    app.setStyleSheet("""
        QMainWindow { background-color: #1e1e2e; }
        QWidget { background-color: #1e1e2e; color: #cdd6f4; }
        QGroupBox {
            border: 1px solid #45475a;
            border-radius: 6px;
            margin-top: 8px;
            padding-top: 14px;
            font-weight: bold;
            color: #cdd6f4;
        }
        QGroupBox::title {
            subcontrol-origin: margin;
            left: 10px;
            padding: 0 6px;
        }
        QSlider::groove:horizontal {
            border: 1px solid #45475a;
            height: 8px;
            background: #313244;
            border-radius: 4px;
        }
        QSlider::handle:horizontal {
            background: #89b4fa;
            border: 1px solid #74c7ec;
            width: 16px;
            margin: -5px 0;
            border-radius: 8px;
        }
        QSlider::sub-page:horizontal {
            background: #89b4fa;
            border-radius: 4px;
        }
        QPushButton {
            background-color: #313244;
            border: 1px solid #45475a;
            border-radius: 6px;
            padding: 6px 14px;
            color: #cdd6f4;
            font-weight: bold;
        }
        QPushButton:hover { background-color: #45475a; }
        QPushButton:pressed { background-color: #585b70; }
        QPushButton:disabled { color: #6c7086; }
        QLineEdit {
            background-color: #313244;
            border: 1px solid #45475a;
            border-radius: 4px;
            padding: 4px 8px;
            color: #cdd6f4;
        }
        QLabel { color: #cdd6f4; }
    """)

    window = MainWindow()
    window.show()
    sys.exit(app.exec())


if __name__ == "__main__":
    main()
