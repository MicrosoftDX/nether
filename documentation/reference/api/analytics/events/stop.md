# Generic Stop Event

## When?

## Why?

## Request

### JSON Body
```json
{
    "type": "stop",
    "version": "1.0.0",
    "clientUtcTime": "2016-09-07T13:37:00",
    "eventCorrelationId": "8630EAE1-6A83-4208-BB56-0530A6A75CF3",
    "gameSessionId": "4FCBF6D4-CF59-45D0-A6EC-A42002E182EC",
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
gameSessionId      | No       | String | GUID that uniquely identifies the game session
properties         | No       | String | Option for the developer to integrate more information, e.g. country, colour, etc.