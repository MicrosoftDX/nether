# Get Player Extended by Gamertag

Get extended information about a player by gamertag.

## Request

See Common parameters and headers that are used by all requests related to the Player Management Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/playersextended/{gamerTag}`

### Request Parameters
Name        | Required |   Type   | Description
------------|----------|----------|------------
gamerTag|Yes|String|Tag of the player

### JSON Body

Empty body

### Response

| Status Code | Description |
|-------------|-------------|
|200|Success|
|404|Player not found|

### JSON Body

```json
{
  "playerExtended": {
    "gamertag": "string",
    "extendedInformation": "string"
  }
}
```

Element name        | Required  | Type       | Description
------------------- | --------- | ---------  | -----------
gamertag            | Yes       | String     | Tag of the player
extendedInformation | No        | String     | Extended information of the player
