# https://github.com/vinnytheviking/nether/tree/serverless/src/cloud/functions/leaderboards/top-n


 $url = "https://raw.githubusercontent.com/vinnytheviking/nether/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/function.json"

 # https://raw.githubusercontent.com/vinnytheviking/nether/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/function.json
 # https://github.com/vinnytheviking/nether/blob/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/project.json
 # https://github.com/vinnytheviking/nether/blob/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/run.csx
 # https://github.com/vinnytheviking/nether/blob/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/sample.dat

#$storageDir = $pwd
        

#New-Item -Type File -Force -Path $PWD | out-null

#$wc = New-Object System.Net.WebClient

#$wc.Encoding = [System.Text.Encoding]::UTF8

 #($wc.DownloadString("$url")) | Out-File "function.json"

 $wc = New-Object System.Net.WebClient; New-Item -Type Directory -Force -Path "top-n" | out-null;  Add-Content -Path "$pwd\top-n\vinny.json" -Value $wc.DownloadString("https://raw.githubusercontent.com/vinnytheviking/nether/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/function.json")

