using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentPropertyDisplayArgumentsAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentPropertyDisplayArgumentsTests
    {
        [Fact]
        public async Task IgnoreContentPropertyWithArguments()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

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

                    [CatalogContentType]
                    public class ProductType : ProductContent
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

        [Fact]
        public async Task DetectContentPropertyWithMissingArguments()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    [ContentType]
                    public class TypeName : PageData
                    {
                        [Display]
                        public virtual string Intro {get;set;}
                    }

                    [CatalogContentType]
                    public class ProductType : ProductContent
                    {
                        [Display]
                        public virtual string Intro {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2009ContentPropertyShouldHaveOrder).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(20, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(20, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(20, 26),
                Verify.Diagnostic(Descriptors.Epi2009ContentPropertyShouldHaveOrder).WithLocation(20, 26));
        }

        [Fact]
        public async Task DetectContentPropertyWithNullValues()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

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

                    [CatalogContentType]
                    public class ProductType : ProductContent
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
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(24, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(24, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(24, 26));
        }

        [Fact]
        public async Task DetectContentPropertyWithEmptyValues()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using System.ComponentModel.DataAnnotations;
                using EPiServer.Commerce.Catalog.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

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

                    [CatalogContentType]
                    public class ProductType : ProductContent
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
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(13, 26),
                Verify.Diagnostic(Descriptors.Epi2006ContentPropertyShouldHaveName).WithLocation(24, 26),
                Verify.Diagnostic(Descriptors.Epi2007ContentPropertyShouldHaveDescription).WithLocation(24, 26),
                Verify.Diagnostic(Descriptors.Epi2008ContentPropertyShouldHaveGroupName).WithLocation(24, 26));
        }
    }
}