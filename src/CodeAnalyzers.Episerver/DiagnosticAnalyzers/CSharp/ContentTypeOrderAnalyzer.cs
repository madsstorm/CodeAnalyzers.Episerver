using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Linq;
using System.Collections.Concurrent;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeOrderAnalyzer : DiagnosticAnalyzer
    {
        private const string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string OrderArgument = "Order";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi2003ContentTypeShouldHaveOrder,
                Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder);

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

                CompilationAnalyzer analyzer = new CompilationAnalyzer(contentTypeAttribute);

                compilationContext.RegisterSymbolAction(analyzer.AnalyzeSymbol, SymbolKind.NamedType);
            });
        }

        private class CompilationAnalyzer
        {
            private readonly INamedTypeSymbol contentTypeAttribute;

            private readonly ConcurrentDictionary<int, (INamedTypeSymbol Type, AttributeData Attribute)> contentTypeOrders =
                new ConcurrentDictionary<int, (INamedTypeSymbol Type, AttributeData Attribute)>();

            public CompilationAnalyzer(INamedTypeSymbol contentTypeAttribute)
            {
                this.contentTypeAttribute = contentTypeAttribute;
            }

            internal void AnalyzeSymbol(SymbolAnalysisContext symbolContext)
            {
                var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
                var attributes = namedTypeSymbol.GetAttributes();

                var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
                if (contentAttribute is null)
                {
                    return;
                }

                VerifyContentTypeOrder(symbolContext, namedTypeSymbol, contentAttribute);
            }

            private void VerifyContentTypeOrder(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData attribute)
            {
                if (TryGetOrderFromAttribute(attribute, out int contentOrder))
                {
                    var (existingType, existingAttribute) = contentTypeOrders.GetOrAdd(contentOrder, (namedTypeSymbol, attribute));

                    if (existingType != namedTypeSymbol)
                    {
                        ReportDuplicateOrder(symbolContext, namedTypeSymbol, attribute, existingType);
                        ReportDuplicateOrder(symbolContext, existingType, existingAttribute, namedTypeSymbol);
                    }
                }
                else
                {
                    ReportInvalidOrder(symbolContext, namedTypeSymbol, attribute);
                }
            }

            private static bool TryGetOrderFromAttribute(AttributeData attribute, out int order)
            {
                TypedConstant orderValue = default;

                foreach (var namedArgument in attribute.NamedArguments)
                {
                    if (string.Equals(namedArgument.Key, OrderArgument, StringComparison.Ordinal))
                    {
                        orderValue = namedArgument.Value;
                        break;
                    }
                }

                return int.TryParse(orderValue.Value?.ToString(), out order);
            }

            private static void ReportDuplicateOrder(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType,
                AttributeData attribute, INamedTypeSymbol matchingType)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                            matchingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }

            private static void ReportInvalidOrder(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi2003ContentTypeShouldHaveOrder,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }
        }
    }
}
