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
                using EPiServer.Commerce.Catalog.ContentTypes;

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
                    public class NodeType : NodeContent
                    {
                    }

                    [ContentType]
                    public class ProductType : ProductContent
                    {
                    }

                    [ContentType]
                    public class VariationType : VariationContent
                    {
                    }

                    [ContentType]
                    public class BundleType : BundleContent
                    {
                    }

                    [ContentType]
                    public class PackageType : PackageContent
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
        public async Task DetectContentDataWithoutContentTypeAttribute()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

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

                    public class NodeType : NodeContent
                    {
                    }

                    public class ProductType : ProductContent
                    {
                    }

                    public class VariationType : VariationContent
                    {
                    }

                    public class BundleType : BundleContent
                    {
                    }

                    public class PackageType : PackageContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(8, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(12, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(16, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(20, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(24, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(28, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(32, 34),
                Verify.Diagnostic(Descriptors.Epi1004ContentDataMustHaveContentTypeAttribute).WithLocation(36, 34));
        }
    }
}
