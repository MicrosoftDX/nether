#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

# cheeck node.js and NPM are installed
command -v nodejs >/dev/null 2>%1 || { echo >&2 "Node.js is not installed. Aborting."; exit 1;}
command -v npm >/dev/null 2>%1 || { echo >&2 "NPM is not installed. Aborting."; exit 1;}

# fix for Ubuntu (node binary is called nodejs)
ln -s /usr/bin/nodejs /usr/bin/node

# install bower
if command bower 2>/dev/null; then
    echo "bower already installed"
else
    echo "bower not present, installing..."
    npm install -g bower
fi

# install gulp
if command gulp 2>/dev/null; then
    echo "gulp already installed"
else
    echo "gulp not present, installing..."
    npm install -g gulp
fi

# Set environment variables for development and testing__
export ASPNETCORE_ENVIRONMENT=Development # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services

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
npm install
bower install
gulp npmtolib
dotnet watch run
popd
