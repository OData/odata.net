@echo off
pushd %~dp0
setlocal

goto Test

if exist bin goto build
mkdir bin

:Build

set MSBuild="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
if not exist %MSBuild% @set MSBuild="%ProgramFiles%\MSBuild\12.0\Bin\MSBuild.exe"

set TESTLOG=BuildResult.txt

:BuildFull
echo *** Build Microsoft.Odata.Full ***

%MSBuild% sln/Microsoft.OData.Full.sln /m /nr:false  /p:Platform="Any CPU" /p:Desktop=true /v:M /fl /flp:LogFile=bin\msbuild.log;Verbosity=Normal > TESTLOG

if %ERRORLEVEL% neq 0 goto BuildFail
goto BuildE2E


:BuildE2E
echo *** Build Microsoft.Odata.E2E ***

%MSBuild% sln/Microsoft.OData.E2E.sln /m /nr:false  /p:Platform="Any CPU" /p:Desktop=true /v:M /fl /flp:LogFile=bin\msbuild.log;Verbosity=Normal >> TESTLOG

if %ERRORLEVEL% neq 0 goto BuildFail
goto BuildSuccess


:BuildFail
echo.
echo *** BUILD FAILED ***
goto Test

:BuildSuccess
echo.
echo **** BUILD SUCCESSFUL ***
goto Test

:Test
set NUGETPACK=%~dp0\sln\packages
pushd %~dp0bin\AnyCPU\Debug\Test\Desktop\
setlocal

echo ************starting to run test**********

copy /y %NUGETPACK%\EntityFramework.4.3.1\lib\net40\EntityFramework.dll

set MSTest="%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe"

%MSTest% /testcontainer:Microsoft.Test.Data.Services.DDBasics.dll  ^
       /testcontainer:Microsoft.OData.Client.Design.T4.UnitTests.dll ^
       /testcontainer:AstoriaUnitTests.TDDUnitTests.dll ^
       /testcontainer:EdmLibTests.dll ^
       /testcontainer:Microsoft.OData.Client.TDDUnitTests.dll ^
       /testcontainer:Microsoft.Spatial.TDDUnitTests.dll ^
       /testcontainer:Microsoft.Test.Edm.TDD.Tests.dll ^
       /testcontainer:Microsoft.Test.OData.TDD.Tests.dll ^
       /testcontainer:Microsoft.Test.OData.Query.TDD.Tests.dll ^
       /testcontainer:Microsoft.Test.Taupo.OData.Common.Tests.dll ^
       /testcontainer:Microsoft.Test.Taupo.OData.Query.Tests.dll ^
       /testcontainer:Microsoft.Test.Taupo.OData.Reader.Tests.dll ^
       /testcontainer:Microsoft.Test.Taupo.OData.Writer.Tests.dll ^
       /testcontainer:Microsoft.Test.Taupo.OData.Scenario.Tests.dll ^
       /testcontainer:AstoriaUnitTests.ClientCSharp.dll ^
       /testcontainer:Microsoft.Data.NamedStream.UnitTests.dll ^
       /testcontainer:Microsoft.Data.ServerUnitTests1.UnitTests.dll ^
       /testcontainer:Microsoft.Data.ServerUnitTests2.UnitTests.dll ^
       /testcontainer:RegressionUnitTests.dll ^
       /testcontainer:Microsoft.Data.MetadataObjectModel.UnitTests.dll ^
       /testcontainer:AstoriaUnitTests.dll ^
       /testcontainer:AstoriaClientUnitTests.dll ^
       /testcontainer:Microsoft.Test.OData.User.Tests.dll ^
       /testcontainer:TestCategoryAttributeCheck.dll 
  
pause>nul

popd