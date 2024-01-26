<#
.PARAMETER versionPath
  The path to the msbuild props file where the version number is specified
#>

Param(
  [string]
  $versionPath
)

if ($versionPath -eq "")
{
  $versionPath = "$PSScriptRoot\..\CustomMSBuild\Versioning.props"
}

$versions = New-Object xml
$versions.PreserveWhitespace = $true
$versions.Load($versionPath)
foreach ($propertyGroup in $versions.Project.PropertyGroup)
{
  if ($propertyGroup.VersionRelease -ne $null)
  {
    $versionNumber = $propertyGroup.VersionMajor.'#text' + "." + $propertyGroup.VersionMinor.'#text' + "." + $propertyGroup.VersionBuildNumber.'#text'

    break;
  }
}

if ($versionNumber -eq $null)
{
  Write-Error "No version number was found in $versionPath."
  Exit
}

$branchName = "releases/$versionNumber"

$branchName

git checkout -b $branchName

git add *
git commit -m "revving version number to $versionNumber and updating PublicAPI.Shipped.txt files to reflect API changes for this release"

git push --set-upstream origin $branchName

Write-Host
Write-Host -ForegroundColor Green "A new release branch at $branchName has been created and pushed; create a PR for that branch by navigating to:"
Write-Host -ForegroundColor Green "https://github.com/OData/odata.net/compare/main...$branchName"