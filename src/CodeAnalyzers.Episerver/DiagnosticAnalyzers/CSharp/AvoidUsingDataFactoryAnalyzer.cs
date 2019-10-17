﻿using System;
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
        private const string DataFactoryName = "EPiServer.DataFactory";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1000_AvoidUsingDataFactory);

        protected override void AnalyzeIdentifierName(SyntaxNodeAnalysisContext syntaxContext, IdentifierNameSyntax identifierName)
        {
            string typeName = syntaxContext.SemanticModel?.GetTypeInfo(identifierName).Type?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

            if (string.Equals(typeName, DataFactoryName, StringComparison.Ordinal))
            {
                syntaxContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.Epi1000_AvoidUsingDataFactory,
                        identifierName?.GetLocation()));
            }
        }
    }
}