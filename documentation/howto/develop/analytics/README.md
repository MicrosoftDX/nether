# Create Analytics Reports using Nether

[TO DO]

## Introduction

Any game or game studio would be beneficial by understanding how the game is behaving and what the players of the game are doing. The information that could be collected and analyzed could help provide answers to questions like:

* How often do my players play my game?
* Where in the world are my players?
* What kind of hardware is popular among my players?
* What is it that makes a certain type of player enjoy my game?
* How much money do I make on the game and from what?

These kinds of questions might often be answered by a report or a graphical dashboard, while other questions, like the below might be better consumed by the game itself:

* Exactly on what latitude and longitude do I have clusters of players during Fridays?
* What is the probability that the current user will stop playing within 10 minutes?
* What mission is most fun for players that are predicted to stop playing within 10 minutes from now?

Nether Analytics is designed to make the above questions possible to answer, either in the form of a graphical result or perhaps in the form of an API to call directly from your game.

Disclaimer: The above questions where designed as examples on what Nether Analytics is designed to help you solve, but Nether might not provide you with an out of the box solution to any or all of the above.

## Architecture

As seen in the [architecture documentation](../architecture.md), you can see that Nether Analytics is built ontop of a set of services with an "Analytics Pipeline" as the "brain" in the middle.

```
+-------------+    +----------+    +--------------------+    +-----------------+
|             |    |          |    |                    |    |                 |
| Game Client +----> EventHub +----> Analytics Pipeline +----> Data Lake Store |
|             |    |          |    |     Simplified     |    |                 |
+----------+--+    +----------+    +-------------+-+-+-++    +-----------------+
           |                                     | | | |
           |                                     | | | |     +-----------------+
           |                                     | | | |     |                 |
         +-v----------+                          | | | +-----> Blob Storage    |
         |            |                          | | |       |                 |
         | Nether.Web |                          | | |       +-----------------+
         |            |                          | | |
         +------------+                          | | |       +-----------------+
                                                 | | |       |                 |
                                                 | | +-------> SQL Database    |
                                                 | |         |                 |
                                                 | |         +-----------------+
                                                 | |
                                                 | |         +-----------------+
                                                 | |         |                 |
                                                 | +---------> PowerBI         |
                                                 |           |                 |
                                                 |           +-----------------+
                                                 |
                                                 |           +-----------------+
                                                 |           |                 |
                                                 +-----------> Custom Output   |
                                                             |                 |
                                                             +-----------------+

```

### Ingest through Azure EventHubs

