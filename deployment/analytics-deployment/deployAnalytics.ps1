param
(
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceGroupName,

    # user Get-AzureRmLocation to get the list of valid location values
    [Parameter(Mandatory=$true)]
    [string]
    $Location,  
    
    [Parameter(Mandatory=$true)]
    [string]
    $SteamAnalyticsQuery       
)
$ErrorActionPreference = "Stop";

$random = Get-Random
$ScriptsStorageAccountName = "assetstore" + $random

$currentPath = Convert-Path .


# $build = "Release"
$build = "Debug"
dotnet build ../../src/Nether.Analytics.EventProcessor -c $build

# Create ZIP (requires PowerShell 5.0 upwards)
Write-Host
Write-Host "Creating EventProcessorJob.zip ..."

$path = "../../src/Nether.Analytics.EventProcessor/bin/Job/App_Data/jobs/continuous/EventProcessor"
If(!(test-path $path))
{
    New-Item $path -Type Directory 
}

# copy webjob build output to the new folde
Copy-Item ../..//src/Nether.Analytics.EventProcessor/bin/$build/* ../../src/Nether.Analytics.EventProcessor/bin/Job/App_Data/jobs/continuous/EventProcessor -Recurse -Force

$source = "$currentPath/../../src/Nether.Analytics.EventProcessor/bin/Job"
$destination = "$currentPath/../../src/Nether.Analytics.EventProcessor/EventProcessorJob.zip"

If(Test-path $destination) {Remove-item $destination}

[Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" ) | out-null
[System.IO.Compression.ZipFile]::CreateFromDirectory($Source, $destination, "Fastest", $false)

Write-Host "Checking for resource group $ResourceGroupName..."

$resourceGroup = Get-AzureRmResourceGroup -name $ResourceGroupName -ErrorAction SilentlyContinue
if ($resourceGroup -eq $null){
    Write-Host "creating new resource group $ResourceGroupName ... in $Location"
    $resourceGroup = New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location
}

$storageAccount = Get-AzureRmStorageAccount `
                    -ResourceGroupName $ResourceGroupName `
                    -Name $ScriptsStorageAccountName `
                    -ErrorAction SilentlyContinue
if ($storageAccount -eq $null){
    Write-Host
    Write-Host "Creating storage account $ScriptsStorageAccountName..."

    $storageAccount = New-AzureRmStorageAccount `
        -ResourceGroupName $ResourceGroupName `
        -Name $ScriptsStorageAccountName `
        -Location $Location `
        -SkuName Standard_LRS
}

$containerName = "deployment"
$container = Get-AzureStorageContainer `
                    -Context $storageAccount.Context `
                    -Container $containerName `
                    -ErrorAction SilentlyContinue
if ($container -eq $null){
    # TODO - create SAS URL rather than making the container public??
    Write-Host
    Write-Host "Creating public (blob) storage container $containerName..."
    $container = New-AzureStorageContainer `
                        -Context $storageAccount.Context `
                        -Name $containerName `
                        -Permission Blob
}

Write-Host
Write-Host "Uploading webjob zip to storage..."
$jobZipblob = Set-AzureStorageBlobContent `
        -Context $storageAccount.Context `
        -Container $containerName `
        -File $destination `
        -Blob "EventProcessorJob.zip" `
        -Force

Write-Host
Write-Host "Uploading Deployment scripts to storage..."
Get-ChildItem -File $currentPath/* | Set-AzureStorageBlobContent `
        -Context $storageAccount.Context `
        -Container $containerName `
        -Force


Write-Host "Deploying ingest and hot path flow"

$deploymentName = "Nether-Analytics-Deployment-{0:yyyy-MM-dd-HH-mm-ss}" -f (Get-Date)

$templateParameters = @{      
    DeployPackageURI = $jobZipblob.ICloudBlob.Uri.AbsoluteUri
    TemplateBaseURL = $container.CloudBlobContainer.StorageUri.PrimaryUri.AbsoluteUri + "/" 
    Query = $SteamAnalyticsQuery   
}

New-AzureRmResourceGroupDeployment `
            -ResourceGroupName $ResourceGroupName `
            -Name $deploymentName `
            -TemplateFile "./NetherAnalytics.json" `
            -TemplateParameterObject $templateParameters
