# Deploy a Nether Scenario

Nether is a project composed of reusable set of building blocks, projects, services and best practices designed for Gaming workloads powered by Microsoft Azure. Many of those building blocks are captured as scenarios that can be solved by deploying a set of Azure Functions on top of your [Base Architecture](deploy-base-architecture.md).

## Prerequisite

In order to follow along this instruction, you need to have completed the [deployment of the Base Architecture for a Serverless Nether Architecture](deploy-base-architecture.md).

## Azure Functions

Nether uses Azure Functions for a serverless implemenation of your game services. Functions allow you to respond to different triggers and execute custom logic. A function can be triggered by: HTTP Requests, messages on queues, blobs created, etc. and can be implemented using several different languages. Even if you could have chosen any language to implement your server side code, in Nether we've taken a decission to use C# as a programming language whenever fit.

Functions can be created and added to your Function App from within the Azure Portal, from your favorite terminal (PowerShell, Bash, Command Line, etc.) or development tool and by many other ways. In Nether we've implemented a custom script that automatically allow you to pull down functions available on the Nether GitHub Repository. This script is automatically installed inside your Function App Account if you deploy the base architecture using the provided template.

> Even if we do recommend you to play with the tools and the Azure Portal, the rest of this documentation will tell you how to deploy the required functions included in the implemented scenarios in Nether, using the custom deployment script.

## Creating a function

Once the [templated deployment](deploy-base-architecture) has completed then you can deploy one or more of the built-in scenarios. You do that by using the build in developer experience, aka. Kudu, in your Azure Function App.

### Finding Kudu
In the outputs for the template deployment there is a `KuduUri` value that will be something like `yourapp.scm.azurewebsites.net`, and you can browse to that.

![template output](images/template-outputs.png)

Alternatively, you can navigate to the Function App in the Azure Portal, click on "Platform Features" and then choose "Advanced Tools (Kudu)" as shown below:

![advanced tools - kudu](images/advanced-tools-kudu.png)

#### Deploying a template

In Kudu, select "PowerShell" from the "Debug console" menu:

![debug console - powershell](images/debug-console-powershell.png)


In the PowerShell console, navigate to `site\wwwroot` (either by clicking the hyperlinked directories, or by typing `cd site\wwwroot`)

At this point you should see `deploy.ps1` listed. This is the script that allows you to easily deploy scenarios.

To deploy a scenario, run `./deploy.ps1 -Group <groupname> -Scenario <scenario>`

E.g `./deploy.ps1 -Group leaderboards -Scenario top-n`

Current supported combinations for Group and Scenario are:

|Group|Scenario|Description|
|-|-|-|
|leaderboards|top-n|functions to capture scores and show a top-n (e.g. top 10) leaderboard|


After running the `deploy.ps1` script you should see new folders that have been created:

![kudu scenario folders](images/kudu-scenario-folders.png)

And navigating back to the Azure Functions blade in the [Azure Portal](https://portal.azure.com) you will see the newly created functions:

![new functions](images/new-functions.png)

Unless the scenario description tells you otherwise, you should now be able to test the deployed scenario.