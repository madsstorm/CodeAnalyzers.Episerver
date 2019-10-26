using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeOrderAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Cms9.Test
{
    public class ContentTypeOrderTests
    {
        [Fact]
        public async Task IgnoreContentTypeWithOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentTypeWithoutOrder()
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
                Verify.Diagnostic(Descriptors.Epi2003ContentTypeShouldHaveOrder).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectContentTypesWithDuplicateOrder()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(Order = 100)]
                    public class TypeName : PageData
                    {
                    }

                    [ContentType(Order = 100)]
                    public class OtherType : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(7, 22).WithArguments("TypeName", "OtherType"),
                Verify.Diagnostic(Descriptors.Epi2004ContentTypeShouldHaveUniqueOrder).WithLocation(12, 22).WithArguments("OtherType", "TypeName"));
        }
    }
}
