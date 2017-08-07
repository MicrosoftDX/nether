# Game Start Event

Event sent by the game client as application is started. Will be used together with Session End Event to calculate length of played sessions, etc.

## When?
When player starts the game.

## Why?
To calculate the session length in combination with the session stop event. 

## Request

See Common parameters and headers that are used by all requests related to analytics

Method  | Request URI
------- | -----------
POST    | <event hub url>

TODO: Write description of request here

### JSON Body
```json
{
    "type": "game-start",
    "version": "1.0.0",
    "clientUtcTime": "2016-09-07T13:37:00",
    "gameSessionId": "A3A22EE1-563A-4697-9EDF-B69B998CD214",
    "gamerTag": "gamertag",
    "properties": {
        "country": "Germany"
    }
}

```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
type              | Yes      | String | Specifies the type of event being sent. Has to be "game-start".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
clientUtcTime      | Yes      | DateTime | Specifies the UTC timestamp of the client.
gameSessionId      | Yes      | String | GUID that uniquely identifies the game session. Used for correlating it with the corresponding game-heartbeat and game-stop event (if present)
properties         | No       | String | Option for the developer to integrate more information, e.g. country, colour, etc.