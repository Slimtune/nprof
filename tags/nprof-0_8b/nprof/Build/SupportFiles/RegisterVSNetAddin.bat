@echo off
echo Registering nprof DLL... 
Setup\regasm /silent /codebase bin/nprof.vsnetaddin.dll 2> %TEMP%\nprof-regasm-output.txt
if errorlevel 1 (
	echo.
	echo === Error while registering .NET assembly ===
	type %TEMP%\nprof-regasm-output.txt 
	echo =============================================
	echo.
)
echo Registering nprof as a VS.NET add-in...
regedit /s Setup\RegisterVSNetAddin.reg
pause
