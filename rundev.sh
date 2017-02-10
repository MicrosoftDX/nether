#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.



# Set environment variables for development and testing__
export ASPNETCORE_ENVIRONMENT=Development # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
export Identity__IdentityServer__RequireHttps="false";

# The following settings configure the Identity clients for integration tests
# They are also in the launchSettings.json for Nether.Web for the VS experience
export Identity__Clients__clientcreds-test__Name="Test Client for client credentials flow"
export Identity__Clients__clientcreds-test__AllowedGrantTypes="client_credentials"
export Identity__Clients__clientcreds-test__ClientSecrets="devsecret"
export Identity__Clients__clientcreds-test__AllowedScopes="openid, profile, nether-all"

export Identity__Clients__resourceowner-test__AllowedGrantTypes="password"
export Identity__Clients__resourceowner-test__ClientSecrets="devsecret"
export Identity__Clients__resourceowner-test__AllowedScopes="nether-all"

export Identity__Clients__customgrant-test__AllowedGrantTypes="fb-usertoken"
export Identity__Clients__customgrant-test__ClientSecrets="devsecret"
export Identity__Clients__customgrant-test__AllowedScopes="nether-all"

pushd .
cd src/Nether.Web
dotnet watch run
popd
