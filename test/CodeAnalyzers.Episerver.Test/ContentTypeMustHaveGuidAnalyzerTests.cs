using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeMustHaveGuidAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeMustHaveGuidAnalyzerTests
    {
        [Fact]
        public async Task IgnoreEmptySource()
        {
            await Verify.VerifyAnalyzerAsync("");
        }

        [Fact]
        public async Task IgnoreOtherAttribute()
        {
            var test = @"
                using System;

                namespace Test
                {
                    public class OtherAttribute : Attribute
                    {
                    }

                    [Other]
                    public class TypeName
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentTypeWithGuid()
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
        public async Task IgnoreCustomContentTypeWithGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentTypeWithNoArgumentList()
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
        public async Task DetectCustomContentTypeWithNoArgumentList()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectContentTypeWithEmptyArgumentList()
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
        public async Task DetectCustomContentTypeWithEmptyArgumentList()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType()]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithEmptyGuid()
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

        [Fact(Skip = "TODO")]
        public async Task DetectCustomContentTypeWithEmptyGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID="")]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(6, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypeWithInvalidGuid()
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

        [Fact(Skip = "TODO")]
        public async Task DetectCustomContentTypeWithInvalidGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID=""abc"")]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(6, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}