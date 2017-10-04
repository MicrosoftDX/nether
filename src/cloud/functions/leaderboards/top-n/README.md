# Top N Leaderboard

This guide will help you implement a Top N Leaderboard on Microsoft Azure using a serverless architecture. The Top N leaderboard will save the top N scores, where N is a configurable positive integer, and retrieve the the sorted highscore list when requested.

## Scenario Requirements

The following requirements where considered while creating this leaderboard implementation:

* Game client should be able to send achieved scores to a REST API.
* Game client should be able to retrieve a leaderboard using a REST API.

### Non Functional Requirements

* Keep scenario simple
* Minimize the need of maintainence
* Scale from low, to extreme usage
* Keep cost to a minimum
* Service should be implemented using C# Script in order for ease of understanding and modification

## Azure Services Used

This implementation uses the following services:

* [Azure Functions](https://azure.microsoft.com/en-us/services/functions/)
* [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/)

## Clients Frameworks/Tools

The leaderboard created in this scenario can be called from any client that can make regular HTTP requests. This documentation however will show you how to make those calls from:

* Command Line, using "curl"
* Unity, using Nether Client Library for Unity

## Prerequisite

In order to follow along in this scenario the following prerequisite need to be fulfilled:

* Have access to a Microsoft Azure Account
* Have Installed Unity 3D *

*) If you plan to call the leaderboard from a Unity game

## Getting started

### 1. Understanding the architecture

This scenario uses a very simple architecture in order to achieve the required outcome. The game will communicate with a custom REST API, hosted as a set of "Functions" in Azure that in turn will connect to Cosmos DB for storing and querying of data.

![Architecture Diagram](../../../architectures/game-function-cosmos.png "Game-> Functions-> CosmosDB")

This architecture works both for small scale workloads and for big workloads since both Azure Functions and Azure Cosmos DB are cloud services designed with scale in mind.

Now, let's move on to deploying these services on Azure.

### 2. Deploy Required Services on Microsoft Azure

There are many ways to deploy services on Microsoft Azure, pick the one that feels most natural to you, then continue to next step.

> If you already have the required services setup in Azure, you could re-use them and just add or tweak the required "functions" that implement this scenario.

#### 2A. Deploy using an Azure Resource Manager, ARM, Template

Your resources in Azure can be deployed automatically using a JSON formated document, called Azure Resource Manager, ARM, Template. The template describes the needed resources/services for your deployment and if there are any dependencies between them.

> You don't need to understand how the template is designed in order to use it, but please have a look at the [ARM Template documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-authoring-templates) if you are interested in learning more and perhaps designing your own templates.

Have a quick look at the template being used in this scenario before you click the "Deploy to Azure" button below in order to start deployment of required resources/services. You'll be taken to the Azure Portal and might have to sign in on your account if you are not allready signed in.

Template used in this scenario: [azuredeploy.json](azuredeploy.json)

Click the below button to start deployment.

(Ctrl + Click will open the portal with the deployment in a separate tab in most browsers, hence allow you to keep this documentation available and ready to use)

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fkrist00fer%2Fnether%2Fserverless%2Fsrc%2Fcloud%2Ffunctions%2Fleaderboards%2Ftop-n%2Fazuredeploy.json" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>

You'll be asked to fill in a few parameters before deployment begins. Please provide them as following.

Parameter           | Description                   | Example Values
--------------------|-------------------------------|--------------------
Subscription        | The Azure Subscription you want to deploy to. Often you will not have more than one option here. | MySubscription
Resource group      | A resource group keeps related resources together. | Leaderboard
Location            | Defines where in the world you want your service to be deployed. | North Europe
Function App Name   | A globally unique name that will identify your Function App in Azure. Pick a name that identifies your solution. | nether
Cosmos DB Account Name | A globally unique name that will identify your Cosmos DB Account in Azure. Pick a name that identifies your solution. | netherdb

Agree to the terms and click "Purchase" to start deployment of required services for your leaderboard.

This will setup the services needed to implement this scenario and also provide the provisioned Azure Function App with insights into where to find the provisioned Cosmos DB Account.

> The above used ARM Template can also be used to deploy the solution from [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli), [Azure PowerShell](https://docs.microsoft.com/en-us/powershell/azure/install-azurerm-ps) or from your Build Server within your CI/CD pipeline.

Please continue to step 3 below once deployment has finished.

#### 2B. Deploy Manually using the Azure Portal

// TODO: Describe here how to deploy manually

### 3. Create the API

So at this stage you should have the required services setup in Azure. Now it's time to implement some server side logic that make up our leaderboard.

Resource          | Verb | Description
------------------|------|-----------------------------
/api/score        | POST | Adds or updates a highscore for a specific player
/api/leaderboard  | GET  | Retrieves the leaderboard

#### 3.1 Create the "Score API"

1. In the Azure Portal, find your "Function App"
2. Add a new C# "Function" triggered by HTTP requests
3. Replace the sample implementation in run.csx with [the script found here (run.csx)](score/run.csx). This will give you the basic logic to accept incomming requests and forward them to Cosmos DB. Remeber to save before continuing.
4. Click on the option "View files" to see all related files to this "Function", and click "+ Add" to create a new file. Name the file: project.json
5. Replace the content of Project.json with [the script found here (project.json)](score/project.json). This will pull down any dependencies needed to implement the function. Remember to save before continuing.

#### 3.2 Create the "Leaderboard API"

1. In the Azure Portal, stay within the same "Function App" as you used above to implement the Score API. Note that a single Function App can host several "Functions". You'll notice that the below instructions are very similar to the above, but the implementation of the "Function" is slightly different.
2. Add a new C# "Function" triggered by HTTP requests
3. Replace the sample implementation in run.csx with [the script found here (leaderboard/run.csx)](leaderboard/run.csx). This will give you the basic logic to accept incomming requests and forward them to Cosmos DB. Remeber to save before continuing.
4. Click on the option "View files" to see all related files to this "Function", and click "+ Add" to create a new file. Name the file: project.json
5. Replace the content of Project.json with [the script found here (leaderboard/project.json)](leaderboard/project.json). This will pull down any dependencies needed to implement the function. Remember to save before continuing.

### Calling the API from a Client Application/Game

Now you've created your leaderboard API and a next step would be to test it out from a client application.