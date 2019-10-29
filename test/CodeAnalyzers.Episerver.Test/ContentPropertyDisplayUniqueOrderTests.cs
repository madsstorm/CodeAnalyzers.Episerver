using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentPropertyDisplayUniqueOrderAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentPropertyDisplayUniqueOrderTests
    {
        [Fact(Skip = "TODO")]
        public async Task IgnoreContentPropertiesWithUniqueOrder()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;

                namespace Test
                {
                    [ContentType]
                    public class PageType : PageData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 200)]
                        public virtual string Intro {get;set;}
                    }

                    [ContentType]
                    public class OtherType : PageData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 200)]
                        public virtual string Intro {get;set;}
                    }

                    [ContentType]
                    public class BlockType : BlockData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 200)]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }


        [Fact(Skip = "TODO")]
        public async Task DetectContentPropertiesWithDuplicateOrder()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;

                namespace Test
                {
                    [ContentType]
                    public class PageType : PageData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 100)]
                        public virtual string Intro {get;set;}
                    }

                    [ContentType]
                    public class BlockType : BlockData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 100)]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(12, 47).WithArguments("Title", "Intro"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(15, 47).WithArguments("Intro", "Title"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(22, 22).WithArguments("Title", "Intro"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(25, 22).WithArguments("Intro", "Title"));
        }
    }
}