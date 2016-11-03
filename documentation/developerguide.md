# Developer Guide


## Set the development environment

Follow the instructions [here](devenvironment.md)

## Providers

This repository uses the "Providers" concept in order to enable developers to use the Device Silhouette with different existing technologies. For example, for messaging broker, there are a few possible technologies like IoTHub, Kafka etc.
Provider is an interface that a developer can implement for his desired technology.

### Existing providers and implementations in this repro:

#### Communication Providers

The communication providers are a group of interfaces that enable the usage of a specific messaging broker.
It includes the following interfaces:

```
    public interface IMessageReceiver
    {
        // read messages from the communication provider endpoint
        Task RunAsync(CancellationToken cancellationToken);
    }
    public interface IMessageSender
    {
        /* send messages to the communication provider endpoint
         * DeviceId - device it to set message to
         * MessageType - State:Set or State:Get
         * Meesage - message json string         
        */
        Task SendCloudToDeviceAsync(DeviceMessage silhouetteMessage);
    }

    public interface IFeedbackReceiver
    {
        // process feedbacks from C2D messages
        Task ReceviceFeedbackAsync(CancellationToken cancellationToken);
    }
```

This repository includes implementation for IoTHub.
It also includes recommendation how to implement provider for Kafka in [here](CommunicationProviderArchitecture.md)

#### Persistency Providers

The Persistency provider consists of an interface to enable the usage of a specific storage for long term Persistency:

```
    public interface IHistoryStorage
    {
        Task StoreStateMessageAsync(DeviceMessage stateMessage);

        Task StoreStateMessagesAsync(List<DeviceMessage> stateMessages);
    }
```
This repository includes implementation for Azure Blob Storage.


## Test 

This repository contains Visual Studio test projects for functionality and load tests. The test projects are located under SilhouetteTests folder. There you can find the SilhouetteTests.sln with all projects.
for details see [here](test.md)


