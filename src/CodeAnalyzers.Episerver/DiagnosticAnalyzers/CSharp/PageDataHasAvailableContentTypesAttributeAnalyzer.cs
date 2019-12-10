using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageDataHasAvailableContentTypesAttributeAnalyzer : AvailableContentTypesAttributeAnalyzerBase
    {
        protected override ImmutableArray<string> RootTypeNames => ImmutableArray.Create(TypeNames.PageDataMetadataName);

        protected override DiagnosticDescriptor Descriptor => Descriptors.Epi2012PageDataShouldHaveAvailableContentTypesAttribute;
    }
}
