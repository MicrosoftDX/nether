param
(
    [Parameter(Mandatory = $true)]
    [string]
    $ResourceGroupName,

    # user Get-AzureRmLocation to get the list of valid location values
    [Parameter(Mandatory = $true)]
    [string]
    $Location,

    [Parameter(Mandatory = $true)]
    [string]
    $DataLakeAnalyticsName,

    [Parameter(Mandatory = $true)]
    [string]
    $DataLakeStoreName,

    [Parameter(Mandatory = $true)]
    [string]
    $StorageAccountName,

    [Parameter(Mandatory = $true)]
    [string]
    $EventHubName,

    [Parameter(Mandatory = $true)]
    [string]
    $NamespaceName,

    [Parameter(Mandatory = $true)]
    [string]
    $AADApplicationName,

    [Parameter(Mandatory = $true)]
    [securestring]
    $AADApplicationPassword,

    [Parameter(Mandatory = $true)]
    [string]
    $SubscriptionName,

    [Parameter(Mandatory = $true)]
    [string]
    $DirectoryName
)
$ErrorActionPreference = "Stop";

$AuthorizationRule = "RootManageSharedAccessKey"
$ConsumerGroupName = "nether"

# Set correct subscription
Select-AzureRmSubscription -SubscriptionName $SubscriptionName

# Create resource group
Write-Host "Creating a resource group..." -ForegroundColor "yellow"
New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location

# Create storage account
Write-Host "Creating storage account..." -ForegroundColor "yellow"
New-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $StorageAccountName -Location $Location -SkuName Standard_GRS

# Create event hub
Write-Host "Creating event hub..." -ForegroundColor "yellow"
New-AzureRmEventHubNamespace -ResourceGroupName $ResourceGroupName -NamespaceName $NamespaceName -Location $Location
New-AzureRmEventHub -ResourceGroupName $ResourceGroupName -EventHubName $EventHubName -Location $Location -NamespaceName $NamespaceName
New-AzureRmEventHubConsumerGroup -ResourceGroupName $ResourceGroupName -NamespaceName $NamespaceName -EventHubName $EventHubName -ConsumerGroupName $ConsumerGroupName

# Create ADLA and ADLS account
# TODO check if the location is being supported or not
Write-Host "Creating data lake store and analytics account..." -ForegroundColor "yellow"
Write-Host "Checking region availability..." -ForegroundColor "yellow"
$ADLregions = "centralus", "eastus2", "northeurope"
if ($ADLregions.Contains($Location)) {
    $ADLlocation = $Location
} else {
    $centralus = "westus", "northcentralus", "southcentralus", "brazilsouth", "westcentralus"
    $eastus = "eastus", "canadacentral", "canadaeast", "japanwest", "japaneast", "australiaeast", "australiasoutheast", "koreacentral", "koreasouth"
    $europe = "eastasia", "southeastasia", "westeurope", "southindia", "centralindia", "westindia", "uksouth", "ukwest"

    if ($centralus.Contains($Location)) {
        $ADLlocation = "centralus"
    } elseif ($eastus.Contains($Location)) {
        $ADLlocation = "eastus2"
    } else {
        $ADLlocation = "northeurope"
    }
}
New-AzureRmDataLakeStoreAccount -ResourceGroupName $ResourceGroupName -Name $DataLakeStoreName -Location $ADLlocation
Write-Host "Creating data lake analytics account..." -ForegroundColor "yellow"
New-AzureRmDataLakeAnalyticsAccount -ResourceGroupName $ResourceGroupName -Name $DataLakeAnalyticsName -Location $ADLlocation -DefaultDataLakeStore $DataLakeStoreName

# Create service principal
Write-Host "Creating Azure Active Directory application..." -ForegroundColor "yellow"
$ServicePrincipal = New-AzureRmADServicePrincipal -DisplayName $AADApplicationName -Password $AADApplicationPassword
$Scope = (Get-AzureRmResourceGroup -Name $ResourceGroupName -ErrorAction Stop).ResourceId
# Add some wait or while loop
Sleep 50
New-AzureRmRoleAssignment -RoleDefinitionName Owner -ServicePrincipalName $ServicePrincipal.ApplicationId -Scope $Scope
$ClientId = $ServicePrincipal.ApplicationId


# Set environment variables
Write-Host "Setting environment variables..." -ForegroundColor "yellow"
$StorageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $ResourceGroupName -Name $StorageAccountName).Value[0]
$env:NAH_EHLISTENER_CONNECTIONSTRING = (Get-AzureRmEventHubNamespaceKey -ResourceGroupName $ResourceGroupName -NamespaceName $NamespaceName -AuthorizationRuleName $AuthorizationRule).PrimaryConnectionString
$env:NAH_EHLISTENER_EVENTHUBPATH = $EventHubName
$env:NAH_EHLISTENER_CONSUMERGROUP = $ConsumerGroupName
$env:NAH_EHLISTENER_STORAGECONNECTIONSTRING = "DefaultEndpointsProtocol=https;AccountName=" + $StorageAccountName + ";AccountKey=" + $StorageAccountKey + ";EndpointSuffix=core.windows.net"
$env:NAH_EHLISTENER_LEASECONTAINERNAME = "sync"
$env:NAH_AAD_DOMAIN = $DirectoryName + ".onmicrosoft.com"
$env:NAH_AAD_CLIENTID = $ClientId
$env:NAH_AAD_CLIENTSECRET = $AADApplicationPassword
$env:NAH_AZURE_SUBSCRIPTIONID = (Get-AzureRmSubscription -SubscriptionName $SubscriptionName).SubscriptionId
$env:NAH_AZURE_DLSOUTPUTMANAGER_ACCOUNTNAME = $DataLakeStoreName
$env:NAH_AZURE_DLA_ACCOUNTNAME = $DataLakeAnalyticsName
$env:NAH_FileOutputManager_LocalDataFolder = "c:\tmp\USQLDataRoot"

# Run Visual Studio Solution
Write-Host "Opening the Visual Studio solution..." -ForegroundColor "yellow"
.\Nether.Ingest.sln