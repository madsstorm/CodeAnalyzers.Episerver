namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class ProductContentUniqueOrderAnalyzer : ContentTypeUniqueOrderAnalyzerBase
    {
        protected override string ContentRootTypeName =>
            "EPiServer.Commerce.Catalog.ContentTypes.ProductContent";
    }
}
