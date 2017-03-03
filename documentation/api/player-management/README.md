# Nether Players (part of Player Management)

Player Management (players) provides a simple way to manage players and teams (or groups)
* Player Management serves a single game


## Common Parameters and Headers

The following information is common to all tasks that you might do related to player management:

* Replace `{api-version}` with 2016-09-01 in the URI.
* Set the Content-Type header to application/json.
* Set the Authorization header to a JSON Web Token that you obtained from the [Identity Service](../identity/identity.md)

## API Overview
There are two sets of APIs - those intented to be callable by players, and those callable by administrators.

### Player-callable APIs 

|Method | Request URI   | Description |
|-------|---------------|-------------|
|GET|[`/api/player`](get-current-player.md)|Gets the player information from currently logged in user.|
|PUT|[`/api/player`](put-current-player.md)|Updates information about the current player.|
|DELETE|[`/api/player`](delete-current-player.md)|Deletes information about the current player.|
|GET|[`/api/player/state`](get-current-playerstate.md)|Gets the player state for the current player.|
|PUT|[`/api/player/state`](put-current-playerstate.md)|Updates player state for the current player.|
|GET|[`/api/player/groups`](get-current-player-groups.md)|Gets the list of groups current player belongs to.|
|PUT|[`/api/player/groups/{groupName}`](add-current-player-to-group.md)|Adds currently logged in user to a group.|
|GET|[`/api/groups/{groupName}/players`](get-group-members.md)|Gets the members of the group as gamertags.|

### Administrator-callable APIs 

|Method | Request URI   | Description |
|-------|---------------|-------------|
|GET|[`/api/admin/players`](admin-get-all-players.md)|Gets all players.|
|POST|[`/api/admin/players`](admin-add-new-player.md)|Creates or updates extended information about a player.|
|GET|[`/api/admin/players/{gamerTag}`](admin-get-player-by-gamertag.md)|Gets player information by player's gamer tag.|
|GET|[`/api/admin/players/{gamertag}/state`](admin-get-playerstate.md)|Gets the player state for a player.|
|PUT|[`/api/admin/players/{gamertag}/state`](admin-put-playerstate.md)|Updates player state for a player.|
|GET|[`/api/admin/players/{gamertag}/groups`](admin-get-player-groups.md)|Gets the list of groups a player belongs to.|
|GET|[`/api/admin/groups`](admin-get-all-groups.md)|Get list of all groups.|
|POST|[`/api/admin/groups`](admin-add-new-group.md)|Creates a new group.|
|GET|[`/api/admin/groups/{groupName}`](admin-get-group-by-name.md)|Gets a group by name.|
|PUT|[`/api/admin/groups/{groupName}`](admin-put-group.md)|Updates group information.|
|GET|[`/api/admin/groups/{groupName}/players`](admin-get-group-members.md)|Gets the members of the group as gamertags.|
|PUT|[`/api/admin/players/{playerName}/groups/{groupName}`](admin-add-player-to-group.md)|Adds player to a group.|
|DELETE|[`/api/admin/groups/{groupName}/players/{playerName}`](admin-delete-player-from-group.md)|Removes player from a group.|   
