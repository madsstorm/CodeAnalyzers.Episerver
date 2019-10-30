using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentPropertyDisplayUniqueOrderAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentPropertyDisplayUniqueOrderTests
    {
        [Fact]
        public async Task IgnoreContentPropertiesWithUniqueOrder()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using System.ComponentModel.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

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

                    [CatalogContentType]
                    public class VariationType : VariationContent
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 200)]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentPropertiesWithoutDisplayAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using System.ComponentModel.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType]
                    public class PageType : PageData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        public virtual string Intro {get;set;}
                    }

                    [ContentType]
                    public class OtherType : PageData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 200)]
                        public virtual string Intro {get;set;}

                        public virtual string MainBody {get;set;}
                    }

                    [ContentType]
                    public class BlockType : BlockData
                    {
                        public virtual string Title {get;set;}

                        [Display(Order = 100)]
                        public virtual string Intro {get;set;}

                        [Display(Order = 200)]
                        public virtual string MainBody {get;set;}
                    }

                    [CatalogContentType]
                    public class VariationType : VariationContent
                    {
                        public virtual string Title {get;set;}

                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }


        [Fact]
        public async Task DetectContentPropertiesWithDuplicateOrder()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using System.ComponentModel.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType]
                    public class PageType : PageData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 100)]
                        public virtual string Intro {get;set;}

                        [Display(Order = 100)]
                        public virtual string MainBody {get;set;}

                        [Display(Order = 100)]
                        public virtual string Footer {get;set;}
                    }

                    [ContentType]
                    public class BlockType : BlockData
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 100)]
                        public virtual string Intro {get;set;}

                        [Display(Order = 200)]
                        public virtual string MainBody {get;set;}

                        [Display(Order = 300)]
                        public virtual string Footer {get;set;}
                    }

                    [CatalogContentType]
                    public class VariationType : VariationContent
                    {
                        [Display(Order = 100)]
                        public virtual string Title {get;set;}

                        [Display(Order = 100)]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(14, 47).WithArguments("PageType.Title"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(17, 47).WithArguments("PageType.Intro"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(20, 47).WithArguments("PageType.MainBody"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(23, 47).WithArguments("PageType.Footer"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(30, 47).WithArguments("BlockType.Title"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(33, 47).WithArguments("BlockType.Intro"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(46, 47).WithArguments("VariationType.Title"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(49, 47).WithArguments("VariationType.Intro"));
        }
    }
}