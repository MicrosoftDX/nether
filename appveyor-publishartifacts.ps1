$netherRoot = $PSScriptRoot

Push-AppveyorArtifact src/Nether.Web/bin/Release/netcoreapp1.1/Nether.Web.zip -FileName Nether.Web.Zip -Type WebDeployPackage
Push-AppveyorArtifact commit.txt -FileName commit.txt -Type Auto


Get-Content "$netherRoot\build\build-versioned-projects.txt" `
        | Where-Object { $_ -ne "" } `
        | ForEach-Object {
    $nupkg = Get-ChildItem "$_/bin/Debug/*.nupkg"
    Push-AppveyorArtifact $nupkg.FullName -FileName $nupkg.Name -Type NuGetPackage
}