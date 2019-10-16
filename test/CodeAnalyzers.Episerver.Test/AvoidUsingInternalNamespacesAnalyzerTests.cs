using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.AvoidUsingInternalNamespacesAnalyzer>;

using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingInternalNamespacesAnalyzerTests
    {
        [Fact]
        public async Task CanIgnoreEmptySource()
        {
            await Verify.VerifyAnalyzerAsync("");
        }

        [Fact]
        public async Task CanIgnorePublicProperty()
        {
            var test = @"
                using EPiServer.Web.Routing;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(IPageRouteHelper helper)
                        {
                            var page = helper.Page;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task CanIgnorePublicMethod()
        {
            var test = @"
                using System;
                using EPiServer;
                using EPiServer.Core;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(IContentRepository repository)
                        {
                            repository.Get<PageData>(Guid.NewGuid());
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task CanIgnorePublicEvent()
        {
            var test = @"
                using EPiServer.Core;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(IContentEvents events)
                        {
                            events.SavedContent += null;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task CanDetectInternalProperty()
        {
            var test = @"
                using EPiServer.Web.Routing.Internal;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(DefaultPageRouteHelper helper)
                        {
                            var page = helper.Page;
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 40).WithArguments("EPiServer.Web.Routing.Internal");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectInternalMethod()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.Core.Internal;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(DefaultContentRepository repository)
                        {
                            repository.Get<PageData>(Guid.NewGuid());
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(12, 29).WithArguments("EPiServer.Core.Internal");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectInternalMediachaseMethod()
        {
            var test = @"
                using Mediachase.Commerce.Internal;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(ConfigurationReader reader)
                        {
                            reader.ReadInt(null);
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 29).WithArguments("Mediachase.Commerce.Internal");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectStaticUsingInternalEvent()
        {
            var test = @"
                using static EPiServer.Commerce.Order.Internal.DefaultOrderEvents;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var events = Instance;
                            events.SavedOrder += null;
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(11, 29).WithArguments("EPiServer.Commerce.Order.Internal");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}