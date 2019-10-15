using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingDataFactoryAnalyzer : MemberAccessIdentifierNameAnalyzerBase
    {
        private const string DataFactoryName = "DataFactory";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CAE1000_AvoidUsingDataFactory);

        override protected void AnalyzeIdentifierName(SyntaxNodeAnalysisContext syntaxContext, IdentifierNameSyntax identifierName)
        {
            string typeName = syntaxContext.SemanticModel?.GetTypeInfo(identifierName).Type?.MetadataName;

            if (string.Equals(typeName, DataFactoryName, StringComparison.Ordinal))
            {
                syntaxContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CAE1000_AvoidUsingDataFactory,
                        identifierName?.GetLocation()));
            }
        }
    }
}