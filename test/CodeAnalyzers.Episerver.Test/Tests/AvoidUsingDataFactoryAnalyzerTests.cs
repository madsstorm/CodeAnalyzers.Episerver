using CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace CodeAnalyzers.Episerver.Test.Tests
{
    [TestClass]
    public class AvoidUsingDataFactoryAnalyzerTests : DiagnosticVerifier
    {
        [TestMethod]
        public void CanAnalyzeEmptySource()
        {
            var test = "";

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new AvoidUsingDataFactoryAnalyzer();
    }
}
