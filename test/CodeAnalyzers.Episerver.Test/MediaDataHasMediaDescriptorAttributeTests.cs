using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.MediaDataHasMediaDescriptorAttributeAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class MediaDataHasMediaDescriptorAttributeTests
    {
        [Fact]
        public async Task IgnoreMediaDataWithMediaDescriptorAttribute()
        {
            var test = @"
                using EPiServer.Framework.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    [MediaDescriptor]
                    public class ImageType : ImageData
                    {
                    }

                    [MediaDescriptor]
                    public class VideoType : VideoData
                    {
                    }

                    [MediaDescriptor]
                    public class MediaType : MediaData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreOtherTypes()
        {
            var test = @"
                using EPiServer.Commerce.Catalog.ContentTypes;
                using EPiServer.Framework.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class PageType : PageData
                    {
                    }

                    public class BlockType : BlockData
                    {
                    }

                    public class VariationType : VariationContent
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectMediaDataWithoutMediaDescriptorAttribute()
        {
            var test = @"
                using EPiServer.Framework.DataAnnotations;
                using EPiServer.Core;

                namespace Test
                {
                    public class ImageType : ImageData
                    {
                    }

                    public class VideoType : VideoData
                    {
                    }

                    public class MediaType : MediaData
                    {
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1007MediaDataShouldHaveMediaDescriptorAttribute).WithLocation(7, 34),
                Verify.Diagnostic(Descriptors.Epi1007MediaDataShouldHaveMediaDescriptorAttribute).WithLocation(11, 34),
                Verify.Diagnostic(Descriptors.Epi1007MediaDataShouldHaveMediaDescriptorAttribute).WithLocation(15, 34));
        }
    }
}
