@echo off
title Kompilator SystemOptimizer (Wersje Portable)
color 0A

echo ========================================================
echo Trwa przygotowywanie wersji Portable (Single File)...
echo ========================================================
echo.

echo [1/3] Kompilacja wersji 64-bitowej (win-x64)...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
if %ERRORLEVEL% neq 0 goto :error
echo Gotowe.
echo.

echo [2/3] Kompilacja wersji 32-bitowej (win-x86)...
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true
if %ERRORLEVEL% neq 0 goto :error
echo Gotowe.
echo.

echo [3/3] Kompilacja wersji ARM64 (win-arm64)...
dotnet publish -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true
if %ERRORLEVEL% neq 0 goto :error
echo Gotowe.
echo.

echo ========================================================
echo KOMPILACJA ZAKONCZONA SUKCESEM!
echo ========================================================
echo Gotowe pliki .exe znajdziesz w podfolderach:
echo bin\Release\net9.0-windows\ (win-x64, win-x86, win-arm64) \publish\
echo.
pause
exit

:error
color 0C
echo.
echo ========================================================
echo WYSTAPIL BLAD PODCZAS KOMPILACJI!
echo Sprawdz powyzsze komunikaty, aby znalezc przyczyne.
echo ========================================================
pause
exit