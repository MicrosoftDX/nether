# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

param(
    [switch] $NoDotNetRestore,
    [switch] $NoWebClientRestore
)

$here = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet --version

function CheckDependencies() {
    $buildExitCode=0
    $missing = @()

    if((Get-Command "node" -ErrorAction SilentlyContinue) -eq $null) {
        $missing += "Node.js is not installed"
    }
    if((Get-Command "npm" -ErrorAction SilentlyContinue) -eq $null) {
        $missing += "NPM is not installed"
    }
    if((Get-Command "bower" -ErrorAction SilentlyContinue) -eq $null) {
        $missing += "bower is not installed: run 'npm install -g bower'"
    }
    if((Get-Command "gulp" -ErrorAction SilentlyContinue) -eq $null) {
        $missing += "gulp is not installed: run 'npm install -g bower'"
    }

    if($missing.Length -gt 0) {
        Write-Error "missing dependencides: $missing"
        exit 123
    }
}



if ($NoDotNetRestore) {
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

if ($NoWebClientRestore) {
    Write-Output "*** Skipping npm/bower install"
} else {
    $buildExitCode=0
    Write-Output "*** CheckDependencies..."
    CheckDependencies

    Push-Location
    Set-Location src/Nether.Web
    try{
        Write-Output "*** npm install ..."
        npm install
        if ($LASTEXITCODE -ne 0){
            $buildExitCode = $LASTEXITCODE
            Write-Output "*** npm install failed"
            exit $buildExitCode
        }

        Write-Output "*** bower install ..."
        bower install
        if ($LASTEXITCODE -ne 0){
            $buildExitCode = $LASTEXITCODE
            Write-Output "*** bower install failed"
            exit $buildExitCode
        }

        Write-Output "*** gulp npmtolib ..."
        gulp npmtolib
        if ($LASTEXITCODE -ne 0){
            $buildExitCode = $LASTEXITCODE
            Write-Output "*** gulp npmtolib failed"
            exit $buildExitCode
        }

        Write-Output "*** gulp compiletsforadminui ..."
        gulp npmtolib
        if ($LASTEXITCODE -ne 0){
            $buildExitCode = $LASTEXITCODE
            Write-Output "*** gulp compiletsforadminui failed"
            exit $buildExitCode
        }

        Write-Output "*** done."
    }
    finally{
        Pop-Location
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

if($buildExitCode -ne 0) {
    Write-Output ""
    Write-Output "*** Build failed"
    exit $buildExitCode
}

Write-Output "*** Build completed"
