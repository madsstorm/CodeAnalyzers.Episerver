using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static CodeAnalyzers.Episerver.Category;

namespace CodeAnalyzers.Episerver
{
    internal enum Category
    {
        Usage,        // 1xxx
        Content       // 2xxx
    }

    public static class Descriptors
    {
        static readonly ConcurrentDictionary<Category, string> categoryMapping = new ConcurrentDictionary<Category, string>();

        static DiagnosticDescriptor Rule(string id, string title, Category category, DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {
            return new DiagnosticDescriptor(id, title, messageFormat,
                categoryMapping.GetOrAdd(category, c => c.ToString()), defaultSeverity, isEnabledByDefault: true, description, helpLinkUri: null);
        }

        public static DiagnosticDescriptor Epi1000AvoidUsingDataFactory { get; } =
            Rule("Epi1000", "Avoid using DataFactory", Usage, Warning,
                "Avoid using EPiServer.DataFactory", "Legacy data source in Episerver. This API has been replaced by the IContentRepository, IContentEvents and a number of related interfaces.");

        public static DiagnosticDescriptor Epi1001AvoidUsingInternalNamespaces { get; } =
            Rule("Epi1001", "Avoid using internal namespaces", Usage, Warning,
                "Avoid using namespace {0}", "Internal namespaces are not considered part of the public supported API.");

        public static DiagnosticDescriptor Epi1002AvoidUsingCacheManager { get; } =
            Rule("Epi1002", "Avoid using CacheManager", Usage, Warning,
                "Avoid using EPiServer.CacheManager", "Legacy cache manager in Episerver. This API has been replaced by the ISynchronizedObjectInstanceCache and IObjectInstanceCache interfaces.");

        public static DiagnosticDescriptor Epi2000ContentTypeMustHaveGuid { get; } =
            Rule("Epi2000", "Content type must have a valid GUID attribute", Content, Error,
                "{0} does not have a valid GUID attribute", "All content types must be identified by a unique GUID attribute.");
    }
}
