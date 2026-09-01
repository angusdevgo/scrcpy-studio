@echo off
set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist %CSC% (
    echo csc.exe not found at %CSC%
    pause
    exit /b 1
)

echo Compiling ScrcpyGUI...
%CSC% /target:winexe /out:ScrcpyGUI.exe ScrcpyGUI.cs
if %ERRORLEVEL% equ 0 (
    echo Compilation successful! ScrcpyGUI.exe has been generated.
) else (
    echo Compilation failed.
)
pause
