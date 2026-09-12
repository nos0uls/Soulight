#!/usr/bin/env bash
# Устанавливает .desktop-ярлык Soulight в меню приложений.
# Иконка копируется в hicolor, путь к репо подставляется автоматически.
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
EOF

desktop-file-validate "$APPS_DIR/soulight.desktop" 2>/dev/null || true
gtk-update-icon-cache -q "${XDG_DATA_HOME:-$HOME/.local/share}/icons/hicolor" 2>/dev/null || true
update-desktop-database "$APPS_DIR" 2>/dev/null || true
echo "Installed: $APPS_DIR/soulight.desktop"
echo "Примечание: ярлык ищет интерпретатор в .venv/ venv/ ~/.venvs/soulight/ (см. run_soulight.sh)"
