using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ProductContentUniqueOrderAnalyzer>;
using System.Threading.Tasks;
using Xunit;


namespace CodeAnalyzers.Episerver.Test
{
    public class ProductContentUniqueOrderTests
    {
        [Fact]
        public async Task IgnoreProductContentWithUniqueOrders()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class TypeName : ProductContent
                    {
                    }

                    [ContentType(Order = 200)]
                    public class OtherType : ProductContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreMixedContentTypesWithDuplicateOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class PageType : PageData
                    {
                    }

                    [ContentType(Order = 100)]
                    public class BlockType : BlockData
                    {
                    }

                    [ContentType(Order = 100)]
                    public class MediaType : MediaData
                    {
                    }

                    [ContentType(Order = 100)]
                    public class VariationType : VariationContent
                    {
                    }

                    [ContentType(Order = 100)]
                    public class ProductType : ProductContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectProductContentWithDuplicateOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class TypeName : ProductContent
                    {
                    }

                    [ContentType(Order = 100)]
                    public class OtherType : ProductContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(8, 22).WithArguments("ProductContent", "TypeName", "OtherType"),
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(13, 22).WithArguments("ProductContent", "OtherType", "TypeName"));
        }
    }
}
