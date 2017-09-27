![Nether Logo](https://raw.githubusercontent.com/MicrosoftDX/nether/master/logos/both-logo-and-title/logo-title-1109x256.png)
# Building blocks for gaming on Azure

master branch status

AppVeyor: [![Build status](https://ci.appveyor.com/api/projects/status/v5btbm617bcmu6nq/branch/master?svg=true)](https://ci.appveyor.com/project/stuartleeks/nether/branch/master)

<!--
Travis:   [![Build Status](https://travis-ci.org/MicrosoftDX/nether.svg?branch=master)](https://travis-ci.org/MicrosoftDX/nether)
-->


<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FMicrosoftDX%2Fnether%2Fmaster%2Fdeployment%2Fnether-deploy-quickstart.json" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>


## About

Nether is a project composed of reusable set of building blocks, projects, services and best practices designed for Gaming workloads powered by Microsoft Azure, aimed to be beneficial for many type of game developers seeking inspiration or a fully implemented solution.

## Getting Started

Unless you are using an already existing multitenant service for you game backend, no two backends will look the same. Nether is divided into separate scenarios with receipts that will help you implement your custom backend. Scenarios can be used alone or combined into a bigger backend for your game.

Click on the scenario below that best fit your needs to get started:

### Serverless Implementations

Scenarios described below uses serverless architectures in Azure in order to achieve the decired outcome. Using a serverless architecture is a recommended starting point if you are new to server side development or if you just want to avoid the hassle of managing your own cluster of servers.

#### Leaderboards

* [Top N Leaderboard](src/cloud/functions/leaderboards/top-n/)
* Around Me Leaderboard

#### Inventories

* Basic Cloud Inventory

#### Match Making

* N Player Match

#### Analytics

* Daily / Monthly Active Users, DAU/MAU

## Reporting issues and feedback

If you encounter any bugs with the tool please file an issue in the Issues
section of our GitHub repo.

## Contribute Code

We welcome contributions. To contribute please follow the instructions on
[How to contribute?](CONTRIBUTING.md)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License

Nether is licensed under the MIT License.