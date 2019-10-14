using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingInternalNamespacesAnalyzer : DiagnosticAnalyzer
    {
        private const string EpiserverNamespace = "EPiServer";
        private const string MediachaseNamespace = "Mediachase";
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

            context.RegisterCompilationStartAction(compilationContext =>
            {
                compilationContext.RegisterOperationAction(AnalyzePropertyReference, OperationKind.PropertyReference);
                compilationContext.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
            });
        }

        private void AnalyzePropertyReference(OperationAnalysisContext operationContext)
        {
            var operation = (IPropertyReferenceOperation)operationContext.Operation;
            var space = operation.Property?.ContainingType?.ContainingNamespace;
            ReportIfInternal(operationContext, space, operation);
        }

        private void AnalyzeInvocation(OperationAnalysisContext operationContext)
        {
            var operation = (IInvocationOperation) operationContext.Operation;
            var space = operation.TargetMethod?.ContainingType?.ContainingNamespace;
            ReportIfInternal(operationContext, space, operation);
        }

        private static void ReportIfInternal(OperationAnalysisContext operationContext, INamespaceSymbol space, IOperation operation)
        {
            if (!string.Equals(space?.MetadataName, InternalNamespace, StringComparison.Ordinal))
            {
                return;
            }

            var displayParts = space.ToDisplayParts(SymbolDisplayFormat.CSharpErrorMessageFormat);
            if (!displayParts.Any())
            {
                return;
            }

            string rootNamespace = displayParts.First().Symbol?.MetadataName;
            if (string.Equals(rootNamespace, EpiserverNamespace, StringComparison.Ordinal) ||
                string.Equals(rootNamespace, MediachaseNamespace, StringComparison.Ordinal))
            {
                operationContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.CAE1001_AvoidUsingInternalNamespaces,
                        operation.Syntax.GetLocation(),
                        space.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }
        }
    }
}
