if (($args.Count -eq 0) -or ($args[0] -match 'Nightly')) 
{
    $TestType = 'Nightly'
    $CONFIGURATION = 'Release'
}
elseif ($args[0] -match 'Rolling')
{
    $TestType = "Rolling"
    $CONFIGURATION = 'Debug'
}
else 
{
    Write-Host 'Please choose Nightly Test or Rolling Test!' -ForegroundColor Red
    exit
}   
$PROGRAMFILESX86 = [Environment]::GetFolderPath("ProgramFilesX86")
$env:ENLISTMENT_ROOT = Split-Path -Parent $MyInvocation.MyCommand.Definition
$ENLISTMENT_ROOT = Split-Path -Parent $MyInvocation.MyCommand.Definition
$MSBUILD = $PROGRAMFILESX86 + "\MSBuild\12.0\Bin\MSBuild.exe"
$MSTEST = $PROGRAMFILESX86 + "\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe"
$FXCOPDIR = $PROGRAMFILESX86 + "\Microsoft Visual Studio 12.0\Team Tools\Static Analysis Tools\FxCop"
$FXCOP = $FXCOPDIR + "\FxCopCmd.exe"
$BUILDLOG = $ENLISTMENT_ROOT + "\BuildResult.txt"
$TESTLOG = $ENLISTMENT_ROOT + "\TestResult.txt"
$TESTDIR = $ENLISTMENT_ROOT + "\bin\AnyCPU\$CONFIGURATION\Test\Desktop"
$PRODUCTDIR = $ENLISTMENT_ROOT + "\bin\AnyCPU\$Configuration\Product\Desktop"
$NUGETPACK = $ENLISTMENT_ROOT + "\sln\packages"
$RollingTestSuite = "/testcontainer:Microsoft.Test.Data.Services.DDBasics.dll", 
        "/testcontainer:Microsoft.OData.Client.Design.T4.UnitTests.dll", 
        "/testcontainer:AstoriaUnitTests.TDDUnitTests.dll", 
        "/testcontainer:EdmLibTests.dll", 
        "/testcontainer:Microsoft.OData.Client.TDDUnitTests.dll", 
        "/testcontainer:Microsoft.Spatial.TDDUnitTests.dll", 
        "/testcontainer:Microsoft.Test.Edm.TDD.Tests.dll", 
        "/testcontainer:Microsoft.Test.OData.TDD.Tests.dll", 
        "/testcontainer:Microsoft.Test.OData.Query.TDD.Tests.dll", 
        "/testcontainer:Microsoft.Test.Taupo.OData.Common.Tests.dll", 
        "/testcontainer:Microsoft.Test.Taupo.OData.Query.Tests.dll", 
        "/testcontainer:Microsoft.Test.Taupo.OData.Reader.Tests.dll", 
        "/testcontainer:Microsoft.Test.Taupo.OData.Writer.Tests.dll", 
        "/testcontainer:Microsoft.Test.Taupo.OData.Scenario.Tests.dll", 
        "/testcontainer:AstoriaUnitTests.ClientCSharp.dll", 
        "/testcontainer:Microsoft.Data.NamedStream.UnitTests.dll", 
        "/testcontainer:Microsoft.Data.ServerUnitTests1.UnitTests.dll", 
        "/testcontainer:Microsoft.Data.ServerUnitTests2.UnitTests.dll", 
        "/testcontainer:RegressionUnitTests.dll", 
        "/testcontainer:Microsoft.Test.OData.PluggableFormat.Tests.dll"
$AddtionalNightlyTestSuite = "/testcontainer:Microsoft.Data.MetadataObjectModel.UnitTests.dll", 
        "/testcontainer:AstoriaUnitTests.dll", 
        "/testcontainer:AstoriaClientUnitTests.dll", 
        "/testcontainer:Microsoft.Test.OData.User.Tests.dll", 
        "/testcontainer:TestCategoryAttributeCheck.dll"
[System.Collections.ArrayList]$NightlyTestSuite=$RollingTestSuite
foreach ($test in $AddtionalNightlyTestSuite) {$null=$NightlyTestSuite.Add($test)}

