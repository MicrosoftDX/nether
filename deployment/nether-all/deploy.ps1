param
(
    [string]
    $ResourceGroupName
)

#Login-AzureRmAccount

<#
    - from Nether.Web run "dotnet publish -c Release"
#>

#uncomment sample to delete previous resource groups by prefix
#Get-AzureRmResourceGroup | Where-Object { $_.ResourceGroupName.StartsWith("nether-deploy-") } | Remove-AzureRmResourceGroup -Force

#deploy from template
$rg = New-AzureRmResourceGroup -Name $ResourceGroupName -Location "ukwest"
New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile "$PSScriptRoot\template.json"

<#
    Template notes:
    
    - MSDeploy extension conflicts with configuration deployment for website, therefore you must have proper dependencies set up
      i.e. MSDeploy depends on website, configuraton depends on MSDeploy success.
#>