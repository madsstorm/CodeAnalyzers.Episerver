using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AvoidUsingDataFactoryAnalyzer : DiagnosticAnalyzer
    {
        private const string Title = "Avoid using legacy API DataFactory";
        private const string MessageFormat = "Avoid using legacy API {0}";
        private const string Description = "Legacy data source in EPiServer CMS. This API has been replaced by the IContentRepository, IContentEvents and a number of related interfaces.";

        private const string TypeMetadataName = "EPiServer.DataFactory";

        private static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(
                DiagnosticIds.AvoidUsingDataFactoryAnalyzerRuleId,
                Title,
                MessageFormat,
                DiagnosticCategories.Usage,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

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
                                    Rule,
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
                                        Rule,
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
                                        Rule,
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
