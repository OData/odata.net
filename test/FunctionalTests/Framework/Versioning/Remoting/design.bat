@echo off
setlocal

:: find .NET framework version
:: change current directory
:: generate proxy classes using datasvcutil.exe

:: parameters: %1 - path to datasvcutil.exe, %2,%3,%4 - arguments for datasvcutil.exe

cd /D %systemdrive%\inetpub\wwwroot\RemoteDesign
cd > design.log

for /D %%i in (%1*) do (

    for %%j in (1 2 3 4 5) do (

		echo "%%i\datasvcutil.exe" %2 %3 %4 >> design.log
		"%%i\datasvcutil.exe" %2 %3 %4 >> design.log 2>&1
	
		if not errorlevel 1 exit
		timeout /t 3		
	)

	echo error > design.err
	dir >> design.log
	exit
)
echo datasvcutil.exe not found in %1 ! >> design.err
