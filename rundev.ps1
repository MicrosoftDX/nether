# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.
$env:ASPNETCORE_ENVIRONMENT="Development" # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
Push-Location
cd src/Nether.Web
try{
    dotnet run
}
finally{
    Pop-Location
}
