@echo off
REM IPSW Downloader Helper - Opens browser to download IPSW files
REM Uses ipsw.me for official Apple firmware downloads

echo [*] IPSW Downloader Helper for infex1rn
echo.

if "%~1"=="" (
    echo Usage: download_ipsw.bat ^<device_identifier^>
    echo.
    echo Examples:
    echo   download_ipsw.bat iPhone6,1    - iPhone 5s (GSM)
    echo   download_ipsw.bat iPhone7,2    - iPhone 6
    echo   download_ipsw.bat iPhone8,1    - iPhone 6s
    echo   download_ipsw.bat iPhone9,1    - iPhone 7 (Global)
    echo   download_ipsw.bat iPhone10,1   - iPhone 8 (Global)
    echo   download_ipsw.bat iPhone10,3   - iPhone X (Global)
    echo.
    echo Common Device Identifiers:
    echo   iPhone5s:  iPhone6,1 (GSM) / iPhone6,2 (Global)
    echo   iPhone6:   iPhone7,2 / iPhone7,1 (Plus)
    echo   iPhone6s:  iPhone8,1 / iPhone8,2 (Plus)
    echo   iPhone7:   iPhone9,1 (Global) / iPhone9,3 (GSM)
    echo   iPhone8:   iPhone10,1 (Global) / iPhone10,4 (GSM)
    echo   iPhoneX:   iPhone10,3 (Global) / iPhone10,6 (GSM)
    echo.
    echo Opening ipsw.me in browser...
    start https://ipsw.me
    exit /b 0
)

echo [*] Opening download page for device: %~1
start https://ipsw.me/product/%~1
echo.
echo [*] Browser opened to ipsw.me
echo [*] Download the desired iOS version and save to the 'ipsw' folder
echo.
