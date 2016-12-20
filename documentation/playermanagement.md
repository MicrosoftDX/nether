# Player Management

Player Management functionality, implementing Nether [Player Management APIs](api/players), using SQL Database as a data store.

> WARNING: The player management implementation in still under development

## Prerequisites
* SQL Database - [learn how to create a SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started)
  > To test locally, you may use an on prem installation of SQL Server database
* Microsoft SQL Server Management Studio or Visual Studio - to query against the SQL tables

## Setup

1. Create the Player Management schema:
   
   **ARM Template**
   
   Use the ARM template in this repository to deply a **new** SQL Azure Database and the schema from a bacpac file (located in this repository as well).
   All deployment templates and assest are located under the [deployment](https://github.com/dx-ted-emea/nether/tree/master/deployment) folder.
   1. Currently, you will need to download the bacpac file, until this repo will be public. Please it in Azure Storage and take a note of the URI. You will need to privde it as an input to the template.
   For the player management store, bacpac files are located under [player-management-assets](https://github.com/dx-ted-emea/nether/tree/master/deployment/player-management-assets) folder.
   2. Deploy the [playerManagementSqlDeploy](https://github.com/dx-ted-emea/nether/blob/master/deployment/playerManagementSqlDeploy.json) template 
   
   **SQL Query:**
   
   ```sql
	
    CREATE TABLE [dbo].[Players]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
        [PlayerId] VARCHAR(50) NOT NULL , 
        [Gamertag] VARCHAR(50) NOT NULL, 
        [Country] VARCHAR(50) NULL, 
        [CustomTag] VARCHAR(50) NULL, 
        [PlayerImage] VARBINARY(50) NULL, 
        PRIMARY KEY ([Id]), 
        CONSTRAINT [AK_Players_PlayerId] UNIQUE ([PlayerId])
    )


    GO

    CREATE INDEX [IX_Players_Gamertag] ON [dbo].[Players] ([Gamertag])

    GO

    CREATE INDEX [IX_Players_PlayerId] ON [dbo].[Players] ([PlayerId])

    GO

    CREATE TABLE [dbo].[Groups]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), 
        [Name] VARCHAR(50)  NOT NULL, 
        [CustomType] VARCHAR(50) NULL, 
        [Description] VARCHAR(50) NULL, 
        [Image] VARBINARY(50) NULL, 
        PRIMARY KEY ([Id]), 
        CONSTRAINT [CK_Groups_Name] UNIQUE([Name])
    )

    GO

    CREATE INDEX [IX_Groups_Name] ON [dbo].[Groups] ([Name])

    GO

    CREATE TABLE [dbo].[PlayerGroups]
    (
        [Gamertag] varchar(50) NOT NULL, 
        [GroupName] varchar(50) NOT NULL, 
        CONSTRAINT [FK_PlayerGroups_Players] FOREIGN KEY ([Gamertag]) REFERENCES [Players](Gamertag), 
        CONSTRAINT [FK_PlayerGroups_Groups] FOREIGN KEY ([GroupName]) REFERENCES [Groups]([Name])
    )
    GO

    CREATE INDEX [IX_PlayerGroups_Gamertag] ON [dbo].[PlayerGroups] ([Gamertag])

    GO

    CREATE INDEX [IX_PlayerGroups_GroupName] ON [dbo].[PlayerGroups] ([GroupName])

   ```
   **Deploy from Visual Studio**
   
    - Open Nether solution in Visual Studio
	- Right click on project Nether.Data.Sql.Schema	and select **Publish** to the SQL Database

2. Get connection string from Azure portal:
   [How to get sql database connection string?](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-dotnet-simple)

3. Update connection string in appsetting.json file:
   ```json
    "PlayerManagement": {
      "Store": {
         "wellknown": "sql",
         "properties": {
            "ConnectionString": "<connection string>",            
         }
      }
   }
   ```     
   Follow the [configuration](configuration.md) section in this repo for more details.


