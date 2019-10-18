using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BannedApiAnalyzer : DiagnosticAnalyzer
    {
        private const string InternalNamespace = "Internal";

        private static readonly Dictionary<string, DiagnosticDescriptor> BannedTypes = new Dictionary<string, DiagnosticDescriptor>
        {
            { "EPiServer.DataFactory", Descriptors.Epi3000AvoidUsingDataFactory },
            { "EPiServer.CacheManager", Descriptors.Epi3001AvoidUsingCacheManager }
        };

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi1000AvoidUsingInternalNamespaces,
                Descriptors.Epi3000AvoidUsingDataFactory,
                Descriptors.Epi3001AvoidUsingCacheManager);

        public override void Initialize(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            analysisContext.RegisterOperationAction(OperationAction,
                OperationKind.ObjectCreation,
                OperationKind.Invocation,
                OperationKind.EventReference,
                OperationKind.FieldReference,
                OperationKind.MethodReference,
                OperationKind.PropertyReference,
                OperationKind.ArrayCreation);

            analysisContext.RegisterSymbolAction(
                symbolContext => VerifyAttributes(symbolContext.ReportDiagnostic, symbolContext.Symbol.GetAttributes()),
                SymbolKind.NamedType,
                SymbolKind.Method,
                SymbolKind.Field,
                SymbolKind.Property,
                SymbolKind.Event);
        }

        private static void OperationAction(OperationAnalysisContext operationContext)
        {
            switch (operationContext.Operation)
            {
                case IObjectCreationOperation objectCreation:
                    VerifyType(operationContext.ReportDiagnostic, objectCreation.Type, operationContext.Operation.Syntax);
                    break;

                case IInvocationOperation invocation:
                    VerifyType(operationContext.ReportDiagnostic, invocation.TargetMethod.ContainingType, operationContext.Operation.Syntax);
                    break;

                case IMemberReferenceOperation memberReference:
                    VerifyType(operationContext.ReportDiagnostic, memberReference.Member.ContainingType, operationContext.Operation.Syntax);
                    break;

                case IArrayCreationOperation arrayCreation:
                    VerifyType(operationContext.ReportDiagnostic, arrayCreation.Type, operationContext.Operation.Syntax);
                    break;
            }
        }

        private static bool VerifyType(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode)
        {
            do
            {
                if (!VerifyTypeArguments(reportDiagnostic, type, syntaxNode, out type))
                {
                    return false;
                }
                if (type == null)
                {
                    // Type will be null for arrays and pointers.
                    return true;
                }

                if(!VerifyTypeBanned(reportDiagnostic, type, syntaxNode))
                {
                    return false;
                }

                if(!VerifyTypeInternal(reportDiagnostic, type, syntaxNode))
                {
                    return false;
                }

                type = type.ContainingType;
            }
            while (!(type is null));

            return true;
        }

        private static bool VerifyTypeBanned(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode)
        {
            string typeName = type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

            foreach (var pair in BannedTypes)
            {
                if (string.Equals(typeName, pair.Key, StringComparison.Ordinal))
                {
                    reportDiagnostic(
                        Diagnostic.Create(
                            pair.Value,
                            syntaxNode?.GetLocation()));

                    return false;
                }
            }

            return true;
        }

        private static bool VerifyTypeInternal(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode)
        {
            var space = type.ContainingNamespace;

            if (string.Equals(space?.MetadataName, InternalNamespace, StringComparison.Ordinal))
            {
                reportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.Epi1000AvoidUsingInternalNamespaces,
                        syntaxNode?.GetLocation(),
                        type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));

                return false;
            }

            return true;
        }

        private static bool VerifyTypeArguments(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode, out ITypeSymbol originalDefinition)
        {
            switch (type)
            {
                case INamedTypeSymbol namedTypeSymbol:
                    originalDefinition = namedTypeSymbol.ConstructedFrom;
                    foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                    {
                        if (typeArgument.TypeKind != TypeKind.TypeParameter &&
                            typeArgument.TypeKind != TypeKind.Error &&
                            !VerifyType(reportDiagnostic, typeArgument, syntaxNode))
                        {
                            return false;
                        }
                    }
                    break;

                case IArrayTypeSymbol arrayTypeSymbol:
                    originalDefinition = null;
                    return VerifyType(reportDiagnostic, arrayTypeSymbol.ElementType, syntaxNode);

                default:
                    originalDefinition = type.OriginalDefinition;
                    break;
            }

            return true;
        }

        private static void VerifyAttributes(Action<Diagnostic> reportDiagnostic, ImmutableArray<AttributeData> attributes)
        {
            foreach (var attribute in attributes)
            {
                var space = attribute.AttributeClass.ContainingNamespace;

                if (string.Equals(space?.MetadataName, InternalNamespace, StringComparison.Ordinal))
                {
                    var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                    if (node != null)
                    {
                        reportDiagnostic(
                            node.CreateDiagnostic(
                                Descriptors.Epi1000AvoidUsingInternalNamespaces,
                                attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
                    }
                }
            }
        }
    }
}