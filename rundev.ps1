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
        Write-Error "missing dependencides: $missing"
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

# Set environment variables for development and testing:
$env:ASPNETCORE_ENVIRONMENT="Development" # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services

# The following settings configure the Identity clients for integration tests
# They are also in the launchSettings.json for Nether.Web for the VS experience
${env:Identity:Clients:clientcreds-test:Name} = "Test Client for client credentials flow"
${env:Identity:Clients:clientcreds-test:AllowedGrantTypes} = "client_credentials"
${env:Identity:Clients:clientcreds-test:ClientSecrets} = "devsecret"
${env:Identity:Clients:clientcreds-test:AllowedScopes} = "openid, profile, nether-all"

${env:Identity:Clients:resourceowner-test:AllowedGrantTypes} = "password"
${env:Identity:Clients:resourceowner-test:ClientSecrets} = "devsecret"
${env:Identity:Clients:resourceowner-test:AllowedScopes} = "nether-all"

${env:Identity:Clients:customgrant-test:AllowedGrantTypes} = "fb-usertoken"
${env:Identity:Clients:customgrant-test:ClientSecrets} = "devsecret"
${env:Identity:Clients:customgrant-test:AllowedScopes} = "nether-all"

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

