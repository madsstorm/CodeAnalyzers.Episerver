#if INTERNAL_NAMESPACES

using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BannedApiAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingInternalNamespacesTests
    {
        [Fact]
        public async Task IgnoreEmptySource()
        {
            await Verify.VerifyAnalyzerAsync("");
        }

        [Fact]
        public async Task IgnorePublicProperty()
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
        public async Task IgnorePublicMethod()
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
        public async Task IgnorePublicEvent()
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
        public async Task DetectInternalProperty()
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

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(10, 40).WithArguments("DefaultPageRouteHelper");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectInternalMethod()
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

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(12, 29).WithArguments("DefaultContentRepository");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectStaticUsingInternalEvent()
        {
            var test = @"
                using static EPiServer.Core.Internal.DefaultContentEvents;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var events = Instance;
                            events.SavedContent += null;
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(10, 42).WithArguments("DefaultContentEvents");
            var expected2 = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(11, 29).WithArguments("DefaultContentEvents");

            await Verify.VerifyAnalyzerAsync(test, expected, expected2);
        }

        [Fact]
        public async Task DetectStaticUsingInternalMethod()
        {
            var test = @"
                using static EPiServer.Core.Internal.ThumbnailManager;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            CreateThumbnailUri(null,null);
                        }
                    }
                }";


            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(10, 29).WithArguments("ThumbnailManager");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectInternalClassAttribute()
        {
            var test = @"
                using EPiServer.Data.Dynamic.Internal;

                namespace Test
                {
                    [Queryable]
                    public class TypeName
                    {
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(6, 22).WithArguments("QueryableAttribute");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectInternalMethodAttribute()
        {
            var test = @"
                using EPiServer.Cms.Shell.UI.Controllers.Internal;

                namespace Test
                {
                    public class TypeName
                    {
                        [JsonErrorHandling]
                        public void Test()
                        {
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(8, 26).WithArguments("JsonErrorHandlingAttribute");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectInternalCreation()
        {
            var test = @"
                using EPiServer.Web.Internal;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var segment = new UrlSegment(null);
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(10, 43).WithArguments("UrlSegment");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectInternalTypeParameterCreation()
        {
            var test = @"
                using EPiServer.Web.Internal;
                using System.Collections.Generic;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var list = new List<UrlSegment>();
                        }                        
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(11, 40).WithArguments("UrlSegment");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectInternalArrayCreation()
        {
            var test = @"
                using EPiServer.Web.Internal;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var arr = new UrlSegment[0];
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi1002AvoidUsingInternalNamespaces).WithLocation(10, 39).WithArguments("UrlSegment");

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}

#endif