using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CatalogContentDataHasCatalogContentTypeAttributeAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1006CatalogContentDataMustHaveCatalogContentTypeAttribute);

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

            if(namedTypeSymbol.IsAbstract)
            {
                return;
            }

            if (!catalogContentBaseData.IsAssignableFrom(namedTypeSymbol))
            {
                return;
            }

            var attributes = namedTypeSymbol.GetAttributes();

            var contentAttribute = attributes.FirstOrDefault(attr => catalogContentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
            if (contentAttribute is null)
            {
                ReportMissingCatalogContentTypeAttribute(symbolContext, namedTypeSymbol);
            }
        }

        private static void ReportMissingCatalogContentTypeAttribute(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                namedType.CreateDiagnostic(
                    Descriptors.Epi1006CatalogContentDataMustHaveCatalogContentTypeAttribute));
        }
    }
}
