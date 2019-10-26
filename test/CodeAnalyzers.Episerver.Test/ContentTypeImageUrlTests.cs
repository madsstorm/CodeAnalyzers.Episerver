using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeImageUrlAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeImageUrlTests
    {
        [Fact]
        public async Task IgnoreContentTypeWithImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentTypeWithCustomImageUrl()
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

                    [ContentType]
                    [CustomImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentTypeWithInheritedImageUrl()
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

                    [ContentType]
                    [CustomImageUrl]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentTypeWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentTypeShouldHaveImageUrl).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectContentTypeWithNullImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType]
                    [ImageUrl(null)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentTypeShouldHaveImageUrl).WithLocation(8, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectContentTypeWithEmptyImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType]
                    [ImageUrl("""")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentTypeShouldHaveImageUrl).WithLocation(8, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectContentTypeWithEmptyInheritedImageUrl()
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

                    [ContentType]
                    [CustomImageUrl("""")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentTypeShouldHaveImageUrl).WithLocation(24, 22).WithArguments("TypeName"));
        }
    }
}