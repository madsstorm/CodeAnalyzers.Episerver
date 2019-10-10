using Microsoft.CodeAnalysis;
using System.Collections.Concurrent;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static CodeAnalyzers.Episerver.Category;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CodeAnalyzers.Episerver.Test")]
namespace CodeAnalyzers.Episerver
{
    internal enum Category
    {
        Usage
    }

    internal static class Descriptors
    {
        static ConcurrentDictionary<Category, string> categoryMapping = new ConcurrentDictionary<Category, string>();

        static DiagnosticDescriptor Rule(string id, string title, Category category, DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {
            var isEnabledByDefault = true;
            return new DiagnosticDescriptor(id, title, messageFormat, categoryMapping.GetOrAdd(category, c => c.ToString()), defaultSeverity, isEnabledByDefault, description);
        }

        internal static DiagnosticDescriptor EPI1000_AvoidUsingDataFactory { get; } =
            Rule("Epi1000", "Avoid using DataFactory", Usage, Warning,
                "Avoid using {0}");
    }
}
