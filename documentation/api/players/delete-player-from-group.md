# Removes a Player from a Group

Removes a player from a group. Administrator permissions are required to perform this action.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
DELETE  | `/api/groups/{groupName}/players/{playerName}`

### Request parameters

Name        | Required |   Type   | Description
------------|----------|----------|------------
playerName|Yes|String|Tag of the player
groupName|Yes|String|Group name

### JSON Body

empty body


## Response

| Status Code | Description |
|-------------|-------------|
|204|Success|


### JSON Body

Empty body