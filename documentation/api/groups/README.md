# Nether Groups (part of Player Management)

Player Management (groups) provides a simple way to manage players and teams (or groups)
* Player Management serves a single game


## Common Parameters and Headers

The following information is common to all tasks that you might do related to player management:

* Replace {api-version} with 2016-09-01 in the URI.
* Set the Content-Type header to application/json.
* Set the Authorization header to a JSON Web Token that you obtained from ... TODO

## API Overview

Method  | Request URI
------- | -----------------------
GET     | /groups/
GET     | /groups/{groupname}/
GET     | /groups/{groupname}/players
POST    | /groups/
POST    | /groups/{groupname}/players/{player}/
DELETE  | /groups/{groupname}/players/{player}/
PUT     | /group/{groupname}/
