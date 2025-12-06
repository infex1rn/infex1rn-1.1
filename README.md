# infex1rn v1.1 - iOS Device Management & iCloud Bypass Tool

![Version](https://img.shields.io/badge/version-1.1-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![.NET](https://img.shields.io/badge/.NET-6.0-purple)
![License](https://img.shields.io/badge/license-Educational-orange)

A powerful Windows desktop application for iOS device management and iCloud activation lock bypass. Built with C# and WPF, infex1rn provides advanced tools for iOS forensics, device recovery, and research.

## ⚠️ Legal Notice

**This tool is for educational, research, and authorized device recovery purposes ONLY.**

- Only use on devices you own or have explicit permission to access
- Bypassing activation locks may violate Apple's Terms of Service
- Use responsibly and ethically
- The developers are not responsible for misuse

## ✨ What's New in v1.1

### 🎯 Major Improvements

1. **Fixed IPSW Loading Crashes**
   - Robust error handling prevents crashes with malformed IPSW files
   - Clear error messages guide users when issues occur
   - Graceful degradation when BuildManifest.plist is missing

2. **Drag-and-Drop Support** 🆕
   - Simply drag IPSW files into the CFW Studio tab
   - No more navigating through file dialogs
   - Faster, more intuitive workflow

3. **Fixed Extract Ramdisk**
   - Reliable extraction with comprehensive error checking
   - Auto-creates directories as needed
   - Opens Windows Explorer to show extracted files
   - Clear success/failure feedback

4. **Modern UI with Animations** 🎨
   - Professional dark theme (#1E1E1E)
   - Smooth fade-in animations
   - Custom styled buttons with hover effects
   - Emoji icons for quick recognition
   - Larger window (1000x600) for better usability
   - Status bar with version info and warnings

See [NEW_FEATURES_v1.1.md](docs/NEW_FEATURES_v1.1.md) for detailed information.

## 🚀 Features

### Device Management
- **Device Detection**: Automatic detection of connected iOS devices via USB
- **Device Information**: View detailed device specs (model, iOS version, serial number, etc.)
- **Recovery Mode**: Enter/exit recovery mode with one click
- **App Management**: Install/manage IPA files
- **File Browser**: Access app sandboxes with file sharing enabled

### iCloud Bypass Tools
- **Tethered Bypass**: Traditional activation lock bypass using ramdisk
- **Untethered Bypass**: Permanent bypass for A7-A11 devices (iPhone 5s - iPhone X)
- **A10 Hello Bypass**: Specialized bypass for iPhone 7/7+ using Signals method
- **Auto Ramdisk Extraction**: Automatically extract and patch ramdisks from IPSW

### Firmware Tools (CFW Studio)
- **IPSW Explorer**: Browse and extract contents of iOS firmware files
- **Drag-and-Drop**: Drop IPSW files directly into the interface
- **Ramdisk Extraction**: Extract ramdisk from IPSW for bypass operations
- **Component Extraction**: Extract individual firmware components

### Advanced Tools
- **checkm8 Exploit**: Boot ramdisk using checkm8 vulnerability
- **Purple Mode**: Enter purple restore mode
- **PwnDFU**: Enter pwned DFU mode for exploit execution
- **Ramdisk Studio**: Edit and customize ramdisk images

## 🖥️ System Requirements

- **Operating System**: Windows 7 SP1 or later (Windows 10/11 recommended)
- **.NET Runtime**: .NET 6.0 Desktop Runtime
- **USB Drivers**: iTunes drivers (Apple Mobile Device Support)
- **Hardware**: Any modern PC with USB ports

## 📦 Installation

### Prerequisites

1. Install [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/6.0)
2. Install iTunes (for device drivers) - you can uninstall iTunes after installation, just keep the drivers

### Running infex1rn

1. Download the latest release from the [Releases](../../releases) page
2. Extract the ZIP file to a folder
3. Run `infex1rn.exe`
4. Connect your iOS device via USB

## 📖 Quick Start Guide

### Basic Device Management

1. **Connect Device**: Plug in your iOS device via USB
2. **List Devices**: Click "🔄 List Devices" in the left panel
3. **Select Device**: Click on your device in the list
4. **View Info**: Check the "📱 Device Info" tab for device details

### Loading an IPSW (Two Methods)

**Method 1: File Dialog**
1. Go to "🔧 CFW Studio" tab
2. Click "📁 Load IPSW"
3. Select your IPSW file

**Method 2: Drag and Drop** (NEW!)
1. Go to "🔧 CFW Studio" tab
2. Drag your IPSW file from Windows Explorer
3. Drop it onto the TreeView
4. IPSW loads automatically!

### Extracting Ramdisk

1. Load an IPSW (using either method above)
2. If a ramdisk is found, the "💾 Extract Ramdisk" button will be enabled
3. Click the button
4. Wait for the success message
5. Windows Explorer opens showing the extracted ramdisk

### Performing an Untethered Bypass

1. Download the IPSW for your device model and iOS version
2. Click "🎯 Auto Extract Ramdisk" in System Utilities
3. Select your IPSW file
4. Wait for extraction to complete
5. Put your device in DFU mode
6. Click "✅ Untethered Bypass"
7. Follow the on-screen instructions

## 🎨 UI Features

### Modern Dark Theme
- Professional appearance with #1E1E1E background
- High contrast for readability
- Blue accents (#0078D7) for important actions
- Consistent design language throughout

### Smooth Animations
- Fade-in effects when switching tabs
- Button hover transitions
- Modern, polished feel

### Visual Icons
All tabs and buttons feature emoji icons:
- 📱 Device information
- 📦 Apps and packages
- 📁 Files and folders
- 🛠️ System tools
- 🔧 Firmware studio
- 💿 Ramdisk tools

See [UI_IMPROVEMENTS.md](docs/UI_IMPROVEMENTS.md) for complete UI documentation.

## 🏗️ Architecture

infex1rn is built using the MVVM (Model-View-ViewModel) pattern:

```
infex1rn/
├── ViewModels/          # Application logic and data binding
│   ├── MainViewModel.cs
│   └── ViewModelBase.cs
├── Services/            # Business logic and device communication
│   ├── DeviceService.cs
│   ├── AppService.cs
│   ├── FirmwareService.cs
│   └── RamdiskService.cs
├── tools/               # External utilities
│   ├── libimobiledevice/
│   ├── gaster/
│   └── *.bat scripts
├── MainWindow.xaml      # UI definition
└── MainWindow.xaml.cs   # UI code-behind
```

## 🔧 Technology Stack

- **Language**: C# 10
- **Framework**: .NET 6.0
- **UI**: WPF (Windows Presentation Foundation)
- **iOS Communication**: imobiledevice-net (v1.3.17)
- **External Tools**: libimobiledevice, gaster (checkm8)

## 🤝 Contributing

Contributions are welcome! Please read our contributing guidelines before submitting PRs.

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 Documentation

- [New Features in v1.1](docs/NEW_FEATURES_v1.1.md) - Detailed changelog
- [UI Improvements Guide](docs/UI_IMPROVEMENTS.md) - UI documentation
- [Untethered Bypass Guide](docs/UNTETHERED_BYPASS.md) - Bypass instructions
- [Ramdisk README](ramdisks/README.md) - Ramdisk file information
- [IPSW README](ipsw/README.md) - Firmware file information

## 🐛 Known Issues

- This is a Windows-only application (WPF requirement)
- checkm8-based features only work on A7-A11 devices (iPhone 5s through iPhone X)
- Some antivirus software may flag the checkm8 tools as suspicious (false positive)

## 🆘 Troubleshooting

### Device Not Detected
- Ensure iTunes drivers are installed
- Try a different USB cable/port
- Trust the computer on your iOS device

### IPSW Won't Load
- Verify the file is a valid .ipsw file
- Check if the file is corrupted (try re-downloading)
- Make sure you have enough disk space

### Bypass Not Working
- Verify device is in correct mode (DFU for untethered bypass)
- Check device compatibility (A7-A11 only for checkm8)
- Try different USB ports
- Restart the application

## 📞 Support

- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)
- **Documentation**: [docs/](docs/) folder

## ⚖️ License

This project is for educational purposes only. See LICENSE file for details.

## 🙏 Credits

- **libimobiledevice** - iOS device communication
- **checkm8** - bootrom exploit
- **imobiledevice-net** - .NET wrapper for libimobiledevice

## 🔐 Security Note

This tool interacts with iOS devices at a low level. Always:
- Use on your own devices or with explicit permission
- Understand what each operation does
- Keep backups of important data
- Be aware of the legal implications in your jurisdiction

---

**Version**: 1.1  
**Last Updated**: 2024  
**Platform**: Windows (.NET 6)  
**Status**: Active Development

**⚠️ Use Responsibly - Educational & Authorized Use Only ⚠️**
