using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentDataHasContentTypeAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentDataHasContentTypeAttributeTests
    {
        [Fact]
        public async Task IgnoreContentDataWithContentTypeAttribute()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Marketing;

                namespace Test
                {
                    [ContentType]
                    public class PageType : PageData
                    {
                    }

                    [ContentType]
                    public class BlockType : BlockData
                    {
                    }

                    [ContentType]
                    public class MediaType : MediaData
                    {
                    }

                    [ContentType]
                    public class PromotionType : EntryPromotion
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAbstractContentDataWithoutContentTypeAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.Commerce.Marketing;

                namespace Test
                {
                    public abstract class PageBase : PageData
                    {
                    }

                    public abstract class BlockBase : BlockData
                    {
                    }

                    public abstract class MediaBase : MediaData
                    {
                    }

                    public abstract class PromotionBase : EntryPromotion
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentDataInterfaceWithoutContentTypeAttribute()
        {
            var test = @"
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
        public async Task IgnoreCatalogContent()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Commerce.Catalog.DataAnnotations;

                namespace Test
                {
                    [CatalogContentType]
                    public class VariationType : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentDataWithoutContentTypeAttribute()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Marketing;

                namespace Test
                {
                    public class PageType : PageData
                    {
                    }

                    public class BlockType : BlockData
                    {
                    }

                    public class MediaType : MediaData
                    {
                    }

                    public class PromotionType : EntryPromotion
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(8, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(12, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(16, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(20, 34));
        }

        [Fact]
        public async Task DetectContentDataWithCatalogContentTypeAttribute()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using EPiServer.Commerce.Marketing;

                namespace Test
                {
                    [CatalogContentType]
                    public class PageType : PageData
                    {
                    }

                    [CatalogContentType]
                    public class BlockType : BlockData
                    {
                    }

                    [CatalogContentType]
                    public class MediaType : MediaData
                    {
                    }

                    [CatalogContentType]
                    public class PromotionType : EntryPromotion
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(10, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(15, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(20, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataShouldHaveContentTypeAttribute).WithLocation(25, 34));
        }
    }
}
