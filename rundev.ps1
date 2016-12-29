# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

function CheckDependencies()
{
    $missing = @()

    if((Get-Command "node" -ErrorAction SilentlyContinue) -eq $null)
    {
        $missing += "Node.js is not installed"
    }

    if((Get-Command "npm" -ErrorAction SilentlyContinue) -eq $null)
    {
        $missing += "NPM is not installed"
    }

    if($missing.Length -gt 0)
    {
        Write-Error "missing dependencides"
        $missing
        exit
    }

    if((Get-Command "bower" -ErrorAction SilentlyContinue) -eq $null)
    {
        Write-Host "installing bower..."
        npm install -g bower
    }

    if((Get-Command "gulp" -ErrorAction SilentlyContinue) -eq $null)
    {
        Write-Host "installing gulp..."
        npm install -g gulp
    }
}

function PrepareWebEnvironment
{
    npm install
    bower install
    gulp npmtolib
}

$env:ASPNETCORE_ENVIRONMENT="Development" # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
Push-Location
cd src/Nether.Web
try{
    CheckDependencies
    PrepareWebEnvironment

    dotnet watch run
}
finally{
    Pop-Location
}

