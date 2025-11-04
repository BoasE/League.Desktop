@echo off
echo ========================================
echo  Auto Accept - Build Script
echo ========================================
echo.

REM Wechsle zum Projektverzeichnis
cd /d "%~dp0"

echo [1/3] Cleaning old builds...
if exist "bin\Release\net9.0\win-x64\publish" (
    rmdir /s /q "bin\Release\net9.0\win-x64\publish"
)

echo.
echo [2/3] Publishing application...
echo Target: Windows x64
echo Mode: Self-Contained + Trimmed
echo.

dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed!
    pause
    exit /b 1
)

echo.
echo [3/3] Build complete!
echo.
echo Output: bin\Release\net9.0\win-x64\publish\BE.League.Desktop.AutoAccept.exe
echo.

REM Zeige Dateigröße
for %%A in ("bin\Release\net9.0\win-x64\publish\BE.League.Desktop.AutoAccept.exe") do (
    set size=%%~zA
)
set /a sizeMB=%size%/1024/1024
echo File size: ~%sizeMB% MB
echo.

REM Frage ob die Datei geöffnet werden soll
echo.
set /p open="Open output folder? (Y/N): "
if /i "%open%"=="Y" (
    explorer "bin\Release\net9.0\win-x64\publish"
)

echo.
echo Done!
pause

