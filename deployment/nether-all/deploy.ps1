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
Get-AzureRmResourceGroup | Where-Object { $_.ResourceGroupName.StartsWith("nether-deploy-") } | Remove-AzureRmResourceGroup -Force

#deploy from template
$rg = New-AzureRmResourceGroup -Name $ResourceGroupName -Location "ukwest"
New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile "$PSScriptRoot\template.json"