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
        public async Task CanDetectProperty()
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
        public async Task CanDetectMethod()
        {
            var test = @"
                using EPiServer;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = DataFactory.Instance.GetChildren(null);
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 43);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task CanDetectEvent()
        {
            var test = @"
                using EPiServer;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            DataFactory.Instance.SavingContent += null;
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 29);

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
                            factory.GetChildren(null);
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(11, 29);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }
    }
}
