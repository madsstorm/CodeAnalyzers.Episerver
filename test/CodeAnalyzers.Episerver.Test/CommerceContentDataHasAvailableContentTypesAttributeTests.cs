using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.CommerceContentDataHasAvailableContentTypesAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class CommerceContentDataHasAvailableContentTypesAttributeTests
    {
        [Fact]
        public async Task IgnoreContentDataWithAvailableContentTypes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Commerce.Marketing;

                namespace Test
                {
                    [AvailableContentTypes]
                    public class NodeType : NodeContent
                    {
                    }

                    [AvailableContentTypes]
                    public class ProductType : ProductContent
                    {
                    }

                    [AvailableContentTypes]
                    public class PackageType : PackageContent
                    {
                    }

                    [AvailableContentTypes]
                    public class BundleType : BundleContent
                    {
                    }

                    [AvailableContentTypes]
                    public class SalesCampaignType : SalesCampaign
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAbstractContentDataWithoutAvailableContentTypes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Commerce.Marketing;

                namespace Test
                {
                    public abstract class NodeType : NodeContent
                    {
                    }

                    public abstract class ProductType : ProductContent
                    {
                    }

                    public abstract class PackageType : PackageContent
                    {
                    }

                    public abstract class BundleType : BundleContent
                    {
                    }

                    public abstract class SalesCampaignType : SalesCampaign
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentDataInterfaceWithoutAvailableContentTypes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public interface ITypeName : IContentData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreVariationContentWithoutAvailableContentTypes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    public class VariationType : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnorePromotionDataWithoutAvailableContentTypes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Marketing;

                namespace Test
                {
                    public class PromotionType : PromotionData
                    {
                        public override DiscountType DiscountType => DiscountType.LineItem;
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreUnknownContentDataWithoutAvailableContentTypes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class UnknownContentType : StandardContentBase
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentDataWithCustomAvailableContentTypes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Commerce.Marketing;

                namespace Custom
                {
                    public class CustomAvailableContentTypesAttribute : AvailableContentTypesAttribute
                    {
                    }
                }

                namespace Test
                {
                    using Custom;

                    [CustomAvailableContentTypes]
                    public class NodeType : NodeContent
                    {
                    }

                    [CustomAvailableContentTypes]
                    public class ProductType : ProductContent
                    {
                    }

                    [CustomAvailableContentTypes]
                    public class PackageType : PackageContent
                    {
                    }

                    [CustomAvailableContentTypes]
                    public class BundleType : BundleContent
                    {
                    }

                    [CustomAvailableContentTypes]
                    public class SalesCampaignType : SalesCampaign
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectNodeDataWithoutAvailableContentTypes()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Core;

                namespace Test
                {
                    public class NodeType : NodeContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2013CommerceContentDataShouldHaveAvailableContentTypesAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectProductDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Core;

                namespace Test
                {
                    public class ProductType : ProductContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2013CommerceContentDataShouldHaveAvailableContentTypesAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectBundleDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Core;

                namespace Test
                {
                    public class BundleType : BundleContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2013CommerceContentDataShouldHaveAvailableContentTypesAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectPackageDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Core;

                namespace Test
                {
                    public class PackageType : PackageContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2013CommerceContentDataShouldHaveAvailableContentTypesAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectSalesCampaignWithoutImageUrl()
        {
            var test = @"
                using EPiServer.Commerce.Marketing;
                using EPiServer.Core;

                namespace Test
                {
                    public class SalesCampaignType : SalesCampaign
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2013CommerceContentDataShouldHaveAvailableContentTypesAttribute).WithLocation(7, 34));
        }
    }
}