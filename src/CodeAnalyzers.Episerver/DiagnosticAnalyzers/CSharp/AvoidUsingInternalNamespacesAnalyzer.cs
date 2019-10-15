using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingInternalNamespacesAnalyzer : DiagnosticAnalyzer
    {
        private const string InternalNamespace = "Internal";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CAE1001_AvoidUsingInternalNamespaces);

        public override void Initialize(AnalysisContext context)
        {
            if(context is null)
            {
                return;
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeMemberAccess(SyntaxNodeAnalysisContext syntaxContext)
        {
            var syntax = syntaxContext.Node as MemberAccessExpressionSyntax;
            if(syntax?.Expression is null)
            {
                return;
            }

            var space = syntaxContext.SemanticModel?.GetTypeInfo(syntax.Expression).Type?.ContainingNamespace;

            ReportIfInternal(syntaxContext, space, syntax.Expression);
        }

        private static void ReportIfInternal(SyntaxNodeAnalysisContext syntaxContext, INamespaceSymbol space, ExpressionSyntax syntax)
        {
            if (string.Equals(space?.MetadataName, InternalNamespace, StringComparison.Ordinal))
            {
                syntaxContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CAE1001_AvoidUsingInternalNamespaces,
                        syntax.GetLocation(),
                        space.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }
        }
    }
}