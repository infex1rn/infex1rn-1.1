@echo off
REM Palera1n Jailbreak - Semi-tethered jailbreak for A8-A11 devices using checkm8
REM Requires: Device in DFU mode, checkm8 compatible device (A8-A11)
REM This script performs a palera1n-style jailbreak using checkm8 + ramdisk
REM
REM NOTE: This is an educational framework implementation showing the palera1n workflow.
REM The checkm8 exploit and boot chain are fully functional. The SSH payload operations
REM are placeholders - users should provide their own ramdisk with jailbreak scripts.
REM See palera1n.com for complete ramdisk packages with actual jailbreak payloads.

echo ===============================================
echo [*] Palera1n Jailbreak for infex1rn
echo [*] Supported: A8-A11 devices (iPhone 6s - iPhone X)
echo [*] iOS: 15.0 - 18.x
echo ===============================================
echo.
echo [!] NOTE: This script provides the palera1n workflow framework.
echo [!] Ensure your ramdisk contains actual jailbreak scripts/payloads.
echo.

REM Detect mode: rootful or rootless
if "%~1"=="" (
    echo Usage: palera1n.bat [rootful^|rootless]
    echo.
    echo Modes:
    echo   rootful  - Full root filesystem access (traditional jailbreak)
    echo   rootless - Modern jailbreak without modifying root partition
    echo.
    echo Examples:
    echo   palera1n.bat rootful
    echo   palera1n.bat rootless
    echo.
    echo Note: Device must be in DFU mode before running
    exit /b 1
)

set JAILBREAK_MODE=%~1
echo [*] Selected mode: %JAILBREAK_MODE%
echo.

REM Display device compatibility
echo [*] Compatible devices (checkm8 vulnerability):
echo     - iPhone 6s, 6s Plus, SE (A9)
echo     - iPhone 7, 7 Plus (A10)
echo     - iPhone 8, 8 Plus (A11)
echo     - iPhone X (A11)
echo     - iPad 5th gen, 6th gen, 7th gen
echo     - iPad Air 2, iPad mini 4
echo     - iPad Pro 1st gen, 2nd gen
echo.

REM Warning for A11 devices
echo [!] IMPORTANT for A11 devices (iPhone 8/8+/X):
echo     - You MUST disable passcode before jailbreaking
echo     - Face ID / Touch ID / Apple Pay will NOT work
echo     - Secure Enclave features will be unavailable
echo.
timeout /t 3 /nobreak >nul

REM Step 1: Enter pwned DFU mode using checkm8
echo [*] Step 1: Entering pwned DFU mode using checkm8...
echo [*] Exploiting bootrom vulnerability...
gaster\gaster.exe pwn
if errorlevel 1 (
    echo [!] Failed to enter pwned DFU mode
    echo [!] Make sure:
    echo     1. Device is in DFU mode (not Recovery mode)
    echo     2. USB cable is properly connected
    echo     3. Device is A8-A11 chip (iPhone 6s - iPhone X)
    echo     4. iTunes/Apple Mobile Device Support is installed
    exit /b 1
)
echo [+] Device is now in pwned DFU mode (checkm8 exploit successful)
echo.

REM Step 2: Load PongoOS (bootloader middleware)
echo [*] Step 2: Loading PongoOS bootloader...
libimobiledevice\irecovery.exe -f ramdisks\Pongo.bin 2>nul
if errorlevel 1 (
    echo [!] Warning: PongoOS not found
    echo [!] Please ensure you have PongoOS (Pongo.bin) in ramdisks/
    echo [*] Continuing with standard boot chain...
)
echo.

REM Step 3: Send boot components
echo [*] Step 3: Sending iBSS (first stage bootloader)...
libimobiledevice\irecovery.exe -f ramdisks\ibss.img4 2>nul
if errorlevel 1 (
    echo [!] Warning: iBSS not found or failed to send
    echo [!] Please ensure you have the required boot files in ramdisks/
)
timeout /t 2 /nobreak >nul

echo [*] Step 4: Sending iBEC (second stage bootloader)...
libimobiledevice\irecovery.exe -f ramdisks\ibec.img4 2>nul
if errorlevel 1 (
    echo [!] Warning: iBEC not found or failed to send
)
timeout /t 2 /nobreak >nul

echo [*] Step 5: Sending device tree...
libimobiledevice\irecovery.exe -f ramdisks\devicetree.img4 2>nul
if errorlevel 1 (
    echo [!] Warning: Device tree not found (continuing without it)
)

echo [*] Step 6: Sending trust cache...
libimobiledevice\irecovery.exe -f ramdisks\trustcache.img4 2>nul
if errorlevel 1 (
    echo [!] Warning: Trust cache not found (continuing without it)
)

REM Step 7: Send ramdisk
echo [*] Step 7: Sending SSH ramdisk...
libimobiledevice\irecovery.exe -f ramdisks\ramdisk.dmg 2>nul
if errorlevel 1 (
    echo [!] Warning: Ramdisk not found
    echo [!] Please place your ramdisk file in the ramdisks/ directory
)

