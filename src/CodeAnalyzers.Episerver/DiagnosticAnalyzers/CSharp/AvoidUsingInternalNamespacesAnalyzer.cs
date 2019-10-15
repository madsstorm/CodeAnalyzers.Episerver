using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingInternalNamespacesAnalyzer : MemberExpressionAnalyzerBase
    {
        private const string InternalNamespace = "Internal";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CAE1001_AvoidUsingInternalNamespaces);

        override protected void AnalyzeMemberAccess(SyntaxNodeAnalysisContext syntaxContext, ExpressionSyntax expression)
        {
            var space = syntaxContext.SemanticModel?.GetTypeInfo(expression).Type?.ContainingNamespace;

            if (string.Equals(space?.MetadataName, InternalNamespace, StringComparison.Ordinal))
            {
                syntaxContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CAE1001_AvoidUsingInternalNamespaces,
                        expression?.GetLocation(),
                        space.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }
        }
    }
}