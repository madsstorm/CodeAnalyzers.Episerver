<img src="docs/icon/icon64.png" align="right" />

# Code analyzers for Episerver
![Build Status](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_apis/build/status/CodeAnalyzers.Episerver?branchName=master)
<!---
[![Nuget](https://img.shields.io/badge/nuget-v1.0-blue)][nuget]
--->

> **_Analyzes your code for Episerver best practices_**

## Intro
**Code analyzers for Episerver** are [Roslyn](https://docs.microsoft.com/visualstudio/extensibility/getting-started-with-roslyn-analyzers) based analyzers that look for Episerver best practices and produce warnings ("squiggles") in Visual Studio as you type.
The analyzers are installed per-project via a NuGet package, and are also executed by build servers.

## Examples
| Code | Problem | Severity |
|-----|---------|:-------:|
| `[ContentType(DisplayName="...")]` | **_Missing GUID_** | :no_entry: |
| `[Display(Name="...")]` | **_Missing Description_** | :warning: |
| `DataFactory.Instance` | **_Legacy type_** | :warning: |
| `ApprovalDB.SaveAsync()` | **_Internal type_** | :warning: |
| `public class Block : BlockData`<br>`{`<br>&nbsp;&nbsp;&nbsp;&nbsp;`public virtual ContentArea ...` | **_ContentArea<br>in Block_** | :information_source: |

[**See all analyzer rules**](/docs/rules/rules.md)

<!---
### Install
`Install-Package CodeAnalyzers.Episerver`

_Available at [nuget.episerver.com][nuget]_
--->

## Configure
Analyzer rule severity can be configured in a [ruleset file](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules).

## Compatibility
[![vs2019](docs/icon/vs2019.png)](#.#) **Visual Studio 2019**

[![vs2017](docs/icon/vs2017.png)](#.#) **Visual Studio 2017 (15.5)+**

[![compiler](docs/icon/nuget.png)](#.#) **Microsoft.Net.Compilers 2.6.1+**

[nuget]: https://nuget.episerver.com/package/?id=CodeAnalyzers.Episerver
