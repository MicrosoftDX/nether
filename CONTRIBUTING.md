# Contributing

## How to contribute?

We appriciate and welcome community contributions.

1. You must sign a Contributor License Agreement (CLA) before submitting your pull request. To complete the CLA, submit a request via the form and electronically sign the CLA when you receive the email containing the link to the document. You need to complete the CLA only once to cover all Microsoft Open Technologies OSS projects. To sign a CLA please follow the instructions on [Microsoft CLA](https://cla.microsoft.com/)

1. Before starting work on a new feature, enhancement, or fix, please create an issue and optionally assign it to yourself or a developer.
2. Fork the repository and make your changes against the fork just created (and not againt master).
3. After making your changes in your fork, run tests and ensure that the code runs well on all suported platform. Ensure that your branch has no merge conflicts with master.
5. Issue a Pull Request. Please provide a short description of your contribution while creating the Pull Request.
6. The admins will review your code and may optionally request conformance, functional or other changes. Work with them to resolve any issues.
7. Upon acceptance, your code will be merged into the master branch and will become available for all.

## Building

### Windows
Run PowerShell, and execute `build.ps1`

### Bash on Linux/Mac
Run bash and execute `build.sh`

### Adding a project to build
Update `build\build-order.txt` to include the path to the folder of the project you want to build (relative to the root of the repo).


## Coding style
This project uses the [coding style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md) from the [dotnet/corefx](https://github.com/dotnet/corefx) repo.

The coding style (and C# license header) is enforced by the [CodeFormatter](https://github.com/dotnet/codeformatter) tool when building a Pull Request. If you are [running on Windows](https://github.com/dotnet/codeformatter/issues/106) then you can run the tool locally to short-circuit the feedback loop. If you're on another platform then vote up the [xplat issue](https://github.com/dotnet/codeformatter/issues/106) :-)
 
