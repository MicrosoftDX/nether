# Analytics

The analytics building block of Nether builds an architecture specified in [here](analytics-architecture.txt).

## What does it do?
Currently, what the analytics building block gives are the following key KPIs:
* DAU (daily active users): number of distinct users per day
* MAU (monthly active users): number of distinct users per monthly
* non-distinct daily active users, i.e. number of active sessions per day

The analytics part of Nether deploys an architecture using an ARM template and consists of the following high-level parts:
1. Event ingest, including the event data generator
2. Batch layer or cold path
3. Real time layer or hot path


### Event Ingest

Game events are being sent from the client to a then deployed event hub. Information on what the structure of a game event is, refer to the [event APIs of analytics]](api/analytics/ReadMe.md).
There is a simulator game event generator that sends such specified game events to be found [here](). [To Do]

### Real-time Layer / Hot Path

As seen in the [architecture diagram](analytics-architecture.txt), there are two Azure stream analytics (ASA) jobs that run queries on the event ingest:
1. Concurrent Users: This ASA job runs a query to calculate the number of concurrent users in specified time window.
2. Raw Events: This ASA job stores all incoming game events in an Azure blob storage partitioned by date={YY-mm-dddd}. The raw events will be further aggregated and analysed in the batch layer.

The sink of the ASA job "Concurrent Users" is Power BI. Once in powerbi.com, the data ingest can be found under the streaming datasets.


### Batch Layer / Cold Path

[To Do]


## How do I deploy it?

[To Do]

## Configuration required for Azure Data Factory Pipeline

Upload the Hive script dau.hql (under deployment/analytics-assets) onto another blob storage account. Configure the following parameters in analyticsdeploy.parameters.json accordingly:
* hiveScriptFilePath, e.g. [container]/[file-path]
* hiveScriptStorageAccountName
* hiveScriptStorageAccountKey