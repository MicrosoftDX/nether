# Nether Player Management Documentation

The player management service provide features to mange players and their profiles.

## Common Parameters and Headers

The following information is common to all tasks that you might do related to leaderboards:

* Replace {api-version} with 2016-09-01 in the URI.
* Replace {game-id} with the game identifier of the current game. If your backend only supports one game, then pick any string to represent and identifies that game, e.g. "default".
* Set the Content-Type header to application/json.
* Set the Authorization header to a JSON Web Token that you obtained from ... TODO

## API Overview

Method  | Request URI
------- | -----------------------
GET     | /players/
GET     | /players/{player}/
POST    | /players/{player}/