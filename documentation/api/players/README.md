# Nether Players (part of Player Management)

Player Management (players) provides a simple way to manage players and teams (or groups)
* Player Management serves a single game


## Common Parameters and Headers

The following information is common to all tasks that you might do related to player management:

* Replace `{api-version}` with 2016-09-01 in the URI.
* Set the Content-Type header to application/json.
* Set the Authorization header to a JSON Web Token that you obtained from ... TODO

## API Overview

|Method | Request URI   | Description |
|-------|---------------|-------------|
|GET|[`/api/player`](get-current-player.md)|Gets the player information from currently logged in user.|
|PUT|[`/api/player`](put-current-player.md)|Updates information about the current player.|
|GET|[`/api/playerextended`](get-current-playerextended.md)|Gets the extended player information from currently logged in user.|
|PUT|[`/api/playerextended`](put-current-playerextended.md)|Updates extended (e.g. JSON) information about the current player.|
|GET|[`/api/players`](get-all-players.md)|Gets all players.|
|POST|[`/api/players`](add-new-player.md)|Creates or updates extended information about a player.|
|POST|[`/api/playersextended`](add-new-playerextended.md)|Creates or updates information about a player.|
|GET|[`/api/players/{gamerTag}`](get-player-by-gamertag.md)|Gets player information by player's gamer tag.|
|GET|[`/api/player/groups`](get-current-player-groups.md)|Gets the list of groups current player belongs to.|
|GET|[`/api/players/{gamerTag}/groups`](get-player-groups.md)|Gets the list of groups a player belongs to.|
|GET|[`/api/groups`](get-all-groups.md)|Get list of all groups.|
|POST|[`/api/groups`](add-new-group.md)|Creates a new group.|
|GET|[`/api/groups/{groupName}`](get-group-by-name.md)|Gets a group by name.|
|PUT|[`/api/group`](put-group.md)|Updates group information.|
|GET|[`/api/groups/{groupName}/players`](get-group-members.md)|Gets the members of the group as gamertags.|
|PUT|[`/api/players/{playerName}/groups/{groupName}`](add-player-to-group.md)|Adds player to a group.|
|PUT|[`/api/player/groups/{groupName}`](add-current-player-to-group.md)|Adds currently logged in user to a group.|
|DELETE|[`/api/groups/{groupName}/players/{playerName}`](delete-player-from-group.md)|Removes player from a group.|   
