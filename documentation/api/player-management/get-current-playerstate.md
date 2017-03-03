# Get Current Player State

Get player state for the current player.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/player/state`

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
    "gamertag": "string",
    "state": {}
}

```

Element name        | Required  | Type       | Description
------------------- | --------- | ---------  | -----------
gamertag            | Yes       | String     | Tag of the player
state               | No        | JSON Object     | Player state
