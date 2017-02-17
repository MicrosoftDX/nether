Login-AzureRmAccount
Select-AzureRmSubscription -SubscriptionName "<subscription-name>" | Set-AzureRmContext

$resourceGrp = "<resource-group>"
$location = "<location>"
$scriptStorageAccount = "<storage-account-for-scripts>"
$storageAccount = "<storage-account-for-game-events>"
$container = "scripts"
$sqlServerName = "<sql-server-name>"
$sqlDBName = "gameevents"
$sqlServerAdminName = "<sql-server-login-name>"
$sqlServerAdminPassword = "<sql-server-login-password"
$deploymentName = "<deployment-name>"

$stgKey = Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGrp -Name $storageAccount

New-AzureRmStorageAccount -ResourceGroupName $resourceGrp -Name $scriptStorageAccount -SkuName "Standard_GRS" -Location $location
$scriptStgKey = Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGrp -Name $scriptStorageAccount
$ctx = New-AzureStorageContext -StorageAccountName $scriptStorageAccount -StorageAccountKey $scriptStgKey[0].Value
New-AzureStorageContainer -Name $container -Context $ctx

# Upload all duration hive scripts
$files = Get-ChildItem .\src\Nether.Analytics.HiveScripts\duration
foreach ($file in $files) {
    $blobFileName = "durations/" + $file
    Set-AzureStorageBlobContent -File $file.FullName -Container $container -Blob $blobFileName -Context $ctx
}

# Upload all game duration hive scripts
$files = Get-ChildItem .\src\Nether.Analytics.HiveScripts\gamesession
foreach ($file in $files) {
    $blobFileName = "gamesession/" + $file
    Set-AzureStorageBlobContent -File $file.FullName -Container $container -Blob $blobFileName -Context $ctx
}

# Upload all active users / sessions hive scripts
$files = Get-ChildItem .\src\Nether.Analytics.HiveScripts\activeusers
foreach ($file in $files) {
    $blobFileName = "activeusers/" + $file
    Set-AzureStorageBlobContent -File $file.FullName -Container $container -Blob $blobFileName -Context $ctx
}

# Create SQL Server
$securePassword = ConvertTo-SecureString -String $sqlServerAdminPassword -AsPlainText -Force
$serverCreds = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $sqlServerAdminName, $securePassword
$sqlServer = New-AzureRmSqlServer -ResourceGroupName $resourceGrp -ServerName $sqlServerName -Location $location -ServerVersion "12.0" -SqlAdministratorCredentials $serverCreds
New-AzureRmSqlDatabase -ResourceGroupName $resourceGrp -ServerName $sqlServerName -DatabaseName $sqlDBName

New-AzureRmSqlServerFirewallRule -ResourceGroupName $resourceGrp -ServerName $sqlServerName -FirewallRuleName "AllowAllAzureIPs" -StartIpAddress "0.0.0.0" -EndIpAddress "255.255.255.255"

# Create tables in the Azure SQL DB
# TO DO
sqlcmd -U $sqlServerAdminName@$sqlServerName -P $sqlServerAdminPassword -S $sqlServerName.database.windows.net -d $sqlDBName -i .\deployment\analytics-assets\CreateTables.sql

# Provision ARM template deployments
$parameters = @{
    "storageAccountName" = $storageAccount;
    "storageAccountKey" = $stgKey[0].Value;
    "hiveScriptStorageAccountName" = $scriptStorageAccount;
    "hiveScriptStorageAccountKey" = $scriptStgKey[0].Value;
    "sqlServerName" = $sqlServerName;
    "sqlDBName" = $sqlDBName;
    "sqlServerAdminName" = $sqlServerAdminName;
    "sqlServerAdminPassword" = $sqlServerAdminPassword
}
# ADF pipeline for active users, active sessions, game durations
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGrp -Name $deploymentName -TemplateFile .\deployment\analyticsADFactiveUsers.json -TemplateParameterObject $parameters
# ADF pipeline for generic durations
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGrp -Name $deploymentName -TemplateFile .\deployment\analyticsADFdurations.json -TemplateParameterObject $parameters