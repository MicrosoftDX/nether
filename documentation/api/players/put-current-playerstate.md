# Update Current Player Extended

Updates information about currently logged in player extended.
## Request

See Common parameters and headers that are used by all requests related to the Player Management Building Block.

Method  | Request URI
------- | -----------
PUT     | `/api/playerextended`

### Request parameters

No parameters

### JSON Body

```json
{
  "gamertag": "string",
  "extendedInformation": "string"
}
```

### Response

| Status Code | Description |
|-------------|-------------|
|204|Extend Player information (e.g. JSON) updated successfully|

### JSON Body

Empty body
