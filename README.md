<img src="docs/icon/icon64.png" align="right" />

# Code analyzers for Episerver
![Build Status](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_apis/build/status/CodeAnalyzers.Episerver?branchName=master)
<!---
[![Nuget](https://img.shields.io/badge/nuget-v1.0-blue)][nuget]
--->

> **_Analyzes your code for Episerver best practices_**

### Intro
**Code analyzers for Episerver** are [Roslyn](https://docs.microsoft.com/dotnet/csharp/roslyn-sdk) based analyzers that produce warnings in Visual Studio as you type.

### Examples
| Code | Problem | Severity |
|-----|---------|:-------:|
| `[ContentType()]` | **_No GUID_** | :no_entry: |
| `[Display()]` | **_No Name_** | :warning: |
| `DataFactory` | **_Legacy_** | :warning: |
| `ApprovalDB` | **_Internal_** | :warning: |
| `class Block`<br>`{`<br>&nbsp;&nbsp;&nbsp;&nbsp;`ContentArea` | **_ContentArea<br>in block_** | :information_source: |

[**See all rules**](/docs/rules/rules.md)

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
