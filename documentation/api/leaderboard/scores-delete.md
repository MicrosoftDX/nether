# Delete scores for current user

Deletes all achieved scores for the current user.

> Useful for testing purposes

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block

|  Method  | Request URI |
|----------|-------------|
|DELETE|`/api/scores`|

### Request Parameters

No parameters

### Request Body

Empty body

## Response

| Status Code | Description |
|-------------|-------------|
|200|Success|
|403|You dont' have permissions to delete the scores|

