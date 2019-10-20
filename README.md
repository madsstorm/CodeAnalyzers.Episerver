# Code analyzers for Episerver
#### Automatically inspect your Episerver code for best practices
[![Build Status](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_apis/build/status/CodeAnalyzers.Episerver?branchName=master)](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_build/latest?definitionId=2&branchName=master)
<!---
[![Nuget](https://img.shields.io/badge/nuget-v1.0-blue)](https://nuget.episerver.com/package/?id=CodeAnalyzers.Episerver)
--->

### What is it?
Roslyn based analyzers give you warnings and errors in Visual Studio as you type.

### Examples
Code | Problem | Severity
-----|---------|:-------:
`[ContentType()]`<br>`public class StartPage : PageData` | **_Missing GUID_**<br>**_Missing Description_** | :no_entry:<br>:warning:
`DataFactory.Instance.GetChildren()` | **_Legacy API_** | :warning:
`EPiServer.Core.Internal.DefaultContentRepository` | **_Internal namespace_** | :warning:

<!---
### Installation 
`Install-Package CodeAnalyzers.Episerver`
--->

### Requirements
   :computer: Visual Studio 2019  
   :computer: Visual Studio 2017 (15.5)+
