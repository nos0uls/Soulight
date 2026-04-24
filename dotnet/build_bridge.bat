@echo off
set CSC="C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
echo Компилируем SoulightBridge.dll...
%CSC% /target:library /out:SoulightBridge.dll SoulightBridge.cs
if %ERRORLEVEL% equ 0 (
    echo Сборка успешна: SoulightBridge.dll
) else (
    echo Ошибка сборки.
)
