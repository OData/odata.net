Param
(
    [String]
    $Config = "Debug"
)

If("Debug","Release" -notcontains $Config)
{
    Write-Host "Usage: Performancebuild.ps1 -Config Debug|Release" -ForegroundColor Red
    exit
}

$ProgramFilesX86 = [Environment]::GetFolderPath("ProgramFilesX86")
$EnlistmentRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$LogDir = $EnlistmentRoot + "\bin"
$Msbuild= $ProgramFilesX86 + "\MSBuild\12.0\Bin\MSBuild.exe"
$TestDir = $EnlistmentRoot  + "\bin\AnyCPU\$Config\Test\Desktop\Performance\bin"
$PackagesPath = $EnlistmentRoot  + "\sln\packages\"

$Item = Get-ChildItem -Filter xunit.performance.run.exe -Recurse -path $PackagesPath
$PerfRunPath = $Item.FullName

$Item = Get-ChildItem -Filter xunit.performance.analysis.exe -Recurse -path $PackagesPath
$PerfAnalysisPath = $Item.FullName

$Item = Get-ChildItem -Filter xunit.console.exe -Recurse -path $PackagesPath
$XunitConsoleRunnerPath = $Item.FullName

Function RunBuild ($sln, $type, $conf)
{
    Write-Host "*** Building $sln ***"
    $slnpath = $EnlistmentRoot + "\sln\$sln"
    & $Msbuild $slnpath /t:rebuild /m /nr:false /fl "/p:Platform=Any CPU" /p:Configuration=$conf /p:Desktop=true /flp:LogFile=$LogDir/msbuild.log /flp:Verbosity=Normal 1>$null
    if($LASTEXITCODE -eq 0)
    {
        Write-Host "Build $sln SUCCESS" -ForegroundColor Green
        write-host "`n"
    }
    else
    {
        Write-Host "Build $sln FAILED" -ForegroundColor Red
        Write-Host "For more information, please open the following test result files:"
        Write-Host "$LogDir\msbuild.log"
        exit
    }
}

Function ExecuteTests
{
    Param(
        [string]$testFolder,
        [string]$perfRunPath,
        [string]$perfAnalysisPath,
        [string]$xunitConsoleRunnerPath
    )

    $location = Get-Location
    Set-Location $testFolder
    $testDlls = Get-ChildItem -Filter Microsoft.OData.Performance.*.Tests.dll
    $time = Get-Date -Format yyyyMMdd.hhmmss

    foreach ($dll in $testDlls)
    {
        $dllName = $dll.Name; 
        Write-Host "*** Run test for $dllName ***"
        $rawName = $dllName.Replace("Microsoft.OData.Performance.","")
        $rawName = $rawName.Replace(".Tests.dll", "")
        $runid = $rawName + "." + $time
        $result = $runid + ".xml"
        $analysisResult = $runid + ".analysisResult.xml"
        $resultPath = $testfolder + "\" + $analysisResult
        &$perfRunPath $dll.Name -runner $xunitConsoleRunnerPath -runnerargs "-parallel none" -runid $runid
        &$perfAnalysisPath $result -xml $analysisResult
        Write-Host "See result for $dllName in $resultPath"
    }

    Set-Location $location
}

RunBuild 'OData.Tests.Performance.sln' $TestType $Config

ExecuteTests $TestDir $PerfRunPath $PerfAnalysisPath $XunitConsoleRunnerPath