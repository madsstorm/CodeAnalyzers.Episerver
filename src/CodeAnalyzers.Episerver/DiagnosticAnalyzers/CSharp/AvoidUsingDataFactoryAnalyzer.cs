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

            context.RegisterCompilationStartAction(
                (CompilationStartAnalysisContext compilationStartAnalysisContext) =>
                {
                    INamedTypeSymbol dataFactoryTypeSymbol =
                        compilationStartAnalysisContext.Compilation.GetTypeByMetadataName(TypeMetadataName);
                    if (dataFactoryTypeSymbol == null)
                    {
                        return;
                    }

                    compilationStartAnalysisContext.RegisterOperationAction(
                        (OperationAnalysisContext operationAnalysisContext) =>
                        {
                            IPropertyReferenceOperation propertyReferenceOperation = (IPropertyReferenceOperation)operationAnalysisContext.Operation;
                            if (Equals(propertyReferenceOperation.Property?.Type, dataFactoryTypeSymbol))
                            {
                                operationAnalysisContext.ReportDiagnostic(
                                    Diagnostic.Create(
                                    Descriptors.EPI1000_AvoidUsingDataFactory,
                                    propertyReferenceOperation.Syntax.GetLocation(),
                                    propertyReferenceOperation.Property.ToDisplayString(
                                        SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                            }
                        },
                        OperationKind.PropertyReference);


                    compilationStartAnalysisContext.RegisterOperationAction(
                        (OperationAnalysisContext operationAnalysisContext) =>
                        {
                            IInvocationOperation invocationOperation = (IInvocationOperation)operationAnalysisContext.Operation;
                            if (Equals(invocationOperation.Instance?.Type, dataFactoryTypeSymbol))
                            {
                                operationAnalysisContext.ReportDiagnostic(
                                    Diagnostic.Create(
                                        Descriptors.EPI1000_AvoidUsingDataFactory,
                                        invocationOperation.Syntax.GetLocation(),
                                        invocationOperation.TargetMethod.ToDisplayString(
                                            SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                            }
                        },
                        OperationKind.Invocation);

                    compilationStartAnalysisContext.RegisterOperationAction(
                        (OperationAnalysisContext operationAnalysisContext) =>
                        {
                            IMethodReferenceOperation methodReferenceOperation = (IMethodReferenceOperation)operationAnalysisContext.Operation;
                            if (Equals(methodReferenceOperation.Instance?.Type, dataFactoryTypeSymbol))
                            {
                                operationAnalysisContext.ReportDiagnostic(
                                    Diagnostic.Create(
                                        Descriptors.EPI1000_AvoidUsingDataFactory,
                                        methodReferenceOperation.Syntax.GetLocation(),
                                        methodReferenceOperation.Method.ToDisplayString(
                                            SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                            }
                        },
                        OperationKind.MethodReference);
                });
        }
    }
}
