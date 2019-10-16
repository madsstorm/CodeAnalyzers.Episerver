using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static CodeAnalyzers.Episerver.Category;

#if DEBUG
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("CodeAnalyzers.Episerver.Test")]
#endif

namespace CodeAnalyzers.Episerver
{
    internal enum Category
    {
        Usage,
        Editing
    }

    internal static class Descriptors
    {
        static ConcurrentDictionary<Category, string> categoryMapping = new ConcurrentDictionary<Category, string>();

        static DiagnosticDescriptor Rule(string id, string title, Category category, DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {
            var isEnabledByDefault = true;
            string helpLinkUri = "https://world.episerver.com/documentation";
            return new DiagnosticDescriptor(id, title, messageFormat, categoryMapping.GetOrAdd(category, c => c.ToString()), defaultSeverity, isEnabledByDefault, description, helpLinkUri);
        }

        internal static DiagnosticDescriptor CAE1000_AvoidUsingDataFactory { get; } =
            Rule("CAE1000", "Avoid using DataFactory", Usage, Warning,
                "Avoid using EPiServer.DataFactory", "Legacy data source in EPiServer CMS. This API has been replaced by the IContentRepository, IContentEvents and a number of related interfaces.");

        internal static DiagnosticDescriptor CAE1001_AvoidUsingInternalNamespaces { get; } =
            Rule("CAE1001", "Avoid using internal namespaces", Usage, Warning,
                "Avoid using namespace {0}", "Internal namespaces are not considered part of the public supported API.");

        internal static DiagnosticDescriptor CAE1002_ContentTypeMustHaveGuid { get; } =
            Rule("CAE1002", "Content type must have a valid GUID attribute", Usage, Error,
                "{0} does not have a valid GUID attribute");
    }
}
