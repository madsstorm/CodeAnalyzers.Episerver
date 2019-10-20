# Code analyzers for Episerver
[![Build Status](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_apis/build/status/CodeAnalyzers.Episerver?branchName=master)](https://dev.azure.com/madsstorm/CodeAnalyzers.Episerver/_build/latest?definitionId=2&branchName=master)
<!---
[![Nuget](https://img.shields.io/badge/nuget-v1.0-blue)](https://nuget.episerver.com/package/?id=CodeAnalyzers.Episerver)
--->

**_Analyze your code for Episerver best practices_** :heavy_check_mark:

### Description
[Roslyn](https://docs.microsoft.com/dotnet/csharp/roslyn-sdk) analyzers can produce warnings in your code as you type.

### Example
Code | Problem | Severity
-----|---------|:-------:
`[ContentType(DisplayName="StartPage")]` | :x:**_GUID_**<br>:x:**_Description_** | :no_entry:<br>:warning:
`DataFactory.Instance.GetChildren()` | **_Legacy_** | :warning:
`EPiServer.Core.Internal.DefaultContentRepository` | **_Internal_** | :warning:

<!---
### Install
`Install-Package CodeAnalyzers.Episerver`
--->

### Requirements
   :computer: Visual Studio 2019  
   :computer: Visual Studio 2017 (15.5)+
