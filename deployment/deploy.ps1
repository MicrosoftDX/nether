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
    [string]
    $NetherWebDomainPrefix,

    [Parameter(Mandatory=$true)]
    [securestring]
    $initialNetherAdministratorPassword,

    [Parameter(Mandatory=$true)]
    [string]
    $sqlServerName,

    [Parameter(Mandatory=$true)]
    [string]
    $sqlAdministratorLogin,

    [Parameter(Mandatory=$true)]
    [securestring]
    $SqlAdministratorPassword,

    [Parameter(Mandatory=$true)]
    [string]
    $AnalyticsEventHubNamespace,

    [Parameter(Mandatory=$true)]
    [string]
    $AnalyticsStorageAccountName
)
$ErrorActionPreference = "Stop";

$netherRoot = "$PSScriptRoot/.."


# Publish Nether.Web
Write-Host
Write-Host "Publishing Nether.Web ..."
# $build = "Release"
$build = "Debug"
$publishPath = "$netherRoot/src/Nether.Web/bin/$build/netcoreapp1.1/publish"

if (Test-Path $publishPath)
{
    Remove-Item $publishPath -Recurse -Force
}
dotnet publish "$netherRoot/src/Nether.Web" -c $build

# Create ZIP (requires PowerShell 5.0 upwards)
Write-Host
Write-Host "Creating Nether.Web.zip ..."

# copy Nether.Web.xml to publish folder
Copy-Item "$publishPath/../Nether.Web.xml" $publishPath

$zipPath = "$publishPath/../Nether.Web.zip"
if (Test-Path $zipPath){
    Remove-Item $zipPath
}

# want to be able to use Compress-Archive as it shows progress
# but I get a weird error on deploying: " Could not find a part of the path 'D:\\home\\site\\wwwroot\\runtimes\\unix\\'"
# The .NET library approach succeeds, even though the PowerShell cmdlets are built on it!
# Compress-Archive -Path "$publishPath/*" -DestinationPath $zipPath -CompressionLevel Fastest
[Reflection.Assembly]::LoadWithPartialName( "System.IO.Compression.FileSystem" ) | out-null
[System.IO.Compression.ZipFile]::CreateFromDirectory($publishPath, $zipPath, "Fastest", $false)

Write-Host "Checking for resource group $ResourceGroupName..."
$resourceGroup = Get-AzureRmResourceGroup -name $ResourceGroupName -ErrorAction SilentlyContinue
if ($resourceGroup -eq $null){
    Write-Host "creating new resource group $ResourceGroupName ... in $Location"
    $resourceGroup = New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location
}

$storageAccount = Get-AzureRmStorageAccount `
                    -ResourceGroupName $ResourceGroupName `
                    -Name $storageAccountName `
                    -ErrorAction SilentlyContinue
if ($storageAccount -eq $null){
    Write-Host
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
    Write-Host
    Write-Host "Creating public (blob) storage container $containerName..."
    $container = New-AzureStorageContainer `
                        -Context $storageAccount.Context `
                        -Name $containerName `
                        -Permission Blob
}

Write-Host
Write-Host "Uploading Nether.Web.zip to storage..."
$webZipblob = Set-AzureStorageBlobContent `
        -Context $storageAccount.Context `
        -Container $containerName `
        -File $zipPath `
        -Blob "Nether.Web.Zip" `
        -Force



#deploy from template

Write-Host
Write-Host "Uploading Deployment scripts to storage..."
Get-ChildItem -File $netherRoot/deployment/* -Exclude *params.json -filter nether-deploy*.json | Set-AzureStorageBlobContent `
        -Context $storageAccount.Context `
        -Container $containerName `
        -Force

$templateParameters = @{
    NetherWebDomainPrefix = $NetherWebDomainPrefix
    initialNetherAdministratorPassword = $initialNetherAdministratorPassword
    sqlServerName = $sqlServerName
    sqlAdministratorLogin = $sqlAdministratorLogin
    sqlAdministratorPassword = $SqlAdministratorPassword
    analyticsEventHubNamespace = $AnalyticsEventHubNamespace
    analyticsStorageAccountName = $AnalyticsStorageAccountName
    
    webZipUri = $webZipblob.ICloudBlob.Uri.AbsoluteUri
    # webZipUri = "https://netherassets.blob.core.windows.net/packages/package261.zip"
    # webZipUri = "https://netherbits.blob.core.windows.net/deployment/Nether.Web.zip"
    # templateBaseURL is used for linked template deployments, see deployment/readme.md
    #    This must end with "/" or it will break the linked templates
    templateBaseURL = $container.CloudBlobContainer.StorageUri.PrimaryUri.AbsoluteUri + "/"
    
    #
    ### to customize your deployment, uncomment and provide values for the following parameters
    ### you can find sample value by looking at nether-deploy.json's parameters
    #
    # WebHostingPlan = "Free (no 'always on')"
    # InstanceCount = 1
    # databaseSKU = "Basic"
    # templateSASToken = "" #used for linked template deployments from private locations, see deployment/readme.md
    # ... and more! see nether-deploy.json template for full list of available parameters
}

$deploymentName = "Nether-Deployment-{0:yyyy-MM-dd-HH-mm-ss}" -f (Get-Date)
Write-Host
Write-Host "Deploying application... ($deploymentName)"
$result = New-AzureRmResourceGroupDeployment `
            -ResourceGroupName $ResourceGroupName `
            -Name $deploymentName `
            -TemplateFile "$PSScriptRoot\nether-deploy.json" `
            -TemplateParameterObject $templateParameters

Write-Host
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