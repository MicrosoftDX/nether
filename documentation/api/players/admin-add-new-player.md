# Add New Player

Adds a new player. If the player already exists updates information about the player. Administrator permissions are required to perform this action.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
POST    | `/api/admin/players`

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

Element name        | Required  | Type       | Description
--------------------|-----------|------------|------------
gamertag|Yes|String|Tag of the player
country|No|String|Country of the player
customtag|No|String|Custom tag

## Response

| Status Code | Description |
|-------------|-------------|
|201|Player updated successfully|
|400|Gamertag was not provided|

### Headers

`Location` header contains URL to the API call to get information about the created player i.e. `/api/players/gamertag`

### JSON Body

```json
{
	"gamertag": "string"
}
```

Element name        | Type       | Description
--------------------|------------|-------------
gamertag|String|Tag of the player
