$netherRoot = $PSScriptRoot

function RunIntegrationTests() {
    # launch Nether.Web
    Start-Process powershell "$netherRoot\rundev.ps1"

    # test for 200 OK from GET http://localhost:5000
    # with a retry window of 300 seconds 
    $maximumRetryInSeconds = 300
    $startTime = Get-Date
    $r = $null
    $loaded = $false
    while (((Get-Date) - $startTime).TotalSeconds -le $maximumRetryInSeconds) {
        Write-Host "testing http://localhost:5000"
        try { $r = Invoke-WebRequest http://localhost:5000 -ErrorAction SilentlyContinue} catch {}
        if ( ($r -ne $null) -and ($r.StatusCode -eq 200)) {
            $loaded = $true
            Write-Host "success!"
            break
        }
        Start-Sleep -Seconds 10
    }

    if (-not $loaded) {
        exit 100
    }

    &"$netherRoot\run-integration-tests.ps1"
    if ($LASTEXITCODE -ne 0) {
        $testExitCode = $LASTEXITCODE
        Write-Host "Integration tests failed: $testExitCode"
        exit $testExitCode
    }
}

function ConfigureSqlServer() {
    # create nether db
    sqlcmd -S "(local)\SQL2016" -Q "Use [master]; CREATE DATABASE [nether]"

    ${env:Identity:Store:wellknown} = "sql"
    ${env:Identity:Store:properties:ConnectionString} = "Server=(local)\SQL2016;Database=nether;User ID=sa;Password=Password12!"

    ${env:Leaderboard:Store:wellknown} = "sql"
    ${env:Leaderboard:Store:properties:ConnectionString} = "Server=(local)\SQL2016;Database=nether;User ID=sa;Password=Password12!"

    ${env:PlayerManagement:Store:wellknown} = "sql"
    ${env:PlayerManagement:Store:properties:ConnectionString} = "Server=(local)\SQL2016;Database=nether;User ID=sa;Password=Password12!"

    ${env:Analytics:Store:wellknown} = "sql"
    ${env:Analytics:Store:properties:ConnectionString} = "Server=(local)\SQL2016;Database=nether;User ID=sa;Password=Password12!"
}

Write-Host "***************************************************************************"
Write-Host " _       _                       _   _               _            _"
Write-Host "(_)_ __ | |_ ___  __ _ _ __ __ _| |_(_) ___  _ __   | |_ ___  ___| |_ ___"
Write-Host "| | '_ \| __/ _ \/ _`` | '__/ _`` | __| |/ _ \| '_ \  | __/ _ \/ __| __/ __|"
Write-Host "| | | | | ||  __/ (_| | | | (_| | |_| | (_) | | | | | ||  __/\__ \ |_\__ \"
Write-Host "|_|_| |_|\__\___|\__, |_|  \__,_|\__|_|\___/|_| |_|  \__\___||___/\__|___/"
Write-Host "                 |___/"
Write-Host "***************************************************************************"


# Run integration tests with default configuration (in-memory)
Write-Host "******************"
Write-Host "*** Running integration tests: in-memory"
RunIntegrationTests

# kill the web server (brute force - killing all dotnet processes!)
Stop-Process -Name dotnet

ConfigureSqlServer
Write-Host "******************"
Write-Host "*** Running integration tests: SQL Server"
RunIntegrationTests


