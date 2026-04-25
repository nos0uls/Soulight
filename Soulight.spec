# -*- mode: python ; coding: utf-8 -*-
# Soulight PyInstaller spec для сборки в exe

import sys
import os
from pathlib import Path

block_cipher = None

# Путь к проекту (текущая директория откуда запущен pyinstaller)
project_root = Path(os.getcwd())

# Основной скрипт входа
a = Analysis(
    [str(project_root / 'soulight' / 'app.py')],
    pathex=[str(project_root)],
    binaries=[
        # .NET DLL для основного bridge
        (str(project_root / 'beelightLib.dll'), '.'),
        # Быстрый C# bridge (добавлен второй нейросетью)
        (str(project_root / 'dotnet' / 'SoulightBridge.dll'), 'dotnet'),
    ],
    datas=[
        # JSON конфиги
        (str(project_root / 'led_config.json'), '.'),
        (str(project_root / 'soulight' / 'color_preset.json'), 'soulight'),
        # Пакет soulight целиком
        (str(project_root / 'soulight'), 'soulight'),
    ],
    hiddenimports=[
        'clr',  # pythonnet
        'PyQt6.QtCore',
        'PyQt6.QtGui',
        'PyQt6.QtWidgets',
        'serial',
        'serial.tools.list_ports',
        'mss',
        'numpy',
        # Все модули soulight
        'soulight.protocol.bridge',
        'soulight.protocol.serial_driver',
        'soulight.ui.main_window',
        'soulight.led_config',
        'soulight.color_preset',
        'soulight.screen_mirroring.engine',
        'soulight.screen_mirroring.sampler',
        'soulight.screen_mirroring.screen_capture',
        'soulight.screen_mirroring.worker',
        # Scenes: паттерн-режимы (Rainbow, Fire, Aurora...)
        'soulight.scenes.engine',
        'soulight.scenes.patterns',
        # Audio: FFT аудио-режимы
        'soulight.audio.engine',
        'soulight.audio.modes',
        # soundcard: захват аудио (микрофон и WASAPI loopback)
        'soundcard',
        'soundcard.mediafoundation',
    ],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[
        'matplotlib',
        'tkinter',
        'PIL',
        'scipy',
        'pandas',
        'pytest',
        'unittest',
    ],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    cipher=block_cipher,
    noarchive=False,
)

pyz = PYZ(a.pure, a.zipped_data, cipher=block_cipher)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.zipfiles,
    a.datas,
    [],
    name='Soulight',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=False,  # GUI приложение — без консоли
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
    icon=None,  # Можно добавить иконку если есть
)
