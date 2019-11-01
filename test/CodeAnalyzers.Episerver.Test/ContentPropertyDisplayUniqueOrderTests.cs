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
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(13, 26).WithArguments("PageType.Title", "PageType.Intro"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(16, 26).WithArguments("PageType.Intro", "PageType.MainBody"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(19, 26).WithArguments("PageType.MainBody", "PageType.Footer"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(29, 26).WithArguments("BlockType.Title", "BlockType.Intro"),
                Verify.Diagnostic(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder).WithLocation(45, 26).WithArguments("VariationType.Title", "VariationType.Intro"));
        }
    }
}