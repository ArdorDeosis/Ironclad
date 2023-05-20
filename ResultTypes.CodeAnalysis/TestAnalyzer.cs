using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;


[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class TestAnalyzer : DiagnosticAnalyzer
{
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    ImmutableArray.Create(Rule);
  
  public static readonly DiagnosticDescriptor Rule = new("IC", "TITLE---", "FOUND IT, THIS SHOULD HAVE SQUIGGLY LINES", "Naming",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description.");
  
  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
      GeneratedCodeAnalysisFlags.ReportDiagnostics);

    // context.RegisterOperationAction(DoStuff, OperationKind.LocalReference);
    // context.RegisterOperationBlockAction(DoOtherStuff);
  }

  private void DoOtherStuff(OperationBlockAnalysisContext context)
  {
    var operations = context.OperationBlocks
      .Select(context.GetControlFlowGraph)
      .SelectMany(graph => graph.Blocks)
      .SelectMany(basicBlock => basicBlock.Operations)
      .SelectMany(operation => operation.DescendantsAndSelf())
      .OfType<ILocalReferenceOperation>();
    foreach (var operation in operations)
      context.ReportDiagnostic(Diagnostic.Create(Rule, operation.Syntax.GetLocation()));
  }

  private void DoStuff(OperationAnalysisContext context)
  {
      context.ReportDiagnostic(Diagnostic.Create(Rule, context.Operation.Syntax.GetLocation()));
  }
}