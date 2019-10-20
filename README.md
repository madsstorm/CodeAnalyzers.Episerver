# Code analyzers for Episerver
#### Automatically inspect your Episerver code for best practices

<!---
[![Nuget](https://img.shields.io/badge/nuget-v1.0-blue)](https://nuget.episerver.com/package/?id=CodeAnalyzers.Episerver)
--->

## Examples

Code | Problem | Severity
-----|---------|:-------:
`[ContentType()]`<br>`public class StartPage : PageData` | Missing GUID<br>Missing Description | :no_entry:<br>:warning:
`DataFactory.Instance.GetChildren()` | Legacy API | :warning:
`EPiServer.Core.Internal.DefaultContentRepository` | Internal namespace | :warning:

<!---
## Installation 
`Install-Package CodeAnalyzers.Episerver`
--->

## Requirements
* Visual Studio 2019
* Visual Studio 2017 (15.5)+
