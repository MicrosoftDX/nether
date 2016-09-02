# Nether - A set of open sourced building blocks for gaming on Azure

TODO: Put description here

master branch status

[![Build status](https://ci.appveyor.com/api/projects/status/4fgaaeakffhf32vu/branch/master?svg=true)](https://ci.appveyor.com/project/stuartleeks/nether/branch/master)


## Prerequisites

### Development Machine Setup

#### Operating System

The goal of Nether is to be available on both Windows and OSX (Mac), hence any up to date version of those operating systems should be ok.

#### Visual Studio Code and/or full version of Visual Studio

All code should compile and work on both Windows and OSX hence we strive to use [Visual Studio Code](https://code.visualstudio.com) for as much as possible. With that said we also do support the full version of Visual Studio on Windows.

#### .NET Core

Nether is built on top of .NET Core. Install from https://dot.net

## Optional Prerequisites

### [Unity](http://unity3d.com)

We support and build SDKs for Unity but Nether as service is available from any operating system that would have access to Internet.

## Building Nether

### Visual Studio

To build Nether from Visual Studio, open `Nether.sln` and trigger a build

### PowerShell

To build Nether from PowerShell, ensure you have installed the dependencies from the Development Machine Setup above and run `build.ps1`

### Bash

To build Nether from Bash, ensure you have installed the dependencies from the Development Machine Setup above and run `build.sh`

## Packages
MongoDB.Driver 2.3.0-rc1