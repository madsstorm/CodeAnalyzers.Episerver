using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.InterfaceInAllowedTypesHasUIDescriptorAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class InterfaceInAllowedTypesHasUIDescriptorTests
    {
        [Fact]
        public async Task IgnoreAllowedTypesAttributeWithoutArguments()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedTypesAttributeWithEmptyArguments()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes()]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedTypesAttributeWithEmptyTypeArrayArgument()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[0])]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedTypesAttributeWithEmptyTypeArrayArguments()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[0], new Type[0])]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedTypesAttributeWithEmptyTypeArrayAndSuffixArguments()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[0], new Type[0], """")]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreClassConstructorArgument()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public class CustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(CustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreClassConstructorArguments()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public class CustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(CustomBlock)}, new Type[] {typeof(CustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedTypesClassArgument()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public class CustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(AllowedTypes = new Type[] {typeof(CustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreRestrictedTypesClassArgument()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public class CustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(RestrictedTypes = new Type[] {typeof(CustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedAndRestrictedTypesClassArgument()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public class CustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(AllowedTypes = new Type[] {typeof(CustomBlock)}, RestrictedTypes = new Type[] {typeof(CustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreConstructorAndRestrictedTypesClassArgument()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public class CustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(CustomBlock)}, RestrictedTypes = new Type[] {typeof(CustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact(Skip = "TODO")]
        public async Task IgnoreInterfaceConstructorArgumentWithExternalUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes(typeof(IContent))]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact(Skip = "TODO")]
        public async Task IgnoreInterfaceConstructorArgumentsWithExternalUIDescriptors()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(IContent)}, new Type[] {typeof(IContentData)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreInterfaceConstructorArgumentWithUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreInterfaceConstructorArgumentWithCustomUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    public class CustomUIDescriptor<T> : UIDescriptor<T> { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : CustomUIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreInterfaceConstructorArgumentsWithUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)}, new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreInterfaceConstructorArgumentsAndSuffixWithUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)}, new Type[] {typeof(ICustomBlock)}, """")]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedTypesArgumentWithUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(AllowedTypes = new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreRestrictedTypesArgumentWithUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(RestrictedTypes = new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreAllowedAndRestrictedTypesArgumentWithUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(AllowedTypes = new Type[] {typeof(ICustomBlock)}, RestrictedTypes = new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreConstructorAndRestrictedTypesArgumentWithUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)}, RestrictedTypes = new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task IgnoreCustomClassPropertyInterfaceConstructorArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }

                    public class TypeName
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectPagePropertyInterfaceConstructorArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(12, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectInterfaceConstructorArgumentWithIncorrectUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptor : UIDescriptor<ICustomBlock2> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(17, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectBlockPropertyInterfaceConstructorArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }

                    public class TypeName : BlockData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(12, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectProductPropertyInterfaceConstructorArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Commerce.Catalog.ContentTypes;

                namespace Test
                {
                    public interface ICustomBlock { }

                    public class TypeName : ProductContent
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectInterfaceConstructorArgumentsWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)}, new Type[] {typeof(ICustomBlock2)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock2"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectInterfaceConstructorArgumentsAndSuffixWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)}, new Type[] {typeof(ICustomBlock2)}, """")]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock2"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectMultipleInterfaceConstructorArgumentsWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }
                    public interface ICustomBlock3 { }
                    public interface ICustomBlock4 { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock), typeof(ICustomBlock2)}, new Type[] {typeof(ICustomBlock3),typeof(ICustomBlock4)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(15, 26).WithArguments("ICustomBlock4"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(15, 26).WithArguments("ICustomBlock3"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(15, 26).WithArguments("ICustomBlock2"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(15, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectMixedInterfaceConstructorArgumentsWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;
                using EPiServer.Shell;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }
                    public interface ICustomBlock3 { }
                    public interface ICustomBlock4 { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptorA : UIDescriptor<ICustomBlock2> { }

                    [UIDescriptorRegistration]
                    public class ICustomBlockUIDescriptorB : UIDescriptor<ICustomBlock3> { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock), typeof(ICustomBlock2)}, new Type[] {typeof(ICustomBlock3),typeof(ICustomBlock4)})]
                        public virtual ContentArea Area {get;set;}

                        [AllowedTypes(new Type[] {typeof(ICustomBlock4), typeof(ICustomBlock3)}, new Type[] {typeof(ICustomBlock2),typeof(ICustomBlock)})]
                        public virtual ContentArea Area2 {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(22, 26).WithArguments("ICustomBlock4"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(22, 26).WithArguments("ICustomBlock"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(25, 26).WithArguments("ICustomBlock"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(25, 26).WithArguments("ICustomBlock4"));
        }

        [Fact]
        public async Task DetectAllowedTypesArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(AllowedTypes = new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(12, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectMultipleAllowedTypesArgumentsWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(AllowedTypes = new Type[] {typeof(ICustomBlock), typeof(ICustomBlock2)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock2"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectRestrictedTypesArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(RestrictedTypes = new Type[] {typeof(ICustomBlock)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(12, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectMultipleRestrictedTypesArgumentsWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(RestrictedTypes = new Type[] {typeof(ICustomBlock), typeof(ICustomBlock2)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock2"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectAllowedAndRestrictedTypesArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(AllowedTypes = new Type[] {typeof(ICustomBlock)}, RestrictedTypes = new Type[] {typeof(ICustomBlock2)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock2"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock"));
        }

        [Fact]
        public async Task DetectConstructorAndRestrictedTypesArgumentWithoutUIDescriptor()
        {
            var test = @"
                using System;
                using EPiServer.Core;
                using EPiServer.DataAnnotations;

                namespace Test
                {
                    public interface ICustomBlock { }
                    public interface ICustomBlock2 { }

                    public class TypeName : PageData
                    {
                        [AllowedTypes(new Type[] {typeof(ICustomBlock)}, RestrictedTypes = new Type[] {typeof(ICustomBlock2)})]
                        public virtual ContentArea Area {get;set;}
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock2"),
                Verify.Diagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor).WithLocation(13, 26).WithArguments("ICustomBlock"));
        }
    }
}