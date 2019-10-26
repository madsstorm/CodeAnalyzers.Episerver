namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class VariationContentUniqueOrderAnalyzer : ContentTypeUniqueOrderAnalyzerBase
    {
        protected override string ContentRootTypeName =>
            "EPiServer.Commerce.Catalog.ContentTypes.VariationContent";
    }
}
