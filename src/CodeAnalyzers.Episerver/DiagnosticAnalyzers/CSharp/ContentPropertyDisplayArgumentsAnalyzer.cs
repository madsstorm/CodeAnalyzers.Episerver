using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ContentPropertyDisplayArgumentsAnalyzer : DiagnosticAnalyzer
    {
        private readonly ImmutableArray<(string ArgumentName, DiagnosticDescriptor Descriptor)> ContentPropertyArguments =
            ImmutableArray.Create(
                ("Name", Descriptors.Epi2006ContentPropertyShouldHaveName),
                ("Description", Descriptors.Epi2007ContentPropertyShouldHaveDescription),
                ("GroupName", Descriptors.Epi2008ContentPropertyShouldHaveGroupName),
                ("Order", Descriptors.Epi2009ContentPropertyShouldHaveOrder));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(
                Descriptors.Epi2006ContentPropertyShouldHaveName,
                Descriptors.Epi2007ContentPropertyShouldHaveDescription,
                Descriptors.Epi2008ContentPropertyShouldHaveGroupName,
                Descriptors.Epi2009ContentPropertyShouldHaveOrder);

        public override void Initialize(AnalysisContext context)
        {
        }
    }
}
