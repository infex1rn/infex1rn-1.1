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

## Auto Extract Ramdisk (Recommended)

The easiest way to get started is using the **Auto Extract Ramdisk** feature:

### Using the GUI
1. Go to **System Utilities** tab
2. Click **"Auto Extract Ramdisk (from IPSW)"** (blue button)
3. Select your IPSW file
4. Wait for extraction to complete
5. The tool will automatically:
   - Extract the ramdisk from IPSW
   - Extract boot files (iBSS, iBEC, etc.)
   - Create a patch script to remove Setup.app

### Using Command Line
```batch
cd tools
auto_ramdisk.bat path\to\your.ipsw
```

### What Auto Extract Does
1. **Extracts IPSW** - Unpacks the firmware ZIP file
2. **Locates Ramdisk** - Finds the restore ramdisk DMG
3. **Extracts Boot Files** - Copies iBSS, iBEC, device tree, etc.
4. **Creates Patch Script** - Generates `patch_setup_app.sh` that:
   - Mounts device filesystems
   - Renames Setup.app to Setup.app.bak
   - Sets auto-boot for persistent bypass

## Required Files

After extraction, these files will be in the `ramdisks/` directory:

1. **ramdisk.dmg** - SSH ramdisk
2. **patch_setup_app.sh** - Script to remove Setup.app
3. **ibss.im4p** - iBSS bootloader (device-specific)
4. **ibec.im4p** - iBEC bootloader (device-specific)
5. **devicetree.im4p** - Device tree (device-specific)

## Manual File Acquisition

If auto-extraction doesn't work for your device:

### Option 1: Use SSHRD_Script
```bash
# On macOS/Linux
./sshrd.sh <iOS version>
```

### Option 2: Manual IPSW Extract
1. Download IPSW from ipsw.me
2. Use CFW Studio in infex1rn to extract components
3. Decrypt and patch using img4tool

### Option 3: Pre-built Packages
Check community resources for pre-built ramdisk packages.

## Usage

### Step 1: Extract Ramdisk (if not done)
1. Click "Auto Extract Ramdisk (from IPSW)"
2. Select your device's IPSW file
3. Wait for extraction

### Step 2: Put Device in DFU Mode
1. Connect device to computer
2. Hold Power + Home (or Volume Down for iPhone 7+)
3. Release Power, keep holding Home/Volume Down
4. Screen should be black (not showing recovery logo)

### Step 3: Run Untethered Bypass
Click the green **"Untethered Bypass"** button, or run:
```batch
cd tools
untethered_bypass.bat hello
```

### Available Modes

- **hello** - Bypass Hello/Activation screen (removes Setup.app)
- **passcode** - Bypass passcode (for data recovery)
- **disabled** - Bypass disabled device

## What the Bypass Does

### Hello/Activation Bypass (Setup.app Removal)
1. Boots SSH ramdisk using checkm8
2. Mounts device filesystems
3. Renames Setup.app to Setup.app.bak
4. Writes activation bypass markers
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
- Run "Auto Extract Ramdisk" with your device's IPSW
- Or manually download boot files for your specific device
- Place them in the `ramdisks/` directory

### "Ramdisk not found in IPSW"
- Ensure the IPSW is not corrupted
- Try downloading the IPSW again
- Make sure it's for a supported device

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

## Palera1n Jailbreak

infex1rn now includes **Palera1n jailbreak** support! This is a semi-tethered jailbreak for A8-A11 devices.

### What is Palera1n?

Palera1n is a modern jailbreak tool that uses the checkm8 exploit to jailbreak iOS devices. Unlike iCloud bypasses, jailbreaking gives you full control over your device with tweak support.

### Key Features

- **Semi-tethered**: Jailbreak persists after reboot, but must be re-applied after each restart
- **iOS Support**: iOS 15.0 - 18.x
- **Device Support**: A8-A11 devices (iPhone 6s - iPhone X)
- **Two Modes**:
  - **Rootful**: Full root filesystem access (traditional jailbreak, compatible with older tweaks)
  - **Rootless**: Modern jailbreak without modifying root partition (safer, better stability)

### How to Use

1. **Prepare Files**: Place required boot files in `ramdisks/` directory:
   - `ibss.img4` - iBSS bootloader
   - `ibec.img4` - iBEC bootloader
   - `ramdisk.dmg` - SSH ramdisk with jailbreak payload
   - `devicetree.img4` (optional)
   - `trustcache.img4` (optional)
   - `Pongo.bin` (optional but recommended)

2. **Put Device in DFU Mode**:
   - Connect device to computer
   - Follow DFU mode instructions for your device model

3. **Run Palera1n**:
   - Go to **System Utilities** tab
   - Click **"Palera1n Jailbreak"** (purple button)
   - Choose mode:
     - Click **YES** for Rootful mode
     - Click **NO** for Rootless mode
   - Wait for the process to complete (2-3 minutes)

4. **After Jailbreak**:
   - Device will reboot
   - Sileo package manager will be installed
   - OpenSSH server running (default password: `alpine`)
   - Install tweaks from Sileo

### Important Notes for A11 Devices (iPhone 8/8+/X)

⚠️ **MUST disable passcode** before jailbreaking
⚠️ Face ID / Touch ID / Apple Pay **will NOT work**
⚠️ Secure Enclave features will be unavailable

### Palera1n vs iCloud Bypass

| Feature | Palera1n Jailbreak | iCloud Bypass |
|---------|-------------------|---------------|
| Purpose | Full device control + tweaks | Remove activation lock |
| Cellular | ✅ Works | ⚠️ Limited/None |
| Apps | ✅ All apps work | ⚠️ Some may not work |
| Tweaks | ✅ Full tweak support | ❌ No tweaks |
| App Store | ✅ Works | ⚠️ May not work |
| After reboot | Re-jailbreak needed | Works normally |

### Differences from Untethered Bypass

- **Palera1n** = Full jailbreak with package manager and tweaks
- **Untethered Bypass** = Just removes activation lock
- **Use Palera1n** if you want to customize iOS with tweaks
- **Use Untethered Bypass** if you just need to bypass activation

### Troubleshooting Palera1n

**"PongoOS not found"**
- Download Pongo.bin and place in `ramdisks/`
- Or continue without it (will use standard boot chain)

**"Device doesn't boot after jailbreak"**
- Wait 2-3 minutes for first boot
- If stuck, force restart and try again
- Ensure you have the correct boot files for your device

**"Sileo not appearing"**
- Wait a few minutes after boot
- Reboot device and re-jailbreak
- Check if SSH ramdisk has jailbreak payload

### Command Line Usage

```batch
cd tools
palera1n.bat rootful    # For rootful mode
palera1n.bat rootless   # For rootless mode
```

### Resources

- Palera1n Official: https://palera1n.com
- GitHub: https://github.com/palera1n/palera1n
- iOS Guide: https://ios.cfw.guide

