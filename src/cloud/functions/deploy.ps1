param(
    [Parameter(Mandatory = $true)]
    [string] $Group, 
    [Parameter(Mandatory = $true)]
    [string]$Scenario,
    [Parameter()]
    [string]$BaseUrl = "https://raw.githubusercontent.com/MicrosoftDX/nether/serverless/src/cloud/functions/"
)
$here = $PSScriptRoot

function EnsureTrailingSlash($url) {
    if (-not $url.EndsWith("/")) {
        $url + "/"
    }
    else {
        $url
    }
}
function DownloadScenario($url, $groupName, $scenarioName) {
    $webClient = New-Object System.Net.WebClient
    try {
        Write-Output "Determining file list for Group '$groupName', Scenario '$scenarioName'..."
        $scenarioUrl = "$url$groupName/$scenarioName"
        $files = $webClient.DownloadString("$scenarioUrl/functiondeploy.txt").Split() `
            | Where-Object { -not [string]::IsNullOrEmpty($_) }
    }
    catch {
        $e = $error[0]
        if ($e.Exception.InnerException.Response.StatusCode -eq "NotFound") {
            Write-Output "Scenario not found"
        }
        else {
            Write-Output "Error occurred: $($e.Exception.ToString())"
        }
        return
        $webClient.Dispose()
    }

    try {
        $files | ForEach-Object { 
            $filename = $_
            $folder = Split-Path -Path "$here\$filename";
            if ( -not (Test-Path $folder)) { 
                "Creating folder $folder"; mkdir $folder | Out-Null 
            }
            Write-Output "Downloading $filename..."
            $webClient.DownloadFile("$scenarioUrl/$filename", "$here/$filename")
        }
    }
    catch {
        $e = $error[0]
        Write-Output "Error occurred: $($e.Exception.ToString())"
    }
    finally {
        $webClient.Dispose()
    }
}

$BaseUrl = EnsureTrailingSlash $BaseUrl

# TODO - error handling (url not found etc)
# TODO warn if target exists
# TODO allow cherry-picking functions?

DownloadScenario $BaseUrl $Group $Scenario
    
"Deployment done!"
