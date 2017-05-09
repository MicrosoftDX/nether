## Development Machine Setup

The goal of Nether is to be available on both Windows and OSX (Mac), hence any up to date version of those operating systems should be ok. Its more about selecting your preferred IDE for your selected operating system.

#### IDE: Visual Studio or Visual Studio Code

Since this project strives for cross platform compatibility, we support both [Visual Studio Code](https://code.visualstudio.com) and [Visual Studio 2017](https://www.visualstudio.com/vs/). When using Visual Studio 2017, any edition (including the free community edition) will suffice.

These can be installed on your own computer, or you can use of the provided Windows Azure VM Images that already contains Visual Studio 2017 and the Azure SDK 2.9.1 or later.

#### Prerequisites

**.NET Core**

Nether is built on top of .NET Core 1.1. Make sure you install at least version 1.1 RC4 from [https://github.com/dotnet/core/blob/master/release-notes/download-archive.md](https://github.com/dotnet/core/blob/master/release-notes/download-archive.md). This is included in Visual Studio 2017. To install on Linux/OSX see the [RC4 release notes](https://github.com/dotnet/core/blob/master/release-notes/rc4-download.md)

**SQL Server**

The out-of-the-box configuration for nether uses in-memory data stores. You can configure it to run against SQL Server as per [the configuration docs](./configuration.md), and there is a partial implementation of MongoDB providers

**Azure Powershell**

Nether has been designed to take advantage of various services that are part of the Microsoft Azure platform. This includes but is not limited to hosting the REST APIs that are Nether. As such, our default "production" deployment scripts are based on Azure and currently require use of the PowerShell cmdlets for Azure.

Azure Powershell is available via the [Web Platform Installer](https://www.microsoft.com/web/downloads/platform.aspx) or by issuing the command "install-module AzureRM" from the PowerShell command line. For more about the Azure Powershell Cmdlets, please see the [official documentation](https://docs.microsoft.com/en-us/powershell/).

**Unity SDK (optional)**

We support and plan to build SDKs for [Unity](http://unity3d.com). However, since Nether is exposed as a series of easily consumable REST API's, it is accesible by almost any platform/language.

## Gamer Authentication

Nether has been built using ASP.NET Core with a pluggable identity system. Currently, there is support for Facebook authentication (more to follow). To get set up with Facebook authentication you need to register an application with Facebook then configure it for use by your instance of Nether.

For more details on configuring the identity system see the [Nether identity configuration docs](configuration/identity.md).

## Building and running Nether

Assuming you have installed the Prerequisites above, follow the steps to build and run Nether in your chosen environment below.

If you want to configure nether to use a different connection string or data store then see the [Configuration](configuration.md) section.

**Visual Studio**

To build Nether from Visual Studio, open `Nether.sln` and trigger a build of the entire solution. The first time may take some time as Visual Studio will download the necessary Nuget package. Once the build has completed, set Nether.Web as your startup project and start the solution (F5 to debug, or Ctrl+F5 to do so without debugging).

**Visual Studio Code**

To build Nether from Visual Studio Code, open the root of the repo and run the `build` task (`Ctrl+Shift+B`, or `Cmd+Shift+B`).

To run, either press F5 to start with debugging or Ctrl+F5 (Cmd+F5) to run without debugging

**PowerShell Command Line**

To build Nether from PowerShell, ensure you have installed the dependencies from the Development Machine Setup above and run `build.ps1`. Then to run, set the `ASPNETCORE_ENVIRONMENT` environment variable to `Development` and run `rundev.ps1`.

```powershell
    # PowerShell (from the folder that contains the cloned repository)
    ./build.ps1
    ./rundev.ps1
```

**Bash Command Line**

To build Nether from Bash, ensure you have installed the dependencies from the Development Machine Setup above and run `build.sh`

To run, set the `ASPNETCORE_ENVIRONMENT` environment variable to `Development` and then run `rundev.sh`.

```bash
    # bash (from the folder that contains the cloned repository)
    ./build.sh
    ./rundev.sh
```


## Entity Framework migrations

To add migrations for the Entity Framework contexts run this command from `src/Nether.Web`, with your connection string configuration in place to point to the database **in the pre-migration state**:

	dotnet ef migrations add <migration name> -c <context name> -p <project path> -o <output folder>

parameter | description
------- | -------
Migration name | Something descriptive of the changes
Context name | Name of the class with the SQL context
Project path | Path to the Project to add the migration to
Output folder | Where to place the generated migration files. (This should be a folder called Migrations under the folder for the context, e.g. Identity\Migrations) 


E.g. 

	dotnet ef migrations add InitialSqlIdentityContextMigration -c SqlIdentityContext -p ..\Nether.Data.Sql\ -o Identity\Migrations

# 3rd party packages

[MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver/2.3.0-rc1)  
[IdentityServer4](http://identityserver4.readthedocs.io/en/release/)
