# Session Start Event

Event sent by the game client as application is started. Will be used together with Session End Event to calculate length of played sessions, etc.

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
    "game": "space-shooter",
    "player": "playerid",
    "ip": "213.88.131.129",
    "country": "Sweden",
    "city": "Stockholm",
    "longitude": "xxx",
    "latitude": "xxx",
    "max-session-length": 3600
}
```

Element name       | Required | Type   | Description
------------------ | -------- | ------ | -----------
event              | Yes      | String | Specifies the type of event being sent. Has to be "session-start".
game               | No       | String | Specifies what game is being played. Useful if backend handles more than one game.
player             | No       | String | Specifies the player playing the game.
ip                 | No       | String | Specifies IP of game client.
...                | ...      | ...    | ...
max-session-length | No       | Int    | Number of seconds a session is maximum expected to be

### Response

Status code: 201

TODO: Should not be the same response as request

### JSON Body
```json
{
    "event": "session-start",
    "game": "space-shooter",
    "player": "playerid",
    "ip": "213.88.131.129",
    "country": "Sweden",
    "city": "Stockholm",
    "longitude": "xxx",
    "latitude": "xxx"
}
```

Element name | Required | Type   | Description
------------ | -------- | ------ | -----------
event        | Yes      | String | Specifies the type of event being sent. Has to be "session-start".
game         | No       | String | Specifies what game is being played. Useful if backend handles more than one game.
player       | No       | String | Specifies the player playing the game.
ip           | No       | String | Specifies IP of game client.
...          | ...      | ...    | ...
