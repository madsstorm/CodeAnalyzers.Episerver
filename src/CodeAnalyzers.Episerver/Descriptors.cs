using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static CodeAnalyzers.Episerver.Category;

namespace CodeAnalyzers.Episerver
{
    internal enum Category
    {
        Usage,        // 1xxx
        Content,      // 2xxx
        Legacy        // 3xxx
    }

    public static class Descriptors
    {
        static readonly ConcurrentDictionary<Category, string> categoryMapping = new ConcurrentDictionary<Category, string>();

        static DiagnosticDescriptor Rule(string id, string title, Category category, DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {
            return new DiagnosticDescriptor(id, title, messageFormat, categoryMapping.GetOrAdd(category, c => c.ToString()),
                defaultSeverity, isEnabledByDefault: true, description, helpLinkUri: null);
        }

        #region Usage 1xxx

        public static DiagnosticDescriptor Epi1000ContentTypeMustHaveValidGuid { get; } =
            Rule("Epi1000", "Content type must have a valid GUID", Usage, Error,
                "Missing GUID", "All content types must be identified by a valid GUID property.");

        public static DiagnosticDescriptor Epi1001ContentTypeMustHaveUniqueGuid { get; } =
            Rule("Epi1001", "Content type must have a unique GUID", Usage, Error,
                "{0} has the same GUID as {1}", "All content types must be identified by a unique GUID property.");

        public static DiagnosticDescriptor Epi1002AvoidUsingInternalNamespaces { get; } =
            Rule("Epi1002", "Avoid using internal namespaces", Usage, Warning,
                "Avoid using internal type {0}", "Internal namespaces are not considered part of the public supported API.");

        public static DiagnosticDescriptor Epi1003ContentTypeMustImplementContentData { get; } =
            Rule("Epi1003", "Content type must implement content data", Usage, Error,
                "{0} must implement a content data type", "Content types must be non-abstract classes that implement a content data type such as PageData, BlockData and MediaData.");

        #endregion

        #region Content 2xxx

        public static DiagnosticDescriptor Epi2000ContentTypeShouldHaveDisplayName { get; } =
            Rule("Epi2000", "Content type should have a DisplayName", Content, Warning,
                "Missing DisplayName", "Content types should have a readable DisplayName shown in edit view.");

        public static DiagnosticDescriptor Epi2001ContentTypeShouldHaveDescription { get; } =
            Rule("Epi2001", "Content type should have a Description", Content, Warning,
                "Missing Description", "Content types should have a Description shown in edit view.");

        public static DiagnosticDescriptor Epi2002ContentTypeShouldHaveGroupName { get; } =
            Rule("Epi2002", "Content type should have a GroupName", Content, Warning,
                "Missing GroupName", "Content types should have a GroupName for grouping in edit view.");

        public static DiagnosticDescriptor Epi2003ContentTypeShouldHaveOrder { get; } =
            Rule("Epi2003", "Content type should have an Order", Content, Warning,
                "Missing Order", "Content types should have an Order for sorting in edit view.");

        public static DiagnosticDescriptor Epi2004ContentTypeShouldHaveUniqueOrder { get; } =
            Rule("Epi2004", "Content type should have a unique Order", Content, Warning,
                "{0} has the same Order as {1}", "Content type Orders should be unique for sorting in edit view.");

        public static DiagnosticDescriptor Epi2005ContentTypeShouldHaveImageUrl { get; } =
            Rule("Epi2005", "Content type should have an ImageUrl", Content, Warning,
                "Missing ImageUrl attribute", "Content types should have an ImageUrl attribute for showing an icon in edit view.");

        #endregion

        #region Legacy 3xxx

        public static DiagnosticDescriptor Epi3000AvoidUsingDataFactory { get; } =
            Rule("Epi3000", "Avoid using legacy DataFactory", Legacy, Warning,
                "Avoid using legacy DataFactory", "Legacy data source in Episerver. This API has been replaced by the IContentRepository, IContentEvents and a number of related interfaces.");

        public static DiagnosticDescriptor Epi3001AvoidUsingCacheManager { get; } =
            Rule("Epi3001", "Avoid using legacy CacheManager", Legacy, Warning,
                "Avoid using legacy CacheManager", "Legacy cache manager in Episerver. This API has been replaced by the ISynchronizedObjectInstanceCache and IObjectInstanceCache interfaces.");

        #endregion

    }
}
