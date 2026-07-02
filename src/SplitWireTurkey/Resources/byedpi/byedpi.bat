@echo off
title ByeDPI

pushd "%~dp0"
if "%~1" == "" (
    ciadpi.exe -o1 -a1 -r-5+se -H hosts.txt
) else (
    ciadpi.exe %*
)
popd