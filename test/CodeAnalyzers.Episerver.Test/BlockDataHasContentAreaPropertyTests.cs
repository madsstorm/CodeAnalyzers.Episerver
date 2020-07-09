using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BlockDataHasContentAreaPropertyAnalyzer>;
using System.Threading.Tasks;
using Xunit;


namespace CodeAnalyzers.Episerver.Test
{
    public class BlockDataHasContentAreaPropertyTests
    {
        [Fact]
        public async Task IgnoreContentAreaPropertyInOtherContentData()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    public class PageType : PageData
                    {
                        public virtual ContentArea Area { get;set; }
                    }

                    public class ProductType : ProductContent
                    {
                        public virtual ContentArea Area { get;set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreOtherProperties()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    public class BlockType : BlockData
                    {
                        public virtual string Title { get;set; }
                        public virtual int Number { get;set; }
                        public virtual bool Ready { get;set; }
                        public virtual ContentReference Page { get;set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreContentAreaPropertyWithIgnoreAttributeInBlockData()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class BlockType : BlockData
                    {
                        [Ignore]
                        public virtual ContentArea MainArea { get;set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);           
        }

        [Fact]
        public async Task IgnoreNonVirtualContentAreaPropertyInBlockData()
        {
            var test = @"
                using EPiServer.Core;

                namespace Test
                {
                    public class BlockType : BlockData
                    {
                        public ContentArea MainArea { get;set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnorePrivateContentAreaPropertyInBlockData()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class BlockType : BlockData
                    {
                        private ContentArea MainArea { get;set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreGetOnlyContentAreaProperty()
        {
            var test = @"
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class BlockType : BlockData
                    {
                        public virtual ContentArea MainArea { get; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectContentAreaPropertyInBlockData()
        {
            var test = @"
                using EPiServer.Core;

                namespace Test
                {
                    public class BlockType : BlockData
                    {
                        public virtual ContentArea MainArea { get;set; }

                        public virtual ContentArea SecondArea { get;set; }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi2011AvoidContentAreaPropertyInBlock).WithLocation(8, 52),
                Verify.Diagnostic(Descriptors.Epi2011AvoidContentAreaPropertyInBlock).WithLocation(10, 52));
        }
    }
}