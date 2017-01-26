# Deploy to Microsoft Azure

The deployment process is almost fully automated, however some manual steps are involved.

Generally, Nether consists of one or more API application(s) and one or more database(s). This document focuses on deploying the minimum amount of nodes required for Nether to work.

## Publish Web Application Locally

First, you need to create a version of `Nether.Web` application which can be hosted in a web server. In order to do that navigate to the `src/Nether.Web` folder in the command line and type

```cmd
dotnet publish -c release
```

This will create a new folder under `src/Nether.Web/bin/release/netcoreapp1.1/publish`. The folder contains all files required for web application to work.

You need to zip this folder and upload somewhere Azure Resource Manager can access it, like a blob storage. For security purposes you may want to create a SAS token on top of this blob to pass to the deployment script.

## Create Required Resources in Azure

This repository includes a [deployment script](../deployment/deploy.ps1) you can launch from PowerShell. It will create a new resource group, a database server and web application. The script will also set web application parameters to point to the database with a proper connection string. The script accepts resource group name and datacenter location as two required input parameters.

You can find default parameters for this deployment in [nether-web.json](../deployment/nether-web.json) file in this repository, in the `parameters` section:

```json
    "parameters": {
        "sqlAdministratorLogin": {
            "type": "string",
            "defaultValue": "netheradmin",
            "metadata": {
                "description": "The admin user of the SQL Server"
            }
        },

        "sqlAdministratorPassword": {
            "type": "securestring",
            "defaultValue": "Password!123",
            "metadata": {
                "description": "The password of the admin user of the SQL Server"
            }
        },

        "webZipUri": {
            "type": "string",
            "defaultValue": "http://website.com/package.zip",
            "metadata": {
                "description": "Absolute URI containing the package to deploy"
            }
        }
    },

```

 It is recommended to change them for production.

You can also change the default parameters for deployed website (see **properties** section under website resource).


## Deploying Database Schema

ARM template creates an empty database but doesn't deploy any schema at the moment.

Visual Studio solution contains a project called `Nether.Data.Sql.Schema` which produces a `.dacpac` file to deploy the schema on top of the created database. You can use a variety of tools like `Visual Studio 2015`, `SQL Server Management Studio` or [command-line](https://msdn.microsoft.com/en-us/library/hh550080(v=vs.103).aspx) to deploy it. Note that by default SQL Server firewall doesn't allow incoming connection from non-Azure services, therefore you may want to temporarily open a firewall port for it. Another option is to use Visual Studio Team Services dacpac deployment task which does it for you.

Once database schema is deployed Nether deployment is complete.