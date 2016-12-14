# Get All Players

Get a list of all registered players.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/players`

### JSON Body

Empty body

### Response

Status code: 200 - Success

### JSON Body

```json
{
    players: [
        {
        "gamertag": "krist00fer",
        "country" : "Sweden",
        "customtag" : "my tag",
        "imageUrl": "https://storagesample.blob.core.windows.net/mycontainer/photos/kristofer.jpg"    
    },
    {
        "gamertag": "anko",
        "country" : "Netherlands",
        "customtag" : "my tag",
        "imageUrl": "https://storagesample.blob.core.windows.net/mycontainer/photos/kristofer.jpg"    
    },
    {
        "gamertag": "vito",
        "country" : "Italy",
        "customtag" : "my tag",
        "imageUrl": "https://storagesample.blob.core.windows.net/mycontainer/photos/kristofer.jpg"    
    }]
}
```

Element name        | Required  | Type       | Description
------------------- | --------- | ---------  | -----------
gamertag            | Yes       | String     | Tag of the player
country             | Yes       | String     | Country of the player
customtag           | No        | String     | Custom tag
imageUrl            | No        | String     | Reference url to an image
