#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

if [ -f "$SCRIPT_DIR/.venv/bin/python" ]; then
    PYTHON_BIN="$SCRIPT_DIR/.venv/bin/python"
elif [ -f "$SCRIPT_DIR/venv/bin/python" ]; then
    PYTHON_BIN="$SCRIPT_DIR/venv/bin/python"
elif command -v python3 >/dev/null 2>&1; then
    PYTHON_BIN="python3"
else
    PYTHON_BIN="python"
fi

echo "=== Soulight: Запуск на Linux ==="
echo "Примечание: Soulight использует протокол Beelight."
echo "Если контроллер подключен по USB-Serial, убедитесь, что у вас есть права на доступ к порту:"
echo "  sudo usermod -a -G dialout \$USER"
echo ""

exec "$PYTHON_BIN" -m soulight "$@"
