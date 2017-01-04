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

export ASPNETCORE_ENVIRONMENT=Development # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
pushd .
cd src/Nether.Web
npm install
bower install
gulp npmtolib
dotnet watch run
popd
