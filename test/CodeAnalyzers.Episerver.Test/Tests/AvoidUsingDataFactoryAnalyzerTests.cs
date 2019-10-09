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
        [TestMethod]
        public void CanIgnoreEmptySource()
        {
            VerifyCSharpDiagnostic("");
        }

        [TestMethod]
        public void CanIgnoreCustomDataFactoryClass()
        {
            var customDataFactory = @"
                namespace Custom
                {
                    public class DataFactory
                    {
                        public static DataFactory Instance { get; }
                    }
                }";

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

            VerifyCSharpDiagnostic(new string[] { customDataFactory, test });
        }

        [TestMethod]
        public void CanDetectPropertyAccess()
        {
            var EPiServerDataFactory = @"
                namespace EPiServer
                {
                    public class DataFactory
                    {
                        public static DataFactory Instance { get; }
                    }
                }";

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

            var expected = new DiagnosticResult
            {
                Id = DiagnosticIds.AvoidUsingDataFactoryAnalyzerRuleId,
                Message = "Avoid using legacy API DataFactory.Instance",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                   new[] {
                            new DiagnosticResultLocation("Test1.cs", 10, 43)
                       }
            };

            VerifyCSharpDiagnostic(new string[] { EPiServerDataFactory, test }, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new AvoidUsingDataFactoryAnalyzer();
    }
}
