# Get Player State

Get player state for the specified player.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/admin/players/{gamertag}/state`

### Request Body

Empty body

### Response

| Status Code | Description |
|-------------|-------------|
|200|Success|

### Response Body

The returned body is the persisted player state
