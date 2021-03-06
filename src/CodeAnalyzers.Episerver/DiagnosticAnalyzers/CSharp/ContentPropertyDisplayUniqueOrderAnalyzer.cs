﻿using System;
using System.Collections.Immutable;
using System.Linq;
using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentPropertyDisplayUniqueOrderAnalyzer : DiagnosticAnalyzer
    {
        private const string OrderArgument = "Order";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
           ImmutableArray.Create(Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.IContentDataMetadataName);
                if (iContentDataType is null)
                {
                    return;
                }

                var displayAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeNames.DisplayMetadataName);
                if (displayAttribute is null)
                {
                    return;
                }

                compilationContext.RegisterSymbolAction(
                    symbolContext => AnalyzeSymbol(symbolContext, iContentDataType, displayAttribute)
                    , SymbolKind.NamedType);
            });
        }

        private void AnalyzeSymbol(
            SymbolAnalysisContext symbolContext,
            INamedTypeSymbol iContentDataType,
            INamedTypeSymbol displayAttribute)
        {
            var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;
            if(!iContentDataType.IsAssignableFrom(namedTypeSymbol))
            {
                return;
            }

            var propertySymbols = namedTypeSymbol.GetMembers()
                .Where(m => m.Kind == SymbolKind.Property).OfType<IPropertySymbol>();

            var propertyDisplayAttributes = propertySymbols
                .Select(property => TryGetAttribute(property, displayAttribute, out var attribute) ? (property, attribute) : default)
                .Where(x => x != default);

            var propertyAttributeOrders = propertyDisplayAttributes
                .Select(tuple => TryGetOrder(tuple.attribute, out int order) ? (tuple.property, tuple.attribute, order) : default)
                .Where(x => x != default);

            var propertyAttributeOrderGroups = propertyAttributeOrders.GroupBy(p => p.order);

            var duplicatePropertyAttributeOrderGroups = propertyAttributeOrderGroups.Where(x => x.Count() > 1);

            foreach(var group in duplicatePropertyAttributeOrderGroups)
            {
                var propertyAttributeList = group.Select((tuple, index) => (tuple.property, tuple.attribute, index)).ToList();

                foreach (var (property, attribute, index) in propertyAttributeList.Skip(1))
                {
                    var previous = propertyAttributeList[index - 1];
                    ReportDuplicateOrder(symbolContext, previous.property, previous.attribute, property);
                }
            }
        }

        private static bool TryGetOrder(AttributeData attribute, out int order)
        {
            if(attribute is null)
            {
                order = 0;
                return false;
            }

            var argument = attribute.NamedArguments.FirstOrDefault(arg => string.Equals(arg.Key, OrderArgument, StringComparison.Ordinal));

            return int.TryParse(argument.Value.Value?.ToString(), out order);
        }

        private static bool TryGetAttribute(IPropertySymbol property, INamedTypeSymbol displayAttribute, out AttributeData attribute)
        {
            if(property is null)
            {
                attribute = null;
                return false;
            }

            attribute = property.GetAttributes().FirstOrDefault(a => displayAttribute.IsAssignableFrom(a.AttributeClass));
            return (attribute != null);
        }

        private static void ReportDuplicateOrder(SymbolAnalysisContext symbolContext, IPropertySymbol property, AttributeData attribute, IPropertySymbol duplicateProperty)
        {
            var node = attribute.ApplicationSyntaxReference?.GetSyntax();
            if (node != null)
            {
                symbolContext.ReportDiagnostic(
                    node.CreateDiagnostic(
                        Descriptors.Epi2010ContentPropertyShouldHaveUniqueOrder,
                        property.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                        duplicateProperty.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }
    }
}
