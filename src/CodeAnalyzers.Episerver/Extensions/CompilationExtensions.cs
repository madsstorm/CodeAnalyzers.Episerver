using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzers.Episerver.Extensions
{
    internal static class CompilationExtensions
    {
        internal static bool IsAnalyzerSuppressed(this Compilation compilation, DiagnosticDescriptor descriptor)
        {
            var reportDiagnostic = compilation
                .Options
                .SpecificDiagnosticOptions
                .GetValueOrDefault(descriptor.Id);

            switch (reportDiagnostic)
            {
                case ReportDiagnostic.Default:
                    return !descriptor.IsEnabledByDefault;
                case ReportDiagnostic.Suppress:
                    return true;
                default:
                    return false;
            }
        }
    }
}
