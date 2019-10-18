using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BannedApiAnalyzer : DiagnosticAnalyzer
    {
        private const string InternalNamespace = "Internal";

        private static readonly Dictionary<string, DiagnosticDescriptor> bannedTypes = new Dictionary<string, DiagnosticDescriptor>
        {
            { "EPiServer.DataFactory", Descriptors.Epi3000AvoidUsingDataFactory },
            { "EPiServer.CacheManager", Descriptors.Epi3001AvoidUsingCacheManager }
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi3000AvoidUsingDataFactory,
                Descriptors.Epi1000AvoidUsingInternalNamespaces,
                Descriptors.Epi3001AvoidUsingCacheManager);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                return;
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(SyntaxNodeAction,
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxKind.InvocationExpression);

            //context.RegisterOperationAction(OperationAction,
            //    OperationKind.PropertyReference,
            //    OperationKind.Invocation,
            //    OperationKind.FieldReference,
            //    OperationKind.EventReference,
            //    OperationKind.MethodReference);
        }

        private void SyntaxNodeAction(SyntaxNodeAnalysisContext syntaxContext)
        {
            switch(syntaxContext.Node)
            {
                case MemberAccessExpressionSyntax memberAccess:
                    HandleMemberAccess(syntaxContext, memberAccess);
                    break;

                case InvocationExpressionSyntax invocation:
                    HandleInvocation(syntaxContext, invocation);
                    break;
            }
        }

        private static void HandleMemberAccess(SyntaxNodeAnalysisContext syntaxContext, MemberAccessExpressionSyntax memberAccess)
        {
            if (!(memberAccess.Expression is IdentifierNameSyntax identifierName))
            {
                return;
            }

            AnalyzeIdentifierName(syntaxContext, identifierName);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext syntaxContext, InvocationExpressionSyntax invocation)
        {
            if (!(invocation.Expression is IdentifierNameSyntax memberName))
            {
                return;
            }

            AnalyzeMemberName(syntaxContext, memberName);
        }

        private static void AnalyzeMemberName(SyntaxNodeAnalysisContext syntaxContext, IdentifierNameSyntax memberName)
        {
            string typeName = syntaxContext.SemanticModel?.GetSymbolInfo(memberName).Symbol?.ContainingType?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

            foreach (var pair in bannedTypes)
            {
                if (string.Equals(typeName, pair.Key, StringComparison.Ordinal))
                {
                    syntaxContext.ReportDiagnostic(
                        Diagnostic.Create(
                            pair.Value,
                            memberName?.GetLocation()));

                    continue;
                }
            }
        }

        private static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext syntaxContext, IdentifierNameSyntax identifierName)
        {
            string typeName = syntaxContext.SemanticModel?.GetTypeInfo(identifierName).Type?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

            foreach (var pair in bannedTypes)
            {
                if (string.Equals(typeName, pair.Key, StringComparison.Ordinal))
                {
                    syntaxContext.ReportDiagnostic(
                        Diagnostic.Create(
                            pair.Value,
                            identifierName?.GetLocation()));

                    continue;
                }
            }

            var space = syntaxContext.SemanticModel?.GetTypeInfo(identifierName).Type?.ContainingNamespace;

            if (string.Equals(space?.MetadataName, InternalNamespace, StringComparison.Ordinal))
            {
                syntaxContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.Epi1000AvoidUsingInternalNamespaces,
                        identifierName?.GetLocation(),
                        space?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }
        }
    }
}