using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CatalogContentTypeImplementsCatalogContentDataAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1005CatalogContentTypeShouldImplementCatalogContentData);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var catalogContentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.CatalogContentTypeMetadataName);
                if (catalogContentTypeAttribute is null)
                {
                    return;
                }

                var catalogContentBaseData = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.CatalogContentBaseMetadataName);
                if (catalogContentBaseData is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, catalogContentTypeAttribute, catalogContentBaseData)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol catalogContentTypeAttribute,
            INamedTypeSymbol catalogContentBaseData)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            var attributes = namedTypeSymbol.GetAttributes();

            var contentAttribute = attributes.FirstOrDefault(attr => catalogContentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
            if (contentAttribute is null)
            {
                return;
            }

            if (namedTypeSymbol.IsAbstract || !catalogContentBaseData.IsAssignableFrom(namedTypeSymbol))
            {
                ReportInvalidCatalogContentDataType(symbolContext, namedTypeSymbol);
            }
        }

        private static void ReportInvalidCatalogContentDataType(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                namedType.CreateDiagnostic(
                    Descriptors.Epi1005CatalogContentTypeShouldImplementCatalogContentData,
                    namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
        }
    }
}
