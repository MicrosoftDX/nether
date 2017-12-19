$here = $PSScriptRoot
$functionRootPath = "$here"
$functionContentParentPath = "$here\..\..\..\packaged-content"
$functionContentRootPath = "$functionContentParentPath\root"

function ResetOutputDirectory() {
    if (Test-Path $functionContentParentPath) {
        Remove-Item $functionContentParentPath -Recurse -Force
    }
    New-Item $functionContentRootPath -ItemType Directory | out-null
    Set-Content "$functionContentParentPath\.gitignore" "**/**" # avoid accidentally committing this build output to source control :-)
}
function CopyDeployScript() {
    Copy-Item "$functionRootPath\deploy.ps1" "$functionContentRootPath\deploy.ps1"
}
function CreateZip() {
    $zipPath = "$functionContentParentPath/nether-master.zip"
    if (Test-Path $zipPath) {
        Remove-Item $zipPath -Force
    }
    [Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" ) | out-null
    [System.IO.Compression.ZipFile]::CreateFromDirectory("$functionContentParentPath\root", "$zipPath", "Fastest", $false)
}


ResetOutputDirectory
CopyDeployScript

CreateZip