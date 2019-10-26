namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class PackageContentUniqueOrderAnalyzer : ContentTypeUniqueOrderAnalyzerBase
    {
        protected override string ContentRootTypeName =>
            "EPiServer.Commerce.Catalog.ContentTypes.PackageContent";
    }
}
