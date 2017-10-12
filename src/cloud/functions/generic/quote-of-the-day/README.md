# Quote of the Day

Implements a service that will respond with a different quote depending of the day of the week. This scenario can easily be used as a starting point to develop more advance services.

## Scenario Requirements

The following requirements where considered while creating this scenario:

* Game client should be able to retrieve a daily quote using a REST API
* Demonstrate usage of Azure Functions without any interactin with a database

## Prerequisite

In order to follow along in this scenario the following prerequisite need to be fulfilled:

* You need to have access to a Microsoft Azure Subscription. If you don't have access you can sign up for an Azure Subscription at [https://azure.com](https://azure.com)
* Deployment of Nether Base Architecture should be done. If you haven't deployed the Base Architecture, follow the instructions at [Deploy the Base Architecture](../../../../../doc/deploy-base-architecture.md)
* You need to remember or re-visit the instructions described in [Deploy Scenario](../../../../../doc/deploy-scenario.md)
* Installed the required SDK/Tools for the game client development, for example: Unity

## Scenario API Description

When this scenario is deployed you'll have added one Azure function according to the following.

Resource          | Verb | Parameters               | Description
------------------|------|--------------------------|-----------------------------
/api/quote        | GET  | [None](quote/sample.dat) | Retrieves the quote of the day

## Calling the API from a Client Application/Game

_To be provided_

> Even if the documentation is not yet fully provided, we do have a sample implementation and Client Side SDK developed for Unity that you could use as a starting poing. Please have a look at [the implementation here](../../../../client/unity).