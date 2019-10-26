namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class BlockTypeUniqueOrderAnalyzer : ContentTypeUniqueOrderAnalyzerBase
    {
        protected override string ContentRootTypeName => "EPiServer.Core.BlockData";
    }
}
