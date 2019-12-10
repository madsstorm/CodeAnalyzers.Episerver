using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    public class CommerceContentDataHasAvailableContentTypesAttributeAnalyzer : ContentDataHasAvailableContentTypesAttributeAnalyzerBase
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
