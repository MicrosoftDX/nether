# Leaderboards

Simple leaderboard functionality, implementing Nether [leaderboard APIs](api/leaderboard), using SQL Database as a data store.

> WARNING: The leaderboard implementation in still under development

## Prerequisites
* SQL Database - [learn how to create a SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started)
  > To test locally, you may use an on prem installation of SQL Server database
* Microsoft SQL Server Management Studio or Visual Studio - to query against the SQL tables

## Leaderboard Store Configuration

By default appsettings.json configures an in-memory store for leaderboard. To configure for SQL Server, either update the appsettings.json as shown below, or specify the equivalent via environment variables. See the [configuration](configuration.md) section in this repo for more details.

```json
"Leaderboard" : {
  "Store": {
    "wellknown": "sql",
    "properties": {
        "ConnectionString": "<enter SQL Database connection string>"
    }
  }
}
```     
   For help on getting the connection string for Azure SQL Database, see [How to get sql database connection string for Azure SQL Database?](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-dotnet-simple)

   

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

The leaderboard can fire analytics events automatically when new scores are posted. By default this is disabled but you can configure this 

### Event hubs

This implementation of the 

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

