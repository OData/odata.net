$PROGRAMFILESX86 = [Environment]::GetFolderPath("ProgramFilesX86")
$ENLISTMENT_ROOT = Split-Path -Parent $MyInvocation.MyCommand.Definition
$MSBUILD = $PROGRAMFILESX86 + "\MSBuild\12.0\Bin\MSBuild.exe"
$MSTEST = $PROGRAMFILESX86 + "\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe"
$BUILDLOG = $ENLISTMENT_ROOT + "\BuildResult.txt"
$TESTLOG = $ENLISTMENT_ROOT + "\TestResult.txt"
$TESTDIR = $ENLISTMENT_ROOT + "\bin\AnyCPU\Debug\Test\Desktop"
$NUGETPACK = $ENLISTMENT_ROOT + "\sln\packages"
$RollingTests = "Microsoft.Test.Data.Services.DDBasics.dll" ,
       "Microsoft.OData.Client.Design.T4.UnitTests.dll",
       "AstoriaUnitTests.TDDUnitTests.dll",
       "EdmLibTests.dll",
       "Microsoft.OData.Client.TDDUnitTests.dll",
       "Microsoft.Spatial.TDDUnitTests.dll",
       "Microsoft.Test.Edm.TDD.Tests.dll",
       "Microsoft.Test.OData.TDD.Tests.dll",
       "Microsoft.Test.OData.Query.TDD.Tests.dll",
       "Microsoft.Test.Taupo.OData.Common.Tests.dll",
       "Microsoft.Test.Taupo.OData.Query.Tests.dll",
       "Microsoft.Test.Taupo.OData.Reader.Tests.dll",
       "Microsoft.Test.Taupo.OData.Writer.Tests.dll",
       "Microsoft.Test.Taupo.OData.Scenario.Tests.dll",
       "AstoriaUnitTests.ClientCSharp.dll",
       "Microsoft.Data.NamedStream.UnitTests.dll",
       "Microsoft.Data.ServerUnitTests1.UnitTests.dll",
       "Microsoft.Data.ServerUnitTests2.UnitTests.dll",
       "RegressionUnitTests.dll",
       "Microsoft.Data.MetadataObjectModel.UnitTests.dll",
       "AstoriaUnitTests.dll",
       "AstoriaClientUnitTests.dll",
       "Microsoft.Test.OData.User.Tests.dll",
       "TestCategoryAttributeCheck.dll"
$E2ETests = "Microsoft.Test.OData.Tests.Client.dll",
       "PortableTests\Microsoft.Test.OData.Tests.Client.Portable.Desktop.dll",
       "Microsoft.Test.OData.PluggableFormat.dll"
$ODataLocTests = ,"ODataLocTests.dll"

