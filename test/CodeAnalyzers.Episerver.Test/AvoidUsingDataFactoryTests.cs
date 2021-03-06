﻿using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BannedApiAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingDataFactoryTests
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

            var expected = Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(10, 43);

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

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(10, 43),
                Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(10, 43));
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

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(10, 29),
                Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(10, 29));
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

            var expected = Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(12, 43);

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

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(10, 43),
                Verify.Diagnostic(Descriptors.Epi3000AvoidUsingDataFactory).WithLocation(11, 29));
        }
    }
}