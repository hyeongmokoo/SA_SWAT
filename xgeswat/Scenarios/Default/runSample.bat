@echo off
C: 
cd "C:\SWAT\ProgTest\xgeswat\Scenarios\Default\TxtInOut"
start /w SWAT_Edit.exe
swat2012.exe
if %errorlevel% == 0 exit 0
echo.
