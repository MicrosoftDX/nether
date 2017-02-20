# Score Event

## When?

## Why?

## Request

### JSON Body
```json
{
    "type": "location",
    "version": "1.0.0",
    "clientUtcTime": "2016-09-07T13:37:00",
    "gameSessionId": "A3A22EE1-563A-4697-9EDF-B69B998CD214",
    "properties": {
        "country": "Germany",
        "longitude": "",
        "latitude": "",
        "ip": "",
        "city": ""
    }
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
type              | Yes      | String | Specifies the type of event being sent. Has to be "heartbeat".
version            | Yes      | String | Specifies the version of event, based on how much information is being sent.
clientUtcTime      | Yes      | DateTime | Specifies the UTC timestamp of the client.
gameSessionId      | Yes      | String | GUID that uniquely identifies the game session to correlate with the corresponding game-start/heartbeat event.
properties         | No       | String | Option for the developer to integrate more information, e.g. location details