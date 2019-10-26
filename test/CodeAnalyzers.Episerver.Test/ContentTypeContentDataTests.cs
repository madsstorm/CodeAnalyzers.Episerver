using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeContentDataAnalyzer>;
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
                    [ContentType]
                    public abstract class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1003ContentTypeMustImplementContentData)
                    .WithLocation(8, 43).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectNonContentDataType()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType]
                    public class TypeName : object
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1003ContentTypeMustImplementContentData)
                    .WithLocation(8, 34).WithArguments("TypeName"));
        }
    }
}
