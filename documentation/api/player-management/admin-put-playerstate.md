# Update Current Player Extended

Update player state for the specified player.

## Request

See Common parameters and headers that are used by all requests related to the Player Management Building Block.

Method  | Request URI
------- | -----------
PUT     | `/api/admin/players/{gamertag}/state`

### Request parameters

No parameters

### JSON Body

The body should be the new JSON state.

### Response

| Status Code | Description |
|-------------|-------------|
|200|Player state updated successfully|

### JSON Body

Empty body
