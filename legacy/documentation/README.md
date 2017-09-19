# Nether Documentation (Work in progress)

## What is Nether?

In working with gaming partners, from indie developers to AAA studios, we have realized they are often trying to solve common problems. There are hardly any new games that don't require a backend for player management, leader boards and analytics purposes. **Nether** is a project composed of reusable set of building blocks, projects, services and best practices designed to support these commong gaming workloads on [Microsoft Azure](http://azure.microsoft.com).

## Features

- Player Management; a solution authenticate, authorize, and administrate players
- Leader Boards; a building block that implement basic leaderboard functionality
- Analytics; a mechanism to collect, analyze and react to incoming game events
- Unity client SDK

## Benefits

- Rapid Development; Nether services will hide the complexity of dealing with player management, leader boards and game analytics. Once the service is deployed, simply interact with REST based API or leverage the Unity Client SDK
- Reliability; Nether services are built on top of the enterprise quality infrastructure provided by Microsoft Azure
- Performance; a key technical requirement taken into consideration from the start of the project. Additionally, Microsoft Azure is available in many locations across the world, ensuring your services are always near your players
- Scalability; Nether leverages the scaling capabilities of Microsoft Azure. You can start small, and scale easily as the populairty of your game rises

## Documentation Sections

### [Architecture](architecture)

Get an overview of the Architecture behind Nether.

### [Deployment](deployment)

Learn more about hot to deploy Nether to your subscription in Microsoft Azure.

### [Configuration](configuration)

Nether is provided as an "out of the box" experience that can be configured for your needs. Head over to the configuration section to read about how configuration works in Nether.

### [Working with, developing for and contributing to Nether](dev)

Our Dev Section documents anything you need to know as a developer. Things like how you setup your machine in order to build Nether to things like code of conduct while contributing to Nether.

### [Nether SDKs Documentation](SDKs)

Nether comes with SDKs for different platforms that will allow you to easily interact with Nether’s APIs and Analytics solution. Read more about the SDK documentation [here](SDKs).

### [Nether API Documentation](api)

Nether consists of a set of APIs that you can interact with directly or through the provided SDKs. For a detailed documentation of the API look [here](api).

### [Nether Analytics Documentation](analytics)

Nether’s analytics solution is composed using several services in Azure together with a customizable message pipeline. Read all about how to get started using Nether Analytics [here](analytics).
