using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.ContentReferenceListPropertyHasAllowedTypesAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class ContentReferenceListPropertyHasAllowedTypesAttributeTests
    {
        [Fact]
        public async Task IgnoreContentReferenceEnumerablePropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual IEnumerable<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentReferenceListPropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual IList<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentReferenceCollectionPropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual ICollection<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnorePageReferenceEnumerablePropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual IEnumerable<PageReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentReferenceEnumerablePropertyInNonContentDataWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class OtherType
                    {
                        public virtual IEnumerable<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentReferenceEnumerablePropertyInInterfaceWithoutAllowedTypesAttribute()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface IOtherType
                    {
                        IEnumerable<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentReferenceEnumerablePropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        public virtual IEnumerable<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute).WithLocation(10, 70));
        }

        [Fact]
        public async Task DetectContentReferenceListPropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        public virtual IList<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute).WithLocation(10, 64));
        }

        [Fact]
        public async Task DetectContentReferenceCollectionPropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        public virtual ICollection<ContentReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute).WithLocation(10, 70));
        }

        [Fact]
        public async Task DetectPageReferenceEnumerablePropertyWithAllowedTypesAttribute()
        {
            var test = @"
                using System.Collections.Generic;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        public virtual IEnumerable<PageReference> List {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute).WithLocation(10, 67));
        }
    }
}