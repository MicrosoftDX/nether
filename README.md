![Nether Logo](https://github.com/dx-ted-emea/nether/blob/master/logos/both-logo-and-title/logo-title-1109x256.png)
# Building blocks for gaming on Azure

TODO: Put description here

master branch status

[![Build status](https://ci.appveyor.com/api/projects/status/4fgaaeakffhf32vu/branch/master?svg=true)](https://ci.appveyor.com/project/stuartleeks/nether/branch/master)


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
