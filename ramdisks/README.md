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

## Usage

1. Place your ramdisk file in this directory
2. Use the "Load Ramdisk" feature in infex1rn
3. Follow the on-screen instructions

## Important Notes

- Ramdisk bypasses are typically "tethered" - must be re-applied after reboot
- Cellular functions may be disabled after bypass
- Use only for devices you own or have authorization to access
- Educational and research purposes only
