using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.CatalogContentDataHasCatalogContentTypeAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class CatalogContentDataHasCatalogContentTypeAttributeTests
    {
        [Fact]
        public async Task IgnoreCatalogContentDataWithCatalogContentTypeAttribute()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [CatalogContentType]
                    public class NodeType : NodeContent
                    {
                    }

                    [CatalogContentType]
                    public class ProductType : ProductContent
                    {
                    }

                    [CatalogContentType]
                    public class VariationType : VariationContent
                    {
                    }

                    [CatalogContentType]
                    public class BundleType : BundleContent
                    {
                    }

                    [CatalogContentType]
                    public class PackageType : PackageContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAbstractCatalogContentDataWithoutCatalogContentTypeAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    public abstract class NodeBase : NodeContent
                    {
                    }

                    public abstract class ProductBase : ProductContent
                    {
                    }

                    public abstract class VariationBase : VariationContent
                    {
                    }

                    public abstract class BundleBase : BundleContent
                    {
                    }

                    public abstract class PackageBase : PackageContent
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
        public async Task DetectCatalogContentDataWithoutCatalogContentTypeAttribute()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
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
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(8, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(12, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(16, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(20, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(24, 34));
        }

        [Fact]
        public async Task DetectCatalogContentDataWithContentTypeAttribute()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
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

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(9, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(14, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(19, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(24, 34),
                Verify.Diagnostic(Descriptors.Epi1006CatalogContentDataShouldHaveCatalogContentTypeAttribute).WithLocation(29, 34));
        }
    }
}
