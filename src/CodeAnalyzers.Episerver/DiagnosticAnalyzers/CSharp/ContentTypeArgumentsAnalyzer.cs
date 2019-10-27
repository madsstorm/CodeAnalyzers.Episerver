using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private readonly ImmutableArray<(string ArgumentName, DiagnosticDescriptor Descriptor, bool AssumeInherited)> ContentTypeArguments =
            ImmutableArray.Create(
                ("DisplayName", Descriptors.Epi2000ContentTypeShouldHaveDisplayName, false),
                ("Description", Descriptors.Epi2001ContentTypeShouldHaveDescription, false),
                ("GroupName", Descriptors.Epi2002ContentTypeShouldHaveGroupName, true),
                ("Order", Descriptors.Epi2003ContentTypeShouldHaveOrder, false));

        private readonly ImmutableArray<string> KnownContentTypeAttributeNames =
            ImmutableArray.Create(
                TypeNames.ContentTypeMetadataName,
                TypeNames.CatalogContentTypeMetadataName);

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
                var contentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.ContentTypeMetadataName);
                if (contentTypeAttribute is null)
                {
                    return;
                }

                var knownContentTypeAttributes =
                    KnownContentTypeAttributeNames.Select(root => compilationContext.Compilation.GetTypeByMetadataName(root))
                        .Where(symbol => symbol != null);

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, contentTypeAttribute, knownContentTypeAttributes)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol contentTypeAttribute,
            IEnumerable<INamedTypeSymbol> knownContentTypeAttributes)
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
                if (tuple.AssumeInherited && !IsKnownContentTypeAttribute(contentAttribute, knownContentTypeAttributes))
                {
                    if (!contentAttribute.NamedArguments.Any(arg => string.Equals(arg.Key, tuple.ArgumentName, StringComparison.Ordinal)))
                    {
                        // For simplicity, assume that a custom attribute
                        // with missing argument inherits an argument value
                        continue;
                    }
                }

                var argument = contentAttribute.NamedArguments.FirstOrDefault(arg => string.Equals(arg.Key, tuple.ArgumentName, StringComparison.Ordinal));
                if (string.IsNullOrEmpty(argument.Value.Value?.ToString()))
                {
                    ReportInvalidArgument(symbolContext, namedTypeSymbol, contentAttribute, tuple.Descriptor);
                }
            }
        }

        private bool IsKnownContentTypeAttribute(AttributeData attribute, IEnumerable<INamedTypeSymbol> knownContentTypeAttributes)
        {
            foreach(var knownAttribute in knownContentTypeAttributes)
            {
                if(Equals(attribute.AttributeClass, knownAttribute))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ReportInvalidArgument(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute, DiagnosticDescriptor descriptor)
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
