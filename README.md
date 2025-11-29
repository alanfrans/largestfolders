# Largest Folders

A Windows system tray application that displays the top 100 largest folders on the C: drive, ordered by size (largest to smallest).

## Features

- **System Tray Integration**: Runs silently in the background in the Windows system tray
- **One-Click Access**: Left-click the tray icon to view the largest folders
- **Context Menu**: Right-click for options including Refresh and Exit
- **Live Scanning**: Scans the C: drive and displays results with path and size
- **Background Operation**: Designed to run via Windows Task Scheduler at startup

## Requirements

- Windows 10 or later
- .NET 8.0 Runtime (Windows Desktop)

## Building from Source

```bash
cd src/LargestFolders
dotnet build
```

## Running the Application

### Manual Start
Run the built executable:
```bash
dotnet run --project src/LargestFolders
```

Or run the compiled executable:
```bash
src\LargestFolders\bin\Debug\net8.0-windows\LargestFolders.exe
```

### Setting Up Task Scheduler for Automatic Startup

1. Open Task Scheduler (taskschd.msc)
2. Click "Import Task..." from the Actions panel
3. Select the `LargestFolders.xml` file from the `src/LargestFolders` directory
4. Modify the executable path in the Action settings to match your installation location
5. Click OK to save

Alternatively, use the command line:
```cmd
schtasks /create /tn "LargestFolders" /xml "path\to\LargestFolders.xml"
```

## Usage

1. Once running, the application icon appears in the system tray
2. **Left-click** the tray icon to open the folder viewer
3. **Right-click** the tray icon for menu options:
   - **Show Largest Folders**: Opens the main window
   - **Refresh**: Re-scans the C: drive for folder sizes
   - **Exit**: Closes the application

## Screenshots

The main window displays:
- **Rank**: Position in the top 100 list
- **Path**: Full path to the folder
- **Size**: Human-readable size (B, KB, MB, GB, TB)

## Development

Built with:
- .NET 8.0
- Windows Forms
- C# 12

## License

This project is provided as-is for personal use.
