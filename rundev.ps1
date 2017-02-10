# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.


# Set environment variables for development and testing:
$env:ASPNETCORE_ENVIRONMENT="Development" # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
${env:Identity:IdentityServer:RequireHttps} = "false"

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
Set-Location src/Nether.Web
try{
    dotnet watch run
}
finally{
    Pop-Location
}

