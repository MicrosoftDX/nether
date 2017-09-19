# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.


# Set environment variables for development and testing:
$env:ASPNETCORE_ENVIRONMENT="Development" # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
${env:Identity:IdentityServer:RequireHttps} = "false"

# The following settings configure the Identity clients for integration tests
# They are also in the launchSettings.json for Nether.Web for the VS experience
${env:Identity:Clients:clientcredstest:Name} = "Test Client for client credentials flow"
${env:Identity:Clients:clientcredstest:AllowedGrantTypes} = "client_credentials"
${env:Identity:Clients:clientcredstest:ClientSecrets} = "devsecret"
${env:Identity:Clients:clientcredstest:AllowedScopes} = "openid, profile, nether-all"

${env:Identity:Clients:resourceownertest:AllowedGrantTypes} = "password"
${env:Identity:Clients:resourceownertest:ClientSecrets} = "devsecret"
${env:Identity:Clients:resourceownertest:AllowedScopes} = "nether-all"

${env:Identity:Clients:customgranttest:AllowedGrantTypes} = "fb-usertoken"
${env:Identity:Clients:customgranttest:ClientSecrets} = "devsecret"
${env:Identity:Clients:customgranttest:AllowedScopes} = "nether-all"

# ensure guest-access is allowed for 'devclient' client
${env:Identity:Clients:devclient:AllowedGrantTypes} = "password, hybrid, fb-usertoken, guest-access";

# enable guest authn
${env:Identity:SignInMethods:GuestAccess:Enabled} = "true";

# enable username+password user sign up
${env:Identity:SignInMethods:UsernamePassword:AllowUserSignUp} = "true";

Push-Location
Set-Location src/Nether.Web
try{
    dotnet watch run
}
finally{
    Pop-Location
}

