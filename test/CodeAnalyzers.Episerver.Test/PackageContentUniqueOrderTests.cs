using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.PackageContentUniqueOrderAnalyzer>;
using System.Threading.Tasks;
using Xunit;


namespace CodeAnalyzers.Episerver.Test
{
    public class PackageContentUniqueOrderTests
    {
        [Fact]
        public async Task IgnorePackageContentWithUniqueOrders()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class TypeName : PackageContent
                    {
                    }

                    [ContentType(Order = 200)]
                    public class OtherType : PackageContent
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

                    [ContentType(Order = 100)]
                    public class PackageType : PackageContent
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
                    public class TypeName : PackageContent
                    {
                    }

                    [ContentType(Order = 100)]
                    public class OtherType : PackageContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(8, 22).WithArguments("PackageContent", "TypeName", "OtherType"),
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(13, 22).WithArguments("PackageContent", "OtherType", "TypeName"));
        }
    }
}
