using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeImageUrlAnalyzer : DiagnosticAnalyzer
    {
        private const string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string ImageUrlMetadataName = "EPiServer.DataAnnotations.ImageUrlAttribute";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi2005ContentTypeShouldHaveImageUrl);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var contentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(ContentTypeMetadataName);
                if (contentTypeAttribute is null)
                {
                    return;
                }

                var imageUrlType = compilationContext.Compilation.GetTypeByMetadataName(ImageUrlMetadataName);
                if (imageUrlType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, contentTypeAttribute, imageUrlType)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(SymbolAnalysisContext symbolContext, INamedTypeSymbol contentTypeAttribute, INamedTypeSymbol imageUrlType)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            var attributes = namedTypeSymbol.GetAttributes();

            var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
            if (contentAttribute is null)
            {
                return;
            }

            var imageUrlAttribute = attributes.FirstOrDefault(attr => imageUrlType.IsAssignableFrom(attr.AttributeClass));
            if (imageUrlAttribute is null)
            {
                ReportInvalidImageUrl(symbolContext, namedTypeSymbol, contentAttribute);
            }
            else
            {
                VerifyImageUrl(symbolContext, namedTypeSymbol, imageUrlAttribute, imageUrlType);
            }
        }

        private void VerifyImageUrl(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData imageUrlAttribute, INamedTypeSymbol imageUrlType)
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
                ReportInvalidImageUrl(symbolContext, namedTypeSymbol, imageUrlAttribute);
            }
        }

        private static void ReportInvalidImageUrl(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute)
        {
            var node = attribute.ApplicationSyntaxReference?.GetSyntax();
            if (node != null)
            {
                symbolContext.ReportDiagnostic(
                    node.CreateDiagnostic(
                        Descriptors.Epi2005ContentTypeShouldHaveImageUrl,
                        namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }
    }
}
