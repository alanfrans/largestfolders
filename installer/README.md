# Largest Folders Installer

This directory contains the installer scripts for the Largest Folders application.

## Prerequisites

To build the installer, you need:

1. **Windows 10/11** - The installer must be built on Windows
2. **.NET 8.0 SDK** - Download from https://dotnet.microsoft.com/download/dotnet/8.0
3. **Inno Setup 6** - Download from https://jrsoftware.org/isdl.php

## Building the Installer

### Option 1: Using the build script

1. Open Command Prompt or PowerShell
2. Navigate to this directory
3. Run `build.bat`

The installer will be created at `output\LargestFoldersSetup.exe`

### Option 2: Manual build

1. Build the application in Release mode:
   ```cmd
   cd ..
   dotnet publish src\LargestFolders\LargestFolders.csproj -c Release -r win-x64 --self-contained true -o src\LargestFolders\bin\Release\net8.0-windows\publish
   ```

2. Open `LargestFolders.iss` in Inno Setup Compiler
3. Click Build > Compile (or press Ctrl+F9)

## Installer Features

The installer provides:

- Installation to Program Files (user choice)
- Desktop shortcut (optional)
- Start Menu shortcuts
- Run at Windows startup (optional)
- Proper uninstaller with cleanup

## Installation Requirements

The installed application requires:
- Windows 10 or later (Windows 11 supported)
- .NET 8.0 Desktop Runtime (included in self-contained deployment)

## Files

- `LargestFolders.iss` - Inno Setup script
- `build.bat` - Windows build script
- `output/` - Output directory for built installer (created during build)
