param
(
    [string]
    $ResourceGroupName
)

#Login-AzureRmAccount

$rg = New-AzureRmResourceGroup -Name $ResourceGroupName -Location "ukwest"
New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile "template.json"