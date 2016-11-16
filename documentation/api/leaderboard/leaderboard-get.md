# Get leaderboard

Get a sorted list of scores and gamertags for a defined leaderboard. If {name} is omitted then the default leaderboard is returned.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

Method  | Request URI
------- | -----------
GET     | /leaderboard/{name}?api-version={api-version}

### Request Parameters

Element name        | Required  | Type       | Description
------------------- | --------- | ---------- | -----------
name                | No        | string     | Name of defined leaderboard to retrieve, defaults to "default"

### Request Body

Empty body

## Response

Status code: 201 - Created

### Response Body

#### Example

```json
{
    leaderboard: [
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
leaderboard         | Yes       | json array | A list of gamertags and their highest score
