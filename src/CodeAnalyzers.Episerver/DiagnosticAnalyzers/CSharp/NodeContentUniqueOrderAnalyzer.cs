namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class NodeContentUniqueOrderAnalyzer : ContentTypeUniqueOrderAnalyzerBase
    {
        protected override string ContentRootTypeName =>
            "EPiServer.Commerce.Catalog.ContentTypes.NodeContent";
    }
}
