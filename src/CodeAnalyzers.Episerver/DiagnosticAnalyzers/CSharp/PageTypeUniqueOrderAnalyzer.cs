namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class PageTypeUniqueOrderAnalyzer : ContentTypeUniqueOrderAnalyzerBase
    {
        protected override string ContentRootTypeName => "EPiServer.Core.PageData";
    }
}
