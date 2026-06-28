@echo off
setlocal

:: Переходим в папку, где лежит этот батник
:: (важно для относительных импортов и запуска python -m soulight)
cd /d "%~dp0"

:: pythonw / pyw — это windowed-версия Python, она не открывает консольное окно.
:: Сначала пробуем pythonw из PATH (обычно это тот Python, в который установлены
:: зависимости проекта). Если его нет — используем Python Launcher с явным
:: выбором Python 3.12, затем fallback на системный pyw.
where pythonw >nul 2>nul
if %errorlevel% == 0 (
    pythonw -m soulight
) else (
    where pyw >nul 2>nul
    if %errorlevel% == 0 (
        pyw -3.12 -m soulight
    ) else (
        start "" python -m soulight
    )
)

endlocal
