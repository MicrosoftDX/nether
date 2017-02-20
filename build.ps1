# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

param(
    [switch] $NoRestore
    )

$here = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet --version

if ($NoRestore) {
    Write-Output "*** Skipping package restore"
} else {
    $buildExitCode=0
    Get-Content "$here\build\build-order.txt" `
        | Where-Object { $_ -ne "" } `
        | ForEach-Object { 
            Write-Output "*** dotnet restore $_"
                dotnet restore $_
            if ($LASTEXITCODE -ne 0){
                $buildExitCode = $LASTEXITCODE
            }
        }

    if($buildExitCode -ne 0) {
        Write-Output ""
        Write-Output "*** Restore failed"
        exit $buildExitCode
    }
}

#  want to do this, but need to figure it out for xplat
# Write-Output ""
# Write-Output "*** Building solution"
# $buildExitCode=0
# dotnet msbuild .\Nether.sln
# if ($LASTEXITCODE -ne 0){
#     $buildExitCode = $LASTEXITCODE
# }

# if($buildExitCode -ne 0) {
#     Write-Output ""
#     Write-Output "*** Build failed"
#     exit $buildExitCode
# }


Write-Output ""
Write-Output "*** Building projects"
$buildExitCode=0
Get-Content "$here\build\build-order.txt" `
    | Where-Object { $_ -ne "" } `
    | ForEach-Object { 
        Write-Output "*** dotnet build $_"
            dotnet build $_
        if ($LASTEXITCODE -ne 0){
            $buildExitCode = $LASTEXITCODE
        }
    }

# Run gulp task for typescript
Set-Location .\src\Nether.Web
npm install
gulp compiletsforadminui
Set-Location ..\..

if($buildExitCode -ne 0) {
    Write-Output ""
    Write-Output "*** Build failed"
    exit $buildExitCode
}

Write-Output "*** Build completed"
