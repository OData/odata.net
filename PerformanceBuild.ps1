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
        [string]$projectPath,
        [string]$config,
        [string]$servicePath 
    )

    Write-Host "*** Running tests project $projectPath***"
    dotnet run -c $config --project $projectPath -- --filter *

    if($LASTEXITCODE -eq 0)
    {
        Write-Host "$projectPath SUCCESS" -ForegroundColor Green
        write-host "`n"
    }
    else
    {
        Write-Host "FAILED" -ForegroundColor Red
        exit
    }

}


$location = Get-Location
$testServicePath = "$location\test\PerformanceTests\Framework\TestService"
$perfTestsRoot = "$location\test\PerformanceTests"

ExecuteTests "$perfTestsRoot\ComponentTests\Microsoft.OData.Performance.ComponentTests.csproj" $Config $testServicePath
ExecuteTests "$perfTestsRoot\ServiceTests\Microsoft.OData.Performance.ServiceTests.csproj" $Config $testServicePath
