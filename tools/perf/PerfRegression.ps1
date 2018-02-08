<#
Name: PerfRegression.ps1
Usage: -Workspace -CompType -BuildId -RunnerParams
CopyRight: Copyright (C) Microsoft Corporation. All rights reserved.
#>
Param
(
    [String]$Workspace = $(throw "Workspace path is required."),
    [String]$CompType = $(throw "TestType is required."), # the performance test type    
    [String]$BuildId = $(throw "BuildId is required."),
    [String]$RunnerParams
)

# In which has the base running bits
$BaseBitsPath = Join-Path $Workspace "BaseBits"
If((Test-Path $BaseBitsPath) -eq $False)
{
   throw "~/BaseBits is required."
}

# In which has the test running bits
$TestBitsPath = Join-Path $Workspace  "TestBits"
If((Test-Path $TestBitsPath) -eq $False)
{
   throw "~/TestBits is required."
}

<#
Description: Run the performance test cases
#>
Function Execute([string]$testFolder, [string]$testType, [string]$runid)
{
    $location = Get-Location
    Set-Location (Join-Path $testFolder "bin")

    $testDllName = "Microsoft.OData.Performance." + $testType + ".Tests.dll"
    $result = $runid + ".xml"
    $analysisResult = $runid + ".analysisResult.xml"
    .\xunit.performance.run.exe $testDllName -runner .\xunit.console.exe -runnerargs "-parallel none $RunnerParams" -runid $runid
    .\xunit.performance.analysis.exe $result -xml $analysisResult

    Set-Location $location
}

<#
Description: Get the RunId based on inputs, the output is similar as "Component.20170725.101"
#>
Function GetRunId([string]$testType, [string]$date, [string]$buildId)
{
    If($testType -eq "Component")
    {
        return "Component." + $date + "." + $buildId
    }
    
    If($testType -eq "Service")
    {
        return "Service." + $date + "." + $buildId
    }

    # failed.
    throw "Wrong test type input. It should be 'Component|Service'" 
}

# Step 1. Get the running date & test result name
$RunDate = Get-Date -Format yyyyMMdd
$TestRunId = GetRunId $CompType $RunDate $BuildId

# Step 2. Run the current tests
Execute $TestBitsPath $CompType $TestRunId
$TempPath = Join-Path $TestBitsPath "bin"
$TestResult = Join-Path $TempPath ($TestRunId + ".analysisResult.xml")
Move-Item -Path $TestResult -Destination (Join-Path $TestBitsPath "test.xml") -Force

# Step 3. Run the base tests
Execute $BaseBitsPath $CompType $TestRunId
$TempPath = Join-Path $BaseBitsPath "bin"
$BaseResult = Join-Path $TempPath ($TestRunId + ".analysisResult.xml")
Move-Item -Path $BaseResult -Destination (Join-Path $BaseBitsPath "base.xml") -Force
