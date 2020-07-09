using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentDataHasImageUrlAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentDataHasImageUrlAttributeTests
    {
        [Fact]
        public async Task IgnoreContentDataWithImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ImageUrl(""image.png"")]
                    public class PageType : PageData
                    {
                    }

                    [ImageUrl(""image.png"")]
                    public class BlockType : BlockData
                    {
                    }

                    [ImageUrl(""image.png"")]
                    public class VariationType : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAbstractContentDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    public abstract class PageBase : PageData
                    {
                    }

                    public abstract class BlockBase : BlockData
                    {
                    }

                    public abstract class VariationBase : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentDataInterfaceWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    public interface ITypeName : IContentData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreMediaDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class MediaType : MediaData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreUnknownContentDataWithoutImageUrl()
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
        public async Task IgnoreContentDataWithCustomImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Custom
                {
                    public class CustomImageUrl : ImageUrlAttribute
                    {
                        public CustomImageUrl() : base(""base.png"")
                        {
                        }

                        public CustomImageUrl(string path) : base(path)
                        {
                        }
                    }
                }

                namespace Test
                {
                    using Custom;

                    [CustomImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }

                    [CustomImageUrl(""image.png"")]
                    public class BlockType : BlockData
                    {
                    }

                    [CustomImageUrl(""image.png"")]
                    public class VariationType : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentDataWithInheritedImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Custom
                {
                    public class CustomImageUrl : ImageUrlAttribute
                    {
                        public CustomImageUrl() : base(""base.png"")
                        {
                        }

                        public CustomImageUrl(string path) : base(path)
                        {
                        }
                    }
                }

                namespace Test
                {
                    using Custom;

                    [CustomImageUrl]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreOtherType()
        {
            var test = @"
                namespace Test
                {
                    public class TypeName
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectPageDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class PageType : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectBlockDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class BlockType : BlockData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
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
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectVariationDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Core;

                namespace Test
                {
                    public class VariationType : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
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
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
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
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectNodeDataWithoutImageUrl()
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
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectPromotionDataWithoutImageUrl()
        {
            var test = @"
                using EPiServer.Commerce.Marketing;
                using EPiServer.Core;

                namespace Test
                {
                    public class PromotionType : PromotionData
                    {
                        public override DiscountType DiscountType => DiscountType.LineItem;
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
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
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectContentDataWithNullImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ImageUrl(null)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(8, 34));
        }

        [Fact]
        public async Task DetectContentDataWithEmptyImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ImageUrl("""")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(8, 34));
        }

        [Fact]
        public async Task DetectContentDataWithEmptyInheritedImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Custom
                {
                    public class CustomImageUrl : ImageUrlAttribute
                    {
                        public CustomImageUrl() : base(""base.png"")
                        {
                        }

                        public CustomImageUrl(string path) : base(path)
                        {
                        }
                    }
                }

                namespace Test
                {
                    using Custom;

                    [CustomImageUrl("""")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(24, 34));
        }
    }
}