# User Retention

This recipe provides information on one of the most important metrics for a successful free-to-play game.
The user retention gives an indication on the level of engagement and the proportion of users returning to your game.

## End Result

You will answer the following question: The percentage of users who showed up again X days after installing the game. X is typically 1, 7, 30 and 90.

Day 1 Retention

| Date | Day 1 Retention |
|---------|----------|
| 2017-05-16T00:00:00.0000000 |	0.739 |
| 2017-05-17T00:00:00.0000000 |	0.53 |
| 2017-05-18T00:00:00.0000000 |	0.325 |

The retention is mostly interesting to display its trend over time. For instance, the table above indicates that the level of user retention drops with progressing day. The above table shows that 73.9% of users who installed the game on May 15th played the game again on May 16th. However, only 53% of the people who installed the game on May 16th returned to the game on the following day.

Day 7 Retention

| Date | Day 7 Retention |
|---------|----------|
| 2017-05-16T00:00:00.0000000 |	0.739 |
| 2017-05-17T00:00:00.0000000 |	0.53 |
| 2017-05-18T00:00:00.0000000 |	0.325 |


## Pre-requisites

Technologies:
* Azure Data Lake Analytics (ADLA) account and an associated Azure Data Lake Store (ADLS) account
* Generated messages, e.g. start and stop
    * Event Hub
    * AD application

## Recipe Steps

## Recipe Steps

Follow the below steps to implement this recipe. Remember that your setup of Nether Analytics might contain custom configuration or already configured recipes so you might have to merge the below instructions with what already exists in your solution. More than one recipe might need the same telemetry and unless otherwise instructed you don’t need to send the same messages more than once.

Unless otherwise stated, all recipes expect the standard Nether Analytics setup where:
* messages are sent from the client to Azure Event Hubs
* messages are formatted using the standard Nether JSON Format

### 1 Telemetry Needed from the Game

Using the Nether REST API or the provided Client SDKs setup the game to send the following messages at these times.

| Message to send                    | At what time                              |
|------------------------------------|-------------------------------------------|
| [geo-location](../message-types/geo-location.md)      | Send at specified time units, e.g. every 5 minutes  |



### 2 Configuration of Message Processor

(TODO: Describe in a few sentences what the Message Processor has for role in this recipe. It might be to just direct the incoming messages to a correct storage location for further analysis later by the “Job Queries” described below, or some more complex logic that enrich or transform the incoming messages before routing the result to the right place. Only a few sentences needed to have a rough understanding on why the setup need to be done.)

An implementation of a [working Message Processor]() can be found in source code for Nether.Analytics.Host and can be useful to have as a reference while following the below steps.

```cs
var clusteringSerializer = new CsvOutputFormatter("id", "type", "version", "enqueueTimeUtc", "gameSessionId", "lat", "lon", "geoHash", "geoHashPrecision", "geoHashCenterLat", "geoHashCenterLon", "rnd");

var clusteringDlsOutputManager = new DataLakeStoreOutputManager(
    clusteringSerializer,
    new PipelineDateFilePathAlgorithm(newFileOption: NewFileNameOptions.Every5Minutes),
    serviceClientCretentials,
    subscriptionId: _configuration[NAH_Azure_SubscriptionId],
    dlsAccountName: _configuration[NAH_Azure_DLSOutputManager_AccountName]);

var clusteringConsoleOutputManager = new ConsoleOutputManager(clusteringSerializer);

builder.Pipeline("clustering")
    .HandlesMessageType("geo-location", "1.0.0")
    .HandlesMessageType("geo-location", "1.0.1")
    .AddHandler(new GeoHashMessageHandler { CalculateGeoHashCenterCoordinates = true })
    .AddHandler(new RandomIntMessageHandler())
    .OutputTo(clusteringConsoleOutputManager, clusteringDlsOutputManager);
```

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
| [geoclustering.usql](../../../src/Nether.Analytics.DataLakeJobs/geoclustering.usql)             | Using ADLA (Azure Data Lake Analytics)    |


```cs

```

#### 3.1 Setup and Schedule geoclustering.usql

(TODO: Explain how someone should go ahead to setup the required query to be run at appropriate times)
The ADLA-job can be run in a batch mode. In other words, you can run it once a day but specify that you want to see the hotspots in chunks of a 15-minute timespan.

Required parameters:
- ``in``: folder location of all events, e.g. /nether/clustering/geo-location/2017/05/16/{*}.csv
- ``out``: file path where the output file is going to be stored, e.g. /nether/clustering/geo-location/2017/05/16/{*}.csv
- ``timeframe_unit``: unit of timeframe to aggregate the clustering over, i.e. year, month, day, hour or minute
- ``timeframe_frequency``: by default set to 1, unless the unit is minute, then 15. In other words, if you want to see the clusters per geohash over time periods of 15 minutes, then set unit="minute" and frequency=15

### 4 Result Details

(TODO: Briefly explain what result you should expect, when it should be there and where to look for it)
Location of resulting file is specified as a parameter (``out``).
Format:

| Timestamp | Geohash | Count of data points | Latitude | Lontitude |
| 2017-05-16T11:45:00.0000000 |	1073741823 |	1 |	0 |	0 |


#### 4.1 Consume the result

(TODO: Explain in detail what results that should be expected and how to consume the result. Explain file formats produced by using tables.)
