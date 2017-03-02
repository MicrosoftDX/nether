$netherRoot = $PSScriptRoot

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
  try { $r = Invoke-WebRequest http://localhost:5000 -ErrorAction SilentlyContinue} catch{}
  if ( ($r -ne $null) -and ($r.StatusCode -eq 200)) {
    $loaded = $true
    Write-Host "success!"
    break
  }
  Start-Sleep -Seconds 10
}

if (-not $loaded){
    exit 100
}

&"$netherRoot\run-integration-tests.ps1"