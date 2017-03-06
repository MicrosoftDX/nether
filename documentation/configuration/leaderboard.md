# Leaderboard Configuration

Simple leaderboard functionality, implementing Nether [leaderboard APIs](api/leaderboard), using SQL Database as a data store.

## Prerequisites
* SQL Database - [learn how to create a SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started).
    To test locally, you may use an on prem installation of SQL Server database
* Microsoft SQL Server Management Studio or Visual Studio - to query against the SQL tables

## Leaderboard Store Configuration

The leaderboard store defaults to an in-memory data store for ease of local configuration.

### In-memory
To configure the in-memory store, use the configuration below:

```json
  "Leaderboard" : {
        "Store": {
            "wellknown": "in-memory"
        }
  }
```

### SQL Server
To configure the SQL Server store, use the configuration below setting the `ConnectionString` property to the connection string to your database:

```json
  "Leaderboard" : {
        "Store": {
            "wellknown": "sql",
            "properties": {
              "ConnectionString": "<connection string>"
            }
        }
  }
```

The SQL Server implementation works with local SQL Server and [Azure SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started). For help on getting the connection string for Azure SQL Database, see [How to get sql database connection string for Azure SQL Database?](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-dotnet-simple)

## Leaderboards Configuration
The leaderboard _GET_ API will return various leaderboards, based on pre-defined configurations - top 10 ranks, all ranks, ranks around me and more.
The different types of leaderboards are defined in the appsetting.json file under the `Leaderboards` section, and can be extended by simply adding an entry for a new leaderboard.
In this configuration sample, we have 4 types of leaderboards:

```json
"Leaderboards": {
            "Default": {
                "Type": "All",
                "IncludeCurrentPlayer": true
            },
            "5_AroundMe": {
                "Type": "AroundMe",
                "Radius": 5,
                "IncludeCurrentPlayer": true
            },
            "Top_5": {
                "Type": "Top",
                "Top": 5,
                "IncludeCurrentPlayer": true
            },
            "Top_10": {
                "Type": "Top",
                "Top": 10,
                "IncludeCurrentPlayer": true
            }
        }
```

**Usage:**

1. Default (all ranks) leaderboard: /api/leaderboard or /api/leaderboard/Default
2. Top 10 ranking players: /api/leaderboard/Top_10
3. Top 5 ranking players: /api/Leaderboard/Top_5
4. Players around me (5 below and 5 above the logged in player ranking): /api/leaderboard/5_AroundMe   

## Leaderboard Analytics Integration Configuration

The leaderboard can fire analytics events automatically when new scores are posted. By default this is disabled but you can configure this as shown below.

### Disabled

To disable the automatic integration with analytics, set the configuration as shown below:

```json
    "Leaderboard": {
        "AnalyticsIntegrationClient": {
            "wellknown": "null"
        }
    }
```

### Event hubs

This implementation of the integration only works with event hub, but will perform better than the HTTP implementation in this scenario.

```json
    "Leaderboard": {
        "AnalyticsIntegrationClient": {
            "wellknown": "eventhub",
              "properties": {
                "EventHubConnectionString": "<event hub connection string>"
             }
        }
    }
```


### HTTP
For a more generic implementation that pulls the connection information from the `/api/endpoint` API, you can configure the HTTP integration client:

```json
    "Leaderboard": {
        "AnalyticsIntegrationClient": {
            "wellknown": "http",
              "properties": {
                "AnalyticsBaseUrl": "http://localhost:5000/api/" /* URL to the base of the API where the /endpoint API exists */
             }
        }
    }
```

