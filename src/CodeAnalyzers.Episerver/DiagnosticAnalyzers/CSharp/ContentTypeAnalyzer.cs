using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Collections.Concurrent;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeAnalyzer : DiagnosticAnalyzer
    {
        private const string TypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string GuidArgument = "GUID";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi1000ContentTypeMustHaveValidGuid,
                Descriptors.Epi1001ContentTypeMustHaveUniqueGuid);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var contentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeMetadataName);
                if (contentTypeAttribute == null)
                {
                    return;
                }

                CompilationAnalyzer analyzer = new CompilationAnalyzer(contentTypeAttribute);

                compilationContext.RegisterSymbolAction(analyzer.AnalyzeSymbol, SymbolKind.NamedType);
            });
        }

        private class CompilationAnalyzer
        {
            private readonly INamedTypeSymbol _contentTypeAttribute;

            private readonly ConcurrentDictionary<Guid, (INamedTypeSymbol Type, AttributeData Attribute)> _contentTypeGuids =
                new ConcurrentDictionary<Guid, (INamedTypeSymbol Type, AttributeData Attribute)>();

            public CompilationAnalyzer(INamedTypeSymbol contentTypeAttribute)
            {
                _contentTypeAttribute = contentTypeAttribute;               
            }

            internal void AnalyzeSymbol(SymbolAnalysisContext symbolContext)
            {
                if(!(symbolContext.Symbol is INamedTypeSymbol namedTypeSymbol))
                {
                    return;
                }

                if (!(namedTypeSymbol?.GetAttributes() is ImmutableArray<AttributeData> attributes))
                {
                    return;
                }

                foreach (var attribute in attributes)
                {
                    if (attribute.AttributeClass.InheritsOrIs(_contentTypeAttribute))
                    {
                        VerifyContentTypeGuid(symbolContext, namedTypeSymbol, attribute);
                        break;
                    }
                }
            }

            private void VerifyContentTypeGuid(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData attribute)
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

                bool validGuid = Guid.TryParse(guidValue.Value?.ToString(), out Guid contentGuid);

                if (validGuid)
                {
                    var (existingType, existingAttribute) = _contentTypeGuids.GetOrAdd(contentGuid, (namedTypeSymbol, attribute));

                    if(existingType != namedTypeSymbol)
                    {
                        var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                        if (node != null)
                        {
                            symbolContext.ReportDiagnostic(
                                node.CreateDiagnostic(
                                    Descriptors.Epi1001ContentTypeMustHaveUniqueGuid,
                                    namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                                    existingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                        }

                        var existingNode = existingAttribute.ApplicationSyntaxReference?.GetSyntax();
                        if(existingNode != null)
                        {
                            symbolContext.ReportDiagnostic(
                                existingNode.CreateDiagnostic(
                                    Descriptors.Epi1001ContentTypeMustHaveUniqueGuid,
                                    existingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                                    namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                        }
                    }
                }
                else
                {
                    var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                    if (node != null)
                    {
                        symbolContext.ReportDiagnostic(
                            node.CreateDiagnostic(
                                Descriptors.Epi1000ContentTypeMustHaveValidGuid,
                                namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                    }
                }
            }
        }
    }
}
