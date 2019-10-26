using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Linq;
using System.Collections.Concurrent;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    /// <summary>
    /// ContentType 'Order' should be unique within its content type root
    /// </summary>
    public abstract class ContentTypeUniqueOrderAnalyzerBase : DiagnosticAnalyzer
    {
        private const string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string OrderArgument = "Order";

        protected abstract string ContentRootTypeName { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder);

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

                var contentRootType = compilationContext.Compilation.GetTypeByMetadataName(ContentRootTypeName);
                if(contentRootType is null)
                {
                    return;
                }

                CompilationAnalyzer analyzer = new CompilationAnalyzer(contentTypeAttribute, contentRootType);

                compilationContext.RegisterSymbolAction(analyzer.AnalyzeSymbol, SymbolKind.NamedType);
            });
        }

        private class CompilationAnalyzer
        {
            private readonly INamedTypeSymbol contentTypeAttribute;
            private readonly INamedTypeSymbol contentRootType;

            private readonly ConcurrentDictionary<int, (INamedTypeSymbol Type, AttributeData Attribute)> contentTypeOrders =
                new ConcurrentDictionary<int, (INamedTypeSymbol Type, AttributeData Attribute)>();

            public CompilationAnalyzer(INamedTypeSymbol contentTypeAttribute, INamedTypeSymbol contentRootType)
            {
                this.contentTypeAttribute = contentTypeAttribute;
                this.contentRootType = contentRootType;
            }

            internal void AnalyzeSymbol(SymbolAnalysisContext symbolContext)
            {
                var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
                if(!contentRootType.IsAssignableFrom(namedTypeSymbol))
                {
                    return;
                }

                var attributes = namedTypeSymbol.GetAttributes();

                var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
                if (contentAttribute is null)
                {
                    return;
                }

                VerifyContentTypeUniqueOrder(symbolContext, namedTypeSymbol, contentAttribute);
            }

            private void VerifyContentTypeUniqueOrder(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData attribute)
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

            private void ReportDuplicateOrder(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType,
                AttributeData attribute, INamedTypeSymbol matchingType)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder,
                            contentRootType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                            matchingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }
        }
    }
}
