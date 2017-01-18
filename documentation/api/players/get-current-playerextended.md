# Get Current Player Extended

Get extended information about currently logged in player.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/playerextended`

### JSON Body

Empty body

### Response

| Status Code | Description |
|-------------|-------------|
|200|Success|
|404|Extended player information not found|

### JSON Body

```json
{
  "playerExtended": {
    "gamertag": "string",
    "extendedInformation": "string"
  }

```

Element name        | Required  | Type       | Description
------------------- | --------- | ---------  | -----------
gamertag            | Yes       | String     | Tag of the player
extendedInformation | No        | String     | Extended player information (e.g. JSON)
