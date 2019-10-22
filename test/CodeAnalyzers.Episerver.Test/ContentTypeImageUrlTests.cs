using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeImageUrlTests
    {
        [Fact]
        public async Task IgnoreContentTypeWithAllAttributes()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
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

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
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

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    [CustomImageUrl]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithoutImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentTypeShouldHaveImageUrl).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithNullImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    [ImageUrl(null)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentTypeShouldHaveImageUrl).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithEmptyImageUrl()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    [ImageUrl("""")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2005ContentTypeShouldHaveImageUrl).WithLocation(7, 22).WithArguments("TypeName"));
        }
    }
}