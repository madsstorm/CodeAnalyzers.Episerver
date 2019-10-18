using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BannedApiAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingCacheManagerTests
    {
        [Fact]
        public async Task IgnoreEmptySource()
        {
            await Verify.VerifyAnalyzerAsync("");
        }

        [Fact]
        public async Task IgnoreCustomCacheManager()
        {
            var test = @"
                namespace Custom
                {
                    public class CacheManager
                    {
                        public static string VersionKey {get;}
                    }
                }

                namespace Test
                {
                    using Custom;

                    public class TypeName
                    {
                        public void Test()
                        {
                            var key = CacheManager.VersionKey;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectProperty()
        {
            var test = @"
                using EPiServer;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var key = CacheManager.VersionKey;
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi3001AvoidUsingCacheManager).WithLocation(10, 39);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectMethod()
        {
            var test = @"
                using EPiServer;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            CacheManager.Clear();
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi3001AvoidUsingCacheManager).WithLocation(10, 29);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectTypeAliasProperty()
        {
            var test = @"
                using MyManager = EPiServer.CacheManager;

                namespace Test
                {                   
                    using System;

                    public class TypeName
                    {
                        public void Test()
                        {
                            var key = MyManager.VersionKey;
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi3001AvoidUsingCacheManager).WithLocation(12, 39);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectStaticUsingMethod()
        {
            var test = @"
                using static EPiServer.CacheManager;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            Clear();
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi3001AvoidUsingCacheManager).WithLocation(10, 29);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectStaticUsingProperty()
        {
            var test = @"
                using static EPiServer.CacheManager;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var key = VersionKey;
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi3001AvoidUsingCacheManager).WithLocation(10, 39);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}
