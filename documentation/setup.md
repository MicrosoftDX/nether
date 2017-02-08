# Prerequisites

## Development Machine Setup

### Operating System

The goal of Nether is to be available on both Windows and OSX (Mac), hence any up to date version of those operating systems should be ok.

### Visual Studio Code and/or full version of Visual Studio

All code should compile and work on both Windows and OSX hence we strive to use [Visual Studio Code](https://code.visualstudio.com) for as much as possible. We also  support Visual Studio 2017 on Windows. To use Visual Studio, you will need to have Visual Studio 2017 installed, which is currently a [Release Candidate](https://www.visualstudio.com/vs/visual-studio-2017-rc/). Make sure you have the .NET Core and Web tooling installed.

### .NET Core

Nether is built on top of .NET Core 1.1. Install from https://dot.net/core. The current RC of Visual Studio installs SDK version 1.0.0-rc4-004771. To install on Linux/OSX see the [RC4 release notes](https://github.com/dotnet/core/blob/master/release-notes/rc4-download.md)

### SQL Server
The out-of-the-box configuration for nether uses in-memory data stores. You can configure it to run against SQL Server as per [the configuration docs](./configuration.md), and there is a partial implementation of MongoDB providers

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

We support and plan to build SDKs for [Unity](http://unity3d.com) but Nether as service is available from any operating system that would have access to Internet.

# Building and running Nether

Assuming you have installed the Prerequisites above, follow the steps to build and run Nether in your chosen environment below.

If you want to configure nether to use a different connection string or data store then see the [Configuration](documentation/configuration.md) section.

## Visual Studio

Due to issues with the current version of ASP.NET Core tooling we are using, run `build.ps1` in the source root before loading the solution in Visual Studio.

To build Nether from Visual Studio, open `Nether.sln` and trigger a build. 

To run, choose either `Start` (`F5`), or `Start without Debugging` (`Ctrl+F5`) from the `Debug` menu.

## Visual Studio Code

To build Nether from Visual Studio Code, open the root of the repo and run the `build` task (`Ctrl+Shift+B`, or `Cmd+Shift+B`).

To run, either press `F5` to start with debugging or `Ctrl+F5` (`Cmd+F5`) to run without debugging

## PowerShell

To build Nether from PowerShell, ensure you have installed the dependencies from the Development Machine Setup above and run `build.ps1`


To run, set the `ASPNETCORE_ENVIRONMENT` environment variable to `Development` and navigate to `src\Nether.Web` and execute `dotnet run`

## Bash

To build Nether from Bash, ensure you have installed the dependencies from the Development Machine Setup above and run `build.sh`

To run, set the `ASPNETCORE_ENVIRONMENT` environment variable to `Development` and then navigate to `src/Nether.Web` and execute `dotnet run`

# 3rd party packages
[MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver/2.3.0-rc1)
[IdentityServer4](http://identityserver4.readthedocs.io/en/release/)
