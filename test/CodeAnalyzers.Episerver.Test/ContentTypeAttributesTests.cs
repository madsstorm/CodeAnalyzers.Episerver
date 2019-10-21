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

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingDisplayName()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi2000ContentTypeShouldHaveDisplayName).WithLocation(6, 22).WithArguments("TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingDescription()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi2001ContentTypeShouldHaveDescription).WithLocation(6, 22).WithArguments("TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingGroupName()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 Order = 100)]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi2002ContentTypeShouldHaveGroupName).WithLocation(6, 22).WithArguments("TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithMissingOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 GroupName = ""GroupName"",
                                 Description = ""Description"")]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi2003ContentTypeShouldHaveOrder).WithLocation(6, 22).WithArguments("TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypesWithDuplicateOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class TypeName
                    {
                    }

                    [ContentType(GUID = ""07EC1BB0-8355-424E-991C-9098418EF64B"",
                                 DisplayName = ""DisplayName"",
                                 Description = ""Description"",
                                 GroupName = ""GroupName"",
                                 Order = 100)]
                    public class OtherType
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(6, 22).WithArguments("TypeName", "OtherType");
            var expected2 = Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(15, 22).WithArguments("OtherType", "TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected, expected2);
        }
    }
}