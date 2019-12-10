using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentReferencePropertyHasAllowedTypesAttributeAnalyzer : AllowedTypesAttributeAnalyzerBase
    {
        protected override ImmutableArray<string> RootPropertyTypeNames => ImmutableArray.Create(TypeNames.ContentReferenceMetadataName);

        protected override DiagnosticDescriptor Descriptor => Descriptors.Epi2014ContentReferencePropertyShouldHaveAllowedTypesAttribute;
    }
}
