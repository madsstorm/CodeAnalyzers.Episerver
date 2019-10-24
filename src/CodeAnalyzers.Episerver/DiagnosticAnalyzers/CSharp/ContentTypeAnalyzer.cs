using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeAnalyzer : DiagnosticAnalyzer
    {
        private const string ContentTypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string IContentDataMetadataName = "EPiServer.Core.IContentData";
        private const string ImageUrlMetadataName = "EPiServer.DataAnnotations.ImageUrlAttribute";
        private const string GuidArgument = "GUID";
        private const string DescriptionArgument = "Description";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi1000ContentTypeMustHaveValidGuid,
                Descriptors.Epi1001ContentTypeMustHaveUniqueGuid,
                Descriptors.Epi1003ContentTypeMustImplementContentData,
                Descriptors.Epi2001ContentTypeShouldHaveDescription,
                Descriptors.Epi2005ContentTypeShouldHaveImageUrl);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null) { return; }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var contentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(ContentTypeMetadataName);
                if (contentTypeAttribute is null)
                {
                    return;
                }

                var iContentDataType = compilationContext.Compilation.GetTypeByMetadataName(IContentDataMetadataName);
                if(iContentDataType is null)
                {
                    return;
                }

                var imageUrlType = compilationContext.Compilation.GetTypeByMetadataName(ImageUrlMetadataName);
                if(imageUrlType is null)
                {
                    return;
                }

                CompilationAnalyzer analyzer = new CompilationAnalyzer(contentTypeAttribute, iContentDataType, imageUrlType);

                compilationContext.RegisterSymbolAction(analyzer.AnalyzeSymbol, SymbolKind.NamedType);
            });
        }

        private class CompilationAnalyzer
        {
            private readonly INamedTypeSymbol contentTypeAttribute;
            private readonly INamedTypeSymbol iContentDataType;
            private readonly INamedTypeSymbol imageUrlType;

            private readonly ConcurrentDictionary<Guid, (INamedTypeSymbol Type, AttributeData Attribute)> contentTypeGuids =
                new ConcurrentDictionary<Guid, (INamedTypeSymbol Type, AttributeData Attribute)>();

            public CompilationAnalyzer(INamedTypeSymbol contentTypeAttribute, INamedTypeSymbol iContentDataType, INamedTypeSymbol imageUrlType)
            {
                this.contentTypeAttribute = contentTypeAttribute;
                this.iContentDataType = iContentDataType;
                this.imageUrlType = imageUrlType;
            }

            internal void AnalyzeSymbol(SymbolAnalysisContext symbolContext)
            {
                var namedTypeSymbol = (INamedTypeSymbol)symbolContext.Symbol;               
                var attributes = namedTypeSymbol.GetAttributes();

                var contentAttribute = attributes.FirstOrDefault(attr => contentTypeAttribute.IsAssignableFrom(attr.AttributeClass));
                if(contentAttribute is null)
                {
                    return;
                }

                var imageUrlAttribute = attributes.FirstOrDefault(attr => imageUrlType.IsAssignableFrom(attr.AttributeClass));
                if(imageUrlAttribute is null)
                {
                    ReportInvalidImageUrl(symbolContext, namedTypeSymbol, contentAttribute);
                }
                else
                {
                    VerifyImageUrl(symbolContext, namedTypeSymbol, imageUrlAttribute);
                }

                VerifyContentTypeGuid(symbolContext, namedTypeSymbol, contentAttribute);
                VerifyContentTypeDescription(symbolContext, namedTypeSymbol, contentAttribute);

                VerifyContentDataType(symbolContext, namedTypeSymbol);
            }

            private void VerifyImageUrl(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData imageUrlAttribute)
            {
                if (!Equals(imageUrlAttribute.AttributeClass, imageUrlAttribute))
                {
                    if (imageUrlAttribute.ConstructorArguments.IsEmpty)
                    {
                        // For simplicity, assume that a derived attribute
                        // with a parameterless constructor sets an image path
                        return;
                    }
                }

                if (string.IsNullOrEmpty(imageUrlAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString()))
                {
                    ReportInvalidImageUrl(symbolContext, namedTypeSymbol, imageUrlAttribute);
                }
            }

            private void VerifyContentTypeGuid(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData attribute)
            {
                if (TryGetGuidFromAttribute(attribute, out Guid contentGuid))
                {
                    var (existingType, existingAttribute) = contentTypeGuids.GetOrAdd(contentGuid, (namedTypeSymbol, attribute));

                    if(existingType != namedTypeSymbol)
                    {
                        ReportDuplicateGuid(symbolContext, namedTypeSymbol, attribute, existingType);
                        ReportDuplicateGuid(symbolContext, existingType, existingAttribute, namedTypeSymbol);
                    }
                }
                else
                {
                    ReportInvalidGuid(symbolContext, namedTypeSymbol, attribute);
                }
            }

            private void VerifyContentTypeDescription(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol, AttributeData contentAttribute)
            {
                var descriptionArgument = contentAttribute.NamedArguments
                    .FirstOrDefault(arg => string.Equals(arg.Key, DescriptionArgument, StringComparison.Ordinal));

                string description = descriptionArgument.Value.Value?.ToString();

                if(string.IsNullOrEmpty(description))
                {
                    ReportInvalidDescription(symbolContext, namedTypeSymbol, contentAttribute);
                }
            }

            private void VerifyContentDataType(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedTypeSymbol)
            {
                if(namedTypeSymbol.IsAbstract || !(iContentDataType.IsAssignableFrom(namedTypeSymbol)))
                {
                    ReportInvalidContentDataType(symbolContext, namedTypeSymbol);
                }
            }

            private static bool TryGetGuidFromAttribute(AttributeData attribute, out Guid guid)
            {
                TypedConstant guidValue = default;

                foreach (var namedArgument in attribute.NamedArguments)
                {
                    if (string.Equals(namedArgument.Key, GuidArgument, StringComparison.Ordinal))
                    {
                        guidValue = namedArgument.Value;
                        break;
                    }
                }

                return Guid.TryParse(guidValue.Value?.ToString(), out guid);
            }

            private static void ReportDuplicateGuid(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType,
                AttributeData attribute, INamedTypeSymbol matchingType)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi1001ContentTypeMustHaveUniqueGuid,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat),
                            matchingType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }

            private static void ReportInvalidGuid(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi1000ContentTypeMustHaveValidGuid,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }

            private void ReportInvalidContentDataType(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType)
            {
                symbolContext.ReportDiagnostic(
                    namedType.CreateDiagnostic(
                        Descriptors.Epi1003ContentTypeMustImplementContentData,
                        namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }

            private void ReportInvalidImageUrl(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi2005ContentTypeShouldHaveImageUrl,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }

            private void ReportInvalidDescription(SymbolAnalysisContext symbolContext, INamedTypeSymbol namedType, AttributeData attribute)
            {
                var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (node != null)
                {
                    symbolContext.ReportDiagnostic(
                        node.CreateDiagnostic(
                            Descriptors.Epi2001ContentTypeShouldHaveDescription,
                            namedType.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
                }
            }
        }
    }
}
