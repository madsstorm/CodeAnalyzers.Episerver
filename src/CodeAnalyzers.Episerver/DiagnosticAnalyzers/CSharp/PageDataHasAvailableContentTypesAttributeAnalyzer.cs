using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class PageDataHasAvailableContentTypesAttributeAnalyzer : ContentDataHasAvailableContentTypesAttributeAnalyzerBase
    {
        protected override ImmutableArray<string> RootTypeNames => ImmutableArray.Create(TypeNames.PageDataMetadataName);

        protected override DiagnosticDescriptor Descriptor => Descriptors.Epi2012PageDataShouldHaveAvailableContentTypesAttribute;
    }
}
