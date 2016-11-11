# Nether Players (part of Player Management)

Player Management (players) provides a simple way to manage players and teams (or groups)
* Player Management serves a single game


## Common Parameters and Headers

The following information is common to all tasks that you might do related to player management:

* Replace {api-version} with 2016-09-01 in the URI.
* Set the Content-Type header to application/json.
* Set the Authorization header to a JSON Web Token that you obtained from ... TODO

## API Overview

Method  | Request URI
------- | -----------------------
GET     | /players/
GET     | /players/{player}/
GET     | /players/{player}/groups
POST    | /players/
PUT     | /players/{player}/
POST    | /players/{player}/groups/{groupname}/
