using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingInternalNamespacesAnalyzer : MemberAccessIdentifierNameAnalyzerBase
    {
        private const string InternalNamespace = "Internal";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1001_AvoidUsingInternalNamespaces);

        protected override void AnalyzeIdentifierName(SyntaxNodeAnalysisContext syntaxContext, IdentifierNameSyntax identifierName)
        {
            var space = syntaxContext.SemanticModel?.GetTypeInfo(identifierName).Type?.ContainingNamespace;

            if (string.Equals(space?.MetadataName, InternalNamespace, StringComparison.Ordinal))
            {
                syntaxContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.Epi1001_AvoidUsingInternalNamespaces,
                        identifierName?.GetLocation(),
                        space?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }
        }
    }
}