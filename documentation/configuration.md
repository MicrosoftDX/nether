# Configuration

Currently the configuration for Nether.Web is handled by `appSettings.json` and environment variables.

## Dependency injection
There is some configuration that controls dependency injection. It is a work in progress, so if it isn't working for a scenario then start an issue to discuss :-)

### How it works

In the configuration file there can be a top-level property (e.g. `LeaderboardStore`). For the types that are integral to Nether, use the `wellknown` property as shown below. The values for `wellknown` are specific to each service. Additionally, there can be a `properties` property that contains the configuration values for the type in use (e.g. connection string for mongo).

```json 
"LeaderboardStore" : {
  "wellknown": "mongo",
  "properties": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "leaderboard"
  }
}
```

To configure a service type that isn't part of Nether, use can use the `implementation` or `factory` properties. When the type can be created via a default constructor then the `implementation` property can be used. The property value is an object with `assembly` and `type` parameters that specify the type to load (the assembly must be in the bin folder currently):


```json
"LeaderboardStore": {
     "implementation": {
       "type": "Nether.Data.InMemory.Leaderboard.InMemoryLeaderboardStore",
       "assembly": "Nether.Data.InMemory.Leaderboard"
     }
}
``` 

If the service type needs some configuration then you can create a factory class that implements `IDependencyFactory<>`, and configure Nether to use that type to obtain instances of the service by specifying the `factory` property as shown below. As with `implementation`, the `factory` property value is an object with `assembly` and `type` parameters to specify the type of the factory (the assembly must be in the bin folder currently):

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