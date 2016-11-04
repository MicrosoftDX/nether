#!/bin/bash
export ASPNETCORE_ENVIRONMENT=Development # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
pushd .
cd src/Nether.Web
dotnet run
popd
