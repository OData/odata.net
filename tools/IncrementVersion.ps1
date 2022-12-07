<#
.PARAMETER versionPath
  The path to the msbuild props file where the version number is specified
.PARAMETER lastReleaseCommit
  The ID of the last commit to be released
.PARAMETER forceMinorIncrement
  Whether to force an increment of the minor version number, regardless of API changes since the last release
.PARAMETER forceRevisionIncrement
  Whether to force an increment of the revision version number, regardless of API changes since the last release
#>

Param(
  [string]
  $versionPath,
  [string]
  $lastReleaseCommit,
  [Switch]
  $forceMinorIncrement,
  [Switch]
  $forceRevisionIncrement
)

if ($versionPath -eq "")
{
  $versionPath = "$PSScriptRoot\CustomMSBuild\Versioning.props"
}

if ($lastReleaseCommit -eq "")
{
  # use git to find the commit ID of the last change to the versioning.props file; we are assuming that this file is only changed to increment version numbers during release
  $lastReleaseCommit = git log -n 1 --pretty=format:%H -- $versionPath
}

Write-Host "The last release's commit was $lastReleaseCommit"

# use git to find the list of file paths that have been changed since the last commit
$changedFiles = git diff --name-only HEAD $lastReleaseCommit

$apiChanges = $false
$breaks = @()
# for each changed file
ForEach ($line in $($changedFiles -split [System.Environment]::NewLine))
{
  $fileName = [System.IO.Path]::GetFileName($line)
  if ($fileName -eq "PublicAPI.Shipped.txt")
  {
    # if that file represents a breaking API change, add it to the list so we can use it later
    $breaks += $line
  }
  elseif ($fileName -eq "PublicAPI.Unshipped.txt")
  {
    # if that file represents an API change that isn't breaking, go ahead and move the API changes to the shipped file for future releases
    Write-Host "There are API changes in $line. Updating the associated PublicAPI.Shipped.txt"
    $unshipped = Join-Path -Path $PSScriptRoot -ChildPath ..\$line
    $shipped = [System.IO.Path]::GetDirectoryName($unshipped) + "\PublicAPI.Shipped.txt"

    $combined = Get-Content $unshipped,$shipped
    $combined | Set-Content $shipped
    Clear-Content $unshipped

    $apiChanges = $true
  }
}

if ($breaks.Length -gt 0 -and -not $forceMinorIncrement -and -not $forceRevisionIncrement)
{
  # if there is a break and we aren't being forced to incrment, inform the caller of the breaks; they should perform the version number change manually
  Write-Error "There is a breaking change in $($breaks[0]). Please update $versionPath manually"
  Exit
}

# open the versioning.props file and find the property group that contains the version numbers
$versions = New-Object xml
$versions.PreserveWhitespace = $true
$versions.Load($versionPath)
foreach ($propertyGroup in $versions.Project.PropertyGroup)
{
  if ($propertyGroup.VersionRelease -ne $null)
  {
    # if there are api changes or the caller requested a minor version increment
    if ($apiChanges -or $forceMinorIncrement)
    {
      [int] $currentVersion = $propertyGroup.VersionMinor.'#text';
      $currentVersion = $currentVersion + 1
      Write-Host "Incrementing the VersionMinor in $versionPath to $currentVersion"

      $propertyGroup.VersionMinor.'#text' = [string] $currentVersion
      $propertyGroup.VersionBuildNumber.'#text' = '0'
    }

    # if there are not api changes or the caller requested a revision version increment
    if ((-not $apiChanges -and $breaks.Length -eq 0) -or $forceRevisionIncrement)
    {
      [int] $currentVersion = $propertyGroup.VersionBuildNumber.'#text';
      $currentVersion = $currentVersion + 1
      Write-Host "Incrementing the VersionBuildNumber in $versionPath to $currentVersion"

      $propertyGroup.VersionBuildNumber.'#text' = [string] $currentVersion
    }

    break;
  }
}

$versions.Save($versionPath)