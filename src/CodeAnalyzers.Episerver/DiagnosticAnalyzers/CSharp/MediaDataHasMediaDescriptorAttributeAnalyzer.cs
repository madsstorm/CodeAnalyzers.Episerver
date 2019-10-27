using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MediaDataHasMediaDescriptorAttributeAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1007MediaDataShouldHaveMediaDescriptorAttribute);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var iContentMediaType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentMediaMetadataName);
                if (iContentMediaType is null)
                {
                    return;
                }

                var mediaDesciptorType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.MediaDescriptorMetadataName);
                if (mediaDesciptorType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, iContentMediaType, mediaDesciptorType)
                    , SymbolKind.NamedType);
            });
        }


        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol iContentMediaType,
            INamedTypeSymbol mediaDesciptorType)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            if (!iContentMediaType.IsAssignableFrom(namedTypeSymbol))
            {
                return;
            }

            var attributes = namedTypeSymbol.GetAttributes();

            var mediaDescriptorAttribute = attributes.FirstOrDefault(attr => mediaDesciptorType.IsAssignableFrom(attr.AttributeClass));
            if (mediaDescriptorAttribute is null)
            {
                ReportMissingMediaDescriptor(symbolContext, namedTypeSymbol);
            }
        }

        private static void ReportMissingMediaDescriptor(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
        {
            symbolContext.ReportDiagnostic(
                    namedType.CreateDiagnostic(
                        Descriptors.Epi1007MediaDataShouldHaveMediaDescriptorAttribute));
        }
    }
}
