# Deploy to Microsoft Azure

[TO DO: Check if still up to date]

The deployment process is almost fully automated, however some manual steps are involved.

Generally, Nether consists of one or more API application(s) and one or more database(s). This document focuses on deploying the minimum amount of nodes required for Nether to work.

## Publish Web Application Locally

Currently, the publish script is PowerShell only (and requires PowerShell 5.0).
It also requires that you have the Azure PowerShell Cmdlets installed. The link to install the Cmdlets can be found [here](https://azure.microsoft.com/en-us/downloads/)

There is a `deploy.ps1` in the `deployment` folder. This script will coordinate creating the artefacts to publish to [Azure App Service](https://azure.microsoft.com/en-us/services/app-service/web/) and [Azure SQL Database](https://azure.microsoft.com/en-us/services/sql-database/). The deployment uses [Azure Resource Manager Templates](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-overview) for the bulk of the deployment and sets up the configuration for the Web App with the SQL Database connection details etc.


Example script usage:

```powershell

    # The SqlAdministratorPassword parameter is a SecureString... convert the password here
    $sqlPassword = ConvertTo-SecureString -String "PutAStrongPasswordHere;-)" -AsPlainText -Force
    $initialNetherAdminPassword = ConvertTo-SecureString -String "PutAnotherStrongPasswordHere;-)" -AsPlainText -Force

    .\deploy.ps1 `
        -ResourceGroupName "nether" `
        -Location "northeurope" `
        -StorageAccountName "yourstorage" `
        -NetherWebDomainPrefix "yourweb" `
        -initialNetherAdminPassword $initialNetherAdminPassword `
        -sqlServerName "yourserver" `
        -sqlAdministratorLogin "youradminname" `
        -SqlAdministratorPassword $sqlPassword `
        -AnalyticsEventHubNamespace "yourhub" `
        -AnalyticsStorageAccountName "yourstorage2" 

```

### Deployment script parameters

Parameter name | Description
---------------|------------
ResourceGroupName | **Required.** The name of the resource group to deploy into.
Location | **Required.** The Azure location to create the resource group in if it doesn't already exist. You can list the available locations with the `Get-AzureRmLocation` cmdlet.
StorageAccountName | **Required.** The name of the storage account to create/use to upload the web application binaries to for deployment.
initialNetherAdminPassword | **Required.** When the web site starts it checks whether there are any administrator users. If not then it creates an initial `netheradmin` user with the password specified here.
sqlServerName | **Required.** The name of the Azure SQL Server to create/use
SqlAdministratorLogin | **Required.** The username to use for the adminstrator account for the SQL Database.
SqlAdministratorPassword | **Required.** The password to use for the adminstrator account for the SQL Database.
AnalyticsEventHubNamespace |  **Required.** The Event Hub name for the analytics ingestion.
AnalyticsStorageAccountName |  **Required.** The name of the storage account used to store the analytics events.


## Deploying Database Schema

When the web server starts up it checks whether there are any schema changes required and applies them. In the case of an empty database it creates the schema from scratch.