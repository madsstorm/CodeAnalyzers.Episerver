using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentAreaPropertyHasAllowedTypesAttributeAnalyzer : AllowedTypesAttributeAnalyzerBase
    {
        protected override ImmutableArray<string> RootPropertyTypeNames => ImmutableArray.Create(TypeNames.ContentAreaTypeMetadataName);

        protected override DiagnosticDescriptor Descriptor => Descriptors.Epi2015ContentAreaPropertyShouldHaveAllowedTypesAttribute;
    }
}
