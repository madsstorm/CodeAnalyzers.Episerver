using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeArgumentsAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeArgumentsTests
    {
        [Fact]
        public async Task IgnoreContentTypeWithArguments()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentTypeWithInheritedGroupName()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                        public CustomContentTypeAttribute()
                        {
                            GroupName = ""GroupName"";
                        }
                    }

                    [CustomContentType(DisplayName = ""DisplayName"",
                                       Description = ""Description"",
                                       Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentTypeWithMissingArguments()
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
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22),
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22),
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22),
                Verify.Diagnostic(Descriptors.Epi2003ContentTypeShouldHaveOrder).WithLocation(7, 22));
        }

        [Fact]
        public async Task DetectContentTypeWithNullValues()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(DisplayName = null,
                                 Description = null,
                                 GroupName = null,
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22),
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22),
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22));
        }

        [Fact]
        public async Task DetectContentTypeWithEmptyValues()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(DisplayName = """",
                                 Description = """",
                                 GroupName = """",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22),
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22),
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22));
        }

        [Fact]
        public async Task DetectContentTypeWithMissingDisplayName()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22));
        }

        [Fact]
        public async Task DetectContentTypeWithMissingDescription()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(DisplayName = ""DisplayName"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify
                .VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22));
        }

        [Fact]
        public async Task DetectContentTypeWithMissingGroupName()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22));
        }

        [Fact]
        public async Task DetectContentTypeWithMissingOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2003ContentTypeShouldHaveOrder).WithLocation(7, 22));
        }
    }
}