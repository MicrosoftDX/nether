# Update Current Player

Updates information about currently logged in player.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
PUT     | `/api/player`

### Request parameters

No parameters

### JSON Body

```json
{
  "gamertag": "string",
  "country": "string",
  "customTag": "string"
}
```

### Response

| Status Code | Description |
|-------------|-------------|
|204|Player updated successfully|

### JSON Body

Empty body
