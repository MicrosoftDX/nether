param
(
    [Parameter(Mandatory=$true)]
    [string]
    $resourceGrp,

    # user Get-AzureRmLocation to get the list of valid location values
    [Parameter(Mandatory=$true)]
    [string]
    $location,

    [Parameter(Mandatory=$true)]
    [string]
    $scriptStorageAccount,

    [Parameter(Mandatory=$true)]
    [string]
    $storageAccount,

    [Parameter(Mandatory=$true)]
    [string]
    $sqlServerName,

    [Parameter(Mandatory=$true)]
    [string]
    $sqlServerAdminName,

    [Parameter(Mandatory=$true)]
    [securestring]
    $sqlServerAdminPassword,

    [Parameter(Mandatory=$true)]
    [string]
    $dataFactoryName
)
$ErrorActionPreference = "Stop";

$netherRoot = "$PSScriptRoot/.."

$container = "scripts"
$sqlDBName = "gameevents"
$stgKey = Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGrp -Name $storageAccount

# Creating a storage account to contain all Hive scripts
Write-Host
Write-Host "Creating a storage account to contain all Hive scripts"

New-AzureRmStorageAccount -ResourceGroupName $resourceGrp -Name $scriptStorageAccount -SkuName "Standard_GRS" -Location $location
$scriptStgKey = Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGrp -Name $scriptStorageAccount
$ctx = New-AzureStorageContext -StorageAccountName $scriptStorageAccount -StorageAccountKey $scriptStgKey[0].Value
New-AzureStorageContainer -Name $container -Context $ctx

# Uploading all Hive scripts to recently created storage account
Write-Host
Write-Host "Uploading all Hive scripts to recently created storage account"

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

# Upload all counts hive scripts
$files = Get-ChildItem .\src\Nether.Analytics.HiveScripts\counts
foreach ($file in $files) {
    $blobFileName = "counts/" + $file
    Set-AzureStorageBlobContent -File $file.FullName -Container $container -Blob $blobFileName -Context $ctx
}

# Create SQL Server
Write-Host
Write-Host "Creating an Azure SQL DB"

$serverCreds = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $sqlServerAdminName, $sqlServerAdminPassword
$sqlServer = New-AzureRmSqlServer -ResourceGroupName $resourceGrp -ServerName $sqlServerName -Location $location -ServerVersion "12.0" -SqlAdministratorCredentials $serverCreds
New-AzureRmSqlDatabase -ResourceGroupName $resourceGrp -ServerName $sqlServerName -DatabaseName $sqlDBName

New-AzureRmSqlServerFirewallRule -ResourceGroupName $resourceGrp -ServerName $sqlServerName -FirewallRuleName "AllowAllAzureIPs" -StartIpAddress "0.0.0.0" -EndIpAddress "255.255.255.255"

# Create tables in the Azure SQL DB
# TO DO
#sqlcmd -U $sqlServerAdminName@$sqlServerName -P $sqlServerAdminPassword -S $sqlServerName.database.windows.net -d $sqlDBName -i .\deployment\analytics-assets\CreateTables.sql

# Provision ARM template deployments
$parameters = @{
    "storageAccountName" = $storageAccount;
    "storageAccountKey" = $stgKey[0].Value;
    "hiveScriptStorageAccountName" = $scriptStorageAccount;
    "hiveScriptStorageAccountKey" = $scriptStgKey[0].Value;
    "sqlServerName" = $sqlServerName;
    "sqlDBName" = $sqlDBName;
    "sqlServerAdminName" = $sqlServerAdminName;
    "sqlServerAdminPassword" = $sqlServerAdminPassword;
    "dataFactoryName" = $dataFactoryName
}
# ADF pipeline for active users, active sessions, game durations
Write-Host
Write-Host "Deploying first Azure Data Factory to gather information on active users"
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGrp -Name "AnalyticsActiveUsers" -TemplateFile .\deployment\analyticsADFactiveUsers.json -TemplateParameterObject $parameters

# ADF pipeline for generic durations
Write-Host
Write-Host "Deploying another Azure Data Factory to gather information on generic durations, integrated in the first one"
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGrp -Name "AnalyticsDurations" -TemplateFile .\deployment\analyticsADFdurations.json -TemplateParameterObject $parameters

# ADF pipeline for generic durations
Write-Host
Write-Host "Deploying another Azure Data Factory to run aggregations on counts, integrated in the first one"
New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGrp -Name "AnalyticsCounts" -TemplateFile .\deployment\analyticsADFcounts.json -TemplateParameterObject $parameters