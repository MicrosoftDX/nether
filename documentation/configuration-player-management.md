# Player Management

Player Management functionality, implementing Nether [Player Management APIs](api/players), using SQL Database as a data store.

## Prerequisites
* SQL Database - [learn how to create a SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started)
  > To test locally, you may use an on prem installation of SQL Server database
* Microsoft SQL Server Management Studio or Visual Studio - to query against the SQL tables

## Data store configuration

Update connection string in appsetting.json file, or via environment variables as described in the [configuration docs](configuration-dependency-injection.md#overriding-the-dependency-injection-with-environment-variables).

   ```json
    "PlayerManagement" : {
      "Store": {
        "wellknown": "sql",
        "properties": {
            "ConnectionString": "<enter SQL Database connection string>"
        }
      }
    }
   ```     
   For help on getting the connection string for Azure SQL Database, see [How to get sql database connection string for Azure SQL Database?](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-dotnet-simple)
   
