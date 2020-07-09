using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.PageDataHasAvailableContentTypesAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class PageDataHasAvailableContentTypesAttributeTests
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
                    public class PageType : PageData
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
                    public abstract class PageType : PageData
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
        public async Task IgnoreBlockDataWithoutAvailableContentTypes()
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
                    public class PageType : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectPageDataWithoutAvailableContentTypes()
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
                Verify.Diagnostic(Descriptors.Epi2012PageDataShouldHaveAvailableContentTypesAttribute).WithLocation(7, 34));
        }
    }
}