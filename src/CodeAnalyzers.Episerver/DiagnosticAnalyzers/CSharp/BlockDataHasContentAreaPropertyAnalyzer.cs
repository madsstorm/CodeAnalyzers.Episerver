using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlockDataHasContentAreaPropertyAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi2011AvoidContentAreaPropertyInBlock);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var blockDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.BlockDataMetadataName);
                if (blockDataType is null)
                {
                    return;
                }

                var contentAreaType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.ContentAreaTypeMetadataName);
                if (contentAreaType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, blockDataType, contentAreaType)
                    , SymbolKind.Property);
            });
        }

        private void AnalyzeSymbol(SymbolAnalysisContext symbolContext, INamedTypeSymbol blockDataType, INamedTypeSymbol contentAreaType)
        {
            var propertySymbol = (IPropertySymbol)symbolContext.Symbol;
            
            if(!contentAreaType.IsAssignableFrom(propertySymbol.Type))
            {
                return;
            }

            var containingType = propertySymbol.ContainingType;
            if(containingType is null)
            {
                return;
            }

            if(!blockDataType.IsAssignableFrom(containingType))
            {
                return;
            }

            symbolContext.ReportDiagnostic(propertySymbol.CreateDiagnostic(Descriptors.Epi2011AvoidContentAreaPropertyInBlock));
        }
    }
}
