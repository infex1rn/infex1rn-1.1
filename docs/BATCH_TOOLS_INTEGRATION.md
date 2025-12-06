# All Batch Tools Integration

## Overview
All 9 batch files from the `tools` folder are now fully integrated into the infex1rn application UI. Users can run any batch script directly from the System Utilities tab without navigating to the command line.

## Available Batch Tools

### Device Control Tools

#### 🔄 Load Ramdisk
- **File**: `ramdisk.bat`
- **Purpose**: Load ramdisk to device using checkm8
- **Steps**: 
  1. Puts device in pwned DFU mode
  2. Loads selected ramdisk file
  3. Executes ramdisk command
- **Requires**: Device in DFU mode, ramdisk file

#### ⚡ Enter PwnDFU
- **File**: `gaster.bat`
- **Purpose**: Put device in pwned DFU mode using checkm8
- **Supports**: A7-A11 devices (iPhone 5s through iPhone X)
- **Requires**: Device in DFU mode

#### 🟣 Enter Purple Mode
- **File**: `purple.bat`
- **Purpose**: Enter purple restore mode for recovery operations
- **Uses**: irecovery for mode switching
- **Requires**: Device connected via USB

### Bypass Tools

#### 🔓 Bypass Activation Lock
- **File**: `bypass.bat`
- **Purpose**: Basic activation lock bypass using ideviceactivation
- **Method**: Activation server spoofing
- **Requires**: Device connected, activation screen

#### 💾 Ramdisk Exploit (checkm8)
- **File**: `ramdisk.bat`
- **Purpose**: Full ramdisk exploit workflow
- **Process**:
  1. Pwn DFU mode with gaster
  2. Load ramdisk DMG
  3. Execute ramdisk
- **Requires**: Device in DFU mode, ramdisk file

#### 📱 A10 Hello Bypass (Signals)
- **File**: `a10_hello_bypass.bat`
- **Purpose**: Specialized bypass for iPhone 7/7+ using signal manipulation
- **Devices**: iPhone 7, iPhone 7 Plus (A10 Fusion)
- **Method**: Signal-based activation lock bypass
- **Requires**: iPhone 7/7+ in DFU mode

#### ✅ Untethered Bypass
- **File**: `untethered_bypass.bat`
- **Purpose**: Persistent bypass that survives reboots
- **Supports**: A7-A11 devices, iOS 12.0 - 16.7.x
- **Modes**:
  - `hello` - Bypass Hello/Setup screen
  - `passcode` - Bypass passcode lock
  - `disabled` - Bypass disabled device
- **Method**: checkm8 + system file modification
- **Requires**: Device in DFU mode

### Advanced Tools

#### 🎯 Auto Extract Ramdisk (from IPSW)
- **Implementation**: C# native in app
- **Purpose**: Extract ramdisk and boot files from IPSW
- **Features**:
  - Extracts ramdisk.dmg
  - Extracts iBSS, iBEC boot files
  - Creates patch_setup_app.sh
  - Auto-organizes in ramdisks folder
- **Requires**: IPSW file

#### 🔧 Auto Ramdisk Creator (Batch)
- **File**: `auto_ramdisk.bat`
- **Purpose**: Batch script version of auto ramdisk extraction
- **Process**:
  1. Extracts IPSW contents
  2. Locates and extracts ramdisk
  3. Patches ramdisk to remove Setup.app
  4. Prepares for bypass
- **Output**: Ready-to-use ramdisk in ramdisks folder
- **Requires**: IPSW file, device in DFU mode (for deployment)

#### 🔑 SSH Ramdisk Tools
- **File**: `sshrd.bat`
- **Purpose**: Create or boot SSH-enabled ramdisks
- **Modes**:
  - **Create**: Build SSH ramdisk from IPSW
  - **Boot**: Load existing SSH ramdisk to device
- **Features**:
  - SSH access to device filesystem
  - Advanced diagnostics
  - File system modifications
- **Requires**: 
  - Create mode: IPSW file, Python 3, img4tool
  - Boot mode: SSH ramdisk DMG, device in DFU mode

#### 📥 Download IPSW
- **File**: `download_ipsw.bat`
- **Purpose**: Open browser to download official Apple firmware
- **Process**:
  1. Prompts for device identifier
  2. Opens ipsw.me with device-specific page
  3. User downloads appropriate IPSW
- **Device Identifiers**: 
  - iPhone 5s: iPhone6,1 / iPhone6,2
  - iPhone 6: iPhone7,2
  - iPhone 7: iPhone9,1 / iPhone9,3
  - iPhone 8: iPhone10,1 / iPhone10,4
  - iPhone X: iPhone10,3 / iPhone10,6
- **Requires**: Internet connection, web browser

## Usage Guide

### Running Batch Tools from UI

1. **Open System Utilities Tab**
   - Click on "🛠️ System Utilities" tab

