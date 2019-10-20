﻿using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentTypeAnalyzer>;
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

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName
                    {
                    }

                    [ContentType(GUID = ""71D42C7D-FBA6-420C-A837-49C2330AA5C1"")]
                    public class OtherTypeName
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

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName
                    {
                    }

                    [CustomContentType(GUID = ""71D42C7D-FBA6-420C-A837-49C2330AA5C1"")]
                    public class OtherTypeName
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

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName
                    {
                    }

                    [CustomContentType(GUID = ""71D42C7D-FBA6-420C-A837-49C2330AA5C1"")]
                    public class OtherTypeName
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentTypesWithSameGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName
                    {
                    }

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class OtherTypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(6, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectCustomContentTypesWithSameGuid()
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

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class OtherTypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(10, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectMixedContentTypesWithSameGuid()
        {
            var test = @"
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class CustomContentTypeAttribute : ContentTypeAttribute
                    {
                    }

                    [ContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class TypeName
                    {
                    }

                    [CustomContentType(GUID = ""1F218487-9C23-4944-A0E6-76FC1995CBF0"")]
                    public class OtherTypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1001ContentTypeMustHaveUniqueGuid).WithLocation(10, 22).WithArguments("Test.TypeName");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}