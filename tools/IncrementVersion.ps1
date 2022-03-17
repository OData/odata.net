$versionPath = "$PSScriptRoot\CustomMSBuild\Versioning.props"

$lastReleaseCommit = git log -n 1 --pretty=format:%H -- $versionPath
Write-Host "The last release's commit was $lastReleaseCommit"

$changedFiles = git diff --name-only HEAD $lastReleaseCommit

$apiChanges = $false
ForEach ($line in $($changedFiles -split "`r`n"))
{
  $fileName = [System.IO.Path]::GetFileName($line)
  if ($fileName -eq "PublicAPI.Shipped.txt")
  {
    Write-Error "There is a breaking change in $line. Please update $versionPath manually"
    Exit
  }
  elseif ($fileName -eq "PublicAPI.Unshipped.txt")
  {
    Write-Host "There are API changes in $fileName. Updating the associated PublicAPI.Shipped.txt"
    $unshipped = Join-Path -Path $PSScriptRoot -ChildPath ..\$line
    $shipped = [System.IO.Path]::GetDirectoryName($unshipped) + "\PublicAPI.Shipped.txt"

    Copy-Item $unshipped -Destination $shipped

    $apiChanges = $true
    break;
  }
}

$versions = New-Object xml
$versions.PreserveWhitespace = $true
$versions.Load($versionPath)
foreach ($propertyGroup in $versions.Project.PropertyGroup)
{
  if ($propertyGroup.VersionRelease -ne $null)
  {
    if ($apiChanges)
    {
      [int] $currentVersion = $propertyGroup.VersionMinor.'#text';
      $currentVersion = $currentVersion + 1
      Write-Host "Because there were API changes, incrementing the VersionMinor in $versionPath to $currentVersion"

      $propertyGroup.VersionMinor.'#text' = [string] $currentVersion
    }
    else
    {
      [int] $currentVersion = $propertyGroup.VersionBuildNumber.'#text';
      $currentVersion = $currentVersion + 1
      Write-Host "Because there were no API changes, incrementing the VersionBuildNumber in $versionPath to $currentVersion"

      $propertyGroup.VersionBuildNumber.'#text' = [string] $currentVersion
    }

    break;
  }
}

$versions.Save($versionPath)