$here = $PSScriptRoot
$functionRootPath = "$here"
$functionContentParentPath = "$here\..\..\..\packaged-content"
$functionContentRootPath = "$functionContentParentPath\root\App_Data"

function GetScenarios() {
    Get-ChildItem $functionRootPath -Directory `
        | ForEach-Object { @{ "Group" = $PSItem} } `
        | ForEach-Object {
        $group = $PSItem.Group
        Get-ChildItem "$functionRootPath\$group" | ForEach-Object { [PSCustomObject]@{ "Group" = $group; "Name" = $PSItem}}
    }
}
function ResetOutputDirectory() {
    if (Test-Path $functionContentParentPath) {
        Remove-Item $functionContentParentPath -Recurse -Force
    }
    New-Item $functionContentRootPath -ItemType Directory | out-null
    Set-Content "$functionContentParentPath\.gitignore" "**/**" # avoid accidentally committing this build output to source control :-)
}
function CopyScenarios($scenariosToCopy) {
    $scenariosToCopy | ForEach-Object {
        $scenario = $PSItem
        Copy-Item "$functionRootPath\$($scenario.Group)\$($scenario.Name)" "$functionContentRootPath\$($scenario.Group)\$($scenario.Name)" -Recurse
    }
}
function CopyDeployScript() {
    Copy-Item "$functionRootPath\deploy.ps1" "$functionContentParentPath\root\deploy.ps1"
}
function CreateZip() {
    $zipPath = "$functionContentParentPath/nether-serverless.zip"
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }
    [Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" ) | out-null
    [System.IO.Compression.ZipFile]::CreateFromDirectory("$functionContentParentPath\root", "$zipPath", "Fastest", $false)
}


ResetOutputDirectory
$scenarios = GetScenarios
CopyScenarios $scenarios
CopyDeployScript

CreateZip