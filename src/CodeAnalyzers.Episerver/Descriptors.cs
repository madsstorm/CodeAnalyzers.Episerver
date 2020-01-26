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
                "Missing GUID", "All content types must be identified by a valid GUID property. If this content type has already been deployed to a live environment, you must find its existing GUID in Episerver Admin mode or the Episerver database.");

        public static DiagnosticDescriptor Epi1001ContentTypeMustHaveUniqueGuid { get; } =
            Rule("Epi1001", "Content type must have a unique GUID", Usage, Error,
                "{0} has the same GUID as {1}", "All content types must be identified by a unique GUID property.");

        public static DiagnosticDescriptor Epi1002AvoidUsingInternalNamespaces { get; } =
            Rule("Epi1002", "Avoid using Internal namespaces", Usage, Warning,
                "Avoid using internal type {0}", "Internal namespaces are not considered part of the public supported API.");

        public static DiagnosticDescriptor Epi1003ContentTypeShouldImplementContentData { get; } =
            Rule("Epi1003", "Content type should implement content data", Usage, Warning,
                "{0} should implement a content data type", "Content types should be non-abstract classes that implement a content data type such as PageData, BlockData or MediaData.");

        public static DiagnosticDescriptor Epi1004ContentDataShouldHaveContentTypeAttribute { get; } =
            Rule("Epi1004", "Content data should have ContentType attribute", Usage, Warning,
                "Missing ContentType attribute. Did you mean to make an abstract base class ?", "The ContentType attribute should be used for all non-abstract content data types.");

        public static DiagnosticDescriptor Epi1005CatalogContentTypeShouldImplementCatalogContentData { get; } =
            Rule("Epi1005", "Catalog content type should implement catalog content data", Usage, Warning,
                "{0} should implement a catalog content data type", "Catalog content types should be non-abstract classes that implement a catalog content data type such as ProductContent or VariationContent.");

        public static DiagnosticDescriptor Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute { get; } =
            Rule("Epi1006", "Catalog content data should have CatalogContentType attribute", Usage, Warning,
                "Missing CatalogContentType attribute. Did you mean to make an abstract base class ?", "The CatalogContentType attribute should be used for all non-abstract catalog content data types.");

        public static DiagnosticDescriptor Epi1007AvoidUsingConcreteOrderClasses { get; } =
            Rule("Epi1007", "Avoid using concrete Order classes", Usage, Warning,
                "Avoid using concrete type {0}", "Use order abstraction interfaces like IPurchaseOrder, ILineItem and similar interfaces.");

        public static DiagnosticDescriptor Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor { get; } =
            Rule("Epi1008", "Interface in AllowedTypes should have UIDescriptor", Usage, Warning,
                "Missing UIDescriptor for {0}", "Interfaces used in AllowedTypes should have a UIDescriptor.");

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

        public static DiagnosticDescriptor Epi2005ContentDataShouldHaveImageUrlAttribute { get; } =
            Rule("Epi2005", "Content should have an ImageUrl attribute", Content, Warning,
                "Missing ImageUrl attribute", "Content data should have an ImageUrl attribute for showing an icon in edit view.");

        public static DiagnosticDescriptor Epi2006ContentPropertyShouldHaveName { get; } =
            Rule("Epi2006", "Content property should have a Name", Content, Warning,
                "Missing Name", "Content properties should have a Name shown in edit view.");

        public static DiagnosticDescriptor Epi2007ContentPropertyShouldHaveDescription { get; } =
            Rule("Epi2007", "Content property should have a Description", Content, Warning,
                "Missing Description", "Content properties should have a Description shown in edit view.");

        public static DiagnosticDescriptor Epi2008ContentPropertyShouldHaveGroupName { get; } =
            Rule("Epi2008", "Content property should have a GroupName", Content, Warning,
                "Missing GroupName", "Content properties should have a GroupName for organizing in tabs in edit view.");

        public static DiagnosticDescriptor Epi2009ContentPropertyShouldHaveOrder { get; } =
            Rule("Epi2009", "Content property should have an Order", Content, Warning,
                "Missing Order", "Content properties should have an Order for sorting in edit view.");

        public static DiagnosticDescriptor Epi2010ContentPropertyShouldHaveUniqueOrder { get; } =
            Rule("Epi2010", "Content property should have a unique Order", Content, Warning,
                "{0} has the same Order as {1}", "Content property Orders should be unique for sorting in edit view.");

        public static DiagnosticDescriptor Epi2011AvoidContentAreaPropertyInBlock { get; } =
            Rule("Epi2011", "Avoid ContentArea property in block", Content, Info,
                "Avoid ContentArea property in block", "ContentArea properties in blocks make it difficult for editors to manage nested content.");

        public static DiagnosticDescriptor Epi2012PageDataShouldHaveAvailableContentTypesAttribute { get; } =
            Rule("Epi2012", "Page should have an AvailableContentTypes attribute", Content, Info,
                "Missing AvailableContentTypes attribute", "Page data should have an AvailableContentTypes attribute for restricting child content types.");

        public static DiagnosticDescriptor Epi2013CommerceContentDataShouldHaveAvailableContentTypesAttribute { get; } =
            Rule("Epi2013", "Commerce content should have an AvailableContentTypes attribute", Content, Info,
                "Missing AvailableContentTypes attribute", "Commerce content should have an AvailableContentTypes attribute for restricting child content types.");

        public static DiagnosticDescriptor Epi2014ContentReferencePropertyShouldHaveAllowedTypesAttribute { get; } =
            Rule("Epi2014", "ContentReference property should have an AllowedTypes attribute", Content, Info,
                "Missing AllowedTypes attribute", "ContentReference properties should have an AllowedTypes attribute for restricting selectable content types.");

        public static DiagnosticDescriptor Epi2015ContentAreaPropertyShouldHaveAllowedTypesAttribute { get; } =
            Rule("Epi2015", "ContentArea property should have an AllowedTypes attribute", Content, Info,
                "Missing AllowedTypes attribute", "ContentArea properties should have an AllowedTypes attribute for restricting selectable content types.");

        public static DiagnosticDescriptor Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute { get; } =
            Rule("Epi2016", "ContentReferenceList property should have an AllowedTypes attribute", Content, Info,
                "Missing AllowedTypes attribute", "ContentReferenceList properties should have an AllowedTypes attribute for restricting selectable content types.");

        #endregion

        #region Legacy 3xxx

        public static DiagnosticDescriptor Epi3000AvoidUsingDataFactory { get; } =
            Rule("Epi3000", "Avoid using legacy DataFactory", Legacy, Warning,
                "Avoid using legacy DataFactory", "Legacy data source in Episerver. This API has been replaced by the IContentRepository, IContentEvents and a number of related interfaces.");

        public static DiagnosticDescriptor Epi3001AvoidUsingCacheManager { get; } =
            Rule("Epi3001", "Avoid using legacy CacheManager", Legacy, Warning,
                "Avoid using legacy CacheManager", "Legacy cache manager in Episerver. This API has been replaced by the ISynchronizedObjectInstanceCache and IObjectInstanceCache interfaces.");

        public static DiagnosticDescriptor Epi3002AvoidUsingLog4NetLogManager { get; } =
            Rule("Epi3002", "Avoid using legacy log4net.LogManager", Legacy, Warning,
                "Avoid using legacy log4net.LogManager", "Use the log abstraction EPiServer.Logging.LogManager");

        public static DiagnosticDescriptor Epi3003AvoidUsingOrderContext { get; } =
            Rule("Epi3003", "Avoid using legacy OrderContext", Legacy, Warning,
                "Avoid using legacy OrderContext", "This API has been replaced by the IOrderRepository, IOrderEvents and IOrderSearchService interfaces.");

        #endregion

    }
}
