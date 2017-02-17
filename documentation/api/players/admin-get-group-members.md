# Get Group Members

Get a list of all members of a group as a list of gamertags.

## Request

See Common parameters and headers that are used by all requests related to the Leaderboard Building Block.

Method  | Request URI
------- | -----------
GET     | `/api/admin/groups/{groupName}/players`

### JSON Body

Empty body

### Response

| Status Code | Description |
|-------------|-------------|
|200|Success|

### JSON Body

```json
{
  "gamertags": [
    "string"
  ]
}
```

Element name        | Type       | Description
--------------------|------------|-------------
gamertags|String Array|Array of gamertags
