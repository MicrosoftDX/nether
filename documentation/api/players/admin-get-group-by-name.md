# Get Group by Name

Get a list of all groups.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/admin/groups/{groupName}`

### JSON Body

Empty body

### Response

| Status Code | Description |
|-------------|-------------|
|200|Success|

### JSON Body

```json
{
  "group": {
    "name": "string",
    "customType": "string",
    "description": "string",
    "members": [
      "string"
    ]
  }
}
```

Element name        | Type       | Description
--------------------|------------|-------------
groupName|String|Name of the group
customType|String|Custom type
description|String|Group description
members|String Array|List of gamertags who are members of this group
