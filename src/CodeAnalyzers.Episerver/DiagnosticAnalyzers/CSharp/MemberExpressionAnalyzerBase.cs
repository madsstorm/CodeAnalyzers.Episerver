using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public abstract class MemberExpressionAnalyzerBase : DiagnosticAnalyzer
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
            var expression = (syntaxContext.Node as MemberAccessExpressionSyntax)?.Expression;
            if (expression is null)
            {
                return;
            }

            AnalyzeMemberAccess(syntaxContext, expression);
        }

        protected abstract void AnalyzeMemberAccess(SyntaxNodeAnalysisContext syntaxContext, ExpressionSyntax expression);
    }
}