using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentAreaPropertyHasAllowedTypesAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentAreaPropertyHasAllowedTypesAttributeTests
    {
        [Fact]
        public async Task IgnoreContentAreaPropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentAreaPropertyInNonContentDataWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class OtherType
                    {
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentAreaPropertyInInterfaceWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface IOtherType
                    {
                        ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentAreaPropertyInContentDataInterfaceWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ITypeName : IContentData
                    {
                        ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentAreaPropertyWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2015ContentAreaPropertyShouldHaveAllowedTypesAttribute).WithLocation(9, 52));
        }

        [Fact]
        public async Task DetectContentAreaPropertyInAbstractContentDataWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public abstract class TypeNameBase : PageData
                    {
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2015ContentAreaPropertyShouldHaveAllowedTypesAttribute).WithLocation(9, 52));
        }
    }
}