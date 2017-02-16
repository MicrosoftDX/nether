# Game Stop Event

Event sent by the game client when it terminates. Will be used together with Session Start Event to calculate length of played sessions, etc.
In the case of the game crashing and thus not sending a game-stop event, the last game-heartbeat event with the corresponding gameSessionId will be used as a game-stop instead.

## When?
When player terminates the game.

## Why?
To calculate the session length in combination with the session start event. 

## Request

See Common parameters and headers that are used by all requests related to analytics

Method  | Request URI
------- | -----------
POST    | <event hub url>

TODO: Write description of request here

### JSON Body
```json
{
    "type": "game-stop",
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
type              | Yes      | String | Specifies the type of event being sent. Has to be "game-stop".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
clientUtcTime      | Yes      | DateTime | Specifies the UTC timestamp of the client.
gameSessionId      | Yes      | String | GUID that uniquely identifies the game session. Used for correlating it with the corresponding game-start and game-heartbeat event (if present)
properties         | No       | String | Option for the developer to integrate more information, e.g. country, colour, etc.