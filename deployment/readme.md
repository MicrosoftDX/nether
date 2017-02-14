# Deploying Nether to Azure
In this ReadMe, we'll discuss how to deploy your application to Azure, how this is accomplished, as well as how to customize this process for your own needs.

Nether can be run anywhere. However, it was designed from the start to be hosted in the Microsoft Azure cloud to gain the benefits of scale and resiliency while keeping costs to a minimum.

Nether also tries to balance this with both an streamlined "default" deployment option while still giving you the ability to customize the deployment to meet your individual needs.

## Provisioning Resources and Deploying Nether
Deploying a solution like Nether requires several steps
- Compiling/Packaging the solution
- Provisioning the cloud resources
- deploying the solution package (and other assets) to the cloud

To streamline the process of deploying the application, we have created a powershell script (deploy.ps1) that can be used to perform all of these steps. This script (by default) will leverage a mixed set of resources available locally in your clone of this repository as well as in the online version on GitHub.

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
