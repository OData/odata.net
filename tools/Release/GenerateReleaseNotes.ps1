<#
.PARAMETER lastReleaseCommitId
  The path to the msbuild props file where the version number is specified
#>

Param(
  [string]
  $lastReleaseCommitId,
  [System.Version]
  $currentReleaseVersionNumber
)

Write-Host "Retrieving all commits since $lastReleaseCommitId"

$lastReleaseCommit = $lastReleaseCommitId + "..HEAD"
$commits = git log $lastReleaseCommit --pretty=format:%H

function ParseCommitDescription
{
    param
    (
        [string] $commitId
    )

    $description = git --no-pager log --format=%B -n 1 $commitId
    $description = ($description -split "`r`n")[0]

    $commitDescription = New-Object PSObject | Select-Object Description, CommitId
    $commitDescription.Description = $description
    $commitDescription.CommitId = $commitId

    return $commitDescription
}

Write-Host "Parsing commit descriptions"

$allChanges = @()
ForEach ($commit in $($commits -split "`r`n"))
{
    $allChanges += ParseCommitDescription -CommitId $commit
}

function ParsePrMetadata
{
    param
    (
        [string] $commitDescription
    )

    $prStart = $commitDescription.LastIndexOf('(')
    $prEnd = $commitDescription.LastIndexOf(')')
    if ($prStart -eq -1 -or $prEnd -eq -1)
    {
       return $null
    }

    $prMetadata = New-Object PSObject | Select-Object PrId, PrDescription
    $prMetadata.PrId = $commitDescription.Substring($prStart + 2, $prEnd - $prStart - 2)
    $prMetadata.PrDescription = $commitDescription.Substring(0, $prStart)
    
    return $prMetadata
}

Write-Host "Categorizing commits into features, bug fixes, or improvements"
Write-Host

$features = @()
$bugs = @()
$improvements = @()
ForEach ($change in $allChanges)
{
    $prMetadata = ParsePrMetadata -CommitDescription $change.Description
    
    Write-Host -NoNewLine "Found a commit with ID $($change.CommitId). "
    if ($prMetadata -eq $null)
    {
        Write-Host -ForegroundColor Yellow "Could not parse a pull request ID from the description '$($change.Description)'"
    }
    else
    {
        Write-Host "The PR can be found at https://github.com/OData/odata.net/pull/$($prMetadata.PrId)"
        Write-Host "It had a PR description of '$($prMetadata.PrDescription)'."
    }

    $category = $null
    while ($category -eq $null)
    {
        Write-Host "Is this change a feature, bug fix, or improvement, or should it be ignored? (for 'feature', enter 'feature', 'f', or '1'; for 'bug fix', enter 'bug fix', 'b', '2'; for 'improvement', enter 'improvement', 'i', or '3'; to ignore, enter '4'"
        $category = Read-Host
        switch ($category)
        {
            {($_ -eq "feature") -or ($_ -eq "f") -or ($_ -eq "1")} { $features += $prMetadata }
            {($_ -eq "bug fix") -or ($_ -eq "b") -or ($_ -eq "2")} { $bugs += $prMetadata }
            {($_ -eq "improvement") -or ($_ -eq "i") -or ($_ -eq "3")} { $improvements += $prMetadata }
            {($_ -eq "4")} { }
            default 
            { 
                Write-Host "$category is not a valid category."
                $category = $null
                Write-Host
            }
        }
    }
    
    Write-Host
}

Write-Host -ForegroundColor Green "The release notes are:"
Write-Host
Write-Host

Write-Host "## ODataLib $currentReleaseVersionNumber"
Write-Host

Write-Host "***Features***"
Write-Host

if ($features.Length -eq 0)
{
    Write-Host "N/A"
    Write-Host
}
else
{
    ForEach ($change in $features)
    {
        Write-Host "[[#$($change.PrId)]](https://github.com/OData/odata.net/pull/$($change.PrId)) $($change.PrDescription)"
        Write-Host
    }
}

Write-Host "***Fixed Bugs***"
Write-Host

if ($bugs.Length -eq 0)
{
  Write-Host "N/A"
  Write-Host
}
else
{
    ForEach ($change in $bugs)
    {
        Write-Host "[[#$($change.PrId)]](https://github.com/OData/odata.net/pull/$($change.PrId)) $($change.PrDescription)"
        Write-Host
    }
}

Write-Host "***Improvements***"
Write-Host

if ($improvements.Length -eq 0)
{
  Write-Host "N/A"
  Write-Host
}
else
{
    ForEach ($change in $improvements)
    {
        Write-Host "[[#$($change.PrId)]](https://github.com/OData/odata.net/pull/$($change.PrId)) $($change.PrDescription)"
        Write-Host
    }
}
