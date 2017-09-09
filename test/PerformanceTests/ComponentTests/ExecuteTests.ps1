Param
(
    [String]
    $TestType = "Private",

    [String]
    $OdlVersion,

    [String]
    $TestFolder = $(throw "Usage: ExecuteTests.ps1 -TestType Private|Official [-OdlVersion <String>] -TestFolder <String> -BuildId <String>"),

    [String]
    $BuildId = $(throw "Usage: ExecuteTests.ps1 -TestType Private|Official [-OdlVersion <String>] -TestFolder <String> -BuildId <String>")
)

If("Private","Official" -notcontains $TestType)
{
    Write-Host "Usage: ExecuteTests.ps1 -TestType Private|Official [-OdlVersion <String>] -TestFolder <String> -BuildId <String>" -ForegroundColor Red
    exit
}

If($TestType -eq 'Official')
{
    If($OdlVersion -eq '')
    {
        Write-Host "Usage: ExecuteTests.ps1 -TestType Private|Official [-OdlVersion <String>] -TestFolder <String> -BuildId <String>" -ForegroundColor Red
        Write-Host "-OdlVersion is required when -TestType is Official"
        exit
    }
}

Function ParseResult
{
    Param(
        [string]$xmlFile = $(throw "-xmlFile is required."),
        [string]$dllName = $(throw "-dllName is required."),
        [string]$testType,
        [string]$odlVersion
    )

    [Xml]$xml = Get-Content $xmlFile

    $run = $xml.results.run

    $runid = $run.id.Replace($run.id.Split('.')[0]+".", "")  # Remove test dll name from RunId, for example, $runid=20151202.4096
    $tests = $run.test

    $conn = New-Object System.Data.SqlClient.SqlConnection("Data Source=mscxsmqk5t.database.windows.net;Initial Catalog=odataperf;Integrated Security=False;User ID=odatatest;Password=Passw0rd;")
    $conn.Open();

    foreach($test in $tests)
    {
        $duration = $test.summary.Duration
        $fullName = $test.name.Replace("Microsoft.OData.Performance.","")
        $className = $fullName.SubString(0, $fullName.LastIndexOf('.'))
        $testName = $fullName.Split(".")[-1]
        $min = $duration.min
        $mean = $duration.mean
        $max = $duration.max
        $marginOfError = $duration.marginOfError
        $stddev = $duration.stddev

        $cmd = $conn.CreateCommand()
        If($testType -eq 'Official')
        {
          $cmd.CommandText = "INSERT INTO dbo.ODataLib_Official (RunId, Name, Class, Dll, ODataLib_version, Duration_min, Duration_mean, Duration_max, Duration_marginOfError, Duration_stddev) VALUES ('$runid', '$testName', '$className', '$dllName', '$odlVersion', $min, $mean, $max, $marginOfError, $stddev)"
        }
        else
        {
            $cmd.CommandText = "INSERT INTO dbo.ODataLib_Private (RunId, Name, Class, Dll, Duration_min, Duration_mean, Duration_max, Duration_marginOfError, Duration_stddev) VALUES ('$runid', '$testName', '$className', '$dllName', $min, $mean, $max, $marginOfError, $stddev)"
        }
        $cmd.ExecuteNonQuery() > $null
        $cmd.Dispose()
    }

    $conn.Close()
}

Function Execute
{
    Param(
        [string]$testFolder = $(throw "-testFolder is required."),
        [string]$buildId = $(throw "-buildId is required."),
        [string]$testType,
        [string]$odlVersion
    )

    $location = Get-Location
    Set-Location $testFolder
    $testDlls = Get-ChildItem -Filter Microsoft.OData.Performance.*.Tests.dll
    $date = Get-Date -Format yyyyMMdd

    foreach ($dll in $testDlls)
    { 
        $rawName = $dll.Name.Replace("Microsoft.OData.Performance.","")
        $rawName = $rawName.Replace(".Tests.dll", "")
        $runid = $rawName + "." + $date + "." + $buildId
        $result = $runid + ".xml"
        $analysisResult = $runid + ".analysisResult.xml"
        .\xunit.performance.run.exe $dll.Name -runner .\xunit.console.exe -runnerargs "-parallel none" -runid $runid
        .\xunit.performance.analysis.exe $result -xml $analysisResult
        ParseResult $analysisResult $dll.Name $testType $odlVersion
    }

    Set-Location $location
}

Execute $TestFolder $BuildId $TestType $OdlVersion