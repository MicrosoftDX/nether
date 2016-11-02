![Nether Logo](https://github.com/dx-ted-emea/nether/blob/master/logos/both-logo-and-title/logo-title-1109x256.png)
# Building blocks for gaming on Azure

TODO: Put description here

master branch status

[![Build status](https://ci.appveyor.com/api/projects/status/4fgaaeakffhf32vu/branch/master?svg=true)](https://ci.appveyor.com/project/stuartleeks/nether/branch/master)

##About

Nether is an open source framework developed by TED GE EMEA team in Microsoft.


TO DO : Add description


## Documentation Index

2.	[Overview](Documentation/overview.md)
 * [Introduction - What is Nether?] (Documentation/overview.md#introduction---what-is-device-silhouette)
 * [Features](Documentation/overview.md#features)
 * [Benefits](Documentation/overview.md#benefits)
 * [Scenarios](Documentation/overview.md#scenarios)
3.	[How it works?](Documentation/howitworks.md)
 * [REST API](Documentation/RESTAPI.md)
 * Example scenarios: [Shooter Game](Documentation/lightsSampleScenario.md)
4.	[Architecture](Documentation/architecture.md)
 * [Main components](Documentation/architecture.md#main-components)
5.	[Developer guide](Documentation/developerguide.md)
 * [Set the development environment](Documentation/devenvironment.md)
 * [Configuration](Documentation/configuration.md)
 * [Providers](Documentation/developerguide.md#providers)
 * [Test](Documentation/test.md)
6.	[Deploy to production](Documentation/deployment.md)
7. [Player Management](Documentation/deployment.md)
8. [Leader Boards](Documentation/deployment.md)
9. [Analytics](Documentation/deployment.md)

## Reporting issues and feedback

If you encounter any bugs with the tool please file an issue in the Issues
section of our GitHub repo.

## Contribute Code


We welcome contributions. To contribute please follow the instrctions on
[How to contribute?](CONTRIBUTING.md)

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/)
or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.


## License

Device Silhouette is licensed under the MIT License.


## Prerequisites

### Development Machine Setup

#### Operating System

The goal of Nether is to be available on both Windows and OSX (Mac), hence any up to date version of those operating systems should be ok.

#### Visual Studio Code and/or full version of Visual Studio

All code should compile and work on both Windows and OSX hence we strive to use [Visual Studio Code](https://code.visualstudio.com) for as much as possible. We also  support [Visual Studio](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) on Windows.

#### .NET Core

Nether is built on top of .NET Core. Install from https://dot.net/core

#### MongoDB
The out-of-the-box configuration for nether requires MongoDB to be installed on your machine, and started listening on the default port.

### Facebook authentication

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


## Optional Prerequisites

### Unity

We support and build SDKs for [Unity](http://unity3d.com) but Nether as service is available from any operating system that would have access to Internet.

## Building and running Nether

Assuming you have installed the Prerequisites above, follow the steps to build and run Nether in your chosen environment below.

If you want to configure nether to use a different connection string or data store then see the [Configuration](documentation/Configuration.md) section.

### Visual Studio

To build Nether from Visual Studio, open `Nether.sln` and trigger a build. 

To run, choose either `Start` (`F5`), or `Start without Debugging` (`Ctrl+F5`) from the `Debug` menu.

### Visual Studio Code

To build Nether from Visual Studio Code, open the root of the repo and run the `build` task (`Ctrl+Shift+B`, or `Cmd+Shift+B`).

To run, either press `F5` to start with debugging or `Ctrl+F5` (`Cmd+F5`) to run without debugging

### PowerShell

To build Nether from PowerShell, ensure you have installed the dependencies from the Development Machine Setup above and run `build.ps1`


To run, set the `ASPNETCORE_ENVIRONMENT` environment variable to `Development` and navigate to `src\Nether.Web` and execute `dotnet run`

### Bash

To build Nether from Bash, ensure you have installed the dependencies from the Development Machine Setup above and run `build.sh`

To run, set the `ASPNETCORE_ENVIRONMENT` environment variable to `Development` and then navigate to `src/Nether.Web` and execute `dotnet run`

## 3rd party packages
[MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver/2.3.0-rc1)