Function CleanUp
{
    Write-Host 'killing TaupoAstoriaRunner as it should no longer be running'
    taskkill /F /IM "TaupoAstoriaRunner.exe" 1>$null 2>$null

    Write-Host 'killing TaupoConsoleRunner as it should no longer be running'
    taskkill /F /IM "TaupoConsoleRunner.exe" 1>$null 2>$null

    Write-Host 'killing MSTest as it should no longer be running'
    taskkill /F /IM "MsTest.exe" 1>$null 2>$null

    Write-Host 'killing LSbuild as it should no longer be running'
    taskkill /F /IM "LSbuild.exe" 1>$null 2>$null

    Write-Host 'Stopping code coverage gathering...'
    taskkill /f /im VSPerfMon.exe 1>$null 2>$null

    Write-Host 'Killing WinHttpAutoProxySvc as it overflows due to large amount of web calls'
    taskkill /F /FI "SERVICES eq WinHttpAutoProxySvc" >$null

    net stop w3svc 1>$null 2>$null

    Write-Host 'Minimize SQLExpress memory footprint'
    net stop "SQL Server (SQLEXPRESS)"  2>$null
    net start "SQL Server (SQLEXPRESS)" 2>$null
    
    Write-Host 'Clean Done'
}
Function BuildSolution ($sln)
{
    Write-Host "*** Building $sln ***"
    $slnpath = $ENLISTMENT_ROOT + "\sln\$sln"
    & $MSBUILD $slnpath /t:rebuild /m /nr:false /fl "/p:Platform=Any CPU" /p:Desktop=true /flp:LogFile=msbuild.log /flp:Verbosity=Normal >> $BUILDLOG
    if($LASTEXITCODE -eq 0)
    {
        BuildSuccess ($sln)
    }
    else
    {
        BuildFail ($sln)
    }
}
Function BuildSuccess ($sln)
{
    Write-Host "Build $sln SUCCESS" -ForegroundColor Green
}
Function BuildFail ($sln)
{
    Write-Host "Build $sln FAILED" -ForegroundColor Red
    exit
}
Function RestoringFile ($file , $target)
{
    Write-Host "Restoring $file"
    Copy-Item -Path $file -Destination $target -Force
}
Function RunTest ($title , $testdir)
{
    Write-Host "**********Running $title***********"
    foreach ($test in $testdir)
    {
        Write-Host "Running $test"
        & $MSTEST /testcontainer:$test >> $TESTLOG
    }
}
Function TestSummary
{
    Write-Host 'Collecting test results'
    Write-Output "<Playlist Version=`"1.0`">" | Out-File $ENLISTMENT_ROOT\bas.playlist
    $file = Get-Content -Path $ENLISTMENT_ROOT\TestResult.txt
    $pass = 0
    $fail = 0
    $trxfile = New-Object -TypeName System.Collections.ArrayList
    foreach ($line in $file)
    {
        if ($line -match ".*Passed.*") 
        {
            $pass = $pass + 1
        }
        elseif ($line -match ".*Failed\s+(.*)")
        {
            $fail = $fail + 1
            $output = "<Add Test=`"" + $Matches[1] + "`" />"
            Write-Output $output  | Out-File -Append $ENLISTMENT_ROOT\bas.playlist
        }
        elseif ($line -match ".*Results file: (.*)")
        {
            [void]$trxfile.Add($Matches[1])
        }
    }
    Write-Host "Passed :`t$pass"
    Write-Host "Failed :`t$fail"
    Write-Host "---------------"
    Write-Host "Total :`t$($pass + $fail)"
    Write-Host "For more information, please open the following test result files:"
    foreach ($trx in $trxfile)
    {
        Write-Host $trx
    }
    Write-Output "</Playlist>" | Out-File -Append "$ENLISTMENT_ROOT\bas.playlist"
    Write-Host "To replay failed tests, please open the following playlist file:"
    Write-Host "$ENLISTMENT_ROOT\bas.playlist"
    rm $TESTLOG
}

CleanUp 

Write-Host '**********Start To Build The Project*********'
$BUILD_START_TIME = Get-Date
BuildSolution ('Microsoft.Odata.Full.sln')
BuildSolution ('Microsoft.Odata.E2E.sln')
$BUILD_END_TIME = Get-Date

Write-Host '**********Start To Run The Test*********'
$TEST_START_TIME = Get-Date
cd $TESTDIR
RestoringFile -file "$NUGETPACK\EntityFramework.4.3.1\lib\net40\EntityFramework.dll" -target $TESTDIR
RunTest -title 'Rolling Tests' -testdir $RollingTests
RestoringFile -file "$NUGETPACK\EntityFramework.5.0.0\lib\net40\EntityFramework.dll" -target $TESTDIR
RunTest -title 'E2E Tests' -testdir $E2ETests
RunTest -title 'ODataLocTests' -testdir $ODataLocTests
TestSummary 
$TEST_END_TIME = Get-Date

Write-Host "Build time :`t" , (New-TimeSpan $BUILD_START_TIME -end $BUILD_END_TIME).TotalSeconds , "`tseconds"
Write-Host "Test time :`t" , (New-TimeSpan $TEST_START_TIME -end $TEST_END_TIME).TotalSeconds , "`tseconds"

cd $ENLISTMENT_ROOT