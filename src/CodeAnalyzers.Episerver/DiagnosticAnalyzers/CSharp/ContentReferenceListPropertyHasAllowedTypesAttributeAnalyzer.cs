﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentReferenceListPropertyHasAllowedTypesAttributeAnalyzer : DiagnosticAnalyzer
    {
        private readonly ImmutableArray<string> RootPropertyTypeNames = ImmutableArray.Create("System.Collections.IEnumerable");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var ignoreAttributeType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IgnoreAttributeMetadataName);

                var rootPropertyTypes = RootPropertyTypeNames.Select(r => compilationContext.Compilation.GetTypeByMetadataName(r)).Where(s => s != null);

                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                var allowedTypesType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.AllowedTypesMetadataName);
                if (allowedTypesType is null)
                {
                    return;
                }

                var genericArgumentType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.ContentReferenceMetadataName);

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, iContentDataType, allowedTypesType, rootPropertyTypes, genericArgumentType, ignoreAttributeType)
                    , SymbolKind.Property);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol iContentDataType,
            INamedTypeSymbol allowedTypesType,
            IEnumerable<INamedTypeSymbol> rootPropertyTypes,
            INamedTypeSymbol genericArgumentType,
            INamedTypeSymbol ignoreAttribute)
        {
            var propertySymbol = (IPropertySymbol)symbolContext.Symbol;

            if (!propertySymbol.IsModelProperty(ignoreAttribute))
            {
                return;
            }

            var containingType = propertySymbol.ContainingType;
            if (containingType is null)
            {
                return;
            }

            if (containingType.TypeKind == TypeKind.Interface)
            {
                return;
            }

            if (!iContentDataType.IsAssignableFrom(containingType))
            {
                return;
            }

            var propertyNamedType = propertySymbol.Type as INamedTypeSymbol;
            if(propertyNamedType is null)
            {
                return;
            }

            if(!propertyNamedType.IsGenericType)
            {
                return;
            }

            if (!rootPropertyTypes.Any(r => r.IsAssignableFrom(propertyNamedType.ConstructedFrom)))
            {
                return;
            }


            var typeArgument = propertyNamedType.TypeArguments.FirstOrDefault();
            if(typeArgument is null)
            {
                return;
            }

            if(!genericArgumentType.IsAssignableFrom(typeArgument))
            {
                return;
            }

            var attribute = propertySymbol.GetAttributes().FirstOrDefault(a => allowedTypesType.IsAssignableFrom(a.AttributeClass));
            if (attribute is null)
            {
                ReportMissingAllowedTypes(symbolContext, propertySymbol);
            }
        }

        private void ReportMissingAllowedTypes(SymbolAnalysisContext symbolContext, IPropertySymbol property)
        {
            symbolContext.ReportDiagnostic(
                property.CreateDiagnostic(
                    Descriptors.Epi2016ContentReferenceListPropertyShouldHaveAllowedTypesAttribute));
        }
    }
}
