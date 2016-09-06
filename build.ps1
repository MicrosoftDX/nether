dotnet --version

dotnet restore

Get-Content build\build-order.txt | ForEach-Object { dotnet build $_}

#$testExitCode = 0
#Get-Content build\test-order.txt | ForEach-Object { 
#    dotnet test $_
#    if ($LASTEXITCODE -ne 0){
#        $testExitCode = $LASTEXITCODE
#    }
#}
#
#exit $testExitCode
