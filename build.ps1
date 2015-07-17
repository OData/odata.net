if (($args.Count -eq 0) -or ($args[0] -match 'Nightly')) 
{
	$script:TestType = 'Nightly'
	$CONFIGURATION = 'Release'
}
elseif ($args[0] -match 'Rolling')
{
	$script:TestType = "Rolling"
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
$RollingTestSuite =         "Microsoft.Test.Data.Services.DDBasics.dll",
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
        "Microsoft.Test.OData.PluggableFormat.Tests.dll"
$NightlyTestSuite = $RollingTestSuite ,
        "Microsoft.Data.MetadataObjectModel.UnitTests.dll",
        "AstoriaUnitTests.dll",
        "AstoriaClientUnitTests.dll",
        "Microsoft.Test.OData.User.Tests.dll",
        "TestCategoryAttributeCheck.dll"
$E2eTestSuite = "Microsoft.Test.OData.Tests.Client.dll", 
        "PortableTests\Microsoft.Test.OData.Tests.Client.Portable.Desktop.dll"
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
}
Function CleanBeforeScorch
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
Function RunBuild ($sln)
{
    Write-Host "*** Building $sln ***"
    $slnpath = $ENLISTMENT_ROOT + "\sln\$sln"
	if ($script:TestType -eq 'Nightly')
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
Function RunTest ($title , $testdir)
{
    Write-Host "**********Running $title***********"
    foreach ($test in $testdir)
    {
        Write-Host "Running $test"
        & $MSTEST /testcontainer:$test >> $TESTLOG
		if($LASTEXITCODE -ne 0)
    	{
        	Write-Host "Run $test FAILED" -ForegroundColor Red
    		Cleanup
			exit
    	}
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
Function BuildProcess
{
	Write-Host '**********Start To Build The Project*********'
	$script:BUILD_START_TIME = Get-Date
	RunBuild ('Microsoft.Odata.Full.sln')
	RunBuild ('Microsoft.OData.Net35.sln')
	RunBuild ('Microsoft.OData.Net45.sln')
	RunBuild ('Microsoft.OData.Portable45.sln')
	RunBuild ('Microsoft.OData.CodeGen.sln')
	RunBuild ('Microsoft.Odata.E2E.sln')
	$script:BUILD_END_TIME = Get-Date
}
Function TestProcess
{
	Write-Host '**********Start To Run The Test*********'
	$script:TEST_START_TIME = Get-Date
	cd $TESTDIR
	RestoringFile -file "$NUGETPACK\EntityFramework.4.3.1\lib\net40\EntityFramework.dll" -target $TESTDIR
	if ($script:TestType -eq 'Nightly')
	{
		RunTest -title 'Nightly Test' -testdir $NightlyTestSuite
	}
	elseif ($script:TestType -eq 'Rolling')
	{
		RunTest -title 'Rolling Tests' -testdir $RollingTestSuite
	}
	else
	{
		Write-Host 'Error : TestType' -ForegroundColor Red
		Cleanup
		exit
	}
	RestoringFile -file "$NUGETPACK\EntityFramework.5.0.0\lib\net40\EntityFramework.dll" -target $TESTDIR
	RunTest -title 'E2E Tests' -testdir $E2eTestSuite
	TestSummary 
	$script:TEST_END_TIME = Get-Date
	cd $ENLISTMENT_ROOT
}
Function FxCopProcess
{
	& $FXCOP "/f:$ProductDir\Microsoft.Spatial.dll" "/o:$ENLISTMENT_ROOT\SpatialFxCopReport.xml"  $DataWebRulesOption $FxCopRulesOptions
	& $FXCOP "/f:$ProductDir\Microsoft.OData.Core.dll" "/o:$ENLISTMENT_ROOT\CoreFxCopReport.xml"  $FxCopRulesOptions
	& $FXCOP "/f:$ProductDir\Microsoft.OData.Edm.dll" "/o:$ENLISTMENT_ROOT\EdmFxCopReport.xml"  $FxCopRulesOptions
	& $FXCOP "/f:$ProductDir\Microsoft.OData.Client.dll.dll" "/o:$ENLISTMENT_ROOT\ClientFxCopReport.xml"  $DataWebRulesOption $FxCopRulesOptions
}

# Main Process

CleanBeforeScorch 
BuildProcess
TestProcess
FxCopProcess
Cleanup

Write-Host "Build time :`t" , (New-TimeSpan $script:BUILD_START_TIME -end $script:BUILD_END_TIME).TotalSeconds , "`tseconds"
Write-Host "Test time :`t" , (New-TimeSpan $script:TEST_START_TIME -end $script:TEST_END_TIME).TotalSeconds , "`tseconds"
