using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentReferencePropertyHasAllowedTypesAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentReferencePropertyHasAllowedTypesAttributeTests
    {
        [Fact]
        public async Task IgnoreContentReferencePropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual ContentReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnorePageReferencePropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual PageReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentReferencePropertyInNonContentDataWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class OtherType
                    {
                        public virtual ContentReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentReferencePropertyInInterfaceWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface IOtherType
                    {
                        ContentReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentReferencePropertyInContentDataInterfaceWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ITypeName : IContentData
                    {
                        ContentReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnorePageReferencePropertyInContentDataInterfaceWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ITypeName : IContentData
                    {
                        PageReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentReferencePropertyWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        public virtual ContentReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2014ContentReferencePropertyShouldHaveAllowedTypesAttribute).WithLocation(9, 57));
        }

        [Fact]
        public async Task DetectContentReferencePropertyInAbstractContentDataWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public abstract class TypeNameBase : PageData
                    {
                        public virtual ContentReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2014ContentReferencePropertyShouldHaveAllowedTypesAttribute).WithLocation(9, 57));
        }

        [Fact]
        public async Task DetectPageReferencePropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        public virtual PageReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2014ContentReferencePropertyShouldHaveAllowedTypesAttribute).WithLocation(9, 54));
        }

        [Fact]
        public async Task DetectPageReferencePropertyInAbstractContentDataWithAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public abstract class TypeNameBase : PageData
                    {
                        public virtual PageReference Link {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2014ContentReferencePropertyShouldHaveAllowedTypesAttribute).WithLocation(9, 54));
        }
    }
}