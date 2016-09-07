# Heartbeat Event

Event sent by the game client as application is started. Will be used together with Session End Event to calculate length of played sessions, etc.

## When?
In regular time periods. For now per second.

## Why?
For calculating the number of concurrent users, CCUs, in combination with the other session events.

## Request

See Common parameters and headers that are used by all requests related to analytics

Method  | Request URI
------- | -----------
POST    | https://eventhuburl.com/blabla/blabla

TODO: Write description of request here

### JSON Body
```json
{
    "event": "heartbeat",
    "version": "1.0.0",
    "client-utc": "2016-09-07T13:37:00",
    "gamertag": "gamertag"
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
event              | Yes      | String | Specifies the type of event being sent. Has to be "heartbeat".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
gamertag           | Yes      | String | Specifies the player playing the game.