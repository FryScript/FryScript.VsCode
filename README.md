# Introduction 
FryScript.VsCode is a language server extension for Visual Studio Code. It provides basic syntax highlighting and checking when writing FryScript scripts.

# Getting Started
## Installation
The following software is required to build and run this project.
- NodeJs
- Visual Studio Code
- .NET 5.0 SDK

# Build and Test
## Build
Set CWD to FryScript.VsCode.Plugin. Then run the following command:

```
npm install
```

Once all necessary NodeJs dependencies have been installed run the following command:

```
npm run build-server
```

The ```build-server``` command will produce a Visual Studio Code extension that can then be used by running the following command:
```
code .
```

Once Visual Studio Code has loaded then extension can be run using the ```Launch Extension``` profile from the debug side bar.

In the Visual Studio Extension Host create a file with a .fry to enable the language server.

The created extension will run on both Window or Linux. Currently there is no option to run on MacOS.

## Test
There are currently a minimal set of unit tests for the .NET part of the language server. These can be executed by changing the CWD to the directory containing the FryScript.VsCode.sln file and running the following command:

```
dotnet test
```
