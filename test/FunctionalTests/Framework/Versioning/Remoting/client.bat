@echo off
setlocal

:: find .NET framework version
:: change current directory
:: build project using MSBUILD
:: launch client executable

:: parameters: %1 - .NET framework version, %2 - /r:assembly references, %3 - service URI

cd /D %systemdrive%\inetpub\wwwroot\RemoteClient
cd > client.log

for /D %%i in (%windir%\Microsoft.NET\Framework\v%1*) do (

    for %%j in (1 2 3 4 5) do (
	
		echo %%i\csc.exe /out:ArrowheadTestHost.exe *.cs %2 /nologo /debug+ /pdb:ArrowheadTestHost.pdb /w:2 >> client.log
		%%i\csc.exe /out:ArrowheadTestHost.exe *.cs %2 /nologo /debug+ /pdb:ArrowheadTestHost.pdb /w:2 >> client.log
		
		if not errorlevel 1 (
			timeout /t 5
			echo cmd /c .\ArrowheadTestHost.exe %3 >> client.log
			start "ArrowheadTestHost.exe" .\ArrowheadTestHost.exe %3 ^>^>run.log ^2^>^>^&1
			exit
		)
		timeout /t 3
	)
	echo error > client.err
	dir >> client.log
	exit
)
echo .NET framework %1 not found! >> client.err
