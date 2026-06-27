@echo off
title ByeDPI

if "%~1" == "" (
    ciadpi.exe -o1 -a1 -r-5+se
) else (
    ciadpi.exe %*
)