# Add New Group

Adds a new group. Administrator permissions are required to perform this action.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
POST    | `/api/groups`

### Request parameters

No parameters

### JSON Body

```json
{
  "name": "string",
  "description": "string",
  "members": [
    "string"
  ]
}
```

Element name        | Required  | Type       | Description
--------------------|-----------|------------|------------
name|Yes|String|Name of the group
description|No|String|Group description
members|No|String Array|List of user gamertags to add to this group on creation

## Response

| Status Code | Description |
|-------------|-------------|
|201|Group created|

### Headers

`Location` header contains URL to the API call to get information about the created group i.e. `/api/groups/{groupName}`

### JSON Body

```json
{
	"groupName": "string"
}
```

Element name        | Type       | Description
--------------------|------------|-------------
groupName|String|Name of the group
