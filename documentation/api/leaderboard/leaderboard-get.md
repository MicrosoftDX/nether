# Get Leaderboard

Get a sorted list of scores and gamertags for a defined leaderboard. If {name} is omitted then the default leaderboard is returned.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

Method  | Request URI
------- | -----------
GET     | `/api/leaderboard/{type}`

### Request Parameters

Name        | Required |   Type   | Description
------------|----------|----------|------------
type|yes|enumeration|Type of the leaderboard to retrieve. The type can be `default`, `top` or `aroundMe`:<br>- `default` gets all the scores for all the players<br>-`top` gets top 5 scores for all the players<br>- `aroundMe` gets your score and 2 players above and below your score.

### Request Body

Empty body

## Response

| Status Code | Description |
|-------------|-------------|
|200|Success|
|403|You dont' have permissions to sumbit the score|

### Response Body

#### Example

```json
{
    "entries": [
        {
        "gamertag": "krist00fer",
        "score": 900    
    },
    {
        "gamertag": "anko",
        "score": 500    
    },
    {
        "gamertag": "vito",
        "score": 100    
    }]
}
```

Element name        | Required  | Type       | Description
------------------- | --------- | ---------- | -----------
entries         | Yes       | json array | A list of gamertags and their highest score
