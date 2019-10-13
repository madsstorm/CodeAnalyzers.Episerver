using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.AvoidUsingDataFactoryAnalyzer>;

using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingDataFactoryAnalyzerTests
    {
        [Fact]
        public async Task CanIgnoreEmptySource()
        {
            await Verify.VerifyAnalyzerAsync("");
        }

        [Fact]
        public async Task CanIgnoreCustomDataFactoryClass()
        {
            var test = @"
                namespace Custom
                {
                    public class DataFactory
                    {
                        public static DataFactory Instance { get; }
                    }
                }

                namespace Test
                {
                    using Custom;
                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = DataFactory.Instance;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task CanDetectPropertyReference()
        {
            var test = @"
                using EPiServer;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = DataFactory.Instance;
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 43);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectTypeAlias()
        {
            var test = @"
                using MyFactory = EPiServer.DataFactory;

                namespace Test
                {                   
                    using System;

                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = MyFactory.Instance;
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(12, 43);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectStaticUsing()
        {
            var test = @"
                using static EPiServer.DataFactory;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = Instance;
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 43);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}
