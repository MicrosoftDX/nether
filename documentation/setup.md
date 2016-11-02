# Prerequisites

## Development Machine Setup

### Operating System

The goal of Nether is to be available on both Windows and OSX (Mac), hence any up to date version of those operating systems should be ok.

### Visual Studio Code and/or full version of Visual Studio

All code should compile and work on both Windows and OSX hence we strive to use [Visual Studio Code](https://code.visualstudio.com) for as much as possible. We also  support [Visual Studio](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) on Windows.

### .NET Core

Nether is built on top of .NET Core. Install from https://dot.net/core

### MongoDB
The out-of-the-box configuration for nether requires MongoDB to be installed on your machine, and started listening on the default port.

## Facebook authentication

Facebook authentication requires you to create a facebook application. The ASP.NET documentation contains a section on [creating an application in facebook](https://docs.asp.net/en/latest/security/authentication/sociallogins.html#creating-the-app-in-facebook). You will need to add the application token for your facebook application to the Nether configuration.

The simple way is to add the value in the `appsettings.json`. If you are contributing to Nether then this is not recommended as it becomes very easy to commit your token to source control!

Alternatively, you can set the `Facebook:AppToken` or `Facebook__AppToken` environment variable:

```powershell
    # PowerShell
    ${env:Facebook:AppToken} = "<your token here>"
```

```bash
    # bash
    export Facebook__AppToken=<your token here>
```


Facebook user access tokens for working with Nether can be obtained from [https://developers.facebook.com/tools/accesstoken](https://developers.facebook.com/tools/accesstoken). 


# Optional Prerequisites

## Unity