$E2eTestSuite = "/testcontainer:Microsoft.Test.OData.Tests.Client.dll"
$FxCopRulesOptions = "/rule:$FxCopDir\Rules\DesignRules.dll",
        "/rule:$FxCopDir\Rules\NamingRules.dll",
        "/rule:$FxCopDir\Rules\PerformanceRules.dll",
        "/rule:$FxCopDir\Rules\SecurityRules.dll",
        "/rule:$FxCopDir\Rules\GlobalizationRules.dll",
        "/dictionary:$ENLISTMENT_ROOT\src\CustomDictionary.xml",
        "/ruleid:-Microsoft.Design#CA1006", 
        "/ruleid:-Microsoft.Design#CA1016", 
        "/ruleid:-Microsoft.Design#CA1020", 
        "/ruleid:-Microsoft.Design#CA1021", 
        "/ruleid:-Microsoft.Design#CA1045", 
        "/ruleid:-Microsoft.Design#CA2210", 
        "/ruleid:-Microsoft.Performance#CA1814"
$DataWebRulesOption = "/rule:$TESTDIR\DataWebRules.dll"


Function Cleanup 
{    
    cd $ENLISTMENT_ROOT\tools\Scripts    
    Write-Host "Dropping stale databases..."
    cscript "$ENLISTMENT_ROOT\tools\Scripts\artdbclean.js" //Nologo
    cd $ENLISTMENT_ROOT
    Write-Host "Clean Done" -ForegroundColor Yellow
}
Function CleanBeforeScorch
{
    Write-Host 'killing TaupoAstoriaRunner as it should no longer be running'
    taskkill /F /IM "TaupoAstoriaRunner.exe" 1>$null 2>$null

    Write-Host 'killing TaupoConsoleRunner as it should no longer be running'
    taskkill /F /IM "TaupoConsoleRunner.exe" 1>$null 2>$null

    Write-Host 'killing MSTest as it should no longer be running'
    taskkill /F /IM "MsTest.exe" 1>$null 2>$null

    Write-Host 'killing MSbuild as it should no longer be running'
    taskkill /F /IM "MSbuild.exe" 1>$null 2>$null

    Write-Host 'Stopping code coverage gathering...'
    taskkill /f /im VSPerfMon.exe 1>$null 2>$null

    Write-Host 'Killing WinHttpAutoProxySvc as it overflows due to large amount of web calls'
    taskkill /F /FI "SERVICES eq WinHttpAutoProxySvc" >$null

    net stop w3svc 1>$null 2>$null

    Write-Host 'Minimize SQLExpress memory footprint'
    net stop "SQL Server (SQLEXPRESS)"  2>$null
    net start "SQL Server (SQLEXPRESS)" 2>$null
    
    Write-Host "Clean Done" -ForegroundColor Yellow
}
Function RunBuild ($sln)
{
    Write-Host "*** Building $sln ***"
    $slnpath = $ENLISTMENT_ROOT + "\sln\$sln"
    if ($TestType -eq 'Nightly')
    {
        $Conf = "/p:Configuration=Release"
    }
    else
    {
        $Conf = "/p:Configuration=Debug"
    }
    & $MSBUILD $slnpath /t:rebuild /m /nr:false /fl "/p:Platform=Any CPU" $Conf /p:Desktop=true /flp:LogFile=msbuild.log /flp:Verbosity=Normal >> $BUILDLOG
    if($LASTEXITCODE -eq 0)
    {
        Write-Host "Build $sln SUCCESS" -ForegroundColor Green
    }
    else
    {
        Write-Host "Build $sln FAILED" -ForegroundColor Red
        Cleanup
        exit
    }
}
Function RestoringFile ($file , $target)
{
    Write-Host "Restoring $file"
    Copy-Item -Path $file -Destination $target -Force
}

