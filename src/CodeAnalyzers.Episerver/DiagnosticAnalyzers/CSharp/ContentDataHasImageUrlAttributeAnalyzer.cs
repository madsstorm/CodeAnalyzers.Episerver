using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentDataHasImageUrlAttributeAnalyzer : DiagnosticAnalyzer
    {
        // Ignore ImageUrl attribute for content data derived from these types
        private readonly ImmutableArray<string> ExcludedRootTypeNames =
            ImmutableArray.Create(TypeNames.MediaDataMetadataName);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var excludedRootTypes =
                    ExcludedRootTypeNames.Select(root => compilationContext.Compilation.GetTypeByMetadataName(root))
                        .Where(symbol => symbol != null);

                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                var imageUrlType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.ImageUrlMetadataName);
                if (imageUrlType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, iContentDataType, imageUrlType, excludedRootTypes)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol iContentDataType,
            INamedTypeSymbol imageUrlType,
            IEnumerable<INamedTypeSymbol> excludedRootTypes)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            if(!iContentDataType.IsAssignableFrom(namedTypeSymbol))
            {
                return;
            }

            foreach(var rootType in excludedRootTypes)
            {
                if(rootType.IsAssignableFrom(namedTypeSymbol))
                {
                    return;
                }
            }

            var attributes = namedTypeSymbol.GetAttributes();

            var imageUrlAttribute = attributes.FirstOrDefault(attr => imageUrlType.IsAssignableFrom(attr.AttributeClass));
            if (imageUrlAttribute is null)
            {
                ReportInvalidImageUrl(symbolContext, namedTypeSymbol);
            }
            else
            {
                VerifyImageUrl(symbolContext, imageUrlAttribute, imageUrlType, namedTypeSymbol);
            }
        }

        private static void VerifyImageUrl(SymbolAnalysisContext symbolContext, AttributeData imageUrlAttribute, INamedTypeSymbol imageUrlType, INamedTypeSymbol namedTypeSymbol)
        {
            if (!Equals(imageUrlAttribute.AttributeClass, imageUrlType))
            {
                if (imageUrlAttribute.ConstructorArguments.IsEmpty)
                {
                    // For simplicity, assume that a derived attribute
                    // with a parameterless constructor inherits an image path
                    return;
                }
            }

            if (string.IsNullOrEmpty(imageUrlAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString()))
            {
                ReportInvalidImageUrl(symbolContext, namedTypeSymbol);
            }
        }

        private static void ReportInvalidImageUrl(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                    namedType.CreateDiagnostic(
                        Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute));
        }
    }
}
