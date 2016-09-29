# Configuration

Currently the configuration for Nether.Web is handled by `appSettings.json` and environment variables.

## Dependency injection
There is some configuration that controls dependency injection. It is a work in progress, so if it isn't working for a scenario then start an issue to discuss :-)

### How it works

In the configuration file there can be a top-level property (e.g. `LeaderboardStore`) with a known format:

```json
"LeaderboardStore": {
     "implementation": {
       "type": "Nether.Data.InMemory.Leaderboard.InMemoryLeaderboardStore",
       "assembly": "Nether.Data.InMemory.Leaderboard"
     }
}
``` 

or 

```json
"LeaderboardStore": {
    "factory": {
      "type": "Nether.Data.MongoDB.Leaderboard.MongoDBLeaderboardStoreConfigurationFactory",
      "assembly": "Nether.Data.MongoDB"
    },
    "properties": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "leaderboard"
    }
}
```

Here you can see that there can be properties of `implementation` to specify the implementation type to use if it doesn't require any configuration (NOTE: not currently implemented!), or `factory` to specify a type to use to create the required dependency (the type must implement `IDependencyFactory<>`). 

Both `implementation` and `factory` have two properties `type` and `assembly` that specify the type name and assembly name respectively

To consume this configuration in code, add the following to the `Startup.ConfigureServices` method:

```csharp
services.AddServiceFromConfiguration<ILeaderboardStore>(Configuration, "LeaderboardStore");
```

This call adds the object to the DI configuration with a Transient lifetime.


### Overriding the dependency injection with environment variables


In the example above for Mongo, we had:

```json
"LeaderboardStore": {
    "factory": {
      "type": "Nether.Data.MongoDB.Leaderboard.MongoDBLeaderboardStoreConfigurationFactory",
      "assembly": "Nether.Data.MongoDB"
    },
    "properties": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "leaderboard"
    }
}
```

The settings can be changed without editing the config file by setting environment variables. This can be useful for deployment across environments (such as staging, production) as the configuration can be part of the environment rather than the source code. Additionally, it helps to keep configuration secrets out of source code!


In the example above, we can change the `ConnectionString` and `DatabaseName` by setting environment variables called `LeaderboardStore:properties:ConnectionString` and `LeaderboardStore:properties:DatabaseName` respectively.


In a development environment this can be done simply in a script:

```powershell
${env:LeaderboardStore:properties:connectionString} = "<put your connection string here>"
${env:LeaderboardStore:properties:CollectionName} = "<put your database name here>"
``` 

or 
```bash
export LeaderboardStore__properties__connectionString="<put your connection string here>"
export LeaderboardStore__properties__CollectionName="<put your database name here>"
``` 

## Misc notes/comments/ideas

### Scoped configuration
Currently the configuration is exposed in raw form. It might be interesting to explore scoping the configuration to the "properties" section. This would change the code to:

```csharp
string connectionString = configuration["ConnectionString"];
```
rather than 
```csharp
string connectionString = configuration["Leaderboard:properties:ConnectionString"];
```

### Integrate better with built-in DI
Currently the approach doesn't fully integrate with the built-in DI. For example, we explicitly pass the IConfiguration which the DI system could do. Similarly for ILoggerFactory when we come to add logging.

Also, by integrating with the DI system, the DocumentDB store could take a DocumentClient parameter, and the lifetime of that could be handled by the DI system. (e.g. to make it a singleton). 