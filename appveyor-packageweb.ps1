$netherRoot = $PSScriptRoot

dotnet publish "$netherRoot/src/Nether.Web" -c Release
[Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" ) | out-null
[System.IO.Compression.ZipFile]::CreateFromDirectory("$netherRoot/src/Nether.Web/bin/Release/netcoreapp1.1/publish", "$netherRoot/src/Nether.Web/bin/Release/netcoreapp1.1/Nether.Web.zip", "Fastest", $false)
