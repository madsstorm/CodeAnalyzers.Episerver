using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public abstract class AllowedTypesAttributeAnalyzerBase : DiagnosticAnalyzer
    {
        protected abstract ImmutableArray<string> RootPropertyTypeNames { get; }

        protected abstract DiagnosticDescriptor Descriptor { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var rootPropertyTypes = RootPropertyTypeNames.Select(r => compilationContext.Compilation.GetTypeByMetadataName(r)).Where(s => s != null);

                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                var allowedTypesType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.AllowedTypesMetadataName);
                if (allowedTypesType is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, iContentDataType, allowedTypesType, rootPropertyTypes)
                    , SymbolKind.Property);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol iContentDataType,
            INamedTypeSymbol allowedTypesType,
            IEnumerable<INamedTypeSymbol> rootPropertyTypes)
        {
            var propertySymbol = (IPropertySymbol)symbolContext.Symbol;

            var containingType = propertySymbol.ContainingType;
            if (containingType is null)
            {
                return;
            }

            if (containingType.TypeKind == TypeKind.Interface)
            {
                return;
            }

            if (!iContentDataType.IsAssignableFrom(containingType))
            {
                return;
            }

            if (!rootPropertyTypes.Any(root => root.IsAssignableFrom(propertySymbol.Type)))
            {
                return;
            }

            var attribute = propertySymbol.GetAttributes().FirstOrDefault(a => allowedTypesType.IsAssignableFrom(a.AttributeClass));
            if (attribute is null)
            {
                ReportMissingAllowedTypes(symbolContext, propertySymbol);
            }
        }

        private void ReportMissingAllowedTypes(SymbolAnalysisContext symbolContext, IPropertySymbol property)
        {
            symbolContext.ReportDiagnostic(property.CreateDiagnostic(Descriptor));
        }
    }
}
