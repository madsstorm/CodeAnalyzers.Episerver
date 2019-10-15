using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using CodeAnalyzers.Episerver.Extensions;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeMustHaveGuidAnalyzer : DiagnosticAnalyzer
    {
        private const string TypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string GuidArgument = "GUID";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.CAE1002_ContentTypeMustHaveGuid);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                return;
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var contentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeMetadataName);
                if (contentTypeAttribute == null)
                {
                    return;
                }

                var analyzer = new CompilationAnalyzer(contentTypeAttribute);

                compilationContext.RegisterSymbolAction(symbolContext =>
                    analyzer.AnalyzeNamedType(symbolContext), SymbolKind.NamedType);
            });

        }

        private class CompilationAnalyzer
        {
            private readonly INamedTypeSymbol contentTypeAttribute;

            public CompilationAnalyzer(INamedTypeSymbol contentTypeAttribute)
            {
                this.contentTypeAttribute = contentTypeAttribute;
            }

            private readonly Dictionary<INamedTypeSymbol, bool> knownAttributeTypes = new Dictionary<INamedTypeSymbol, bool>();

            public void AnalyzeNamedType(SymbolAnalysisContext symbolContext)
            {
                INamedTypeSymbol namedType = (INamedTypeSymbol)symbolContext.Symbol;
                if (namedType is null)
                {
                    return;
                }

                foreach(var attributeData in namedType.GetAttributes())
                {
                    if (knownAttributeTypes.TryGetValue(attributeData.AttributeClass, out bool isContentType))
                    {
                        if (isContentType)
                        {
                            ReporttIfInvalidGuid(symbolContext, attributeData);
                        }

                        continue;
                    }

                    isContentType = attributeData.AttributeClass.GetBaseTypesAndThis().Any(type => Equals(type, contentTypeAttribute));

                    if(isContentType)
                    {
                        ReporttIfInvalidGuid(symbolContext, attributeData);
                    }

                    knownAttributeTypes[attributeData.AttributeClass] = isContentType;
                }
            }

            private void ReporttIfInvalidGuid(SymbolAnalysisContext symbolContext, AttributeData attributeData)
            {
                // WIP
                var arguments = attributeData.NamedArguments;

                var guidArgument = arguments.FirstOrDefault(pair => string.Equals(pair.Key, GuidArgument, StringComparison.Ordinal));

                bool isValidGuid = Guid.TryParse(guidArgument.Value.Value.ToString(), out Guid guid);
            }
        }
    }
}
