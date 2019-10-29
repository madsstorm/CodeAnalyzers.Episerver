using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentDataHasContentTypeAttributeAnalyzer : DiagnosticAnalyzer
    {
        // Ignore content data derived from these types
        private readonly ImmutableArray<string> IgnoredRootDataNames =
            ImmutableArray.Create(TypeNames.CatalogContentBaseMetadataName);

        // Content type attribute must not derive from these attributes
        private readonly ImmutableArray<string> InvalidRootAttributeNames =
            ImmutableArray.Create(TypeNames.CatalogContentTypeMetadataName);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var ignoredRootData =
                    IgnoredRootDataNames.Select(root => compilationContext.Compilation.GetTypeByMetadataName(root))
                        .Where(symbol => symbol != null);

                var invalidRootAttributes =
                    InvalidRootAttributeNames.Select(root => compilationContext.Compilation.GetTypeByMetadataName(root))
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
                    symbolContext => AnalyzeSymbol(symbolContext, contentTypeAttribute, iContentDataType, ignoredRootData, invalidRootAttributes)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol contentTypeAttribute,
            INamedTypeSymbol iContentDataType,
            IEnumerable<INamedTypeSymbol> ignoredRootData,
            IEnumerable<INamedTypeSymbol> invalidRootAttributes)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;

            if(namedTypeSymbol.IsAbstract)
            {
                return;
            }

            if(!iContentDataType.IsAssignableFrom(namedTypeSymbol))
            {
                return;
            }

            foreach (var rootData in ignoredRootData)
            {
                if(rootData.IsAssignableFrom(namedTypeSymbol))
                {
                    return;
                }
            }

            var attributes = namedTypeSymbol.GetAttributes();

            var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
            if (contentAttribute is null)
            {
                ReportMissingContentTypeAttribute(symbolContext, namedTypeSymbol);
            }
            else
            {
                foreach (var rootAttribute in invalidRootAttributes)
                {
                    if (rootAttribute.IsAssignableFrom(contentAttribute.AttributeClass))
                    {
                        ReportMissingContentTypeAttribute(symbolContext, namedTypeSymbol);
                    }
                }
            }
        }

        private static void ReportMissingContentTypeAttribute(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                namedType.CreateDiagnostic(
                    Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute));
        }
    }
}
