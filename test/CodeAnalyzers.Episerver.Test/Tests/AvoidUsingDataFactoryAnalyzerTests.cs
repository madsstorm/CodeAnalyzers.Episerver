using CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace CodeAnalyzers.Episerver.Test.Tests
{
    [TestClass]
    public class AvoidUsingDataFactoryAnalyzerTests : DiagnosticVerifier
    {
        private const string EPiServerDataFactory = @"
        namespace EPiServer
        {
            public class DataFactory
            {
                public static DataFactory Instance { get; }
                public int Method(int i) { return i; }
            }
        }";

        private const string CustomDataFactory = @"
        namespace Custom
        {
            public class DataFactory
            {
                public static DataFactory Instance { get; }
            }
        }";

        [TestMethod]
        public void CanIgnoreEmptySource()
        {
            VerifyCSharpDiagnostic("");
        }

        [TestMethod]
        public void CanIgnoreCustomDataFactoryClass()
        {
            var test = @"
                using Custom;

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

            VerifyCSharpDiagnostic(new string[] { CustomDataFactory, test });
        }

        [TestMethod]
        public void CanDetectPropertyReference()
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

            var expectedPropertyReference = new DiagnosticResult
            {
                Id = Descriptors.EPI1000_AvoidUsingDataFactory.Id,
                Message = string.Format(Descriptors.EPI1000_AvoidUsingDataFactory.MessageFormat.ToString(), "DataFactory.Instance"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                   new[] {
                            new DiagnosticResultLocation("Test1.cs", 10, 43)
                       }
            };

            VerifyCSharpDiagnostic(new string[] { EPiServerDataFactory, test }, expectedPropertyReference);
        }

        [TestMethod]
        public void CanDetectMethodInvocation()
        {
            var test = @"
                using EPiServer;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            DataFactory.Instance.Method(0);
                        }
                    }
                }";

            var expectedPropertyReference = new DiagnosticResult
            {
                Id = Descriptors.EPI1000_AvoidUsingDataFactory.Id,
                Message = string.Format(Descriptors.EPI1000_AvoidUsingDataFactory.MessageFormat.ToString(), "DataFactory.Instance"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                   new[] {
                            new DiagnosticResultLocation("Test1.cs", 10, 29)
                       }
            };

            var expectedMethodInvocation = new DiagnosticResult
            {
                Id = Descriptors.EPI1000_AvoidUsingDataFactory.Id,
                Message = string.Format(Descriptors.EPI1000_AvoidUsingDataFactory.MessageFormat.ToString(), "DataFactory.Method(int)"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                   new[] {
                            new DiagnosticResultLocation("Test1.cs", 10, 29)
                       }
            };

            VerifyCSharpDiagnostic(new string[] { EPiServerDataFactory, test }, expectedMethodInvocation, expectedPropertyReference);
        }

        [TestMethod]
        public void CanDetectMethodReference()
        {
            var test = @"
                using EPiServer;
                using System;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = DataFactory.Instance;
                            Func<int,int> method = factory.Method;
                        }
                    }
                }";

            var expectedPropertyReference = new DiagnosticResult
            {
                Id = Descriptors.EPI1000_AvoidUsingDataFactory.Id,
                Message = string.Format(Descriptors.EPI1000_AvoidUsingDataFactory.MessageFormat.ToString(), "DataFactory.Instance"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                   new[] {
                            new DiagnosticResultLocation("Test1.cs", 11, 43)
                       }
            };

            var expectedMethodInvocation = new DiagnosticResult
            {
                Id = Descriptors.EPI1000_AvoidUsingDataFactory.Id,
                Message = string.Format(Descriptors.EPI1000_AvoidUsingDataFactory.MessageFormat.ToString(), "DataFactory.Method(int)"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                   new[] {
                            new DiagnosticResultLocation("Test1.cs", 12, 52)
                       }
            };

            VerifyCSharpDiagnostic(new string[] { EPiServerDataFactory, test }, expectedPropertyReference, expectedMethodInvocation);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new AvoidUsingDataFactoryAnalyzer();
    }
}
