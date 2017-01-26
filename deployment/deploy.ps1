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
    $StorageAccountName,

    [Parameter(Mandatory=$true)]
    [securestring]
    $SqlAdministratorPassword
)

$netherRoot = "$PSScriptRoot/.."

# Publish Nether.Web
# $build = "Release"
$build = "Debug"
$publishPath = "$netherRoot/src/Nether.Web/bin/$build/netcoreapp1.1/publish"
if (Test-Path $publishPath)
{
    Remove-Item $publishPath -Recurse -Force
}
dotnet publish src/Nether.Web -c $build

# Create ZIP (requires PowerShell 5.0 upwards)
Compress-Archive -Path "$publishPath/*" -DestinationPath "$publishPath/Nether.Web.zip"


$storageAccount = Get-AzureRmStorageAccount `
                    -ResourceGroupName $ResourceGroupName `
                    -Name $storageAccountName `
                    -ErrorAction SilentlyContinue
if ($storageAccount -eq $null){
    Write-Host "Creating storage account $StorageAccountName..."
    $storageAccount = New-AzureRmStorageAccount `
        -ResourceGroupName $ResourceGroupName `
        -Name $StorageAccountName `
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
    Write-Host "Creating public (blob) storage container $containerName..."
    $container = New-AzureStorageContainer `
                        -Context $storageAccount.Context `
                        -Name $containerName `
                        -Permission Blob
}

Write-Host "Uploading Nether.Web.zip to storage..."
$blob = Set-AzureStorageBlobContent `
        -Context $storageAccount.Context `
        -Container $containerName `
        -File "$publishPath/Nether.Web.zip" `
        -Blob "Nether.Web.Zip" `
        -Force



#uncomment sample to delete previous resource groups by prefix
#Write-Host "deleting old resource groups..."
#$result = Get-AzureRmResourceGroup | Where-Object { $_.ResourceGroupName.StartsWith("nether-deploy-") } | Remove-AzureRmResourceGroup -Force

#deploy from template

Write-Host "Checking for resource group $ResourceGroupName..."
$resourceGroup = Get-AzureRmResourceGroup -name workshare -ErrorAction SilentlyContinue
if ($resourceGroup -eq $null){
    Write-Host "creating new resource group $ResourceGroupName ... in $Location"
    $resourceGroup = New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location
}

$templateParameters = @{
    sqlAdministratorPassword = $SqlAdministratorPassword
    webZipUri = $blob.ICloudBlob.Uri.AbsoluteUri
}

Write-Host "Deploying application..."
$result = New-AzureRmResourceGroupDeployment `
            -ResourceGroupName $ResourceGroupName `
            -TemplateFile "$PSScriptRoot\nether-web.json" `
            -TemplateParameterObject $templateParameters

Write-Host "Done."

<#
    Template notes:
    
    - MSDeploy extension conflicts with configuration deployment for website, therefore you must have proper dependencies set up
      i.e. MSDeploy depends on website, configuraton depends on MSDeploy success.
    - template functions: https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-functions
    - how to get object properties (quote):
        The properties on the object returned from the reference function vary by
        resource type. To see the property names and values for a resource type,
        create a simple template that returns the object in the outputs section.
        If you have an existing resource of that type, your template just returns
        the object without deploying any new resources. If you do not have an
        existing resource of that type, your template deploys only that type and
        returns the object. Then, add those properties to other templates that need
        to dynamically retrieve the values during deployment. 
    - call to reference() is copypasted as I found no way to declare a variable after resource is deployed.
#>