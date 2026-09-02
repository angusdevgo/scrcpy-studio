@echo off
setlocal
set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set ICON=..\app\data\icon.ico

if not exist %CSC% (
    echo csc.exe not found at %CSC%
    pause
    exit /b 1
)

if not exist %ICON% (
    echo icon.ico not found at %ICON%
    pause
    exit /b 1
)

echo Compiling ScrcpyGUI with scrcpy icon...
%CSC% /target:winexe /win32icon:%ICON% /out:ScrcpyGUI.exe ScrcpyGUI.cs
if %ERRORLEVEL% equ 0 (
    copy /Y %ICON% icon.ico >nul
    echo Compilation successful! ScrcpyGUI.exe has been generated.
) else (
    echo Compilation failed.
)
pause
endlocal
