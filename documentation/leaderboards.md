# Leaderboards

Simple leaderboard functionality, implementing Nether [leaderboard APIs](api/leaderboard), using SQL Database as a data store.

> WARNING: The leaderboard implementation in still under development

## Prerequisites
* SQL Database - [learn how to create a SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started)
  > To test locally, you may use an on prem installation of SQL Server database
* Microsoft SQL Server Management Studio or Visual Studio - to query against the SQL tables

## Setup

1. Create the Leaderboard schema:
   
   **ARM Template**
   
   Use the ARM template in this repository to deply a **new** SQL Azure Database and the schema from a bacpac file (located in this repository as well).
   All deployment templates and assest are located under the [deployment](https://github.com/dx-ted-emea/nether/tree/master/deployment) folder.
   1. Currently, you will need to download the bacpac file, until this repo will be public. 
   For the leaderboard, bacpac files are located under [leaderboard-assets](https://github.com/dx-ted-emea/nether/tree/master/deployment/leaderboard-assets) folder.
   2. Deploy the [leaderboardSqlDeploy](https://github.com/dx-ted-emea/nether/blob/master/deployment/leaderboardSqlDeploy.json) template
   
   **SQL Query:**
   
   ```sql
	CREATE TABLE [dbo].[Scores]
	(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
    [Score] INT NOT NULL, 
    [GamerTag] NVARCHAR(50) NOT NULL, 
    [CustomTag] NVARCHAR(50) NULL, 
    [DateAchieved] DATETIME NOT NULL DEFAULT GETUTCDATE() 
	)

	GO

	CREATE INDEX [IX_Scores_1] ON [dbo].[Scores] ([DateAchieved], [GamerTag], [Score] DESC)
   ```
   **Deploy from Visual Studio**
   
    - Open Nether solution in Visual Studio
	- Right click on project Nether.Data.Sql.Schema	and select **Publish** to the SQL Database

2. Get connection string from Azure portal:
   [How to get sql database connection string?](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-dotnet-simple)

3. Update connection string in appsetting.json file:
   ```json
    "LeaderboardStore": {
        "wellknown": "sql",
        "properties": {
            "ConnectionString": "<enter SQL Database connection string>"
        }
    }
   ```     
   Follow the [configuration](configuration.md) section in this repo for more details.

## Leaderboards Configuration
The leaderboard _GET_ API will return various leaderboards, based on pre-defined configurations - top ranks, all ranks, ranks around me and more.
The current implementation is hard coded in [LeaderboardConfig](https://github.com/navalev/nether/blob/master/src/Nether.Web/Features/Leaderboard/Configuration/LeaderboardConfig.cs) class with specific implementation in the [LeaderboardController](https://github.com/navalev/nether/blob/master/src/Nether.Web/Features/Leaderboard/LeaderboardController.cs) class.

**Usage:**

1. Default (all ranks) leaderboard: /api/leaderboard or /api/leaderboard/Default
2. Top 10 ranking players: /api/leaderboard/Top
3. Players around me (5 below and 5 above the logged in player ranking): /api/leaderboard/AroundMe   



