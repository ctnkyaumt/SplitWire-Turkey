@echo off
setlocal

set "INSTALLER=%~1"
set "APPEXE=%~2"
set "APPNAME=SplitWire-Turkey.exe"
set "LOGFILE=%~dp1install.log"
set "WAITCOUNT=0"

:waitloop
tasklist /FI "IMAGENAME eq %APPNAME%" 2>nul | find /I "%APPNAME%" >nul
if not errorlevel 1 (
    set /a WAITCOUNT+=1
    if %WAITCOUNT% GEQ 30 (
        taskkill /IM "%APPNAME%" /F >nul 2>&1
        goto afterwait
    )
    timeout /t 1 /nobreak >nul
    goto waitloop
)
:afterwait

"%INSTALLER%" /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP- /CLOSEAPPLICATIONS /NORESTARTAPPLICATIONS /LOG="%LOGFILE%"

timeout /t 2 /nobreak >nul

start "" "%APPEXE%"

del "%INSTALLER%" >nul 2>&1

(goto) 2>nul & del "%~f0"
