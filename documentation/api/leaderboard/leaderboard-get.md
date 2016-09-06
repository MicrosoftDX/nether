# Get leaderboard

Get all scores for the leaderboard.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

Method  | Request URI
------- | -----------
GET    | /leaderboard?api-version={api-version}

### JSON Body

Empty body

### Response

Status code: 201 - Created

### JSON Body

```json
{
    leaderboard: [
        {
        "gamertag": "krist00fer",
        "score": 4711    
    },
    {
        "gamertag": "anko",
        "score": 10000000001    
    },
    {
        "gamertag": "vito",
        "score": 6789    
    }]
}
```

Element name        | Required  | Type      | Description
------------------- | --------- | --------- | -----------
leaderboard         | Yes       | json array | A list of gamertags and their highest score
