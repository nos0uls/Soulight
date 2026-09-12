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
# - Яркость применяется программно (масштабирование RGB). Отдельной
#   hardware-команды яркости в captures не найдено; software-яркость
#   теряет точность в нижнем диапазоне, но работает везде.
# - make_switch_packet(False) и make_workmode_pc_packet() — кандидаты по
#   симметрии с захваченными пакетами; финальная проверка — на железе.

from soulight.protocol import lightprotocol as lp


class NativeBridge:
    """Drop-in замена BeelightBridge с нативной генерацией пакетов."""

    def __init__(self):
        self._ready = False
        # Программная яркость 0..1, применяется к RGB при сборке пакета.
        self._brightness = 1.0
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
        rgb = self._scale(int(r), int(g), int(b))
        return lp.rgb_transfer([rgb] * lp.NUM_LEDS)

    def make_bright_packet(self, dimmer):
        """
        Программная яркость: сохраняет значение, пакета не генерирует.
        Возвращает None — драйвер пропускает None в _safe_write.
        """
        self._brightness = max(0, min(255, int(dimmer))) / 255.0
        return None

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
        scaled = [self._scale(r, g, b) for r, g, b in colors_rgb]
        return lp.rgb_transfer(scaled)

    def get_heartbeat(self):
        """Heartbeat-пакет (предгенерированный при init)."""
        return self._heartbeat_pkt

    def _scale(self, r, g, b):
        k = self._brightness
        return (int(r * k), int(g * k), int(b * k))
