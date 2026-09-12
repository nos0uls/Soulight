# native_bridge.py — Чисто-Python замена BeelightBridge.
#
# Тот же интерфейс, что у BeelightBridge (protocol/bridge.py), но пакеты
# генерируются нативно через lightprotocol.py — без Beelight.exe, .NET,
# pythonnet и Wine. Работает на Windows и Linux одинаково.
#
# Отличия от BeelightBridge по семантике:
# - make_color_packet() эмулирует solid color через per-LED RGB transfer
#   (все 75 LED одним цветом) — это проверенный путь, тот же что использует
#   screen mirroring.
# - make_bright_packet(dimmer) шлёт настоящую hardware-команду яркости
#   (ctrl=2, dimmer 0..1000) — ту же, что GenBrightPackage в Beelight.exe.
#   Яркость НЕ масштабируется программно: контроллер сам умножает dimmer
#   на RGB, поэтому яркость едина для solid/per-LED/scene/audio режимов.

from soulight.protocol import lightprotocol as lp


class NativeBridge:
    """Drop-in замена BeelightBridge с нативной генерацией пакетов."""

    def __init__(self):
        self._ready = False
        self._heartbeat_pkt = None

    def init(self):
        """Инициализация — нативному протоколу ничего не нужно."""
        self._heartbeat_pkt = lp.heartbeat()
        self._ready = True
        return True

    @property
    def ready(self):
        return self._ready

    def handshake_packets(self):
        """
        Дополнительные пакеты перед стандартным handshake драйвера.
        Повторяют стартовую последовательность оригинального приложения.
        """
        return lp.handshake_sequence()

    def make_color_packet(self, r, g, b):
        """Solid color через per-LED transfer (все LED одним цветом)."""
        if not self._ready:
            return None
        rgb = (int(r), int(g), int(b))
        return lp.rgb_transfer([rgb] * lp.NUM_LEDS)

    def make_bright_packet(self, dimmer):
        """
        Аппаратная яркость контроллера, dimmer в hardware-единицах 0..1000.
        (UI 0..255 конвертирует драйвер — так же, как у BeelightBridge.)
        """
        if not self._ready:
            return None
        return lp.brightness(dimmer)

    def make_switch_packet(self, on):
        """Switch ON/OFF. OFF-вариант — кандидат, проверить на железе."""
        if not self._ready:
            return None
        return lp.switch(bool(on))

    def make_workmode_pc_packet(self):
        """Workmode PC (mode=0) — кандидат, проверить на железе."""
        if not self._ready:
            return None
        return lp.set_work_mode(0)

    def make_rgb_transfer_packet(self, colors_rgb):
        """
        Per-LED пакет. Семантика как у BeelightBridge: список в логическом
        порядке, нормализация до 75 и реверс — внутри lp.rgb_transfer.
        """
        if not self._ready:
            return None
        return lp.rgb_transfer(colors_rgb)

    def get_heartbeat(self):
        """Heartbeat-пакет (предгенерированный при init)."""
        return self._heartbeat_pkt
