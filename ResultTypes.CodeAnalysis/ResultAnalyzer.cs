using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ResultTypes.CodeAnalysis
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  internal sealed class ResultAnalyzer : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor("ID", "TITLE", "format", "Naming",
      DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description.");

    public override void Initialize(AnalysisContext context)
    {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
        GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
      ImmutableArray.Create(Rule);


    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
      // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
      var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

      // Find just those named type symbols with names containing lowercase letters.
      if (namedTypeSymbol.Name != "Result")
        return;
      
      // For all such symbols, produce a diagnostic.
      var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
      context.ReportDiagnostic(diagnostic);
    }
  }
}