using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentPropertyDisplayArgumentsAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentPropertyDisplayArgumentsTests
    {
        [Fact(Skip = "TODO")]
        public async Task IgnoreContentPropertyWithArguments()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;

                namespace Test
                {
                    [ContentType]
                    public class TypeName : PageData
                    {
                        [Display(
                            Name = ""Name"",
                            Description = ""Description"",
                            GroupName = ""GroupName"",
                            Order = 100)]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentPropertyWithMissingArguments()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;

                namespace Test
                {
                    [ContentType]
                    public class TypeName : PageData
                    {
                        [Display]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(11, 22),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(11, 22),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(11, 22),
                Verify.Diagnostic(Descriptors.Epi2009ContentPropertyShouldHaveOrder).WithLocation(11, 22));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentPropertyWithNullValues()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;

                namespace Test
                {
                    [ContentType]
                    public class TypeName : PageData
                    {
                        [Display(
                            Name = null,
                            Description = null,
                            GroupName = null,
                            Order = 100)]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(11, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(11, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(11, 26));
        }

        [Fact(Skip = "TODO")]
        public async Task DetectContentPropertyWithEmptyValues()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;

                namespace Test
                {
                    [ContentType]
                    public class TypeName : PageData
                    {
                        [Display(
                            Name = """",
                            Description = """",
                            GroupName = """",
                            Order = 100)]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(11, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(11, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(11, 26));
        }
    }
}