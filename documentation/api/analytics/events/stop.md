# Generic Stop Event

## When?

## Why?

## Request

### JSON Body
```json
{
    "event": "stop",
    "version": "1.0.0",
    "client-utc": "2016-09-07T13:37:00",
    "gameSessionId": "4FCBF6D4-CF59-45D0-A6EC-A42002E182EC",
    "properties": {
        "country": "Germany"
    }
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
event              | Yes      | String | Specifies the type of event being sent. Has to be "heartbeat".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
gamertag           | Yes      | String | Specifies the player playing the game.