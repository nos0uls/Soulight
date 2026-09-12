#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Порядок: для каждого venv сначала ищем shim python-soulight
# (копия бинаря с KWin-грантом на захват экрана, см. install_desktop.sh),
# затем обычный python. Кандидаты: локальный .venv → venv →
# user-venv (~/.venvs/soulight, нужен когда репо на NTFS) → системный.
PYTHON_BIN=""
for venv_dir in "$SCRIPT_DIR/.venv" "$SCRIPT_DIR/venv" "$HOME/.venvs/soulight"; do
    if [ -f "$venv_dir/bin/python-soulight" ]; then
        PYTHON_BIN="$venv_dir/bin/python-soulight"
        break
    elif [ -f "$venv_dir/bin/python" ]; then
        PYTHON_BIN="$venv_dir/bin/python"
        break
    fi
done
if [ -z "$PYTHON_BIN" ]; then
    if command -v python3 >/dev/null 2>&1; then
        PYTHON_BIN="python3"
    else
        PYTHON_BIN="python"
    fi
fi

if [ -t 1 ]; then
    echo "=== Soulight: Запуск на Linux ==="
    echo "Примечание: Soulight использует протокол Beelight."
    echo "Если контроллер подключен по USB-Serial, убедитесь, что у вас есть права на доступ к порту:"
    echo "  sudo usermod -a -G dialout \$USER"
    echo ""
fi

exec "$PYTHON_BIN" -m soulight "$@"
