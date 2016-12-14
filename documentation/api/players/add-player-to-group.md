# Add a Player to a Group

Adds a specific player to a group.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
PUT     | `/api/players/{playerName}/groups/{groupName} `

### Request parameters

Name        | Required |   Type   | Description
------------|----------|----------|------------
playerName|Yes|String|Tag of the player
groupName|Yes|String|Group name

### JSON Body

Empty body

## Response

| Status Code | Description |
|-------------|-------------|
|200|Success|


### JSON Body

Empty body