# Session Start Event

Event sent by the game client as application is started. Will be used together with Session End Event to calculate length of played sessions, etc.

## When?
When player starts the game.

## Why?
To calculate the session length in combination with the session stop event. 



## Request

See Common parameters and headers that are used by all requests related to analytics

Method  | Request URI
------- | -----------
POST    | https://eventhuburl.com/blabla/blabla

TODO: Write description of request here

### JSON Body
```json
{
    "event": "session-start",
    "version": "1.0.0",
    "client-utc": "2016-09-07T13:37:00",
    "gamertag": "gamertag"
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
event              | Yes      | String | Specifies the type of event being sent. Has to be "session-start".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
gamertag           | Yes      | String | Specifies the player playing the game.