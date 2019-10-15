using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingDataFactoryAnalyzer : DiagnosticAnalyzer
    {
        private const string DataFactoryName = "DataFactory";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CAE1000_AvoidUsingDataFactory);

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
            if (syntax?.Expression is null)
            {
                return;
            }

            string typeName = syntaxContext.SemanticModel?.GetTypeInfo(syntax.Expression).Type?.MetadataName;

            if (string.Equals(typeName, DataFactoryName, StringComparison.Ordinal))
            {
                syntaxContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CAE1000_AvoidUsingDataFactory,
                        syntax.Expression.GetLocation()));
            }
        }
    }
}
