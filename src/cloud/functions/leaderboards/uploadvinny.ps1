# https://github.com/vinnytheviking/nether/tree/serverless/src/cloud/functions/leaderboards/top-n


 $functionurl = "https://raw.githubusercontent.com/vinnytheviking/nether/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/function.json"
 $projeccturl = "https://raw.githubusercontent.com/vinnytheviking/nether/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/project.json"
 $runurl = "https://raw.githubusercontent.com/vinnytheviking/nether/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/run.csx"
 $sampleurl = "https://raw.githubusercontent.com/vinnytheviking/nether/serverless/src/cloud/functions/leaderboards/top-n/leaderboard/sample.dat"


#$storageDir = $pwd
        

#New-Item -Type File -Force -Path $PWD | out-null

#$wc = New-Object System.Net.WebClient

#$wc.Encoding = [System.Text.Encoding]::UTF8

 #($wc.DownloadString("$url")) | Out-File "function.json"

 $wc = New-Object System.Net.WebClient; 
 
 Add-Content -Path "$pwd\site\wwwroot\leaderboard\function.json" -Value $wc.DownloadString($functionurl);
 Add-Content -Path "$pwd\site\wwwroot\leaderboard\project.json" -Value $wc.DownloadString($projeccturl);
 Add-Content -Path "$pwd\site\wwwroot\leaderboard\run.csx" -Value $wc.DownloadString($runurl);
 Add-Content -Path "$pwd\site\wwwroot\leaderboard\sample.dat" -Value $wc.DownloadString($sampleurl)

