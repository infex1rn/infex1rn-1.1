# Custom Ramdisks for infex1rn

This directory is used to store custom ramdisk files for use with the infex1rn tool.

## What are Ramdisks?

SSH ramdisks (also called forensics ramdisks) are minimal system images that boot on iOS devices and enable SSH access. They are commonly used for:
- Data extraction and recovery
- Setting up minimal jailbreak with ROOT/AFC2 access
- Bypassing Activation Lock by editing system files
- Device forensics and diagnostics

## Supported Ramdisk Formats

- `.dmg` - Disk image files
- `.img4` - IMG4 format files (for 64-bit devices)
- Custom ramdisk packages

## Required Files for Untethered Bypass

For untethered bypass, you need the following device-specific files:

| File | Description |
|------|-------------|
| `ibss.img4` | iBSS bootloader (first stage) |
| `ibec.img4` | iBEC bootloader (second stage) |
| `ramdisk.dmg` | SSH ramdisk with bypass scripts |
| `devicetree.img4` | Device tree for your device |
| `trustcache.img4` | Trust cache for code signing |
| `kernelcache.img4` | Patched kernel (optional) |

## Where to Get Ramdisks

### Option 1: Create Your Own
You can create custom ramdisks using the following tools:
1. **SSHRD_Script** - Popular script for creating SSH ramdisks
2. **palera1n** - Includes ramdisk creation capabilities
3. **Legacy-iOS-Kit** - For older iOS versions

### Option 2: Pre-built Ramdisks
Check these resources for pre-built ramdisks:
- The iPhone Wiki (theiphonewiki.com)
- Community forums and repositories

## Supported Devices

Ramdisk-based exploits work on devices with checkm8 vulnerability:
- iPhone 5s to iPhone X (A7-A11 chips)
- iPad Air to iPad 7th generation
- iPad mini 2 to iPad mini 5
- iPad Pro 1st and 2nd generation

## Tethered vs Untethered Bypass

| Type | Survives Reboot | Daily Use | Files Needed |
|------|-----------------|-----------|--------------|
| Tethered | ❌ No | Limited | ramdisk.dmg only |
| Untethered | ✅ Yes | Full | All boot files |

## Usage

### Tethered Bypass
1. Place your ramdisk file in this directory
2. Use the "Ramdisk Exploit (checkm8)" feature in infex1rn
3. Follow the on-screen instructions

### Untethered Bypass
1. Place all required boot files in this directory
2. Use the "Untethered Bypass" feature in infex1rn
3. The bypass will persist after device reboots

## Important Notes

- Ramdisk bypasses are typically "tethered" - must be re-applied after reboot
- Untethered bypasses require additional boot files but persist after reboot
- Cellular functions may be disabled after bypass
- Use only for devices you own or have authorization to access
- Educational and research purposes only
