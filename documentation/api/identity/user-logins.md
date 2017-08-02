# User Logins API (Identity)

* [List all logins for the current user](#list-all-logins-for-the-current-user)
* [Check for a specific login type](#check-for-a-specific-login-type-for-the-current-user)
* [Add or update a login](#add-or-update-a-login)
* [Remove a login](#remove-a-login)

## List all logins for the current user

```
    GET /api/users/logins
```

Lists a summary of all logins for the current user.


### Response: 200 OK

```json
{
    "logins" : [
        {
            "providerType" : "password",
            "providerId" : "netheruser",
            "_link" : "http://.../api/user/logins/password"
        },
        {}
    ]
}
```

The `_link` property of a login summary provides the URL to make a `DELETE` request against to remove the login for the user.


## Check for a specific login type for the current user

```
    GET /api/user/logins/<providerType>
```

Check for a specific login type for the current user


### Response: 200 OK

```json
{
    "providerType" : "password",
    "providerId" : "netheruser",
    "_link" : "http://.../api/user/logins/password"
}
```

The `_link` property of a login summary provides the URL to make a `DELETE` request against to remove the login for the user.



### Response: 404 NotFound
The login type does not exist for the current user



## Add or update a login

```
    PUT /api/user/logins/<providerType>
```

Add a new login for the current user, or update an existing login. This can be used to create or reset a user's password.

### URL Parameters

Parameter | Type | Description
----------|------|------------
providerType | string | **Required**. Currently supported providers are `password` and `facebook` (subject to being enabled in [configuration](../../configuration/identity.md))

### Body Parameters
The format of the request body depends on the provider type:

**Password provider**

Parameter | Type | Description
----------|------|------------
username | string | The username
password | string | The password for the user

**Facebook provider**

Parameter | Type | Description
----------|------|------------
facebookToken | string | The facebook user access token for the user (e.g. as obtained via the facebook JS SDK, or Unity SDK)


#### Example request

```
  PUT /api/user/logins/password
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


```
    DELETE /api/user/logins/<providerType>
```

### Response: 204 NoContent

The user login has been deleted

### Response: 404 NotFound
The login does not exist