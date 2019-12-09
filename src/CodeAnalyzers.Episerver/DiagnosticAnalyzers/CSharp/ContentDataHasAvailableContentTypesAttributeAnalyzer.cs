using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System.Linq;
using System.Collections.Generic;
namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentDataHasAvailableContentTypesAttributeAnalyzer : DiagnosticAnalyzer
    {
        // Detect content data derived from these types
        private readonly ImmutableArray<string> RootTypeNames =
            ImmutableArray.Create(
                TypeNames.PageDataMetadataName,
                TypeNames.NodeContentMetadataName,
                TypeNames.ProductContentMetadataName,
                TypeNames.PackageContentMetadataName,
                TypeNames.BundleContentMetadataName,
                TypeNames.SalesCampaignMetadataName);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi2012ContentDataShouldHaveAvailableContentTypesAttribute);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var rootTypes = RootTypeNames.Select(r => compilationContext.Compilation.GetTypeByMetadataName(r)).Where(s => s != null);

                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                var availableContentTypesType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.AvailableContentTypesMetadataName);
                if (availableContentTypesType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, iContentDataType, availableContentTypesType, rootTypes)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(
           SymbolAnalysisContext symbolContext,
           INamedTypeSymbol iContentDataType,
           INamedTypeSymbol availableContentTypesType,
           IEnumerable<INamedTypeSymbol> rootTypes)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;

            if (namedTypeSymbol.IsAbstract)
            {
                return;
            }

            if (!iContentDataType.IsAssignableFrom(namedTypeSymbol))
            {
                return;
            }

            if (!rootTypes.Any(root => root.IsAssignableFrom(namedTypeSymbol)))
            {
                return;
            }

            var attributes = namedTypeSymbol.GetAttributes();

            var availableContentTypesAttribute = attributes.FirstOrDefault(attr => availableContentTypesType.IsAssignableFrom(attr.AttributeClass));
            if (availableContentTypesAttribute is null)
            {
                ReportMissingAvailableContentTypes(symbolContext, namedTypeSymbol);
            }
        }

        private static void ReportMissingAvailableContentTypes(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                    namedType.CreateDiagnostic(
                        Descriptors.Epi2012ContentDataShouldHaveAvailableContentTypesAttribute));
        }
    }
}
