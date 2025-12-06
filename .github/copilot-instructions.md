# infex1rn GitHub Copilot Instructions

## Project Overview

**infex1rn** is a powerful Windows desktop application for iOS device management and iCloud activation lock bypass. It provides advanced tools for:
- iOS device diagnostics and information retrieval
- iCloud activation lock bypass (tethered and untethered)
- Ramdisk-based exploits using checkm8 vulnerability
- IPSW firmware file management and extraction
- iOS app management and file system access
- Custom firmware studio for advanced users

**Target Audience**: iOS forensics professionals, device recovery specialists, and researchers working with iOS devices (particularly A7-A11 chip devices: iPhone 5s through iPhone X).

**Important**: This tool is for educational, research, and authorized device recovery purposes only.

## Technology Stack

### Core Technologies
- **Language**: C# (.NET 6)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture Pattern**: MVVM (Model-View-ViewModel)
- **Target Platform**: Windows (.NET 6 Windows Desktop)

### Key Dependencies
- **imobiledevice-net** (v1.3.17): .NET wrapper for libimobiledevice
  - Used for iOS device communication via USB
  - Provides access to device lockdown, installation proxy, AFC, etc.
  - Critical for all device interaction features

### External Tools (in `/tools` directory)
- **libimobiledevice**: Suite of command-line tools for iOS device interaction
  - `ideviceactivation.exe`: Activation management
  - `ideviceinfo.exe`: Device information retrieval
  - `ideviceinstaller.exe`: App installation/management
  - Plus 30+ other utilities
- **gaster**: checkm8 exploit tool for entering DFU/recovery modes
- **Batch scripts**: Automation for bypass workflows

## Project Structure

```
infex1rn/
├── .github/              # GitHub configuration
├── Services/             # Business logic and device interaction
│   ├── DeviceService.cs  # iOS device detection and info
│   ├── AppService.cs     # App installation and management
│   ├── FirmwareService.cs # IPSW handling
│   └── RamdiskService.cs # Ramdisk operations
├── ViewModels/           # MVVM view models
│   ├── MainViewModel.cs  # Main window logic
│   └── ViewModelBase.cs  # Base class for ViewModels
├── tools/                # External command-line tools
│   ├── libimobiledevice/ # iOS communication utilities
│   ├── gaster/           # checkm8 exploit tool
│   └── *.bat             # Automation scripts
├── ramdisks/             # Storage for SSH ramdisk files
├── ipsw/                 # Storage for iOS firmware files
├── docs/                 # Documentation
├── App.xaml[.cs]         # Application entry point
├── MainWindow.xaml[.cs]  # Main UI window
└── infex1rn.csproj       # Project configuration
```

## Coding Standards and Conventions

### General Guidelines
- **Language Version**: C# 10 or compatible with .NET 6
- **Naming Conventions**:
  - Use PascalCase for class names, method names, and public properties
  - Use camelCase for private fields with underscore prefix (e.g., `_deviceService`)
  - Use PascalCase for namespaces
- **Comments**: Use XML documentation comments (`///`) for public APIs
- **Error Handling**: 
  - Use `.ThrowOnError()` extension method for imobiledevice-net errors
  - Handle device disconnection scenarios gracefully
  - Show user-friendly error messages in UI

### MVVM Pattern Requirements
- **Separation of Concerns**: Keep UI logic in ViewModels, business logic in Services
- **Data Binding**: Use INotifyPropertyChanged for all ViewModel properties
- **Commands**: Implement ICommand for user actions
- **ObservableCollections**: Use for lists that update the UI
- **ViewModelBase**: Inherit from ViewModelBase for common MVVM functionality

### Service Layer Patterns
- **Dependency Injection**: ViewModels receive services via constructor
- **Resource Management**: Always use `using` statements for iMobileDevice handles
  - `iDeviceHandle`, `LockdownClientHandle`, `InstallationProxyClientHandle`, etc.
  - These are unmanaged resources that must be properly disposed
- **Device Communication Flow**:
  1. Get device handle with `idevice_new()`
  2. Create lockdown client with `lockdownd_client_new_with_handshake()`
  3. Start specific service (e.g., installation_proxy, afc)
  4. Perform operations
  5. Dispose all handles in reverse order

### imobiledevice-net Best Practices
- Always check return values with `.ThrowOnError()` for critical operations
- Use LibiMobileDevice.Instance to access API instances
- Handle NoDevice errors separately from other errors
- Never leak device handles - always dispose properly
- Use client label "infex1rn" for lockdown handshakes

### External Tool Integration
- **Tool Execution**: Use `System.Diagnostics.Process` for running batch scripts and tools
- **Console Output**: Tool output is displayed in allocated console window (see App.xaml.cs)
- **Working Directory**: Set to tools folder when executing tools
- **Error Detection**: Parse tool output for error messages and exit codes

### UI/UX Guidelines
- **Responsive Design**: Use async/await for long-running operations
- **User Feedback**: Show progress indicators for device operations
- **Error Messages**: Display clear, actionable error messages via MessageBox
- **TreeView Usage**: Use TreeViewItem collections for hierarchical data (IPSW contents, file systems)

## Common Patterns and Examples

### Device Service Pattern
```csharp
public Dictionary<string, string> GetDeviceInfo(string udid)
{
    var idevice = LibiMobileDevice.Instance.iDevice;
    var lockdown = LibiMobileDevice.Instance.Lockdown;
    
    iDeviceHandle deviceHandle;
    idevice.idevice_new(out deviceHandle, udid).ThrowOnError();
    
    using (deviceHandle)
    {
        LockdownClientHandle lockdownHandle;
        lockdown.lockdownd_client_new_with_handshake(
            deviceHandle, out lockdownHandle, "infex1rn").ThrowOnError();
        
        using (lockdownHandle)
        {
            // Perform operations
        }
    }
}
```

