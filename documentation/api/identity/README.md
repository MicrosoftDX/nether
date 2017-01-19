# Users API (Identity)

* [List all users](#list-all-users)
* [Get user](#get-user)


## List all users

Authorisation: requires `admin` role

```
    GET /api/identity/users
```

Lists summary information for all users.


### Response: 200 OK

```json
{
    "users" : [
        {
            "userId" : "netheruser",
            "role" : "Player",
            "_link" : "http://.../api/identity/users/netheruser"
        },
        {}
    ]
}
```

The `_link` property of a user summary provides the URL to make a `GET` request against to get the full user information




## Get user

Authorisation: requires `admin` role

```
    GET /api/identity/users
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
        "providerData": "...",
      }
    ]
  }
}
```


Roles: currently Admin or Player

For `logins`, the `providerType` is currently `password` (for username + password flow) or `facebook` for the facebook user access token flow. The meaning of `providerId` and `providerData` are dependent on the `providerType`



