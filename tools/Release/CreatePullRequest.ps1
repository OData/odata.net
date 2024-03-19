<#
.PARAMETER versionPath
  A github Personal Access Token that has the "repo" scope. A PAT can be created [here](https://github.com/settings/tokens/new)
.PARAMETER versionPath
  The path to the msbuild props file where the version number is specified
#>

Param(
  [string]
  $githubPersonalAccessToken,
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

# create the new PR

$headers = @{
	'Accept' = 'application/vnd.github+json'
	'Authorization' = 'Bearer ' + $githubPersonalAccessToken
	'X-GitHub-Api-Version' = '2022-11-28'
}
$body = "{""title"":""$versionNumber release"",""body"":"""",""head"":""$branchName"",""base"":""main""}"

$body

$webResponse = Invoke-WebRequest -Method 'POST' -Uri https://api.github.com/repos/OData/odata.net/pulls -Headers $headers  -Body $body
if ($webResponse.StatusCode -lt 200 -or $webResponse.StatusCode -ge 300)
{
  Write-Error "An error occurred while creating the pull request:"
  Write-Error $webResponse
  Exit
}

$webResponse.Content

Write-Host
Write-Host -ForegroundColor Green "A new release branch at $branchName has been created and pushed; a PR for that branch was created at TODO:"