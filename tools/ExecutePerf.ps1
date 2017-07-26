Param
(
    [String]$Workspace = $(throw "Workspace path is required."), # The performance working space

    [String]$TestType = $(throw "TestType is required."), # the performance test type
    
    [Int]$Threshold = $(throw "Threshold is required."), # Percentage, the how many better.

    [Int]$Judgement = $(throw "Judgement is required."), # Percentage, if the percentage ups to this value, it's regression
    
    [String]$BuildId = $(throw "BuildId is required."),

    [String]$RunnerParams
)

# In which has the base running bits
$BaseBitsPath = $Workspace + "\BaseBits"

# In which has the test running bits
$TestBitsPath = $Workspace + "\TestBits" 

# OData performance test dll
If($TestType -eq "")
{
    $TestType = "Component"
}

Function AnalysisTestResult
{
    Param(        
        [string]$baseResult,
        [string]$currResult,
        [Int] $judgement,
        [Int] $threshold,
        [string]$logFile
    )

    [Xml]$currXml = Get-Content $currResult
    $currRun = $currXml.results.run
    $currTests = $currRun.test
       
    $index = 1
    $a =@()
    $exitResult = $True
    [Int]$betterNum = 0    
    foreach($currTest in $currTests)
    {
        $fullName = $currTest.name
        
        $baseTest = FindBaseTest $baseResult $fullName
        If ($baseTest -eq $null)
        {
            # "Cannot find $fullName in base" | Out-File $logFile
            #exit 1 # something bad
            throw "Cannot find $fullName in base result"
        }
        
        $info = @{}
        $info.no = $index
        $info.name = $fullName

        $currMean = $currTest.summary.Duration.mean
        $baseMean = $baseTest.summary.Duration.mean
        [decimal]$delta = [decimal]$currMean - [decimal]$baseMean

        If ($delta -lt [decimal]0)
        {
            $betterNum += 1 # Find a better one
            $delta*=[decimal](-1.0)
            $info.percentage="-"
        }

        $itemResult = "Pass"
        [decimal]$percentage = ( $delta / [decimal]$baseMean ) * 100;
        If ($percentage -gt [decimal]$judgement)
        {
            $exitResult = $False
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
        throw "Performance regression found, please see result: $logFile"
    }

    # how many better?
    $totalCount = $a.Count
    $betterPercentage = [Int](([Double]$betterNum / [Double]$totalCount) * 100)
    If($betterPercentage -gt $threshold)
    {
        UpdateBase $TestBitsPath $BaseBitsPath
    }
    #Write-Host "betterPercentage=" $betterPercentage
}

Function FindBaseTest
{
    Param(
        [string]$resultFile = $(throw "-resultFile is required."),
        [string]$testName = $(throw "-testName is required.")
    )

    [Xml]$xml = Get-Content $resultFile
    $tests = $xml.results.run.test

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

Function UpdateBase
{
    Param(
        [string]$source,
        [string]$dest
    )

    If((Test-Path $dest) -eq $True)
    {
        Remove-Item $dest -Recurse
    }

    Copy-Item $source -Destination $dest -Recurse
}

# run the performance test cases
Function Execute
{
    Param(
        [string]$testFolder,
        [string]$testType,
        [string]$runid
    )

    $location = Get-Location
    Set-Location ($testFolder + "\bin")
    #dir
    $testDllName = "Microsoft.OData.Performance." + $testType + ".Tests.dll"
    $result = $runid + ".xml"
    $analysisResult = $runid + ".analysisResult.xml"
    .\xunit.performance.run.exe $testDllName -runner .\xunit.console.exe -runnerargs "-parallel none $RunnerParams" -runid $runid
    .\xunit.performance.analysis.exe $result -xml $analysisResult

    Set-Location $location
}

Function GetRunId([string]$testType, [string]$data, [string]$buildId)
{
    If($testType -eq "Component")
    {
        return "Component." + $data + "." + $buildId
    }
    
    If($testType -eq "Service")
    {
        return "Service." + $data + "." + $buildId
    }

    # failed.
    throw "Wrong test type input. It should be '-TestType Component|Service'" 
}

# Step 0. Get the running date & test result name
$RunDate = Get-Date -Format yyyyMMdd
$TestRunId = GetRunId $TestType $RunDate $BuildId

# Step 1. Run the current tests
Execute $TestBitsPath $TestType $TestRunId
$TestResult = $TestBitsPath + "\bin\" + $TestRunId + ".analysisResult.xml"

# Step 2. if the base isnot existed, update the base
If((Test-Path $BaseBitsPath) -eq $False)
{
   # Update the "BaseBits" with "TestBits"
   UpdateBase $TestBitsPath $BaseBitsPath

   #it means we don't do anyting at the first running.
   exit 0
}

# Step 3. Run the base tests
# Remove-Item -Path ($BaseBitsPath + "\bin\") -Include *.etl
# Remove-Item -Path ($BaseBitsPath + "\bin\") -Include Performance*.xml
# Remove-Item -Path ($BaseBitsPath + "\bin\") -Include Performance*.xml
Execute $BaseBitsPath $TestType $TestRunId
$BaseResult = $BaseBitsPath + "\bin\" + $TestRunId + ".analysisResult.xml"

# Step 4. Compare the result
$LogFolder = $Workspace + "\Logs"
$LogFileName = $TestRunId + ".log"
$LogFilePath = $LogFolder + "\" + $LogFileName
If((Test-Path $LogFilePath ) -eq $False)
{
    New-Item -Path $LogFolder -name $LogFileName -ItemType File
}

AnalysisTestResult $BaseResult $TestResult $Threshold $Judgement $LogFilePath
