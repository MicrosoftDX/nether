# Dependency injection Configuration
There is some configuration that controls dependency injection. It is a work in progress, so if it isn't working for a scenario then start an issue to discuss :-)

## How it works

In the configuration file there can be a property under a service name (e.g. `PlayerManagement:Store`). For the types that are integral to Nether, use the `wellknown` property as shown below. The values for `wellknown` are specific to each service. Additionally, there can be a `properties` property that contains the configuration values for the type in use (e.g. connection string for mongo).

```json 
"PlayerManagement" : {
  "Store" : {
    "wellknown": "mongo",
    "properties": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "playermanagement"
    }
}
```

To configure a service type that isn't part of Nether, use can use the `implementation` or `factory` properties. When the type can be created via a default constructor then the `implementation` property can be used. The property value is an object with `assembly` and `type` parameters that specify the type to load (the assembly must be in the bin folder currently):


```json
"PlayerManagement" : {
  "Store" : {
     "implementation": {
       "type": "Nether.Data.InMemory.PlayerManagement.InMemoryPlayerManagementStore",
       "assembly": "Nether.Data.InMemory.PlayerManagement"
     }
}
``` 

If the service type needs some configuration then you can create a factory class that implements `IDependencyFactory<>`, and configure Nether to use that type to obtain instances of the service by specifying the `factory` property as shown below. As with `implementation`, the `factory` property value is an object with `assembly` and `type` parameters to specify the type of the factory (the assembly must be in the bin folder currently):

```json
"PlayerManagement" : {
  "Store" : {
    "factory": {
      "type": "Nether.Data.MongoDB.PlayerManagement.MongoDBPlayerManagementStoreConfigurationFactory",
      "assembly": "Nether.Data.MongoDB"
    },
    "properties": {
      "ConnectionString": "mongodb://localhost:27017",
      "DatabaseName": "playermanagement"
    }
}
```

To consume this configuration in code, add the following to the `Startup.ConfigureServices` method:

```csharp
services.AddServiceFromConfiguration<IPlayerManagementStore>(Configuration, "PlayerManagement:Store");
```

This call adds the object to the DI configuration with a Transient lifetime.