The game will provide telemetry by sending messages to a service called [Azure Event Hubs](https://azure.microsoft.com/en-us/services/event-hubs/).

> "Azure Event Hubs is a hyper-scale telemetry ingestion service that collects, transforms, and stores millions of events. As a distributed streaming platform, it gives you low latency and configurable time retention, which enables you to ingress massive amounts of telemetry into the cloud and read the data from multiple applications using publish-subscribe semantics."

For Nether Analytics to be able to provide meaningful result it is necessary that the game client provides the required messages/events at the correct times. All the messages/events that are needed to be sent are specified in the provided [recipes](recipes) that will guide you in the right direction.

### Message processing using a set of services

The simplified Analytics Pipeline, pictured above, is realized using a set of services that connected create a full pipeline that will process the incoming messages from the game client. Depending on what output you want to achieve, you will need to configure this pipeline differently.

```
+-------------------------------------------------+
|                                                 |
| Custom Code (Nether.Analytics)                  |
|                                                 |
+----------+--------------------------+-----------+
           | (cold path)              | (hot path)
           |                          |
+----------v----------+   +-----------v-----------+
|                     |   |                       |
| Data Lake Store     |   | Event Hubs            |
|                     |   |                       |
+----------+----------+   +-----------+-----------+
           |                          |
           |                          |
           |                          |
+----------v----------+   +-----------v-----------+
|                     |   |                       |
| Data Lake Analytics |   | Stream Analytics      |
|                     |   |                       |
+----------+----------+   +----+------------+---+-+
           |                   |            |   |
           |   +---------------+            |   +------+
           |   |                            |          |
+----------v---v--+  +--------------+  +----v----+ +---v---+
|                 |  |              |  |         | |       |
| Data Lake Store |  | SQL Database |  | PowerBI | | Other |
|                 |  |              |  |         | |       |
+-----------------+  +--------------+  +---------+ +-------+
```

The “custom code” part of the pipeline is there to consume, parse and enrich the incoming messages with information that would only be available on the server side and then route messages to the correct output. Depending on when the information is needed the enriched messages will be routed through the “cold path” of Nether Analytics if a result is not needed immediately or the “hot path” if the result is needed within seconds.

There is not a single architecture that would completely describe the correct setup of the analytics pipeline in Nether Analytics since it all depends on the result you want to achieve. To help you setup this pipeline we have provided an ever growing set of [recipes](recipes) that will tell you how to set it up, together with what messages that need to be sent from the game client.

## How Nether Analytics is provided to you as a game developer

Nether Analytics is provided as one of the building blocks in Nether, the wider project that helps you implement gaming backends on Microsoft Azure. While Nether Analytics belongs to the overall project it is still somewhat different than the core Web APIs provided by Nether Web. This is very much due to the asynchronous nature of telemetry gathering and processing and as such the need for different services in Azure.

### As an out of the box experience

If you want to, you can take Nether Analytics, compile and deploy the provided preconfigured host to one of the many supported services on Azure that allows you to host your own executables. “The host” is configurable so you can provide the details you need to have it up and running in your environment. There are ARM templates that will help you deploy the required infrastructure services on Azure together with your solution.

This alternative is awesome if you want to have a quick start and see how it can all work together with your game without the need of having to write a single line of code yourself. Any updates provided to the Nether project will be available to you by downloading the latest source code and then go through the compile and deploy cycle once again.

### As a library

Many times, your game might already have a backend that you want to extend with functionality from Nether, or you might have other preferences when it comes to the host process, such as: hosting Nether Analytics as part of a bigger Service Fabric Application or using a “server less architecture” with Azure Functions. In these scenarios consuming Nether as a library is probably to prefer.

Nether Analytics is available as a set of NuGet packages that you can pull down from any .NET language in any environment that has the ability to refer to such packages. Benefits to this approach is that the core logic of Nether Analytics is provided as pre-compiled binaries that you can consume and update easily using the NuGet package manager.

### As a combination of the above

And if you are uncertain how you want to do, then just start with the “out of the box experience” since that solution automatically refers to the NuGet packages and provide you with a base that you can later customize to your likings.

## Getting started with Nether Analytics

First step in getting started is to decide whether you want the out of the box experience or if you want the more customizable solution by using the Nuget Packages. Please see the following guidelines to see how to choose the correct starting point:

### Use the out of the box experience if…

* you have no or little experience of backend development
* you want to try Nether Analytics with minimal setup
* you want to show a cool demo
* you need some pointers to understand what services to use in Azure
* you intend to participate in the Nether community and provide updates to the project

### Use Nether as a Library approach if…

* you have no problems using NuGet packages
* you already have a backend running in Azure or plan to implement one
* you need to implement custom analytics solutions that are not implemented in the default out of the box solution
* you have or intend to implement a custom server side configuration system that you want Nether Analytics to adhere to
* you want to have full control when hosting Nether Analytics

Select one of the below for more information

[Getting Started - Nether Analytics out of the box experience](getting-started/analytics-out-of-the-box.md)

[Getting Started - Nether Analytics as a library](getting-started/analytics-as-a-library.md)

## Analytics Reports

### [Daily Active Users](daily-active-users.md)

### [Monthly Active Users](monthly-active-users.md)

### [Geo Clustering](player-geo-cluster-analysis.md)