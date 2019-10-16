using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.AvoidUsingDataFactoryAnalyzer>;

using CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp;
using System.Threading.Tasks;
using Xunit;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingDataFactoryAnalyzerTests
    {
        [Fact]
        public async Task IgnoreEmptySource()
        {
            await Verify.VerifyAnalyzerAsync("");
        }

        [Fact]
        public async Task IgnoreCustomDataFactory()
        {
            var test = @"
                namespace Custom
                {
                    public class DataFactory
                    {
                        public static DataFactory Instance {get;}
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
                            var factory = DataFactory.Instance;
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 43);

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
                            var factory = DataFactory.Instance.GetChildren(null);
                        }
                    }
                }";

            var expected = Verify.Diagnostic().WithLocation(10, 43);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectEvent()
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
        public async Task DetectTypeAlias()
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
        public async Task DetectStaticUsing()
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

        [Fact]
        public void IgnoreEmptyAnalysisContext()
        {
            var analyzer = new AvoidUsingDataFactoryAnalyzer();

            analyzer.Initialize(null);
        }

        [Fact]
        public void IgnoreEmptySyntaxNode()
        {
            var analyzer = new AvoidUsingDataFactoryAnalyzer();
            var syntaxContext = new SyntaxNodeAnalysisContext();

            analyzer.SyntaxNodeAction(syntaxContext);
        }

        [Fact]
        public void IgnoreEmptySemanticModel()
        {
            var analyzer = new AvoidUsingDataFactoryAnalyzer();
            var syntaxContext = new SyntaxNodeAnalysisContext();

            analyzer.AnalyzeIdentifierName(syntaxContext, null);
        }
    }
}