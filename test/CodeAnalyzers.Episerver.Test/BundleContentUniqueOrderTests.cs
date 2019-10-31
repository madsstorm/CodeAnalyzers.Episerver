using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BundleContentUniqueOrderAnalyzer>;
using System.Threading.Tasks;
using Xunit;


namespace CodeAnalyzers.Episerver.Test
{
    public class BundleContentUniqueOrderTests
    {
        [Fact]
        public async Task IgnoreBundleContentWithUniqueOrders()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class TypeName : BundleContent
                    {
                    }

                    [ContentType(Order = 200)]
                    public class OtherType : BundleContent
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
                    public class ProductType : ProductContent
                    {
                    }

                    [ContentType(Order = 100)]
                    public class BundleType : BundleContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectBundleContentWithDuplicateOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class TypeName : BundleContent
                    {
                    }

                    [ContentType(Order = 100)]
                    public class OtherType : BundleContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(8, 22).WithArguments("TypeName", "OtherType"),
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(13, 22).WithArguments("OtherType", "TypeName"));
        }
    }
}
