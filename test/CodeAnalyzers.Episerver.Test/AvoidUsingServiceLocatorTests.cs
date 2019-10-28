using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BannedApiAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingServiceLocatorTests
    {
        [Fact]
        public async Task IgnoreCustomServiceLocator()
        {
            var test = @"
                namespace Custom
                {
                    public class ServiceLocator
                    {
                        public static object Current {get;}
                    }
                }

                namespace Test
                {
                    using Custom;

                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = ServiceLocator.Current;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectServiceLocatorProperty()
        {
            var test = @"
                using EPiServer.ServiceLocation;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = ServiceLocator.Current;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008AvoidUsingServiceLocator).WithLocation(10, 43));
        }

        [Fact]
        public async Task DetectServiceLocatorMethod()
        {
            var test = @"
                using EPiServer.ServiceLocation;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var factory = ServiceLocator.Current.GetInstance<string>();
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008AvoidUsingServiceLocator).WithLocation(10, 43));
        }
    }
}