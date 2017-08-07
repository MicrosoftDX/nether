# Game Heartbeat Event

Event sent by the game client as application is started. Will be used together with Session End Event to calculate length of played sessions, etc.

## When?
In regular time periods. For now per second.

## Why?
For calculating the number of concurrent users, CCUs, in combination with the other session events.

## Request

See Common parameters and headers that are used by all requests related to analytics

Method  | Request URI
------- | -----------
POST    | <event hub url>

TODO: Write description of request here

### JSON Body
```json
{
    "type": "game-heartbeat",
    "version": "1.0.0",
    "clientUtcTime": "2016-09-07T13:37:00",
    "gameSessionId": "4FCBF6D4-CF59-45D0-A6EC-A42002E182EC",
    "properties": {
        "country": "Germany"
    }
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
type              | Yes      | String | Specifies the type of event being sent. Has to be "game-heartbeat".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
clientUtcTime      | Yes      | DateTime | Specifies the UTC timestamp of the client.
gameSessionId      | Yes      | String | GUID that uniquely identifies the game session. Used for correlating it with the game-start and game-stop event (if present)
properties         | No       | String | Option for the developer to integrate more information, e.g. country, colour, etc.