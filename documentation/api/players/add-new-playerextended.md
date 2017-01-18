# Add Player Extended information

Adds extended player information. If the player information already exists updates information about the player. Administrator permissions are required to perform this action.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
POST    | `/api/playersextended`

### Request parameters

No parameters

### JSON Body

```json
{
  "userId": "string",
  "gamertag": "string",
  "extendedInformation": "string"
}
```

Element name        | Required  | Type       | Description
--------------------|-----------|------------|------------
userId|No|String|Id of the player
gamertag|Yes|String|Tag of the player
extendedInformation|No|String|Extended information (e.g. JSON) about the player

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
