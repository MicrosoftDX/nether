param
(
    [Parameter(Mandatory=$true)]
    [string]
    $ResourceGroupName,

    # user Get-AzureRmLocation to get the list of valid location values
    [Parameter(Mandatory=$true)]
    [string]
    $Location
)

#login to your account first
#Login-AzureRmAccount

<#
    - from Nether.Web run "dotnet publish -c Release"
#>

#uncomment sample to delete previous resource groups by prefix
#Write-Host "deleting old resource groups..."
#$result = Get-AzureRmResourceGroup | Where-Object { $_.ResourceGroupName.StartsWith("nether-deploy-") } | Remove-AzureRmResourceGroup -Force

#deploy from template
Write-Host "creating new resource group $ResourceGroupName ..."
$rg = New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location

Write-Host "Deploying application..."
$result = New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile "$PSScriptRoot\template.json"

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