using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeContentDataTests
    {
        [Fact]
        public async Task DetectAbstractContentDataType()
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
                    public abstract class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1003ContentTypeMustImplementContentData).WithLocation(13, 43).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectNonContentDataType()
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
                    public class TypeName : object
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1003ContentTypeMustImplementContentData).WithLocation(13, 34).WithArguments("TypeName"));
        }
    }
}
