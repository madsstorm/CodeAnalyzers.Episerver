using System;
using System.Collections.Immutable;
using System.Linq;
using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentPropertyDisplayArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private readonly ImmutableArray<(string ArgumentName, DiagnosticDescriptor Descriptor)> ContentPropertyArguments =
            ImmutableArray.Create(
                ("Name", Descriptors.Epi2006ContentPropertyShouldHaveName),
                ("Description", Descriptors.Epi2007ContentPropertyShouldHaveDescription),
                ("GroupName", Descriptors.Epi2008ContentPropertyShouldHaveGroupName),
                ("Order", Descriptors.Epi2009ContentPropertyShouldHaveOrder));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi2006ContentPropertyShouldHaveName,
                Descriptors.Epi2007ContentPropertyShouldHaveDescription,
                Descriptors.Epi2008ContentPropertyShouldHaveGroupName,
                Descriptors.Epi2009ContentPropertyShouldHaveOrder);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                var displayAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.DisplayMetadataName);
                if (displayAttribute is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, iContentDataType, displayAttribute)
                    , SymbolKind.Property);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol iContentDataType,
            INamedTypeSymbol displayAttribute)
        {
            var propertySymbol = (IPropertySymbol)symbolContext.Symbol;
            
            var containingType = propertySymbol.ContainingType;
            if(containingType is null)
            {
                return;
            }

            if(!iContentDataType.IsAssignableFrom(containingType))
            {
                return;
            }

            var attribute = propertySymbol.GetAttributes().FirstOrDefault(a => displayAttribute.IsAssignableFrom(a.AttributeClass));
            if(attribute is null)
            {
                return;
            }

            foreach (var tuple in ContentPropertyArguments)
            {
                var argument = attribute.NamedArguments.FirstOrDefault(arg => string.Equals(arg.Key, tuple.ArgumentName, StringComparison.Ordinal));
                if (string.IsNullOrEmpty(argument.Value.Value?.ToString()))
                {
                    ReportInvalidArgument(symbolContext, attribute, tuple.Descriptor);
                }
            }
        }

        private static void ReportInvalidArgument(SymbolAnalysisContext symbolContext, AttributeData attribute, DiagnosticDescriptor descriptor)
        {
            var node = attribute.ApplicationSyntaxReference?.GetSyntax();
            if (node != null)
            {
                symbolContext.ReportDiagnostic(
                    node.CreateDiagnostic(descriptor));
            }
        }
    }
}
