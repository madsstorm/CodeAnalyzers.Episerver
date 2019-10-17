using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentTypeMustHaveGuidAnalyzer : DiagnosticAnalyzer
    {
        private const string TypeMetadataName = "EPiServer.DataAnnotations.ContentTypeAttribute";
        private const string GuidArgument = "GUID";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Descriptors.Epi2000ContentTypeMustHaveGuid);

        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                return;
            }

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var contentTypeAttribute = compilationContext.Compilation.GetTypeByMetadataName(TypeMetadataName);
                if (contentTypeAttribute == null)
                {
                    return;
                }

                compilationContext.RegisterSyntaxNodeAction(syntaxContext =>
                {
                    AnalyzeAttributeNode(syntaxContext, contentTypeAttribute);
                }, SyntaxKind.Attribute);
            });

        }

        private void AnalyzeAttributeNode(SyntaxNodeAnalysisContext syntaxContext, INamedTypeSymbol contentTypeAttribute)
        {
            if (!(syntaxContext.Node is AttributeSyntax attribute))
            {
                return;
            }

            if(!(syntaxContext.SemanticModel?.GetTypeInfo(attribute).Type is ITypeSymbol attributeType))
            {
                return;
            }
           
            if(!attributeType.InheritsOrIs(contentTypeAttribute))
            {
                return;
            }

            if (!(attribute.Parent?.Parent is ClassDeclarationSyntax classDeclaration))
            {
                return;
            }

            var argumentList = attribute.ArgumentList;
            if (argumentList is null || !argumentList.Arguments.Any())
            {
                var classType = syntaxContext.SemanticModel?.GetDeclaredSymbol(classDeclaration);

                syntaxContext.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.Epi2000ContentTypeMustHaveGuid,
                            attribute?.GetLocation(),
                            classType?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }
        }
    }
}
