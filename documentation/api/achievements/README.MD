# Nether Achievements (part of Player Management)

Player Management (achievements) provides a simple way to manage players, teams (or groups) and achievements
* Player Management serves a single game


## Common Parameters and Headers

The following information is common to all tasks that you might do related to player management:

* Replace {api-version} with 2016-09-01 in the URI.
* Set the Content-Type header to application/json.
* Set the Authorization header to a JSON Web Token that you obtained from ... TODO

## API Overview

Method  | Request URI
------- | -----------------------
POST    | /api/achievements
PUT     | /api/achievements/{id}
GET     | /api/achievements/{id}
DELETE  | /api/achievements/{id}
POST    | /api/achievements/{id}/resetall
POST    | /api/achievements/resetall
GET     | /api/player/{id}/achievements
GET     | /api/player/{id}/achievements/{id}
POST    | /api/player/{id}/achievements/{id}/request
POST    | /api/player/{id}/achievements/{id}/reset
