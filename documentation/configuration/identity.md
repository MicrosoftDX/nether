# Identity Configuration

The Identity configuration is all under the `Identity` section of `appsettings.json`, and is broken down into the following sections

 * [Initial Setup](#initial-setup)
 * [PlayerManagementClient](#playermanagementclient)
 * [Store](#store)
 * [Identity Server](#identity-server)
 * [SignIn Methods](#signin-methods)
 * [Clients](#clients)

## Initial setup
This section holds information that is used when the Identity services initially startup.

When the Identity services start up, they check whether there are any users in the database. If not, then an admin user is created with username `netheradmin`. The password for the user is specified in the `AdminPassword` setting. 

## PlayerManagementClient
The Identity service is responsible for the user data. For efficiency within the services, the authentication token/cookie stores the player's gamertag, which is stored by the Player Management services. There is a `default` client that the Identity services use to communicate with the Player Management services to retrieve the gamertags.

The default client has the following properties

Property name | Type | Description
--------------|------|------------
IdentityBaseUrl | string | The base URL for the identity service. This defaults to http://localhost:5000/identity, but needs to be updated for deployment | 
ApiBaseUrl | string | The base URL for the Player Management API. This defaults to http://localhost:5000/api, but needs to be updated for deployment


If you are not using the Nether Player Management services then you can create a client to integrate with your own Player Management services by implementing [IIdentityPlayerManagementClient](https://github.com/MicrosoftDX/nether/blob/master/src/Nether.Integration/Identity/IIdentityPlayerManagementClient.cs) and configure [dependency injection](dependency-injection.md) to wire it up.

## Store
The identity store defaults to an in-memory data store for ease of local configuration.

### In-memory
To configure the in-memory store, use the configuration below:

```json
  "Identity" : {
        "Store": {
            "wellknown": "in-memory"
        }
  }
```

### SQL Server
To configure the SQL Server store, use the configuration below setting the `ConnectionString` property to the connection string to your database:

```json
  "Identity" : {
        "Store": {
            "wellknown": "sql",
            "properties": {
              "ConnectionString": "<connection string>"
            }
        }
  }
```

The SQL Server implementation works with local SQL Server and [Azure SQL Database](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started). For help on getting the connection string for Azure SQL Database, see [How to get sql database connection string for Azure SQL Database?](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-develop-dotnet-simple)

## Identity Server

The Identity services us [Identity Server](https://github.com/IdentityServer/IdentityServer4), and there are a couple of pieces of configuration that are exposed as shown below:

```json
  "Identity" : {
        "IdentityServer": {
            "Authority": "http://localhost:5000/identity",
            "RequireHttps": true // This is overridden to false for dev environments but should be true for production!
        }
  }
```

Property name | Type | Description
--------------|------|------------
Authority | string | The URL for the Authority to use for the issued tokens|
RequireHttps | bool | True to require HTTPS, False to allow insecure connections. For production, set this to True. 


## SignIn Methods
To configure the sign-in methods that can be used, modify the `SignInMethods` as shown below.

Currently supported methods:
* [Facebook](#facebook)
* [Guest](#guest-authentication)
* [Username + password](#username-password-authentication)

### Facebook

To use this sign-in method you need to create a Facebook App at https://developers.facebook.com/apps/. There is a walkthrough of [creating a Facebook application](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins). When setting the `Valid OAuth Redirect URIs` set this to `http://localhost:5000/identity/signin-facebook` (or the URL for your published site)

The options for Facebook authentication can be configured under the `Identity:SignInMethods:Facebook` settings:

```json
  "Identity" : {
      "SignInMethods": {
            "Facebook": {
                "EnableImplicit": true, // Facebook in-browser, interactive flow
                "EnableAccessToken": true, // the custom facebook token flow (e.g. from Unity)
                "AppId": "",
                "AppSecret": ""
            }
      }
  }
```

Property name | Type | Description
--------------|------|------------
EnableImplicit | bool | True to enable the implicit (in-browser) login flow|
EnableAccessToken | bool | True to enable the custom token grant flow (e.g. for use from Unity)
AppId | string | The AppId for your Facebook app from https://developers.facebook.com/apps
AppSecret | string | The AppSecret for your Facebook app from https://developers.facebook.com/apps

### Guest authentication

Guest authentication is disabled by default as it is a relatively weak authentication mechanism.

To enable, set the `Identity:SignInMethods:GuestAccess:Enabled` to `true`

```json
  "Identity" : {
    "SignInMethods": {
        "GuestAccess": {
            "Enabled": true
        }
    }
  }
```


### Username password authentication

By default, sign-up via username + password is disallowed. To enable this (e.g. to allow guest users to create a username + password), set the `Identity:SignInMethods:UsernamePassword:AllowUserSignUp` to `true`

```json
  "Identity" : {
    "SignInMethods": {
        "UsernamePassword": {
            "AllowUserSignUp": true
        }
    }
  }
```



## Player Management Integration Configuration

The tokens issued by the Identity Service include the gamertag if the user is a player. To do this the Identity Service needs to interact with the Player Management service to look up gamertags.

### Default (Nether)

To use the Nether Player Management implementation, configure the default integration client:


```json
    {
        "Identity": 
        {
            "PlayerManagementClient": {
                "wellknown": "default",
                "properties": {
                    "IdentityBaseUrl": "http://localhost:5000/identity",
                    "ApiBaseUrl": "http://localhost:5000/api"
                }
            },
        }
    }

```

## Clients

**TODO**


