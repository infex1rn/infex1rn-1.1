# Untethered Bypass Guide

This guide explains how to perform an untethered iCloud bypass using infex1rn.

## What is Untethered Bypass?

An **untethered bypass** is a permanent bypass that survives device reboots. Unlike tethered bypasses which require re-application after every restart, untethered bypasses modify system files to permanently remove the activation lock.

### Tethered vs Untethered

| Feature | Tethered | Untethered |
|---------|----------|------------|
| Survives reboot | ❌ No | ✅ Yes |
| Requires computer on boot | ✅ Yes | ❌ No |
| Daily usability | Limited | Full |
| Complexity | Lower | Higher |

## Supported Devices

Untethered bypass works on checkm8-compatible devices:

| Device | Chip | iOS Support |
|--------|------|-------------|
| iPhone 5s | A7 | 12.0 - 12.5.x |
| iPhone 6/6+ | A8 | 12.0 - 12.5.x |
| iPhone 6s/6s+ | A9 | 12.0 - 15.8.x |
| iPhone SE (1st) | A9 | 12.0 - 15.8.x |
| iPhone 7/7+ | A10 | 12.0 - 15.8.x |
| iPhone 8/8+ | A11 | 12.0 - 16.7.x |
| iPhone X | A11 | 12.0 - 16.7.x |

## Required Files

Place these files in the `ramdisks/` directory:

1. **ibss.img4** - iBSS bootloader (device-specific)
2. **ibec.img4** - iBEC bootloader (device-specific)
3. **ramdisk.dmg** - SSH ramdisk with bypass tools
4. **devicetree.img4** - Device tree (device-specific)
5. **trustcache.img4** - Trust cache for code signing

## How to Get Required Files

### Option 1: Use SSHRD_Script
```bash
# On macOS/Linux
./sshrd.sh <iOS version>
```

### Option 2: Extract from IPSW
1. Download IPSW from ipsw.me
2. Use CFW Studio in infex1rn to extract components
3. Decrypt and patch using img4tool

### Option 3: Pre-built Packages
Check community resources for pre-built ramdisk packages.

## Usage

### Step 1: Put Device in DFU Mode
1. Connect device to computer
2. Hold Power + Home (or Volume Down for iPhone 7+)
3. Release Power, keep holding Home/Volume Down
4. Screen should be black (not showing recovery logo)

### Step 2: Run Untethered Bypass
```batch
cd tools
untethered_bypass.bat hello
```

### Available Modes

- **hello** - Bypass Hello/Activation screen
- **passcode** - Bypass passcode (for data recovery)
- **disabled** - Bypass disabled device

## What the Bypass Does

### Hello/Activation Bypass
1. Boots SSH ramdisk using checkm8
2. Mounts device filesystems
3. Moves/patches Setup.app
4. Writes activation records
5. Sets nvram to auto-boot
6. Reboots device

### Result
- Device boots directly to home screen
- No activation prompt on reboot
- Wi-Fi works normally
- Cellular may be limited (depends on method)

## Troubleshooting

### "Failed to enter pwned DFU mode"
- Ensure device is in DFU (not Recovery) mode
- Try a different USB cable/port
- Device must be A7-A11 chip

### "iBSS/iBEC not found"
- Download/create boot files for your specific device
- Place them in the `ramdisks/` directory
- File names must match exactly

### Bypass doesn't persist after reboot
- Ensure all steps completed successfully
- Verify ramdisk has proper bypass scripts
- Try running the bypass again

## Important Notes

⚠️ **Legal Notice**: Only use on devices you own or have authorization to access.

⚠️ **Limitations**: 
- Cellular signal may not work on some devices
- Find My iPhone features will be disabled
- Some apps may not work without activation

⚠️ **Backup**: Always backup data before attempting bypass.

## Additional Resources

- The iPhone Wiki: https://www.theiphonewiki.com
- IPSW Downloads: https://ipsw.me
- Firmware Keys: https://www.theiphonewiki.com/wiki/Firmware_Keys
