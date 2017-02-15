# Deploying Nether to Azure
In this ReadMe, we'll discuss how to deploy your application to Azure, how this is accomplished, as well as how to customize this process for your own needs.

Nether can be run anywhere. However, it was designed from the start to be hosted in the Microsoft Azure cloud to gain the benefits of scale and resiliency while keeping costs to a minimum.

Nether also tries to balance this with both an streamlined "default" deployment option while still giving you the ability to customize the deployment to meet your individual needs.

The below instructions assume that you already have an Azure Subscription. If you do not, you can sign up for one by going to: [https://azure.microsoft.com](https://azure.microsoft.com)

## Provisioning Resources and Deploying Nether
Deploying a solution like Nether requires several steps
- Compiling/Packaging the solution
- Provisioning the cloud resources
- deploying the solution package (and other assets) to the cloud

#####Deploy Via PowerShell
To streamline the process of deploying the application, we have created a powershell script ([deploy.ps1](deploy.ps1)) that will perform all of these stemps. The script makes use of the Azure PowerShell cmdlets. These can be installed by following the [instructions found here](https://docs.microsoft.com/en-us/powershell/azureps-cmdlets-docs/#step-1-install-azure-powershell).

To leverage the script, simply clone this repository and change to the /deployment folder. From there, in Powershell, run Deploy.ps1. You will be prompted for the following parameters as part of this script:
**Resource Group Name** - this is a logical container into which the Azure resources will be placed.
**Location** - The Azure Region where your resources will be located. A list of resources can be retrieved using the PowerShell cmdlet [Get-AzureRmLocation](https://msdn.microsoft.com/en-us/library/mt619449.aspx).
**Storage Account Name** - The leading portion of the globally unique Azure Storage account URI that will be used by your deployment. This can contain only letters and '-'. It cannot start or end with a '-' and must be a value that is not already in use in any other Azure subscription then the one you want to deploy into.
**Nether Web Domain Prefix** - the leading portion of the globally unique URL where your Nether APIs will be hosted. The value you specify must contain only letters or '-'. It cannot startor end with a '-'. The full name for your web site will appear as http://<yourvalue>-website.azurewebsites.net and this must be globally unique.
**SQL Server Name** - This will be the unique name of your [Azure SQL Database](https://azure.microsoft.com/en-us/services/sql-database/?b=16.50) for Nether. Like the storage account and web site, it must be globally unique and only composed of letters or '-' and cannot start or end with '-'.
**SQL Administrator Login** - this is the username to be used to administer and access your SQL Database.
**SQL Administer Password** - this is the password associated with the Administrator Login

When run, Deploy.ps1 will build and package the Nether application. It will then Provision the Azure resources necessary for hosting Nether in your Azure subscription and deploy the application to them for you. 

**Note: At this time, the deploy does not create the Nether Database Schema.**

## ARM Templates (Provisioning Cloud Resources)
- why we're breaking up the templates

nether-deploy.json
nether-deploy-db.json
nether-deploy-web.json
- how to ignore deployment parameter files

- what needs to be changed for customizations
	- linked template base URL
	- "deploy to Azure"
- altering "deploy to Azure" url when testing

## Customized deployments

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoftDX%2Fnether%2Fmaster%2Fdeployment%2Fnether-deploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

- how to ignore deployment parameter files
