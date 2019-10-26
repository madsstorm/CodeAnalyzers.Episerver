using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private const string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";

        private readonly ImmutableArray<(string ArgumentName, DiagnosticDescriptor Descriptor, bool AssumeInherited)> ContentTypeArguments =
            ImmutableArray.Create(
                ("DisplayName", Descriptors.Epi2000ContentTypeShouldHaveDisplayName, false),
                ("Description", Descriptors.Epi2001ContentTypeShouldHaveDescription, false),
                ("GroupName", Descriptors.Epi2002ContentTypeShouldHaveGroupName, true),
                ("Order", Descriptors.Epi2003ContentTypeShouldHaveOrder, false));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi2000ContentTypeShouldHaveDisplayName,
                Descriptors.Epi2001ContentTypeShouldHaveDescription,
                Descriptors.Epi2002ContentTypeShouldHaveGroupName,
                Descriptors.Epi2003ContentTypeShouldHaveOrder);

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

            foreach (var tuple in ContentTypeArguments)
            {
                if (tuple.AssumeInherited && !Equals(contentAttribute.AttributeClass, contentTypeAttribute))
                {
                    if (!contentAttribute.NamedArguments.Any(arg => string.Equals(arg.Key, tuple.ArgumentName, StringComparison.Ordinal)))
                    {
                        // For simplicity, assume that a derived attribute
                        // with missing argument inherits an argument value
                        return;
                    }
                }

                var argument = contentAttribute.NamedArguments.FirstOrDefault(arg => string.Equals(arg.Key, tuple.ArgumentName, StringComparison.Ordinal));
                if (string.IsNullOrEmpty(argument.Value.Value?.ToString()))
                {
                    ReportInvalidArgument(symbolContext, namedTypeSymbol, contentAttribute, tuple.Descriptor);
                }
            }
        }

        private void ReportInvalidArgument(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute, DiagnosticDescriptor descriptor)
        {
            var node = attribute.ApplicationSyntaxReference?.GetSyntax();
            if (node != null)
            {
                symbolContext.ReportDiagnostic(
                    node.CreateDiagnostic(
                        descriptor,
                        namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }
    }
}
