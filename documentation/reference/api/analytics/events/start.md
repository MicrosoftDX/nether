# Generic Start Event

Note, it expects a stop event.

## When?

## Why?

## Request

### JSON Body
```json
{
    "type": "start",
    "version": "1.0.0",
    "clientUtcTime": "2016-09-07T13:37:00",
    "eventCorrelationId": "8630EAE1-6A83-4208-BB56-0530A6A75CF3",
    "displayName": "room",
    "gameSessionId": "A3A22EE1-563A-4697-9EDF-B69B998CD214",
    "properties": {
        "country": "Germany"
    }
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
type              | Yes      | String | Specifies the type of event being sent. Has to be "heartbeat".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
clientUtcTime      | Yes      | DateTime | Specifies the UTC timestamp of the client.
eventCorrelationId | Yes      | String | GUID that uniquely identifies the event to be tracked.
displayName        | Yes      | String | Entity that the developer wants to track, e.g. room or authentication journey started
gameSessionId      | No       | String | GUID that uniquely identifies the game session
properties         | No       | String | Option for the developer to integrate more information, e.g. country, colour, etc.