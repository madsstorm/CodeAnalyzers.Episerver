using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System.Linq;
using System;
using System.Collections.Concurrent;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InterfaceInAllowedTypesHasUIDescriptorAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationStartContext =>
            {
                var iContentDataType = compilationStartContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                var allowedTypesType = compilationStartContext.Compilation.GetTypeByMetadataName(TypeNames.AllowedTypesMetadataName);
                if (allowedTypesType is null)
                {
                    return;
                }

                var uiDescriptorType = compilationStartContext.Compilation.GetTypeByMetadataName(TypeNames.UIDescriptorMetadataName);
                if (uiDescriptorType is null)
                {
                    return;
                }

                CompilationAnalyzer analyzer = new CompilationAnalyzer(iContentDataType, allowedTypesType, uiDescriptorType);

                compilationStartContext.RegisterSymbolAction(analyzer.AnalyzeProperty, SymbolKind.Property);
                compilationStartContext.RegisterSymbolAction(analyzer.AnalyzeNamedType, SymbolKind.NamedType);
                compilationStartContext.RegisterCompilationEndAction(analyzer.CompilationEndAction);
            });
        }       

        //private void AnalyzePropertySymbol(
        //    SymbolAnalysisContext symbolContext,
        //    INamedTypeSymbol iContentDataType,
        //    INamedTypeSymbol allowedTypesType)
        //{
        //    var propertySymbol = (IPropertySymbol)symbolContext.Symbol;

        //    var containingType = propertySymbol.ContainingType;
        //    if (containingType is null)
        //    {
        //        return;
        //    }

        //    if (!iContentDataType.IsAssignableFrom(containingType))
        //    {
        //        return;
        //    }

        //    var attribute = propertySymbol.GetAttributes().FirstOrDefault(a => allowedTypesType.IsAssignableFrom(a.AttributeClass));
        //    if (attribute is null)
        //    {
        //        return;
        //    }

        //    if(attribute.ConstructorArguments != null)
        //    {
        //        foreach(var argument in attribute.ConstructorArguments)
        //        {
        //            VerifyTypeArrayArgument(argument, attribute);
        //        }
        //    }

        //    if(attribute.NamedArguments != null)
        //    {
        //        foreach(var pair in attribute.NamedArguments)
        //        {
        //            if(NamedArguments.Any(x => string.Equals(x, pair.Key, StringComparison.Ordinal)))
        //            {
        //                VerifyTypeArrayArgument(pair.Value, attribute);
        //            }
        //        }
        //    }
        //}

        private void AnalyzeTypeArrayArgument(TypedConstant argument, AttributeData attribute)
        {
            if (argument.Kind != TypedConstantKind.Array)
            {
                return;
            }

            if (argument.Values == null)
            {
                return;
            }

            foreach (TypedConstant value in argument.Values)
            {
                if (value.Kind != TypedConstantKind.Type)
                {
                    continue;
                }

                INamedTypeSymbol namedType = value.Value as INamedTypeSymbol;
                if (namedType == null)
                {
                    continue;
                }

                if (namedType.TypeKind != TypeKind.Interface)
                {
                    continue;
                }
            }
        }     

        private class CompilationAnalyzer
        {
            private readonly ImmutableArray<string> NamedArguments =
                ImmutableArray.Create("AllowedTypes", "RestrictedTypes");

            private readonly ConcurrentBag<Type> UIDescriptorTypes = new ConcurrentBag<Type>();

            private readonly INamedTypeSymbol iContentDataType;
            private readonly INamedTypeSymbol allowedTypesType;
            private readonly INamedTypeSymbol uiDescriptorType;

            public CompilationAnalyzer(INamedTypeSymbol iContentDataType, INamedTypeSymbol allowedTypesType, INamedTypeSymbol uiDescriptorType)
            {
                this.iContentDataType = iContentDataType;
                this.allowedTypesType = allowedTypesType;
                this.uiDescriptorType = uiDescriptorType;
            }

            internal void AnalyzeProperty(SymbolAnalysisContext symbolContext)
            {
                var propertySymbol = (IPropertySymbol)symbolContext.Symbol;

                var containingType = propertySymbol.ContainingType;
                if (containingType is null)
                {
                    return;
                }

                if (!iContentDataType.IsAssignableFrom(containingType))
                {
                    return;
                }

                var attribute = propertySymbol.GetAttributes().FirstOrDefault(a => allowedTypesType.IsAssignableFrom(a.AttributeClass));
                if (attribute is null)
                {
                    return;
                }

                if (attribute.ConstructorArguments != null)
                {
                    foreach (var argument in attribute.ConstructorArguments)
                    {
                        AnalyzeTypeArrayArgument(argument, attribute);
                    }
                }

                if (attribute.NamedArguments != null)
                {
                    foreach (var pair in attribute.NamedArguments)
                    {
                        if (NamedArguments.Any(x => string.Equals(x, pair.Key, StringComparison.Ordinal)))
                        {
                            AnalyzeTypeArrayArgument(pair.Value, attribute);
                        }
                    }
                }
            }

            private void AnalyzeTypeArrayArgument(TypedConstant argument, AttributeData attribute)
            {
                if (argument.Kind != TypedConstantKind.Array)
                {
                    return;
                }

                if (argument.Values == null)
                {
                    return;
                }

                foreach (TypedConstant value in argument.Values)
                {
                    if (value.Kind != TypedConstantKind.Type)
                    {
                        continue;
                    }

                    INamedTypeSymbol namedType = value.Value as INamedTypeSymbol;
                    if (namedType == null)
                    {
                        continue;
                    }

                    if (namedType.TypeKind != TypeKind.Interface)
                    {
                        continue;
                    }


                }
            }

            internal void AnalyzeNamedType(SymbolAnalysisContext symbolContext)
            {
                throw new NotImplementedException();
            }

            internal void CompilationEndAction(CompilationAnalysisContext compilationContext)
            {
                throw new NotImplementedException();
            }

            private void ReportMissingUIDescriptor(CompilationAnalysisContext compilationContext, AttributeData attribute)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    compilationContext.ReportDiagnostic(
                        node.CreateDiagnostic(Descriptors.Epi1008InterfaceInAllowedTypesShouldHaveUIDescriptor));
                }
            }
        }
    }
}
