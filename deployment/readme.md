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

Azure's Resource Manager allows you to create a JSON based tmeplate as a declarative way of describing what Azure Resources are needed and how they should be configured. To ensure maximum flexibility, we have opted to break the resources needed down into smaller components that are leveraged using an approach called [linked templates](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-linked-templates). This allows you to individually provision/update any component as needed.

A benefit to the use of resource templates is that they can be run to either do a complete purge/rebuild of a set of resources (-mode complete) or just update existing items (like increasing the performance level of your hosting plan).

The deployment templates follow a consistent naming convention based on nether-deploy*.json. The files are as follows:

**nether-deploy.json** - the "master" template which calls the linked templates
**nether-deploy-db.json** - responsible for provisioning the Azure SQL Database
**nether-deploy-web.json** - responsible for provisioning the Web Site and its associated hosting plan. Also configures the web site to point at other remote resources such as the Nether web deployment package and database connection

You can use these templates directly from PowerShell. For more information, please see [Deploy resources with Resource Manager templates and Azure PowerShell](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy).

Linked Templates require that the child templates are hosted onlinesomewhere (accessible via a hyperlink). That location is designated by the *templateBaseURL* parameter found in the nether-deploy.json master template. If this parameter is not specified, it will default to the location of the master template. If the location is a private storage account, you can grant access by also specifing the optional *templateSASToken* parameter.

When using the templates directly, you may opt to create your own [parameter files](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-deploy#parameters). To avoid accidently committing these files, the Nether repository has already been configured to ignore files that match the file mask ***.privateparams.json**.

Additionally, you can deploy the entire cloud infrastructure of Nether by clicking on this button:
<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoftDX%2Fnether%2Fmaster%2Fdeployment%2Fnether-deploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>
Note that this link does not deploy the Nether application, or any other internal dependencies. Its just a set of empty Azure resources. Additionally, this button will always deploy from the master branch of the Nether project on GitHub.

Lastly, the templates make use of more parameters then are required by the deploy.ps1 script. These additional parameters allow you to take steps like increasing the performance of your web site or database (scaling up), or scaling out the number of copies of your site. You can see more about these options by looking at the templates themselves.

## Customized deployments
Hopefully, many of your customized deployment needs can be accomidated by simply specifying various values in the parameters already exposed by the Nether templates. However, if you want to make your own changes, there's a couple items should be aware of.

If you are calling the "non-master" templates individually, you can make any modifications you need and just deploy them via powershell. However, if you are attempting to use them from the master nether-deploy.json template, you'll need to make sure the modified versions of your templates are available online somewhere. There is code in deploy.ps1 that can be used to upload the templates to Azure storage so they can be asscessed from there. Alternatively, assuming you have forked the Nether repo, you can commit your changes to there and reference them.

Regardless of where you put them, you can specify where hey are located by putting in the complete URI (it must end with a '/') as the value for the nether-deploy.json template's *templateBaseURL* parameter.

If your location is a private storage container (no public link), you can generate a [shared access signature](https://docs.microsoft.com/en-us/azure/storage/storage-dotnet-shared-access-signature-part-1). Specify the query string parameters for the *templateSASToken* parameter.