# Contributing

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
 