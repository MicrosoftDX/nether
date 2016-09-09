# Identity in Nether #

Any game developer should be able to add Identity support easily into their game.

The Nether Identity block provides the ability to manage Sign up and Sign in for your game easily leveraging the OpenIdConnect standard [http://openid.net/](http://openid.net/ "Open Id Connect")

We provide two different implementations for OpenIDConnect repositories:

- IdentityServer4 [https://github.com/IdentityServer/IdentityServer4](https://github.com/IdentityServer/IdentityServer4)

- Azure AD B2C: a managed service running on the Microsoft Azure cloud that provides Identity features [https://azure.microsoft.com/en-us/services/active-directory-b2c/](https://azure.microsoft.com/en-us/services/active-directory-b2c/)

![](/images/Identity-Architecture.png)

Any developer can use one of the existing [libraries for OpenIDConnect](http://openid.net/developers/libraries/) if they need to access the Nether authentication store from another language

The nether.identity.proxy is modeled after the Aspnetcore ADB2C sample published [here](https://azure.microsoft.com/en-us/documentation/samples/active-directory-dotnet-webapp-openidconnect-aspnetcore-b2c/)

The typical "Sign in" and "Sign up" workflows for a typical game app supported by the Nether.Identity implementation are depicted in the following diagrams

## Game Sign in ##
![](/images/app-signin.png)

## Game Sign up ##
![](/images/app-signup.png)