### ViewModel Property Pattern
```csharp
private string _selectedDevice;
public string SelectedDevice
{
    get => _selectedDevice;
    set
    {
        _selectedDevice = value;
        OnPropertyChanged(); // Notify UI of change
        LoadDeviceDetails(); // Trigger related actions
    }
}
```

### Observable Collection Pattern
```csharp
public ObservableCollection<string> Devices { get; } 
    = new ObservableCollection<string>();
```

## Security Considerations

### Critical Security Rules
- **NEVER commit sensitive data**: Device UDIDs, activation data, user credentials
- **Validate all inputs**: Especially file paths, device UDIDs, and user-provided data
- **Safe file operations**: 
  - Validate IPSW and ramdisk files before processing
  - Check file extensions and signatures
  - Prevent path traversal attacks
- **Tool execution safety**:
  - Sanitize command-line arguments
  - Validate tool paths before execution
  - Don't expose tool output that may contain sensitive data

### Legal and Ethical Guidelines
- Code should include warnings about authorized use only
- Document that bypasses may violate terms of service
- Emphasize educational and recovery purposes
- Check device ownership before bypass operations

## Testing Guidelines

### Manual Testing
- Test device detection with actual iOS devices connected
- Verify all features work with target device range (iPhone 5s - iPhone X)
- Test error handling with device disconnection scenarios
- Validate tool execution on clean Windows environment

### Error Scenarios to Test
- No device connected
- Device disconnected during operation
- Invalid IPSW or ramdisk files
- Missing external tools
- Insufficient permissions

## Building and Running

### Build Requirements
- .NET 6 SDK
- Windows OS (required for WPF)
- Visual Studio 2022 or later (recommended)

### Build Commands
```bash
dotnet restore
dotnet build
dotnet run
```

### Dependencies Installation
- NuGet packages are restored automatically
- External tools (libimobiledevice, gaster) are included in `/tools` directory

### Console Window
- Application allocates a console window on startup for tool output
- Console shows messages from external tools like gaster and libimobiledevice
- Keep console window open while application is running

## File Locations and Organization

### User Data Directories
- `/ramdisks/`: User places SSH ramdisk files (.dmg, .img4) here
- `/ipsw/`: User places iOS firmware files (.ipsw) here
- `/tools/`: External utilities (DO NOT modify)

### File Type Handling
- **IPSW files**: ZIP archives containing iOS firmware components
- **Ramdisk files**: Disk images (.dmg) or IMG4 files (.img4)
- **Boot files**: ibss.img4, ibec.img4, devicetree.img4, etc.

## Common Tasks and How to Implement

### Adding a New Device Service Method
1. Add method to appropriate service class in `/Services/`
2. Follow the device communication pattern (handle → lockdown → service → operation)
3. Use proper handle disposal with `using` statements
4. Add error handling with `.ThrowOnError()`
5. Update ViewModel to expose the functionality

### Adding a New UI Feature
1. Add UI elements to MainWindow.xaml
2. Create properties and commands in MainViewModel
3. Implement business logic in appropriate service
4. Bind UI to ViewModel properties
5. Test with actual device

### Integrating a New External Tool
1. Add tool executable to `/tools/` directory
2. Create batch script wrapper if needed
3. Add Process execution code in appropriate service
4. Capture and display output in console
5. Handle errors and exit codes

## Documentation

### Code Documentation
- Use XML comments (`///`) for all public methods and classes
- Document parameters, return values, and exceptions
- Include usage examples for complex methods

### README Updates
- Update ramdisks/README.md for ramdisk-related changes
- Update ipsw/README.md for firmware-related changes
- Update docs/ for major feature additions

## Performance Considerations

- Use async/await for device operations to keep UI responsive
- Minimize device handle lifetime - dispose as soon as operation completes
- Cache device information when appropriate
- Don't repeatedly query device for static information

## Troubleshooting Common Issues

### Device Not Detected
- Check USB connection
- Verify iTunes drivers are installed
- Ensure device is trusted (accept trust dialog on device)

### Tool Execution Failures
- Verify tools exist in `/tools/` directory
- Check Windows permissions for tool execution
- Ensure device is in correct mode (normal/recovery/DFU)

### Memory Issues
- Always dispose imobiledevice-net handles
- Use `using` statements for all handle types
- Don't cache device handles across operations

## Additional Notes

- **checkm8 Compatibility**: Features requiring checkm8 exploit only work on A7-A11 devices
- **Windows Only**: This is a Windows-specific application (WPF requirement)
- **USB Required**: All device operations require USB connection (not Wi-Fi sync)
- **Admin Rights**: Some operations may require administrator privileges

## Questions to Ask Before Coding

1. Does this feature require device communication? → Use appropriate Service class
2. Is this a UI change? → Update XAML and ViewModel
3. Does this use external tools? → Add to tools/ and create Process wrapper
4. Will this work on all supported devices (A7-A11)? → Check compatibility
5. Are there security implications? → Review security guidelines above
6. Does this handle device disconnection? → Add error handling

---

**Remember**: Always prioritize security, handle errors gracefully, and maintain clean separation between UI (Views), logic (ViewModels), and device operations (Services). When in doubt, follow existing patterns in the codebase.
