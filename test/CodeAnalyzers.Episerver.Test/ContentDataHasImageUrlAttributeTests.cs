using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentDataHasImageUrlAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentDataHasImageUrlAttributeTests
    {
        [Fact]
        public async Task IgnoreContentTypeWithImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
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
        public async Task DetectContentTypeWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentDataShouldHaveImageUrlAttribute).WithLocation(7, 34));
        }

        [Fact]
        public async Task DetectContentTypeWithNullImageUrl()
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
        public async Task DetectContentTypeWithEmptyImageUrl()
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