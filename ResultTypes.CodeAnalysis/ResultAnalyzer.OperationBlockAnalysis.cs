using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace ResultTypes.CodeAnalysis;

internal class ResultAnalysisState
{
  private readonly Dictionary<ISymbol, ResultTypeInstanceState> stateDictionary = new(SymbolEqualityComparer.Default);

  private ResultAnalysisState() { }

  private ResultAnalysisState(IDictionary<ISymbol, ResultTypeInstanceState> data)
  {
    stateDictionary = new Dictionary<ISymbol, ResultTypeInstanceState>(data, SymbolEqualityComparer.Default);
  }

  internal ResultTypeInstanceState this[ISymbol symbol]
  {
    get =>
      stateDictionary.TryGetValue(symbol, out var value)
        ? value
        : ResultTypeInstanceState.Unchanged;
    set
    {
      if (value is ResultTypeInstanceState.Unchanged)
        stateDictionary.Remove(symbol);
      else
        stateDictionary[symbol] = value;
    }
  }

  internal bool IsEquivalentTo(ResultAnalysisState other) =>
    stateDictionary.OrderBy(entry => entry.Key).SequenceEqual(
      other.stateDictionary.OrderBy(entry => entry.Key));

  internal ResultAnalysisState Copy() => new(stateDictionary);

  internal ResultAnalysisState With(ISymbol symbol, ResultTypeInstanceState state)
  {
    var result = Copy();
    result[symbol] = state;
    return result;
  }

  internal ResultAnalysisState Combine(ResultAnalysisState other) => Combine(this, other);

  internal static ResultAnalysisState Combine(params ResultAnalysisState[] states) => Combine(states.AsEnumerable());
  
  internal static ResultAnalysisState Combine(IEnumerable<ResultAnalysisState> states) =>
    states.ToList() switch
    {
      [] => new ResultAnalysisState(),
      [var onlyState] => onlyState.Copy(),
      [var firstState, ..] => new ResultAnalysisState(states
        .Skip(1)
        .Aggregate(
          new HashSet<KeyValuePair<ISymbol, ResultTypeInstanceState>>(firstState.stateDictionary),
          (intersection, state) =>
          {
            intersection.IntersectWith(state.stateDictionary);
            return intersection;
          })
        .ToDictionary(pair => pair.Key, pair => pair.Value, SymbolEqualityComparer.Default)),
    };
}

internal sealed partial class ResultAnalyzer
{
  /// <summary>
  /// Analyze all operation blocks in the given context.
  /// </summary>
  /// <param name="context">The context whose operation blocks should be analyzed.</param>
  private void AnalyzeOperationBlocks(OperationBlockAnalysisContext context)
  {
    foreach (var operationBlock in context.OperationBlocks)
      AnalyzeOperationBlock(operationBlock, context);
  }

  private void AnalyzeOperationBlock(IOperation operationBlock, OperationBlockAnalysisContext context)
  {
    var graph = context.GetControlFlowGraph(operationBlock);

    var blocksToBeAnalyzed = new Queue<BasicBlock>(graph.Blocks.Where(block => block.Predecessors.Length is 0));

    var outputState = new Dictionary<BasicBlock, ResultAnalysisState>();
    var lastInputState = new Dictionary<BasicBlock, ResultAnalysisState>();

    while (blocksToBeAnalyzed.Count > 0)
    {
      var block = blocksToBeAnalyzed.Dequeue();

      var inputState = ResultAnalysisState.Combine(
        block.Predecessors
          .Select(branch => branch.Source)
          .Where(predecessor => outputState.ContainsKey(predecessor))
          .Select(predecessor => outputState[predecessor])
      );

      if (lastInputState.TryGetValue(block, out var lastState) && lastState.IsEquivalentTo(inputState))
        continue;

      lastInputState[block] = inputState.Copy();

      foreach (var operation in block.Operations)
        AnalyzeNonBranchingOperation(operation, inputState, context);
      
      if (block is { FallThroughSuccessor.Destination: { } fallthroughSuccessor })
        blocksToBeAnalyzed.Enqueue(fallthroughSuccessor);
      if (block is { ConditionalSuccessor.Destination: { } conditionalSuccessor })
        blocksToBeAnalyzed.Enqueue(conditionalSuccessor);
      
      // TODO: save output state according to branching
    }
  }
}