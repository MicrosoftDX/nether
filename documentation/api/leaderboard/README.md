# Nether Leaderboard

Leaderboard provides a simple way to manage players scores and keep track of who is best based on different criterias

## Common Parameters and Headers

The following information is common to all tasks that you might do related to leaderboards:

* Replace {api-version} with 2016-09-01 in the URI.
* Replace {game-id} with the game identifier of the current game. If your backend only supports one game, then pick any string to represent and identifies that game, e.g. "default".
* Set the Content-Type header to application/json.
* Set the Authorization header to a JSON Web Token that you obtained from ... TODO

## API Overview

Method  | Request URI
------- | -----------------------
GET     | /leaderboards/
POST    | /leaderboards/scores/