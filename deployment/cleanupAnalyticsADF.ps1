param
(
    [Parameter(Mandatory=$true)]
    [string]
    $resourceGrp,

    [Parameter(Mandatory=$true)]
    [string]
    $scriptStorageAccount,

    [Parameter(Mandatory=$true)]
    [string]
    $sqlServerName,

    [Parameter(Mandatory=$true)]
    [string]
    $dataFactoryName
)
$ErrorActionPreference = "Stop";

$sqlDbName = "gameevents"

# Delete the storage account that contains all Hive scripts
Write-Host
Write-Host "Deleting the storage account that contains all Hive scripts"
Remove-AzureRmStorageAccount -ResourceGroupName $resourceGrp -Name $scriptStorageAccount

# Delete Azure SQL DB
Write-Host
Write-Host "Deleting the Azure SQL DB"
Remove-AzureRmSqlDatabase -ResourceGroupName $resourceGrp -ServerName $sqlServerName -DatabaseName $sqlDbName
Remove-AzureRmSqlServer -ResourceGroupName $resourceGrp -ServerName $sqlServerName

# Delete Azure Data Factory
Write-Host
Write-Host "Deleting a storage account to contain all Hive scripts"
Remove-AzureRmDataFactory -ResourceGroupName $resourceGrp -Name $dataFactoryName