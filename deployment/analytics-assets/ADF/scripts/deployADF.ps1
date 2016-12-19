#Note: set the variables according to your Azure environment

Login-AzureRmAccount
Get-AzureRmSubscription -SubscriptionName "Internal Consumption" | Set-AzureRmContext

$resourceGrp = "oldknether"
$dfName = "oldknether"

New-AzureRmDataFactory -ResourceGroupName $resourceGrp -Name $dfName -Location "North Europe"
$df = Get-AzureRmDataFactory -ResourceGroupName $resourceGrp -Name $dfName

#Linked Services
New-AzureRmDataFactoryLinkedService $df -File ..\LinkedServices\storageLinkedService.json
New-AzureRmDataFactoryLinkedService $df -File ..\LinkedServices\sqlLinkedService.json
New-AzureRmDataFactoryLinkedService $df -File ..\LinkedServices\HDIonDemandLinkedService.json

#Datasets
$files = Get-ChildItem ..\Datasets
foreach ($file in $files) {
    New-AzureRmDataFactoryDataset $df -File $file.FullName
}

New-AzureRmDataFactoryPipeline $df -File ..\Pipelines\calcKPIs-sliding-oldk.json
New-AzureRmDataFactoryPipeline $df -File ..\Pipelines\copyDAU.json
New-AzureRmDataFactoryPipeline $df -File ..\Pipelines\copyMAU.json
New-AzureRmDataFactoryPipeline $df -File ..\Pipelines\copyDAUtoSQL.json
New-AzureRmDataFactoryPipeline $df -File ..\Pipelines\copyMAUtoSQL.json

#Pipelines
$files = Get-ChildItem ..\Pipelines
foreach ($file in $files) {
    New-AzureRmDataFactoryPipeline $df -File $file.FullName
}

Remove-AzureRmDataFactory -ResourceGroupName $resourceGrp -Name $dfName