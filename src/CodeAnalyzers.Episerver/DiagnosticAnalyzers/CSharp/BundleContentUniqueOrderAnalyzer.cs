namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class BundleContentUniqueOrderAnalyzer : ContentTypeUniqueOrderAnalyzerBase
    {
        protected override string ContentRootTypeName =>
            "EPiServer.Commerce.Catalog.ContentTypes.BundleContent";
    }
}
