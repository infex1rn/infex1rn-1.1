@echo off
REM Ramdisk loader - Load ramdisk on device in DFU mode
echo [*] Starting ramdisk loader...
echo [*] Note: Device must be in pwned DFU mode before running this script
echo [*] Step 1: Loading ramdisk...
libimobiledevice\irecovery.exe -f "%~1"
if errorlevel 1 (
    echo [!] Failed to send ramdisk file
    exit /b 1
)
echo [*] Step 2: Executing ramdisk command...
libimobiledevice\irecovery.exe -c ramdisk
if errorlevel 1 (
    echo [!] Failed to execute ramdisk command
    exit /b 1
)
echo [*] Ramdisk loaded successfully!
