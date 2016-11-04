#!/bin/bash
#
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

export ASPNETCORE_ENVIRONMENT=Development # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
pushd .
cd src/Nether.Web
dotnet run
popd
