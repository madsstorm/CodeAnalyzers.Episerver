using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.CatalogContentTypeImplementsCatalogContentDataAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class CatalogContentTypeImplementsCatalogContentDataTests
    {
        [Fact]
        public async Task IgnoreCatalogContentData()
        {
            var test = @"
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
        public async Task IgnoreCmsContent()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType]
                    public class PageType : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentData()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.DataAnnotations;

                namespace Test
                {
                    [CatalogContentType]
                    public class PageType : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1005CatalogContentTypeShouldImplementCatalogContentData)
                    .WithLocation(8, 34).WithArguments("PageType"));
        }

        [Fact]
        public async Task DetectAbstractCatalogContentData()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.DataAnnotations;

                namespace Test
                {
                    [CatalogContentType]
                    public abstract class VariationType : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1005CatalogContentTypeShouldImplementCatalogContentData)
                    .WithLocation(9, 43).WithArguments("VariationType"));
        }

        [Fact]
        public async Task DetectNonContentData()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [CatalogContentType]
                    public class TypeName : object
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1005CatalogContentTypeShouldImplementCatalogContentData)
                    .WithLocation(8, 34).WithArguments("TypeName"));
        }
    }
}
