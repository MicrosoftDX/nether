$here = Split-Path -Parent $MyInvocation.MyCommand.Path

dotnet --version

$buildExitCode=0

Write-Output "*** Restoring packages"
dotnet restore
$buildExitCode = $LASTEXITCODE
if ($buildExitCode -ne 0){
    Write-Output "*** Restore failed"
    exit $buildExitCode
}


Write-Output "*** Building projects"
$buildExitCode=0
Get-Content "$here\build\build-order.txt" `
    | Where-Object { $_ -ne "" } `
    | ForEach-Object { 
        Write-Output "*** dotnet build $_"
            dotnet build $_
        if ($LASTEXITCODE -ne 0){
            $buildExitCode = $LASTEXITCODE
        }
    }

if($buildExitCode -ne 0) {
    Write-Output "*** Build failed"
    exit $buildExitCode
}

# TODO - think about how to handle this going forwards. e.g. xplat msbuild?



#$testExitCode = 0
#Get-Content build\test-order.txt | ForEach-Object { 
#    dotnet test $_
#    if ($LASTEXITCODE -ne 0){
#        $testExitCode = $LASTEXITCODE
#    }
#}
#
#exit $testExitCode
