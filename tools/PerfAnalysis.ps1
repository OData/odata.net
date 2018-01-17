<#
Name: PerfAnalysis.ps1
Usage: -Workspace [workspace] -TestType Component|Service -Threshold [Percentage] -BuildId [Num] -RunnerParams [XunitRunnerParams]
CopyRight: Copyright (C) Microsoft Corporation. All rights reserved.
#>

Param
(
    [String]$Workspace = $(throw "Workspace path is required."), # The performance working space
    [String]$TestType = $(throw "TestType is required."), # the performance test type    
    [Int]$Threshold = $(throw "Threshold is required."), # Percentage, if the percentage ups to this value, it's regression
    [String]$BuildId = $(throw "BuildId is required."),
    [String]$RunnerParams
)

# In which has the base running bits
$BaseBitsPath = $Workspace + "\BaseBits"
If((Test-Path $BaseBitsPath) -eq $False)
{
   throw "~/BaseBits is required."
}

# In which has the test running bits
$TestBitsPath = $Workspace + "\TestBits" 
If((Test-Path $TestBitsPath) -eq $False)
{
   throw "~/TestBits is required."
}

# In which saves the logs files
$LogPath = $Workspace + "\Logs"
If((Test-Path $LogPath) -eq $False)
{
    New-Item -Path $Workspace -name Logs -ItemType directory
}

# Performance test type
If($TestType -eq "")
{
    $TestType = "Component"
}

<#
Description: Analysis result between test and base
#>
Function AnalysisTestResult
{
    Param(        
        [string]$baseResult,
        [string]$currResult,
        [Int] $threshold,
        [string]$logFile
    )
    # reading the test xml results
    [Xml]$baseXml = Get-Content $baseResult
    [Xml]$currXml = Get-Content $currResult
    $currRun = $currXml.results.run
    $currTests = $currRun.test
       
    $index = 1
    $a =@()
    $exitResult = $True
    foreach($currTest in $currTests)
    {
        $fullName = $currTest.name
        
        $baseTest = FindBaseTest $baseXml $fullName
        If ($baseTest -eq $null)
        {
            throw "Cannot find $fullName in base result."
        }
        
        $info = @{}
        $info.no = $index
        $info.name = $fullName

        $currMean = $currTest.summary.Duration.mean
        $baseMean = $baseTest.summary.Duration.mean
        [decimal]$delta = [decimal]$currMean - [decimal]$baseMean

        If ($delta -lt [decimal]0)
        {
            $delta*=[decimal](-1.0)
            $info.percentage="-"
        }

        $itemResult = "Pass"
        [decimal]$percentage = ( $delta / [decimal]$baseMean ) * 100;
        If ($percentage -gt [decimal]$threshold)
        {
            $exitResult = $False # It's for global
            $itemResult = "Fail"
        }
                
        $info.percentage += [string]([int]$percentage) + "%"
        $info.result = $itemResult
        $a+=$info
        $index+=1
    }

    $a | ConvertTo-json  | Out-File $logFile

    If($exitResult -eq $False)
    {
        # exit 1 # fail
        throw $a | ConvertTo-json 
    }
}

<#
Description: search the test in base result.
#>
Function FindBaseTest([Xml]$baseXml, [string]$testName)
{
    $tests = $baseXml.results.run.test

    foreach($test in $tests)
    {
        $fullName = $test.name

        If($fullName -eq $testName)
        {
            return $test
        }
    }

    return $null
}

<#
Description: Run the performance test cases
#>
Function Execute([string]$testFolder, [string]$testType, [string]$runid)
{
    $location = Get-Location
    Set-Location ($testFolder + "\bin")

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
    throw "Wrong test type input. It should be '-TestType Component|Service'" 
}

# Step 1. Get the running date & test result name
$RunDate = Get-Date -Format yyyyMMdd
$TestRunId = GetRunId $TestType $RunDate $BuildId

# Step 2. Run the current tests
Execute $TestBitsPath $TestType $TestRunId
$TestResult = $TestBitsPath + "\bin\" + $TestRunId + ".analysisResult.xml"

# Step 3. Run the base tests
Execute $BaseBitsPath $TestType $TestRunId
$BaseResult = $BaseBitsPath + "\bin\" + $TestRunId + ".analysisResult.xml"

# Step 4. Analysis the between test results and base results
$LogFileName = $TestRunId + ".log"
$LogFilePath = $LogPath + "\" + $LogFileName
If((Test-Path $LogFilePath ) -eq $False)
{
    New-Item -Path $LogPath -name $LogFileName -ItemType File
}

AnalysisTestResult $BaseResult $TestResult $Threshold $LogFilePath
