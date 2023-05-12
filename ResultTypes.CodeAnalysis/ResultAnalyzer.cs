using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace ResultTypes.CodeAnalysis;

public enum ResultState
{
  Ambivalent,
  Error,
  Success,
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ResultAnalyzer : DiagnosticAnalyzer
{
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    ImmutableArray.Create(Rule, ImplicitConversionOfError, ImplicitConversionOfUnknown);
  
  public static readonly DiagnosticDescriptor Rule = new("IC", "TITLE---", "format", "Naming",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description.");
  
  public static readonly DiagnosticDescriptor ImplicitConversionOfError = new("IC001", "Implicit conversion of Error Result", 
    "implicit conversion will throw exception", "Ironclad.ResultTypes",
    DiagnosticSeverity.Error, isEnabledByDefault: true, description: "description");
  
  public static readonly DiagnosticDescriptor ImplicitConversionOfUnknown = new("IC002", "Implicit conversion of unknown Result", 
    "implicit conversion might throw exception", "Ironclad.ResultTypes",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description");

  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
      GeneratedCodeAnalysisFlags.ReportDiagnostics);

    context.RegisterOperationBlockAction(AnalyzeOperationBlocks);
  }

  private void AnalyzeOperationBlocks(OperationBlockAnalysisContext context)
  {
    foreach (var operationBlock in context.OperationBlocks)
    {
      Console.WriteLine("### NEW OPERATION BLOCK ###");
      var graph = context.GetControlFlowGraph(operationBlock);
      var transitionedStates =
        new Dictionary<BasicBlock /* to */, Dictionary<BasicBlock /* from */, Dictionary<ISymbol, ResultState>>>();
      var blocksToBeAnalyzed = new List<BasicBlock>(graph.Blocks);
      Console.WriteLine($"found {blocksToBeAnalyzed.Count} blocks to analyze");

      // iterate all blocks until you find one that has all predecessor block-state-transitions or the list is empty
      while (blocksToBeAnalyzed.Find(block => AllPredecessorStatesAreCalculated(block, transitionedStates)) is
               { } block &&
             blocksToBeAnalyzed.Remove(block))
      {
        Console.WriteLine($"analyzing block {block.Ordinal}");
        // get state from block-state-transition dictionary (combine multiple if necessary)
        IEnumerable<Dictionary<ISymbol, ResultState>> incomingStates =
          transitionedStates.TryGetValue(block, out var stateDict)
            ? stateDict.Values
            : Array.Empty<Dictionary<ISymbol, ResultState>>();
        var initialState = CombineStates(incomingStates);
        PrintState(initialState);

        foreach (var operation in block.Operations)
          AnalyzeOperation(operation, initialState, context);

        if (block.BranchValue is not null)
        {
          
        }

        // saving state(s) (condition-separated) to block-state-transition dictionary

        if (block is { FallThroughSuccessor.Destination: { } fallthrough })
        {
          if (!transitionedStates.ContainsKey(fallthrough))
            transitionedStates.Add(fallthrough, new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
          transitionedStates[fallthrough].Add(block,
            new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
        }
        
        if (block is { ConditionalSuccessor.Destination: { } conditional})
        {
          if (!transitionedStates.ContainsKey(conditional))
            transitionedStates.Add(conditional, new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
          transitionedStates[conditional].Add(block,
            new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
        }
      }
    }

    void PrintState(Dictionary<ISymbol, ResultState> state)
    {
      Console.Write("state:");
      if (state.Count is 0)
        Console.WriteLine(" empty");
      foreach (var singleState in state)
        Console.WriteLine($"  {singleState.Key}: {singleState.Value}");
    }

    // checks whether all predecessor states are 
    bool AllPredecessorStatesAreCalculated(BasicBlock block,
      Dictionary<BasicBlock, Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>> transitionedStates) =>
      block.Predecessors.Length == 0
      || transitionedStates.TryGetValue(block, out var incomingStates)
      && block
        .Predecessors
        .Select(branch => branch.Source)
        .All(predecessorBlock => incomingStates.ContainsKey(predecessorBlock));

    Dictionary<ISymbol, ResultState> CombineStates(IEnumerable<Dictionary<ISymbol, ResultState>> states) =>
      !states.Any()
        ? new Dictionary<ISymbol, ResultState>(SymbolEqualityComparer.Default)
        : states
          .Skip(1)
          .Aggregate(
            new HashSet<KeyValuePair<ISymbol, ResultState>>(states.First(),
              SymbolResultStatePairEqualityComparer.Default),
            (set, dict) =>
            {
              set.IntersectWith(dict);
              return set;
            }
          )
          .ToDictionary(pair => pair.Key, pair => pair.Value, SymbolEqualityComparer.Default);
  }

  private static void AnalyzeOperation(IOperation operation, IDictionary<ISymbol, ResultState> state,
    OperationBlockAnalysisContext context)
  {
    Console.WriteLine($"   analyze {operation.Kind}: {operation.Syntax}");
    // TODO: this should not need to be a field, parameter or local for the unknown state, only for error or success
    if (operation.IsImplicitResultToValueConversion(out var symbol))
    {
      var conversion = (IConversionOperation)operation;
      var location = conversion.Operand.Syntax.GetLocation();
      if (state.TryGetValue(symbol, out var resultState))
      {
        if (resultState is ResultState.Error)
        {
          context.ReportDiagnostic(Diagnostic.Create(ImplicitConversionOfError, location));
          return;
        }
      }
      else
      {
        context.ReportDiagnostic(Diagnostic.Create(ImplicitConversionOfUnknown, location));
        state.Add(symbol, ResultState.Success);
        return;
      }
    }

    foreach (var descendant in operation.Descendants())
      AnalyzeOperation(descendant, state, context);
  }

  private class SymbolResultStatePairEqualityComparer : EqualityComparer<KeyValuePair<ISymbol, ResultState>>
  {
    public new static SymbolResultStatePairEqualityComparer Default { get; } = new();

    public override bool Equals(KeyValuePair<ISymbol, ResultState> x, KeyValuePair<ISymbol, ResultState> y) =>
      x.Key.Equals(y.Key, SymbolEqualityComparer.Default) && x.Value.Equals(y.Value);

    public override int GetHashCode(KeyValuePair<ISymbol, ResultState> obj) =>
      new { symbolHash = SymbolEqualityComparer.Default.GetHashCode(obj.Key), obj.Value }.GetHashCode();
  }
}