Function TestSummary ($title)
{
    Write-Host 'Collecting test results'
    $playlist = "$ENLISTMENT_ROOT\$title" + "_bas.playlist"
    Write-Output "<Playlist Version=`"1.0`">" | Out-File $playlist
    $file = Get-Content -Path $ENLISTMENT_ROOT\TestResult.txt
    $pass = 0
    $fail = 0
    $trxfile = New-Object -TypeName System.Collections.ArrayList
    foreach ($line in $file)
    {
        if ($line -match "Passed.*") 
        {
            $pass = $pass + 1
        }
        elseif ($line -match "Failed\s+(.*)")
        {
            $fail = $fail + 1
            $output = "<Add Test=`"" + $Matches[1] + "`" />"
            Write-Output $output  | Out-File -Append $playlist
        }
        elseif ($line -match "Results file: (.*)")
        {
            [void]$trxfile.Add($Matches[1])
        }
    }
    Write-Host "The summary of $title :" -ForegroundColor Green
    Write-Host "Passed :`t$pass"  -ForegroundColor Green
    Write-Host "Failed :`t$fail"  -ForegroundColor Green
    Write-Host "---------------"  -ForegroundColor Green
    Write-Host "Total :`t$($pass + $fail)"  -ForegroundColor Green
    Write-Host "For more information, please open the following test result files:"
    foreach ($trx in $trxfile)
    {
        Write-Host $trx
    }
    Write-Output "</Playlist>" | Out-File -Append $playlist
    Write-Host "To replay failed tests, please open the following playlist file:"
    Write-Host $playlist
    if (Test-Path $TESTLOG)
    {
        rm $TESTLOG
    }
}
Function RunTest ($title , $testdir)
{
    Write-Host "**********Running $title***********"
    if (Test-Path $TESTLOG)
    {
        rm $TESTLOG
    }
    & $MSTEST $testdir >> $TESTLOG
    if($LASTEXITCODE -ne 0)
    {
        Write-Host "Run $title FAILED" -ForegroundColor Red
        Cleanup
        exit
    }
    TestSummary -title $title
}
Function BuildProcess
{
    Write-Host '**********Start To Build The Project*********'
    $script:BUILD_START_TIME = Get-Date
    if (Test-Path $BUILDLOG)
    {
        rm $BUILDLOG
    }
    RunBuild ('Microsoft.Odata.Full.sln')
    RunBuild ('Microsoft.OData.Net35.sln')
    RunBuild ('Microsoft.OData.Net45.sln')
    RunBuild ('Microsoft.OData.Portable45.sln')
    RunBuild ('Microsoft.OData.CodeGen.sln')
    RunBuild ('Microsoft.Odata.E2E.sln')
    Write-Host "Build Done" -ForegroundColor Yellow
    $script:BUILD_END_TIME = Get-Date
}
Function TestProcess
{
    Write-Host '**********Start To Run The Test*********'
    $script:TEST_START_TIME = Get-Date
    cd $TESTDIR
    RestoringFile -file "$NUGETPACK\EntityFramework.4.3.1\lib\net40\EntityFramework.dll" -target $TESTDIR
    if ($TestType -eq 'Nightly')
    {
        RunTest -title 'NightlyTests' -testdir $NightlyTestSuite
    }
    elseif ($TestType -eq 'Rolling')
    {
        RunTest -title 'RollingTests' -testdir $RollingTestSuite
    }
    else
    {
        Write-Host 'Error : TestType' -ForegroundColor Red
        Cleanup
        exit
    }
    RestoringFile -file "$NUGETPACK\EntityFramework.5.0.0\lib\net40\EntityFramework.dll" -target $TESTDIR
    RunTest -title 'E2ETests' -testdir $E2eTestSuite
    Write-Host "Test Done" -ForegroundColor Yellow
    $script:TEST_END_TIME = Get-Date
    cd $ENLISTMENT_ROOT
}
Function FxCopProcess
{
    Write-Host '**********Start To FxCop*********'
    & $FXCOP "/f:$ProductDir\Microsoft.Spatial.dll" "/o:$ENLISTMENT_ROOT\SpatialFxCopReport.xml"  $DataWebRulesOption $FxCopRulesOptions 1>$null 2>$null
    & $FXCOP "/f:$ProductDir\Microsoft.OData.Core.dll" "/o:$ENLISTMENT_ROOT\CoreFxCopReport.xml"  $FxCopRulesOptions 1>$null 2>$null
    & $FXCOP "/f:$ProductDir\Microsoft.OData.Edm.dll" "/o:$ENLISTMENT_ROOT\EdmFxCopReport.xml"  $FxCopRulesOptions 1>$null 2>$null
    & $FXCOP "/f:$ProductDir\Microsoft.OData.Client.dll" "/o:$ENLISTMENT_ROOT\ClientFxCopReport.xml"  $DataWebRulesOption $FxCopRulesOptions 1>$null 2>$null
    Write-Host "For more information, please open the following test result files:"
    Write-Host "$ENLISTMENT_ROOT\SpatialFxCopReport.xml"
    Write-Host "$ENLISTMENT_ROOT\CoreFxCopReport.xml"
    Write-Host "$ENLISTMENT_ROOT\EdmFxCopReport.xml"
    Write-Host "$ENLISTMENT_ROOT\ClientFxCopReport.xml"
    Write-Host "FxCop Done" -ForegroundColor Yellow
}

# Main Process

CleanBeforeScorch 
BuildProcess
TestProcess
FxCopProcess
Cleanup

Write-Host "Build time :`t" , (New-TimeSpan $script:BUILD_START_TIME -end $script:BUILD_END_TIME).TotalSeconds , "`tseconds"
Write-Host "Test time :`t" , (New-TimeSpan $script:TEST_START_TIME -end $script:TEST_END_TIME).TotalSeconds , "`tseconds"
