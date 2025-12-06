# Auto-Prepare Bypass Files Feature

## Overview
The Ramdisk Studio now includes an automatic bypass file preparation feature that checks and prepares all required files for a successful iCloud activation lock bypass.

## What It Does

When you load a ramdisk file in Ramdisk Studio, the application automatically:

1. **Checks for gaster.exe** - Copies from `tools/gaster/` to `ramdisks/` folder
2. **Checks for iBSS*.im4p** - Bootchain stage 1 file
3. **Checks for iBEC*.im4p** - Bootchain stage 2 file  
4. **Checks for ramdisk.dmg** - The actual ramdisk file
5. **Creates patch_setup_app.sh** - Bypass patch script (if missing)

## How to Use

### Automatic Check on Load
1. Go to **Ramdisk Studio** tab
2. Click **"📁 Load Ramdisk"**
3. Select your ramdisk file
4. The app automatically checks all bypass files
5. You'll see a notification showing what's ready or missing

### Manual Check
1. Go to **Ramdisk Studio** tab
2. Click **"🔧 Auto-Prepare Bypass Files"** button
3. The app checks all required files
4. View detailed output in **System Utilities** tab

## Required Files for Bypass

| Purpose | File | Status Check |
|---------|------|--------------|
| Boot pwned DFU | gaster.exe | Auto-copied from tools/ |
| Bootchain stage 1 | iBSS*.im4p | Must extract from IPSW |
| Bootchain stage 2 | iBEC*.im4p | Must extract from IPSW |
| Actual ramdisk | ramdisk.dmg | Must extract from IPSW |
| Patch bypass | patch_setup_app.sh | Auto-created if missing |

## Success Scenario

When all files are ready, you'll see:

```
✅ ALL FILES READY FOR BYPASS!

Files in ramdisks folder:
  • gaster.exe (boot pwned DFU)
  • iBSS.d22.RELEASE.im4p (bootchain stage 1)
  • iBEC.d22.RELEASE.im4p (bootchain stage 2)
  • ramdisk.dmg (actual ramdisk)
  • patch_setup_app.sh (patch bypass)

Next steps:
  1. Put device in DFU mode
  2. Run 'Untethered Bypass' from System Utilities
  3. Or use the bypass batch scripts in tools/
```

## Missing Files Scenario

If files are missing, you'll see:

```
⚠️  MISSING FILES - Cannot proceed with bypass

Missing files:
  • iBSS*.im4p
  • iBEC*.im4p
  • ramdisk.dmg

To get missing files:
  Go to System Utilities → 'Auto Extract Ramdisk'
  Select your device's IPSW file
```

## Getting Missing Files

### Option 1: Auto Extract Ramdisk (Recommended)
1. Go to **System Utilities** tab
2. Click **"🎯 Auto Extract Ramdisk (from IPSW)"**
3. Select your device's IPSW file
4. Wait for extraction to complete
5. All required files will be extracted automatically

### Option 2: Manual Extraction
1. Go to **CFW Studio** tab
2. Load or drag-drop your IPSW file
3. Navigate to appropriate files in the tree
4. Right-click → Extract

## File Locations

All bypass files are stored in:
```
<app_directory>/ramdisks/
```

Typical contents after auto-preparation:
```
ramdisks/
├── gaster.exe
├── iBSS.d22.RELEASE.im4p
├── iBEC.d22.RELEASE.im4p
├── ramdisk.dmg
├── patch_setup_app.sh
└── (other extracted boot files)
```

## Benefits

✅ **No More Manual File Gathering** - Everything is checked automatically  
✅ **Clear Status Feedback** - Know exactly what's ready and what's missing  
✅ **Smart Auto-Copy** - gaster.exe copied automatically from tools  
✅ **Auto-Creation** - patch_setup_app.sh created if needed  
✅ **Detailed Instructions** - Clear guidance on getting missing files  
✅ **One-Click Check** - Manual re-check available anytime  

## Technical Details

### Implementation
- **Method**: `RamdiskService.AutoPrepareBypassFiles()`
- **Result**: `BypassPreparationResult` object
- **Triggers**:
  - Automatically on ramdisk load
  - Manually via "Auto-Prepare Bypass Files" button
- **Output**: Real-time progress to System Utilities tab

### File Detection Logic
- **gaster.exe**: Copied from `tools/gaster/gaster.exe`
- **iBSS*.im4p**: Wildcard search in ramdisks folder
- **iBEC*.im4p**: Wildcard search in ramdisks folder
- **ramdisk.dmg**: Exact filename match
- **patch_setup_app.sh**: Created from template if missing

### Status Checks
Each file has individual status:
- ✓ **Ready**: File exists and is accessible
- ✗ **Missing**: File not found, extraction needed

All files must be ready for successful bypass.

## Compatibility

- **Devices**: A7-A11 (iPhone 5s through iPhone X)
- **iOS Versions**: Varies by device and IPSW
- **checkm8**: Required for this bypass method

## Example Workflow

### Complete Bypass Preparation
1. **Get IPSW**: Download iOS firmware for your device
2. **Extract Files**: System Utilities → "Auto Extract Ramdisk" → Select IPSW
3. **Load Ramdisk**: Ramdisk Studio → "Load Ramdisk" → Select ramdisk.dmg
4. **Auto-Prepare**: Automatically checks all files (or click "Auto-Prepare")
5. **Success**: All files ready notification
6. **Bypass**: System Utilities → "Untethered Bypass"

### Quick Re-Check
1. **Ramdisk Studio** → "🔧 Auto-Prepare Bypass Files"
2. **View Output**: Switch to System Utilities tab
3. **Check Status**: All files ready or missing files listed

## Troubleshooting

### "gaster.exe not found"
- Ensure gaster.exe exists in `tools/gaster/` folder
- Reinstall or download latest version

### "iBSS*.im4p not found"
- Extract from IPSW using Auto Extract Ramdisk
- Ensure IPSW matches your device model

### "iBEC*.im4p not found"  
- Extract from IPSW using Auto Extract Ramdisk
- Ensure IPSW matches your device model

### "ramdisk.dmg not found"
- Extract from IPSW using Auto Extract Ramdisk
- Ensure file is named exactly "ramdisk.dmg"

## Related Features

- **Auto Extract Ramdisk**: Extracts all files from IPSW automatically
- **CFW Studio**: Manual IPSW browsing and extraction
- **System Utilities**: Bypass execution tools
- **Untethered Bypass**: Main bypass command

## Security Note

⚠️ **Use Responsibly**
- Only use on devices you own
- Understand the bypass process
- Follow local laws and regulations
- Educational and authorized use only

---

**Feature Added**: v1.1  
**Commit**: 462e2c4  
**Status**: Production Ready
