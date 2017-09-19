# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

$here = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Output "*** Executing tests"
$testExitCode=0
Get-Content "$here\build\integration-test-order.txt" `
    | Where-Object { $_ -ne "" } `
    | ForEach-Object { 
        Write-Output "*** dotnet test $_"
            dotnet test $_
        if ($LASTEXITCODE -ne 0){
            $testExitCode = $LASTEXITCODE
        }
    }

if($testExitCode -ne 0) {
    Write-Output ""
    Write-Output "*** Tests failed"
    exit $testExitCode
}