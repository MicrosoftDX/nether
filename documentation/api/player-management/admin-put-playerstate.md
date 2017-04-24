# Update Current Player Extended

Update player state for the specified player.

## Request

See Common parameters and headers that are used by all requests related to the Player Management Building Block.

Method  | Request URI
------- | -----------
PUT     | `/api/admin/players/{gamertag}/state`

### Request parameters

No parameters

### Request Body

The body should be the new player state.

### Response

| Status Code | Description |
|-------------|-------------|
|200|Player state updated successfully|

### Response Body

Empty body
