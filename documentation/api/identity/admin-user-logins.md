# User Logins Admin API (Identity)

* [List all logins for a user](#list-all-logins-for-a-user)
* [Check for a specific login type](#check-for-a-specific-login-type-for-a-user)
* [Add or update a login](#add-or-update-a-login)
* [Remove a login](#remove-a-login)

## List all logins for a user

Authorisation: requires `admin` role

```
    GET /api/admin/users/<userId>/logins
```

Lists a summary of all logins for a user.


### Response: 200 OK

```json
{
    "logins" : [
        {
            "providerType" : "password",
            "providerId" : "netheruser",
            "_link" : "http://.../api/admin/users/netheruser/logins/password"
        },
        {}
    ]
}
```

The `_link` property of a login summary provides the URL to make a `DELETE` request against to remove the login for the user.

See [Get a user](admin-users.md#get-a-user) for more information on `providerType` and `providerId`.


### Response: 404 NotFound
The user does not exist


## Check for a specific login type for a user

Authorisation: requires `admin` role

```
    GET /api/admin/users/<userId>/logins/<providerType
```

Check for a specific login type for a user


### Response: 200 OK

```json
{
    "providerType" : "password",
    "providerId" : "netheruser",
    "_link" : "http://.../api/admin/users/netheruser/logins/password"
}
```

The `_link` property of a login summary provides the URL to make a `DELETE` request against to remove the login for the user.

See [Get a user](admin-users.md#get-a-user) for more information on `providerType` and `providerId`.


### Response: 404 NotFound
The user or login does not exist


## Add or update a login

```
    PUT /api/admin/users/<userId>/logins/<providerType>
```

Add a new login for a user, or update an existing login. This can be used to create or reset a user's password.

### URL Parameters
The API is designed to allow it to be used with multiple login providers, but currently it only supports the `password` provider.

Parameter | Type | Description
----------|------|------------
providerType | string | **Required**. Currently only `password` is supported for this API

### Body Parameters
The format of the request body depends on the provider type:

**Password provider**

Parameter | Type | Description
----------|------|------------
username | string | The username
password | string | The password for the user


#### Example request

```
  PUT /api/admin/users/netheruser/logins/password
```
Body:
```json
{
  "username": "netheruserlogin",
  "password": "SssshItIsASecret!"
}
```

### Response: 201 Created

Response contains a `Location` header with the URL for the newly created login. Issuing a `DELETE` against the header value will remove the login.



## Remove a login

Authorisation: requires `admin` role

```
    DELETE /api/admin/users/<userId>/logins/<providerType>
```

### Response: 204 NoContent

The user login has been deleted

### Response: 404 NotFound
The user does not exist