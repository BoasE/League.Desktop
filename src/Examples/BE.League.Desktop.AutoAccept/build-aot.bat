@echo off
echo ========================================
echo  Auto Accept - Native AOT Build
echo ========================================
echo.
echo Native AOT creates the smallest and fastest executable
echo but requires longer compilation time.
echo.

REM Wechsle zum Projektverzeichnis
cd /d "%~dp0"

echo [1/4] Cleaning old builds...
if exist "bin\Release\net9.0\win-x64\publish" (
    rmdir /s /q "bin\Release\net9.0\win-x64\publish"
)

echo.
echo [2/4] Checking prerequisites...
echo.
echo Checking for Native AOT prerequisites...
echo - Desktop development with C++ workload
echo - MSVC v143 or later
echo.

REM Check if cl.exe exists (Visual Studio C++ compiler)
where cl.exe >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [WARNING] Visual Studio C++ compiler not found in PATH!
    echo.
    echo Native AOT requires Visual Studio with C++ workload.
    echo.
    echo Please install:
    echo 1. Visual Studio 2022 or later
    echo 2. Desktop development with C++ workload
    echo 3. MSVC v143 - VS 2022 C++ x64/x86 build tools
    echo.
    echo After installation, run this from "Developer Command Prompt for VS 2022"
    echo.
    pause
    exit /b 1
)

echo ✓ C++ compiler found
echo.

echo [3/4] Publishing with Native AOT...
echo Target: Windows x64
echo Mode: Native AOT (Ahead-of-Time Compilation)
echo.
echo This may take 2-5 minutes...
echo.

dotnet publish -c Release -r win-x64 /p:PublishAot=true

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Native AOT build failed!
    echo.
    echo Common issues:
    echo - Not running from Developer Command Prompt
    echo - Missing C++ build tools
    echo - Code not AOT-compatible (reflection, dynamic code)
    echo.
    pause
    exit /b 1
)

echo.
echo [4/4] Build complete!
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
echo Native AOT Benefits:
echo ✓ Smallest file size (typically 8-12 MB)
echo ✓ Fastest startup time (no JIT compilation)
echo ✓ Lower memory usage
echo ✓ No .NET runtime required
echo.

REM Frage ob die Datei geöffnet werden soll
set /p open="Open output folder? (Y/N): "
if /i "%open%"=="Y" (
    explorer "bin\Release\net9.0\win-x64\publish"
)

echo.
echo Done!
pause

