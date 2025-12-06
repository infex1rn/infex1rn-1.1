@echo off
REM SSH Ramdisk Creator - Creates and boots SSH ramdisks for iOS devices
REM Requires: Python 3, img4tool, and device in DFU mode

echo [*] SSH Ramdisk Creator for infex1rn
echo [*] Supported: checkm8 devices (A7-A11)
echo.

if "%~1"=="" (
    echo Usage: sshrd.bat [create^|boot] [options]
    echo.
    echo Commands:
    echo   create ^<ipsw_path^>  - Create SSH ramdisk from IPSW
    echo   boot ^<ramdisk^>      - Boot an existing ramdisk
    echo.
    echo Examples:
    echo   sshrd.bat create ..\ipsw\iPhone_4.7_15.8_19H370_Restore.ipsw
    echo   sshrd.bat boot ..\ramdisks\ssh_ramdisk.dmg
    exit /b 1
)

if /i "%~1"=="create" (
    if "%~2"=="" (
        echo [!] Error: Please specify IPSW path
        exit /b 1
    )
    echo [*] Creating SSH ramdisk from: %~2
    echo [*] Step 1: Extracting ramdisk from IPSW...
    echo [!] Note: This requires additional tools (img4tool, ipsw)
    echo [!] Please use the CFW Studio in infex1rn GUI for extraction
    exit /b 0
)

if /i "%~1"=="boot" (
    if "%~2"=="" (
        echo [!] Error: Please specify ramdisk path
        exit /b 1
    )
    echo [*] Booting SSH ramdisk: %~2
    echo [*] Step 1: Putting device in pwned DFU mode...
    gaster\gaster.exe pwn
    if errorlevel 1 (
        echo [!] Failed to put device in pwned DFU mode
        echo [!] Make sure device is in DFU mode and connected
        exit /b 1
    )
    echo [*] Device is now in pwned DFU mode
    echo [*] Step 2: Sending ramdisk...
    libimobiledevice\irecovery.exe -f "%~2"
    if errorlevel 1 (
        echo [!] Failed to send ramdisk
        exit /b 1
    )
    echo [*] Step 3: Booting ramdisk...
    libimobiledevice\irecovery.exe -c ramdisk
    if errorlevel 1 (
        echo [!] Failed to boot ramdisk
        exit /b 1
    )
    echo [*] Ramdisk booted successfully!
    echo [*] You can now connect via SSH (default: root@localhost, port 22)
    exit /b 0
)

echo [!] Unknown command: %~1
echo [!] Use 'create' or 'boot'
exit /b 1
