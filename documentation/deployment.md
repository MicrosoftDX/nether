# Deploy to Microsoft Azure

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

    .\deploy.ps1 -ResourceGroupName "nether" -Location "northeurope" -StorageAccountName "netherstorage" -SqlAdministratorPassword $sqlPassword

```

### Deployment script parameters

Parameter name | Description
---------------|------------
ResourceGroupName | **Required.** The name of the resource group to deploy into
Location | **Required.** The Azure location to create the resource group in if it doesn't already exist. You can list the available locations with the `Get-AzureRmLocation` cmdlet
StorageAccountName | **Required.** The name of the storage account to create/use to upload the web application binaries to for deployment
SqlAdministratorPassword | **Required.** The password to use for the adminstrator account for the SQL Database
AnalyticsEventHubNamespace |  **Required.** The Event Hub name for the analytics ingestion
AnalyticsStorageAccountName |  **Required.** The name of the storage account used to store the analytics events


## Deploying Database Schema

The deployment script currently creates an empty database but doesn't yet deploy the schema.

Visual Studio solution contains a project called `Nether.Data.Sql.Schema` which produces a `.dacpac` file to deploy the schema on top of the created database. You can use a variety of tools like `Visual Studio 2015`, `SQL Server Management Studio` or [command-line](https://msdn.microsoft.com/en-us/library/hh550080(v=vs.103).aspx) to deploy it. Note that by default SQL Server firewall doesn't allow incoming connection from non-Azure services, therefore you may want to temporarily open a firewall port for it. Another option is to use Visual Studio Team Services dacpac deployment task which does it for you.

Once database schema is deployed Nether deployment is complete.
