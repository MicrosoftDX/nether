$baseurl = "https://raw.githubusercontent.com/MicrosoftDX/nether/serverless/src/cloud/functions/leaderboards/top-n/deploy.ps1";
$wc = New-Object System.Net.WebClient; 
$wc.DownloadString("$baseurl/functiondeploy.txt").Split()| ? { $_ } | % { $folder = Split-Path -Path $_; if (!(Test-Path $folder)) { "Creating folder $folder"; mkdir $folder | Out-Null }; "Downloading $_"; Add-Content $_ -Value $wc.DownloadString("$baseurl/$_"); }
