![Nether Logo](https://raw.githubusercontent.com/MicrosoftDX/nether/master/logos/both-logo-and-title/logo-title-1109x256.png)
# Building blocks for gaming on Azure

serverless branch status

AppVeyor: [![Build status](https://ci.appveyor.com/api/projects/status/v5btbm617bcmu6nq/branch/serverless?svg=true)](https://ci.appveyor.com/project/stuartleeks/nether/branch/serverless)

<!--
Travis:   [![Build Status](https://travis-ci.org/MicrosoftDX/nether.svg?branch=master)](https://travis-ci.org/MicrosoftDX/nether)
-->

## About

Nether is a project composed of reusable set of building blocks, projects, services and best practices designed for Gaming workloads powered by Microsoft Azure, aimed to be beneficial for many type of game developers seeking inspiration or a fully implemented solution.

## Getting Started

Getting started is an easy three step project:

1. Sign up for an Azure Subscription if you don't already have one at [https://azure.com](https://azure.com)
2. [Deploy the Base Architecture](doc/deploy-base-architecture.md)
3. Pick one of the scenarios below, read through the scenario description and deploy using [these instructions](doc/deploy-scenario.md)

> That's it. Even though Azure definitively have a lot of SDKs and client tools that can help you out, the Server Less Architecture used in these scenarios require no local tools at all. In fact you could use your phone to deploy a leaderboard if you wanted to.

## Scenarios

These are the scenarios that we currently have implemented in Nether. Each scenario contains some documentation as well as the required code to implement them. Unless otherwise stated, the scenarios all use the "base architecture".

### Generic

#### [Quote of the Date](src/cloud/functions/generic/quote-of-the-day/)

Give your players a different quote ever day of the week. Perhaps this is not the main end-scenario, but with simple tweaks this could be used to give your players a message what is going on with the game, what features are new or comming outages. You decide.

_Tags: #serverless, #functions_

### Leaderboards

#### [Top N Leaderboard](src/cloud/functions/leaderboards/top-n/)

A simple top 10 global leaderboard implementation that allows you to submit scores and retrieve the higscore.

_Tags: #serverless, #functions, #cosmosdb_

#### Around Me Leaderboard (_work in progress_)

A simple leaderboard that shows your position among other players.

_Tags: #serverless, #functions, #cosmosdb_

#### Facebook Friends Leaderboard (_work in progress_)

A leaderboard that shows your position among your Facebook friends.

_Tags: #serverless, #functions, #cosmosdb_

### Match Making

#### Simple Match Making (_work in progress_)

Simplest version of matchmaking service where two players looking for a game will be paired together.

_Tags: #serverless, #functions, #cosmosdb_

## Contribute Code

We welcome contributions. To contribute please follow the instructions on
[How to contribute?](CONTRIBUTING.md)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Reporting issues and feedback

If you encounter any bugs please file an issue in the Issues section of our GitHub repo.

## License

Nether is licensed under the MIT License.
