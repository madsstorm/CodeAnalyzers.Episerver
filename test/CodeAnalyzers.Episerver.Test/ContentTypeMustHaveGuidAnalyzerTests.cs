using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeMustHaveGuidAnalyzer>;

using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeMustHaveGuidAnalyzerTests
    {
        [Fact]
        public async Task CanIgnoreEmptySource()
        {
            await Verify.VerifyAnalyzerAsync("");
        }

        [Fact]
        public async Task CanIgnoreContentTypeWithGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task CanDetectContentTypeWithNoArgumentList()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(6, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectContentTypeWithEmptyArgumentList()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType()]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(6, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectContentTypeWithEmptyGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID="")]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(6, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectContentTypeWithInvalidGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID=""abc"")]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(6, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}