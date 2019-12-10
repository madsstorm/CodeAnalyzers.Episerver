using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommerceContentDataHasAvailableContentTypesAttributeAnalyzer : AvailableContentTypesAttributeAnalyzerBase
    {
        protected override ImmutableArray<string> RootTypeNames =>
            ImmutableArray.Create(
                TypeNames.NodeContentMetadataName,
                TypeNames.ProductContentMetadataName,
                TypeNames.PackageContentMetadataName,
                TypeNames.BundleContentMetadataName,
                TypeNames.SalesCampaignMetadataName);

        protected override DiagnosticDescriptor Descriptor => Descriptors.Epi2013CommerceContentDataShouldHaveAvailableContentTypesAttribute;
    }
}
