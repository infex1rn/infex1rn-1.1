# IPSW Files for infex1rn

This directory is used to store iOS firmware (IPSW) files for the CFW Studio feature.

## What are IPSW Files?

IPSW (iPhone Software) files are firmware packages used by Apple for iOS devices. They contain:
- The operating system
- Ramdisks
- Kernel
- Root filesystem
- Device tree and other components

## Where to Download IPSW Files

### Official Sources
1. **ipsw.me** - https://ipsw.me
   - Direct links to all official Apple firmware files
   - Organized by device and iOS version
   
2. **The iPhone Wiki** - https://www.theiphonewiki.com/wiki/Firmware
   - Comprehensive firmware database
   - Includes firmware keys for decryption

### Recommended IPSW Files for Ramdisk Creation

For creating custom ramdisks, you'll need IPSW files matching your target device:

| Device | Identifier | Recommended iOS |
|--------|-----------|-----------------|
| iPhone 5s | iPhone6,1 / iPhone6,2 | iOS 12.5.x |
| iPhone 6/6+ | iPhone7,1 / iPhone7,2 | iOS 12.5.x |
| iPhone 6s/6s+ | iPhone8,1 / iPhone8,2 | iOS 15.8.x |
| iPhone 7/7+ | iPhone9,1-4 | iOS 15.8.x |
| iPhone 8/8+ | iPhone10,1-6 | iOS 16.x |
| iPhone X | iPhone10,3 / iPhone10,6 | iOS 16.x |

## Usage with infex1rn

1. Download the IPSW file for your target device
2. Place it in this directory
3. Use "CFW Studio" → "Load IPSW" in infex1rn
4. Extract components as needed

## IPSW Structure

A typical IPSW contains:
```
├── BuildManifest.plist
├── Restore.plist
├── Firmware/
│   ├── all_flash/
│   ├── dfu/
│   └── ...
├── kernelcache.*
├── xxx.dmg (root filesystem)
└── xxx.dmg (ramdisk)
```

## Extracting Ramdisks from IPSW

1. Load the IPSW in CFW Studio
2. Navigate to the ramdisk DMG file (usually named with your device identifier)
3. Right-click → Extract
4. The extracted ramdisk can be modified and used with ramdisk tools

## Important Notes

- IPSW files can be large (2-6 GB)
- Ensure you have the correct IPSW for your device model
- Some operations may require firmware decryption keys
- Use only for devices you own or have authorization to access
