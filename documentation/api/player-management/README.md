# Nether Players (part of Player Management)

Player Management (players) provides a simple way to manage players
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

### Administrator-callable APIs 

|Method | Request URI   | Description |
|-------|---------------|-------------|
|GET|[`/api/admin/players`](admin-get-all-players.md)|Gets all players.|
|POST|[`/api/admin/players`](admin-add-new-player.md)|Creates or updates extended information about a player.|
|GET|[`/api/admin/players/{gamerTag}`](admin-get-player-by-gamertag.md)|Gets player information by player's gamer tag.|
|GET|[`/api/admin/players/{gamertag}/state`](admin-get-playerstate.md)|Gets the player state for a player.|
|PUT|[`/api/admin/players/{gamertag}/state`](admin-put-playerstate.md)|Updates player state for a player.|
|GET|[`/api/admin/players/{gamertag}/groups`](admin-get-player-groups.md)|Gets the list of groups a player belongs to.|
