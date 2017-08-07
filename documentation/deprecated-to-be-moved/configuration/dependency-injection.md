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

Internally, each `wellknown` value maps to a class derived from `DependencyConfiguration` (from `Nether.Common`) that performs the dependency registration required. To register a custom implementation of a dependency, create a class derived from `DependencyConfiguration` for your service and specify it using the `configureWith` option as shown below.


```json
"PlayerManagement" : {
  "Store" : {
     "configureWith": {
       "type": "MyCustomStoreDependencyConfiguration",
       "assembly": "MyCustomStore.PlayerManagement"
     }
}
``` 


If you are implementing a custom store and need to perform some initialisation (e.g. initialising the schema), then your `DependencyConfiguration` implementation can also register an implementation of `IDependencyInitialiation<TStore>` for your store type (`TStore`). Nether.Web will then invoke that on startup.
