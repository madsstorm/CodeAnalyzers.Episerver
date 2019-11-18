using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using CodeAnalyzers.Episerver.Extensions;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BannedApiAnalyzer : DiagnosticAnalyzer
    {
        private const string InternalNamespace = "Internal";

        private readonly ImmutableArray<string> InternalRootNamespaces =
            ImmutableArray.Create("EPiServer", "Mediachase");

        private readonly ImmutableArray<(DiagnosticDescriptor Descriptor, ImmutableArray<string> TypeNames)> BannedTypes =
            ImmutableArray.Create(
                (Descriptors.Epi1007AvoidUsingConcreteOrderClasses,
                    ImmutableArray.Create(
                        "Mediachase.Commerce.Orders.OrderGroup",
                        "Mediachase.Commerce.Orders.Cart",
                        "Mediachase.Commerce.Orders.LineItem",
                        "Mediachase.Commerce.Orders.OrderAddress",
                        "Mediachase.Commerce.Orders.OrderForm",
                        "Mediachase.Commerce.Orders.OrderNote",
                        "Mediachase.Commerce.Orders.Payment",
                        "Mediachase.Commerce.Orders.PaymentPlan",
                        "Mediachase.Commerce.Orders.PurchaseOrder",
                        "Mediachase.Commerce.Orders.Shipment",
                        "Mediachase.Commerce.Orders.TaxValue")),
                (Descriptors.Epi3000AvoidUsingDataFactory, ImmutableArray.Create("EPiServer.DataFactory")),
                (Descriptors.Epi3001AvoidUsingCacheManager, ImmutableArray.Create("EPiServer.CacheManager")),
                (Descriptors.Epi3002AvoidUsingLog4NetLogManager, ImmutableArray.Create("log4net.LogManager")),
                (Descriptors.Epi3003AvoidUsingOrderContext, ImmutableArray.Create("Mediachase.Commerce.Orders.OrderContext")));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi1002AvoidUsingInternalNamespaces,
                Descriptors.Epi1007AvoidUsingConcreteOrderClasses,
                Descriptors.Epi3000AvoidUsingDataFactory,
                Descriptors.Epi3001AvoidUsingCacheManager,
                Descriptors.Epi3002AvoidUsingLog4NetLogManager,
                Descriptors.Epi3003AvoidUsingOrderContext);

        public override void Initialize(AnalysisContext analysisContext)
        {
            if (analysisContext is null) { return; }

            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            analysisContext.RegisterOperationAction(OperationAction,
                OperationKind.ObjectCreation,
                OperationKind.Invocation,
                OperationKind.EventReference,
                OperationKind.FieldReference,
                OperationKind.MethodReference,
                OperationKind.PropertyReference,
                OperationKind.ArrayCreation);

            analysisContext.RegisterSymbolAction(
                symbolContext => VerifyAttributes(symbolContext.ReportDiagnostic, symbolContext.Symbol.GetAttributes()),
                SymbolKind.NamedType,
                SymbolKind.Method,
                SymbolKind.Field,
                SymbolKind.Property,
                SymbolKind.Event);
        }

        private void OperationAction(OperationAnalysisContext operationContext)
        {
            switch (operationContext.Operation)
            {
                case IObjectCreationOperation objectCreation:
                    VerifyType(operationContext.ReportDiagnostic, objectCreation.Type, operationContext.Operation.Syntax);
                    break;

                case IInvocationOperation invocation:
                    VerifyType(operationContext.ReportDiagnostic, invocation.Instance?.Type ?? invocation.TargetMethod.ContainingType, operationContext.Operation.Syntax);
                    break;

                case IMemberReferenceOperation memberReference:
                    VerifyType(operationContext.ReportDiagnostic, memberReference.Instance?.Type ?? memberReference.Member.ContainingType, operationContext.Operation.Syntax);
                    break;

                case IArrayCreationOperation arrayCreation:
                    VerifyType(operationContext.ReportDiagnostic, arrayCreation.Type, operationContext.Operation.Syntax);
                    break;
            }
        }

        private bool VerifyType(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode)
        {
            do
            {
                if (!VerifyTypeArguments(reportDiagnostic, type, syntaxNode, out type))
                {
                    return false;
                }
                if (type == null)
                {
                    // Type will be null for arrays and pointers.
                    return true;
                }

                if(!VerifyTypeBanned(reportDiagnostic, type, syntaxNode))
                {
                    return false;
                }

                if(!VerifyTypeInternal(reportDiagnostic, type, syntaxNode))
                {
                    return false;
                }

                type = type.ContainingType;
            }
            while (!(type is null));

            return true;
        }

        private bool VerifyTypeBanned(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode)
        {
            string typeName = type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

            foreach (var entry in BannedTypes)
            {
                foreach (string bannedType in entry.TypeNames)
                {
                    if (string.Equals(typeName, bannedType, StringComparison.Ordinal))
                    {
                        reportDiagnostic(
                            Diagnostic.Create(
                                entry.Descriptor,
                                syntaxNode?.GetLocation(),
                                typeName));

                        return false;
                    }
                }
            }

            return true;
        }

        private bool VerifyTypeInternal(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode)
        {
            var space = type.ContainingNamespace;

            if (IsInternal(space))
            {
                reportDiagnostic(
                    Diagnostic.Create(
                        Descriptors.Epi1002AvoidUsingInternalNamespaces,
                        syntaxNode?.GetLocation(),
                        type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));

                return false;
            }

            return true;
        }

        private bool VerifyTypeArguments(Action<Diagnostic> reportDiagnostic, ITypeSymbol type, SyntaxNode syntaxNode, out ITypeSymbol originalDefinition)
        {
            switch (type)
            {
                case INamedTypeSymbol namedTypeSymbol:
                    originalDefinition = namedTypeSymbol.ConstructedFrom;
                    foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                    {
                        if (typeArgument.TypeKind != TypeKind.TypeParameter &&
                            typeArgument.TypeKind != TypeKind.Error &&
                            !VerifyType(reportDiagnostic, typeArgument, syntaxNode))
                        {
                            return false;
                        }
                    }
                    break;

                case IArrayTypeSymbol arrayTypeSymbol:
                    originalDefinition = null;
                    return VerifyType(reportDiagnostic, arrayTypeSymbol.ElementType, syntaxNode);

                default:
                    originalDefinition = type.OriginalDefinition;
                    break;
            }

            return true;
        }

        private void VerifyAttributes(Action<Diagnostic> reportDiagnostic, ImmutableArray<AttributeData> attributes)
        {
            foreach (var attribute in attributes)
            {
                var space = attribute.AttributeClass.ContainingNamespace;

                if (IsInternal(space))
                {
                    var node = attribute.ApplicationSyntaxReference?.GetSyntax();
                    if (node != null)
                    {
                        reportDiagnostic(
                            node.CreateDiagnostic(
                                Descriptors.Epi1002AvoidUsingInternalNamespaces,
                                attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
                    }
                }
            }
        }

        private bool IsInternal(INamespaceSymbol space)
        {
            if(space is null)
            {
                return false;
            }

            if(!string.Equals(space.MetadataName, InternalNamespace, StringComparison.Ordinal))
            {
                return false; ;
            }

            while(!space.IsGlobalNamespace)
            {
                space = space.ContainingNamespace;
                
                if(InternalRootNamespaces.Any(root => string.Equals(space.MetadataName, root, StringComparison.Ordinal)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}