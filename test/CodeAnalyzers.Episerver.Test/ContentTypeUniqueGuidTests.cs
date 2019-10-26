using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeGuidAnalyzer>;
using System.Threading.Tasks;
using Xunit;


namespace CodeAnalyzers.Episerver.Cms10.Test
{
    public class ContentTypeUniqueGuidTests
    {
        [Fact]
        public async Task IgnoreContentTypesWithUniqueGuids()
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

                    [ContentType(GUID = ""71D42C7D-FBA6-420C-A837-49C2330AA5C1"")]
                    public class OtherTypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreCustomContentTypesWithUniqueGuids()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName : PageData
                    {
                    }

                    [CustomContentType(GUID = ""71D42C7D-FBA6-420C-A837-49C2330AA5C1"")]
                    public class OtherTypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreMixedContentTypesWithUniqueGuids()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName : PageData
                    {
                    }

                    [CustomContentType(GUID = ""71D42C7D-FBA6-420C-A837-49C2330AA5C1"")]
                    public class OtherTypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentTypesWithSameGuid()
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

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class OtherTypeName : PageData
                    {
                    }
                }";
            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(7, 22).WithArguments("TypeName", "OtherTypeName"),
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(12, 22).WithArguments("OtherTypeName", "TypeName"));
        }

        [Fact]
        public async Task DetectSameGuidWithDifferentFormat()
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

                    [ContentType(GUID = ""{1F218487-9C23-4944-A0E6-76FC1995CBF0}"")]
                    public class OtherTypeName : PageData
                    {
                    }
                }";
            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(7, 22).WithArguments("TypeName", "OtherTypeName"),
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(12, 22).WithArguments("OtherTypeName", "TypeName"));
        }

        [Fact]
        public async Task DetectCustomContentTypesWithSameGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName : PageData
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class OtherTypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(11, 22).WithArguments("TypeName", "OtherTypeName"),
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(16, 22).WithArguments("OtherTypeName", "TypeName"));
        }

        [Fact]
        public async Task DetectMixedContentTypesWithSameGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName : PageData
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class OtherTypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(11, 22).WithArguments("TypeName", "OtherTypeName"),
                Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(16, 22).WithArguments("OtherTypeName", "TypeName"));
        }
    }
}