2. **Select Desired Tool**
   - Tools are organized in three sections
   - Each button clearly labeled with function

3. **Follow Prompts**
   - Input dialogs appear when needed
   - File selection dialogs for IPSW/ramdisk files
   - Device ID input for downloads

4. **Monitor Output**
   - Console output appears in System Utilities tab
   - Real-time progress updates
   - Error messages displayed clearly

### Examples

#### Example 1: Untethered Bypass
```
1. Click "✅ Untethered Bypass"
2. Ensure device is in DFU mode
3. Script runs automatically
4. Monitor output in console
5. Device bypasses activation lock
```

#### Example 2: Download IPSW
```
1. Click "📥 Download IPSW"
2. Enter device identifier (e.g., iPhone9,1)
3. Browser opens to download page
4. Download appropriate IPSW
5. Use with other tools
```

#### Example 3: SSH Ramdisk
```
1. Click "🔑 SSH Ramdisk Tools"
2. Select IPSW (create mode) or DMG (boot mode)
3. Process runs automatically
4. SSH ramdisk ready or booted
5. Access device via SSH
```

## File Organization

### Tools Folder Structure
```
tools/
├── a10_hello_bypass.bat      # A10 signal bypass
├── auto_ramdisk.bat          # Auto ramdisk creator
├── bypass.bat                # Basic bypass
├── download_ipsw.bat         # IPSW downloader
├── gaster.bat                # checkm8 exploit
├── purple.bat                # Purple mode
├── ramdisk.bat               # Ramdisk loader
├── sshrd.bat                 # SSH ramdisk
├── untethered_bypass.bat     # Untethered bypass
├── gaster/                   # checkm8 binaries
│   └── gaster.exe
└── libimobiledevice/         # iOS tools
    ├── ideviceactivation.exe
    ├── irecovery.exe
    └── (30+ other tools)
```

## Benefits

### Before Integration
- Navigate to tools folder in file explorer
- Open command prompt
- Type batch file name
- Provide arguments manually
- Switch between windows for output

### After Integration
- Single click from UI
- File dialogs for easy selection
- Input dialogs when needed
- Output in same window
- Organized by category

## Technical Details

### Implementation
- **Method**: Process execution via `DeviceService.RunExternalTool()`
- **Output Capture**: Real-time stdout/stderr capture
- **UI Thread**: Dispatcher.Invoke for thread-safe UI updates
- **Error Handling**: Try-catch with user-friendly messages
- **File Dialogs**: OpenFileDialog for file selection
- **Input Dialogs**: Custom WPF windows for text input

### Command Handlers
Each batch file has dedicated command handler:
- `BypassActivationLock()` → bypass.bat
- `RamdiskExploit()` → ramdisk.bat
- `A10HelloBypass()` → a10_hello_bypass.bat
- `UnthetheredBypass()` → untethered_bypass.bat
- `EnterPwnDfu()` → gaster.bat
- `EnterPurpleMode()` → purple.bat
- `AutoRamdiskBatch()` → auto_ramdisk.bat
- `SshRamdisk()` → sshrd.bat
- `DownloadIpsw()` → download_ipsw.bat

### Code Structure
```csharp
private async void ToolName()
{
    ToolOutput = "";
    try
    {
        // File/input dialog if needed
        // Construct command
        // Execute batch file
        await _deviceService.RunExternalTool(path, args, callback);
    }
    catch (Exception ex)
    {
        ToolOutput = $"Error: {ex.Message}";
    }
}
```

## Compatibility

### Device Support
- **A7-A11 checkm8**: iPhone 5s through iPhone X
- **All iOS devices**: For basic tools (activation, recovery)

### Operating System
- **Windows**: All batch files require Windows
- **WPF**: Application is Windows-only

### Dependencies
- **libimobiledevice**: iOS communication tools
- **gaster**: checkm8 exploit implementation
- **Python 3**: For sshrd.bat (optional)
- **img4tool**: For SSH ramdisk creation (optional)

## Troubleshooting

### Tool Not Running
- Check tool output in console
- Verify device is connected
- Ensure device is in correct mode (DFU, recovery, etc.)
- Check USB connection

### Missing Dependencies
- Verify tools folder contains all files
- Download missing components
- Check libimobiledevice installation

### Device Not Detected
- Install iTunes drivers
- Try different USB port/cable
- Check device trust status

## Security Notes

⚠️ **Important Warnings**
- Only use on devices you own
- Understand what each tool does
- Some tools modify system files
- Bypassing activation may violate TOS
- For educational/authorized use only

## Future Enhancements

Potential improvements:
- Batch file output parsing for progress bars
- Advanced parameter configuration
- Tool favorites/shortcuts
- Recent tool history
- Batch operation scheduling

---

**Feature Version**: 1.1  
**Commit**: 26c89bc  
**All Batch Files**: Integrated ✅
