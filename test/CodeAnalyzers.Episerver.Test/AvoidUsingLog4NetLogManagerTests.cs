using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BannedApiAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingLog4NetLogManagerTests
    {
        [Fact]
        public async Task IgnoreEpiserverLogManager()
        {
            var test = @"
                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var log = EPiServer.Logging.LogManager.GetLogger();
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreEpiserverCompatibilityLogManager()
        {
            var test = @"
                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var log = EPiServer.Logging.Compatibility.LogManager.GetLogger("""");
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectLog4NetLogManager()
        {
            var test = @"
                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var log = log4net.LogManager.GetLogger("""");
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi3002AvoidUsingLog4NetLogManager).WithLocation(8, 39));
        }
    }
}