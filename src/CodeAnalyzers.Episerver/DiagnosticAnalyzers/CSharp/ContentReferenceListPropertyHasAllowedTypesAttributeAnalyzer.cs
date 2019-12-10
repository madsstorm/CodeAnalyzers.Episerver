using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentReferenceListPropertyHasAllowedTypesAttributeAnalyzer : AllowedTypesAttributeAnalyzerBase
    {
        protected override ImmutableArray<string> RootPropertyTypeNames => ImmutableArray.Create(TypeNames.ContentReferenceListTypeMetadataName);

        protected override DiagnosticDescriptor Descriptor => Descriptors.Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute;
    }
}
