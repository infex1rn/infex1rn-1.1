# infex1rn v1.1 - Quick Reference Card

## 🚀 What's New in v1.1

### 1. No More Crashes! ✅
IPSW loading is now rock-solid with comprehensive error handling.

### 2. Drag & Drop IPSW Files 🎯
Simply drag your .ipsw file into the CFW Studio tab - no more clicking through dialogs!

### 3. Working Ramdisk Extraction 💾
Extract ramdisk with one click, automatic folder creation, and explorer auto-open.

### 4. Beautiful Modern UI 🎨
Dark theme, smooth animations, emoji icons, and professional styling throughout.

---

## 📖 Quick Start

### First Time Setup
1. Install .NET 6.0 Desktop Runtime
2. Install iTunes (for device drivers)
3. Run infex1rn.exe
4. Connect iOS device via USB

### Connect Your Device
1. Click "🔄 List Devices"
2. Select your device from the list
3. View info in "📱 Device Info" tab

### Load IPSW (Two Ways!)

**Easy Way (NEW!):**
1. Go to "🔧 CFW Studio" tab
2. Drag .ipsw file from Windows Explorer
3. Drop it on the tree
4. Done!

**Traditional Way:**
1. Go to "🔧 CFW Studio" tab
2. Click "📁 Load IPSW"
3. Select your file
4. Click Open

### Extract Ramdisk
1. Load an IPSW (any method)
2. Wait for "💾 Extract Ramdisk" to enable (green)
3. Click the button
4. Windows Explorer opens showing your file!

### Perform Untethered Bypass
1. Download IPSW for your device
2. Click "🎯 Auto Extract Ramdisk" in System Utilities
3. Select IPSW and wait for extraction
4. Put device in DFU mode
5. Click "✅ Untethered Bypass"
6. Follow on-screen instructions

---

## 🎨 UI Guide

### Color Meanings
- **Blue Buttons** 🔵 Important actions (Load, Install, Bypass)
- **Green Buttons** 🟢 Completion actions (Save, Success, Untethered)
- **Gray Buttons** ⚫ Standard actions

### Tab Icons
- 📱 Device Info - View device details
- 📦 App Management - Install/manage apps
- 📁 File Manager - Browse app files
- 🛠️ System Utilities - Bypass and recovery tools
- 🔧 CFW Studio - IPSW firmware tools
- 💿 Ramdisk Studio - Edit ramdisk images

---

## ⚡ Keyboard Shortcuts

### Navigation
- `Tab` - Move between controls
- `Ctrl+Tab` - Switch tabs
- `Enter` - Activate selected button
- `Escape` - Cancel dialogs

---

## 🔧 Common Tasks

### Install an IPA
1. Connect device and select it
2. Go to "📦 App Management"
3. Click "📲 Install IPA"
4. Select your .ipa file
5. Watch progress bar

### Enter Recovery Mode
1. Connect and select device
2. Click "⚡ Enter Recovery" (left panel)
3. Or go to System Utilities tab

### Browse App Files
1. Connect and select device
2. Go to "📁 File Manager"
3. Select an app from dropdown
4. Browse files in tree view

### Extract Boot Files
1. Load IPSW in CFW Studio
2. Right-click a file/folder in tree
3. Select "📤 Extract..."
4. Files saved to extracted_firmware/

---

## ⚠️ Troubleshooting

### Device Not Showing
- Install iTunes drivers
- Try different USB port
- Trust computer on device
- Click "🔄 List Devices" again

### IPSW Won't Load
- Check file is .ipsw format
- File may be corrupted - redownload
- Check disk space
- Try drag-and-drop method

### Extract Ramdisk Disabled
- IPSW doesn't contain ramdisk
- Try a different iOS version
- Check error message in popup

### Bypass Not Working
- Device must be in DFU mode
- Only works on A7-A11 devices
- Check USB connection
- Restart application

---

## 💡 Tips & Tricks

### Faster Workflow
- Use drag-and-drop for IPSW files
- Keep device connected while working
- Use Quick Actions in left panel
- Explorer auto-opens after extraction

### Best Practices
- Always backup device first
- Verify IPSW matches your device model
- Read tool output in System Utilities
- Keep ramdisk files organized

### Device Compatibility
- **checkm8 exploits**: A7-A11 only
  - iPhone 5s, 6, 6s, 7, 8, X
  - iPad Air, Air 2, mini 2-4, Pro (some)
- **Standard features**: All iOS devices

---

## 🎯 Button Quick Reference

### Left Panel
- 🔄 List Devices - Refresh device list
- ⚡ Enter Recovery - Recovery mode
- 🔓 Exit Recovery - Exit recovery

### System Utilities
- 🔄 Load Ramdisk - Load ramdisk to device
- ⚡ Enter PwnDFU - Enter pwned DFU mode
- 🟣 Enter Purple Mode - Purple restore mode
- 🔓 Bypass Activation Lock - Tethered bypass
- 💾 Ramdisk Exploit - checkm8 ramdisk boot
- 📱 A10 Hello Bypass - iPhone 7/7+ bypass
- 🎯 Auto Extract Ramdisk - Extract from IPSW
- ✅ Untethered Bypass - Permanent bypass

### CFW Studio
- 📁 Load IPSW - Open file dialog
- 💾 Extract Ramdisk - Extract ramdisk file
- 📤 Extract (right-click) - Extract selected item

### Ramdisk Studio
- 📁 Load Ramdisk - Open ramdisk file
- ➕ Add File - Add to ramdisk
- ➖ Remove File - Remove from ramdisk
- 💾 Save Ramdisk - Save changes

---

## 📊 Status Bar Info

### Left Side
Version and application name

### Right Side
⚠️ **For Educational & Authorized Use Only**
- Only use on devices you own
- Respect laws and regulations
- Educational/research purposes

---

## 🔐 Security Reminders

✅ **Always:**
- Use on your own devices
- Keep backups
- Understand what each tool does
- Follow legal guidelines

❌ **Never:**
- Use on others' devices without permission
- Violate Apple Terms of Service
- Use for illegal purposes
- Ignore security warnings

---

## 📚 More Information

- **Full Documentation**: See README.md
- **New Features**: See docs/NEW_FEATURES_v1.1.md
- **UI Guide**: See docs/UI_IMPROVEMENTS.md
- **Technical Details**: See IMPLEMENTATION_SUMMARY.md
- **Visual Changes**: See VISUAL_CHANGES.md

---

## 🆘 Getting Help

### If you encounter issues:
1. Check troubleshooting section above
2. Read the error message carefully
3. Check documentation folder
4. Open GitHub issue with details

### Include in bug reports:
- Windows version
- Device model and iOS version
- What you were trying to do
- Exact error message
- Tool output from System Utilities tab

---

## 🎉 Enjoy infex1rn v1.1!

Thank you for using infex1rn. We hope these improvements make your iOS device management and research work easier and more enjoyable.

**Remember**: Use responsibly and ethically!

---

**Version**: 1.1  
**Last Updated**: 2024  
**For**: Windows 7+ (.NET 6)
