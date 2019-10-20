using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;

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

                compilationContext.RegisterSymbolAction(
                    symbolContext => VerifyAttributes(symbolContext.ReportDiagnostic, symbolContext.Symbol, contentTypeAttribute),
                    SymbolKind.NamedType);
            });
        }

        private static void VerifyAttributes(Action<Diagnostic> reportDiagnostic, ISymbol namedTypeSymbol, INamedTypeSymbol contentTypeAttribute)
        {
            var attributes = namedTypeSymbol.GetAttributes();

            foreach (var attribute in attributes)
            {
                if(attribute.AttributeClass.InheritsOrIs(contentTypeAttribute))
                {
                    TypedConstant guidValue = default;

                    foreach(var namedArgument in attribute.NamedArguments)
                    {
                        if(string.Equals(namedArgument.Key, GuidArgument, StringComparison.Ordinal))
                        {
                            guidValue = namedArgument.Value;
                            break;
                        }
                    }

                    if(!Guid.TryParse(guidValue.Value?.ToString(), out Guid value))
                    {
                        var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                        if (node != null)
                        {
                            reportDiagnostic(
                                node.CreateDiagnostic(
                                    Descriptors.Epi1000ContentTypeMustHaveValidGuid,
                                    namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
                        }
                    }

                    break;
                }
            }
        }
    }
}
