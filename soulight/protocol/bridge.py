# bridge.py — Мост между Python и Beelight.exe через pythonnet.
#
# Загружает .NET assembly Beelight.exe, находит методы LightProtocol
# через reflection, и предоставляет Python-обёртки для генерации
# wire-format пакетов (зашифрованных, готовых к отправке в serial).
#
# CryptoObfuscator (обфускатор Beelight.exe) расшифровывает тела методов
# автоматически при загрузке assembly — поэтому reflection работает.

import os
import sys
import clr  # pythonnet — мост Python ↔ .NET CLR

# Путь к директории Beelight (содержит Beelight.exe и все DLL зависимости)
BEELIGHT_DIR = r"C:\Program Files (x86)\Beelight\Beelight V3.0"
BEELIGHT_EXE = os.path.join(BEELIGHT_DIR, "Beelight.exe")


class BeelightBridge:
    """
    Мост к LightProtocol методам внутри Beelight.exe.

    Загружает .NET assembly через pythonnet, находит нужные типы и методы
    через System.Reflection, и предоставляет простые Python-обёртки.

    Каждый вызов make_color_packet() / make_bright_packet() генерирует
    пакет с уникальным nonce (шифрование внутри .NET кода).
    """

    def __init__(self):
        # Флаг успешной инициализации
        self._ready = False
        # .NET reflection объекты (MethodInfo) для вызова LP методов
        self._gen_color = None
        self._gen_bright = None
        self._gen_switch = None
        self._gen_work_mode = None
        self._gen_frame = None
        # .NET enum типы
        self._wk_mode_type = None
        self._cmd_type = None
        self._attr_type = None
        # Предгенерированный heartbeat (не зависит от параметров)
        self._heartbeat_pkt = None

    def init(self):
        """
        Загружает Beelight.exe и находит все нужные методы.
        Вызывать один раз при старте приложения.
        Возвращает True при успехе, False при ошибке.
        """
        if self._ready:
            return True

        try:
            return self._load_assembly()
        except Exception as e:
            print(f"[Bridge] Ошибка инициализации: {e}")
            return False

    def _load_assembly(self):
        """
        Внутренний метод: загрузка assembly и поиск методов через reflection.
        """
        # Импортируем .NET типы через pythonnet
        from System.Reflection import Assembly, BindingFlags
        from System.IO import Path as NetPath
        from System import AppDomain, ResolveEventHandler, Enum, Type

        # Проверяем наличие файла
        if not os.path.exists(BEELIGHT_EXE):
            print(f"[Bridge] Beelight.exe не найден: {BEELIGHT_EXE}")
            return False

        # Регистрируем обработчик зависимостей — при загрузке Beelight.exe
        # .NET будет искать DLL в той же директории
        def resolve_handler(sender, args):
            name = args.Name.split(",")[0]
            path = os.path.join(BEELIGHT_DIR, name + ".dll")
            if os.path.exists(path):
                return Assembly.LoadFrom(path)
            return None

        AppDomain.CurrentDomain.AssemblyResolve += resolve_handler

        # Загружаем assembly (CryptoObfuscator расшифрует method bodies)
        asm = Assembly.LoadFrom(BEELIGHT_EXE)
        all_types = asm.GetTypes()

        # Флаги для поиска public + private + static методов
        flags = (BindingFlags.Public | BindingFlags.NonPublic
                 | BindingFlags.Static | BindingFlags.Instance)

        # Ищем нужные типы в assembly
        lp_ctrl = None   # LProtocolCtrl — генерация control пакетов
        lp_base = None   # LProtocolBase — генерация frame пакетов

        for t in all_types:
            name = t.Name
            if name == "LProtocolCtrl":
                lp_ctrl = t
            elif name == "LProtocolBase":
                lp_base = t
            elif name == "LP_WK_MODE":
                self._wk_mode_type = t
            elif name == "LP_CMD":
                self._cmd_type = t
            elif name == "LP_ATTR":
                self._attr_type = t

        if lp_ctrl is None:
            print("[Bridge] LProtocolCtrl не найден в assembly")
            return False

        # Получаем MethodInfo через GetMethods() + фильтр по имени и числу параметров.
        # GetMethod() бросает исключение при неоднозначных перегрузках,
        # поэтому перебираем вручную.
        for m in lp_ctrl.GetMethods(flags):
            name = m.Name
            n_params = len(m.GetParameters())
            if name == "GenColorPackage" and n_params == 2:
                self._gen_color = m
            elif name == "GenBrightPackage" and n_params == 2:
                self._gen_bright = m
            elif name == "GenSwitchPackage" and n_params == 2:
                self._gen_switch = m
            elif name == "GenWorkModePackage" and n_params == 2:
                self._gen_work_mode = m
            elif name == "GenRGBTransferPackage":
                self._gen_rgb_transfer = m

        if lp_base is not None:
            for m in lp_base.GetMethods(flags):
                if m.Name == "GenFramePackage":
                    self._gen_frame = m
                    break

        if self._gen_color is None:
            print("[Bridge] GenColorPackage не найден")
            return False

        # Предгенерируем heartbeat пакет
        if (self._gen_frame is not None
                and self._attr_type is not None
                and self._cmd_type is not None):
            try:
                from System import Array, Byte
                attr_req = Enum.ToObject(self._attr_type, 0)  # LP_ATTR_REQ
                cmd_hb = Enum.ToObject(self._cmd_type, 0)     # LP_CMD_HEARTBEAT
                empty = Array.CreateInstance(Byte, 0)
                result = self._gen_frame.Invoke(None, [attr_req, cmd_hb, empty])
                if result is not None:
                    self._heartbeat_pkt = bytes(result)
            except Exception as e:
                print(f"[Bridge] Heartbeat generation failed: {e}")

        self._ready = True
        print("[Bridge] Инициализация OK")
        return True

    @property
    def ready(self):
        """Готов ли bridge к генерации пакетов."""
        return self._ready

    def make_color_packet(self, r, g, b):
        """
        Генерирует wire-format пакет цвета (RGB 0-255).
        Каждый вызов создаёт пакет с уникальным nonce.
        Возвращает bytes или None при ошибке.
        """
        if not self._ready or self._gen_color is None:
            return None
        try:
            from System.Drawing import Color
            color = Color.FromArgb(int(r), int(g), int(b))
            result = self._gen_color.Invoke(None, [color, Byte(0)])
            return bytes(result) if result is not None else None
        except Exception as e:
            print(f"[Bridge] make_color_packet error: {e}")
            return None

    def make_bright_packet(self, dimmer):
        """
        Генерирует wire-format пакет яркости (0-255).
        Возвращает bytes или None при ошибке.
        """
        if not self._ready or self._gen_bright is None:
            return None
        try:
            from System import Int32
            result = self._gen_bright.Invoke(None, [Int32(int(dimmer)), Byte(0)])
            return bytes(result) if result is not None else None
        except Exception as e:
            print(f"[Bridge] make_bright_packet error: {e}")
            return None

    def make_switch_packet(self, on):
        """
        Генерирует wire-format пакет включения (True) или выключения (False).
        Возвращает bytes или None при ошибке.
        """
        if not self._ready or self._gen_switch is None:
            return None
        try:
            from System import Boolean
            result = self._gen_switch.Invoke(None, [Boolean(bool(on)), Byte(0)])
            return bytes(result) if result is not None else None
        except Exception as e:
            print(f"[Bridge] make_switch_packet error: {e}")
            return None

    def make_workmode_pc_packet(self):
        """
        Генерирует wire-format пакет переключения в PC mode.
        Возвращает bytes или None при ошибке.
        """
        if not self._ready or self._gen_work_mode is None or self._wk_mode_type is None:
            return None
        try:
            from System import Enum as SysEnum
            pc_mode = SysEnum.ToObject(self._wk_mode_type, 0)  # LP_WK_MODE_PC
            result = self._gen_work_mode.Invoke(None, [pc_mode, Byte(0)])
            return bytes(result) if result is not None else None
        except Exception as e:
            print(f"[Bridge] make_workmode_pc_packet error: {e}")
            return None

    def get_heartbeat(self):
        """
        Возвращает предгенерированный heartbeat пакет (bytes).
        Heartbeat одинаковый каждый раз, поэтому кешируется.
        """
        return self._heartbeat_pkt


# Вспомогательная функция для импорта .NET Byte (используется в Invoke)
def Byte(value):
    """Конвертирует Python int в .NET System.Byte для reflection вызовов."""
    from System import Byte as NetByte
    return NetByte(int(value))
