using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace ResultTypes.CodeAnalysis;

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

      // combine the output state of all predecessors
      var inputState = ResultAnalysisState.Combine(
        block.Predecessors
          .Select(branch => branch.Source)
          .Where(predecessor => outputState.ContainsKey(predecessor))
          .Select(predecessor => outputState[predecessor])
      );

      // if input state hasn't changed, we can stop analysis for this branch
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