@echo off
setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION
pushd "%~dp0"
powershell -ExecutionPolicy bypass -command "& { %~dpn0.ps1 %* ; exit $LASTEXITCODE }" 
set BANGERROR=!ERRORLEVEL!
popd
exit /b !BANGERROR!

