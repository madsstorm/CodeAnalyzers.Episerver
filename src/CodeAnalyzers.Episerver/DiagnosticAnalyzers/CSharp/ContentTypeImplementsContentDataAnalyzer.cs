using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeImplementsContentDataAnalyzer : DiagnosticAnalyzer
    {
        // Ignore content type attribute derived from these attributes
        private readonly ImmutableArray<string> IgnoredRootAttributeNames =
            ImmutableArray.Create(TypeNames.CatalogContentTypeMetadataName);

        // Content data must not derive from these types
        private readonly ImmutableArray<string> InvalidRootDataNames =
            ImmutableArray.Create(TypeNames.CatalogContentBaseMetadataName);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1003ContentTypeMustImplementContentData);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var ignoredRootAttributes =
                    IgnoredRootAttributeNames.Select(root => compilationContext.Compilation.GetTypeByMetadataName(root))
                        .Where(symbol => symbol != null);

                var invalidRootData =
                    InvalidRootDataNames.Select(root => compilationContext.Compilation.GetTypeByMetadataName(root))
                        .Where(symbol => symbol != null);

                var contentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.ContentTypeMetadataName);
                if (contentTypeAttribute is null)
                {
                    return;
                }

                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, contentTypeAttribute, iContentDataType, ignoredRootAttributes, invalidRootData)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol contentTypeAttribute,
            INamedTypeSymbol iContentDataType,
            IEnumerable<INamedTypeSymbol> ignoredRootAttributes,
            IEnumerable<INamedTypeSymbol> invalidRootData)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            var attributes = namedTypeSymbol.GetAttributes();

            var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
            if (contentAttribute is null)
            {
                return;
            }

            foreach (var rootAttribute in ignoredRootAttributes)
            {
                if (rootAttribute.IsAssignableFrom(contentAttribute.AttributeClass))
                {
                    return;
                }
            }

            if (namedTypeSymbol.IsAbstract || !iContentDataType.IsAssignableFrom(namedTypeSymbol))
            {
                ReportInvalidContentDataType(symbolContext, namedTypeSymbol);
            }

            foreach(var rootData in invalidRootData)
            {
                if(rootData.IsAssignableFrom(namedTypeSymbol))
                {
                    ReportInvalidContentDataType(symbolContext, namedTypeSymbol);
                }
            }
        }

        private static void ReportInvalidContentDataType(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                namedType.CreateDiagnostic(
                    Descriptors.Epi1003ContentTypeMustImplementContentData,
                    namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
        }
    }
}
