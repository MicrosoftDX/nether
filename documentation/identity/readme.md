# Identity in Nether #

Any game developer should be able to add Identity support easily into their game.

## Configuring Nether Identity
WARNING: The identity implementation in still under development, so expect these details to change :-)

### Configuring facebook authentication

Information on configuring facebook sign in is in the [configuration docs](../configuration/identity.md)

### Configuring users
The Users and Logins APIs allow you to programmatically add users. Additionally, the facebook custom flow creates users in the player role based on a facebook user acces token.

The users for testing with the in-memory  are currently created in `Nether.Web/Features/Identity/Configuration/Users.cs` as a temporary workaround.

When Nether initially starts up it performs a check to see whether there are any users in the user store. If not then it will pre-create a user with username `netheradmin` and password specified in the config under `Identity:InitialSetup:AdminPassword`. It is strongly recommended that you change this password when you deploy.

## Authenticating with Nether Identity
The Nether Identity endpoint currently supports two means of authenticating: facebook user token and resource owner password (i.e. user name and password).
The facebook user token authentication uses a custom grant flow (`fb-usertoken`) to allow a non-interactive flow that receives a facebook user token (e.g. as per the facebook Unity extension) and converting it to a Nether token.
The resource owner password flow allows a user to sign in with user name and password combination. The main drivers for this flow are integration and load testing.


Since we don't currently have the client SDK for Nether created, I have created two sample projects showing how you can authenticate against the Nether Identity endpoint to get a token to use with other Nether APIs (under `/src/TestClients`):

* FacebookUserTokenClient
* IdentityServerTestClient

### FacebookUserTokenClient
This sample shows how to take a facebook user token (as available [here](https://developers.facebook.com/tools/accesstoken) for testing).
It has minimal dependencies, so will hopefully be easiest to make available to work across target environments

The `GetAccessTokenAsync` method performs the authentication call using the custom `fb-usertoken` authentication flow


### IdentityServerTestClient
This sample was created to show the different flows that are currently implemented:
* ClientCredentials - identifies the client application (not the user). Was useful as a validation step in the implementation
* ResourceOwnerPassword - signs in with a user name and password
* CustomGrant - signs in with a facebook user accesstoken

For speed of coding, this project used the [IdentityModel](https://www.nuget.org/packages/IdentityModel/) client library from IdentityServer, but this isn't a requirement for clients.  


## Game Sign in ##
![](images/app-signin.png)

## Game Sign up ##
![](images/app-signup.png)


