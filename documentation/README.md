# Nether Documentation (Work in progress)

## What is Nether?

In working with gaming partners, from indie developers to AAA studios, we have realized they are often trying to solve common problems. There are hardly any new games that don't require a backend for player management, leader boards and analytics purposes. **Nether** is a project composed of reusable set of building blocks, projects, services and best practices designed to support these commong gaming workloads on [Microsoft Azure](http://azure.microsoft.com).

## Who is it for?

## Features and Components

- **Identity**
- **Player Management**: a solution authenticate, authorize, and administrate players
- **Leaderboard**: a building block that implement basic leaderboard functionality
- **Analytics**: a mechanism to collect, analyze and react to incoming game events

## Benefits

- **Rapid Development**: Nether services will hide the complexity of dealing with player management, leader boards and game analytics. Once the service is deployed, simply interact with REST based API or leverage the Unity Client SDK
- **Reliability**: Nether services are built on top of the enterprise quality infrastructure provided by Microsoft Azure
- **Performance**: a key technical requirement taken into consideration from the start of the project. Additionally, Microsoft Azure is available in many locations across the world, ensuring your services are always near your players
- **Scalability**: Nether leverages the scaling capabilities of Microsoft Azure. You can start small, and scale easily as the populairty of your game rises

## Documentation Sections

### [Architecture](architecture)

Get an overview of the Architecture of Nether.

### [Get Started](getstarted)

Get started with end-to-end scenarios:

- [Nether sample](getstarted/nethersample.md)
- [Analytics for your game](getstarted/analytics.md)
- [Leaderboard for your game](getstarted/leaderboard.md)
- Player management for your game
- Identity for your game

### [How to develop using Nether](howto/develop)

The **How to develop** section provides all the information you need to know as a developer: how to setup your machine in order to build Nether, code of conduct while contributing to Nether, etc.

#### [Nether SDKs](howto/develop/SDKs)

Nether comes with SDKs for different platforms that will allow you to easily interact with Nether’s APIs and analytics solution: Unity, Android, JavaScript, iOS.

#### [Nether Output Managers](howto/develop/output)

Nether includes a number of plugins to output ingested messages to, including Azure CosmosDB, Azure Data Lake Store and Azure SQL Database.

#### [Analytics Reports](howto/develop/analytics)

Nether contains an analytics component with which you can build analytics reports on your game, such as on daily active users or user retention.

#### [Customizing and Extending Nether](howto/develop/extend)

This section shows how to extend or customize Nether to create custom analytics reports or send custom messages, for instance.

#### [Deployment](howto/deploy)

Learn more about how to deploy Nether to your Azure subscription in various manners, e.g. serverless using Azure Functions.

### [Reference](reference)

The Reference section comprises an overview of Azure services used within Nether, Nether's API reference and automatically generated .NET documentation.

### [Resources](resources)

Further resources on how to contribute to Nether and on pricing, learning, videos and more can be found in this section.


[TO BE DEPRECATED]

### [Configuration](configuration)

Nether is provided as an "out of the box" experience that can be configured for your needs. Head over to the configuration section to read about how configuration works in Nether.