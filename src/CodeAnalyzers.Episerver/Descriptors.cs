using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static CodeAnalyzers.Episerver.Category;

namespace CodeAnalyzers.Episerver
{
    internal enum Category
    {
        Usage
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

        internal static DiagnosticDescriptor EPI1000_AvoidUsingDataFactory { get; } =
            Rule("EPI1000", "Avoid using DataFactory", Usage, Warning,
                "Avoid using {0}", "Legacy data source in EPiServer CMS. This API has been replaced by the IContentRepository, IContentEvents and a number of related interfaces.");
    }
}
