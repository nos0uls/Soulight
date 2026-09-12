#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Порядок: локальный .venv → venv → user-venv (~/.venvs/soulight,
# нужен когда репо лежит на NTFS и symlink-venv невозможен) → системный.
if [ -f "$SCRIPT_DIR/.venv/bin/python" ]; then
    PYTHON_BIN="$SCRIPT_DIR/.venv/bin/python"
elif [ -f "$SCRIPT_DIR/venv/bin/python" ]; then
    PYTHON_BIN="$SCRIPT_DIR/venv/bin/python"
elif [ -f "$HOME/.venvs/soulight/bin/python" ]; then
    PYTHON_BIN="$HOME/.venvs/soulight/bin/python"
elif command -v python3 >/dev/null 2>&1; then
    PYTHON_BIN="python3"
else
    PYTHON_BIN="python"
fi

if [ -t 1 ]; then
    echo "=== Soulight: Запуск на Linux ==="
    echo "Примечание: Soulight использует протокол Beelight."
    echo "Если контроллер подключен по USB-Serial, убедитесь, что у вас есть права на доступ к порту:"
    echo "  sudo usermod -a -G dialout \$USER"
    echo ""
fi

exec "$PYTHON_BIN" -m soulight "$@"
