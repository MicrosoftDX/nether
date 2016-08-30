# Session Start Event

Post a score

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

Method  | Request URI
------- | -----------
POST    | https://nethertheproject.com/scores?api-version={api-version}

### JSON Body
```json
{
    "player-id": "krist00fer",
    "score": 4711,
    "game-id": "space-shooter",
    "ip-address": "213.88.131.129",
    "country": "Sweden",
    "city": "Stockholm",
    "longitude": "xxx",
    "latitude": "xxx"
}
```

Element name        | Required  | Type      | Description
------------------- | --------- | --------- | -----------
player-id           | Yes       | String    | Specifies the player playing the game.
score               | Yes       | Int       | The achieved score
game-id             | No        | String    | Specifies what game is being played. Useful if backend handles more than one game.
ip-address          | No        | String    | Specifies IP of game client.
country             | No        | String    | Country of player
city                | No        | String    | City of player
longitude           | No        | String    | Longitude of player
latitude            | No        | String    | Latitude of player

### Response

Status code: 201 - Created

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
