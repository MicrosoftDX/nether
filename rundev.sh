#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.



# Set environment variables for development and testing__
export ASPNETCORE_ENVIRONMENT=Development # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
export Identity__IdentityServer__RequireHttps="false";

# The following settings configure the Identity clients for integration tests
# They are also in the launchSettings.json for Nether.Web for the VS experience
export Identity__Clients__clientcredstest__Name="Test Client for client credentials flow"
export Identity__Clients__clientcredstest__AllowedGrantTypes="client_credentials"
export Identity__Clients__clientcredstest__ClientSecrets="devsecret"
export Identity__Clients__clientcredstest__AllowedScopes="openid, profile, nether-all"

export Identity__Clients__resourceownertest__AllowedGrantTypes="password"
export Identity__Clients__resourceownertest__ClientSecrets="devsecret"
export Identity__Clients__resourceownertest__AllowedScopes="nether-all"

export Identity__Clients__customgranttest__AllowedGrantTypes="fb-usertoken"
export Identity__Clients__customgranttest__ClientSecrets="devsecret"
export Identity__Clients__customgranttest__AllowedScopes="nether-all"

# ensure guest-access is allowed for 'devclient' client
export Identity__Clients__devclient__AllowedGrantTypes="password, hybrid, fb-usertoken, guest-access"

# enable guest authn
export Identity__SignInMethods__GuestAccess__Enabled="true"

# enable username+password user sign up
export Identity__SignInMethods__UsernamePassword__AllowUserSignUp="true"



pushd .
cd src/Nether.Web
dotnet watch run
popd
