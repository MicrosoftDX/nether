# User Logins API (Identity)

* [List all logins for a user](#list-all-logins-for-a-user)
* [Add or update a login](#add-or-update-a-login)
* [Remove a login](#remove-a-login)

## List all logins for a user

Authorisation: requires `admin` role

```
    GET /api/identity/users/&lt;userId&gt;/logins
```

Lists a summary of all logins for a user.


### Response: 200 OK

```json
{
    "userId" : "netheruser",
    "login" : [
        {
            "providerType" : "password",
            "providerId" : "netheruser",
            "_link" : "http://.../api/identity/users/netheruser/logins/password/netheruser"
        },
        {}
    ]
}
```

The `_link` property of a login summary provides the URL to make a `DELETE` request against to remove the login for the user.

See [Get a user](users.md#get-a-user) for more information on `providerType` and `providerId`.


### Response: 404 NotFound
The user does not exist


## Add or update a login

Authorisation: requires `admin` role

```
    PUT /api/identity/users/&lt;userId&gt;/logins/&lt;providerType&gt;/&lt;providerId&gt;
```

Add a new login for a user, or update and existing login. This can be used to create or reset a user's password.

### URL Parameters
The API is designed to allow it to be used with multiple login providers, but currently it only supports the `password` provider.

Parameter | Type | Description
----------|------|------------
providerType | string | **Required**. Currently only `password` is supported for this API
providerId   | string | **Required**. The username to use for logging in (doesn't have to be the same as userId)

### Body Parameters
The format of the request body depends on the provider type:

**Password provider**
Parameter | Type | Description
----------|------|------------
password | string | The password for the user


#### Example request

```
  PUT /api/identity/users/netheruser/logins/password/netheruserlogin
```
Body:
```json
{
  "password": "SssshItIsASecret!"
}
```

### Response: 201 Created

Response contains a `Location` header with the URL for the newly created login. Issuing a `DELETE` against the header value will remove the login.

### Response: 404 NotFound
The user does not exist


## Remove a login

Authorisation: requires `admin` role

```
    DELETE /api/identity/users/&lt;userId&gt;/logins/&lt;providerType&gt;/&lt;providerId&gt;
```

### Response: 204 NoContent

The user login has been deleted

### Response: 404 NotFound
The user does not exist