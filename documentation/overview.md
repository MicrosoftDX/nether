# Overview

## Introduction - What is Nether?

Device Silhouette is a solution for managing IoT devices state in the cloud.
The "Silhouette" is virtual version of the device that includes the device’s
latest state. The Silhouette enables applications to interact with the device;
querying the most recent state, and altering the device state. With the Silhouette
applications can interact with the device even when it is offline, the Silhouette
stores the latest reported state, and when receiving commands to change an offline
device's state, Silhouette will send the command once the device is back online,
if the command TimeToLive hasn't expired yet.

The Silhouette holds a sequence of latest received messages with information that
can help to solve conflicts. Every new message received in the Silhouette increases
the Silhouette version. Data stored in a Silhouette include: messages sent from the
device with its latest state, command messages sent to the device, and messages
that indicates if the command was received successfully by the device.

For example, when sending a command to change the device state, the message will
be logged in the Silhouette with type "CommandRequest", the Silhouette will send
the command to the device, monitoring to determine if the message was received
by the device by checking for ACK and NACK from the message broker. When receiving
an ACK a new message will be saved in the silhouette with type "CommandResponse"
and subtype "Acknowledged" and increased version number.


## Features:

- REST API for external applications to interact with the Silhouette.
- The device state is represented in the Silhouette as a JSON object, where the schema is not fixed and determined by the developer.
- The Silhouette holds a sequence of last received messages. Each message has its own version and timestamp, the version is increased with every new message received by the Silhouette.
- Ability to set TimeToLive when sending command to the device to change its state.
- Message delivery status report when sending command to change the device state.
- Old messages are being persisted to a long term persistency database.
- Providers to enable extending the code to support any desired messaging broker technology and storage/DB.

## Benefits:

- Reliability. There are many IoT scenarios where the connection between the devices and the cloud isn't stable. In the situations you still want to know the latest state of your device, or even update the state of your device in a reliable way. The update will be postponed until the connection with your device is available again.
- Performance will be increased because you don't have to "query or ask" the device.
- Service scalability.
- For the developer, the service will hide all the complexity of dealing with publish/subscribe of messages, etc. in the context of a request/response type of pattern. Once the service is deployed, all he needs to do is interact with a REST API to access his devices and he doesn’t have to care about the details of the underlying messaging.

## Scenarios
