# Get Current Player

Get information about currently logged in player.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/player`

### JSON Body

Empty body

### Response

| Status Code | Description |
|-------------|-------------|
|200|Success|
|404|Player information not found|

### JSON Body

```json
{
    player: {
        "gamertag": "krist00fer",
        "country" : "Sweden",
        "customtag" : "my tag",
        "imageUrl": "https://storagesample.blob.core.windows.net/mycontainer/photos/kristofer.jpg"    
	}
}
```

Element name        | Required  | Type       | Description
------------------- | --------- | ---------  | -----------
gamertag            | Yes       | String     | Tag of the player
country             | Yes       | String     | Country of the player
customtag           | No        | String     | Custom tag
imageUrl            | No        | String     | Reference url to an image
