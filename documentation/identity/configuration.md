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


If you are not using the Nether Player Management services then you can create a client to integrate with your own Player Management services by implementing [IIdentityPlayerManagementClient](https://github.com/MicrosoftDX/nether/blob/master/src/Nether.Integration/Identity/IIdentityPlayerManagementClient.cs) and configure [dependency injection](../configuration-dependency-injection.md) to wire it up.

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
* [Facebook](#Facebook) - in-browser, interactive Facebook login
* [FacebookUserAccessToken](#FacebookUserAccessToken) - supports token translation from a Facebook user access token to a Nether token


```json
  "Identity" : {
      "SignInMethods": {
            // Facebook in-browser, interactive flow
            "Facebook": {
                "Enabled": false,
                "AppId": "",
                "AppSecret": ""
            },
            // the custom facebook token flow (e.g. from Unity)
            "FacebookUserAccessToken": {
                "Enabled": false,
                "AppToken": ""
            }
      }
  }
```

### Facebook
To use this sign-in method you need to create a Facebook App at https://developers.facebook.com/apps/. There is a walkthrough of [creating a Facebook application](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins). When setting the `Valid OAuth Redirect URIs` set this to `http://localhost:5000/identity/signin-facebook` (or the URL for your published site)

Property name | Type | Description
--------------|------|------------
Enabled | bool | True to enable this login method|
AppId | string | The AppId for your Facebook app from https://developers.facebook.com/apps
AppSecret | string | The AppSecret for your Facebook app from https://developers.facebook.com/apps

### FacebookUserAccessToken
To use this sign-in method you need to create a Facebook App at https://developers.facebook.com/apps/. There is a walkthrough of [creating a Facebook application](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins). 

Property name | Type | Description
--------------|------|------------
Enabled | bool | True to enable this login method|
AppToken | string | The AppToken for your Facebook app from https://developers.facebook.com/tools/accesstoken


## Clients

**TODO**


