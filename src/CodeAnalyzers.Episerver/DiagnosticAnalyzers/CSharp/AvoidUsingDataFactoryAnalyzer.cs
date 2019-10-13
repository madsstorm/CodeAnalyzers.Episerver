using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingDataFactoryAnalyzer : DiagnosticAnalyzer
    {
        private const string TypeMetadataName = "EPiServer.DataFactory";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.EPI1000_AvoidUsingDataFactory);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var dataFactory = compilationContext.Compilation.GetTypeByMetadataName(TypeMetadataName);
                if (dataFactory == null)
                {
                    return;
                }

                compilationContext.RegisterOperationAction(operationContext =>
                    AnalyzePropertyReference(operationContext, dataFactory), OperationKind.PropertyReference);
            });
        }       

        private void AnalyzePropertyReference(OperationAnalysisContext operationContext, INamedTypeSymbol dataFactory)
        {
            var operation = (IPropertyReferenceOperation)operationContext.Operation;
            if (Equals(operation.Property?.Type, dataFactory))
            {
                operationContext.ReportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.EPI1000_AvoidUsingDataFactory,
                        operation.Syntax.GetLocation()));
            }
        }
    }
}
