#Note: set the variables according to your Azure environment

Login-AzureRmAccount
Get-AzureRmSubscription -SubscriptionName "<insert-azure-subscription>" | Set-AzureRmContext

$resourceGrp = "<insert-resource-group-name>"
$dfName = "<insert-data-factory-name>"

New-AzureRmDataFactory -ResourceGroupName $resourceGrp -Name $dfName -Location "North Europe"
$df = Get-AzureRmDataFactory -ResourceGroupName $resourceGrp -Name $dfName

#Linked Services
New-AzureRmDataFactoryLinkedService $df -File .\deployment\analytics-assets\ADF\LinkedServices\storageLinkedService.json
New-AzureRmDataFactoryLinkedService $df -File .\deployment\analytics-assets\ADF\LinkedServices\sqlLinkedService.json
New-AzureRmDataFactoryLinkedService $df -File .\deployment\analytics-assets\ADF\LinkedServices\HDIonDemandLinkedService.json

#Datasets
$files = Get-ChildItem .\deployment\analytics-assets\ADF\Datasets
foreach ($file in $files) {
    New-AzureRmDataFactoryDataset $df -File $file.FullName
}

New-AzureRmDataFactoryPipeline $df -File .\deployment\analytics-assets\ADF\Pipelines\calcKPIs-sliding.json
New-AzureRmDataFactoryPipeline $df -File .\deployment\analytics-assets\ADF\Pipelines\copyDAU.json
New-AzureRmDataFactoryPipeline $df -File .\deployment\analytics-assets\ADF\Pipelines\copyMAU.json
New-AzureRmDataFactoryPipeline $df -File .\deployment\analytics-assets\ADF\Pipelines\copyDAUtoSQL.json
New-AzureRmDataFactoryPipeline $df -File .\deployment\analytics-assets\ADF\Pipelines\copyMAUtoSQL.json

#Pipelines
$files = Get-ChildItem ..\Pipelines
foreach ($file in $files) {
    New-AzureRmDataFactoryPipeline $df -File $file.FullName
}

Remove-AzureRmDataFactory -ResourceGroupName $resourceGrp -Name $dfName