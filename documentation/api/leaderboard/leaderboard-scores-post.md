# Post a Score

Post a score

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

Method  | Request URI
------- | -----------
POST    | /leaderboard/scores?api-version={api-version}

### JSON Body
```json
{
    "gamertag": "krist00fer",
    "score": 4711    
}
```

Element name        | Required  | Type      | Description
------------------- | --------- | --------- | -----------
gamertag            | Yes       | String    | Specifies the gamer playing the game.
score               | Yes       | Int       | The achieved score

### Response

Status code: 201 - Created

### JSON Body

Empty body