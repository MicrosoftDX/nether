# Top N Leaderboard

This guide will help you implement a Top N Leaderboard on Microsoft Azure using a serverless architecture. The Top N leaderboard will save the top N scores, where N is a configurable positive integer, and retrieve the the sorted highscore list when requested.

## Scenario Requirements

The following requirements where considered while creating this leaderboard implementation:

* Game client should be able to send achieved scores to the leaderboard using a REST API.
* Game client should be able to retrieve the leaderboard using a REST API.

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
