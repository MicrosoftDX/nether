# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.
param(
    [switch] $ForceCodeFormatterDownload
)

# $codeFormatterDownloadUri = "https://github.com/dotnet/codeformatter/releases/download/v1.0.0-alpha6/CodeFormatter.zip"
# Use temporary download location. This is for a custom build that we've made using VS2017
# See https://github.com/MicrosoftDX/nether/issues/451 for details
$codeFormatterDownloadUri = "https://netherartifacts.blob.core.windows.net/code-formatter/code-formatter.zip"
$codeFormatterDownloadLocation = "$env:TEMP\code-formatter.zip"
$codeFormatterExtractLocation = "$env:TEMP\code-formatter\"

# Check that there are no outstanding changes as we will use git status to test if CodeFormatter applied any changes
Write-Host "Checking for changes..."
$gitOutput = $(git status)
$gitHasChanges = ($gitOutput | ?{ $_.Contains("nothing to commit") } | measure).Count -eq 0 # counts lines with "nothing to commit" message
if($gitHasChanges)
{
    Write-Error "Outstanding changes to workspace (before code formatting) - aborting!"
    git status
    exit 10
}
Write-Host "No changes, continuing..."

$downloadCodeFormatter = $false
if (Test-Path "$codeFormatterExtractLocation\CodeFormatter\CodeFormatter.exe"){
    if ($ForceCodeFormatterDownload){
        Write-Host "CodeFormatter found, but ForceCodeFormatterDownload switch was passed. Deleting and re-downloading..."
        Remove-Item $codeFormatterExtractLocation -Recurse -Force
        $downloadCodeFormatter = $true
    }
}else {
    Write-Host "CodeFormatter not found, downloading..."
    $downloadCodeFormatter = $true
}
if ($downloadCodeFormatter){
    (New-Object Net.WebClient).DownloadFile($codeFormatterDownloadUri, $codeFormatterDownloadLocation)
    Expand-Archive -Path $codeFormatterDownloadLocation -DestinationPath $codeFormatterExtractLocation -Force
}

# Run CodeFormatter
Write-Host "Running CodeFormatter..."
&"$codeFormatterExtractLocation\CodeFormatter\CodeFormatter.exe" code-formatter.csproj /copyright:code-formatter.license.txt
if ($LASTEXITCODE -ne 0){
    $exitCode = $LASTEXITCODE
    Write-Error "CodeFormatter failed - exiting"
    exit $exitCode
}
    
# Check for changes, i.e. did CodeFormatter make changes?
Write-Host "Checking for changes..."
git add -A | out-null # add all files as this avoids changes due to CRLF/LF issues (providing we're set up to normalize to LF)
$gitOutput = $(git status)
$gitHasChanges = ($gitOutput | ?{ $_.Contains("nothing to commit") } | measure).Count -eq 0 # counts lines with "nothing to commit" message
if($gitHasChanges)
{
    Write-Error "Outstanding changes to workspace - aborting!"
    Write-Error "Run CodeFormatter and resubmit the pull request"
    git status
    git diff
    exit 20
}

## Check if license is included in GUI files
$pathlist = @("src\Nether.Web\wwwroot\Features")
$filter = "*.ts"
$filelist = Get-ChildItem -Filter $filter -Path $pathlist -Recurse|Select-Object -ExpandProperty FullName
foreach($file in $filelist)
{
    $content = Get-Content $file -TotalCount 2
    if (-not($content -match "LICENSE"))
    {
        Write-Host "License info missing in file " $file
    }
}
$filter = "*.html"
$filelist = Get-ChildItem -Filter $filter -Path $pathlist -Recurse|Select-Object -ExpandProperty FullName
foreach($file in $filelist)
{
    $content = Get-Content $file -TotalCount 2
    if (-not($content -match "LICENSE"))
    {
        Write-Host "License info missing in file " $file
    }
}
