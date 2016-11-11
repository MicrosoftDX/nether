# Get Players

Get a list of all registered players.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

Method  | Request URI
------- | -----------
GET     | /players?api-version={api-version}

### JSON Body

Empty body

### Response

Status code: 201 - Created

### JSON Body

```json
{
    players: [
        {
        "gamertag": "krist00fer",
        "country" : "Sweden",
        "customtag" : "my tag",
        "image": 4711    
    },
    {
        "gamertag": "anko",
        "country" : "Netherlands",
        "customtag" : "my tag",
        "image": 4711    
    },
    {
        "gamertag": "vito",
        "country" : "Italy",
        "customtag" : "my tag",
        "image": 4711    
    }]
}
```

Element name        | Required  | Type       | Description
------------------- | --------- | ---------  | -----------
players             | Yes       | json array | A list of players
