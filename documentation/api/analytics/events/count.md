# Count Event

## When?

## Why?

## Request

### JSON Body
```json
{
    "event": "",
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