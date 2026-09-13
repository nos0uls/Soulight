#!/usr/bin/env bash
# Устанавливает .desktop-ярлык Soulight в меню приложений.
# Иконка копируется в hicolor, путь к репо подставляется автоматически.
#
# Дополнительно настраивает Wayland/KDE: KWin разрешает ScreenShot2
# только процессам, у которых есть desktop-файл, чей Exec совпадает
# с /proc/<pid>/exe и содержит X-KDE-DBUS-Restricted-Interfaces.
# Скрипт интерпретируется python'ом, поэтому создаётся "shim" —
# реальная копия бинаря интерпретатора в venv (python-soulight) —
# и скрытый desktop-файл с её путём. Тогда авторизацию на захват
# экрана получает только этот бинарь, а не любой python-процесс.
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
APPS_DIR="${XDG_DATA_HOME:-$HOME/.local/share}/applications"
ICON_DIR="${XDG_DATA_HOME:-$HOME/.local/share}/icons/hicolor/256x256/apps"

mkdir -p "$APPS_DIR" "$ICON_DIR"
cp "$SCRIPT_DIR/soulight/assets/icon.png" "$ICON_DIR/soulight.png"

cat > "$APPS_DIR/soulight.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=Soulight
GenericName=LED Controller
Comment=LED strip controller: static color, screen mirroring, scenes, audio
Exec="$SCRIPT_DIR/run_soulight.sh"
Path=$SCRIPT_DIR
Icon=soulight
Terminal=false
Categories=Settings;HardwareSettings;
Keywords=led;ambient;backlight;soulight;beelight;lytmi;
StartupWMClass=Soulight
X-KDE-DBUS-Restricted-Interfaces=org.kde.KWin.ScreenShot2
EOF

# --- Wayland/KDE shim: копия интерпретатора под узкий grant ---
SHIM="$HOME/.venvs/soulight/bin/python-soulight"
VENV_PY=""
for cand in "$SCRIPT_DIR/.venv/bin/python" "$SCRIPT_DIR/venv/bin/python" \
            "$HOME/.venvs/soulight/bin/python"; do
    if [ -f "$cand" ]; then VENV_PY="$cand"; break; fi
done

if [ -n "$VENV_PY" ]; then
    REAL_PY="$(readlink -f "$VENV_PY")"
    SHIM_DIR="$(dirname "$VENV_PY")"
    SHIM="$SHIM_DIR/python-soulight"
    cp "$REAL_PY" "$SHIM"
    chmod +x "$SHIM"

    cat > "$APPS_DIR/soulight-python-shim.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=Soulight Runtime
Exec=$SHIM
Icon=soulight
NoDisplay=true
X-KDE-DBUS-Restricted-Interfaces=org.kde.KWin.ScreenShot2
EOF
    echo "Installed: $APPS_DIR/soulight-python-shim.desktop (KWin screen-capture grant → $SHIM)"
else
    echo "Предупреждение: venv не найден — shim для Wayland screen capture не создан"
fi

# --- udev-правило для доступа к контроллеру без root ---
# Порт создаётся как root:dialout 0660; без правила обычный пользователь
# получает EACCES. uaccess-ACL выдаёт доступ активной сессии без
# добавления в группу и без relogin.
RULE_SRC="$SCRIPT_DIR/71-soulight-serial.rules"
RULE_DST="/etc/udev/rules.d/71-soulight-serial.rules"
if [ -f "$RULE_SRC" ]; then
    if ! cmp -s "$RULE_SRC" "$RULE_DST" 2>/dev/null; then
        if [ -w /etc/udev/rules.d ]; then
            cp "$RULE_SRC" "$RULE_DST"
        elif command -v sudo >/dev/null && sudo -n true 2>/dev/null; then
            sudo cp "$RULE_SRC" "$RULE_DST"
        else
            echo "udev-правило не установлено (нет root). Выполни вручную:"
            echo "  sudo cp '$RULE_SRC' $RULE_DST"
            echo "  sudo udevadm control --reload && sudo udevadm trigger"
        fi
    fi
    if [ -f "$RULE_DST" ]; then
        udevadm control --reload 2>/dev/null || sudo -n udevadm control --reload 2>/dev/null || true
        udevadm trigger --subsystem-match=tty --attr-match=idVendor=2e3c 2>/dev/null \
            || sudo -n udevadm trigger --subsystem-match=tty 2>/dev/null || true
        echo "udev rule: $RULE_DST"
    fi
fi

desktop-file-validate "$APPS_DIR/soulight.desktop" 2>/dev/null || true
gtk-update-icon-cache -q "${XDG_DATA_HOME:-$HOME/.local/share}/icons/hicolor" 2>/dev/null || true
update-desktop-database "$APPS_DIR" 2>/dev/null || true
echo "Installed: $APPS_DIR/soulight.desktop"
echo "Примечание: ярлык ищет интерпретатор в .venv/ venv/ ~/.venvs/soulight/ (см. run_soulight.sh)"
