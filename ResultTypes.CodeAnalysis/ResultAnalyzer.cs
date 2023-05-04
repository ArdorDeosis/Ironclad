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
  private static readonly DiagnosticDescriptor Rule = new("IC", "TITLE---", "format", "Naming",
    DiagnosticSeverity.Warning, isEnabledByDefault: true, description: "description.");

  private static DiagnosticDescriptor GetDiagnostic(string message) =>
    new("IC", message, message, "Naming",
      DiagnosticSeverity.Warning, true);

  public override void Initialize(AnalysisContext context)
  {
    context.EnableConcurrentExecution();
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
      GeneratedCodeAnalysisFlags.ReportDiagnostics);

    // context.RegisterOperationAction(MarkImplicitConversion, OperationKind.Conversion);
    // context.RegisterOperationAction(MarkInvocations, OperationKind.Invocation);
    // context.RegisterOperationAction(MarkProperties, OperationKind.PropertyReference);

    context.RegisterOperationBlockAction(CheckOperation);
  }


  private class BlockTransitionState
  {
    public BasicBlock From { get; init; }

    public BasicBlock To { get; init; }

    // IOperation here is either ILocalSymbol or IFieldSymbol
    public Dictionary<IOperation, ResultState> KnownStates { get; init; }
  }

  private void AnalyzeOperationBlocks(OperationBlockAnalysisContext context)
  {
    foreach (var operationBlock in context.OperationBlocks)
    {
      var graph = context.GetControlFlowGraph(operationBlock);
      var transitionedStates =
        new Dictionary<BasicBlock /* to */, Dictionary<BasicBlock /* from */, Dictionary<ISymbol, ResultState>>>();
      var blocksToBeAnalyzed = new List<BasicBlock>(graph.Blocks);

      // iterate all blocks until you find one that has all predecessor block-state-transitions or the list is empty
      while (blocksToBeAnalyzed.Find(block => AllPredecessorStatesAreCalculated(block, transitionedStates)) is
               { } block &&
             blocksToBeAnalyzed.Remove(block))
      {
        // get state from block-state-transition dictionary (combine multiple if necessary)
        IEnumerable<Dictionary<ISymbol, ResultState>> incomingStates =
          transitionedStates.TryGetValue(block, out var stateDict)
            ? stateDict.Values
            : Array.Empty<Dictionary<ISymbol, ResultState>>();
        var initialState = CombineStates(incomingStates);

        // TODO: analyze operations while updating state
        foreach (var operation in block.Operations)
        {
          // check if operation triggers warning
          // check if operation changes state
        }

        if (block.BranchValue is not null)
        {
          
        }

        // saving state(s) (condition-separated) to block-state-transition dictionary

        // not branching
        if (block is { ConditionKind: ControlFlowConditionKind.None, FallThroughSuccessor.Source: { } successor })
        {
          if (!transitionedStates.ContainsKey(successor))
            transitionedStates.Add(successor, new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
          transitionedStates[successor].Add(block,
            new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
        }
        // branching but with the same state (should be combined with the above case)
        else if (block.BranchValue is null /* or it does not change the state */)
        {
          if (block.FallThroughSuccessor is { Source: { } fallThroughSuccessor })
          {
            if (!transitionedStates.ContainsKey(fallThroughSuccessor))
              transitionedStates.Add(fallThroughSuccessor,
                new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
            transitionedStates[fallThroughSuccessor].Add(block,
              new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
          }

          if (block.ConditionalSuccessor is { Source: { } conditionalSuccessor })
          {
            if (!transitionedStates.ContainsKey(conditionalSuccessor))
              transitionedStates.Add(conditionalSuccessor,
                new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
            transitionedStates[conditionalSuccessor].Add(block,
              new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
          }
        }
        // branching and condition changes state
        else
        {
          IOperation resultSymbol; // get result symbol from block.BranchValue
          ResultState fallThroughState; // get fall-through state
          ResultState conditionalState; // get fall-through state
          if (block.FallThroughSuccessor is { Source: { } fallThroughSuccessor })
          {
            if (!transitionedStates.ContainsKey(fallThroughSuccessor))
              transitionedStates.Add(fallThroughSuccessor,
                new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
            transitionedStates[fallThroughSuccessor].Add(block,
              new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
          }

          if (block.ConditionalSuccessor is { Source: { } conditionalSuccessor })
          {
            if (!transitionedStates.ContainsKey(conditionalSuccessor))
              transitionedStates.Add(conditionalSuccessor,
                new Dictionary<BasicBlock, Dictionary<ISymbol, ResultState>>());
            transitionedStates[conditionalSuccessor].Add(block,
              new Dictionary<ISymbol, ResultState>(initialState, SymbolEqualityComparer.Default));
          }
        }
      }
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

  private class SymbolResultStatePairEqualityComparer : EqualityComparer<KeyValuePair<ISymbol, ResultState>>
  {
    public new static SymbolResultStatePairEqualityComparer Default { get; } = new();

    public override bool Equals(KeyValuePair<ISymbol, ResultState> x, KeyValuePair<ISymbol, ResultState> y) =>
      x.Key.Equals(y.Key, SymbolEqualityComparer.Default) && x.Value.Equals(y.Value);

    public override int GetHashCode(KeyValuePair<ISymbol, ResultState> obj) =>
      new { symbolHash = SymbolEqualityComparer.Default.GetHashCode(obj.Key), obj.Value }.GetHashCode();
  }

  private void CheckOperation(OperationBlockAnalysisContext context)
  {
    var state = new Dictionary<BasicBlock, Dictionary<string, bool?>>();

    for (var i = 0; i < context.OperationBlocks.Length; i++)
    {
      var operationBlock = context.OperationBlocks[i];
      var graph = context.GetControlFlowGraph(operationBlock);

      foreach (var basicBlock in graph.Blocks.Where(basicBlock =>
                 basicBlock.ConditionKind is not ControlFlowConditionKind.None))
      {
        if (!basicBlock.BranchValue.IsResultTypeInvocation(out var invocation))
          continue;
        if (invocation.Instance is IFieldReferenceOperation { Field: { } field })
        {
          context.ReportDiagnostic(Diagnostic.Create(GetDiagnostic(
              $"{basicBlock.BranchValue?.Syntax}: field {field.Name}"),
            basicBlock.BranchValue?.Syntax.GetLocation()));
        }

        if (invocation.Instance is ILocalReferenceOperation { Local: { } local })
        {
          context.ReportDiagnostic(Diagnostic.Create(GetDiagnostic(
              $"{basicBlock.BranchValue?.Syntax}: local variable {local.Name}"),
            basicBlock.BranchValue?.Syntax.GetLocation()));
        }
      }

      return;

      var blocks = new Stack<BasicBlock>(graph.Blocks);
      // blocks.Push(graph.Blocks.First());

      while (blocks.Count > 0)
      {
        var basicBlock = blocks.Pop();

        context.ReportDiagnostic(Diagnostic.Create(GetDiagnostic(
            $"block predecessors: {basicBlock.Predecessors.Length}"),
          basicBlock.Operations.FirstOrDefault()?.Syntax.GetLocation()));
        continue;
        if (basicBlock.ConditionKind is ControlFlowConditionKind.None)
          continue;
        context.ReportDiagnostic(Diagnostic.Create(GetDiagnostic(
            $"block {basicBlock.Ordinal}: {basicBlock.ConditionKind}; {basicBlock.BranchValue?.Syntax}"),
          basicBlock.BranchValue?.Syntax.GetLocation()));
        if (basicBlock.ConditionalSuccessor is { Destination: { } successor })
          blocks.Push(successor);
        if (basicBlock.FallThroughSuccessor is { Destination: { } fallthrough })
          blocks.Push(fallthrough);
      }
    }
  }

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    ImmutableArray.Create(Rule);


  private void MarkImplicitConversion(OperationAnalysisContext context)
  {
    var conversion = (IConversionOperation)context.Operation;
    var operand = conversion.Operand.Type as INamedTypeSymbol;

    if (!operand.IsResultType())
      return;

    if (operand.TypeParameters.Length != 2 ||
        operand.TypeParameters[0].Name != "TValue" ||
        !operand.TypeArguments[0].Equals(conversion.Type, SymbolEqualityComparer.Default))
      return;

    context.ReportDiagnostic(Diagnostic.Create(
      DiagnosticsHelper.Found(ResultSymbol.ImplicitConversion),
      conversion.Syntax.GetLocation()));
  }

  private void MarkInvocations(OperationAnalysisContext context)
  {
    if (!context.IsResultTypeInvocation(out var invocation))
      return;

    ResultSymbol? symbol = invocation.TargetMethod.Name switch
    {
      "OrThrow" => ResultSymbol.OrThrow,
      "OrDefault" => ResultSymbol.OrDefault,
      "Or" => ResultSymbol.Or,
      "IsError" when invocation.TargetMethod.Parameters.Length == 1 => ResultSymbol.IsErrorOutValue,
      "IsError" => ResultSymbol.IsError,
      _ => null,
    };

    if (symbol is not null)
    {
      context.ReportDiagnostic(Diagnostic.Create(
        DiagnosticsHelper.Found(symbol.Value),
        invocation.Syntax.GetLocation()));
    }
  }

  private void MarkProperties(OperationAnalysisContext context)
  {
    if (!context.IsResultTypePropertyReference(out var invocation))
      return;

    if (invocation.Property.Name != "OrDefault")
      return;

    context.ReportDiagnostic(Diagnostic.Create(
      DiagnosticsHelper.Found(ResultSymbol.OrDefault),
      invocation.Syntax.GetLocation()));
  }
}

internal class ResultAnalysisData { }