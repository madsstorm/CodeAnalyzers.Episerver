using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BannedApiAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingOrderContextTests
    {
        [Fact]
        public async Task IgnoreCustomOrderContext()
        {
            var test = @"
                namespace Custom
                {
                    public class OrderContext
                    {
                        public static OrderContext Instance {get;}
                    }
                }

                namespace Test
                {
                    using Custom;

                    public class TypeName
                    {
                        public void Test()
                        {
                            var context = OrderContext.Instance;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectProperty()
        {
            var test = @"
                using Mediachase.Commerce.Orders;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var instance = OrderContext.Current;
                        }
                    }
                }";

            var expected = Verify.Diagnostic(Descriptors.Epi3003AvoidUsingOrderContext).WithLocation(10, 44);

            await Verify.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task DetectMethod()
        {
            var test = @"
                using System;
                using Mediachase.Commerce.Orders;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            var cart = OrderContext.Current.GetCart("""", Guid.NewGuid());
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi3003AvoidUsingOrderContext).WithLocation(11, 40),
                Verify.Diagnostic(Descriptors.Epi3003AvoidUsingOrderContext).WithLocation(11, 40));
        }

        [Fact]
        public async Task DetectEvent()
        {
            var test = @"
                using Mediachase.Commerce.Orders;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            OrderContext.Current.OrderGroupUpdated += null;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi3003AvoidUsingOrderContext).WithLocation(10, 29),
                Verify.Diagnostic(Descriptors.Epi3003AvoidUsingOrderContext).WithLocation(10, 29));
        }
    }
}