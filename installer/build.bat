@echo off
REM Build script for Largest Folders Installer
REM This script must be run on Windows with .NET 8 SDK and Inno Setup installed

echo Building Largest Folders Application...
echo.

REM Navigate to the solution directory
cd /d "%~dp0.."

REM Clean and build in Release mode with self-contained deployment
echo Step 1: Building application in Release mode...
dotnet publish src\LargestFolders\LargestFolders.csproj -c Release -r win-x64 --self-contained true -o src\LargestFolders\bin\Release\net8.0-windows\publish
if %ERRORLEVEL% neq 0 (
    echo Error: Build failed!
    pause
    exit /b 1
)

echo.
echo Step 2: Creating installer...

REM Check if Inno Setup is installed
set INNO_PATH=
if exist "%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" (
    set "INNO_PATH=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
) else if exist "%ProgramFiles%\Inno Setup 6\ISCC.exe" (
    set "INNO_PATH=%ProgramFiles%\Inno Setup 6\ISCC.exe"
)

if not defined INNO_PATH (
    echo Error: Inno Setup 6 not found!
    echo Please install Inno Setup 6 from https://jrsoftware.org/isdl.php
    pause
    exit /b 1
)

REM Create installer
"%INNO_PATH%" installer\LargestFolders.iss
if %ERRORLEVEL% neq 0 (
    echo Error: Installer creation failed!
    pause
    exit /b 1
)

echo.
echo Build completed successfully!
echo Installer location: installer\output\LargestFoldersSetup.exe
echo.
pause
