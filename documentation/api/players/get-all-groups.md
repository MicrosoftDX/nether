# Get All Groups

Get a list of all groups.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/groups`

### JSON Body

Empty body

### Response

| Status Code | Description |
|-------------|-------------|
|200|Success|

### JSON Body

```json
{
  "groups": [
    {
      "name": "string",
      "customType": "string",
      "description": "string"
    }
  ]
}
```

Element name        | Type       | Description
--------------------|------------|-------------
groupName|String|Name of the group
customType|String|Custom type
description|String|Group description
