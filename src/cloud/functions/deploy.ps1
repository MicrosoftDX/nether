param(
    [Parameter(Mandatory = $true)]
    [string] $Group, 
    [Parameter(Mandatory = $true)]
    [string]$Scenario
)

$here = $PSScriptRoot

# check group exists
$path = "$here\App_Data\$Group"
if (-Not (Test-Path $path)) {
    Write-Output "Group '$group' not found"
    exit
}
# check scenario exists
$path = "$path\$Scenario"
if (-Not (Test-Path $path)) {
    Write-Output "Scenario '$Scenario' not found"
    exit
}

# TODO warn if target exists
# TODO allow cherry-picking functions?
Get-Content "$path\functiondeploy.txt" `
    | Where-Object { -not [string]::IsNullOrEmpty($_) } `
    | ForEach-Object {
        $folder = Split-Path "$here\$_"
        if (-not(Test-Path($folder))){
            New-Item -Path $folder -ItemType Directory | out-null
        }
        Copy-Item "$path\$_" "$here\$_"
    }
