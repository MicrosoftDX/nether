# Title of recipe

(TODO: Describe why anyone would care about this recipe with a few sentences.)

## End Result

(TODO: In plain English describe what result that will be achieved if this recipe is implemented. Don’t go into details about format of output, but rather what the end value of this recipe is. If the recipe results in visible output, like reports, diagrams, etc. then add a few pictures here to illustrate the result.)

## Pre-Requisites

(TODO: Is there anything that must be done before this recipe can be followed? If so explain it here and why. All Nether Analytics Recipes expect you to have a working Nether Analytics solution up and running so there is no need to describe that here, so only include pre-requisites that are out of the ordinary.)

## Recipe Steps

Follow the below steps to implement this recipe. Remember that your setup of Nether Analytics might contain custom configuration or already configured recipes so you might have to merge the below instructions with what already exists in your solution. More than one recipe might need the same telemetry and unless otherwise instructed you don’t need to send the same messages more than once.

Unless otherwise stated, all recipes expect the standard Nether Analytics setup where:
* messages are sent from the client to Azure Event Hubs
* messages are formatted using the standard Nether JSON Format

### 1 Telemetry Needed from the Game

Using the Nether REST API or the provided Client SDKs setup the game to send the following messages at these times.

| Message to send                    | At what time                              |
|------------------------------------|-------------------------------------------|
| [MessageName1](LinkToMsg1Doc)      | (TODO: Explain when to send the message)  |
| [MessageName2](LinkToMsg2Doc)      | (TODO: Explain when to send the message)  |


### 2 Configuration of Message Processor

(TODO: Describe in a few sentences what the Message Processor has for role in this recipe. It might be to just direct the incoming messages to a correct storage location for further analysis later by the “Job Queries” described below, or some more complex logic that enrich or transform the incoming messages before routing the result to the right place. Only a few sentences needed to have a rough understanding on why the setup need to be done.)

An implementation of a [working Message Processor]() can be found in source code for Nether.Analytics.Host and can be useful to have as a reference while following the below steps.

#### 2.1 Setup Listener


```cs
// Setup Listener
var listenerConfig = new EventHubsListenerConfiguration
{
    EventHubConnectionString = "YOUR_EVENT_HUB_CONNECTION_STRING_GOES_HERE",
    EventHubPath = "YOUR_EVENTHUB_NAME_GOES_HERE",
    ConsumerGroupName = "YOUR_CONSUMER_GROUP_GOES_HERE",
    StorageConnectionString = "YOUR_STORAGE_ACCOUNT_CONNECTION_STRING_GOES_HERE",
    LeaseContainerName = "NAME_OF_LEASE_CONTAINER_GOES_HERE"
};

var listener = new EventHubsListener(listenerConfig);
```

### 2.2 Setup Message Parser

(TODO: Make sure the below code is accurate and up to date)
```cs
// Setup Message Parser
var parser = new EventHubJsonMessageParser();
```

### 2.3 Setup Output Managers

(TODO: Make sure the below code is accurate and up to date)
```cs
// Setup Output Managers
var outputManager = new DataLakeStoreOutputManager(domain, webApp_clientId, clientSecret, subscriptionId, adlsAccountName);
```

### 2.4 Setup Message Router

(TODO: Make sure the below code is accurate and up to date)
```cs
// Build up the Router Pipeline
var builder = new MessageRouterBuilder();

builder.AddMessageHandler(new GamerInfoEnricher());
builder.UnhandledEvent().OutputTo(eventHubOutputManager);

builder.Event("location|1.0.0")
    .AddHandler(new NullMessageHandler())
    .OutputTo(consoleOutputManager, dlsOutputManager);

var router = builder.Build();
```

### 2.5 Setup Message Processor

(TODO: Make sure the below code is accurate and up to date)
```cs
var messageProcessor = new MessageProcessor<EventHubJsonMessage>(listener, parser, router);


// Run in an async context since main method is not allowed to be marked as async
Task.Run(async () =>
{
    await messageProcessor.ProcessAndBlockAsync();
}).GetAwaiter().GetResult();
```

### 3 Setup Job Queries

(TODO: Describe with a few sentences what purpose the job queries have (if any). Refer to the queries by linking to them in the source code and explain when they should be run.)

| Query that should be run           | By what service                           |
|------------------------------------|-------------------------------------------|
| [Query1](LinkToQuery1)             | (TODO: ADLA / Stream Analytics / etc.)    |
| [Query2](LinkToQuery2)             | (TODO: ADLA / Stream Analytics / etc.)    |

#### 3.1 Setup and Schedule Query1

(TODO: Explain how someone should go ahead to setup the required query to be run at appropriate times)

### 4 Result Details

(TODO: Briefly explain what result you should expect, when it should be there and where to look for it)

#### 4.1 Consume the result

(TODO: Explain in detail what results that should be expected and how to consume the result. Explain file formats produced by using tables.)