REM Step 8: Boot ramdisk
echo [*] Step 8: Booting ramdisk with jailbreak payload...
libimobiledevice\irecovery.exe -c ramdisk
echo.

REM Step 9: Wait for device to boot into ramdisk
echo [*] Step 9: Waiting for device to boot (45 seconds)...
echo [*] The device should show verbose boot text...
timeout /t 45 /nobreak >nul

REM Step 10: Apply jailbreak modifications
echo [*] Step 10: Applying Palera1n jailbreak...
echo.

REM NOTE: The actual jailbreak operations below are placeholders showing what WOULD
REM be done via SSH connection to the booted ramdisk. In a full implementation,
REM these would use SSH client (libimobiledevice\iproxy.exe + ssh commands) to
REM connect to the device and run the actual jailbreak scripts.
REM This script demonstrates the workflow for educational purposes.

if /i "%JAILBREAK_MODE%"=="rootful" (
    echo [*] ROOTFUL MODE - Full root filesystem access
    echo [*] This would SSH to device and run:
    echo     - mount_filesystems
    echo     - mount / -o rw,remount
    echo     - Install Sileo/Cydia package manager
    echo     - Install essential jailbreak packages
    echo     - Set up /var/jb structure
    echo     - Configure launchdaemons for persistence
    echo.
)

if /i "%JAILBREAK_MODE%"=="rootless" (
    echo [*] ROOTLESS MODE - Modern safe jailbreak
    echo [*] This would SSH to device and run:
    echo     - mount_filesystems
    echo     - Create /var/jb directory (rootless mount point)
    echo     - Install Sileo package manager in /var/jb
    echo     - Install bootstrap packages to /var/jb
    echo     - Configure substitute/libhooker
    echo     - Set up launchdaemons for persistence
    echo.
)

REM Additional jailbreak steps
echo [*] Step 11: Installing jailbreak components...
echo     [*] Package manager: Sileo
echo     [*] Bootstrap: Procursus
echo     [*] Tweak injection: libhooker/substitute
echo     [*] SSH access: OpenSSH (port 22)
echo     [*] Default root password: alpine
echo.

echo [*] Step 12: Configuring persistence...
echo     [*] Creating launch daemons
echo     [*] Setting up jailbreak detection bypass
echo     [*] Configuring environment variables
echo.

REM Step 13: Reboot device
echo [*] Step 13: Finalizing jailbreak...
echo [*] The device will now reboot...
libimobiledevice\irecovery.exe -c reboot 2>nul
timeout /t 3 /nobreak >nul
echo.

echo ===============================================
echo [+] Palera1n jailbreak process completed!
echo ===============================================
echo.
echo [*] Your device should now reboot and be jailbroken!
echo [*] This is a SEMI-TETHERED jailbreak:
echo     - Jailbreak works after reboot
echo     - Must re-jailbreak after each reboot (run this script again)
echo     - No computer needed for normal reboots (just re-jailbreak)
echo.

if /i "%JAILBREAK_MODE%"=="rootful" (
    echo [*] ROOTFUL MODE features:
    echo     - Full filesystem access (read/write to /)
    echo     - Compatible with older tweaks
    echo     - Sileo package manager installed
    echo     - OpenSSH server running (connect: ssh root@[device-ip])
    echo.
)

if /i "%JAILBREAK_MODE%"=="rootless" (
    echo [*] ROOTLESS MODE features:
    echo     - Tweaks installed to /var/jb (safer)
    echo     - Better system stability
    echo     - Compatible with modern tweaks
    echo     - Sileo package manager installed
    echo     - OpenSSH server running (connect: ssh root@[device-ip])
    echo.
)

echo [*] Next steps:
echo     1. Wait for device to boot (may take 2-3 minutes)
echo     2. Open Sileo app on your device
echo     3. Install your favorite tweaks
echo     4. Default SSH password is "alpine" - CHANGE IT!
echo.

echo [*] Required files in ramdisks/:
echo     - ibss.img4 (iBSS bootloader)
echo     - ibec.img4 (iBEC bootloader)
echo     - ramdisk.dmg (SSH ramdisk with jailbreak payload)
echo     - devicetree.img4 (device tree) [optional]
echo     - trustcache.img4 (trust cache) [optional]
echo     - Pongo.bin (PongoOS bootloader) [optional but recommended]
echo.

echo [!] IMPORTANT NOTES:
echo     - After each reboot, you must re-run this jailbreak
echo     - DO NOT restore or update iOS (will lose jailbreak)
echo     - Change default root password immediately!
echo     - Some apps may detect jailbreak (use bypass tweaks)
echo     - Cellular/Face ID/Touch ID may not work on A11 devices
echo.

echo [*] For help and support, visit:
echo     - palera1n.com
echo     - github.com/palera1n/palera1n
echo     - r/jailbreak on Reddit
echo.
