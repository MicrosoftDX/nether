# Users Admin API (Identity)

* [List all users](#list-all-users)
* [Get a user](#get-a-user)
* [Add a user](#add-a-user)
* [Update a user](#update-a-user)
* [Remove a user](#remove-a-user)

## List all users

Authorisation: requires `admin` role

```
    GET /api/admin/users
```

Lists summary information for all users.


### Response: 200 OK

```json
{
    "users" : [
        {
            "userId" : "netheruser",
            "role" : "Player",
            "_link" : "http://.../api/admin/users/netheruser"
        },
        {}
    ]
}
```

The `_link` property of a user summary provides the URL to make a `GET` request against to get the full user information




## Get a user

Authorisation: requires `admin` role

```
    GET /api/admin/users
```

Gets details of the user and their logins.

### Response: 200 OK

``` json
{
  "user": {
    "userId": "netheruser",
    "active": true,
    "role": "Admin",
    "logins": [
      {
        "providerType": "password",
        "providerId": "netheruser",
        "_link" : "http://.../api/admin/users/netheruser/logins/password"
      }
    ]
  }
}
```


Roles: currently Admin or Player

For `logins`, the `providerType` can currently be: `password` (for username + password flow) or `facebook` for the facebook flows. The `providerId` is the identifier for the user for the given `providerType`. The provider may store additional information (e.g. the password hash for the `password` provider, but the API intentionally doesn't provide this).

Issuing a `DELETE` against the `_link` property for a login will remove the login for the user.

### Response: 404 NotFound
The user does not exist


## Add a user

Authorisation: requires `admin` role

```
    POST /api/admin/users
```

Creates a new user and assigns them a user id.

### Parameters
Parameter | Type | Description
----------|------|------------
role | string | **Required**. Specifies the user's role. Currently `Admin` and `Player` are supported
active | boolean | **Required**. Specifies whether the user is active (i.e. should be allowed to log in)



#### Example

```json
{
  "role": "Admin",
  "active": true
}
```

### Response: 201 Created

Response contains a `Location` header with the URL for the newly created user. Issuing a `GET` against the header value provides the user details.




## Update a user

Authorisation: requires `admin` role

```
    PUT /api/admin/users/<userId>
```

Updates an existing user.

### Parameters
Parameter | Type | Description
----------|------|------------
role | string | **Required**. Specifies the user's role. Currently `Admin` and `Player` are supported
active | boolean | **Required**. Specifies whether the user is active (i.e. should be allowed to log in)



#### Example

```json
{
  "role": "Admin",
  "active": false
}
```


### Response: 200 OK

The response contains the updated user details:

``` json
{
  "user": {
    "userId": "netheruser",
    "active": false,
    "role": "Admin",
    "logins": [
      {
        "providerType": "password",
        "providerId": "netheruser"
      },
      { }
    ]
  }
}
```

### Response: 404 NotFound
The user does not exist



## Remove a user

Authorisation: requires `admin` role

```
    DELETE /api/admin/users/<userId>
```

### Response: 204 NoContent

The user has been deleted

### Response: 404 NotFound
The user does not exist