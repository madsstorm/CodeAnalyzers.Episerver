using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzers.Episerver.Extensions
{
    internal static class DiagnosticExtensions
    {
        public static Diagnostic CreateDiagnostic(this SyntaxNode node, DiagnosticDescriptor rule, params object[] args)
        {
            return node.GetLocation().CreateDiagnostic(rule, args);
        }

        public static Diagnostic CreateDiagnostic(this Location location, DiagnosticDescriptor rule, params object[] args)
            => location.CreateDiagnostic(rule, ImmutableDictionary<string, string>.Empty, args);

        public static Diagnostic CreateDiagnostic(this Location location, DiagnosticDescriptor rule,
            ImmutableDictionary<string, string> properties, params object[] args)
        {
            var inSourceLocation = location.IsInSource ? location : null;

            return Diagnostic.Create(rule, inSourceLocation, properties, args);
        }
    }
}
