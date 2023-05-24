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
    // context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
      GeneratedCodeAnalysisFlags.ReportDiagnostics);

    context.RegisterOperationBlockAction(AnalyzeOperationBlocks);
  }
  private void AnalyzeOperationBlocks(OperationBlockAnalysisContext context)
  {
    foreach (var operationBlock in context.OperationBlocks)
      AnalyzeOperationBlock(operationBlock, context);
  }

  private void AnalyzeOperationBlock(IOperation operationBlock, OperationBlockAnalysisContext context)
  {
    var graph = context.GetControlFlowGraph(operationBlock);
    var entryBlocks = graph.Blocks.Where(block => block.Predecessors.Length is 0);
    Console.WriteLine($"entry blocks: {string.Join(", ", entryBlocks.Select(block => block.Ordinal))}");

    foreach (var entryBlock in entryBlocks) 
      Traverse(entryBlock);
  }

  private void Traverse(BasicBlock entry)
  {
    var visited = new HashSet<BasicBlock>();
    var queue = new Queue<BasicBlock>();
    queue.Enqueue(entry);

    while (queue.Count > 0)
    {
      var block = queue.Dequeue();
      visited.Add(block);
      Console.WriteLine($"visiting {block.Ordinal}");
      foreach (var operation in block.Operations) 
        Console.WriteLine($">  {operation.Syntax}");
      if (block.BranchValue is not null)
        Console.WriteLine($">  branch on: {block.BranchValue.Syntax}");

      if (block.ConditionalSuccessor is { Destination: { } conditional })
      {
        Console.WriteLine($"conditional: {conditional.Ordinal}");
        if (!visited.Contains(conditional))
          queue.Enqueue(conditional);
      }
      if (block.FallThroughSuccessor is { Destination: { } fallthrough })
      {
        Console.WriteLine($"fallthrough: {fallthrough.Ordinal}");
        if (!visited.Contains(fallthrough))
          queue.Enqueue(fallthrough);
      }
    }
  }
}