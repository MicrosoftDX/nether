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
|GET|[`/api/players`](get-all-players.md)|Gets all players.|
|POST|[`/api/players`](add-new-player.md)|Creates or updates information about a player.|
|GET|[`/api/players/{gamerTag}`](get-player-by-gamertag.md)|Gets player information by player's gamer tag.|
|GET|/api/player/groups|Gets the list of groups current player belongs to.|
|GET|/api/groups|Get list of all groups.|
|POST|/api/groups|Creates a new group.|
|GET|/api/groups/`{groupName}`|Gets a group by name.|
|PUT|/api/group|Updates group information.|
|GET|/api/groups/`{groupName}`/players|Gets the members of the group as player objects.|
|GET|/api/players/`{gamerTag}`/groups|Gets the list of group a player belongs to.|
|PUT|/api/players/`{playerName}`/groups/`{groupName}`|Adds player to a group.|
|PUT|/api/player/groups/`{groupName}`|Adds currently logged in user to a group.|
|DELETE|/api/groups/`{groupName}`/players/`{playerName}`|Removes player from a group.|   
