﻿using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeImplementsContentDataAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1003ContentTypeMustImplementContentData);

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

                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, contentTypeAttribute, iContentDataType)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(SymbolAnalysisContext symbolContext, INamedTypeSymbol contentTypeAttribute, INamedTypeSymbol iContentDataType)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            var attributes = namedTypeSymbol.GetAttributes();

            var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
            if (contentAttribute is null)
            {
                return;
            }

            if (namedTypeSymbol.IsAbstract || !iContentDataType.IsAssignableFrom(namedTypeSymbol))
            {
                ReportInvalidContentDataType(symbolContext, namedTypeSymbol);
            }
        }

        private static void ReportInvalidContentDataType(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                namedType.CreateDiagnostic(
                    Descriptors.Epi1003ContentTypeMustImplementContentData,
                    namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
        }
    }
}
