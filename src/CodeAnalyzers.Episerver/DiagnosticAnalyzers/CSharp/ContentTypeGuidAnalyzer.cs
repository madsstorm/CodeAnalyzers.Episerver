using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeGuidAnalyzer : DiagnosticAnalyzer
    {
        private const string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string GuidArgument = "GUID";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi1000ContentTypeMustHaveValidGuid,
                Descriptors.Epi1001ContentTypeMustHaveUniqueGuid);

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

            private readonly ConcurrentDictionary<Guid, (INamedTypeSymbol Type, AttributeData Attribute)> contentTypeGuids =
                new ConcurrentDictionary<Guid, (INamedTypeSymbol Type, AttributeData Attribute)>();

            public CompilationAnalyzer(INamedTypeSymbol contentTypeAttribute)
            {
                this.contentTypeAttribute = contentTypeAttribute;
            }

            internal void AnalyzeSymbol(SymbolAnalysisContext symbolContext)
            {
                var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;               
                var attributes = namedTypeSymbol.GetAttributes();

                var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
                if(contentAttribute is null)
                {
                    return;
                }

                VerifyContentTypeGuid(symbolContext, namedTypeSymbol, contentAttribute);
            }

            private void VerifyContentTypeGuid(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData attribute)
            {
                if (TryGetGuidFromAttribute(attribute, out Guid contentGuid))
                {
                    var (existingType, existingAttribute) = contentTypeGuids.GetOrAdd(contentGuid, (namedTypeSymbol, attribute));

                    if(existingType != namedTypeSymbol)
                    {
                        ReportDuplicateGuid(symbolContext, namedTypeSymbol, attribute, existingType);
                        ReportDuplicateGuid(symbolContext, existingType, existingAttribute, namedTypeSymbol);
                    }
                }
                else
                {
                    ReportInvalidGuid(symbolContext, namedTypeSymbol, attribute);
                }
            }

            private static bool TryGetGuidFromAttribute(AttributeData attribute, out Guid guid)
            {
                TypedConstant guidValue = default;

                foreach (var namedArgument in attribute.NamedArguments)
                {
                    if (string.Equals(namedArgument.Key, GuidArgument, StringComparison.Ordinal))
                    {
                        guidValue = namedArgument.Value;
                        break;
                    }
                }

                return Guid.TryParse(guidValue.Value?.ToString(), out guid);
            }

            private static void ReportDuplicateGuid(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType,
                AttributeData attribute, INamedTypeSymbol matchingType)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi1001ContentTypeMustHaveUniqueGuid,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                            matchingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }

            private static void ReportInvalidGuid(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi1000ContentTypeMustHaveValidGuid,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }
        }
    }
}
