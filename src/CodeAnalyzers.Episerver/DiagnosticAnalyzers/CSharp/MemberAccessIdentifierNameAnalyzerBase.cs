using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public abstract class MemberAccessIdentifierNameAnalyzerBase : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                return;
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxNodeAction, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void SyntaxNodeAction(SyntaxNodeAnalysisContext syntaxContext)
        {
            if (!(syntaxContext.Node is MemberAccessExpressionSyntax memberAccess))
            {
                return;
            }

            if (!(memberAccess.Expression is IdentifierNameSyntax identifierName))
            {
                return;
            }

            AnalyzeIdentifierName(syntaxContext, identifierName);
        }

        protected abstract void AnalyzeIdentifierName(SyntaxNodeAnalysisContext syntaxContext, IdentifierNameSyntax identifierName);
    }
}