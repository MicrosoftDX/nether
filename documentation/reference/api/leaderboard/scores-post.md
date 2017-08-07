# Post a new score to the Leaderboard

Called after the game session has finished and user achieved a new score in the game.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

Method  | Request URI |
--------|-------------|
POST    | `/api/leaderboard` |

### Request parameters

No parameters

### Request body

```json
{
	"country": "US",
	"score": 4711    
}
```

|  Name  | Required  | Type  | Description |
|--------|-----------|-------|-------------|
|country|yes|string|Country code (US, UK, etc.)|
|score|yes|number|Achieved score, must be non-negative|

### Response

| Status Code | Description |
|-------------|-------------|
|200|Operation completed successfully|
|400|Score is negative or user has no assigned `gamerTag`|
|403|You dont' have permissions to sumbit the score|

### JSON Body

Empty body
