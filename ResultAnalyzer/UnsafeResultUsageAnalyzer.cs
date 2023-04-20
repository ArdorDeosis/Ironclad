using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ResultAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnsafeResultUsageAnalyzer : DiagnosticAnalyzer
{
  public override void Initialize(AnalysisContext context)
  {
    // context.EnableConcurrentExecution();
    // context.ConfigureGeneratedCodeAnalysis(
    //   GeneratedCodeAnalysisFlags.Analyze |
    //   GeneratedCodeAnalysisFlags.ReportDiagnostics);
    context.RegisterSyntaxTreeAction(context =>
    {
      context.ReportDiagnostic(Diagnostic.Create(Rule, Location.None));
    });
  }
  
  private static readonly DiagnosticDescriptor Rule = new(
    "ResultAnalyzer",
    "Unsafe Result Usage",
    "Unsafe Result Usage",
    "Ironclad.Analyzers",
    DiagnosticSeverity.Warning,
    true);

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => new() { Rule };
}