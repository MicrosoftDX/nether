dotnet restore

Get-Content build\build-order.txt | ForEach-Object { dotnet build $_}