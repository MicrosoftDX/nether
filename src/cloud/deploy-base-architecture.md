# Deploying the base architecture for the "Server Less" Nether Scenarios

Nether is a project composed of reusable set of building blocks, projects, services and best practices designed for Gaming workloads powered by Microsoft Azure. Some of these scenarios uses an architecture style that is known as a Server Less Architecture.

> "Serverless architectures refer to applications that significantly depend on third-party services" -Martin Fowler

In other words, Server Less Architecture is a Software Architecture where you don't need to  care about individual servers, but rather rely on platform services to provide you with the features you need. Microsoft Azure provides a plethora of such services in many different areas such as: custom code, databases, storage, AI, Connected Everything (IoT), analytics, etc. Nether is not the place to describe them all but different scenarios will use a few of those services. Please head over to [http://azure.com](http://azure.com) for more information of all services available.

## Prerequisite

In order to deploy the base architecture you need to have a valid Azure Subscription. Head over to [http://azure.com](http://azure.com) to sign up for a free trial if you don't already have an account.

## The base architecture

Unless otherwise stated, all scenarios that uses the server less approach in Nether use the same base architecture. A very simple architecture, but still scalable.

![Architecture Diagram](architectures/game-function-cosmos.png "Game-> Functions-> CosmosDB")

* [Azure Functions](https://azure.microsoft.com/en-us/services/functions/)
* [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/)

This architecture works well in many cases, both for small scale workloads and for big workloads since both Azure Functions and Azure Cosmos DB are cloud services designed with scale in mind.

## Deploying the base architecture to your Azure Subscription

In order to get started and deploy the differnt scenarios, you first need to deploy the base architecture to your Azure Subscription.

To get you up and running fast we have described the required services in a JSON-template format, quite often referred to as an Azure Resrouce Manager Template or ARM Template, that you can use to deploy the required services.

> There is nothing that hinders you from deploying the base architecture manually using the Azure Portal, the Azure PowerShell CmdLets or the Cross Platform Azure CLI. This template will just automate the setup for you.

During the deployment you'll need to provide an answer to the following parameters.

Parameter           | Description                   | Example Values
--------------------|-------------------------------|--------------------
Subscription        | The Azure Subscription you want to deploy to. Often you will not have more than one option here. | MySubscription
Resource group      | A resource group keeps related resources together. | Leaderboard
Location            | Defines where in the world you want your service to be deployed. | North Europe
Function App Name   | A globally unique name that will identify your Function App in Azure. Pick a name that identifies your solution. | nether
Cosmos DB Account Name | A globally unique name that will identify your Cosmos DB Account in Azure. Pick a name that identifies your solution. | netherdb

Click the below button to start the deployment.

> WARNING! Due to a deployment feature that we are using to make sure individual scenarios are easy to download and install in your architecture, every deployment will start from scratch: DO NOT DEPLOY TWICE USING THE SAME PARAMETERS, SINCE THAT WILL OVERRIDE ANY IMPLEMENTED SCENARIOS/FUNCTIONS

(Ctrl + Click will open the portal with the deployment in a separate tab in most browsers, hence allow you to keep this documentation available and ready to use)

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoftDX%2Fnether%2Fserverless%2Fsrc%2Fcloud%2Ffunctions%2F%2Fazuredeploy.json" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>

Wait for the deployment to finish before continuing.