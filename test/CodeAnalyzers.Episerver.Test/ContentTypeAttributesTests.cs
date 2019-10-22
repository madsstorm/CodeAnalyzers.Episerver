using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeAttributesTests
    {
        [Fact]
        public async Task IgnoreContentTypeWithAllValues()
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

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingValues()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22).WithArguments("TypeName"),
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22).WithArguments("TypeName"),
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22).WithArguments("TypeName"),
                Verify.Diagnostic(Descriptors.Epi2003ContentTypeShouldHaveOrder).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithNullValues()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = null,
                                 Description = null,
                                 GroupName = null,
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22).WithArguments("TypeName"),
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22).WithArguments("TypeName"),
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithEmptyValues()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = """",
                                 Description = """",
                                 GroupName = """",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22).WithArguments("TypeName"),
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22).WithArguments("TypeName"),
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingDisplayName()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingDescription()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingGroupName()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 GroupName = ""GroupName"",
                                 Description = ""Description"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2003ContentTypeShouldHaveOrder).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypesWithDuplicateOrder()
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

                    [ContentType(GUID = ""07EC1BB0-8355-424E-991C-9098418EF64B"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class OtherType : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(7, 22).WithArguments("TypeName", "OtherType"),
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(16, 22).WithArguments("OtherType", "TypeName"));
        }
    }
}