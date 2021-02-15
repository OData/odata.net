Param
(
    [String]
    $Config = "Release"
)

If("Debug","Release" -notcontains $Config)
{
    Write-Host "Usage: Performancebuild.ps1 -Config Debug|Release" -ForegroundColor Red
    exit
}


Function ExecuteTests
{
    Param(
        [string]$projectRoot,
        [string]$config,
        [string]$servicePath,
        [string]$baseName  
    )


    $exeName = "$baseName.exe"
    $exePath = "$projectRoot\bin\$config\$exeName";

    Write-Host "*** Building tests project $projectRoot***"
    dotnet build -c $config $projectRoot

    if($LASTEXITCODE -eq 0)
    {
        Write-Host "Build $projectRoot SUCCESS" -ForegroundColor Green
        write-host "`n"
    }
    else
    {
        Write-Host "Build FAILED" -ForegroundColor Red
        exit
    }

    Write-Host "Running $exePath tests...";
    &$exePath --filter * --envVars ServicePath:"$servicePath"

}


$location = Get-Location
$testServicePath = "$location\test\PerformanceTests\Framework\TestService"
$perfTestsRoot = "$location\test\PerformanceTests"

ExecuteTests "$perfTestsRoot\ComponentTests" $Config $testServicePath "Microsoft.OData.Performance.Component.Tests"
ExecuteTests "$perfTestsRoot\ServiceTests" $Config $testServicePath "Microsoft.OData.Performance.Service.Tests"
