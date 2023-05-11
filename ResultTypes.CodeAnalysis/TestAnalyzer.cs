using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace ResultTypes.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TestAnalyzer : DiagnosticAnalyzer
{
  private static readonly DiagnosticDescriptor Rule = new(
    "TEST",
    "a test diagnostic",
    "this is a test diagnostic, maybe with some data {0}, {1}, {2}",
    "Analyzer Tests",
    DiagnosticSeverity.Warning,
    true,
    "description.");

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    ImmutableArray.Create(Rule);

  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
    context.RegisterOperationBlockAction(Diagnose);
  }

  private void Diagnose(OperationBlockAnalysisContext context)
  {
    foreach (var operationBlock in context.OperationBlocks)
    {
      Console.WriteLine($"=== NEW OPERATION BLOCK ===");
      var graph = context.GetControlFlowGraph(operationBlock);
      foreach (var block in graph.Blocks)
      {
        Console.WriteLine($"block {block.Ordinal}");
        foreach (var operation in block.Operations.Where(operation => operation is not null))
        {
          Console.WriteLine($"{operation.GetType().Name}: {operation.Syntax}");
        }

        if (block.ConditionKind is not ControlFlowConditionKind.None)
        {
          Console.WriteLine($"{operationBlock.GetType().Name}: {block.ConditionKind} ({block.BranchValue!.Syntax}) {block.ConditionalSuccessor!.Destination?.Ordinal} else {block.FallThroughSuccessor?.Destination?.Ordinal}");
        }
      }
    }

    
  }
}