<img src="docs/icon/icon64.png" align="right" />

# Code analyzers for Episerver
[![Build Status](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_apis/build/status/CodeAnalyzers.Episerver?branchName=master)](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_build/latest?definitionId=2&branchName=master)
<!---
[![Nuget](https://img.shields.io/badge/nuget-v1.0-blue)][nuget]
--->

> **_Analyzes your code for Episerver best practices_**

### Intro
**Code analyzers for Episerver** are [Roslyn](https://docs.microsoft.com/dotnet/csharp/roslyn-sdk) based analyzers that produce warnings in Visual Studio as you type.

### Example rules
Code | Problem | Severity
-----|---------|:-------:
`[ContentType(DisplayName="StartPage")]` | **_No GUID_**<br>**_No Description_** | :no_entry:<br>:warning:
`DataFactory.Instance.GetChildren()` | **_Legacy type_** | :warning:
`EPiServer.Core.Internal.DefaultContentRepository` | **_Internal type_** | :warning:

[See all rules](/docs/rules/rules.md)

<!---
### Install
`Install-Package CodeAnalyzers.Episerver`

_Available at [nuget.episerver.com][nuget]_
--->

### Configure
Analyzer rule severity can be configured in a [ruleset file](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules).

### Requirements
[![vs2019](docs/icon/vs2019.png)](#.#) <sup>**Visual Studio 2019**</sup>

[![vs2017](docs/icon/vs2017.png)](#.#) <sup>**Visual Studio 2017 (15.5)+**</sup>

[nuget]: https://nuget.episerver.com/package/?id=CodeAnalyzers.Episerver
