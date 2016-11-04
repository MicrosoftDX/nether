$env:ASPNETCORE_ENVIRONMENT="Development" # Set ASP.NET Core environment to Development to enable dev logging, and other dev-only services
Push-Location
cd src/Nether.Web
try{
    dotnet run
}
finally{
    Pop-Location
}
