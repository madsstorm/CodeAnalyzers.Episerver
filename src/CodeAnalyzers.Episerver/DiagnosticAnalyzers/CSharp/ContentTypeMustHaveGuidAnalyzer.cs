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
            ImmutableArray.Create(Descriptors.CAE1002_ContentTypeMustHaveGuid);

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
            var attribute = syntaxContext.Node as AttributeSyntax;
            if (attribute is null)
            {
                return;
            }

            var attributeType = syntaxContext.SemanticModel?.GetTypeInfo(attribute).Type;
            if(attributeType is null)
            {
                return;
            }          
            
            if(!attributeType.InheritsOrIs(contentTypeAttribute))
            {
                return;
            }

            var classDeclaration = attribute.Parent?.Parent as ClassDeclarationSyntax;
            if(classDeclaration is null)
            {
                return;
            }           

            var argumentList = attribute.ArgumentList;
            if (argumentList is null || !argumentList.Arguments.Any())
            {
                var classType = syntaxContext.SemanticModel?.GetDeclaredSymbol(classDeclaration);

                syntaxContext.ReportDiagnostic(
                        Diagnostic.Create(
                            Descriptors.CAE1002_ContentTypeMustHaveGuid,
                            attribute?.GetLocation(),
                            classType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
            }
        }
    }
}
