using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        public static Diagnostic CreateDiagnostic(this ISymbol symbol, DiagnosticDescriptor rule, params object[] args)
        {
            return symbol.Locations.CreateDiagnostic(rule, args);
        }

        public static Diagnostic CreateDiagnostic(this IEnumerable<Location> locations, DiagnosticDescriptor rule, params object[] args)
        {
            return locations.CreateDiagnostic(rule, null, args);
        }

        public static Diagnostic CreateDiagnostic(this IEnumerable<Location> locations, DiagnosticDescriptor rule,
            ImmutableDictionary<string, string> properties, params object[] args)
        {
            IEnumerable<Location> inSource = locations.Where(l => l.IsInSource);
            if (!inSource.Any())
            {
                return Diagnostic.Create(rule, null, args);
            }

            return Diagnostic.Create(rule,
                     location: inSource.First(),
                     additionalLocations: inSource.Skip(1),
                     properties: properties,
                     messageArgs: args);
        }
    }
}
