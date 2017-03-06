# Player Management

Player Management functionality, implementing Nether [Player Management APIs](../api/players/README.md), using SQL Database as a data store.

## Prerequisites
* SQL Database - [learn how to create a SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started)
  > To test locally, you may use an on prem installation of SQL Server database
* Microsoft SQL Server Management Studio or Visual Studio - to query against the SQL tables

## Player Management Store configuration

The player management store defaults to an in-memory data store for ease of local configuration.

### In-memory
To configure the in-memory store, use the configuration below:

```json
  "PlayerManagement" : {
        "Store": {
            "wellknown": "in-memory"
        }
  }
```

### SQL Server
To configure the SQL Server store, use the configuration below setting the `ConnectionString` property to the connection string to your database:

```json
  "PlayerManagement" : {
        "Store": {
            "wellknown": "sql",
            "properties": {
              "ConnectionString": "<connection string>"
            }
        }
  }
```

The SQL Server implementation works with local SQL Server and [Azure SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started). For help on getting the connection string for Azure SQL Database, see [How to get sql database connection string for Azure SQL Database?](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-dotnet-simple)
