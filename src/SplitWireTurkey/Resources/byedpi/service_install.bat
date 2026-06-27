@echo off
title ByeDPI - Auto Install Service
pushd "%~dp0"

echo Autmoated ByeDPI Service Installation

set svc_name="ByeDPI"
set svc_desc="Local SOCKS proxy server to bypass DPI (Deep Packet Inspection)."

:: Set up launch args (bypass methods) here.
if "%~1" == "" (
    set svc_bin="\"%cd%\ciadpi.exe\" --split 1 --disorder 3+s --mod-http=h,d --auto=torst --tlsrec 1+s"
) else (
    set svc_bin="\"%cd%\ciadpi.exe\" %*"
)

sc stop %svc_name%
sc delete %svc_name%
sc create %svc_name% binPath= %svc_bin% start= "auto"
sc description %svc_name% %svc_desc%
sc start %svc_name%

popd
