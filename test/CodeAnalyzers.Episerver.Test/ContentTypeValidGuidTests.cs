﻿using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentTypeValidGuidTests
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
                using EPiServer.Core;

                namespace Test
                {
                    public class OtherAttribute : Attribute
                    {
                    }

                    [Other]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentTypesWithGuids()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    [ImageUrl(""image.png"")]
                    public class PlainType : PageData
                    {
                    }

                    [ContentType(GUID = ""{9B2399B3-1840-4FFC-975F-F140BCB1D72C}"")]
                    [ImageUrl(""image.png"")]
                    public class CurlyType : PageData
                    {
                    }

                    [ContentType(GUID = ""(41364022-AED5-4294-B20D-5352AFCCEEB3)"")]
                    [ImageUrl(""image.png"")]
                    public class ParensType : PageData
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
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
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
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectCustomContentTypeWithNoArgumentList()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(11, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectContentTypeWithEmptyArgumentList()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType()]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectCustomContentTypeWithEmptyArgumentList()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType()]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(11, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectContentTypeWithEmptyGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID="""")]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectCustomContentTypeWithEmptyGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID="""")]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(11, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectContentTypeWithInvalidGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [ContentType(GUID=""abc"")]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(7, 22).WithArguments("TypeName"));
        }

        [Fact]
        public async Task DetectCustomContentTypeWithInvalidGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID=""abc"")]
                    [ImageUrl(""image.png"")]
                    public class TypeName : PageData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1000ContentTypeMustHaveValidGuid).WithLocation(11, 22).WithArguments("TypeName"));
        }
    }
}