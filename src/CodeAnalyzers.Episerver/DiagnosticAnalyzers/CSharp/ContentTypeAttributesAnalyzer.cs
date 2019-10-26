using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeAttributesAnalyzer : DiagnosticAnalyzer
    {
        private const string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string DescriptionArgument = "Description";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi2001ContentTypeShouldHaveDescription);

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

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, contentTypeAttribute)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(SymbolAnalysisContext symbolContext, INamedTypeSymbol contentTypeAttribute)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            var attributes = namedTypeSymbol.GetAttributes();

            var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
            if (contentAttribute is null)
            {
                return;
            }

            var descriptionArgument = contentAttribute.NamedArguments
                .FirstOrDefault(arg => string.Equals(arg.Key, DescriptionArgument, StringComparison.Ordinal));

            string description = descriptionArgument.Value.Value?.ToString();

            if (string.IsNullOrEmpty(description))
            {
                ReportInvalidDescription(symbolContext, namedTypeSymbol, contentAttribute);
            }
        }

        private void ReportInvalidDescription(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute)
        {
            var node = attribute.ApplicationSyntaxReference?.GetSyntax();
            if (node != null)
            {
                symbolContext.ReportDiagnostic(
                    node.CreateDiagnostic(
                        Descriptors.Epi2001ContentTypeShouldHaveDescription,
                        namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }
    }
}
